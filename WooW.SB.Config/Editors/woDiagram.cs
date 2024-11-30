using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using WooW.Core;
using WooW.Core.Common;

namespace WooW.SB.Config
{

    public delegate void SelectTransicion(string name);

    public partial class woDiagram : DevExpress.XtraEditors.XtraUserControl
    {
        public Proyecto proyecto { get; set; }

        private ModeloDiagrama _currDiagrama;

        public SelectTransicion selectTransicion = null;

        public ModeloDiagrama currDiagrama
        { get { return _currDiagrama; } set { _currDiagrama = value; } }

        private Modelo _currModelo;

        public Modelo currModelo
        { get { return _currModelo; } set { _currModelo = value; } }

        public bool MostrarPermisos { get; set; }
        private eShapeSelection eCurrSelection;
        private Estado _currEstado;
        private Transicion currentTransition;
        private bool bFirstDraw = true;

        public woDiagram()
        {
            InitializeComponent();

            var oStencil = DiagramToolboxRegistrator.Stencils.ToList();
            foreach (DiagramStencil oItem in oStencil)
                DiagramToolboxRegistrator.UnregisterStencil(oItem);

            toolboxControl1.Groups.Clear();
            this.toolboxControl1.OptionsView.MenuButtonCaption = "";

            diagramControl1.OptionsView.GridSize = new SizeF(25, 25);
            diagramControl1.OptionsView.Landscape = true;
            diagramControl1.OptionsView.PageSize = new SizeF(900, 800);

            DataTable dtDTO = new DataTable();
            dtDTO.Columns.Add("Selección", typeof(bool));
            dtDTO.Columns.Add("No_Editar", typeof(bool));
            dtDTO.Columns.Add("Nombre", typeof(string));
            grdDTO.DataSource = dtDTO;

            grdDTOView.ValidateRow += GrdDTOView_ValidateRow;
            grdDTOView.Columns["Nombre"].OptionsColumn.AllowEdit = false;

        }

        private void woDiagram_Load(object sender, EventArgs e)
        {
            RegisterShapes();


            diagramControl1.ZoomIn();
            diagramControl1.ZoomIn();
            diagramControl1.ZoomIn();
        }

        private void GrdDTOView_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            if (diagramControl1.SelectedItems.Count == 0)
                return;

            NoEditarDefault();

            if (e != null)
            {
                DataRow dr = grdDTOView.GetDataRow(e.RowHandle);
                if (dr == null)
                    return;

                if ((dr["Selección"].ToBoolean() == false) &&
                    (dr["No_Editar"].ToBoolean() == true))
                {
                    e.ErrorText = "Para marcar No Editar debe también marcado Seleccionado";
                    e.Valid = false;
                    return;
                }

                e.Valid = true;
            }

