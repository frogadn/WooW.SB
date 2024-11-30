using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.CodeEditor.CodeComponents;
using WooW.SB.CodeEditor.SyntaxEditor.SyntaxEditorSettings;
using WooW.SB.Config;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.CodeEditor.CodeDialogs
{
    public partial class fmUsingSelector : DevExpress.XtraEditors.XtraForm
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia principal del proyecto con las configuraciones y opciones generales.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        /// <summary>
        /// Observador de logs.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Path del fichero con los usings del código.
        /// </summary>
        private string _usingsPath = string.Empty;

        /// <summary>
        /// Nombre del modelo sobre el que se trabajara.
        /// </summary>
        private string _modelName = string.Empty;

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor principal, recive el nombre del modelo sobre el que se trabajara.
        /// </summary>
        /// <param name="modelName"></param>
        [SupportedOSPlatform("windows")]
        public fmUsingSelector(string modelName)
        {
            InitializeComponent();

            _usingsCol = new List<(string dllName, string usingName)>();
            _selectedUsingsCol = new List<string>();

            _modelName = modelName;

            _usingsPath = $@"{_project.DirLayOuts}\UserCode\{modelName}_proj\{modelName}Usings.cs";

            ReadUsingsFile();

            ChargeGrid();

            ChargeSyntax();
        }

        #endregion Constructor


        #region Carga del syntax

        /// <summary>
        /// Definición de las reglas de lenguaje
        /// (objeto de active pro)
        /// </summary>
        [SupportedOSPlatform("windows")]
        private CSharpSyntaxLanguage _language = new CSharpSyntaxLanguage();

        /// <summary>
        /// Editor de codigo donde se mostraran los usings.
        /// </summary>
        private WoHeaderEditor _headerEditor;

        /// <summary>
        /// Carga el syntax editor
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeSyntax()
        {
            pnlCode.Controls.Clear();

            _headerEditor = new WoHeaderEditor(_language);
            _headerEditor.Parent = pnlCode;
            _headerEditor.Dock = DockStyle.Fill;
            string usingsText = WoDirectory
                .ReadFile(_usingsPath)
                .Replace("\n", "")
                .Replace("\r", "");
            usingsText = usingsText.Replace(";", ";\n");
            _headerEditor.SetCode(usingsText);

            string serverScripts =
                $@"{_project.DirProyectTemp}\ServerUnitModel_proj\UserCode\ServerUnitModelScriptsUser.cs";

            if (File.Exists(serverScripts))
            {
                UpdateUsings(serverScripts, projectName: "ServerUnitModel_proj");
            }

            string wasmScripts =
                $@"{_project.DirProyectTemp}\WasmUnitModel_proj\UserCode\WasmUnitModelScriptsUser.cs";

            if (File.Exists(wasmScripts))
            {
                UpdateUsings(wasmScripts, projectName: "WasmUnitModel_proj");
            }
        }

        /// <summary>
        /// Actualiza los usigns del fichero en caso de que existan.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void UpdateUsings(string scriptsPath, string projectName)
        {
            if (File.Exists(scriptsPath))
            {
                string scriptsText = WoDirectory.ReadFile(scriptsPath);
                string header = scriptsText.Split("using").First();
                string code = scriptsText.Split("namespace").Last();

                string usingsText = WoDirectory
                    .ReadFile(_usingsPath)
                    .Replace("using System.Private.CoreLib;", "");

                WoDirectory.WriteFile(
                    scriptsPath,
                    data: @$"{header} 
{usingsText} 
using {projectName}.ControlModels;

namespace {code}"
                );
            }
        }

        #endregion Carga del syntax


        #region Fichero de usings

        /// <summary>
        /// Lista de los usings seleccionados.
        /// </summary>
        List<string> _selectedUsingsCol = new List<string>();

        /// <summary>
        /// Lee el fichero con los usings del codigo, en caso de no existir lo crea.
        /// </summary>
        private void ReadUsingsFile()
        {
            CreateRawFile();

            string usingsFile = WoDirectory.ReadFile(_usingsPath);

            string[] usingsNameCol = usingsFile.Split("using");

            foreach (string usingName in usingsNameCol)
            {
                _selectedUsingsCol.Add(usingName.Split(';').First().Replace(" ", ""));
            }
        }

        #endregion Fichero de usings


        #region Carga de la grid

        /// <summary>
        /// Tabla con los usings bindeado a la grid.
        /// </summary>
        private DataTable _tbUsingsCol;

        /// <summary>
        /// Carga la grid con los usings del fichero.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeGrid()
        {
            ChargeAllUsigns();

            _tbUsingsCol = new DataTable();

            _tbUsingsCol.Columns.Add("DLL", typeof(string));
            _tbUsingsCol.Columns.Add("Using", typeof(string));
            _tbUsingsCol.Columns.Add("Cargar", typeof(bool));

            foreach ((string dllName, string usingName) usingItem in _usingsCol)
            {
                DataRow newRow = _tbUsingsCol.NewRow();
                newRow["DLL"] = usingItem.dllName;
                newRow["Using"] = usingItem.usingName;
                newRow["Cargar"] = _selectedUsingsCol.Contains(usingItem.usingName);

                _tbUsingsCol.Rows.Add(newRow);
            }

            grdUsings.DataSource = _tbUsingsCol;

            grdUsingsView.Columns["DLL"].OptionsColumn.AllowEdit = false;
            grdUsingsView.Columns["Using"].OptionsColumn.AllowEdit = false;

            grdUsingsView.ClearSorting();
            grdUsingsView.Columns[@"DLL"].SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
        }

        #endregion Carga de la grid

        #region Carga de las dlls

        /// <summary>
        /// Colección de usings completa
        /// </summary>
        List<(string dllName, string usingName)> _usingsCol =
            new List<(string dllName, string usingName)>();

        /// <summary>
        /// Recupera todos los usings de los assemblies.
        /// </summary>
        private void ChargeAllUsigns()
        {
            string assembliesPath = $@"{_project.DirProyectData}\Assembly";
            List<string> assembliesCol = WoDirectory.ReadDirectoryFiles(assembliesPath);

            AddBaseDlls();

            foreach (string asembly in assembliesCol)
            {
                if (asembly.Split('.').Last() == "dll" && (!asembly.Contains("WooW.WebClient.dll")))
                {
                    try
                    {
                        Assembly dll = Assembly.LoadFile(asembly);
                        Type[] typesCol = dll.GetTypes();

                        foreach (Type type in typesCol)
                        {
                            string usingName = type.Namespace;
                            if (usingName != null)
                            {
                                var findUsing = _usingsCol.FirstOrDefault(u =>
                                    u.usingName == usingName
                                );
                                if (findUsing.usingName == null)
                                {
                                    string fileAsemblyName = asembly.Split('\\').Last();
                                    string asemblyName = fileAsemblyName.Split('.').First();
                                    _usingsCol.Add((asemblyName, usingName));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ///dfsa
                    }
                }
            }
        }

        #endregion Carga de las dlls


        #region DLLs base

        /// <summary>
        /// Agrega las dlls base al proyecto.
        /// </summary>
        private void AddBaseDlls()
        {
            /// Usings del system

            _usingsCol.Add(("System", "System"));
            _usingsCol.Add(("System", "System.Collections.Generic"));
            _usingsCol.Add(("System", "System.Linq"));
            _usingsCol.Add(("System", "System.IO"));
            _usingsCol.Add(("System", "System.Text"));
            _usingsCol.Add(("System", "System.Text.RegularExpressions"));
            _usingsCol.Add(("System", "System.Reflection"));
            _usingsCol.Add(("System", "System.Private.CoreLib"));
            _usingsCol.Add(("System", "System.Threading.Tasks"));

            /// Usigns de UI

            _usingsCol.Add(("WooW.Blazor", "WooW.Blazor.Resources"));

            _usingsCol.Add(("WooW", "WooW.DTO"));

            _usingsCol.Add(("Microsoft", "Microsoft.JSInterop"));
        }

        #endregion DLLs base

        #region DLLs base de cualquier modelo

        /// <summary>
        /// en caso de no existir el fichero de usings lo crea con los usings base para el modelo.
        /// </summary>
        private void CreateRawFile()
        {
            if (!File.Exists(_usingsPath))
            {
                WoDirectory.WriteFile(
                    _usingsPath,
                    data: WoSyntaxEditorHeaderAndFooter.GetMainUsings()
                );
            }
        }

        #endregion DLLs base de cualquier modelo


        #region Actualizar usings

        /// <summary>
        /// actualiza el fichero de usings con los usings seleccionados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void grdUsingsView_CellValueChanging(
            object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e
        )
        {
            if (e != null && e.RowHandle >= 0)
            {
                DataRowView selectedRow = (DataRowView)grdUsingsView.GetRow(e.RowHandle);
                string usingChanged = selectedRow.Row.ItemArray[1].ToString();

                if ((bool)e.Value)
                {
                    WoDirectory.AddText(_usingsPath, $"using {usingChanged};\n\r");
                }
                else
                {
                    string usingsFile = WoDirectory.ReadFile(_usingsPath);
                    string newVersionUsings = usingsFile.Replace($@"using {usingChanged};", "");
                    WoDirectory.WriteFile(_usingsPath, data: newVersionUsings);
                }
            }

            ChargeSyntax();
        }

        #endregion Actualizar usings

        private void fmUsingSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
