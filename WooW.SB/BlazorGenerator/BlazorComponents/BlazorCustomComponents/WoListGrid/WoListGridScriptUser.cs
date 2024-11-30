using System.IO;
using System.Text;
using Newtonsoft.Json;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorGenerator.BlazorComponents.BlazorCustomComponents.WoListGrid
{
    public class WoListGridScriptUser
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con las configuraciones e información del proyecto con el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Resultado final del razor del List con los eventos de la list.
        /// </summary>
        private StringBuilder _listGrid = new StringBuilder();

        /// <summary>
        /// Instancia con la meta data del item.
        /// </summary>
        private WoItem _item = new WoItem();

        /// <summary>
        /// Nivel de identación
        /// </summary>
        private string _identMethodsAndProperties = string.Empty;

        /// <summary>
        /// Propiedades de la grid
        /// </summary>
        private WoGridProperties _gridProperties = new WoGridProperties();

        /// <summary>
        /// Modelo del grid que se esta generando.
        /// </summary>
        private Modelo _model;

        #endregion Atributos


        #region Constructor de la clase

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        /// <param name="identMethodsAndProperties"></param>
        /// <param name="woItem"></param>
        public WoListGridScriptUser(string identMethodsAndProperties)
        {
            _identMethodsAndProperties = identMethodsAndProperties;
        }

        #endregion Constructor de la clase


        #region Método principal

        /// <summary>
        /// Construye la base de los métodos para los eventos de la grid
        /// </summary>
        /// <param name="woItem"></param>
        /// <returns></returns>
        public string GetCode(WoItem woItem)
        {
            _item = woItem;

            //_model = _project.ModeloCol.Modelos.FirstOrDefault(model =>
            //    model.Id == _item.BaseModelName
            //);

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            _model = woProjectDataHelper.GetMainModel(_item.BaseModelName);

            string pathJson =
                $@"{_project.DirLayOuts}\ListDesign\{_item.BaseModelName}GridList.json";

            if (!File.Exists(pathJson))
            {
                WoGridDesignerRawHelper rawHelper = new WoGridDesignerRawHelper();
                WoGridProperties rawDesign = rawHelper.GetRawGrid(
                    _item.BaseModelName,
                    isSlave: true
                );
                string rawJson = JsonConvert.SerializeObject(rawDesign);
                WoDirectory.WriteFile(pathJson, rawJson);
            }

            string json = WoDirectory.ReadFile(pathJson);
            _gridProperties = JsonConvert.DeserializeObject<WoGridProperties>(json);

            _listGrid.Clear();

            _listGrid.AppendLine($@"{_identMethodsAndProperties}#region {_item.BaseModelName}");

            _listGrid.AppendLine(BuildTag());

            _listGrid.AppendLine(BuildEventMethods());

            _listGrid.AppendLine($@"{_identMethodsAndProperties}#endregion {_item.BaseModelName}");

            return _listGrid.ToString();
        }

        #endregion Método principal


        #region Tag

        /// <summary>
        /// Construlle el tag
        /// </summary>
        /// <returns></returns>
        private string BuildTag()
        {
            return $@"
{_identMethodsAndProperties}// Este código fue generado por el fichero WoTextEditScriptUser.cs.
{_identMethodsAndProperties}// WoWSB por el generador a día 25-10-2023";
        }

        #endregion Tag

        #region Build event methods

        /// <summary>
        /// Constriction del método de los eventos de la grid
        /// </summary>
        /// <returns></returns>
        private string BuildEventMethods()
        {
            StringBuilder strEventMethods = new StringBuilder();

            strEventMethods.AppendLine(
                $@"
{_identMethodsAndProperties}public void OnPageIndexChanged()
{_identMethodsAndProperties}{{

{_identMethodsAndProperties}}}"
            );

            strEventMethods.AppendLine(
                $@"
{_identMethodsAndProperties}public void OnPageSizeChanged()
{_identMethodsAndProperties}{{

{_identMethodsAndProperties}}}"
            );

            strEventMethods.AppendLine(
                $@"
{_identMethodsAndProperties}public void OnFilterChanged()
{_identMethodsAndProperties}{{

{_identMethodsAndProperties}}}"
            );

            strEventMethods.AppendLine(
                $@"
{_identMethodsAndProperties}public void OnFilterReset()
{_identMethodsAndProperties}{{

{_identMethodsAndProperties}}}"
            );

            strEventMethods.AppendLine(
                $@"
{_identMethodsAndProperties}public void OnSelectedRowChanged({_item.BaseModelName} SelectedRow)
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    {_item.BaseModelName} = SelectedRow;

{_identMethodsAndProperties}}}"
            );

            if (_model.TipoModelo != Core.WoTypeModel.View)
            {
                strEventMethods.AppendLine(
                    $@"
{_identMethodsAndProperties}public void Selected{_item.BaseModelName}ItemsChanged(IEnumerable<{_item.BaseModelName}> {_item.BaseModelName.ToLower()})
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    {_item.BaseModelName}Seleccionados = {_item.BaseModelName.ToLower()}.ToList();
{_identMethodsAndProperties}}}"
                );
            }
            else if (_model.TipoModelo == Core.WoTypeModel.View && _gridProperties.GridSelect)
            {
                strEventMethods.AppendLine(
                    $@"
{_identMethodsAndProperties}public void Selected{_item.BaseModelName}ItemsChanged(IEnumerable<{_item.BaseModelName}> {_item.BaseModelName.ToLower()})
{_identMethodsAndProperties}{{
{_identMethodsAndProperties}    {_item.BaseModelName}Seleccionados = {_item.BaseModelName.ToLower()}.ToList();
{_identMethodsAndProperties}}}"
                );
            }

            return strEventMethods.ToString();
        }

        #endregion Build event methods
    }
}
