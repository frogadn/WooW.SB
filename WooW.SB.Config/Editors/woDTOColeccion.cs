using System;
using System.Data;
using System.Linq;
using WooW.Core;
using WooW.Core.Common;

namespace WooW.SB.Config
{
    public partial class woDTOColeccion : DevExpress.XtraEditors.XtraUserControl
    {
        private Modelo modelo;
        private ModeloDiagramaTransicionDTOColeccion DTO;
        //public ModeloDiagramaTransicionDTOColeccion DTOColeccion
        //{
        //    get
        //    {
        //        var d = new ModeloDiagramaTransicionDTOColeccion();
        //        d.ModeloId = modelo.Id;
        //        d.Insertar = optInsertar.Checked;
        //        d.Actualizar = optActualizar.Checked;
        //        d.Borrar = optBorrar.Checked;
        //        foreach (CheckedListBoxItem i in lstDTO.Items)
        //            if (i.CheckState == CheckState.Checked)
        //                d.Columnas.Add(i.Value.ToString());
        //        return d;
        //    }
        //}

        public woDTOColeccion(Modelo _modelo)
        {
            InitializeComponent();

            modelo = _modelo;

            DataTable dtDTO = new DataTable();
            dtDTO.Columns.Add(@"Selección", typeof(bool));
            dtDTO.Columns.Add(@"No_Editar", typeof(bool));
            dtDTO.Columns.Add(@"Nombre", typeof(string));
            grdDTO.DataSource = dtDTO;

            grdDTOView.ValidateRow += GrdDTOView_ValidateRow;
            grdDTOView.Columns["Nombre"].OptionsColumn.AllowEdit = false;

            foreach (var col in modelo.Columnas.OrderBy(j => j.Orden))
            {
                DataRow drRow = dtDTO.NewRow();
                drRow["Selección"] = false;
                drRow["No_Editar"] = false;
                drRow["Nombre"] = col.Id;
                dtDTO.Rows.Add(drRow);
            }
        }

        private void GrdDTOView_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            if (DTO == null)
                return;

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


            DTO.Columnas.Clear();
            DTO.ColumnasNoEditar.Clear();

            bool bRenglon = false;
            DataTable dtDTO = (grdDTO.DataSource as DataTable);
            foreach (DataRow drRow in dtDTO.Rows)
            {
                if (drRow["Selección"].ToBoolean())
                {
                    DTO.Columnas.Add(drRow["Nombre"].ToString());
                    if (drRow["No_Editar"].ToBoolean())
                    {
                        DTO.ColumnasNoEditar.Add(drRow["Nombre"].ToString());
                        if (drRow["Nombre"].ToString() == WoConst.WORENGLON)
                            bRenglon = true;
                    }
                }
            }
            if (!bRenglon)
            {
                DataRow[] drRow = dtDTO.Select("Nombre = 'Renglon'");

                if ((drRow.Length > 0) && (DTO.Columnas.Where(e => e == WoConst.WORENGLON).FirstOrDefault() != null))
                {
                    DTO.ColumnasNoEditar.Add(WoConst.WORENGLON);
                    drRow[0]["No_Editar"] = true;
                }
            }
        }

        private void woDTOColeccion_Load(object sender, EventArgs e)
        {
        }

        public void Asignar(ModeloDiagramaTransicionDTOColeccion locDto)
        {
            DTO = locDto;
            optInsertar.Checked = DTO.Insertar;
            optActualizar.Checked = DTO.Actualizar;
            optBorrar.Checked = DTO.Borrar;

            DataTable dtDTO = (grdDTO.DataSource as DataTable);
            foreach (DataRow drRow in dtDTO.Rows)
            {
                drRow["Selección"] = false;
                drRow["No_Editar"] = false;
            }

            foreach (string s in DTO.Columnas)
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

            foreach (string s in DTO.ColumnasNoEditar)
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

            GrdDTOView_ValidateRow(null, null);
        }


        private void optInsertar_CheckedChanged(object sender, EventArgs e)
        {
            DTO.Insertar = optInsertar.Checked;
        }

        private void optActualizar_CheckedChanged(object sender, EventArgs e)
        {
            DTO.Actualizar = optActualizar.Checked;
        }

        private void optBorrar_CheckedChanged(object sender, EventArgs e)
        {
            DTO.Borrar = optBorrar.Checked;
        }

    }
}