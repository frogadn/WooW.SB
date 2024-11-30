using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using ActiproSoftware.Text.Languages.DotNet.Reflection;
using ActiproSoftware.UI.WinForms.Controls.SyntaxEditor.EditActions;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace WooW.SB.Config.Editors
{
    public partial class woSyntaxEditor : UserControl
    {

        #region Atributos

        /// <summary>
        /// Permite la carga de dlls al SyntaxEditor.
        /// Se inicializa en el constructor y se puede obtener desde la definición del lenguaje de active pro.
        /// </summary>
        private IProjectAssembly _assemblyProject;

        /// <summary>
        /// Corre en segundo plano para ir revisando el código que se va cargando en el syntax y
        /// permitir cosas como la detección de errores de sintaxis.
        /// </summary>
        private BackgroundWorker _worker = new BackgroundWorker();

        /// <summary>
        /// Lista de las dll que se cargaran como contexto al syntax
        /// </summary>
        private List<string> _dllsCol = new List<string>();

        /// <summary>
        /// Definición de las reglas de lenguaje
        /// (objeto de active pro)
        /// </summary>
        [SupportedOSPlatform("windows")]
        private ISyntaxLanguage _language;

        #endregion Atributos


        #region Constructor principal

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        [SupportedOSPlatform("windows")]
        public woSyntaxEditor(ISyntaxLanguage syntaxLanguage, List<string> dlls = null)
        {
            InitializeComponent();

            try
            {
                // Inicialización de la lista de dlls para el contexto
                _dllsCol = (dlls != null) ? dlls : new List<string>();

                // Definición del lenguaje
                syeCode.Document.Language = syntaxLanguage;
                _language = syntaxLanguage;

                // configuración del contexto y del worker
                _assemblyProject = _language.GetService<IProjectAssembly>();
                _worker.WorkerSupportsCancellation = true;

                // Si esta configurado el lenguaje como C# cargamos dlls
                if (syntaxLanguage.GetType() == typeof(CSharpSyntaxLanguage))
                {
                    ValidateDotNet();

                    _worker.DoWork += ChargeDlls;
                }

                // Iniciamos el worker
                _worker.RunWorkerAsync();

                // Suscripción de eventos
                syeCode.Document.TextChanged += CodeChanged;

                // Inicialización de botones
                btnOpenFileVisualCode.Enabled = false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al inicializar el editor de codigo. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Constructor principal

        #region Ribbon 

        /// <summary>
        /// Retornamos el ribbon para hacer un merge con el ribbon principal
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public RibbonControl GetRibbon()
        {
            try
            {
                ribbonControl1.Hide();
                return ribbonControl1;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al intentar recuperar el ribbon. {ex.Message}");
            }
        }

        #endregion Ribbon


        #region Modificación externa en el codigo

        /// <summary>
        /// Action que se detonara al modificar el código del syntax.
        /// no recibe parámetros ni retorna nada.
        /// </summary>
        public Action CodeChangedEvt;

        /// <summary>
        /// Método suscrito al evento de cambio de texto del syntax.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void CodeChanged(object sender, TextSnapshotChangedEventArgs e)
        {
            try
            {
                if (e.OldSnapshot.Text != e.NewSnapshot.Text && e.OldSnapshot.Text != string.Empty)
                {
                    CodeChangedEvt?.Invoke();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al indicar la modificación en el editor de codigo {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }

        }

        #endregion Modificación externa en el codigo


        #region Asignar Header y Footer

        /// <summary>
        /// Define el header y el footer del syntax.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void AssignHeaderAndFooter(string headerCode, string footerCode)
        {
            try
            {
                syeCode.Document.SetHeaderAndFooterText(headerCode, footerCode);
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la asignación del codigo. {ex.Message}");
            }
        }

        #endregion Asignar Header y Footer

        #region Ribbon code functions

        /// <summary>
        /// Comenta el código seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnComent_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                syeCode.ActiveView.TextChangeActions.CommentLines();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al comentar el código seleccionado. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Des comenta el codigo seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnUncomment_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                syeCode.ActiveView.TextChangeActions.UncommentLines();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al des comentar el código seleccionado. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Formatea el codigo seleccionado o todo el documento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnFormatCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string selectedText = syeCode.ActiveView.SelectedText;

                if (selectedText != string.Empty)
                {
                    syeCode.ActiveView.TextChangeActions.FormatSelection();
                }
                else
                {
                    syeCode.ActiveView.TextChangeActions.FormatDocument();
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al comentar el código seleccionado. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Aumenta la identación del código seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnIncreaseIdent_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                syeCode.ActiveView.TextChangeActions.Indent();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al aumentar la sangria en el código seleccionado. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }

        }

        /// <summary>
        /// Reduce la identación del código seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDecreaseIdent_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                syeCode.ActiveView.TextChangeActions.Outdent();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al reducir la sangria en el código seleccionado. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Expande todas las regiones del syntax editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnExpandRegions_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                syeCode.ActiveView.ExecuteEditAction(new MoveUpAction());
                syeCode.ActiveView.ExecuteEditAction(new ExpandAllOutliningAction());
                syeCode.ActiveView.Scroller.ScrollLineToVisibleMiddle();
                syeCode.ActiveView.ExecuteEditAction(new MoveDownAction());
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al expandir las regiones. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Contrae todas las regiones del syntax editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCollapseRegions_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                syeCode.ActiveView.ExecuteEditAction(new MoveUpAction());
                syeCode.ActiveView.ExecuteEditAction(new ApplyDefaultOutliningExpansionAction());
                syeCode.ActiveView.Scroller.ScrollLineToVisibleMiddle();
                syeCode.ActiveView.ExecuteEditAction(new MoveDownAction());
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al contraer las regiones. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Ribbon code functions

        #region Gestión del código

        /// <summary>
        /// Remplaza el código actual del syntax por el que se recibe por parámetro.
        /// </summary>
        /// <param name="code"></param>
        [SupportedOSPlatform("windows")]
        public void SetCode(string code)
        {
            try
            {
                syeCode.Text = string.Empty;
                syeCode.Text = code;

                syeCode.Refresh();

                CodeChangedEvt?.Invoke();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al asignar codigo al syntax. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }

        }

        /// <summary>
        /// Retorna el código actual del syntax.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public string GetCode()
        {
            try
            {
                return syeCode.Text;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al retornar el codigo.");
            }
        }

        /// <summary>
        /// Limpia el editor de código.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CleanEditor()
        {
            try
            {
                syeCode.Text = string.Empty;

                CodeChangedEvt?.Invoke();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al limpiar el editor de codigo. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Gestión del código

        #region Snippets

        /// <summary>
        /// Permite enviar código del snippet desde el método a través del parámetro
        /// </summary>
        /// <param name="snippet"></param>
        [SupportedOSPlatform("windows")]
        public void SetSnippet(string snippet)
        {
            try
            {
                if (snippet != null && snippet != string.Empty)
                {
                    string selectedText = syeCode.ActiveView.SelectedText;

                    if (selectedText == string.Empty)
                    {
                        int characterPosition = syeCode.ActiveView.CurrentViewLine.EndOffset;

                        int lineIndex = syeCode.ActiveView.CurrentViewLine.EndPosition.Line;

                        syeCode.Text = syeCode.Text.Insert(
                            characterPosition + lineIndex,
                            $"{snippet}\n"
                        );
                    }
                    else
                    {
                        string line = syeCode.ActiveView.CurrentViewLine.Text.ReplaceFirst(
                            selectedText,
                            snippet
                        );

                        syeCode.ActiveView.CurrentViewLine.Text = line;
                    }
                }
                else
                {
                    throw new Exception("El snippet se encuentra no contiene nada");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al enviar el spinet al editor. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        /// <summary>
        /// Permite enviar como snippet codigo a través del porta papeles
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void SetSnippetFormClipboard()
        {
            try
            {
                syeCode.ActiveView.PasteFromClipboard();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al enviar el spinet al editor. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Snippets


        #region Remplazar fragmentos de código

        /// <summary>
        /// Remplaza un fragmento de código por otro
        /// </summary>
        /// <exception cref="Exception"></exception>
        [SupportedOSPlatform("windows")]
        public void ReplaceCode(string oldCode, string newCode)
        {
            try
            {
                string code = syeCode.Text.Replace(oldCode, newCode);
                syeCode.Text = code;
            }

            catch (Exception ex)
            {
                throw new Exception($@"Error al remplazar el fragmento de codigo. {ex.Message}");
            }
        }

        #endregion Remplazar fragmentos de código


        #region Comprobaciones de .Net

        /// <summary>
        /// Ruta base para recuperar dlls de .net para el syntax
        /// </summary>
        private string _dotNetDirectoryPath = string.Empty;

        /// <summary>
        /// Validamos que se encuentre instalada la versión de Dot Net correcta para compilar codigo
        /// </summary>
        private void ValidateDotNet()
        {
            try
            {
                string pathDotNet = "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App";

                List<string> dotnetVer = Directory.GetDirectories(pathDotNet).ToList();
                if (dotnetVer == null || dotnetVer.Count == 0)
                {
                    throw new Exception("Instale alguna version de .net 8 en adelante para continuar.");
                }

                foreach (string dllDirectory in dotnetVer)
                {
                    string[] dllDirectoryCol = dllDirectory.Split("\\");
                    string dllDirectoryLast = dllDirectoryCol.Last();
                    string ver = dllDirectoryLast.Split(".").First();
                    if (ver == "8")
                    {
                        _dotNetDirectoryPath = dllDirectory;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Comprobaciones de .Net

        #region Carga de las dll al syntax

        /// <summary>
        /// Método suscrito al controlador de eventos del worker para que se carguen las dlls al momento
        /// que se inicia el hilo del worker para validar el código.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeDlls(object sender, EventArgs e)
        {
            try
            {
                _assemblyProject.AssemblyReferences.AddMsCorLib();

                /// System
                foreach (var dll in _dllsCol)
                {
                    if (File.Exists(dll))
                    {
                        _assemblyProject.AssemblyReferences.AddFrom(dll);
                    }
                    else
                    {
                        XtraMessageBox.Show(
                            text: $@"La dll: {dll} no existe.",
                            caption: "Alerta",
                            icon: MessageBoxIcon.Warning,
                            buttons: MessageBoxButtons.OK
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al cargar las dlls al syntax editor. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Carga de las dll al syntax


        #region Compilación del codigo

        /// <summary>
        /// árbol de sintaxis del codigo a compilar
        /// </summary>
        private SyntaxTree _syntaxTree = null;

        /// <summary>
        /// Action que se detona cuando se realiza una compilación del codigo en el syntax.
        /// </summary>
        public Action<List<ListViewItem>> UpdateErrorListEvt;

        /// <summary>
        /// Copila el codigo presente en el syntax y envía la salida por un action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnCompile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _syntaxTree = CSharpSyntaxTree.ParseText(syeCode.Text);

                List<ListViewItem> output = new List<ListViewItem>();

                List<MetadataReference> references = new List<MetadataReference>();

                foreach (var dll in _dllsCol)
                {
                    if (File.Exists(dll))
                    {
                        references.Add(MetadataReference.CreateFromFile(dll));
                    }
                    else
                    {
                        XtraMessageBox.Show(
                            "No se encontró la dll: " + dll,
                            "Alerta",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }

                CSharpCompilation compilation = CSharpCompilation
                    .Create("Prog.dll")
                    .AddReferences(references)
                    .AddSyntaxTrees(_syntaxTree);

                Stream resultStream = new MemoryStream();
                var result = compilation.Emit(resultStream);

                if (result.Success)
                {
                    XtraMessageBox.Show(
                        text: $@"Compilación realizada correctamente",
                        caption: "Información",
                        icon: MessageBoxIcon.Information,
                        buttons: MessageBoxButtons.OK
                    );
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        int errorLine = 0;
                        int errorColumn = 0;

                        if (diagnostic.ToString().IndexOf(":") > 0)
                        {
                            var parts = diagnostic.ToString()
                                .Substring(0, diagnostic.ToString().IndexOf(":"))
                                .Replace("(", "")
                                .Replace(")", "")
                                .Split(',');
                            if (parts.Length > 1)
                            {
                                int.TryParse(parts[0], out errorLine);
                                int.TryParse(parts[1], out errorColumn);
                            }
                        }

                        ListViewItem outputItem = new ListViewItem(
                                new string[]
                                {
                                    diagnostic.Id,
                                    diagnostic.GetMessage(),
                                    errorLine.ToString(),
                                    errorColumn.ToString()
                                }
                            );

                        output.Add(outputItem);
                    }
                }

                resultStream.Dispose();

                UpdateErrorListEvt?.Invoke(output);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error en la compilación. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Compilación del codigo


        #region Asignar y recuperar directorio

        /// <summary>
        /// Ruta completa del fichero sobre del que estamos trabajando
        /// </summary>
        private string _filePath = string.Empty;

        /// <summary>
        /// Asignamos el path del directorio al componente para poder tener un contexto del componente
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void SetFilePath(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    _filePath = path;
                    btnOpenFileVisualCode.Enabled = true;
                }
                else
                {
                    throw new Exception($@"La ruta {path} del path no existe.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la asignación del path. {ex.Message}");
            }
        }

        /// <summary>
        /// Recuperamos el fichero sobre del que esta trabajando el syntax
        /// </summary>
        /// <param name="path"></param>
        [SupportedOSPlatform("windows")]
        public string GetFilePath()
        {
            try
            {
                if (_filePath != string.Empty && _filePath != null)
                {
                    return _filePath;
                }
                else
                {
                    throw new Exception($@"No se ah asignado ningún path.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en la asignación del path. {ex.Message}");
            }
        }

        #endregion Asignar y recuperar directorio

        #region Abrir fichero en visual code

        /// <summary>
        /// Abre el fichero 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnOpenFileVisualCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string visualPaht =
                    "C:\\Users\\Frog\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe";
                if (File.Exists(visualPaht))
                {
                    if (File.Exists(_filePath))
                    {
                        Process processVisualStudio = new Process();
                        processVisualStudio.StartInfo.FileName = visualPaht;
                        processVisualStudio.StartInfo.Arguments = _filePath;
                        processVisualStudio.StartInfo.ErrorDialog = true;
                        processVisualStudio.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        processVisualStudio.Start();
                        processVisualStudio.WaitForExit();
                    }
                    else
                    {
                        throw new Exception($@"No se encontró el fichero {_filePath}.");
                    }
                }
                else
                {
                    throw new Exception(
                        $@"No se encontró el ejecutable de visual code: {visualPaht}"
                    );
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al intentar abrir. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }
        }

        #endregion Abrir fichero en visual code


        #region Salvado del fichero

        /// <summary>
        /// Salvado del fichero en edición
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (_filePath != string.Empty)
                {
                    File.WriteAllText(_filePath, syeCode.Text);
                }
                else
                {
                    throw new Exception("Asigne el path del fichero en edición para guardarlo");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $@"Error al salvar el proyecto. {ex.Message}",
                    caption: "Error",
                    icon: MessageBoxIcon.Error,
                    buttons: MessageBoxButtons.OK
                );
            }

        }

        #endregion Salvado del fichero
    }
}
