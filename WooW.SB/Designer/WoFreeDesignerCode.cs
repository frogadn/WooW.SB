using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using WooW.Core;
using WooW.SB.CodeEditor;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer
{
    public partial class WoFreeDesigner : UserControl
    {
        #region Variables principales del proyecto generado

        /// <summary>
        /// Nombre del proyecto de blazor de unitarias.
        /// (Se usara el mismo proyecto para todas las unitarias)
        /// </summary>
        private string _projectName = "BlazorUnitTest";

        /// <summary>
        /// Es el nombre de los ficheros que funge como nombre de modelo genérico para las
        /// clases y los archivos del generador puesto que no se pueden cargar archivos en el watch
        /// </summary>
        private string _className = "";

        /// <summary>
        /// Define el nombre de la clase de la que se creara el formulario.
        /// </summary>
        private string _modelName = "";

        #endregion Variables principales del proyecto generado

        #region Árbol de métodos

        /// <summary>
        /// Instancia del data table del selector de métodos.
        /// </summary>
        private DataTable _dtMethodsTree;

        /// <summary>
        /// Instancia del data table del selector de métodos.
        /// </summary>
        private void InitializeMethodsTree()
        {
            _dtMethodsTree = new DataTable();

            _dtMethodsTree.Columns.Add($@"Controles", typeof(string));
            _dtMethodsTree.Columns.Add($@"Tipo", typeof(string));
            _dtMethodsTree.Columns.Add($@"Eventos", typeof(string));
            _dtMethodsTree.Columns.Add($@"Código", typeof(bool));
            _dtMethodsTree.Columns.Add($@"Activo", typeof(bool));

            grdMethod.DataSource = _dtMethodsTree;

            grdViewMethods.Columns[@"Controles"].Group();

            grdViewMethods.Columns[@"Tipo"].Resize(10);
            grdViewMethods.Columns[@"Código"].Resize(5);
            grdViewMethods.Columns[@"Activo"].Resize(5);

            grdViewMethods.OptionsView.ShowGroupPanel = false;

            ChargeControlMethods(
                control: "Formulario",
                events: new List<(string eventType, string eventName, bool code, bool used)>()
                {
                    ("Constructor", $@"Validaciones Fluent", false, false),
                    ("void", $@"Formulario Iniciado", false, false)
                }
            );
        }

        /// <summary>
        /// Carga al Data Table el control y sus respectivos métodos
        /// </summary>
        private void ChargeControlMethods(
            string control,
            List<(string eventType, string eventName, bool code, bool used)> events
        )
        {
            foreach ((string eventType, string eventName, bool code, bool used) evt in events)
            {
                DataRow drForm = _dtMethodsTree.NewRow();
                drForm[@"Controles"] = control;
                drForm[@"Tipo"] = evt.eventType;
                drForm[@"Eventos"] = evt.eventName;
                drForm[@"Código"] = evt.code;
                drForm[@"Activo"] = evt.used;

                _dtMethodsTree.Rows.Add(drForm);
            }
        }

        #endregion Árbol de métodos


        #region Initialize syntax editor

        /// <summary>
        /// Instancia principal del syntax editor.
        /// </summary>
        private WoSyntaxEditor _woSyntaxEditor = null;

        /// <summary>
        /// Inicializa el syntax editor para el modelo seleccionado.
        /// Verifica si ya hay una instancia y la reemplaza.
        /// </summary>
        private void InitializeWoSyntaxEditor()
        {
            string pathProject =
                $@"{_project.DirProyectData}/LayOuts/UserCodeFreeStyle/{_layoutName}_proj";
            if (File.Exists($@"{pathProject}/{_layoutName}ScriptsUser.cs"))
            {
                if (!_woSyntaxEditor.IsNull())
                {
                    _woSyntaxEditor.Dispose();
                    pnlSyntaxEditor.Controls.Clear();
                    tabCode.PageVisible = false;
                }

                tabCode.PageVisible = true;
                _woSyntaxEditor = new WoSyntaxEditor(
                    pathDirSaveProject: $@"{_project.DirProyectData}\LayOuts\UserCodeFreeStyle\{_layoutName}_proj",
                    modelName: _modelName,
                    classModelName: _className,
                    methodParamsCol: null,
                    isFreeStyle: true
                );

                _woSyntaxEditor.Parent = pnlSyntaxEditor;
                _woSyntaxEditor.Dock = DockStyle.Fill;
            }
            else
            {
                ///todo: Send trow: base files not ready
            }
        }

        #endregion Initialize syntax editor


        #region Creación de los ficheros de código base del diseño

        /// <summary>
        /// Crea los ficheros base del diseño.
        /// </summary>
        /// <param name="woContainer"></param>
        private void BuildBaseClass(string newLayoutName)
        {
            WoContainer container = new WoContainer()
            {
                ModelId = newLayoutName,
                CustomDesignAplied = false,
                ThemeSuperiorAplied = false,
                Theme = "Default",
                Id = "Root",
                Etiqueta = "Root",
                MaskText = "",
                TypeContainer = eTypeContainer.FormRoot,
                Enable = eItemEnabled.Activo,
                ModelType = Core.WoTypeModel.TransactionFreeStyle,
                Proceso = string.Empty,
                BackgorundColorContainerItem = eContainerItemColor.Default,
                BackgorundColorGroup = eGroupColor.Default,
                ComponentFontColor = eTextColor.FontDefault,
                ComponentFontSize = eTextSize.Normal,
                ComponentFontItalic = eTextItalic.None,
                ComponentFontWide = eTextWeight.Normal,
                ComponentFontDecoration = eTextDecoration.None,
                Parent = string.Empty,
                BeginRow = false,
                ColSpan = 1,
                RowSpan = 1,
                ColumnIndex = 0,
                RowIndex = 0,
                Col = 12,
                Row = 0,
                Icon = eBoostrapIcons.None
            };

            //WoBlazorSave woBlazorSave = new WoBlazorSave(
            //    modelName: container.ModelId,
            //    className: newLayoutName,
            //    isSlave: false,
            //    woContainer: container,
            //    isFreeLayout: true
            //);
            //woBlazorSave.BuildBaseSave();
        }

        #endregion Creación de los ficheros de código base del diseño
    }
}
