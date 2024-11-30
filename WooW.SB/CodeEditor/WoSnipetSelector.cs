using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using WooW.SB.Config;
using WooW.SB.Helpers;

namespace WooW.SB.CodeEditor
{
    public partial class WoSnipetSelector : UserControl
    {
        #region Event handler

        /// <summary>
        /// Controlador de eventos que se dispara cuando se seleccione alguno de los snipets de la grid
        /// </summary>
        public Action<string> SetSnipetEvt { get; set; }

        #endregion Event handler

        #region Atributos de la clase

        /// <summary>
        /// Instancia de la tabla con los snipets que funjira de datasource para la grid.
        /// </summary>
        private DataTable _dtSnipets = null;

        /// <summary>
        /// Nombre del modelo seleccionado.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Instancia del modelo del que se generara el código.
        /// </summary>
        private Modelo _model = null;

        /// <summary>
        /// Instancia con herramientas de búsquedas y operaciones de modelos.
        /// </summary>
        private WoToolModelsHelper _woToolModelsHelper = new WoToolModelsHelper();

        /// <summary>
        /// Indica si el control sera para formulario o para listas
        /// </summary>
        private bool _isForm = false;

        /// <summary>
        /// Indicador de si el modelo contiene esclavas
        /// </summary>
        private bool _containSlaves = false;

        #endregion Atributos de la clase

        #region Constructor

        /// <summary>
        /// Constructor principal de la clase.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public WoSnipetSelector(string modelName, bool isForm = true)
        {
            InitializeComponent();

            _isForm = isForm;
            _modelName = modelName;
            _model = _woToolModelsHelper.SearchModel(_modelName);

            InitializeSnipedGrid();
        }

        #endregion Constructor


        #region grid

        /// <summary>
        /// Inicializa la grid de snippets relacionada a la grid como data source.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InitializeSnipedGrid()
        {
            _dtSnipets = new DataTable();

            _dtSnipets.Columns.Add("Nombre", typeof(string));
            _dtSnipets.Columns.Add("Snippet", typeof(string));
            _dtSnipets.Columns.Add("Tipo", typeof(string));

            IEnumerable<ModeloColumna> slaves = _model.Columnas.Where(column =>
                column.EsColeccion == true && column.TipoColumna != Core.WoTypeColumn.Complex
            );

            if (slaves.Count() > 0)
            {
                _containSlaves = true;
                _dtSnipets.Columns.Add("Modelo", typeof(string));
            }

            grdSnipets.DataSource = _dtSnipets;

            // Agrupaciones
            if (_containSlaves)
            {
                grdViewSnipets.Columns[@"Modelo"].Group();
                grdViewSnipets.Columns[@"Tipo"].Group();
            }
            else
            {
                grdViewSnipets.Columns[@"Tipo"].Group();
            }

            grdSnipets.DataSource = _dtSnipets;

            AddBaseSnipets();
            AddSnipets(_modelName);

            if (_containSlaves && _isForm)
            {
                foreach (ModeloColumna slave in slaves)
                {
                    AddSnipets(slave.ModeloId);
                }
            }

            grdSnipets.RefreshDataSource();
            grdSnipets.Refresh();
            grdViewSnipets.RowClick += GrdSnipetClick;
        }

        private void AddBaseSnipets()
        {
            DataRow drSnipe = _dtSnipets.NewRow();

            if (_isForm)
            {
                #region fluent

                drSnipe["Nombre"] = "Snippet Fluent NotEmpty";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).NotEmpty().WithMessage(""No puede estar vació""); ";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent NotEqual";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).NotEqual(""a"").WithMessage(""La propiedad MyProperty no debe contener la letra 'a'"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent Equal";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).Equal(""Hola"").WithMessage(""La descripcion no puede ser hola"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent regex";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).Matches(@""^[a-zA-Z]+$"").WithMessage(""La propiedad solo debe contener letras."");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent Must contains";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).Must(x => x.Contains(""Hola"")).WithMessage(""El texto no puede contener la palabra hola"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent InclusiveBetween";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).InclusiveBetween(18, 60).WithMessage(""La edad tiene que estar entre 18 y 60"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent Must";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).Must(x => x % 2 == 0).WithMessage(""El número debe ser par"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent Length";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).Length(20, 250).WithMessage(""La longitud es de 20 a 250"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent CreditCard";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).CreditCard().WithMessage(""No es un número de una tarJeta"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);

