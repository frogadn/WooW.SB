using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using DevExpress.Data;
using DevExpress.DataAccess.Native.Web;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList.Nodes;
using FastMember;
using ServiceStack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Forms;
using WooW.Core;
using WooW.Core.Common;
using WooW.SB.Config.Helpers;

namespace WooW.SB.Config.Editors
{
    public delegate void setSnippet(string snippet);

    public partial class woGridModel : DevExpress.XtraEditors.XtraUserControl
    {

        Modelo currentModel = null;

        Assembly assemblyModelData = null;
        Type ODataType = null;
        Type ODataTypeList = null;
        int ODataPageSize = 20;
        int ODataCurrentTop = 0;
        bool ODataNextPage = true;
        bool ODataBestFit = false;
        string ODataOrderBy = string.Empty;
        string ODataFilter = string.Empty;
        string ODataLastModel = string.Empty;

        JsonApiClient woTarget = null;

        private DevExpress.Data.VirtualServerModeSource virtualServerModeSource;

        private Proyecto principal = null;
        public enum eSnippet
        { NoMostrar, SnippetServidor, SnippetCliente };

        public setSnippet SetSnippet { get; set; } = null;

        public eSnippet Snippet { get; set; } = eSnippet.NoMostrar;

        public woGridModel()
        {
            InitializeComponent();

            //principal = ModelMakerPrincipal.Do(Proyecto.getInstance().ArchivoDeProyecto);
            //principal = Proyecto.getInstance();
        }

        public void ClearLogin()
        {
            woTarget = null;
        }

        public void Filtra(string Filtra)
        {

            if (!buAutoFiltro.Checked)
                return;

            var modelo = principal.ModeloCol.Modelos.Where(e => e.Id == Filtra.Trim()).FirstOrDefault();
            if (modelo == null)
            {
                if (Filtra.StartsWith("o") && (Filtra.Length > 2))
                    Filtra = Filtra.Substring(1);
                else if ((Filtra.StartsWith("cur") || Filtra.StartsWith("dto") || Filtra.StartsWith("req")) && (Filtra.Length > 4))
                    Filtra = Filtra.Substring(3);

                if (Filtra.EndsWith("Col") && (Filtra.Length > 4))
                    Filtra = Filtra.Substring(0, Filtra.Length - 3);


                modelo = principal.ModeloCol.Modelos.Where(e => e.Id == Filtra.Trim()).LastOrDefault();

                if (modelo == null)
                {
                    modelo = principal.ModeloCol.Modelos.Where(e => Filtra.ToUpper().StartsWith(e.Id.ToUpper())).OrderBy(e => e.Id.Length).LastOrDefault();
                    if (modelo != null)
                        Filtra = modelo.Id;
                }

                if (modelo == null)
                {
                    searchControl2.EditValue = string.Empty;
                    return;
                }
            }

            searchControl2.EditValue = Filtra;
        }

        private void woGridModelo_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                ProyectoConPaquetes.Clear();
                principal = ProyectoConPaquetes.Get(Proyecto.getInstance().ArchivoDeProyecto);
                woDiagram1.proyecto = principal;
            }

            var language = new CSharpSyntaxLanguage();

            txtScriptPre.Document.Language = language;
            var formatterPre = (CSharpTextFormatter)txtScriptPre.Document.Language.GetTextFormatter();
            formatterPre.IsOpeningBraceOnNewLine = true;

            txtScriptPost.Document.Language = language;
            var formatterPost = (CSharpTextFormatter)txtScriptPost.Document.Language.GetTextFormatter();
            formatterPost.IsOpeningBraceOnNewLine = true;

            CreaModeloPropiedades();
            CreaArbolModelos();

