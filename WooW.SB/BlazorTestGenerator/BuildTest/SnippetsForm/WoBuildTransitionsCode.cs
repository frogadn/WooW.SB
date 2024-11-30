using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.BuildTest.SnippetsForm
{
    public class WoBuildTransitionsCode
    {
        #region Instancias singleton

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Método principal

        /// <summary>
        /// Data de la transición seleccionada
        /// </summary>
        private Transicion _transicion;

        /// <summary>
        /// Data del modelo seleccionado
        /// </summary>
        private Modelo _model;

        /// <summary>
        /// Método principal para orquestar la creación del
        /// codigo en función de la transacción del modelo seleccionada.
        /// </summary>
        /// <returns></returns>
        public string BuildTransitionCode(string modelName, string transitionName)
        {
            try
            {
                _strTestDefaultCode.Clear();

                _model = _project.ModeloCol.Modelos.FirstOrDefault(model => model.Id == modelName);

                _transicion = _model.Diagrama.Transiciones.FirstOrDefault(transition =>
                    transition.Id == transitionName
                );

                _strTestDefaultCode.AppendLine();

                string pathFormDesign = $@"{_project.DirFormDesign}\{modelName}.json";
                string rawJson = WoDirectory.ReadFile(pathFormDesign);

                WoContainer mainContainer = JsonConvert.DeserializeObject<WoContainer>(rawJson);

                BuildCodeRec(mainContainer);

                return $"{_transitionSelect} \n\r{_strTestDefaultCode.ToString()}";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Método principal

        #region Construcción del código

        /// <summary>
        /// Código default del test
        /// </summary>
        private StringBuilder _strTestDefaultCode = new StringBuilder();

        /// <summary>
        /// Selecciona la transición
        /// </summary>
        private string _transitionSelect = string.Empty;

        /// <summary>
        /// Indica que la tab ya cambio
        /// </summary>
        private bool _changedTab = false;

        /// <summary>
        /// Construcción del codigo recorriendo el diseño del formulario
        /// </summary>
        /// <param name="container"></param>
        /// <exception cref="Exception"></exception>
        private void BuildCodeRec(WoContainer mainContainer)
        {
            try
            {
                if (mainContainer.ContainersCol != null)
                {
                    foreach (WoContainer subContainer in mainContainer.ContainersCol)
                    {
                        WoContainer container = subContainer;
                        BuildCodeRec(container);
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    string changeTab = string.Empty;

                    if (
                        mainContainer.ItemsCol.Count > 0
                        && mainContainer.TypeContainer == eTypeContainer.FormTab
                    )
                    {
                        changeTab =
                            $@"        await _{_model.Id}Formulario.{mainContainer.Id}.Select();";
                        _changedTab = false;
                    }

                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        if (_transicion.DTO.Columnas.Contains(item.Id))
                        {
                            if (item.Control == "Label" && item.Id == "Controles")
                            {
                                _transitionSelect =
                                    $@"_{_model.Id}Formulario.ToolBar.cmbTransition.SelectItem('{_transicion.Id}');";
                            }

                            if (!_changedTab)
                            {
                                _strTestDefaultCode.AppendLine(changeTab);
                                _changedTab = true;
                            }

                            if (
                                item.Control == "Decimal"
                                || item.Control == "Text"
                                || item.Control == "byte[]"
                                || item.Control == "Memo"
                                || item.Control == "Spin"
                                || item.Control == "LookUp"
                                || item.Control == "LookUpDialog"
                                || item.Control == "EnumString"
                                || item.Control == "EnumInt"
                                || item.Control == "Date"
                            )
                            {
                                _strTestDefaultCode.AppendLine(
                                    $@"        await {_model.Id}.{item.Id}.Set(""{item.Id}"");"
                                );
                            }
                            else if (item.Control == "Bool")
                            {
                                _strTestDefaultCode.AppendLine(
                                    $@"        await {_model.Id}.{item.Id}.Set(true);"
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Construcción del código
    }
}