                drSnipe = _dtSnipets.NewRow();
                drSnipe["Nombre"] = "Snippet Fluent EmailAddress";
                drSnipe["Snippet"] =
                    @$"RuleFor(x => x.<field>).EmailAddress().WithMessage(""El correo no es valido"");";
                drSnipe["Tipo"] = "Fluent Validation";
                if (_containSlaves)
                {
                    drSnipe["Modelo"] = _model.Id;
                }
                _dtSnipets.Rows.Add(drSnipe);
                #endregion fluent
            }

            #region Alertas

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Agregar Alerta";
            drSnipe["Snippet"] = $@"{_modelName}Controles.Alertas.AgregarAlerta(""mensaje"");";
            drSnipe["Tipo"] = "Alertas";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Agregar Alerta Error";
            drSnipe["Snippet"] =
                $@"{_modelName}Controles.Alertas.AgregarAlerta(""Mensaje"", eTipoDeAlerta.Error);";
            drSnipe["Tipo"] = "Alertas";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Agregar Alerta Advertencia";
            drSnipe["Snippet"] =
                $@"{_modelName}Controles.Alertas.AgregarAlerta(""Mensaje"", eTipoDeAlerta.Advertencia);";
            drSnipe["Tipo"] = "Alertas";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Agregar Alerta Informativa";
            drSnipe["Snippet"] =
                $@"{_modelName}Controles.Alertas.AgregarAlerta(""Mensaje"", eTipoDeAlerta.Mensaje);";
            drSnipe["Tipo"] = "Alertas";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Agregar Alerta Success";
            drSnipe["Snippet"] =
                $@"{_modelName}Controles.Alertas.AgregarAlerta(""Mensaje"", eTipoDeAlerta.Ok);";
            drSnipe["Tipo"] = "Alertas";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            #endregion Alertas

            #region Data

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet OData";

            drSnipe["Snippet"] =
                @$"var o<model>Col = <model>.List(woTarget, new <model>List()
             {{
                select = """",
                filter = """",
                orderby = """",
                top = 10,
                skip = 0
             }});";
            drSnipe["Tipo"] = "Data";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Get";
            drSnipe["Snippet"] = @$"var o<model> = await <model>.GetAsync(woTarget, ""..."");";
            drSnipe["Tipo"] = "Data";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet GetCheck";
            drSnipe["Snippet"] = @$"var o<model> = await <model>.GetCheckAsync(woTarget, ""..."");";
            drSnipe["Tipo"] = "Data";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Single";
            drSnipe["Snippet"] = @$"var o<model> = await <model>.SingleAsync(woTarget, ""..."");";
            drSnipe["Tipo"] = "Data";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet SingleCheck";
            drSnipe["Snippet"] =
                @$"var o<model> =await  <model>.SingleCheckAsync(woTarget, ""..."");";
            drSnipe["Tipo"] = "Data";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            #endregion Data

            #region JSInterop

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Save File";
            drSnipe["Snippet"] =
                @$"byte[] bytes = {{ 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 }};
JS.InvokeAsync<object>(""SaveFile"", @$""fileName.txt"", bytes);";
            drSnipe["Tipo"] = "JSInterop";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet PrintFile";
            drSnipe["Snippet"] =
                @$"byte[] bytes = {{ 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 }};
JS.InvokeAsync<object>(""PrintFile"", @$""fileName.txt"",""application/pdf"", bytes);";
            drSnipe["Tipo"] = "JSInterop";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet OpenInNewtab";
            drSnipe["Snippet"] =
                @$"JS.InvokeAsync<object>(""OpenInNewtab"", ""https://www.frog.com.mx"");
JS.InvokeAsync<object>(""OpenInNewtab"", new object[2] {{ ""https://www.frog.com.mx"", ""_blank"" }});";
            drSnipe["Tipo"] = "JSInterop";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet NavigateTo";
            drSnipe["Snippet"] =
                @$"JS.InvokeAsync<object>(""NavigateTo"", ""https://www.frog.com.mx/"");";
            drSnipe["Tipo"] = "JSInterop";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);

            #endregion JSInterop

            drSnipe = _dtSnipets.NewRow();
            drSnipe["Nombre"] = "Snippet Invoke StateHasChange";
            drSnipe["Snippet"] = @$"StateHasChangeEvt?.Invoke();";
            drSnipe["Tipo"] = "Helpers";
            if (_containSlaves)
            {
                drSnipe["Modelo"] = _model.Id;
            }
            _dtSnipets.Rows.Add(drSnipe);
        }

        /// <summary>
        /// Agrega los snippets al data source de la grid.
        /// </summary>
        private void AddSnipets(string modelName)
        {
            modelName = (_isForm) ? modelName : $@"{modelName}GridList";
            modelName = modelName.Replace("GridList", String.Empty);
            Modelo model = _woToolModelsHelper.SearchModel(modelName);

            if (_isForm)
            {
                #region style

                foreach (ModeloColumna column in model.Columnas)
                {
                    if (
                        (
                            column.Id.Contains("__")
                            || column.EsColeccion
                            || column.ModeloId == model.Id
                        )
                        && column.TipoColumna != Core.WoTypeColumn.Complex
                    )
                    {
                        continue;
                    }

                    DataRow drModelSnipe = _dtSnipets.NewRow();

                    drModelSnipe = _dtSnipets.NewRow();
                    drModelSnipe["Nombre"] = column.Id + " Snippet Cambiar color de fondo";
                    drModelSnipe["Snippet"] =
                        $@"{modelName}Controles.{column.Id}.ColorDeFondo(eContainerItemColor.Default);";
                    drModelSnipe["Tipo"] = $@"{modelName} " + column.Id;
                    if (_containSlaves)
                    {
                        drModelSnipe["Modelo"] = modelName;
                    }
                    _dtSnipets.Rows.Add(drModelSnipe);

                    drModelSnipe = _dtSnipets.NewRow();
                    drModelSnipe["Nombre"] = column.Id + " Snippet Cambiar letra itálica";
                    drModelSnipe["Snippet"] =
                        $@"{modelName}Controles.{column.Id}.ControlEnItalica(eTextItalic.None);";
                    drModelSnipe["Tipo"] = $@"{modelName} " + column.Id;
                    if (_containSlaves)
                    {
                        drModelSnipe["Modelo"] = modelName;
                    }
                    _dtSnipets.Rows.Add(drModelSnipe);

                    //drModelSnipe = _dtSnipets.NewRow();
                    //drModelSnipe["Nombre"] = column.Id + " Snippet Cambiar color de letra";
                    //drModelSnipe["Snippet"] =
                    //    $@"{modelName}Controles.{column.Id}.ColorDeLetra(eTextColor.Primary);";
                    //drModelSnipe["Tipo"] = "Modelo " + column.Id;
                    //_dtSnipets.Rows.Add(drModelSnipe);

                    drModelSnipe = _dtSnipets.NewRow();
                    drModelSnipe["Nombre"] =
                        column.Id + " Snippet Cambiar color de letra del control";
                    drModelSnipe["Snippet"] =
                        $@"{modelName}Controles.{column.Id}.ColorDeLetraDeControl(eTextColor.FontDefault);";
                    drModelSnipe["Tipo"] = $@"{modelName} " + column.Id;
                    if (_containSlaves)
                    {
                        drModelSnipe["Modelo"] = modelName;
                    }
                    _dtSnipets.Rows.Add(drModelSnipe);

                    drModelSnipe = _dtSnipets.NewRow();
                    drModelSnipe["Nombre"] = column.Id + " Snippet Cambiar decoración";
                    drModelSnipe["Snippet"] =
                        $@"{modelName}Controles.{column.Id}.DecoracionDeLetraDeControl(eTextDecoration.None);";
                    drModelSnipe["Tipo"] = $@"{modelName} " + column.Id;
                    if (_containSlaves)
                    {
                        drModelSnipe["Modelo"] = modelName;
                    }
                    _dtSnipets.Rows.Add(drModelSnipe);

                    drModelSnipe = _dtSnipets.NewRow();
                    drModelSnipe["Nombre"] = column.Id + " Snippet Cambiar estado habilitado";
                    drModelSnipe["Snippet"] =
                        $@"{modelName}Controles.{column.Id}.EstadoHabilitado(true);";
                    drModelSnipe["Tipo"] = $@"{modelName} " + column.Id;
                    if (_containSlaves)
                    {
                        drModelSnipe["Modelo"] = modelName;
                    }
                    _dtSnipets.Rows.Add(drModelSnipe);

                    drModelSnipe = _dtSnipets.NewRow();
                    drModelSnipe["Nombre"] = column.Id + " Snippet Cambiar grosor del texto";
                    drModelSnipe["Snippet"] =
                        $@"{modelName}Controles.{column.Id}.GrosorDeLetraDeControl(eTextWeight.Normal);";
                    drModelSnipe["Tipo"] = $@"{modelName} " + column.Id;
                    if (_containSlaves)
                    {
                        drModelSnipe["Modelo"] = modelName;
                    }
                    _dtSnipets.Rows.Add(drModelSnipe);

                    drModelSnipe = _dtSnipets.NewRow();
                    drModelSnipe["Nombre"] = column.Id + " Snippet Cambiar tamaño de letra";
                    drModelSnipe["Snippet"] =
                        $@"{modelName}Controles.{column.Id}.TamañoDeLetra(eItemSize.Normal);";
                    drModelSnipe["Tipo"] = $@"{modelName} " + column.Id;
                    if (_containSlaves)
                    {
                        drModelSnipe["Modelo"] = modelName;
                    }
                    _dtSnipets.Rows.Add(drModelSnipe);
                }
                #endregion style
            }
        }

        /// <summary>
        /// Evento suscrito al constrolador de eventos del click, que detona el accion para enviar el snipet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        [SupportedOSPlatform("windows")]
        private void GrdSnipetClick(object sender, EventArgs args)
        {
            if (grdViewSnipets.GetFocusedRowCellValue("Snippet") != null)
            {
                string snippetText = grdViewSnipets.GetFocusedRowCellValue("Snippet").ToString();
                SetSnipetEvt?.Invoke(snippetText);
            }
        }

        #endregion grid
    }
}
