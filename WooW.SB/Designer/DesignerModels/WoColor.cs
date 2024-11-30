using System.Drawing;

namespace WooW.SB.Designer.DesignerModels
{
    public enum eWoColorType
    {
        Gradient,
        Solid
    }

    public class WoColor
    {
        #region Tipo

        public eWoColorType ColorType { get; set; } = eWoColorType.Solid;

        #endregion Tipo

        #region SolidColor

        public Color SolidColor { get; set; } = Color.White;

        #endregion SolidColor

        #region GradientColor

        public Color GradientColor1 { get; set; } = Color.White;
        public Color GradientColor2 { get; set; } = Color.White;
        public Color GradientColor3 { get; set; } = Color.White;
        public float GradientAngle { get; set; } = 180;

        #endregion GradientColor
    }
}
