namespace WooW.SB.GeneratorComponentBase.GeneratorComponent
{
    public class WoDesignerCommons { }

    #region Tipos de control

    public enum eTypeContainer
    {
        None,
        FormRoot,
        FormTabGroup,
        FormTab,
        FormGroup,
        Grid,
        SubMenu,
        Menu
    }

    public enum eTypeItem
    {
        None,
        MenuItem,
        FormItem,
        Slave,
        ReportItem,
        List
    }

    #endregion Tipos de control

    #region Iconos

    public enum eEstiloIcono
    {
        Icono,
        Etiqueta_Icono
    }

    public enum ePosicionImagen
    {
        Derecha,
        Izquieda
    }

    #endregion Iconos
}
