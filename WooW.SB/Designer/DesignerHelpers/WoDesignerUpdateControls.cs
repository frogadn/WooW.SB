using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using DevExpress.XtraEditors;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Helpers;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoDesignerUpdateControls
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia inyectada como singleton del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Helpers y clases auxiliares

        /// <summary>
        /// Helper de búsqueda del modelo.
        /// </summary>
        private WoToolModelsHelper _modelHelper = new WoToolModelsHelper();

        #endregion Helpers y clases auxiliares


        #region Atributos

        /// <summary>
        /// Modelo seleccionado.
        /// </summary>
        private Modelo _modelSelected = null;

        /// <summary>
        /// Nueva instancia de WoContainer con la actualización de los tipos de input.
        /// </summary>
        private WoContainer _woContainer = new WoContainer();

        /// <summary>
        /// Instancia que maneja la clase con el código del usuario.
        /// </summary>
        private WoSyntaxManagerUserCode _woSyntaxManagerUserCode = null;

        /// <summary>
        /// Instancia que maneja la clase con las instancias de las vistas de los controles.
        /// </summary>
        private WoSyntaxManagerModelClass _woSyntaxManagerModelClass = null;

        /// <summary>
        /// Instancia del helper para el manejo de tipos de datos
        /// </summary>
        private WoDesignerTypeHelper _designerTypeHelper = null;

        /// <summary>
        /// Helper para el manejo de los proyectos y las extensiones
        /// </summary>
        private WoProjectDataHelper _woProjectDataHelper = new WoProjectDataHelper();

        /// <summary>
        /// Lista de todas las columnas del modelo, incluidas las columnas de las extensiones
        /// </summary>
        private List<ModeloColumna> _fullColumnsModel = new List<ModeloColumna>();

        #endregion Atributos


        #region Método principal

        /// <summary>
        /// Recibe una instancia principal de contaminen con el formulario
        /// </summary>
        private List<string> _controlsAddedCol = new List<string>();

        /// <summary>
        /// Recibe una instancia principal de contaminen con el formulario
        /// y verifica que todos los componentes de este coincidan con el tipo de componentes
        /// del modelo.
        /// </summary>
        /// <param name="woContainer"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public WoContainer UpdateControls(
            WoContainer woContainer,
            WoSyntaxManagerUserCode woSyntaxManagerUserCode,
            WoSyntaxManagerModelClass woSyntaxManagerModelClass
        )
        {
            _fullColumnsModel = _woProjectDataHelper.GetFullColumns(woContainer.ModelId);

            _controlsAddedCol = new List<string>();

            _woSyntaxManagerUserCode = woSyntaxManagerUserCode;
            _woSyntaxManagerModelClass = woSyntaxManagerModelClass;

            _modelSelected = _modelHelper.SearchModel(woContainer.ModelId);
            _designerTypeHelper = new WoDesignerTypeHelper(woContainer.ModelId);

            _woContainer = Update(woContainer);

            return _woContainer;
        }

        /// <summary>
        /// Recorre recursiva mente WoContainer y va construyendo la otra instancia
        /// de woContainer que se tomara como la nueva base.
        /// </summary>
        /// <param name="woContainerBase"></param>
        [SupportedOSPlatform("windows")]
        private WoContainer Update(WoContainer woContainerBase)
        {
            WoContainer woContainer = woContainerBase.GetInstance();

            if (woContainerBase.ContainersCol.Count > 0)
            {
                woContainer.ContainersCol.Clear();
                foreach (WoContainer container in woContainerBase.ContainersCol)
                {
                    woContainer.ContainersCol.Add(Update(container));
                }
            }

            if (woContainerBase.ItemsCol.Count > 0)
            {
                woContainer.ItemsCol.Clear();
                foreach (WoItem item in woContainerBase.ItemsCol)
                {
                    if (!_controlsAddedCol.Contains(item.Id))
                    {
                        _controlsAddedCol.Add(item.Id);

                        ModeloColumna modeloColumna = _fullColumnsModel
                            .Where(x => x.Id == item.BindedProperty)
                            .FirstOrDefault();

                        if (modeloColumna != null)
                        {
                            if (modeloColumna.TipoColumna == Core.WoTypeColumn.Reference)
                            {
                                //Modelo referenceModel = _project
                                //    .ModeloCol.Modelos.Where(x => x.Id == item.ClassModelType)
                                //    .FirstOrDefault();

                                Modelo referenceModel = _woProjectDataHelper.GetMainModel(
                                    item.ClassModelType
                                );

                                if (referenceModel != null)
                                {
                                    item.Control =
                                        (modeloColumna.TipoControl.ToString() == "CollectionEditor")
                                            ? "Slave"
                                            : modeloColumna.TipoControl.ToString();

                                    string type = modeloColumna.TipoColumna.ToString();
                                    type = _designerTypeHelper.DesignerTypeToCodeType(
                                        designerType: type,
                                        isNullable: modeloColumna.Nulo,
                                        attributeName: modeloColumna.Id
                                    );
                                    item.BindingType = type;

                                    woContainer.ItemsCol.Add(item);
                                }
                                else
                                {
                                    XtraMessageBox.Show(
                                        text: $"El modelo {item.ClassModelType} que se usaba como referencia, ya no existe en el proyecto",
                                        caption: "Alerta",
                                        buttons: System.Windows.Forms.MessageBoxButtons.OK,
                                        icon: System.Windows.Forms.MessageBoxIcon.Information
                                    );

                                    if (_woSyntaxManagerModelClass != null)
                                    {
                                        _woSyntaxManagerModelClass.DeleteBaseAttribute(
                                            item.BindedProperty,
                                            "Controls"
                                        );

                                        _woSyntaxManagerUserCode.DeleteMethod(
                                            $@"{item.Id}_OnChange"
                                        );
                                        _woSyntaxManagerUserCode.DeleteMethod($@"{item.Id}_OnBlur");
                                        _woSyntaxManagerUserCode.DeleteMethod(
                                            $@"{item.Id}_OnFocus"
                                        );
                                    }
                                }
                            }
                            else
                            {
                                item.Control =
                                    (
                                        modeloColumna.TipoControl
                                        == Core.WoTypeControl.CollectionEditor
                                    )
                                        ? "Slave"
                                        : modeloColumna.TipoControl.ToString();

                                string type = modeloColumna.TipoColumna.ToString();
                                type = _designerTypeHelper.DesignerTypeToCodeType(
                                    designerType: type,
                                    isNullable: modeloColumna.Nulo,
                                    attributeName: modeloColumna.Id
                                );
                                item.BindingType = type;

                                woContainer.ItemsCol.Add(item);
                            }
                        }
                        else
                        {
                            if (item.ComponenteExtra)
                            {
                                woContainer.ItemsCol.Add(item);
                            }
                            else if (_woSyntaxManagerModelClass != null)
                            {
                                _woSyntaxManagerModelClass.DeleteBaseAttribute(
                                    item.BindedProperty,
                                    "Controls"
                                );

                                _woSyntaxManagerUserCode.DeleteMethod($@"{item.Id}_OnChange");
                                _woSyntaxManagerUserCode.DeleteMethod($@"{item.Id}_OnBlur");
                                _woSyntaxManagerUserCode.DeleteMethod($@"{item.Id}_OnFocus");
                            }
                        }
                    }
                }
            }

            return woContainer;
        }

        #endregion Método principal
    }
}