            woDiagram1.Enabled = true;
            woDiagram1.SetReadOnly(true);
        }

        private void CreaModeloPropiedades()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(@"Columna", typeof(string));

            dt.Columns.Add(@"Descripcion", typeof(string));

            dt.Columns.Add(@"TipoCol", typeof(string));
            dt.Columns.Add(@"Longitud", typeof(int));
            dt.Columns.Add(@"Precision", typeof(int));

            dt.Columns.Add(@"TipoDato", typeof(string));
            dt.Columns.Add(@"Modelo", typeof(string));


            dt.Columns.Add(@"AceptaNulos", typeof(bool));
            dt.Columns.Add(@"Primaria", typeof(bool));
            dt.Columns.Add(@"Default", typeof(string));
            dt.Columns.Add(@"Control", typeof(string));
            dt.Columns.Add(@"Legacy", typeof(string));

            grdColumna.DataSource = dt;

            GridColumn col = grdColumnaView.Columns["Columna"];
            col.Width = 100;
            col = grdColumnaView.Columns[@"Descripcion"];
            col.Width = 200;
            col = grdColumnaView.Columns["TipoCol"];
            col.Width = 80;
            col.Caption = "Tipo de Columna";
            col = grdColumnaView.Columns["Longitud"];
            col.Width = 50;
            col = grdColumnaView.Columns[@"Precision"];
            col.Width = 50;
            col = grdColumnaView.Columns["TipoDato"];
            col.Width = 100;
            col.Caption = "Tipo de Dato";
            col = grdColumnaView.Columns["Modelo"];
            col.Width = 100;
            col = grdColumnaView.Columns["AceptaNulos"];
            col.Width = 50;
            col.Caption = "Acepta Nulos?";
            col = grdColumnaView.Columns["Primaria"];
            col.Width = 50;
            col.Caption = "LLave Primaria?";
            col = grdColumnaView.Columns["Default"];
            col.Width = 100;
            col.Caption = "Valor Default";
            col = grdColumnaView.Columns["Control"];
            col.Width = 50;
            col = grdColumnaView.Columns["Legacy"];
            col.Width = 100;
        }

        private void CreaArbolModelos()
        {
            if (principal != null)
                foreach (var model in principal.ModeloCol.Modelos.OrderBy(e => e.Id))
                {
                    TreeListNode newNode = treeModelos.AppendNode(new object[] { model.Id }, -1, model);

                    newNode.ImageIndex = 4;
                    newNode.SelectImageIndex = 5;
                    newNode.StateImageIndex = 4;
                    newNode.Tag = model;

                    foreach (var transicion in model.Diagrama.Transiciones)
                    {
                        TreeListNode NodeChild = treeModelos.AppendNode(
                            new object[] { model.Id + transicion.Id },
                            newNode.Id,
                            transicion
                        );

                        NodeChild.ImageIndex = 6;
                        NodeChild.SelectImageIndex = 7;
                        NodeChild.StateImageIndex = 6;
                        NodeChild.Tag = new Tuple<Modelo, Transicion>(model, transicion);
                    }
                }
        }

        public Action ModelFocusedChangeEvt;

        private void treeModelos_FocusedNodeChanged(
            object sender,
            DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e
        )
        {
            if ((treeModelos.FocusedNode.IsNull()) || (treeModelos.FocusedNode.Tag.IsNull()))
                return;

            if (treeModelos.FocusedNode.Tag is Modelo)
            {
                ConstruyePropiedadesModelo((treeModelos.FocusedNode.Tag as Modelo));

                woDiagram1.New();
                woDiagram1.currDiagrama = (treeModelos.FocusedNode.Tag as Modelo).Diagrama;
                woDiagram1.CreaDiagrama();

                txtScriptPre.Text = (treeModelos.FocusedNode.Tag as Modelo).GetPreCondiciones();
                txtScriptPost.Text = (treeModelos.FocusedNode.Tag as Modelo).GetPostCondiciones();

                if (ODataLastModel == (treeModelos.FocusedNode.Tag as Modelo).Id)
                    return;

                ModelFocusedChangeEvt?.Invoke();
            }
            else
            {
                Tuple<Modelo, Transicion> t = (
                    treeModelos.FocusedNode.Tag as Tuple<Modelo, Transicion>
                );
                ConstruyePropiedadesTransicion(t.Item1, t.Item2);

                woDiagram1.New();
                woDiagram1.currDiagrama = (t.Item1).Diagrama;
                woDiagram1.CreaDiagrama();

                txtScriptPre.Text = (t.Item1).GetPreCondiciones();
                txtScriptPost.Text = (t.Item1).GetPostCondiciones();

                if (ODataLastModel == (t.Item1).Id)
                    return;
            }

            if (tabExplorador.SelectedTabPage == tabOData)
                DoODataModel();
        }

        private void ConstruyePropiedadesTransicion(Modelo model, Transicion transicion)
        {
            DataTable dt = grdColumna.DataSource as DataTable;

            dt.Rows.Clear();

            foreach (var colDTO in transicion.DTO.Columnas)
            {
                var col = model.Columnas.Where(e => e.Id == colDTO).FirstOrDefault();

                if (col == null)
                    return;

                AddColumRow(
                    dt,
                    col.Id,
                    col.Descripcion,
                    col.TipoColumna,
                    col.Longitud,
                    col.Precision,
                    col.TipoDato,
                    col.ModeloId,
                    col.Nulo,
                    col.Primaria,
                    col.Default,
                    col.TipoControl,
                    col.Legacy
                );
            }
        }

        private void ConstruyePropiedadesModelo(Modelo model)
        {
            string[] SystemStringCols = new string[] { WoConst.CREATEDBY, WoConst.MODIFIEDBY, WoConst.DELETEDBY };
            string[] SystemDateCols = new string[] { WoConst.CREATEDDATE, WoConst.MODIFIEDDATE, WoConst.DELETEDDATE };

            woDiagram1.AsignaDTO(model);

            DataTable dt = grdColumna.DataSource as DataTable;

            dt.Rows.Clear();

            foreach (var col in model.Columnas.OrderBy(e => e.Orden))
            {
                AddColumRow(
                    dt,
                    col.Id,
                    col.Descripcion,
                    col.TipoColumna,
                    col.Longitud,
                    col.Precision,
                    col.TipoDato,
                    col.ModeloId,
                    col.Nulo,
                    col.Primaria,
                    col.Default,
                    col.TipoControl,
                    col.Legacy
                );
            }

            grdColumnaView.BestFitColumns();
        }

        private void AddColumRow(
            DataTable dt,
            string Columna,
            string Descripcion,
            WoTypeColumn tc,
            int Long,
            int Prec,
            WoTypeData td,
            string Modelo,
            bool AceptaNulos,
            bool Primaria,
            string Default,
            WoTypeControl tco,
            string legacy
        )
        {
            DataRow dr;

            dr = dt.NewRow();

            dr["Columna"] = Columna;
            dr[@"Descripcion"] = Descripcion;
            dr["TipoCol"] = tc.ToString();
            dr["Longitud"] = Long;
            dr[@"Precision"] = Prec;
            dr["TipoDato"] = td.ToString();
            dr["Modelo"] = Modelo;
            dr["AceptaNulos"] = AceptaNulos;
            dr["Primaria"] = Primaria;
            dr["Default"] = Default;
            dr["Control"] = tco.ToString();
            dr["Legacy"] = legacy;

            dt.Rows.Add(dr);
        }

        public void ZoomIn()
        {
            woDiagram1.ZoomIn();
        }

        public void ZoomOut()
        {
            woDiagram1.ZoomOut();
        }

        public void MostraSoloDiagrama()
        {
            woDiagram1.MostraSoloDiagrama();
        }

        public void OcultaReglas()
        {
            woDiagram1.OcultaReglas();
        }

        #region Manejo de snippets custom

        /// <summary>
        /// Lista de opciones del menu custom para agregar snippets desde la capa superior
        /// </summary>
        private List<(string title, Action<string> actionEvt)> _customMenuContext = new List<(string title, Action<string> actionEvt)>();

        /// <summary>
        /// Agrega a la lista del menu custom otra opción desde la capa superior
        /// </summary>
        public void SetSnippetOption(string title, Action<string> actionEvt)
        {
            try
            {
                _customMenuContext.Add((title, actionEvt));
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al asignar la opción custom: {title} del menu. {ex.Message}");
            }
        }

        /// <summary>
        /// Limpia los métodos de la lista de los menus custom
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CleanCustomMethods()
        {
            try
            {
                _customMenuContext.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al eliminar los métodos custom del menu. {ex.Message}");
            }
        }

        #endregion Manejo de snippets custom

        [SupportedOSPlatform("windows")]
        private void treeModelos_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            currentModel = null;

            if (Snippet == eSnippet.NoMostrar)
                return;

            currentModel = ModeloSeleccionado();
            if (currentModel == null)
                return;


            if (SetSnippet != null)
            {
                if ((treeModelos.FocusedNode.IsNull()) || (treeModelos.FocusedNode.Tag.IsNull()))
                    return;

                bool EsModelo = (treeModelos.FocusedNode.Tag is Modelo);

                if (EsModelo)
                {
                    var TipoModelo = (treeModelos.FocusedNode.Tag as Modelo).TipoModelo;

                    string modelId = (treeModelos.FocusedNode.Tag as Modelo).Id;

                    e.Menu.Items.Add(
                        new DXMenuItem("Snippet New", CrearNew_Click));

                    if (
                        (TipoModelo == WoTypeModel.Configuration)
                        || (TipoModelo == WoTypeModel.CatalogType)
                        || (TipoModelo == WoTypeModel.Catalog)
                        || (TipoModelo == WoTypeModel.TransactionContable)
                        || (TipoModelo == WoTypeModel.TransactionNoContable)
                        || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                        || (TipoModelo == WoTypeModel.Control)
                        || (TipoModelo == WoTypeModel.Kardex)
                        || (TipoModelo == WoTypeModel.Parameter)
                        || (TipoModelo == WoTypeModel.TransactionSlave)
                        || (TipoModelo == WoTypeModel.CatalogSlave)
                    )
                    {

                        foreach ((string title, Action<string> actionEvt) menuSnippet in _customMenuContext)
                        {
                            e.Menu.Items.Add(new DXMenuItem(
                                caption: menuSnippet.title,
                                click: (object sender, EventArgs e) => menuSnippet.actionEvt.Invoke(modelId)));
                        }

                        e.Menu.Items.Add(
                            new DXMenuItem("Snippet Get", CrearGet_Click));

                        e.Menu.Items.Add(
                            new DXMenuItem("Snippet GetCheck", CrearGet_Click));

                        e.Menu.Items.Add(
                            new DXMenuItem("Snippet Single", CrearGet_Click));

                        e.Menu.Items.Add(
                            new DXMenuItem("Snippet SingleCheck", CrearGet_Click));

                        bool bUniqueGet = ((treeModelos.FocusedNode.Tag as Modelo).Columnas.Where(f => f.TipoDato == WoTypeData.UniqueGet).FirstOrDefault() != null);
                        if (bUniqueGet)
                            e.Menu.Items.Add(
                                new DXMenuItem("Snippet GetBy", CrearGetBy_Click));


                        e.Menu.Items.Add(
                            new DXMenuItem("Snippet OData", CrearOData_Click));

                        if (Snippet == eSnippet.SnippetCliente)
                        {
                            e.Menu.Items.Add(
                                new DXMenuItem("Snippet Dump", CrearDump_Click));
                            e.Menu.Items.Add(
                                new DXMenuItem("Snippet DumpTable", CrearDumpTable_Click));
                        }
                        else
                        {
                            var locMenu = new DXSubMenuItem("Snippet Query");
                            e.Menu.Items.Add(locMenu);

                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select", CrearSelect_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Single", CrearSelectSingle_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Single By Id", CrearSelectSingleById_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Count", CrearSelectCount_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Exist", CrearSelectExist_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Scalar", CrearSelectScalar_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Diccionario", CrearSelectDictionry_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Par de Valores", CrearSelectPairValues_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select por SQL", CrearSelectSQL_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select por SQL Scalar", CrearSelectSQLScalar_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select por SQL Execute", CrearSelectSQLExecute_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Join Implícito", CrearSelectJoinImplicit_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Select Join Explicito", CrearSelectJoinExplicit_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet SQL Generado", CrearSelectStatement_Click));
                            locMenu.Items.Add(
                                new DXMenuItem("Snippet Ultimo SQL ejecutado", CrearSelectLastest_Click));

                        }
                    }

                    if (TipoModelo == WoTypeModel.Request)
                        e.Menu.Items.Add(
                            new DXMenuItem("Snippet Get", PostPatch_Click)
                        );

                }

                if (!EsModelo) // Es transicion
                {
                    var Transicion = (treeModelos.FocusedNode.Tag as Tuple<Modelo, Transicion>);
                    var TipoModelo = (Transicion.Item1 as Modelo).TipoModelo;

                    bool bPost = false;

                    if (
                         (TipoModelo == WoTypeModel.CatalogType)
                        || (TipoModelo == WoTypeModel.Catalog)
                        || (TipoModelo == WoTypeModel.TransactionContable)
                        || (TipoModelo == WoTypeModel.TransactionNoContable)
                        || (TipoModelo == WoTypeModel.TransactionFreeStyle)
                        || (TipoModelo == WoTypeModel.Parameter)
                    )
                        bPost = true;
                    else if ((Snippet == eSnippet.SnippetServidor) &&
                            ((TipoModelo == WoTypeModel.Configuration)
                            || (TipoModelo == WoTypeModel.Control)
                            || (TipoModelo == WoTypeModel.Kardex)
                            || (TipoModelo == WoTypeModel.DataMart)))
                        bPost = true;

                    if (bPost)
                    {
                        string Verb = "Put";
                        if (Transicion.Item2.EstadoInicial == 0)
                            Verb = "Post";
                        else if (((TipoModelo == WoTypeModel.CatalogType)
                                || (TipoModelo == WoTypeModel.Catalog)) &&
                            (Transicion.Item2.EstadoFinal == 600))
                            Verb = "SoftDelete";

                        var locMenu = new DXMenuItem($"Snippet {Verb}", PostPatch_Click);
                        locMenu.Tag = Verb;
                        e.Menu.Items.Add(locMenu);

                        if (Verb == "Put")
                        {
                            Verb = "Patch";
                            locMenu = new DXMenuItem($"Snippet {Verb}", PostPatch_Click);
                            locMenu.Tag = Verb;
                            e.Menu.Items.Add(locMenu);
                        }
                    }
                }
            }
        }


        public Modelo ModeloSeleccionado()
        {
            if (SetSnippet == null)
                return null;

            Modelo modelo = null;

            if ((treeModelos.FocusedNode.IsNull()) || (treeModelos.FocusedNode.Tag.IsNull()))
                return null;

            if (treeModelos.FocusedNode.Tag is Modelo)
            {
                modelo = (treeModelos.FocusedNode.Tag as Modelo);
            }
            else
            {
                Tuple<Modelo, Transicion> t = (
                    treeModelos.FocusedNode.Tag as Tuple<Modelo, Transicion>
                );

                modelo = t.Item1;
            }

            return modelo;
        }


        private void CrearNew_Click(object sender, EventArgs e)
        {
            Get_Servicio((treeModelos.FocusedNode.Tag as Modelo), "", "");
        }


        private void CrearGet_Click(object sender, EventArgs e)
        {
            string[] Tokens = (sender as DXMenuItem).Caption.Split(' ');

            string verb = Tokens[Tokens.Length - 1];

            SnippetGet(verb, string.Empty);
        }

        private void SnippetGet(string verb, string Id)
        {
            Id = (Id.IsNullOrEmpty() ? "..." : Id);

            if (currentModel.TipoModelo == WoTypeModel.Parameter)
                SetSnippet($"\r\n\t\t\tvar o{currentModel.Id} = {currentModel.Id}.{verb}(woTarget);");
            else
                SetSnippet($"\r\n\t\t\tvar o{currentModel.Id} = {currentModel.Id}.{verb}(woTarget, \"{Id}\");");
        }

        private void CrearGetBy_Click(object sender, EventArgs e)
        {
            string Snippet = $"\r\n\t\t\tvar o{currentModel.Id} = {currentModel.Id}.GetBy(woTarget, new {currentModel.Id}GetBy ()\r\n\t\t\t\t{{";

            foreach (ModeloColumna col in currentModel.Columnas.Where(f => f.TipoDato == WoTypeData.UniqueGet).OrderBy(f => f.Orden))
                Snippet += $"\r\n\t\t\t\t\t{col.Id} = \"...\",";

            Snippet += $"\r\n\t\t\t\t}});";

            SetSnippet(Snippet);
        }

        private void CrearOData_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"\t\t\tvar o{currentModel.Id}Col = {currentModel.Id}.List(woTarget, new {currentModel.Id}List()");
            sb.AppendLine(
@"             {
                select = """",
                filter = """",
                orderby = """",
                top = 10,
                skip = 0
             });");

            SetSnippet(sb.ToString());
        }


        private void CrearDump_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tUth.Log(o{currentModel.Id}.Dump());");
        }

        private void CrearDumpTable_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tUth.Log(o{currentModel.Id}.DumpTable());");
        }

        private void CrearSelect_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\t\t\tvar {currentModel.Id}s = woTarget.Db.Select<{currentModel.Id}>(x => ???)");
            sb.AppendLine($"\t\t\t\t\t\t\t// string.Compare(x.Propiedad, \"Valor\") > 0 // Compara string ");
            sb.AppendLine($"\t\t\t\t\t\t\t// x.Propiedad > 0                            // Compara int");
            sb.AppendLine($"\t\t\t\t\t\t\t// x.Propiedad > new DateTime(2020, 1, 1)     // Compara date");
            sb.AppendLine($"\t\t\t\t\t\t\t// Sql.In(x.Propiedad, \"London\", \"Madrid\")// Esta en");
            sb.AppendLine($"\t\t\t\t\t\t\t// x.Propiedad.StartsWith(\"A\")              // Comienza con");
            sb.AppendLine($"\t\t\t\t\t\t\t// x.Propiedad.EndsWith(\"B\")                // Termina con");
            sb.AppendLine($"\t\t\t\t\t\t\t// x.Propiedad.Contains(\"C\")                // Contiene");
            sb.AppendLine($"\t\t\t\t\t\t\t// !x.Propiedad.Contains(\"C\")               // No Contiene");
            sb.AppendLine($"\t\t\t\t\t\t\t// .Skip(3)                                   // Saltar ");
            sb.AppendLine($"\t\t\t\t\t\t\t// .Take(3)                                   // Solo los X primeros");
            sb.AppendLine($"\t\t\t\t\t\t\t// .OrderBy(x => x.Propiedad)                 // Ordenar por ascendente");
            sb.AppendLine($"\t\t\t\t\t\t\t// .OrderByDescending(x => x.Propiedad)       // Ordenar por descendente");

            SetSnippet(sb.ToString());
        }

        private void CrearSelectSingle_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tvar o{currentModel.Id} = woTarget.Db.SingleById<{currentModel.Id}>(\"Id\"); // Un registro por Id");
        }
        private void CrearSelectSingleById_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tvar o{currentModel.Id} = woTarget.Db.Single<{currentModel.Id}>(x => x.Propiedad == XX); // Un registro");
        }

        private void CrearSelectCount_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tlong Valor = woTarget.Db.Count<{currentModel.Id}>(x => x.Propiedad == XX); // Contar");
        }

        private void CrearSelectExist_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tbool Exists = woTarget.Db.Exists<{currentModel.Id}>(new {{Propiedad = XX}}); // Existe?");
        }

        private void CrearSelectScalar_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tlong Scalar = woTarget.Db.Scalar<{currentModel.Id}, int>(x => Sql.Max(x.Propiedad1), x => x.Propiedad2 < 50); // Escalar");
        }

        private void CrearSelectDictionry_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\t\t\tvar Query = woTarget.Db.From<{currentModel.Id}> ()");
            sb.AppendLine($"\t\t\t\t\t\t\t.Where(x => x.Propiedad < 50)");
            sb.AppendLine($"\t\t\t\t\t\t\t.Select(x => new {{x.Propiedad, x.Propiedad2}});");
            sb.AppendLine($"\t\t\tDictionary<int, string> results = woTarget.Db.Dictionary<int, string>(Query);");

            SetSnippet(sb.ToString());
        }

        private void CrearSelectPairValues_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\t\t\tvar Query = woTarget.Db.From<{currentModel.Id}>()");
            sb.AppendLine($"\t\t\t\t\t\t\t.GroupBy(x => x.Propiedad)");
            sb.AppendLine($"\t\t\t\t\t\t\t.Select(x => new {{ x.Propiedad, Count = Sql.Count(\"*\") }})");
            sb.AppendLine($"\t\t\t\t\t\t\t.OrderByDescending(x => x.Propiedad);");
            sb.AppendLine($"\t\t\tvar results = woTarget.Db.KeyValuePairs<string, int>(Query);");

            SetSnippet(sb.ToString());
        }

        private void CrearSelectSQL_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\t\t\tList<{currentModel.Id}> results = db.SqlList<{currentModel.Id}>(");
            sb.AppendLine($"\t\t\t\t\t\t\t\"SELECT * FROM {currentModel.Id} WHERE Id < @Id\", new {{ Id = 50 }});");
            SetSnippet(sb.ToString());
        }

        private void CrearSelectSQLScalar_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tint result = db.SqlScalar<int>(\"SELECT COUNT(*) FROM modelo.Id WHERE Id < 50\");");
        }

        private void CrearSelectSQLExecute_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\twoTarget.Db.ExecuteSql(\"UPDATE {currentModel.Id} SET xxx = yyyy\");");
        }

        private void CrearSelectJoinImplicit_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\t\t\tvar Query = woTarget.Db.From<{currentModel.Id}>() // Hace el Join por la relación implícita ");
            sb.AppendLine($"\t\t\t\t\t.Join<Tabla2>();");
            sb.AppendLine($"\t\t\tvar result = woTarget.Db.Select(Query);");

            SetSnippet(sb.ToString());
        }
        private void CrearSelectJoinExplicit_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\t\t\tvar explicitJoin = woTarget.Db.Select(woTarget.Db.From<{currentModel.Id}>() // Se indica el Join de manera explicita ");
            sb.AppendLine($"\t\t\t\t\t.Join<Tabla2>(({currentModel.Id}, Tabla2) => {currentModel.Id}.Id == Tabla2.{currentModel.Id}Id)");
            sb.AppendLine($"\t\t\t\t\t.Where<{currentModel.Id}>(x => x.Propiedad == \"XXXXX\"));");

            SetSnippet(sb.ToString());
        }

        private void CrearSelectStatement_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"\t\t\tvar Query = woTarget.Db.From<{currentModel.Id}> ()");
            sb.AppendLine($"\t\t\t\t\t\t\t.Where(x => x.Propiedad < 50);");
            sb.AppendLine($"\t\t\tstring s = Query.ToSelectStatement(); // SQL Generado");
            SetSnippet(sb.ToString());
        }

        private void CrearSelectLastest_Click(object sender, EventArgs e)
        {
            SetSnippet($"\t\t\tstring s = woTarget.Db.GetLastSql();");
        }

        private void PostPatch_Click(object sender, EventArgs e)
        {
            if (SetSnippet == null)
                return;

            if ((treeModelos.FocusedNode.IsNull()) || (treeModelos.FocusedNode.Tag.IsNull()))
                return;

            if (treeModelos.FocusedNode.Tag is Modelo)
                PostPatch_Modelo();
            else
                PostPatch_Transicion((sender as DXMenuItem).Tag != null ? (sender as DXMenuItem).Tag.ToString() : string.Empty);
        }

        private void PostPatch_Modelo()
        {

            Modelo modelo = (treeModelos.FocusedNode.Tag as Modelo);

            if (modelo.SubTipoModelo == WoSubTypeModel.BackGroudTask)
                PostPatch_BackGround(modelo);
            else
                Get_Servicio(modelo, $"var o{modelo.ScriptRequest.ResponseId} = {modelo.Id}.Get(woTarget,", ")");
        }

        private string GetDefaultValue(ModeloColumna columna, string ModeloBase)
        {
            string Constante = "null";

            if ((columna.Id == "Id") ||
                (columna.Id == WoConst.WOSERIE))
                Constante = @"""""";
            else if (columna.Id == WoConst.WOPERIODOID)
                Constante = @"DateTime.Today.Month.ToString(""D2"")";
            else if (columna.Id == "CfgMonedaId")
                Constante = @"""MXN""";
            else if (columna.Id == "TipoCambio")
                Constante = @"1.0m";
            else if (columna.Id == WoConst.WOGUID)
                Constante = @$"""{Guid.NewGuid().ToString()}""";
            else if (columna.Id == WoConst.WOORIGEN)
                Constante = $"e{ModeloBase}_{columna.Id}.Manual";
            else if (!columna.Default.IsNullOrStringEmpty())
                Constante = columna.Default;
            else if (!columna.Nulo)
            {
                switch (columna.TipoColumna)
                {
                    case WoTypeColumn.Smallint:
                    case WoTypeColumn.Integer:
                    case WoTypeColumn.Long: Constante = "0"; break;
                    case WoTypeColumn.Decimal: Constante = "0.0m"; break;
                    case WoTypeColumn.Double: Constante = "0.0"; break;
                    case WoTypeColumn.Boolean: Constante = "false"; break;
                    case WoTypeColumn.DateTime: Constante = @"DateTime.Now"; break;
                    case WoTypeColumn.Date: Constante = @"DateTime.Today"; break;
                    case WoTypeColumn.Blob: Constante = @"new byte[1]()"; break;
                    case WoTypeColumn.EnumInt:
                    case WoTypeColumn.EnumString: Constante = $"e{ModeloBase}_{columna.Id}.XXX"; break;
                    case WoTypeColumn.String: Constante = "\"...\""; break;
                }
            }
            else
                Constante = "null";

            return Constante;

        }


        private void Get_Servicio(Modelo modelo, string Prefijo, string SubFijo)
        {

            string Servicio = "Get";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            //sb.AppendLine($"\t\t\tvar o{modelo.ScriptRequest.ResponseId} = {modelo.Id}.{Servicio}(woTarget, new {modelo.Id}()");

            sb.AppendLine($"\t\t\t{Prefijo} new {modelo.Id}()");
            sb.AppendLine("\t\t\t{");

            bool bInicio = true;
            foreach (var col in modelo.Columnas)
            {
                string Constante = GetDefaultValue(col, modelo.Id);

                if (!bInicio)
                    sb.AppendLine(",");
                bInicio = false;

                sb.Append($"\t\t\t    {col.Id} = {Constante}");
            }

            //sb.AppendLine("\r\n\t\t\t});");
            sb.AppendLine($"\r\n\t\t\t}}{SubFijo};");

            SetSnippet(sb.ToString());
        }

        private void PostPatch_BackGround(Modelo modelo)
        {

            string Servicio = "DoBackGround";

            StringBuilder sb = new StringBuilder();


            sb.AppendLine();
            sb.AppendLine($"\t\t\tstring BackGroundId = {modelo.Id}.{Servicio}(woTarget, new {modelo.Id}()");
            sb.AppendLine("\t\t\t{");



            bool bInicio = true;
            foreach (var col in modelo.Columnas)
            {
                string Constante = GetDefaultValue(col, modelo.Id);

                if (!bInicio)
                    sb.AppendLine(",");
                bInicio = false;

                sb.Append($"\t\t\t    {col.Id} = {Constante}");
            }

            sb.AppendLine("\r\n\t\t\t});");
            sb.AppendLine();
            sb.AppendLine(string.Format(
@"			WoBackGround task = null; 

			for(int i=0;;i++){{
					Thread.Sleep(2000);
					task = JsonConvert.DeserializeObject<WoBackGround>(
						woTarget.Send<string>(new {0}BackGround()
						{{
							BackGroundId = BackGroundId
						}})
					);
				
					if(task.IsCompleted)
						break;
				}}
			
				{1} o{1} = null;			
				if(!task.IsError)				
					try {{
						o{1} = {0}.BackGroundResult(woTarget, new {0}BackGround()
						{{
							BackGroundId = BackGroundId
						}});
					}}
					catch(Exception ex)
					{{
				        Uth.LogError(""Error "" + ex.Message);
				        return false;
					}}

		        if(o{1}.NumeroErrores > 0) 
		        {{
			        Uth.LogError(o{1}.Error);
			        return false;
		        }}
			
			    if(task.IsError) 
			    {{
				    Uth.LogError(""Error "" + task.Error);
				    return false;
			    }}

			    Uth.Log(""Correcto "" + oWoResponseGeneric.Dump());
",
            modelo.Id, modelo.ScriptRequest.ResponseId));

            SetSnippet(sb.ToString());
        }


        private void PostPatch_Transicion(string Servicio)
        {

            Tuple<Modelo, Transicion> t = (
                treeModelos.FocusedNode.Tag as Tuple<Modelo, Transicion>
            );

            Modelo modelo = t.Item1;
            Transicion transicion = t.Item2;

            //string Servicio;
            //if (transicion.EstadoInicial == 0)
            //    Servicio = "Post";
            //else if (transicion.EstadoFinal == 600)
            //    Servicio = "SoftDelete";
            //else
            //    Servicio = "Patch";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"\t\t\tvar o{modelo.Id} = {modelo.Id}.{Servicio}(woTarget, new {modelo.Id}{transicion.Id}()");
            sb.AppendLine("\t\t\t{");

            bool bInicio = true;
            foreach (var dto in transicion.DTO.Columnas)
            {
                var col = modelo.Columnas.Where(f => f.Id == dto).FirstOrDefault();

                if (col == null)
                    continue;

                if ((col.Id == "WoState") ||
                    (col.Id == WoConst.ROWVERSION))
                    continue;

                string Constante = string.Empty;

                if (col.EsColeccion)
                {
                    string Ind1 = "\r\n\t\t\t\t\t";
                    string Ind2 = "\r\n\t\t\t\t";

                    if (col.Id == WoConst.WOPERIODOID)
                        Constante = @"DateTime.Today.Month.ToString(""D2"")";
                    else if (col.Id == "CfgMonedaId")
                        Constante = @"""MXN""";
                    else if (col.Id == "TipoCambio")
                        Constante = @"1.0m";
                    else if (col.Id == WoConst.WOGUID)
                        Constante = @$"""{Guid.NewGuid().ToString()}""";
                    else
                    {
                        switch (col.TipoColumna)
                        {
                            case WoTypeColumn.Smallint: Constante = $"new List<short>() {{{Ind1}0,{Ind1}1{Ind2}}}"; break;
                            case WoTypeColumn.Integer: Constante = $"new List<int>() {{{Ind1}0,{Ind1}1{Ind2}}}"; break;
                            case WoTypeColumn.Long: Constante = $"new List<long>() {{{Ind1}0,{Ind1}1{Ind2}}}"; break;
                            case WoTypeColumn.Decimal: Constante = $"new List<decimal>() {{{Ind1}0.0m,{Ind1}1.0m{Ind2}}}"; break;
                            case WoTypeColumn.Double: Constante = $"new List<decimal>() {{{Ind1}0.0,{Ind1}1.0{Ind2}}}"; break;
                            case WoTypeColumn.Boolean: Constante = $"new List<bool>() {{{Ind1}false,{Ind1}false{Ind2}}}"; break;
                            case WoTypeColumn.DateTime:
                            case WoTypeColumn.Date:
                                Constante = $"new List<DateTime>() {{{Ind1}DateTime.Today,{Ind1}DateTime.Today{Ind2}}}"; break;
                            case WoTypeColumn.Blob:
                                Constante = $"new List<byte[]>() {{\r\n\t\tnew byte[](),\r\n\t\tnew byte[](){Ind2}}}"; break;
                            case WoTypeColumn.EnumInt:
                            case WoTypeColumn.EnumString:
                                {
                                    string ModeloBase = modelo.Id;
                                    if (!col.ModeloId.IsNullOrEmpty())
                                        ModeloBase = col.ModeloId;

                                    Constante = $"new List<e{ModeloBase}_{col.Id.Substring(0, col.Id.Length - 3)}>() {{{Ind1}e{ModeloBase}_{col.Id.Substring(0, col.Id.Length - 3)}.XXX,{Ind1}e{ModeloBase}_{col.Id.Substring(0, col.Id.Length - 3)}.XXX{Ind2}}}";
                                    break;
                                }
                            case WoTypeColumn.Complex: Constante = $"new List<{modelo.Id}()>[]() {{\\r\\n\\t\\tnew {modelo.Id}(),\\r\\n\\t\\tnew {modelo.Id}{Ind2}}}"; break;
                            default:
                                Constante =
                                    Constante = $"new List<string>() {{{Ind1}\"...\",{Ind1}\"...\"{Ind2}}}"; break;
                        }
                    }
                }
                else
                {
                    string ModeloBase = modelo.Id;
                    if (!col.ModeloId.IsNullOrEmpty())
                        ModeloBase = col.ModeloId;
                    Constante = GetDefaultValue(col, ModeloBase);
                }

                if (!bInicio)
                    sb.AppendLine(",");
                bInicio = false;

                sb.Append($"\t\t\t    {dto} = {Constante}");
            }

            var estado = modelo.Diagrama.Estados.Where(g => g.NumId == transicion.EstadoFinal).FirstOrDefault();
            if (estado != null)
            {
                sb.AppendLine(",");
                sb.Append($"\t\t\t    WoState = e{modelo.Id}_WoState.{estado.Id}");
            }

            if (transicion.EstadoInicial != 0)
            {
                sb.AppendLine(",");
                sb.Append("\t\t\t    RowVersion = 1");
            }

            foreach (var dtoColllecion in transicion.DTO.Colleccion)
            {
                if ((dtoColllecion.Insertar) || (dtoColllecion.Actualizar) || (dtoColllecion.Borrar))
                {
                    var modloc = principal.ModeloCol.Modelos.Where(m => m.Id == dtoColllecion.ModeloId).FirstOrDefault();

                    if (modloc == null)
                        continue;

                    sb.AppendLine(",");
                    sb.AppendLine($"\t\t\t    {dtoColllecion.ModeloId}Col = new[] {{");
                    sb.AppendLine($"\t\t\t        new {dtoColllecion.ModeloId}{transicion.Id}");
                    sb.AppendLine("\t\t\t        {");

                    bool bInicioLoc = true;
                    foreach (var dtocol in dtoColllecion.Columnas)
                    {
                        var col = modloc.Columnas.Where(f => f.Id == dtocol).FirstOrDefault();

                        if (col == null)
                            continue;

                        string Constante = string.Empty;

                        if (col.EsColeccion)
                        {
                            string Ind1 = "\r\n\t\t\t\t\t";
                            string Ind2 = "\r\n\t\t\t\t";

                            switch (col.TipoColumna)
                            {
                                case WoTypeColumn.Smallint: Constante = $"new List<short>() {{{Ind1}0,{Ind1}1{Ind2}}}"; break;
                                case WoTypeColumn.Integer: Constante = $"new List<int>() {{{Ind1}0,{Ind1}1{Ind2}}}"; break;
                                case WoTypeColumn.Long: Constante = $"new List<long>() {{{Ind1}0,{Ind1}1{Ind2}}}"; break;
                                case WoTypeColumn.Decimal: Constante = $"new List<decimal>() {{{Ind1}0.0m,{Ind1}1.0m{Ind2}}}"; break;
                                case WoTypeColumn.Double: Constante = $"new List<decimal>() {{{Ind1}0.0,{Ind1}1.0{Ind2}}}"; break;
                                case WoTypeColumn.Boolean: Constante = $"new List<bool>() {{{Ind1}false,{Ind1}false{Ind2}}}"; break;
                                case WoTypeColumn.DateTime:
                                case WoTypeColumn.Date:
                                    Constante = $"new List<DateTime>() {{{Ind1}DateTime.Today,{Ind1}DateTime.Today{Ind2}}}"; break;
                                case WoTypeColumn.Blob:
                                    Constante = $"new List<byte[]>() {{\r\n\t\tnew byte[](),\r\n\t\tnew byte[](){Ind2}}}"; break;
                                case WoTypeColumn.EnumInt:
                                case WoTypeColumn.EnumString:
                                    Constante = $"new List<e{modelo.Id}_{col.Id.Substring(0, col.Id.Length - 3)}>() {{{Ind1}e{modelo.Id}_{col.Id.Substring(0, col.Id.Length - 3)}.XXX,{Ind1}e{modelo.Id}_{col.Id.Substring(0, col.Id.Length - 3)}.XXX{Ind2}}}"; break;
                                default:
                                    Constante =
                                        Constante = $"new List<string>() {{{Ind1}\"...\",{Ind1}\"...\"{Ind2}}}"; break;
                            }
                        }
                        else
                        {
                            Constante = GetDefaultValue(col, modelo.Id);
                        }

                        if (!bInicioLoc)
                            sb.AppendLine(",");
                        bInicioLoc = false;

                        sb.Append($"\t\t\t            {dtocol} = {Constante}");
                    }
                    sb.AppendLine();
                    sb.AppendLine("\t\t\t        }");
                    sb.AppendLine("\t\t\t    }");
                }
            }

            sb.AppendLine("\r\n\t\t\t});");

            SetSnippet(sb.ToString());
        }

        private void ProcesaColecciones(StringBuilder sb, ModeloColumna col)
        {
            /*
            InvAjusteAltaDet = new[] {
                        new InvAjusteAltaDet
                        {
                            Renglon = 1,
                            ProductoId = "P1",
                            Cantidad = 10,
                            Cuenta = "100.100"
                        },
            */

            var modloc = principal.ModeloCol.Modelos.Where(m => m.Id == col.ModeloId).FirstOrDefault();

            if (modloc == null)
                return;

            sb.AppendLine(",");
            sb.AppendLine($"    {col.ModeloId} = new[] {{");
            sb.AppendLine($"        new {col.ModeloId}");
            sb.AppendLine("        {");

            bool bInicioLoc = true;
            foreach (var colloc in modloc.Columnas)
            {
                string Constante = GetDefaultValue(col, modloc.Id);
                if (!bInicioLoc)
                    sb.AppendLine(",");
                bInicioLoc = false;

                sb.Append($"            {colloc.Id} = {Constante}");
            }
            sb.AppendLine();
            sb.AppendLine("        }");
            sb.AppendLine("    }");
        }

        private void grdColumnaView_DoubleClick(object sender, EventArgs e)
        {
            DataRow drRow = grdColumnaView.GetFocusedDataRow();
            if (drRow != null)
            {
                SetSnippet(drRow["Columna"].ToString());
            }

        }

        private void treeModelos_DoubleClick(object sender, EventArgs e)
        {
            if (treeModelos.FocusedNode.Tag is Modelo)
            {
                SetSnippet((treeModelos.FocusedNode.Tag as Modelo).Id);
            }
            else
            {
                Tuple<Modelo, Transicion> t = (
                    treeModelos.FocusedNode.Tag as Tuple<Modelo, Transicion>
                );

                SetSnippet(t.Item1.Id + t.Item2.Id);
            }
        }

        private void tabExplorador_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == tabOData)
                DoODataModel();
        }



        private void DoODataModel()
        {

            grdOData.DataSource = null;
            grdODataView.Columns.Clear();
            ODataCurrentTop = 0;
            ODataBestFit = false;
            ODataNextPage = true;
            ODataOrderBy = string.Empty;
            ODataFilter = string.Empty;


            if (ModeloSeleccionado() == null)
                return;

            var TipoModelo = ModeloSeleccionado().TipoModelo;

            if (
                (TipoModelo == WoTypeModel.Response)
                || (TipoModelo == WoTypeModel.Request)
                || (TipoModelo == WoTypeModel.Complex)
                || (TipoModelo == WoTypeModel.Interface)
                || (TipoModelo == WoTypeModel.Class)
                )
            {
                ODataLastModel = string.Empty;
                return;
            }

            string modelo = ModeloSeleccionado().Id;
            ODataLastModel = ModeloSeleccionado().Id;


            if (Proyecto.getInstance().ParConexion.proc == null)
                return;

            if (assemblyModelData == null)
            {
                assemblyModelData = Assembly.LoadFile(
                    Proyecto.getInstance().FileWooWWebClientdll
                );
                Proyecto.getInstance().AssemblyModelCargado = true;
            }

            ODataType = assemblyModelData.GetType($"WooW.Model.{modelo}");
            ODataTypeList = assemblyModelData.GetType($"WooW.DTO.{modelo}List");

            LogIn();

            virtualServerModeSource = new DevExpress.Data.VirtualServerModeSource(this.components);
            virtualServerModeSource.RowType = ODataType;
            virtualServerModeSource.ConfigurationChanged += new System.EventHandler<DevExpress.Data.VirtualServerModeRowsEventArgs>(this.virtualServerModeSource_ConfigurationChanged);
            virtualServerModeSource.MoreRows += new System.EventHandler<DevExpress.Data.VirtualServerModeRowsEventArgs>(this.virtualServerModeSource_MoreRows);
            virtualServerModeSource.GetUniqueValues += new System.EventHandler<DevExpress.Data.VirtualServerModeGetUniqueValuesEventArgs>(this.virtualServerModeSource_GetUniqueValues);

            grdOData.DataSource = virtualServerModeSource;
            if (grdODataView.Columns.Count > 0)
            {
                grdODataView.Columns[0].Fixed = FixedStyle.Left;
                grdODataView.BestFitColumns();
            }

        }

        private void LogIn()
        {
            Proyecto proyecto = Proyecto.getInstance();

            if (woTarget == null)
            {
                woTarget = new JsonApiClient("https://localhost:5101");

                var AuthenticateResponse = woTarget.Post<AuthenticateResponse>(
                    new Authenticate
                    {
                        provider = "credentials", // CredentialsAuthProvider.Name, //= credentials
                        UserName = proyecto.ParConexion.Usuario,
                        Password = proyecto.ParConexion.Password,
                        RememberMe = true,
                    }
                );

                woTarget.BearerToken = AuthenticateResponse.BearerToken;
                woTarget.GetHttpClient().DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticateResponse.BearerToken);

                var WoInstanciaUdnResponse = woTarget.Post(
                    new WoInstanciaUdnAsignar
                    {
                        Instance = proyecto.ParConexion.Instance,
                        Udn = proyecto.ParConexion.Udn,
                        Year = proyecto.ParConexion.Year,
                        InstanceType = proyecto.ParConexion.InstanceType
                    }
                );
            }
        }


        private void virtualServerModeSource_ConfigurationChanged(object sender, VirtualServerModeRowsEventArgs e)
        {


            if (e.ConfigurationInfo.SortInfo != null)
            {
                string sSort = string.Empty;

                foreach (ServerModeOrderDescriptor order in e.ConfigurationInfo.SortInfo)
                {
                    if (!string.IsNullOrEmpty(sSort))
                        sSort += ", ";

                    sSort += order.SortPropertyName;

                    if (order.IsDesc)
                        sSort += " DESC";
                }

                ODataOrderBy = sSort;
            }

            if (e.ConfigurationInfo.Filter != null)
                ODataFilter = ODataHelpers.FormatODataQueryString(e.ConfigurationInfo.Filter);

            ODataCurrentTop = 0;
            ODataNextPage = true;
        }

        private void virtualServerModeSource_MoreRows(object sender, VirtualServerModeRowsEventArgs e)
        {
            e.RowsTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                bool moreRows;
                var enumerator = e.UserData as IEnumerator;
                ICollection nextBatch = GetItems();
                if (nextBatch == null)
                {
                    nextBatch = null; // new List();
                    moreRows = false;
                }
                else
                {
                    moreRows = true;
                }

                return new VirtualServerModeRowsTaskResult(nextBatch, moreRows, enumerator);
            }, e.CancellationToken);

        }

        private void virtualServerModeSource_GetUniqueValues(object sender, VirtualServerModeGetUniqueValuesEventArgs e)
        {
            //e.UniqueValuesTask = new System.Threading.Tasks.Task<object[]>(() =>
            //{
            //    switch (e.ValuesPropertyName)
            //    {
            //        case "ModelPrice":
            //            return new object[] { 15000m, 150000m };
            //        case "Discount":
            //            return new object[] { 0.00, 0.05, 0.10, 0.15 };
            //        case "SalesDate":
            //            DateTime today = TutorialConstants.Today;
            //            DateTime sevenYearsAgo = new DateTime(today.Year - 7, 1, 1);
            //            int totalDays = (int)Math.Ceiling(today.Subtract(sevenYearsAgo).TotalDays);
            //            object[] days = new object[totalDays];
            //            for (int i = 0; i < days.Length; i++)
            //                days[i] = sevenYearsAgo.AddDays(i);
            //            return days;
            //        case "Trademark":
            //            return models.Select(m => m.Trademark).Distinct().Cast<object>().ToArray();
            //        case "Name":
            //            return models.Select(m => m.Name).Distinct().Cast<object>().ToArray();
            //        case "Modification":
            //            return models.Select(m => m.Modification).Distinct().Cast<object>().ToArray();
            //        default:
            //            return null;
            //    }
            //}, e.CancellationToken);
        }


        private ICollection GetItems()
        {

            if ((ODataType == null) || (ODataTypeList == null))
                return null;

            if (!ODataNextPage)
                return null;

            LogIn();

            var modelList = Activator.CreateInstance(ODataTypeList);

            if (modelList == null)
                return null;

            var ODataRequest = (modelList as IWoODataRequest);

            ODataRequest.top = ODataPageSize;
            ODataRequest.skip = ODataCurrentTop;
            ODataRequest.orderby = ODataOrderBy;
            ODataRequest.filter = ODataFilter;


            MethodInfo ODataList = ODataType.GetMethod("List");

            if (ODataList == null)
                return null;

            object ODataResponse;
            ICollection col;

            try
            {
                ODataResponse = ODataList.Invoke(null, new object[] { woTarget, modelList });
                Type type = ODataResponse.GetType();
                PropertyInfo info = type.GetProperty("Value");
                if (info == null) 
                    return null; 
                var locValue = info.GetValue(ODataResponse, null);
                col = (ICollection)locValue;
                ODataCurrentTop += col.Count;
                ODataNextPage = col.Count >= ODataPageSize;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.IsNullOrStringEmpty())
                    XtraMessageBox.Show(ex.Message, "Verifique...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    XtraMessageBox.Show(ex.InnerException.Message, "Verifique...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }


            return col;
        }

        private void grdColumnaView_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GridView view = sender as GridView;
            BaseEdit edit = view.ActiveEditor;
            if (edit != null && view.FocusedRowHandle == GridControl.AutoFilterRowHandle)
            {
                edit.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
                edit.Properties.EditValueChangedDelay = 500;
            }
        }

        private void grdODataView_DoubleClick(object sender, EventArgs e)
        {
            object cell = grdODataView.GetFocusedValue();
            if (cell != null)
            {
                var col = grdODataView.FocusedColumn;
                if (col.ColumnType.IsEnum)
                    SetSnippet($"{col.ColumnType.Name}.{cell.ToString()}");
                else if (col.ColumnType == typeof(string))
                    SetSnippet($"\"{cell.ToString()}\"");
                else if (col.ColumnType == typeof(DateTime))
                {
                    DateTime dateTime = (DateTime)cell;
                    SetSnippet($"new DateTime({dateTime.Year}, {dateTime.Month}, {dateTime.Day})");
                }
                else if (col.ColumnType == typeof(bool))
                    SetSnippet(cell.ToString().ToLower());
                else if (col.ColumnType == typeof(decimal))
                    SetSnippet($"{cell.ToString()}m");
                else
                    SetSnippet(cell.ToString());

            }
        }

        private void grdODataView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (grdODataView.GetFocusedRow() == null)
                return;

            currentModel = ModeloSeleccionado();
            if (currentModel == null)
                return;

            e.Menu.Items.Add(
                    new DXMenuItem("Get", CrearODataGet_Click));
            e.Menu.Items.Add(
                    new DXMenuItem("GetCheck", CrearODataGet_Click));
            e.Menu.Items.Add(
                    new DXMenuItem("Single", CrearODataGet_Click));
            e.Menu.Items.Add(
                    new DXMenuItem("SingleCheck", CrearODataGet_Click));

            foreach (var transicion in currentModel.Diagrama.Transiciones)
            {
                string Verb = string.Empty;
                if (transicion.EstadoInicial == 0)
                    Verb = "Post ";
                else
                    Verb = "Put ";

                var menu = new DXMenuItem($"{Verb}{transicion.Id}", CrearODataPostPatch_Click);
                menu.Tag = transicion;
                e.Menu.Items.Add(menu);

                if (transicion.EstadoInicial != 0)
                {
                    Verb = "Patch ";
                    menu = new DXMenuItem($"{Verb}{transicion.Id}", CrearODataPostPatch_Click);
                    menu.Tag = transicion;
                    e.Menu.Items.Add(menu);
                }
            }
        }

        private void CrearODataGet_Click(object sender, EventArgs e)
        {
            object data = grdODataView.GetFocusedRow();
            var modelAccesor = TypeAccessor.Create(data.GetType());
            string Id = modelAccesor[data, "Id"].ToString();

            string[] Tokens = (sender as DXMenuItem).Caption.Split(' ');
            string verb = Tokens[Tokens.Length - 1];

            SnippetGet(verb, Id);
        }

        private void CrearODataPostPatch_Click(object sender, EventArgs e)
        {
            object dataRow = grdODataView.GetFocusedRow();
            var modelAccesor = TypeAccessor.Create(dataRow.GetType());

            // Ejecutar Get
            MethodInfo[] methodGets = ODataType.GetMethods().Where(x => x.Name == "Get").ToArray();

            if (methodGets.Length == 0)
                return;

            MethodInfo methodGet = null;

            foreach (var method in methodGets)
            {
                if (method.GetParameters().Where(x => x.Name == "Id").FirstOrDefault() != null)
                {
                    methodGet = method;
                    break;
                }
            }

            if (methodGet == null)
                return;


            string Id = modelAccesor[dataRow, "Id"].ToString();

            object data = methodGet.Invoke(null, new object[] { woTarget, Id });

            Modelo modelo = currentModel;
            Transicion transicion = (sender as DXMenuItem).Tag as Transicion;

            string Servicio;
            if (transicion.EstadoInicial == 0)
                Servicio = "Post";
            else if (transicion.EstadoFinal == 600)
                Servicio = "SoftDelete";
            else
            {
                if ((sender as DXMenuItem).Caption.StartsWith("Patch"))
                    Servicio = "Patch";
                else
                    Servicio = "Put";
            }


            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine($"\t\t\tvar o{modelo.Id} = {modelo.Id}.{Servicio}(woTarget, new {modelo.Id}{transicion.Id}()");
            sb.AppendLine("\t\t\t{");

            bool bInicio = true;
            foreach (var dto in transicion.DTO.Columnas)
            {
                var col = modelo.Columnas.Where(f => f.Id == dto).FirstOrDefault();

                if (col == null)
                    continue;

                if ((col.Id == "WoState") ||
                    (col.Id == WoConst.ROWVERSION))
                    continue;

                string Constante = string.Empty;

                if (col.EsColeccion)
                {
                    ICollection oDatoList = (ICollection)modelAccesor[data, col.Id];

                    // Todo Corregir Colecciones

                    string Ind1 = "\r\n\t\t\t\t\t";
                    string Ind2 = "\r\n\t\t\t\t";

                    switch (col.TipoColumna)
                    {
                        case WoTypeColumn.Smallint: Constante += $"new List<short>() {{{ToStringListShort(oDatoList)}}}"; break;
                        case WoTypeColumn.Integer: Constante += $"new List<int>() {{{ToStringListInt(oDatoList)}}}"; break;
                        case WoTypeColumn.Long: Constante += $"new List<long>() {{{ToStringListLong(oDatoList)}}}"; break;
                        case WoTypeColumn.Decimal: Constante += $"new List<decimal>() {{{ToStringListDecimal(oDatoList)}}}"; break;
                        case WoTypeColumn.Double: Constante += $"new List<decimal>() {{{ToStringListDouble(oDatoList)}}}"; break;
                        case WoTypeColumn.Boolean: Constante += $"new List<bool>() {{{ToStringListBool(oDatoList)}}}"; break;
                        case WoTypeColumn.DateTime:
                        case WoTypeColumn.Date:
                            Constante += $"new List<DateTime>() {{{ToStringListDateTime(oDatoList)}}}"; break;
                        case WoTypeColumn.Blob:
                            Constante += $"new List<byte[]>() {{\r\n\t\tnew byte[](),\r\n\t\tnew byte[](){Ind2}}}"; break;
                        case WoTypeColumn.EnumInt:
                        case WoTypeColumn.EnumString:
                            {
                                string ModeloBase = modelo.Id;
                                if (!col.ModeloId.IsNullOrEmpty())
                                    ModeloBase = col.ModeloId;
                                string EnumType = $"e{ModeloBase}_{col.Id.Substring(0, col.Id.Length - 3)}";
                                Constante += $"new List<{EnumType}>() {{{ToStringListEnum(EnumType, oDatoList)}}}";
                                break;
                            }
                        case WoTypeColumn.Complex: Constante = $"new List<{modelo.Id}()>[]() {{\\r\\n\\t\\tnew {modelo.Id}(),\\r\\n\\t\\tnew {modelo.Id}{Ind2}}}"; break;
                        default:
                            Constante =
                                Constante += $"new List<string>() {{{ToStringListString(oDatoList)}}}"; break;
                    }
                }
                else
                {
                    object oDato = modelAccesor[data, col.Id];

                    if (oDato != null)
                    {
                        switch (col.TipoColumna)
                        {
                            case WoTypeColumn.Smallint:
                            case WoTypeColumn.Integer:
                            case WoTypeColumn.Long: Constante = oDato.ToString(); break;
                            case WoTypeColumn.Decimal: Constante = oDato.ToString() + "m"; break;
                            case WoTypeColumn.Double: Constante = oDato.ToString(); break;
                            case WoTypeColumn.Boolean: Constante = oDato.ToString().ToLower(); break;
                            case WoTypeColumn.DateTime:
                            case WoTypeColumn.Date: Constante = $"new DateTime({oDato.ToDateTime().Year}, {oDato.ToDateTime().Month}, {oDato.ToDateTime().Day})"; break;
                            case WoTypeColumn.Blob: Constante = @"new byte[1]()"; break;
                            case WoTypeColumn.EnumInt:
                            case WoTypeColumn.EnumString:
                                {
                                    string ModeloBase = modelo.Id;
                                    if (!col.ModeloId.IsNullOrEmpty())
                                        ModeloBase = col.ModeloId;
                                    Constante = $"e{ModeloBase}_{col.Id}.{oDato.ToString()}"; break;
                                }
                            case WoTypeColumn.Complex: Constante = $"new {modelo.Id}() {{ }}"; break;
                            default:
                                Constante = $"\"{oDato.ToString()}\""; break;
                        }
                    }
                    else
                        Constante = "null";
                }

                if (!bInicio)
                    sb.AppendLine(",");
                bInicio = false;

                sb.Append($"\t\t\t    {dto} = {Constante}");
            }

            var estado = modelo.Diagrama.Estados.Where(g => g.NumId == transicion.EstadoFinal).FirstOrDefault();
            if (estado != null)
            {
                sb.AppendLine(",");
                sb.Append($"\t\t\t    WoState = e{modelo.Id}_WoState.{estado.Id}");
            }

            if (transicion.EstadoInicial != 0)
            {
                sb.AppendLine(",");
                try
                {
                    sb.Append("\t\t\t    RowVersion = " + modelAccesor[dataRow, WoConst.ROWVERSION].ToString());
                }
                catch
                {
                    sb.Append("\t\t\t    RowVersion = 1");
                }
            }

            foreach (var dtoColllecion in transicion.DTO.Colleccion)
            {
                if ((dtoColllecion.Insertar) || (dtoColllecion.Actualizar) || (dtoColllecion.Borrar))
                {
                    var modloc = principal.ModeloCol.Modelos.Where(m => m.Id == dtoColllecion.ModeloId).FirstOrDefault();

                    if (modloc == null)
                        continue;

                    ICollection oDatoList = (ICollection)modelAccesor[data, dtoColllecion.ModeloId + "Col"];

                    if (oDatoList == null)
                        continue;

                    TypeAccessor ItemAccesor = null;

                    sb.AppendLine(",");
                    sb.AppendLine($"\t\t\t    {dtoColllecion.ModeloId}Col = new[] {{");

                    bool bInicio2 = true;

                    foreach (var ItemList in oDatoList)
                    {
                        if (ItemAccesor == null)
                            ItemAccesor = TypeAccessor.Create(ItemList.GetType());

                        if (!bInicio2)
                        {
                            sb.AppendLine();
                            sb.AppendLine("\t\t\t        },");
                        }
                        bInicio2 = false;

                        sb.AppendLine($"\t\t\t        new {dtoColllecion.ModeloId}{transicion.Id}");
                        sb.AppendLine("\t\t\t        {");

                        bool bInicioLoc = true;
                        foreach (var dtocol in dtoColllecion.Columnas)
                        {
                            var col = modloc.Columnas.Where(f => f.Id == dtocol).FirstOrDefault();

                            if (col == null)
                                continue;

                            object oDato = ItemAccesor[ItemList, col.Id];

                            string Constante = string.Empty;

                            if (col.EsColeccion)
                            {
                                string Ind1 = "\r\n\t\t\t\t\t";
                                string Ind2 = "\r\n\t\t\t\t";

                                ICollection oDatoListLoc = (ICollection)oDato;

                                switch (col.TipoColumna)
                                {
                                    case WoTypeColumn.Smallint: Constante += $"new List<short>() {{{ToStringListShort(oDatoListLoc)}}}"; break;
                                    case WoTypeColumn.Integer: Constante += $"new List<int>() {{{ToStringListInt(oDatoListLoc)}}}"; break;
                                    case WoTypeColumn.Long: Constante += $"new List<long>() {{{ToStringListLong(oDatoListLoc)}}}"; break;
                                    case WoTypeColumn.Decimal: Constante += $"new List<decimal>() {{{ToStringListDecimal(oDatoListLoc)}}}"; break;
                                    case WoTypeColumn.Double: Constante += $"new List<decimal>() {{{ToStringListDouble(oDatoListLoc)}}}"; break;
                                    case WoTypeColumn.Boolean: Constante += $"new List<bool>() {{{ToStringListBool(oDatoListLoc)}}}"; break;
                                    case WoTypeColumn.DateTime:
                                    case WoTypeColumn.Date:
                                        Constante += $"new List<DateTime>() {{{ToStringListDateTime(oDatoListLoc)}}}"; break;
                                    case WoTypeColumn.Blob:
                                        Constante += $"new List<byte[]>() {{\r\n\t\tnew byte[](),\r\n\t\tnew byte[](){Ind2}}}"; break;
                                    case WoTypeColumn.EnumInt:
                                    case WoTypeColumn.EnumString:
                                        {
                                            string ModeloBase = modelo.Id;
                                            if (!col.ModeloId.IsNullOrEmpty())
                                                ModeloBase = col.ModeloId;
                                            string EnumType = $"e{ModeloBase}_{col.Id.Substring(0, col.Id.Length - 3)}";
                                            Constante += $"new List<{EnumType}>() {{{ToStringListEnum(EnumType, oDatoListLoc)}}}";
                                            break;
                                        }
                                    case WoTypeColumn.Complex: Constante = $"new List<{modelo.Id}()>[]() {{\\r\\n\\t\\tnew {modelo.Id}(),\\r\\n\\t\\tnew {modelo.Id}{Ind2}}}"; break;
                                    default:
                                        Constante =
                                            Constante += $"new List<string>() {{{ToStringListString(oDatoListLoc)}}}"; break;
                                }
                            }
                            else
                            {
                                if (!col.Nulo)
                                {
                                    switch (col.TipoColumna)
                                    {
                                        case WoTypeColumn.Smallint:
                                        case WoTypeColumn.Integer:
                                        case WoTypeColumn.Long: Constante = oDato.ToString(); break;
                                        case WoTypeColumn.Decimal: Constante = oDato.ToString() + "m"; break;
                                        case WoTypeColumn.Double: Constante = oDato.ToString(); break;
                                        case WoTypeColumn.Boolean: Constante = oDato.ToString().ToLower(); break;
                                        case WoTypeColumn.DateTime:
                                        case WoTypeColumn.Date: Constante = $"new DateTime({oDato.ToDateTime().Year}, {oDato.ToDateTime().Month}, {oDato.ToDateTime().Day})"; break;
                                        case WoTypeColumn.Blob: Constante = @"new byte[1]()"; break;
                                        case WoTypeColumn.EnumInt:
                                        case WoTypeColumn.EnumString:
                                            {
                                                string ModeloBase = modelo.Id;
                                                if (!col.ModeloId.IsNullOrEmpty())
                                                    ModeloBase = col.ModeloId;
                                                Constante = $"e{ModeloBase}_{col.Id}.{oDato.ToString()}"; break;
                                            }
                                        case WoTypeColumn.Complex: Constante = $"new {modelo.Id}() {{ }}"; break;
                                        default:
                                            Constante = $"\"{oDato.ToString()}\""; break;
                                    }
                                }
                                else
                                    Constante = "null";
                            }

                            if (!bInicioLoc)
                                sb.AppendLine(",");
                            bInicioLoc = false;

                            sb.Append($"\t\t\t            {dtocol} = {Constante}");
                        }
                    }
                }
                sb.AppendLine();
                sb.AppendLine("\t\t\t        }");
                sb.AppendLine("\t\t\t    }");
            }

            sb.AppendLine("\r\n\t\t\t});");

            SetSnippet(sb.ToString());
        }

        private string ToStringListString(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (string i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += $"\"{i}\"";
            }
            return s;
        }
        private string ToStringListShort(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (short i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += i.ToString();
            }
            return s;
        }

        private string ToStringListInt(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (int i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += i.ToString();
            }
            return s;
        }

        private string ToStringListLong(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (long i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += i.ToString();
            }
            return s;
        }

        private string ToStringListDecimal(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (long i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += i.ToString() + "m";
            }
            return s;
        }

        private string ToStringListDouble(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (double i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += i.ToString().ToLower();
            }
            return s;
        }

        private string ToStringListBool(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (bool i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += i.ToString().ToLower();
            }
            return s;
        }

        private string ToStringListDateTime(ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (System.DateTime i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += $"new DateTime({i.Year}, {i.Month}, {i.Day})";
            }
            return s;
        }

        private string ToStringListEnum(string EnumType, ICollection col)
        {
            string s = string.Empty;
            bool bInicio = true;
            foreach (object i in col)
            {
                if (!bInicio)
                    s += ", ";
                bInicio = false;
                s += $"{EnumType}.{i.ToString()}";
            }
            return s;
        }

        //private void grdODataView_FilterExpressionEditorCreated(object sender, DevExpress.XtraGrid.Views.Base.FilterExpressionEditorEventArgs e)
        //{
        //    FileStream fs = new FileStream("test.xml", FileMode.Create);
        //    grdODataView.SaveLayoutToStream(fs);
        //    fs.Close();

        //    //private void simpleButton2_Click(object sender, EventArgs e)
        //    //{
        //    //    gridView1.RestoreLayoutFromXml("test.xml");
        //    //}
        //}

    }
}