            if (diagramControl1.SelectedItems[0] is DiagramShapeTransition)
            {
                DiagramShapeTransition curentObjectSelcted = (
                    diagramControl1.SelectedItems[0] as DiagramShapeTransition
                );

                var Transition = FindTransitionByShape(curentObjectSelcted);
                if (Transition != null)
                {
                    Transition.DTO.Columnas.Clear();
                    Transition.DTO.ColumnasNoEditar.Clear();

                    DataTable dtDTO = (grdDTO.DataSource as DataTable);
                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        if (drRow["Selección"].ToBoolean())
                        {
                            Transition.DTO.Columnas.Add(drRow["Nombre"].ToString());
                            if (drRow["No_Editar"].ToBoolean())
                                Transition.DTO.ColumnasNoEditar.Add(drRow["Nombre"].ToString());
                        }
                    }
                }
            }

        }

        public void MostraSoloDiagrama()
        {
            splDiagrama.PanelVisibility = SplitPanelVisibility.Panel2;
        }

        public void OcultaReglas()
        {
            diagramControl1.OptionsView.ShowRulers = false;
        }

        public void RegisterShapes()
        {
            var oStencil = DiagramToolboxRegistrator.Stencils.ToList();
            foreach (DiagramStencil oItem in oStencil)
                DiagramToolboxRegistrator.UnregisterStencil(oItem);

            var toolEstado = new FactoryItemTool(
                @"Estado",
                () => @"Estado",
                diagram =>
                    new DiagramShapeEstado()
                    {
                        Shape = BasicShapes.Ellipse,
                        Size = new System.Drawing.Size(100, 100),
                        BackgroundId = DiagramThemeColorId.Accent1_3,
                        ForegroundId = DiagramThemeColorId.Black,
                        CanAttachConnectorBeginPoint = true,
                        CanAttachConnectorEndPoint = true,
                        Content = "",
                        TemplateName = @"DD_Entrada"
                    }
            );
            var toolConectCurv = new FactoryItemTool(
                @"Transicion",
                () => @"Transicion",
                diagram =>
                    new DiagramShapeTransition()
                    {
                        Type = DevExpress.Diagram.Core.ConnectorType.Curved,
                        EndArrow = DevExpress.Diagram.Core.ArrowDescriptions.Filled90,
                        Width = 200,
                        Height = 200
                    }
            );

            DiagramControl.ItemTypeRegistrator.Register(typeof(DiagramShape));

            var Input_Stencil = new DiagramStencil(@"customShapes", @"Entradas");
            Input_Stencil.RegisterTool(toolConectCurv);
            Input_Stencil.RegisterTool(toolEstado);

            DiagramToolboxRegistrator.RegisterStencil(Input_Stencil);

            SelectCustomStencilTools();
        }

        private void SelectCustomStencilTools()
        {
            StencilCollection oStencilTools = new StencilCollection();
            oStencilTools.Add(@"customShapes");
            diagramControl1.OptionsBehavior.SelectedStencils = oStencilTools;

            var quickShapes = toolboxControl1.Groups.FirstOrDefault(
                g => g.Caption == @"Quick Shapes"
            );
            if (quickShapes != null)
                quickShapes.Visible = false;

            quickShapes = toolboxControl1.Groups.FirstOrDefault(
                g => g.Caption == @"Formas rápidas"
            );
            if (quickShapes != null)
                quickShapes.Visible = false;
        }

        private PointCollection NewPoints()
        {
            // Version 8 puntos a 70
            float d1 = 0.5f * 0.342f;
            float e1 = 0.5f * (1.0f - 0.9396f);

            return new PointCollection(
                new[]
                {
                    new PointFloat(0.5f + d1, e1), // Parte superior del circulo
                    new PointFloat(1.0f - e1, (0.5f - d1)),
                    new PointFloat(1.0f - e1, (0.5f + d1)),
                    new PointFloat(0.5f + d1, 1.0f - e1),
                    new PointFloat(0.5f - d1, 1.0f - e1), // Abajo
                    new PointFloat(e1, 0.5f + d1),
                    new PointFloat(e1, 0.5f - d1), // Izquierdo
                    new PointFloat(0.5f - d1, e1)
                }
            );
        }

        private void diagramControl1_SelectionChanged(
            object sender,
            DiagramSelectionChangedEventArgs e
        )
        {
            grpDTO.Visible = false;

            if (sender is DiagramControl)
            {
                if ((sender as DiagramControl).SelectedItems.Count == 1)
                {
                    if ((sender as DiagramControl).SelectedItems[0] is DiagramShapeEstado)
                    {
                        DiagramShapeEstado curentObjectSelcted = (
                            (sender as DiagramControl).SelectedItems[0] as DiagramShapeEstado
                        );

                        eCurrSelection = eShapeSelection.Estado;

                        _currEstado = FindStateByShape(curentObjectSelcted);
                        if (_currEstado != null)
                        {
                            currentTransition = null;
                            pgPropiedades.SelectedObject = _currEstado;
                            pgPropiedades.RetrieveFields();
                            grpControl.Text = "Estado";
                        }
                    }
                    else if ((sender as DiagramControl).SelectedItems[0] is DiagramShapeTransition)
                    {
                        grpDTO.Visible = true;

                        DiagramShapeTransition curentObjectSelcted = (
                            (sender as DiagramControl).SelectedItems[0] as DiagramShapeTransition
                        );

                        eCurrSelection = eShapeSelection.Transicion;

                        currentTransition = FindTransitionByShape(curentObjectSelcted);
                        if (currentTransition != null)
                        {
                            _currEstado = null;
                            if (pgPropiedades.SelectedObject != null)
                                pgPropiedades.FocusedRow = pgPropiedades.Rows[0];

                            pgPropiedades.SelectedObject = currentTransition;
                            pgPropiedades.RetrieveFields();

                            grpControl.Text = @"Transición";

                            DataTable dtDTO = (grdDTO.DataSource as DataTable);
                            foreach (DataRow drRow in dtDTO.Rows)
                            {
                                drRow["Selección"] = false;
                                drRow["No_Editar"] = false;
                            }

                            foreach (string s in currentTransition.DTO.Columnas)
                            {
                                foreach (DataRow drRow in dtDTO.Rows)
                                {
                                    if (drRow["Nombre"].ToString() == s)
                                    {
                                        drRow["Selección"] = true;
                                        break;
                                    }
                                }
                            }

                            foreach (string s in currentTransition.DTO.ColumnasNoEditar)
                            {
                                foreach (DataRow drRow in dtDTO.Rows)
                                {
                                    if (drRow["Nombre"].ToString() == s)
                                    {
                                        drRow["No_Editar"] = true;
                                        break;
                                    }
                                }
                            }

                            NoEditarDefault();

                            foreach (XtraTabPage page in tabDTO.TabPages)
                            {
                                if (page != tabModelos)
                                {
                                    var woloc = page.Tag as woDTOColeccion;
                                    var modloc = woloc.Tag as Modelo;

                                    var col = currentTransition.DTO.Colleccion.Where(k => k.ModeloId == modloc.Id).FirstOrDefault();
                                    if (col == null)
                                    {
                                        col = new ModeloDiagramaTransicionDTOColeccion();
                                        col.ModeloId = modloc.Id;
                                        currentTransition.DTO.Colleccion.Add(col);
                                    }

                                    woloc.Asignar(col);
                                }
                            }
                            if (selectTransicion != null)
                                selectTransicion(currentTransition.Id);

                        }

                    }
                    else
                    {
                        eCurrSelection = eShapeSelection.Transaccion;

                        pgPropiedades.SelectedObject = _currDiagrama;
                        pgPropiedades.Refresh();
                        pgPropiedades.RetrieveFields();

                        grpControl.Text = "Transacción";
                    }
                }
                else
                {
                    try
                    {
                        eCurrSelection = eShapeSelection.Transaccion;

                        pgPropiedades.SelectedObject = _currDiagrama;
                        pgPropiedades.Refresh();
                        pgPropiedades.RetrieveFields();

                        grpControl.Text = "Transacción";
                    }
                    catch { }
                }
            }
        }

        private Estado FindStateByShape(DiagramShapeEstado parShape)
        {
            if (parShape != null)
            {
                foreach (Estado itemEstado in _currDiagrama.Estados)
                {
                    if (itemEstado.Idgrafico.Equals(parShape.ID.ToString()))
                        return itemEstado;
                }
            }
            return null;
        }

        private Transicion FindTransitionByShape(DiagramShapeTransition parConector)
        {
            if ((parConector != null) && (_currDiagrama != null))
            {
                foreach (Transicion itemTransicion in _currDiagrama.Transiciones)
                {
                    if (parConector.ID == null)
                        return null;

                    if (itemTransicion.IdGrafico.Equals(parConector.ID.ToString()))
                        return itemTransicion;
                }
            }
            return null;
        }

        private void diagramControl1_DragOver(object sender, DragEventArgs e)
        {
            string[] sFormats = e.Data.GetFormats();
            if (sFormats.IsNull())
                return;

            object oData = e.Data.GetData(sFormats[0]);
            if (!(oData is DiagramItem))
                return;
        }

        private void diagramControl1_AddingNewItem(object sender, DiagramAddingNewItemEventArgs e)
        {
            if (e.Item != null)
            {
                if (e.Item is DiagramShapeEstado)
                {
                    DiagramShapeEstado oShapeEstado = (e.Item as DiagramShapeEstado);
                    Estado currState;
                    if (_currDiagrama.Estados.Count == 0)
                    {
                        currState = new Estado();
                        currState.NumId = 0;
                        currState.Tipo = eEstadoTipo.Nulo;

                        oShapeEstado.TipoEstado = eEstadoTipo.Nulo;
                        oShapeEstado.Estado = currState.NumId;
                        oShapeEstado.Descripcion = "Nulo";

                        oShapeEstado.ConnectionPoints = NewPoints();
                    }
                    else
                    {
                        using (
                            fmElementNameDialog transacNameForm = new fmElementNameDialog()
                        )
                        {
                            while (oShapeEstado.Descripcion.IsNullOrStringTrimEmpty())
                            {
                                transacNameForm.ShowDialog();
                                oShapeEstado.Descripcion = transacNameForm.Nombre;
                            }
                        }

                        currState = new Estado();
                        currState.Id = oShapeEstado.Descripcion;
                        currState.NumId = _currDiagrama.Estados.Count * 100;
                        oShapeEstado.Estado = currState.NumId;

                        if (_currDiagrama.Estados.Count == 1)
                        {
                            currState.Tipo = eEstadoTipo.Inicial;
                            oShapeEstado.TipoEstado = eEstadoTipo.Inicial;
                        }
                        else
                        {
                            currState.Tipo = eEstadoTipo.Inicial;
                            oShapeEstado.TipoEstado = eEstadoTipo.Inicial;
                        }
                    }
                    oShapeEstado.ID = System.Guid.NewGuid().ToString();
                    currState.Idgrafico = oShapeEstado.ID;
                    //currState.EtiquetaId = oShapeEstado.Descripcion;
                    _currDiagrama.Estados.Add(currState);

                    pgPropiedades.SelectedObject = currState;
                    pgPropiedades.RetrieveFields();
                    grpControl.Text = "Estado";
                }
                else if (e.Item is DiagramShapeTransition)
                {
                    DiagramShapeTransition TransaccionConnector = (
                        e.Item as DiagramShapeTransition
                    );
                    using (fmElementNameDialog fmnombre = new fmElementNameDialog())
                    {
                        while (TransaccionConnector.Descripcion.IsNullOrStringTrimEmpty())
                        {
                            fmnombre.ShowDialog();
                            TransaccionConnector.Descripcion = fmnombre.Nombre;
                            TransaccionConnector.Content = fmnombre.Nombre;
                        }
                    }

                    TransaccionConnector.ID = System.Guid.NewGuid().ToString();
                    Transicion currTransition = new Transicion
                    {
                        IdGrafico = TransaccionConnector.ID
                    };

                    currTransition.Id = TransaccionConnector.Descripcion;
                    _currDiagrama.Transiciones.Add(currTransition);

                    pgPropiedades.SelectedObject = currTransition;
                    pgPropiedades.RetrieveFields();
                    grpControl.Text = "Transición";
                }
            }
        }

        private void diagramControl1_ItemsMoving(object sender, DiagramItemsMovingEventArgs e)
        {
            if (e.Items != null)
            {
                foreach (MovingItem oItem in e.Items)
                {
                    if (oItem.Item is DiagramShapeEstado)
                    {
                        DiagramShapeEstado currentState = (oItem.Item as DiagramShapeEstado);
                        foreach (Estado itemEstado in _currDiagrama.Estados)
                        {
                            if (currentState.ID != null)
                                if (currentState.ID.Equals(itemEstado.Idgrafico))
                                {
                                    itemEstado.Abscisa = oItem.NewDiagramPosition.X;
                                    itemEstado.Ordenada = oItem.NewDiagramPosition.Y;
                                }
                        }
                    }
                    else if (oItem.Item is DiagramShapeTransition)
                    {
                        DiagramShapeTransition locTransitionShape = (
                            oItem.Item as DiagramShapeTransition
                        );

                        if (e.Stage == DiagramActionStage.Start)
                        {
                            if ((e.Items.Count == 1) && (e.Items[0].OldParent.IsNull())) { }
                            else
                            {
                                e.Cancel = true;
                                e.Allow = false;
                                return;
                            }
                        }

                        foreach (Transicion itemTransicion in _currDiagrama.Transiciones)
                        {
                            if (locTransitionShape.ID != null)
                            {
                                if (locTransitionShape.ID.Equals(itemTransicion.IdGrafico))
                                {
                                    if (locTransitionShape.Points.Count == 0)
                                    {
                                        if (
                                            (locTransitionShape.Width > -0.5)
                                            && (locTransitionShape.Width < 0.5)
                                        )
                                            // Arriba
                                            locTransitionShape.Points = new PointCollection(
                                                new[]
                                                {
                                                    new PointFloat(
                                                        locTransitionShape.X - 60f,
                                                        locTransitionShape.Y
                                                            + (locTransitionShape.Height / 2f)
                                                    )
                                                }
                                            );
                                        else
                                            locTransitionShape.Points = new PointCollection(
                                                new[]
                                                {
                                                    new PointFloat(
                                                        locTransitionShape.X
                                                            + (locTransitionShape.Width / 2f),
                                                        locTransitionShape.Y - 60f
                                                    )
                                                }
                                            ); // Parte superior del circulo});
                                    }

                                    if (locTransitionShape.Points.Count >= 1)
                                    {
                                        itemTransicion.FirstControlPoint.x =
                                            locTransitionShape.Points[0].X;
                                        itemTransicion.FirstControlPoint.y =
                                            locTransitionShape.Points[0].Y;
                                        itemTransicion.SecondControlPoint.x = 0;
                                        itemTransicion.SecondControlPoint.y = 0;
                                        itemTransicion.ThirdControlPoint.x = 0;
                                        itemTransicion.ThirdControlPoint.y = 0;
                                        itemTransicion.FourControlPoint.x = 0;
                                        itemTransicion.FourControlPoint.y = 0;
                                    }
                                    //if (locTransitionShape.Points.Count == 2)
                                    //{
                                    //    itemTransicion.FirstControlPoint.x =
                                    //        locTransitionShape.Points[0].X;
                                    //    itemTransicion.FirstControlPoint.y =
                                    //        locTransitionShape.Points[0].Y;
                                    //    itemTransicion.SecondControlPoint.x =
                                    //        locTransitionShape.Points[1].X;
                                    //    itemTransicion.SecondControlPoint.y =
                                    //        locTransitionShape.Points[1].Y;
                                    //    itemTransicion.ThirdControlPoint.x = 0;
                                    //    itemTransicion.ThirdControlPoint.y = 0;
                                    //    itemTransicion.FourControlPoint.x = 0;
                                    //    itemTransicion.FourControlPoint.y = 0;
                                    //}
                                    //if (locTransitionShape.Points.Count == 3)
                                    //{
                                    //    itemTransicion.FirstControlPoint.x =
                                    //        locTransitionShape.Points[0].X;
                                    //    itemTransicion.FirstControlPoint.y =
                                    //        locTransitionShape.Points[0].Y;
                                    //    itemTransicion.SecondControlPoint.x =
                                    //        locTransitionShape.Points[1].X;
                                    //    itemTransicion.SecondControlPoint.y =
                                    //        locTransitionShape.Points[1].Y;
                                    //    itemTransicion.ThirdControlPoint.x =
                                    //        locTransitionShape.Points[2].X;
                                    //    itemTransicion.ThirdControlPoint.y =
                                    //        locTransitionShape.Points[2].Y;
                                    //    itemTransicion.FourControlPoint.x = 0;
                                    //    itemTransicion.FourControlPoint.y = 0;
                                    //}
                                    //if (locTransitionShape.Points.Count >= 4)
                                    //{
                                    //    itemTransicion.FirstControlPoint.x =
                                    //        locTransitionShape.Points[0].X;
                                    //    itemTransicion.FirstControlPoint.y =
                                    //        locTransitionShape.Points[0].Y;
                                    //    itemTransicion.SecondControlPoint.x =
                                    //        locTransitionShape.Points[1].X;
                                    //    itemTransicion.SecondControlPoint.y =
                                    //        locTransitionShape.Points[1].Y;
                                    //    itemTransicion.ThirdControlPoint.x =
                                    //        locTransitionShape.Points[2].X;
                                    //    itemTransicion.ThirdControlPoint.y =
                                    //        locTransitionShape.Points[2].Y;
                                    //    itemTransicion.FourControlPoint.x =
                                    //        locTransitionShape.Points[3].X;
                                    //    itemTransicion.FourControlPoint.y =
                                    //        locTransitionShape.Points[3].X;
                                    //}

                                    if (locTransitionShape.BeginItem != null)
                                    {
                                        itemTransicion.EstadoInicial = (
                                            (DiagramShapeEstado)locTransitionShape.BeginItem
                                        ).Estado;
                                    }

                                    if (locTransitionShape.EndItem != null)
                                    {
                                        itemTransicion.EstadoFinal = (
                                            (DiagramShapeEstado)locTransitionShape.EndItem
                                        ).Estado;
                                    }

                                    itemTransicion.BeginItemPointIndex =
                                        locTransitionShape.BeginItemPointIndex;
                                    itemTransicion.EndItemPointIndex =
                                        locTransitionShape.EndItemPointIndex;
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                pgPropiedades.Refresh();
                pgPropiedades.RetrieveFields();
            }
            catch { }
        }

        private void diagramControl1_ItemsDeleting(object sender, DiagramItemsDeletingEventArgs e)
        {
            if (e.Items != null)
            {
                foreach (DiagramItem oItem in e.Items)
                {
                    if (oItem is DiagramShapeEstado)
                    {
                        DiagramShapeEstado oShapeEstado = (oItem as DiagramShapeEstado);
                        Estado itemEstadoEliminar = null;
                        foreach (Estado itemEstado in _currDiagrama.Estados)
                        {
                            if (oShapeEstado.ID != null)
                                if (oShapeEstado.ID.Equals(itemEstado.Idgrafico))
                                {
                                    itemEstadoEliminar = itemEstado;
                                }
                        }
                        if (itemEstadoEliminar != null)
                            _currDiagrama.Estados.Remove(itemEstadoEliminar);
                    }
                    else if (oItem is DiagramShapeTransition)
                    {
                        DiagramShapeTransition TransaccionConnector = (
                            oItem as DiagramShapeTransition
                        );
                        Transicion itemTransaccionEliminar = null;
                        foreach (Transicion itemTransicion in _currDiagrama.Transiciones)
                        {
                            if (TransaccionConnector.ID != null)
                                if (TransaccionConnector.ID.Equals(itemTransicion.IdGrafico))
                                {
                                    itemTransaccionEliminar = itemTransicion;
                                }
                        }

                        if (itemTransaccionEliminar != null)
                            _currDiagrama.Transiciones.Remove(itemTransaccionEliminar);
                    }
                }
            }
        }

        private void diagramControl1_ConnectionChanged(
            object sender,
            DiagramConnectionChangedEventArgs e
        )
        {
            if (e.Connector.BeginItem != null && e.Connector.EndItem != null)
            {
                DiagramShapeEstado oShapeEstadoInicial = (
                    e.Connector.BeginItem as DiagramShapeEstado
                );
                DiagramShapeEstado oShapeEstadoFinal = (e.Connector.EndItem as DiagramShapeEstado);

                if (oShapeEstadoInicial.Estado == oShapeEstadoFinal.Estado)
                {
                    DiagramShapeTransition TransaccionConnector = (
                        e.Connector as DiagramShapeTransition
                    );

                    var points = new List<PointFloat>();
                    foreach (var point in TransaccionConnector.Points)
                        points.Add(new PointFloat(point.X - 20, point.Y));

                    TransaccionConnector.Points = new PointCollection(points);

                    //PointCollection oColection = TransaccionConnector.Points.;
                    //for(int iPuntos = 0; iPuntos < oColection.Count; iPuntos++)
                    //{
                    //    oColection[iPuntos].X += 10f;
                    //    oColection[iPuntos].Y += 10f;
                    //    //oPoint.X += 10f;
                    //    //oPoint.Y += 10f;
                    //}
                }
            }

            DiagramShapeTransition locTransitionShape = (e.Connector as DiagramShapeTransition);

            if (locTransitionShape.ID != null)
            {
                if ((pgPropiedades.SelectedObject != null) && (pgPropiedades.Rows.Count > 0))
                    pgPropiedades.FocusedRow = pgPropiedades.Rows[0];

                foreach (Transicion itemTransicion in _currDiagrama.Transiciones)
                {
                    if (locTransitionShape.ID.Equals(itemTransicion.IdGrafico))
                    {
                        if (locTransitionShape.Points.Count == 0)
                        {
                            if (
                                (locTransitionShape.Width > -0.5)
                                && (locTransitionShape.Width < 0.5)
                            )
                                // Arriba
                                locTransitionShape.Points = new PointCollection(
                                    new[]
                                    {
                                        new PointFloat(
                                            locTransitionShape.X - 60f,
                                            locTransitionShape.Y + (locTransitionShape.Height / 2f)
                                        )
                                    }
                                );
                            else
                                locTransitionShape.Points = new PointCollection(
                                    new[]
                                    {
                                        new PointFloat(
                                            locTransitionShape.X + (locTransitionShape.Width / 2f),
                                            locTransitionShape.Y - 60f
                                        )
                                    }
                                ); // Parte superior del circulo});
                        }

                        if (locTransitionShape.Points.Count >= 1)
                        {
                            itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                            itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                            itemTransicion.SecondControlPoint.x = 0;
                            itemTransicion.SecondControlPoint.y = 0;
                            itemTransicion.ThirdControlPoint.x = 0;
                            itemTransicion.ThirdControlPoint.y = 0;
                            itemTransicion.FourControlPoint.x = 0;
                            itemTransicion.FourControlPoint.y = 0;
                        }
                        //if (locTransitionShape.Points.Count == 2)
                        //{
                        //    itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                        //    itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                        //    itemTransicion.SecondControlPoint.x = locTransitionShape.Points[1].X;
                        //    itemTransicion.SecondControlPoint.y = locTransitionShape.Points[1].Y;
                        //    itemTransicion.ThirdControlPoint.x = 0;
                        //    itemTransicion.ThirdControlPoint.y = 0;
                        //    itemTransicion.FourControlPoint.x = 0;
                        //    itemTransicion.FourControlPoint.y = 0;
                        //}
                        //if (locTransitionShape.Points.Count == 3)
                        //{
                        //    itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                        //    itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                        //    itemTransicion.SecondControlPoint.x = locTransitionShape.Points[1].X;
                        //    itemTransicion.SecondControlPoint.y = locTransitionShape.Points[1].Y;
                        //    itemTransicion.ThirdControlPoint.x = locTransitionShape.Points[2].X;
                        //    itemTransicion.ThirdControlPoint.y = locTransitionShape.Points[2].Y;
                        //    itemTransicion.FourControlPoint.x = 0;
                        //    itemTransicion.FourControlPoint.y = 0;
                        //}
                        //if (locTransitionShape.Points.Count >= 4)
                        //{
                        //    itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                        //    itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                        //    itemTransicion.SecondControlPoint.x = locTransitionShape.Points[1].X;
                        //    itemTransicion.SecondControlPoint.y = locTransitionShape.Points[1].Y;
                        //    itemTransicion.ThirdControlPoint.x = locTransitionShape.Points[2].X;
                        //    itemTransicion.ThirdControlPoint.y = locTransitionShape.Points[2].Y;
                        //    itemTransicion.FourControlPoint.x = locTransitionShape.Points[3].X;
                        //    itemTransicion.FourControlPoint.y = locTransitionShape.Points[3].X;
                        //}
                        if (locTransitionShape.BeginItem != null)
                        {
                            itemTransicion.EstadoInicial = (
                                (DiagramShapeEstado)locTransitionShape.BeginItem
                            ).Estado;
                        }

                        if (locTransitionShape.EndItem != null)
                        {
                            itemTransicion.EstadoFinal = (
                                (DiagramShapeEstado)locTransitionShape.EndItem
                            ).Estado;
                        }

                        itemTransicion.BeginItemPointIndex = locTransitionShape.BeginItemPointIndex;
                        itemTransicion.EndItemPointIndex = locTransitionShape.EndItemPointIndex;
                    }
                }
            }

            try
            {
                if (pgPropiedades.SelectedObject != null)
                {
                    pgPropiedades.Refresh();
                    pgPropiedades.RetrieveFields();
                }
            }
            catch { }
        }

        private void diagramControl1_ConnectionChanging(
            object sender,
            DiagramConnectionChangingEventArgs e
        )
        {
            if (e.NewItem.IsNull())
            {
                e.Cancel = true;
            }
        }

        private void diagramControl1_CustomDrawItem(object sender, CustomDrawItemEventArgs e)
        {
            if (e.Item is DiagramShapeEstado)
            {
                #region ...

                if (e.Context == DiagramDrawingContext.Canvas)
                {
                    var shape = e.Item as DiagramShapeEstado;

                    var state = FindStateByShape(shape);

                    if (state == null)
                        return;

                    shape.TipoEstado = state.Tipo;


                    if (shape.TipoEstado == eEstadoTipo.Nulo)
                    {
                        RectangleF itemRect = new RectangleF(0, 0, e.Item.Width, e.Item.Height);
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddEllipse(itemRect);

                        LinearGradientBrush gradientBrush = new LinearGradientBrush(
                            itemRect,
                            System.Drawing.Color.DarkSlateGray,
                            System.Drawing.Color.WhiteSmoke,
                            90f,
                            true
                        );
                        gradientBrush.SetBlendTriangularShape(.5f, 1.0f);
                        e.Graphics.FillPath(gradientBrush, gp);
                        Pen pathPen = new Pen(Color.Black, 1);
                        e.Graphics.DrawPath(pathPen, gp);
                        gp.Dispose();
                    }
                    else if (shape.TipoEstado == eEstadoTipo.Inicial)
                    {
                        RectangleF itemRect = new RectangleF(0, 0, e.Item.Width, e.Item.Height);
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddEllipse(itemRect);

                        LinearGradientBrush gradientBrush = new LinearGradientBrush(
                            itemRect,
                            System.Drawing.Color.Orange,
                            System.Drawing.Color.WhiteSmoke,
                            90f,
                            true
                        );
                        gradientBrush.SetBlendTriangularShape(.5f, 1.0f);
                        e.Graphics.FillPath(gradientBrush, gp);
                        Pen pathPen = new Pen(Color.Black, 1);
                        e.Graphics.DrawPath(pathPen, gp);
                        gp.Dispose();
                    }
                    else if (shape.TipoEstado == eEstadoTipo.Intermedio)
                    {
                        RectangleF itemRect = new RectangleF(0, 0, e.Item.Width, e.Item.Height);
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddEllipse(itemRect);

                        LinearGradientBrush gradientBrush = new LinearGradientBrush(
                            itemRect,
                            System.Drawing.Color.DeepSkyBlue,
                            System.Drawing.Color.WhiteSmoke,
                            90f,
                            true
                        );

                        gradientBrush.SetBlendTriangularShape(.5f, 1.0f);
                        e.Graphics.FillPath(gradientBrush, gp);
                        Pen pathPen = new Pen(Color.Black, 1);
                        e.Graphics.DrawPath(pathPen, gp);
                        gp.Dispose();
                    }
                    else if (shape.TipoEstado == eEstadoTipo.FinalNormal)
                    {
                        RectangleF itemRect = new RectangleF(0, 0, e.Item.Width, e.Item.Height);
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddEllipse(itemRect);

                        LinearGradientBrush gradientBrush = new LinearGradientBrush(
                            itemRect,
                            System.Drawing.Color.Green,
                            System.Drawing.Color.WhiteSmoke,
                            90f,
                            true
                        );
                        gradientBrush.SetBlendTriangularShape(.5f, 1.0f);
                        e.Graphics.FillPath(gradientBrush, gp);
                        Pen pathPen = new Pen(Color.Black, 1);
                        e.Graphics.DrawPath(pathPen, gp);
                        gp.Dispose();
                    }
                    else if (shape.TipoEstado == eEstadoTipo.FinalAlternativo)
                    {
                        RectangleF itemRect = new RectangleF(0, 0, e.Item.Width, e.Item.Height);
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddEllipse(itemRect);

                        LinearGradientBrush gradientBrush = new LinearGradientBrush(
                            itemRect,
                            System.Drawing.Color.Red,
                            System.Drawing.Color.WhiteSmoke,
                            90f,
                            true
                        );
                        gradientBrush.SetBlendTriangularShape(.5f, 1.0f);
                        e.Graphics.FillPath(gradientBrush, gp);
                        Pen pathPen = new Pen(Color.Black, 1);
                        e.Graphics.DrawPath(pathPen, gp);
                        gp.Dispose();
                    }

                    Brush oTextBrus = new SolidBrush(System.Drawing.Color.Black);
                    Font font = new Font(@"Arial", 12.0f, FontStyle.Bold, GraphicsUnit.Pixel);

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    e.Graphics.DrawString(
                        shape.Estado.ToString() + System.Environment.NewLine + shape.Descripcion,
                        font,
                        Brushes.Black,
                        new PointF(e.Item.Width / 2, e.Item.Height / 2),
                        sf
                    );

                    shape.Appearance.ForeColor = Color.Black;
                    shape.Appearance.BorderColor = Color.Black;
                    shape.Appearance.BorderSize = 2;

                    e.Handled = true;
                }

                #endregion ...
            }
            else if (e.Item is DiagramShapeTransition)
            {
                DiagramShapeTransition locTransitionShape = e.Item as DiagramShapeTransition;

                var transition = FindTransitionByShape(locTransitionShape);

                if (transition == null)
                    return;

                if (transition.Selected)
                {
                    locTransitionShape.Appearance.BorderColor = Color.Orange;
                    locTransitionShape.Appearance.BorderSize = 5;
                    //locTransitionShape.Appearance.BorderDashPattern = new DiagramDoubleCollection(new double[] { 10, 3 });
                }
                else if (transition.Tipo == eTransicionTipo.Local)
                {
                    locTransitionShape.Appearance.BorderColor = Color.Black;
                    locTransitionShape.Appearance.BorderSize = 3;
                    //locTransitionShape.Appearance.BorderDashPattern = null;
                }
                else if (transition.Tipo == eTransicionTipo.Interna)
                {
                    locTransitionShape.Appearance.BorderColor = Color.Red;
                    locTransitionShape.Appearance.BorderSize = 3;
                    //locTransitionShape.Appearance.BorderDashPattern = null;

                }
                else if (transition.Tipo == eTransicionTipo.Publica)
                {
                    locTransitionShape.Appearance.BorderColor = Color.Lime;
                    locTransitionShape.Appearance.BorderSize = 3;
                    //locTransitionShape.Appearance.BorderDashPattern = null;

                }
                //e.Handled = true;
            }
        }

        public void New()
        {
            diagramControl1.Items.Clear();
            diagramControl1.NewDocument();
        }

        public void CreaDiagrama()
        {
            if (bFirstDraw)
            {
                if (_currDiagrama.Alto > 0 && _currDiagrama.Ancho > 0)
                {
                    diagramControl1.Height = _currDiagrama.Alto;
                    diagramControl1.Width = _currDiagrama.Ancho;
                }
                else
                {
                    diagramControl1.OptionsView.PageSize = new SizeF(900, 800);
                }

                diagramControl1.ZoomIn();
            }

            diagramControl1.NewDocument();

            bFirstDraw = false;

            diagramControl1.SuspendLayout();
            diagramControl1.BeginUpdate();

            Cursor curOriginal = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                diagramControl1.Items.Clear();

                float width = 100;
                float height = 100;
                float x = 0;
                float y = 0;

                foreach (Estado itemEstado in _currDiagrama.Estados)
                {
                    DiagramShapeEstado diagramState = new DiagramShapeEstado();
                    diagramState.Height = height;
                    diagramState.Width = width;

                    float x1 = 0.5f * 0.71f;
                    float y1 = 0.5f - x1;

                    diagramState.ConnectionPoints = NewPoints();

                    if ((itemEstado.Abscisa == 0) && (itemEstado.Ordenada == 0))
                        diagramState.Bounds = new RectangleF(x, y, width, height);
                    else
                    {
                        diagramState.Bounds = new RectangleF(
                            itemEstado.Abscisa,
                            itemEstado.Ordenada,
                            width,
                            height
                        );
                        diagramState.X = itemEstado.Abscisa;
                        diagramState.Y = itemEstado.Ordenada;
                        diagramState.Position = new DevExpress.Utils.PointFloat(
                            itemEstado.Abscisa,
                            itemEstado.Ordenada
                        );
                    }

                    diagramState.Content =
                        itemEstado.NumId.ToString()
                        + Environment.NewLine
                        + EtiquetaCol.Get(itemEstado.EtiquetaId);
                    diagramState.Estado = itemEstado.NumId;
                    diagramState.Descripcion = itemEstado.Id; //  EtiquetaCol.Get(itemEstado.EtiquetaId);
                    diagramState.TipoEstado = itemEstado.Tipo;

                    diagramState.Shape = BasicShapes.Ellipse;
                    diagramState.ID = itemEstado.Idgrafico;
                    //itemEstado.Idgrafico = nStateShape.UniqueId.ToString();
                    diagramControl1.Items.Add(diagramState);

                    x += 120;
                    if (x >= diagramControl1.Width)
                    {
                        y += 120;
                        x = 0;
                    }
                }

                Font font = new Font(@"Arial", (Screen.PrimaryScreen.Bounds.Width == 3840 ? 24.0f : 12f), FontStyle.Bold, GraphicsUnit.Pixel);

                foreach (Transicion itemTransicion in _currDiagrama.Transiciones)
                {
                    DiagramShapeTransition connector = new DiagramShapeTransition();
                    connector.Type = DevExpress.Diagram.Core.ConnectorType.Curved;
                    connector.AffectedByLayoutAlgorithms = false;
                    connector.CanMove = false;
                    connector.CanCopy = false;
                    connector.CanDragBeginPoint = true;
                    connector.CanDragEndPoint = true;
                    connector.CanChangeRoute = true;

                    connector.EndArrow = DevExpress.Diagram.Core.ArrowDescriptions.Filled90;
                    connector.Appearance.BackColor = System.Drawing.Color.Red;
                    connector.Appearance.BackColor2 = System.Drawing.Color.Red;
                    connector.Appearance.BorderColor = System.Drawing.Color.Red;
                    connector.Appearance.ForeColor = System.Drawing.Color.Black;
                    connector.Appearance.BorderSize = 4;
                    connector.ID = itemTransicion.IdGrafico;

                    //connector.Notificacion_Tipo = itemTransicion.Notificacion.Tipo;

                    // Resolucion del monitor
                    connector.Appearance.Font = font;

                    string Content = itemTransicion.Id;

                    if (MostrarPermisos)
                    {
                        foreach (var Permiso in itemTransicion.Permisos)
                        {
                            Content += Environment.NewLine;
                            Content += Permiso.PermisoId;
                        }
                    }

                    connector.Content = Content;

                    DiagramShapeEstado oShapeIni = getDiagramShapeOfState(
                        itemTransicion.EstadoInicial
                    );
                    DiagramShapeEstado oShapeFin = getDiagramShapeOfState(
                        itemTransicion.EstadoFinal
                    );

                    if (oShapeIni != null)
                        connector.BeginItem = oShapeIni;

                    if (oShapeFin != null)
                        connector.EndItem = oShapeFin;

                    List<DevExpress.Utils.PointFloat> oListPoint =
                        new List<DevExpress.Utils.PointFloat>();

                    if (
                        (itemTransicion.FirstControlPoint.x != 0f)
                        && (itemTransicion.FirstControlPoint.y != 0)
                    )
                        oListPoint.Add(
                            new DevExpress.Utils.PointFloat(
                                itemTransicion.FirstControlPoint.x,
                                itemTransicion.FirstControlPoint.y
                            )
                        );

                    if (
                        (itemTransicion.SecondControlPoint.x != 0f)
                        && (itemTransicion.SecondControlPoint.y != 0)
                    )
                        oListPoint.Add(
                            new DevExpress.Utils.PointFloat(
                                itemTransicion.SecondControlPoint.x,
                                itemTransicion.SecondControlPoint.y
                            )
                        );

                    if (
                        (itemTransicion.ThirdControlPoint.x != 0f)
                        && (itemTransicion.ThirdControlPoint.y != 0)
                    )
                        oListPoint.Add(
                            new DevExpress.Utils.PointFloat(
                                itemTransicion.ThirdControlPoint.x,
                                itemTransicion.ThirdControlPoint.y
                            )
                        );

                    //if ((itemTransicion.FourControlPoint.x != 0f) && (itemTransicion.FourControlPoint.y != 0))
                    //    oListPoint.Add(new DevExpress.Utils.PointFloat(itemTransicion.FourControlPoint.x, itemTransicion.FourControlPoint.y));

                    connector.Points = new DevExpress.XtraDiagram.PointCollection(oListPoint);

                    if (itemTransicion.EstadoInicial == itemTransicion.EstadoFinal)
                    {
                        connector.BeginItemPointIndex = 0;
                        connector.EndItemPointIndex = 7;
                    }
                    else
                    {
                        connector.BeginItemPointIndex = itemTransicion.BeginItemPointIndex;
                        connector.EndItemPointIndex = itemTransicion.EndItemPointIndex;
                    }

                    diagramControl1.Items.Add(connector);
                }

                pgPropiedades.SelectedObject = _currDiagrama;
                pgPropiedades.RetrieveFields();

                grpDTO.Visible = false;
            }
            finally
            {
                diagramControl1.ResumeLayout();
                diagramControl1.EndUpdate();
                Cursor.Current = curOriginal;
            }
        }

        private DiagramShapeEstado getDiagramShapeOfState(int iParState)
        {
            foreach (DiagramItem iTem in diagramControl1.Items)
            {
                if (iTem is DiagramShapeEstado)
                {
                    if ((iTem as DiagramShapeEstado).Estado == iParState)
                        return (iTem as DiagramShapeEstado);
                }
            }
            return null;
        }

        public void ActulizaPosiciones()
        {
            pgPropiedades.PostEditor();

            #region Actualiza Coordenadas

            foreach (DiagramItem oItem in diagramControl1.Items)
            {
                if (oItem is DiagramShapeEstado)
                {
                    DiagramShapeEstado currentState = (oItem as DiagramShapeEstado);
                    foreach (Estado itemEstado in _currDiagrama.Estados)
                    {
                        if (currentState.ID.Equals(itemEstado.Idgrafico))
                        {
                            itemEstado.Abscisa = oItem.X;
                            itemEstado.Ordenada = oItem.Y;
                        }
                    }
                }
                else if (oItem is DiagramShapeTransition)
                {
                    DiagramShapeTransition locTransitionShape = (oItem as DiagramShapeTransition);

                    foreach (Transicion itemTransicion in _currDiagrama.Transiciones)
                    {
                        if (locTransitionShape.ID.Equals(itemTransicion.IdGrafico))
                        {
                            if (locTransitionShape.Points.Count == 0)
                            {
                                if (
                                    (locTransitionShape.Width > -0.5)
                                    && (locTransitionShape.Width < 0.5)
                                )
                                    // Arriba
                                    locTransitionShape.Points = new PointCollection(
                                        new[]
                                        {
                                            new PointFloat(
                                                locTransitionShape.X - 60f,
                                                locTransitionShape.Y
                                                    + (locTransitionShape.Height / 2f)
                                            )
                                        }
                                    );
                                else
                                    locTransitionShape.Points = new PointCollection(
                                        new[]
                                        {
                                            new PointFloat(
                                                locTransitionShape.X
                                                    + (locTransitionShape.Width / 2f),
                                                locTransitionShape.Y - 60f
                                            )
                                        }
                                    ); // Parte superior del circulo});
                            }

                            if (locTransitionShape.Points.Count >= 1)
                            {
                                itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                                itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                                itemTransicion.SecondControlPoint.x = 0;
                                itemTransicion.SecondControlPoint.y = 0;
                                itemTransicion.ThirdControlPoint.x = 0;
                                itemTransicion.ThirdControlPoint.y = 0;
                                itemTransicion.FourControlPoint.x = 0;
                                itemTransicion.FourControlPoint.y = 0;
                            }
                            //if (locTransitionShape.Points.Count == 2)
                            //{
                            //    itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                            //    itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                            //    itemTransicion.SecondControlPoint.x = locTransitionShape.Points[
                            //        1
                            //    ].X;
                            //    itemTransicion.SecondControlPoint.y = locTransitionShape.Points[
                            //        1
                            //    ].Y;
                            //    itemTransicion.ThirdControlPoint.x = 0;
                            //    itemTransicion.ThirdControlPoint.y = 0;
                            //    itemTransicion.FourControlPoint.x = 0;
                            //    itemTransicion.FourControlPoint.y = 0;
                            //}
                            //if (locTransitionShape.Points.Count == 3)
                            //{
                            //    itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                            //    itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                            //    itemTransicion.SecondControlPoint.x = locTransitionShape.Points[
                            //        1
                            //    ].X;
                            //    itemTransicion.SecondControlPoint.y = locTransitionShape.Points[
                            //        1
                            //    ].Y;
                            //    itemTransicion.ThirdControlPoint.x = locTransitionShape.Points[2].X;
                            //    itemTransicion.ThirdControlPoint.y = locTransitionShape.Points[2].Y;
                            //    itemTransicion.FourControlPoint.x = 0;
                            //    itemTransicion.FourControlPoint.y = 0;
                            //}
                            //if (locTransitionShape.Points.Count >= 4)
                            //{
                            //    itemTransicion.FirstControlPoint.x = locTransitionShape.Points[0].X;
                            //    itemTransicion.FirstControlPoint.y = locTransitionShape.Points[0].Y;
                            //    itemTransicion.SecondControlPoint.x = locTransitionShape.Points[
                            //        1
                            //    ].X;
                            //    itemTransicion.SecondControlPoint.y = locTransitionShape.Points[
                            //        1
                            //    ].Y;
                            //    itemTransicion.ThirdControlPoint.x = locTransitionShape.Points[2].X;
                            //    itemTransicion.ThirdControlPoint.y = locTransitionShape.Points[2].Y;
                            //    itemTransicion.FourControlPoint.x = 0;
                            //    itemTransicion.FourControlPoint.y = 0;

                            //    //itemTransicion.FourControlPoint.x = locTransitionShape.Points[3].X;
                            //    //itemTransicion.FourControlPoint.y = locTransitionShape.Points[3].X;
                            //}
                            if (locTransitionShape.BeginItem != null)
                            {
                                itemTransicion.EstadoInicial = (
                                    (DiagramShapeEstado)locTransitionShape.BeginItem
                                ).Estado;
                            }

                            if (locTransitionShape.EndItem != null)
                            {
                                itemTransicion.EstadoFinal = (
                                    (DiagramShapeEstado)locTransitionShape.EndItem
                                ).Estado;
                            }

                            itemTransicion.BeginItemPointIndex =
                                locTransitionShape.BeginItemPointIndex;
                            itemTransicion.EndItemPointIndex = locTransitionShape.EndItemPointIndex;
                        }
                    }
                }
            }

            #endregion Actualiza Coordenadas
        }

        public void ZoomIn()
        {
            diagramControl1.ZoomIn();
        }

        public void ZoomOut()
        {
            diagramControl1.ZoomOut();
        }

        public void AsignaDTO(Modelo model)
        {
            // Todo falta referencias

            this.currModelo = model;

            DataTable dtDTO = (grdDTO.DataSource as DataTable);
            dtDTO.Rows.Clear();
            foreach (var col in model.Columnas.OrderBy(j => j.Orden))
            {
                if ((col.EsColeccion) && (col.TipoColumna == WoTypeColumn.Reference))
                    continue;

                if ((WoConst.ColumnasSistema.IndexOf(col.Id) == -1) ||
                    (col.Id == WoConst.ROWVERSION))
                {
                    DataRow drRow = dtDTO.NewRow();
                    drRow["Selección"] = false;
                    drRow["No_Editar"] = false;
                    drRow["Nombre"] = col.Id;
                    dtDTO.Rows.Add(drRow);
                }
            }


            List<XtraTabPage> tabBorrar = new List<XtraTabPage>();
            foreach (XtraTabPage page in tabDTO.TabPages)
                if (page != tabModelos)
                    tabBorrar.Add(page);

            foreach (var page in tabBorrar)
                tabDTO.TabPages.Remove(page);

            foreach (var col in model.Columnas.OrderBy(j => j.Orden))
            {
                if ((col.EsColeccion) && (!col.ModeloId.IsNullOrStringEmpty()) && (col.TipoColumna == WoTypeColumn.Reference))
                    DTOColeccion(col);
            }
        }

        private void DTOColeccion(ModeloColumna col)
        {
            if (proyecto == null)
                return;

            var modloc = proyecto.ModeloCol.Modelos.Where(j => j.Id == col.ModeloId).FirstOrDefault();

            if (modloc == null)
                return;

            var wo = new woDTOColeccion(modloc);

            XtraTabPage tab = new XtraTabPage();

            tab.Tag = modloc;

            tabDTO.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
                    tab});

            tab.Name = "tabx" + modloc.Id;
            tab.Padding = new System.Windows.Forms.Padding(5);
            tab.Size = new System.Drawing.Size(243, 228);
            tab.Text = modloc.Id;

            tab.Controls.Add(wo);

            wo.Dock = System.Windows.Forms.DockStyle.Fill;
            wo.Location = new System.Drawing.Point(5, 5);
            wo.Name = "wox" + modloc.Id;
            wo.Size = new System.Drawing.Size(233, 218);
            wo.Tag = modloc;
            tab.Tag = wo;
        }


        public void SetReadOnly(bool ReadOnly)
        {
            diagramControl1.OptionsProtection.IsReadOnly = ReadOnly;
            pgPropiedades.OptionsBehavior.Editable = !ReadOnly;
            toolboxControl1.Visible = !ReadOnly;
            splitContainerControl2.Enabled = !ReadOnly;
            grdDTOView.OptionsBehavior.ReadOnly = ReadOnly;
        }


        public void SetReadOnlyDiagram(bool ReadOnly)
        {
            diagramControl1.OptionsProtection.IsReadOnly = ReadOnly;
            toolboxControl1.Visible = !ReadOnly;
        }

        public void SetSelectable()
        {
            SetReadOnly(true);
            //diagramControl1.OptionsProtection.select allows .IsReadOnly = false;
        }


        public void HideTools()
        {
            splDiagrama.PanelVisibility = SplitPanelVisibility.Panel2;
        }

        private void mnuZoomOut_Click(object sender, EventArgs e)
        {
            diagramControl1.ZoomOut();
        }

        private void mnuZoomIn_Click(object sender, EventArgs e)
        {
            diagramControl1.ZoomIn();
        }


        public void MarcarPorFlujo()
        {
            foreach (var transicion in currDiagrama.Transiciones)
            {
                foreach (DiagramItem oItem in diagramControl1.Items)
                {
                    if (oItem is DiagramShapeTransition)
                    {
                        diagramControl1.SelectItem(oItem);
                        mnuMarcadoFlujo_Click(null, null);
                        NoEditarDefault();
                        GrdDTOView_ValidateRow(null, null);
                    }
                }
            }

            GrdDTOView_ValidateRow(null, null);
        }

        public void MarcarDefault()
        {
            foreach (var transicion in currDiagrama.Transiciones)
            {
                foreach (DiagramItem oItem in diagramControl1.Items)
                {
                    if (oItem is DiagramShapeTransition)
                    {
                        diagramControl1.SelectItem(oItem);
                        NoEditarDefault();
                        GrdDTOView_ValidateRow(null, null);
                    }
                }
            }


        }

        private void mnuMarcadoFlujo_Click(object sender, EventArgs e)
        {

            int EstadoInicial = 0;
            int EstadoFinal = 0;
            bool bMarcarRowVersion = true;


            if (diagramControl1.SelectedItems.Count == 0)
                return;

            DataTable dtDTO = (grdDTO.DataSource as DataTable);

            if (diagramControl1.SelectedItems[0] is DiagramShapeTransition)
            {
                DiagramShapeTransition curentObjectSelcted = (
                    diagramControl1.SelectedItems[0] as DiagramShapeTransition
                );

                var Transition = FindTransitionByShape(curentObjectSelcted);
                if (Transition != null)
                {
                    EstadoInicial = Transition.EstadoInicial;
                    EstadoFinal = Transition.EstadoFinal;
                }
            }

            if (EstadoInicial == 0)
                bMarcarRowVersion = false;

            if ((_currModelo.TipoModelo == WoTypeModel.CatalogType) ||
                (_currModelo.TipoModelo == WoTypeModel.Catalog) ||
                (_currModelo.TipoModelo == WoTypeModel.Parameter))
            {
                bool bMarcarSuspendInfo = false;
                bool bMarcarDeleteInfo = false;

                List<string> CamposOmitir = new List<string>(new string[] { WoConst.SUSPENDINFO, WoConst.SUSPENDBY, WoConst.SUSPENDDATE, WoConst.DELETEINFO });
                List<string> CamposBase = new List<string>(new string[] { "Id", WoConst.ROWVERSION, "WoState" });

                if ((EstadoFinal == 500) || (EstadoFinal == 600) || (EstadoInicial == 500))
                {
                    if (EstadoFinal == 500)
                        bMarcarSuspendInfo = true;
                    else if (EstadoFinal == 600)
                        bMarcarDeleteInfo = true;

                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        drRow["Selección"] = false;
                        if ((bMarcarSuspendInfo) && (drRow["Nombre"].ToString() == WoConst.SUSPENDINFO))
                            drRow["Selección"] = true;
                        else if ((bMarcarDeleteInfo) && (drRow["Nombre"].ToString() == WoConst.DELETEINFO))
                            drRow["Selección"] = true;
                        else if ((!drRow["Nombre"].ToString().StartsWith("__")) &&
                                (CamposBase.IndexOf(drRow["Nombre"].ToString()) != -1))
                            drRow["Selección"] = true;
                    }
                }
                else
                {
                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        if ((!bMarcarRowVersion) && (drRow["Nombre"].ToString() == WoConst.ROWVERSION))
                            continue;
                        if ((!drRow["Nombre"].ToString().StartsWith("__")) &&
                            (CamposOmitir.IndexOf(drRow["Nombre"].ToString()) == -1))
                            drRow["Selección"] = true;
                    }
                }
            }
            else if ((_currModelo.TipoModelo == WoTypeModel.TransactionContable) ||
                    (_currModelo.TipoModelo == WoTypeModel.TransactionNoContable))
            {
                bool bMarcarObservacion = false;

                List<string> CamposOmitir = new List<string>(new string[] { WoConst.CREATEDDATE, WoConst.CREATEDBY, WoConst.MODIFIEDDATE, WoConst.MODIFIEDBY, WoConst.DELETEDDATE, WoConst.DELETEDBY });
                List<string> CamposOmitir2 = new List<string>(new string[] { WoConst.WOUDNID, WoConst.WOSERIE, WoConst.WOFOLIO, WoConst.WOPERIODOID, WoConst.WOCONPOLIZAID, WoConst.WOORIGEN, WoConst.WOGUID });
                List<string> CamposBase = new List<string>(new string[] { "Id", WoConst.ROWVERSION, WoConst.WOSTATE });

                if (((EstadoInicial != 0) && (EstadoFinal == 200)) || (EstadoFinal == 300) || (EstadoFinal == 600))
                {
                    if (EstadoFinal == 600)
                        bMarcarObservacion = true;

                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        if ((bMarcarObservacion) && (drRow["Nombre"].ToString() == WoConst.WOOBSERVACION))
                            drRow["Selección"] = true;
                        else if ((!drRow["Nombre"].ToString().StartsWith("__")) &&
                                (CamposBase.IndexOf(drRow["Nombre"].ToString()) != -1))
                            drRow["Selección"] = true;
                    }
                }
                else if (EstadoInicial == 0)
                {
                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        if ((drRow["Nombre"].ToString() == WoConst.WOCONPOLIZAID) ||
                            (drRow["Nombre"].ToString() == WoConst.ROWVERSION))
                            continue;
                        if ((!drRow["Nombre"].ToString().StartsWith("__")) &&
                            (CamposOmitir.IndexOf(drRow["Nombre"].ToString()) == -1))
                            drRow["Selección"] = true;
                    }
                }
                else
                {
                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        if ((!drRow["Nombre"].ToString().StartsWith("__")) &&
                            ((CamposOmitir.IndexOf(drRow["Nombre"].ToString()) == -1) &&
                              (CamposOmitir2.IndexOf(drRow["Nombre"].ToString()) == -1)))
                            drRow["Selección"] = true;
                    }

                }
            }

            NoEditarDefault();
        }

        private void mnuDesMarcarTodo_Click(object sender, EventArgs e)
        {
            DataTable dtDTO = (grdDTO.DataSource as DataTable);
            foreach (DataRow drRow in dtDTO.Rows)
            {
                drRow["Selección"] = false;
                drRow["No_Editar"] = false;
            }

            NoEditarDefault();
        }

        private void NoEditarDefault()
        {
            if (_currModelo == null)
                return;

            int EstadoInicial = 0;
            int EstadoFinal = 0;

            if (diagramControl1.SelectedItems.Count == 0)
                return;

            DataTable dtDTO = (grdDTO.DataSource as DataTable);

            if (diagramControl1.SelectedItems[0] is DiagramShapeTransition)
            {
                DiagramShapeTransition curentObjectSelcted = (
                    diagramControl1.SelectedItems[0] as DiagramShapeTransition
                );

                var Transition = FindTransitionByShape(curentObjectSelcted);
                if (Transition != null)
                {
                    EstadoInicial = Transition.EstadoInicial;
                    EstadoFinal = Transition.EstadoFinal;
                }
            }


            if ((_currModelo.TipoModelo == WoTypeModel.CatalogType) ||
                (_currModelo.TipoModelo == WoTypeModel.Catalog) ||
                (_currModelo.TipoModelo == WoTypeModel.Parameter))
            {
                if (EstadoInicial == 0)
                {
                    List<string> CamposNoEditar = new List<string>(new string[] { "WoState" });

                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        if (drRow["Nombre"].ToString() == "Id")
                            drRow["No_Editar"] = false;
                        if (CamposNoEditar.IndexOf(drRow["Nombre"].ToString()) != -1)
                            drRow["No_Editar"] = true;
                    }
                }
                else
                {
                    List<string> CamposNoEditar = new List<string>(new string[] { "WoState", WoConst.ROWVERSION });

                    foreach (DataRow drRow in dtDTO.Rows)
                    {
                        if (drRow["Nombre"].ToString() == "Id")
                            drRow["No_Editar"] = true;
                        if (CamposNoEditar.IndexOf(drRow["Nombre"].ToString()) != -1)
                            drRow["No_Editar"] = true;
                    }
                }
            }
            else if ((_currModelo.TipoModelo == WoTypeModel.TransactionContable) ||
                    (_currModelo.TipoModelo == WoTypeModel.TransactionNoContable))
            {


                foreach (DataRow drRow in dtDTO.Rows)
                {
                    if (EstadoInicial == 0)
                    {
                        List<string> CamposNoEditar = new List<string>(new string[] { WoConst.WOUDNID, WoConst.WOSERIE, WoConst.WOFOLIO, "Id", "WoState" });
                        if (CamposNoEditar.IndexOf(drRow["Nombre"].ToString()) != -1)
                            drRow["No_Editar"] = true;
                    }
                    else
                    {
                        List<string> CamposNoEditar = new List<string>(new string[] { "Id", WoConst.ROWVERSION, "WoState" });
                        if (CamposNoEditar.IndexOf(drRow["Nombre"].ToString()) != -1)
                            drRow["No_Editar"] = true;
                    }
                }
            }


        }


        public void EngruesaTransicion(List<string> slTransiciones)
        {
            foreach (var transicion in _currDiagrama.Transiciones)
            {
                transicion.Selected = slTransiciones.IndexOf(transicion.Id) != -1;
            }

            diagramControl1.Refresh();
        }

    }
}