using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.Utils.Extensions;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using ServiceStack;
using ServiceStack.Text;
using Svg;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Menus.MenuModels;
using WooW.SB.Themes;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Menus
{
    public partial class WoMenuDesigner : UserControl
    {
        #region Temas

        /// <summary>
        /// Instancia del selector de teamas
        /// </summary>
        private WoThemeSelector _woThemeSelector = new WoThemeSelector();

        private string _themeSelected { get; set; } = "Default";

        /// <summary>
        /// Agrega el selector de temas al componente
        /// </summary>
        private void InstanceThemeSelector()
        {
            _woThemeSelector.ApliedThemeEvt += AplyTheme;

            pnlThemes.AddControl(_woThemeSelector);
            _woThemeSelector.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Aplica el tema seleccionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="theme"></param>
        private void AplyTheme(object sender, string theme)
        {
            _themeSelected = theme;
            _menuView.Refresh();
        }

        #endregion Temas


        #region Catalogos

        /// <summary>
        /// Catalogos generales
        /// </summary>
        private WoCommonDesignOptions _woCommonDesignOptions = new WoCommonDesignOptions();

        /// <summary>
        /// Catalogos recuperados de los temas
        /// </summary>
        private WoThemeOptions _woThemeOptions = WoThemeOptions.GetInstance();

        #endregion Catalogos

        #region Dibujado de los nodos

        /// <summary>
        /// Dibujado principal del nodo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void menusTree_CustomDrawNodeCell(
            object sender,
            DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e
        )
        {
            for (int i = 0; i < lbcAllProcess.ItemCount; i++)
            {
                lbcAllProcess.SetSelected(i, false);
            }
            for (int i = 0; i < lbcSelectedProcess.ItemCount; i++)
            {
                lbcSelectedProcess.SetSelected(i, false);
            }
            TreeList treeList = (TreeList)sender;
            TreeListNode selectedNode = treeList.FocusedNode;
            dynamic aux = (WoMenuProperties)selectedNode.Data;
            //if (aux == null)
            //{
            //    if (aux.TypeContainer == eTypeContainer.Menu)
            //    {
            //        _selectedNode = aux;
            //    }
            //}

            e.Handled = false;
            dynamic auxWoContainer = e.Node.Data;
            if (auxWoContainer is TreeListNode)
            {
                return;
            }

            #region Variables principales

            // Recuperación del componente.
            WoMenuProperties componentProperties = null;
            if (e.Node.Tag is WoMenuProperties)
            {
                componentProperties = (WoMenuProperties)e.Node.Tag;
            }
            else
            {
                dynamic woConvert = e.Node.Tag;
                if (woConvert == null)
                    return; //Send log and trow ToDo
                componentProperties = woConvert.ConvertToComponentProperties();
            }

            string maskText =
                (
                    componentProperties.MaskText == null
                    || componentProperties.MaskText == string.Empty
                )
                    ? componentProperties.Id
                    : componentProperties.MaskText;

            // Variables generales
            WoColor borderColor = new WoColor();
            borderColor.SolidColor = Color.Gray;

            int borderWith = 1;

            int marginLabelControl = 3;

            int fontSize = 5;
            int iconSize = 10;
            int marginBorder = 4;
            int iconTextMargin = 3;

            fontSize = 10;
            iconSize = 15;
            //switch (componentProperties.FontSize)
            //{
            //    case eTextSize.Small:
            //        fontSize = 5;
            //        iconSize = 10;
            //        break;
            //    case eTextSize.Normal:
            //        fontSize = 10;
            //        iconSize = 15;
            //        break;
            //    case eTextSize.Large:
            //        fontSize = 12;
            //        iconSize = 20;
            //        break;
            //}

            Font font = new Font("Arial", fontSize, FontStyle.Regular);
            if (componentProperties.Wide != eTextWeightMenu.Normal)
            {
                if (componentProperties.Wide == eTextWeightMenu.Bold)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Bold);
                }
                //else if (componentProperties.Wide == eTextWeight.Light)
                //{
                //    font = new Font("Arial", fontSize, font.Style | FontStyle.Regular);
                //}
            }
            if (componentProperties.Italic == eTextItalic.Italic)
            {
                font = new Font("Arial", fontSize, font.Style | FontStyle.Italic);
            }
            if (componentProperties.Decoration != eTextDecoration.None)
            {
                if (componentProperties.Decoration == eTextDecoration.Underline)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Underline);
                }
                else if (componentProperties.Decoration == eTextDecoration.Through)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Strikeout);
                }
            }

            // Creación de las variables base
            SizeF textSize = e.Graphics.MeasureString(maskText, font);
            RectangleF bounds = e.Bounds;

            int yTextBase = (int)(e.Bounds.Y + (25 - textSize.Height) / 2);
            int yIconBase = (int)(e.Bounds.Y + (25 - iconSize) / 2);

            #endregion Variables principales

            #region Dibujado del fondo Root

            WoColor backgroundColorRoot = new WoColor();

            backgroundColorRoot = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.BackgroundRoot.ToString()
            );

            LinearGradientBrush backgroundBrushRoot = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                backgroundColorRoot.SolidColor,
                backgroundColorRoot.SolidColor,
                180
            );

            if (backgroundColorRoot.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    backgroundColorRoot.GradientColor1,
                    backgroundColorRoot.GradientColor2,
                    backgroundColorRoot.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                backgroundBrushRoot.InterpolationColors = colorBlend;
            }
            //if (componentProperties.TypeContainer != eTypeContainer.Menu)
            //{
            e.Graphics.FillRectangle(backgroundBrushRoot, e.Bounds);
            //}
            #endregion Dibujado del fondo Root

            #region Dibujado del fondo

            WoColor backgroundColor = new WoColor();

            backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.BackgroundColor.ToString()
            );

            LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                backgroundColor.SolidColor,
                backgroundColor.SolidColor,
                180
            );

            if (backgroundColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    backgroundColor.GradientColor1,
                    backgroundColor.GradientColor2,
                    backgroundColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                backgroundBrush.InterpolationColors = colorBlend;
            }
            if (componentProperties.TypeContainer != eTypeContainer.Menu)
            {
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            }
            #endregion Dibujado del fondo

            #region Calculo del inicio de icono y texto

            int xBase = e.Bounds.Left + marginBorder;

            int xTextBase = 0;
            int xIconBase = 0;

            int labelSize = e.Bounds.Left + marginBorder + (int)textSize.Width + marginLabelControl;

            if (componentProperties.Icon == null)
            {
                xTextBase = xBase;
            }
            else
            {
                xIconBase = xBase;
                xTextBase = xBase + iconTextMargin + iconSize;
                labelSize =
                    e.Bounds.Left
                    + marginBorder
                    + iconSize
                    + iconTextMargin
                    + (int)textSize.Width
                    + marginLabelControl;
            }

            #endregion Dibujado del fondo Root

            #region Dibujado
            WoColor fontColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.FontColor.ToString()
            );

            if (componentProperties.Icon != null && componentProperties.Icon != "Sin Icono")
            {
                System.Drawing.Image icon = null;
                string rute = "";
                if (componentProperties.Icon.ToString() != "0")
                {
                    rute =
                        $@"{_iconPath}\{_woCommonDesignOptions.FileIcons.Get(componentProperties.Icon.ToString())}";
                    //var a = $@"{_iconPath}\{componentProperties.Icon.ToString()}.svg";
                    var svgDoc = SvgDocument.Open(rute);
                    for (int i = 0; i < svgDoc.Children.Count; i++)
                    {
                        svgDoc.Children[i].Fill = new SvgColourServer(fontColor.SolidColor);
                    }
                    icon = new Bitmap(svgDoc.Draw());

                    e.Graphics.DrawImage(icon, xIconBase, yIconBase, iconSize, iconSize);
                }
            }
            else
            {
                if (componentProperties.TypeContainer == eTypeContainer.Menu)
                {
                    if (componentProperties.Icon != null)
                    {
                        e.Graphics.FillRectangle(
                            backgroundBrushRoot,
                            new Rectangle(xIconBase, yIconBase, iconSize, iconSize)
                        );
                    }

                    xTextBase = xBase;
                }
                else
                {
                    if (componentProperties.Icon != null)
                    {
                        e.Graphics.FillRectangle(
                            backgroundBrush,
                            new Rectangle(xIconBase, yIconBase, iconSize, iconSize)
                        );
                    }
                    xTextBase = xBase;
                }
            }

            LinearGradientBrush fontBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                fontColor.SolidColor,
                fontColor.SolidColor,
                180
            );

            if (fontColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    fontColor.GradientColor1,
                    fontColor.GradientColor2,
                    fontColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                fontBrush.InterpolationColors = colorBlend;
            }

            e.Graphics.DrawString(maskText, font, fontBrush, xTextBase, yTextBase);

            #endregion Dibujado

            #region Dibujado del borde Root

            borderColor = new WoColor();

            borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.BorderColorRoot.ToString()
            );

            LinearGradientBrush bordeBrushRoot = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 1
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                borderColor.SolidColor,
                borderColor.SolidColor,
                180
            );

            if (borderColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    borderColor.GradientColor1,
                    borderColor.GradientColor2,
                    borderColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                bordeBrushRoot.InterpolationColors = colorBlend;
            }

            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(bordeBrushRoot, 2f),
                e.Bounds.X - 1,
                e.Bounds.Y - 1,
                e.Bounds.Width + 2,
                e.Bounds.Height + 1
            );

            #endregion Dibujado del borde Root

            #region Dibujado del borde

            borderColor = new WoColor();

            borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.BorderColor.ToString()
            );

            LinearGradientBrush bordeBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 1
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                borderColor.SolidColor,
                borderColor.SolidColor,
                180
            );

            if (borderColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    borderColor.GradientColor1,
                    borderColor.GradientColor2,
                    borderColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                bordeBrush.InterpolationColors = colorBlend;
            }
            if (componentProperties.TypeContainer != eTypeContainer.Menu)
            {
                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(bordeBrush, 2f),
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 1
                );
            }

            #endregion Dibujado del borde

            #region Indicador de seleccionado

            if (componentProperties.Selected)
            {
                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(Color.Blue, 2f),
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 1
                );
            }

            #endregion Indicador de seleccionado

            e.Handled = true;
        }

        #endregion Dibujado de los nodos

        #region Funciones auxiliares

        static string QuitarEspaciosYGuiones(string input)
        {
            // Crear un diccionario para mapear números a nombres
            Dictionary<char, string> numerosNombres = new Dictionary<char, string>
            {
                { '0', "cero" },
                { '1', "uno" },
                { '2', "dos" },
                { '3', "tres" },
                { '4', "cuatro" },
                { '5', "cinco" },
                { '6', "seis" },
                { '7', "siete" },
                { '8', "ocho" },
                { '9', "nueve" }
            };

            // Reemplaza los espacios con cadena vacía ""
            string sinEspacios = input.Replace(" ", "");

            // Reemplaza los guiones "-" con cadena vacía ""
            string sinGuiones = sinEspacios.Replace("-", "");

            // Inicializa una cadena para el resultado
            string resultado = "";

            // Itera a través de cada carácter de la cadena sin guiones
            foreach (char caracter in sinGuiones)
            {
                // Si el carácter es un número, reemplázalo con su nombre
                if (numerosNombres.ContainsKey(caracter))
                {
                    resultado += numerosNombres[caracter];
                }
                else
                {
                    resultado += caracter; // Mantén otros caracteres sin cambios
                }
            }

            return resultado;
        }
        #endregion Funciones auxiliares
    }
}
