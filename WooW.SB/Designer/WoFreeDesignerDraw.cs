using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using Microsoft.CodeAnalysis;
using ServiceStack.Text;
using Svg;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer
{
    public partial class WoFreeDesigner : UserControl
    {
        #region Dibujado

        /// <summary>
        /// Indica si los controles que se mostraran son de tamaño para el ancho.
        /// </summary>
        private bool _sizeModifyW = false;

        /// <summary>
        /// Indica si los controles mostrados son de tamaño para el largo.
        /// </summary>
        private bool _sizeModifyH = false;

        /// <summary>
        /// Path base de los iconos.
        /// </summary>
        private string _iconPath = string.Empty;

        [SupportedOSPlatform("windows")]
        private void FormCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.Handled = false;

            #region Variables principales

            // Recuperación del componente.
            WoComponentProperties componentProperties = (WoComponentProperties)e.Item.Tag;
            string maskText = componentProperties.MaskText;

            // Variables generales
            WoColor borderColor = new WoColor();
            int borderWith = 1;

            int fontSize = 5;
            int iconSize = 10;
            int marginBorder = 4;
            int iconTextMargin = 3;

            switch (componentProperties.ComponentFontSize)
            {
                case eTextSize.Small:
                    fontSize = 5;
                    iconSize = 10;
                    break;
                case eTextSize.Normal:
                    fontSize = 10;
                    iconSize = 15;
                    break;
                case eTextSize.Large:
                    fontSize = 12;
                    iconSize = 20;
                    break;
            }

            Font font = new Font("Arial", fontSize, FontStyle.Regular);
            if (componentProperties.ComponentFontWide != eTextWeight.Normal)
            {
                if (componentProperties.ComponentFontWide == eTextWeight.Bold)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Bold);
                }
                else if (componentProperties.ComponentFontWide == eTextWeight.Normal)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Regular);
                }
            }
            if (componentProperties.ComponentFontItalic == eTextItalic.Italic)
            {
                font = new Font("Arial", fontSize, font.Style | FontStyle.Italic);
            }
            if (componentProperties.ComponentFontDecoration != eTextDecoration.None)
            {
                if (componentProperties.ComponentFontDecoration == eTextDecoration.Underline)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Underline);
                }
                else if (componentProperties.ComponentFontDecoration == eTextDecoration.Through)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Strikeout);
                }
            }

            // Creación de las variables base
            SizeF textSize = e.Graphics.MeasureString(maskText, font); // Tamaño del texto
            RectangleF bounds = e.Bounds; // Rectángulo que se dibujará como fondo
            int yTextBase = (int)(e.Bounds.Y + (25 - textSize.Height) / 2);
            int yIconBase = (int)(e.Bounds.Y + (25 - iconSize) / 2);

            #endregion Variables principales

            #region Dibujado del fondo

            WoColor backgroundColor = new WoColor();

            if (componentProperties.BackgorundColorContainerItem.ToString().Contains("Border"))
            {
                backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Default");
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties
                        .BackgorundColorContainerItem.ToString()
                        .Replace("Border", "")
                );
            }
            else
            {
                backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties.BackgorundColorContainerItem.ToString()
                );
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Border");
            }

            LinearGradientBrush brush = new LinearGradientBrush(
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
                brush.InterpolationColors = colorBlend;
            }

            e.Graphics.FillRectangle(brush, e.Bounds);

            #endregion Dibujado del fondo

            #region Dibujado del borde

            LinearGradientBrush borderBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
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
                borderBrush.InterpolationColors = colorBlend;
            }

            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(borderBrush),
                e.Bounds.X,
                e.Bounds.Y,
                e.Bounds.Width,
                26
            );

            // Dibujado del borde.
            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(borderBrush),
                e.Bounds.X,
                e.Bounds.Y,
                e.Bounds.Width,
                e.Bounds.Height
            );

            #endregion Dibujado del borde

            #region Calculo del inicio de icono y texto

            int labelSize = e.Bounds.Left + marginBorder + (int)textSize.Width + 2;
            int xBase = e.Bounds.Left + marginBorder;

            int xTextBase = 0;
            int xIconBase = 0;

            if (componentProperties.Icon == eBoostrapIcons.None)
            {
                xTextBase = xBase;
            }

            #endregion Calculo del inicio de icono y texto

            #region Dibujado

            WoColor fontColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.ComponentFontColor.ToString()
            );

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

            if (componentProperties.Icon != eBoostrapIcons.None)
            {
                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open(
                    $@"{_iconPath}\{_woCommonDesignOptions.BoostrapIcons.Get(componentProperties.Icon.ToString())}"
                );
                for (int i = 0; i < svgDoc.Children.Count; i++)
                {
                    svgDoc.Children[i].Fill = new SvgColourServer(fontColor.SolidColor);
                }
                icon = new Bitmap(svgDoc.Draw());

                e.Graphics.DrawImage(icon, xIconBase, yIconBase, iconSize, iconSize);
            }

            #endregion Dibujado

            #region Dibujado interno

            e.Graphics.DrawString("Item", font, fontBrush, xTextBase, yTextBase + 30);

            e.Graphics.DrawString("Item", font, fontBrush, xTextBase, yTextBase + 60);

            e.Graphics.DrawString("Item", font, fontBrush, xTextBase, yTextBase + 90);

            int widthComponent = e.Bounds.Width - labelSize + e.Bounds.Left - 5;

            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(borderBrush),
                labelSize,
                e.Bounds.Y + 30,
                widthComponent,
                25
            );

            e.Graphics.DrawString(
                $@"Empty",
                font,
                fontBrush,
                labelSize + marginBorder,
                yTextBase + 30
            );

            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(borderBrush),
                labelSize,
                e.Bounds.Y + 60,
                widthComponent,
                25
            );

            e.Graphics.DrawString(
                $@"Empty",
                font,
                fontBrush,
                labelSize + marginBorder,
                yTextBase + 60
            );

            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(borderBrush),
                labelSize,
                e.Bounds.Y + 90,
                widthComponent,
                25
            );

            e.Graphics.DrawString(
                $@"Empty",
                font,
                fontBrush,
                labelSize + marginBorder,
                yTextBase + 90
            );

            #endregion Dibujado interno

            #region Indicador de seleccionado

            if (componentProperties.Selected)
            {
                #region Variables principales

                int sizeWidthIndicator = 24;
                int sizeHeightIndicator = 12;

                int sizeIconIndicator = 20;

                Color baseControlsColor = Color.Blue;
                LinearGradientBrush baseControlsBrush = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlsColor,
                    baseControlsColor,
                    180
                );
                Color baseControlColorSecondary = Color.Orange;
                LinearGradientBrush baseControlsBrushSecondary = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlColorSecondary,
                    baseControlColorSecondary,
                    180
                );

                #endregion Variables principales

                #region Indicador principal

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(baseControlsBrush),
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 2
                );

                #endregion Indicador principal

                if (!_sizeModifyW && !_sizeModifyH)
                {
                    #region Control de eliminado

                    int baseXDeleteControl = e.Bounds.Right - 26;

                    ///Delete
                    _startXButtonDelete = baseXDeleteControl;
                    _endXButtonDelete = _startXButtonDelete + sizeWidthIndicator;

                    _startYButtonDelete = e.Bounds.Y + 1;
                    _endYButtonDelete = _startYButtonDelete + (sizeHeightIndicator * 2);

                    Rectangle btnDelete = new Rectangle(
                        _startXButtonDelete,
                        _startYButtonDelete,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDelete);

                    System.Drawing.Image iconBtnDelete = null;
                    SvgDocument svgIconDelete = SvgDocument.Open(_pathIconDelete);
                    for (int i = 0; i < svgIconDelete.Children.Count; i++)
                    {
                        svgIconDelete.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnDelete = new Bitmap(svgIconDelete.Draw());

                    e.Graphics.DrawImage(
                        iconBtnDelete,
                        _startXButtonDelete + 2,
                        _startYButtonDelete + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    int baseXLabelControl = baseXDeleteControl - 26;

                    ///Label
                    _startXButtonLabel = baseXLabelControl;
                    _endXButtonLabel = _startXButtonLabel + sizeWidthIndicator;

                    _startYButtonLabel = e.Bounds.Y + 1;
                    _endYButtonLabel = _startYButtonLabel + (sizeHeightIndicator * 2);

                    Rectangle btnLabelControl = new Rectangle(
                        _startXButtonLabel,
                        _startYButtonLabel,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLabelControl);

                    System.Drawing.Image iconBtnLabelControl = null;
                    SvgDocument svgIconLabelControl = SvgDocument.Open(_pathIconLabel);
                    for (int i = 0; i < svgIconLabelControl.Children.Count; i++)
                    {
                        svgIconLabelControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnLabelControl = new Bitmap(svgIconLabelControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLabelControl,
                        _startXButtonLabel + 2,
                        _startYButtonLabel + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    int baseXWidthControl = baseXLabelControl - 26;

                    ///Width
                    _startXButtonWidth = baseXWidthControl;
                    _endXButtonWidth = _startXButtonWidth + sizeWidthIndicator;

                    _startYButtonWidth = e.Bounds.Y + 1;
                    _endYButtonWidth = _startYButtonWidth + (sizeHeightIndicator * 2);

                    Rectangle btnWidthControl = new Rectangle(
                        _startXButtonWidth,
                        _startYButtonWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnWidthControl);

                    System.Drawing.Image iconBtnWidthControl = null;
                    SvgDocument svgIconWidthControl = SvgDocument.Open(_buttonWidth);
                    for (int i = 0; i < svgIconWidthControl.Children.Count; i++)
                    {
                        svgIconWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnWidthControl = new Bitmap(svgIconWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnWidthControl,
                        _startXButtonWidth + 2,
                        _startYButtonWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    int baseXHeightControl = baseXWidthControl - 26;

                    ///Width
                    _startXButtonHeight = baseXHeightControl;
                    _endXButtonHeight = _startXButtonHeight + sizeWidthIndicator;

                    _startYButtonHeight = e.Bounds.Y + 1;
                    _endYButtonHeight = _startYButtonHeight + (sizeHeightIndicator * 2);

                    Rectangle btnHeightControl = new Rectangle(
                        _startXButtonHeight,
                        _startYButtonHeight,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnHeightControl);

                    System.Drawing.Image iconBtnHeightControl = null;
                    SvgDocument svgIconHeightControl = SvgDocument.Open(_buttonHeight);
                    for (int i = 0; i < svgIconHeightControl.Children.Count; i++)
                    {
                        svgIconHeightControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnHeightControl = new Bitmap(svgIconHeightControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnHeightControl,
                        _startXButtonHeight + 2,
                        _startYButtonHeight + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs
                }
                else if (_sizeModifyW)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicCol.Clear();
                    int iLast = 13 - (e.Item.OptionsTableLayoutItem.ColumnIndex);

                    for (int i = 3; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnLeftMove = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.ColumnSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnLeftMove
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnLeftMove);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLeftMove);

                            e.Graphics.FillRectangle(baseControlsBrush, btnLeftMove);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicCol.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
                else if (_sizeModifyH)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicRow.Clear();

                    int iLast =
                        (e.Item.Parent.OptionsTableLayoutGroup.RowCount)
                        - (e.Item.OptionsTableLayoutItem.RowIndex)
                        + 1;

                    for (int i = 1; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnDinamicH = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.RowSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnDinamicH
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnDinamicH);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDinamicH);

                            e.Graphics.FillRectangle(baseControlsBrush, btnDinamicH);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicRow.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
            }

            #endregion Indicador de seleccionado

            e.Handled = true;
        }

        [SupportedOSPlatform("windows")]
        private void ItemCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.Handled = false;

            #region Variables principales

            // Recuperación del componente.
            WoComponentProperties componentProperties = (WoComponentProperties)e.Item.Tag;
            string maskText = componentProperties.MaskText;
            string parentName = (e.Item.Parent == null) ? "Root" : e.Item.Parent.Name;

            int marginLabelControl = 3;

            int fontSize = 5;
            int iconSize = 10;
            int marginBorder = 4;
            int iconTextMargin = 3;
            int componentSize = 26;

            switch (componentProperties.ItemSize)
            {
                case eItemSize.Small:
                    fontSize = 5;
                    iconSize = 10;
                    marginBorder = 2;
                    componentSize = 15;
                    break;
                case eItemSize.Normal:
                    fontSize = 10;
                    iconSize = 15;
                    componentSize = 26;
                    break;
                case eItemSize.Large:
                    fontSize = 12;
                    iconSize = 20;
                    componentSize = 30;
                    break;
            }

            #endregion Variables principales

            #region Dibujado del fondo principal

            WoColor backgroundColor = new WoColor();
            WoComponentProperties parentComponentProperties =
                (e.Item.Parent == null)
                    ? (WoComponentProperties)_layoutDesigner.Root.Tag
                    : (WoComponentProperties)e.Item.Parent.Tag;
            backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                parentComponentProperties.BackgorundColorGroup.ToString()
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

            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);

            #endregion Dibujado del fondo principal

            #region Creación de la fuente del caption

            Font fontCaption = new Font("Arial", fontSize, FontStyle.Regular);
            if (componentProperties.CaptionWide != eTextWeight.Normal)
            {
                if (componentProperties.CaptionWide == eTextWeight.Bold)
                {
                    fontCaption = new Font("Arial", fontSize, fontCaption.Style | FontStyle.Bold);
                }
                else if (componentProperties.CaptionWide == eTextWeight.Normal)
                {
                    fontCaption = new Font(
                        "Arial",
                        fontSize,
                        fontCaption.Style | FontStyle.Regular
                    );
                }
            }
            if (componentProperties.CaptionItalic == eTextItalic.Italic)
            {
                fontCaption = new Font("Arial", fontSize, fontCaption.Style | FontStyle.Italic);
            }
            if (componentProperties.CaptionDecoration != eTextDecoration.None)
            {
                if (componentProperties.CaptionDecoration == eTextDecoration.Underline)
                {
                    fontCaption = new Font(
                        "Arial",
                        fontSize,
                        fontCaption.Style | FontStyle.Underline
                    );
                }
                else if (componentProperties.CaptionDecoration == eTextDecoration.Through)
                {
                    fontCaption = new Font(
                        "Arial",
                        fontSize,
                        fontCaption.Style | FontStyle.Strikeout
                    );
                }
            }

            // Creación de las variables base
            SizeF textSize = e.Graphics.MeasureString(maskText, fontCaption);
            RectangleF bounds = e.Bounds;

            int yTextBase = (int)(e.Bounds.Y + (componentSize - textSize.Height) / 2);
            int yIconBase = (int)(e.Bounds.Y + (componentSize - iconSize) / 2);

            #endregion Creación de la fuente del caption

            #region Calculo del inicio de icono y texto

            int xBase = e.Bounds.Left + marginBorder;

            int xTextBase = 0;
            int xIconBase = 0;

            int labelSize = e.Bounds.Left + marginBorder + (int)textSize.Width + marginLabelControl;

            if (componentProperties.Icon == eBoostrapIcons.None)
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

            #endregion Calculo del inicio de icono y texto

            #region Dibujado del caption

            WoColor fontColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.CaptionColor.ToString()
            );

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

            e.Graphics.DrawString(maskText, fontCaption, fontBrush, xTextBase, yTextBase);

            if (componentProperties.Icon != eBoostrapIcons.None)
            {
                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open(
                    $@"{_iconPath}\{_woCommonDesignOptions.BoostrapIcons.Get(componentProperties.Icon.ToString())}"
                );
                for (int i = 0; i < svgDoc.Children.Count; i++)
                {
                    svgDoc.Children[i].Fill = new SvgColourServer(fontColor.SolidColor);
                }
                icon = new Bitmap(svgDoc.Draw());

                e.Graphics.DrawImage(icon, xIconBase, yIconBase, iconSize, iconSize);
            }

            #endregion Dibujado del caption

            #region Creación de la fuente del input

            Font componentFont = new Font("Arial", fontSize, FontStyle.Regular);
            if (componentProperties.ComponentFontWide != eTextWeight.Normal)
            {
                if (componentProperties.ComponentFontWide == eTextWeight.Bold)
                {
                    componentFont = new Font(
                        "Arial",
                        fontSize,
                        componentFont.Style | FontStyle.Bold
                    );
                }
                else if (componentProperties.ComponentFontWide == eTextWeight.Normal)
                {
                    componentFont = new Font(
                        "Arial",
                        fontSize,
                        componentFont.Style | FontStyle.Regular
                    );
                }
            }
            if (componentProperties.ComponentFontItalic == eTextItalic.Italic)
            {
                componentFont = new Font("Arial", fontSize, componentFont.Style | FontStyle.Italic);
            }
            if (componentProperties.ComponentFontDecoration != eTextDecoration.None)
            {
                if (componentProperties.ComponentFontDecoration == eTextDecoration.Underline)
                {
                    componentFont = new Font(
                        "Arial",
                        fontSize,
                        componentFont.Style | FontStyle.Underline
                    );
                }
                else if (componentProperties.ComponentFontDecoration == eTextDecoration.Through)
                {
                    componentFont = new Font(
                        "Arial",
                        fontSize,
                        componentFont.Style | FontStyle.Strikeout
                    );
                }
            }

            WoColor componentFontColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.ComponentFontColor.ToString()
            );

            LinearGradientBrush componentFontBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                componentFontColor.SolidColor,
                componentFontColor.SolidColor,
                180
            );

            if (componentFontColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    componentFontColor.GradientColor1,
                    componentFontColor.GradientColor2,
                    componentFontColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                componentFontBrush.InterpolationColors = colorBlend;
            }

            #endregion Creación de la fuente del input

            #region Dibujado del control

            WoColor backgroundControlColor = new WoColor();
            WoColor borderColor = new WoColor();

            if (componentProperties.BackgorundColorContainerItem.ToString().Contains("Border"))
            {
                backgroundControlColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Default");
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties
                        .BackgorundColorContainerItem.ToString()
                        .Replace("Border", "")
                );
            }
            else
            {
                backgroundControlColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties.BackgorundColorContainerItem.ToString()
                );
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Border");
            }

            LinearGradientBrush backgroundControlBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                backgroundControlColor.SolidColor,
                backgroundControlColor.SolidColor,
                180
            );

            if (backgroundControlColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    backgroundControlColor.GradientColor1,
                    backgroundControlColor.GradientColor2,
                    backgroundControlColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                backgroundControlBrush.InterpolationColors = colorBlend;
            }

            LinearGradientBrush borderBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
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
                borderBrush.InterpolationColors = colorBlend;
            }

            (string parent, int colIndex, int labelSize) findLabelSize = _labelSizes
                .Where(x =>
                    x.parent == e.Item.Parent.Name
                    && x.colIndex == e.Item.OptionsTableLayoutItem.ColumnIndex
                )
                .FirstOrDefault();

            if (findLabelSize.parent == null)
            {
                _labelSizes.Add(
                    (e.Item.Parent.Name, e.Item.OptionsTableLayoutItem.ColumnIndex, labelSize)
                );
            }
            else
            {
                if (findLabelSize.labelSize < labelSize)
                {
                    _labelSizes.Remove(
                        _labelSizes
                            .Where(x =>
                                x.parent == e.Item.Parent.Name
                                && x.colIndex == e.Item.OptionsTableLayoutItem.ColumnIndex
                            )
                            .FirstOrDefault()
                    );

                    _labelSizes.Add(
                        (e.Item.Parent.Name, e.Item.OptionsTableLayoutItem.ColumnIndex, labelSize)
                    );
                }
                else
                {
                    labelSize = findLabelSize.labelSize;
                }
            }

            if (componentProperties.Control == "Text")
            {
                int widthComponent = e.Bounds.Width - labelSize + e.Bounds.Left;

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    componentSize
                );

                e.Graphics.DrawString(
                    $@"Empty",
                    componentFont,
                    componentFontBrush,
                    labelSize + marginBorder,
                    yTextBase
                );
            }
            else if (componentProperties.Control == "Date")
            {
                int unitComponent = componentSize + 2;
                int widthComponent = (e.Bounds.Width - labelSize + e.Bounds.Left) - unitComponent;

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    labelSize,
                    e.Bounds.Y,
                    widthComponent + unitComponent,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    componentSize
                );

                e.Graphics.DrawString(
                    $@"01/12/2023",
                    componentFont,
                    componentFontBrush,
                    labelSize + marginBorder,
                    yTextBase
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize + widthComponent,
                    e.Bounds.Y,
                    unitComponent,
                    componentSize
                );

                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open($@"{_iconPath}\calendar-week.svg");
                for (int i = 0; i < svgDoc.Children.Count; i++)
                {
                    svgDoc.Children[i].Fill = new SvgColourServer(borderColor.SolidColor);
                }
                icon = new Bitmap(svgDoc.Draw());

                e.Graphics.DrawImage(
                    icon,
                    labelSize + widthComponent + (unitComponent / 2) - (iconSize / 2),
                    yIconBase,
                    iconSize,
                    iconSize
                );
            }
            else if (
                componentProperties.Control == "Spin"
                || componentProperties.Control == "Decimal"
            )
            {
                int widthComponent = e.Bounds.Width - labelSize + e.Bounds.Left;

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    25
                );

                e.Graphics.DrawString(
                    $@"00",
                    componentFont,
                    componentFontBrush,
                    labelSize + marginBorder,
                    yTextBase
                );
            }
            else if (componentProperties.Control == "Memo")
            {
                int widthComponent = e.Bounds.Width - labelSize + e.Bounds.Left;

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    e.Bounds.Height
                );

                e.Graphics.DrawString(
                    $@"Empty",
                    componentFont,
                    componentFontBrush,
                    labelSize + marginBorder,
                    yTextBase
                );
            }
            else if (
                componentProperties.Control == "WoState"
                || componentProperties.Control == "EnumInt"
                || componentProperties.Control == "EnumString"
            )
            {
                int unitComponent = componentSize + 2;
                int widthComponent = (e.Bounds.Width - labelSize + e.Bounds.Left) - unitComponent;

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    labelSize,
                    e.Bounds.Y,
                    widthComponent + unitComponent,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize,
                    e.Bounds.Y,
                    widthComponent,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize + widthComponent,
                    e.Bounds.Y,
                    unitComponent,
                    componentSize
                );

                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open($@"{_iconPath}\caret-right-fill.svg");
                for (int i = 0; i < svgDoc.Children.Count; i++)
                {
                    svgDoc.Children[i].Fill = new SvgColourServer(borderColor.SolidColor);
                }
                icon = new Bitmap(svgDoc.Draw());

                e.Graphics.DrawImage(
                    icon,
                    labelSize + widthComponent + (unitComponent / 2) - (iconSize / 2),
                    yIconBase,
                    iconSize,
                    iconSize
                );

                e.Graphics.DrawString(
                    $@"Seleccione...",
                    componentFont,
                    componentFontBrush,
                    labelSize + marginBorder,
                    yTextBase
                );
            }
            else if (
                componentProperties.Control == "LookUp"
                || componentProperties.Control == "LookUpDialog"
            )
            {
                int widthComponent = e.Bounds.Width - labelSize + e.Bounds.Left;
                int sizeComponent2 = (widthComponent / 2) - 2;
                int unitComponent = componentSize + 2;
                int sizeComponent1 = (widthComponent / 2) - 2 - unitComponent;

                int secondLabel = labelSize + 4 + sizeComponent2;

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    labelSize,
                    e.Bounds.Y,
                    sizeComponent2,
                    componentSize
                );

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    secondLabel,
                    e.Bounds.Y,
                    sizeComponent2,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize,
                    e.Bounds.Y,
                    sizeComponent1,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize + sizeComponent1,
                    e.Bounds.Y,
                    componentSize,
                    componentSize
                );

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    secondLabel,
                    e.Bounds.Y,
                    sizeComponent2,
                    componentSize
                );

                e.Graphics.DrawString(
                    $@"Seleccione...",
                    componentFont,
                    componentFontBrush,
                    labelSize + marginBorder,
                    yTextBase
                );

                e.Graphics.DrawString(
                    $@"Descripción",
                    componentFont,
                    componentFontBrush,
                    labelSize + sizeComponent2 + 6,
                    yTextBase
                );

                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open($@"{_iconPath}\caret-down-fill.svg");
                for (int i = 0; i < svgDoc.Children.Count; i++)
                {
                    svgDoc.Children[i].Fill = new SvgColourServer(borderColor.SolidColor);
                }
                icon = new Bitmap(svgDoc.Draw());

                e.Graphics.DrawImage(
                    icon,
                    labelSize + sizeComponent1 + (unitComponent / 2) - (iconSize / 2),
                    yIconBase,
                    iconSize,
                    iconSize
                );
            }
            else if (componentProperties.Control == "Bool")
            {
                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    labelSize,
                    e.Bounds.Y,
                    componentSize,
                    componentSize
                );
            }

            #endregion Dibujado del control

            #region Indicador de seleccionado

            if (componentProperties.Selected)
            {
                #region Variables principales

                int sizeWidthIndicator = 24;
                int sizeHeightIndicator = 12;

                int sizeIconIndicator = 20;

                Color baseControlsColor = Color.Blue;
                LinearGradientBrush baseControlsBrush = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlsColor,
                    baseControlsColor,
                    180
                );
                Color baseControlColorSecondary = Color.Orange;
                LinearGradientBrush baseControlsBrushSecondary = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlColorSecondary,
                    baseControlColorSecondary,
                    180
                );

                #endregion Variables principales

                #region Indicador principal

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(baseControlsBrush),
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 2
                );

                #endregion Indicador principal

                if (!_sizeModifyW && !_sizeModifyH)
                {
                    #region Control de eliminado

                    int baseXDeleteControl = e.Bounds.Right - 26;

                    ///Delete
                    _startXButtonDelete = baseXDeleteControl;
                    _endXButtonDelete = _startXButtonDelete + sizeWidthIndicator;

                    _startYButtonDelete = e.Bounds.Y + 1;
                    _endYButtonDelete = _startYButtonDelete + (sizeHeightIndicator * 2);

                    Rectangle btnDelete = new Rectangle(
                        _startXButtonDelete,
                        _startYButtonDelete,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDelete);

                    System.Drawing.Image iconBtnDelete = null;
                    SvgDocument svgIconDelete = SvgDocument.Open(_pathIconDelete);
                    for (int i = 0; i < svgIconDelete.Children.Count; i++)
                    {
                        svgIconDelete.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnDelete = new Bitmap(svgIconDelete.Draw());

                    e.Graphics.DrawImage(
                        iconBtnDelete,
                        _startXButtonDelete + 2,
                        _startYButtonDelete + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    int baseXLabelControl = baseXDeleteControl - 26;

                    ///Label
                    _startXButtonLabel = baseXLabelControl;
                    _endXButtonLabel = _startXButtonLabel + sizeWidthIndicator;

                    _startYButtonLabel = e.Bounds.Y + 1;
                    _endYButtonLabel = _startYButtonLabel + (sizeHeightIndicator * 2);

                    Rectangle btnLabelControl = new Rectangle(
                        _startXButtonLabel,
                        _startYButtonLabel,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLabelControl);

                    System.Drawing.Image iconBtnLabelControl = null;
                    SvgDocument svgIconLabelControl = SvgDocument.Open(_pathIconLabel);
                    for (int i = 0; i < svgIconLabelControl.Children.Count; i++)
                    {
                        svgIconLabelControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnLabelControl = new Bitmap(svgIconLabelControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLabelControl,
                        _startXButtonLabel + 2,
                        _startYButtonLabel + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    int baseXWidthControl = baseXLabelControl - 26;

                    ///Width
                    _startXButtonWidth = baseXWidthControl;
                    _endXButtonWidth = _startXButtonWidth + sizeWidthIndicator;

                    _startYButtonWidth = e.Bounds.Y + 1;
                    _endYButtonWidth = _startYButtonWidth + (sizeHeightIndicator * 2);

                    Rectangle btnWidthControl = new Rectangle(
                        _startXButtonWidth,
                        _startYButtonWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnWidthControl);

                    System.Drawing.Image iconBtnWidthControl = null;
                    SvgDocument svgIconWidthControl = SvgDocument.Open(_buttonWidth);
                    for (int i = 0; i < svgIconWidthControl.Children.Count; i++)
                    {
                        svgIconWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnWidthControl = new Bitmap(svgIconWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnWidthControl,
                        _startXButtonWidth + 2,
                        _startYButtonWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    int baseXHeightControl = baseXWidthControl - 26;

                    ///Width
                    _startXButtonHeight = baseXHeightControl;
                    _endXButtonHeight = _startXButtonHeight + sizeWidthIndicator;

                    _startYButtonHeight = e.Bounds.Y + 1;
                    _endYButtonHeight = _startYButtonHeight + (sizeHeightIndicator * 2);

                    Rectangle btnHeightControl = new Rectangle(
                        _startXButtonHeight,
                        _startYButtonHeight,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnHeightControl);

                    System.Drawing.Image iconBtnHeightControl = null;
                    SvgDocument svgIconHeightControl = SvgDocument.Open(_buttonHeight);
                    for (int i = 0; i < svgIconHeightControl.Children.Count; i++)
                    {
                        svgIconHeightControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnHeightControl = new Bitmap(svgIconHeightControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnHeightControl,
                        _startXButtonHeight + 2,
                        _startYButtonHeight + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs
                }
                else if (_sizeModifyW)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicCol.Clear();
                    int iLast = 13 - (e.Item.OptionsTableLayoutItem.ColumnIndex);

                    for (int i = 3; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnLeftMove = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.ColumnSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnLeftMove
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnLeftMove);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLeftMove);

                            e.Graphics.FillRectangle(baseControlsBrush, btnLeftMove);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicCol.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
                else if (_sizeModifyH)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicRow.Clear();

                    int iLast =
                        (e.Item.Parent.OptionsTableLayoutGroup.RowCount)
                        - (e.Item.OptionsTableLayoutItem.RowIndex)
                        + 1;

                    for (int i = 1; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnDinamicH = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.RowSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnDinamicH
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnDinamicH);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDinamicH);

                            e.Graphics.FillRectangle(baseControlsBrush, btnDinamicH);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicRow.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
            }

            #endregion Indicador de seleccionado

            e.Handled = true;
        }

        [SupportedOSPlatform("windows")]
        private void ButtonCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.Handled = false;

            #region Variables principales

            // Recuperación del componente.
            WoComponentProperties componentProperties = (WoComponentProperties)e.Item.Tag;
            string maskText = componentProperties.MaskText;
            string parentName = e.Item.Parent.Name;

            int fontSize = 5;
            int iconSize = 10;
            int iconTextMargin = 3;

            switch (componentProperties.ItemSize)
            {
                case eItemSize.Small:
                    fontSize = 5;
                    iconSize = 10;
                    break;
                case eItemSize.Normal:
                    fontSize = 10;
                    iconSize = 15;
                    break;
                case eItemSize.Large:
                    fontSize = 12;
                    iconSize = 20;
                    break;
            }

            #endregion Variables principales

            #region Creación de la fuente del caption

            Font fontCaption = new Font("Arial", fontSize, FontStyle.Regular);
            if (componentProperties.ComponentFontWide != eTextWeight.Normal)
            {
                if (componentProperties.ComponentFontWide == eTextWeight.Bold)
                {
                    fontCaption = new Font("Arial", fontSize, fontCaption.Style | FontStyle.Bold);
                }
                else if (componentProperties.ComponentFontWide == eTextWeight.Normal)
                {
                    fontCaption = new Font(
                        "Arial",
                        fontSize,
                        fontCaption.Style | FontStyle.Regular
                    );
                }
            }
            if (componentProperties.ComponentFontItalic == eTextItalic.Italic)
            {
                fontCaption = new Font("Arial", fontSize, fontCaption.Style | FontStyle.Italic);
            }
            if (componentProperties.ComponentFontDecoration != eTextDecoration.None)
            {
                if (componentProperties.ComponentFontDecoration == eTextDecoration.Underline)
                {
                    fontCaption = new Font(
                        "Arial",
                        fontSize,
                        fontCaption.Style | FontStyle.Underline
                    );
                }
                else if (componentProperties.ComponentFontDecoration == eTextDecoration.Through)
                {
                    fontCaption = new Font(
                        "Arial",
                        fontSize,
                        fontCaption.Style | FontStyle.Strikeout
                    );
                }
            }

            // Creación de las variables base
            SizeF textSize = e.Graphics.MeasureString(maskText, fontCaption);
            RectangleF bounds = e.Bounds;

            int yTextBase = (int)(e.Bounds.Y + (e.Bounds.Height - textSize.Height) / 2);
            int yIconBase = (int)(e.Bounds.Y + (e.Bounds.Height - iconSize) / 2);

            #endregion Creación de la fuente del caption

            #region Calculo del inicio de icono y texto

            int xBase = e.Bounds.Left;

            int xTextBase = 0;
            int xIconBase = 0;

            if (componentProperties.Icon == eBoostrapIcons.None)
            {
                xTextBase = e.Bounds.Left + (e.Bounds.Width / 2) - ((int)textSize.Width / 2);
            }
            else
            {
                xIconBase =
                    e.Bounds.Left
                    + (e.Bounds.Width / 2)
                    - (((int)textSize.Width + iconTextMargin + iconSize) / 2);
                xTextBase = xIconBase + iconTextMargin + iconSize;
            }

            #endregion Calculo del inicio de icono y texto

            #region Dibujado del control

            WoColor backgroundControlColor = new WoColor();
            WoColor borderColor = new WoColor();

            if (componentProperties.BackgorundColorContainerItem.ToString().Contains("Border"))
            {
                backgroundControlColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Default");
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties
                        .BackgorundColorContainerItem.ToString()
                        .Replace("Border", "")
                );
            }
            else
            {
                backgroundControlColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties.BackgorundColorContainerItem.ToString()
                );
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Border");
            }

            LinearGradientBrush backgroundControlBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                backgroundControlColor.SolidColor,
                backgroundControlColor.SolidColor,
                180
            );

            if (backgroundControlColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    backgroundControlColor.GradientColor1,
                    backgroundControlColor.GradientColor2,
                    backgroundControlColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                backgroundControlBrush.InterpolationColors = colorBlend;
            }

            LinearGradientBrush borderBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
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
                borderBrush.InterpolationColors = colorBlend;
            }

            e.Graphics.FillRectangle(
                backgroundControlBrush,
                e.Bounds.X,
                e.Bounds.Y,
                e.Bounds.Width,
                e.Bounds.Height
            );

            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(borderBrush),
                e.Bounds.X,
                e.Bounds.Y,
                e.Bounds.Width,
                e.Bounds.Height - 1
            );

            #endregion Dibujado del control

            #region Dibujado del caption

            WoColor fontColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                componentProperties.ComponentFontColor.ToString()
            );

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

            e.Graphics.DrawString(maskText, fontCaption, fontBrush, xTextBase, yTextBase);

            if (componentProperties.Icon != eBoostrapIcons.None)
            {
                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open(
                    $@"{_iconPath}\{_woCommonDesignOptions.BoostrapIcons.Get(componentProperties.Icon.ToString())}"
                );
                for (int i = 0; i < svgDoc.Children.Count; i++)
                {
                    svgDoc.Children[i].Fill = new SvgColourServer(fontColor.SolidColor);
                }
                icon = new Bitmap(svgDoc.Draw());

                e.Graphics.DrawImage(icon, xIconBase, yIconBase, iconSize, iconSize);
            }

            #endregion Dibujado del caption

            #region Indicador de seleccionado

            if (componentProperties.Selected)
            {
                #region Variables principales

                int sizeWidthIndicator = 24;
                int sizeHeightIndicator = 12;

                int sizeIconIndicator = 20;

                Color baseControlsColor = Color.Blue;
                LinearGradientBrush baseControlsBrush = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlsColor,
                    baseControlsColor,
                    180
                );
                Color baseControlColorSecondary = Color.Orange;
                LinearGradientBrush baseControlsBrushSecondary = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlColorSecondary,
                    baseControlColorSecondary,
                    180
                );

                #endregion Variables principales

                #region Indicador principal

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(baseControlsBrush),
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 2
                );

                #endregion Indicador principal

                if (!_sizeModifyW && !_sizeModifyH)
                {
                    #region Control de eliminado

                    int baseXDeleteControl = e.Bounds.Right - 26;

                    ///Delete
                    _startXButtonDelete = baseXDeleteControl;
                    _endXButtonDelete = _startXButtonDelete + sizeWidthIndicator;

                    _startYButtonDelete = e.Bounds.Y + 1;
                    _endYButtonDelete = _startYButtonDelete + (sizeHeightIndicator * 2);

                    Rectangle btnDelete = new Rectangle(
                        _startXButtonDelete,
                        _startYButtonDelete,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDelete);

                    System.Drawing.Image iconBtnDelete = null;
                    SvgDocument svgIconDelete = SvgDocument.Open(_pathIconDelete);
                    for (int i = 0; i < svgIconDelete.Children.Count; i++)
                    {
                        svgIconDelete.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnDelete = new Bitmap(svgIconDelete.Draw());

                    e.Graphics.DrawImage(
                        iconBtnDelete,
                        _startXButtonDelete + 2,
                        _startYButtonDelete + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    int baseXLabelControl = baseXDeleteControl - 26;

                    ///Label
                    _startXButtonLabel = baseXLabelControl;
                    _endXButtonLabel = _startXButtonLabel + sizeWidthIndicator;

                    _startYButtonLabel = e.Bounds.Y + 1;
                    _endYButtonLabel = _startYButtonLabel + (sizeHeightIndicator * 2);

                    Rectangle btnLabelControl = new Rectangle(
                        _startXButtonLabel,
                        _startYButtonLabel,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLabelControl);

                    System.Drawing.Image iconBtnLabelControl = null;
                    SvgDocument svgIconLabelControl = SvgDocument.Open(_pathIconLabel);
                    for (int i = 0; i < svgIconLabelControl.Children.Count; i++)
                    {
                        svgIconLabelControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnLabelControl = new Bitmap(svgIconLabelControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLabelControl,
                        _startXButtonLabel + 2,
                        _startYButtonLabel + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    int baseXWidthControl = baseXLabelControl - 26;

                    ///Width
                    _startXButtonWidth = baseXWidthControl;
                    _endXButtonWidth = _startXButtonWidth + sizeWidthIndicator;

                    _startYButtonWidth = e.Bounds.Y + 1;
                    _endYButtonWidth = _startYButtonWidth + (sizeHeightIndicator * 2);

                    Rectangle btnWidthControl = new Rectangle(
                        _startXButtonWidth,
                        _startYButtonWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnWidthControl);

                    System.Drawing.Image iconBtnWidthControl = null;
                    SvgDocument svgIconWidthControl = SvgDocument.Open(_buttonWidth);
                    for (int i = 0; i < svgIconWidthControl.Children.Count; i++)
                    {
                        svgIconWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnWidthControl = new Bitmap(svgIconWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnWidthControl,
                        _startXButtonWidth + 2,
                        _startYButtonWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    int baseXHeightControl = baseXWidthControl - 26;

                    ///Width
                    _startXButtonHeight = baseXHeightControl;
                    _endXButtonHeight = _startXButtonHeight + sizeWidthIndicator;

                    _startYButtonHeight = e.Bounds.Y + 1;
                    _endYButtonHeight = _startYButtonHeight + (sizeHeightIndicator * 2);

                    Rectangle btnHeightControl = new Rectangle(
                        _startXButtonHeight,
                        _startYButtonHeight,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnHeightControl);

                    System.Drawing.Image iconBtnHeightControl = null;
                    SvgDocument svgIconHeightControl = SvgDocument.Open(_buttonHeight);
                    for (int i = 0; i < svgIconHeightControl.Children.Count; i++)
                    {
                        svgIconHeightControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnHeightControl = new Bitmap(svgIconHeightControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnHeightControl,
                        _startXButtonHeight + 2,
                        _startYButtonHeight + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs
                }
                else if (_sizeModifyW)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicCol.Clear();
                    int iLast = 13 - (e.Item.OptionsTableLayoutItem.ColumnIndex);

                    for (int i = 3; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnLeftMove = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.ColumnSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnLeftMove
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnLeftMove);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLeftMove);

                            e.Graphics.FillRectangle(baseControlsBrush, btnLeftMove);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        int yTextTempBase = (int)(e.Bounds.Top + 13 - (textSize.Height / 2));
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextTempBase
                        );

                        _buttonsSizeDynamicCol.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
                else if (_sizeModifyH)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicRow.Clear();

                    int iLast =
                        (e.Item.Parent.OptionsTableLayoutGroup.RowCount)
                        - (e.Item.OptionsTableLayoutItem.RowIndex)
                        + 1;

                    for (int i = 1; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnDinamicH = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.RowSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnDinamicH
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnDinamicH);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDinamicH);

                            e.Graphics.FillRectangle(baseControlsBrush, btnDinamicH);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        int yTextTempBase = (int)(e.Bounds.Top + 13 - (textSize.Height / 2));
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextTempBase
                        );

                        _buttonsSizeDynamicRow.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
            }

            #endregion Indicador de seleccionado

            e.Handled = true;
        }

        [SupportedOSPlatform("windows")]
        private void ContainerCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.Handled = false;

            #region Variables principales

            // Recuperación del componente.
            WoComponentProperties componentProperties = (WoComponentProperties)e.Item.Tag;
            string maskText = componentProperties.MaskText;

            // Variables generales
            WoColor borderColor = new WoColor();
            int borderWith = 1;

            int fontSize = 5;
            int iconSize = 10;
            int marginBorder = 4;
            int iconTextMargin = 3;

            switch (componentProperties.ComponentFontSize)
            {
                case eTextSize.Small:
                    fontSize = 5;
                    iconSize = 10;
                    break;
                case eTextSize.Normal:
                    fontSize = 10;
                    iconSize = 15;
                    break;
                case eTextSize.Large:
                    fontSize = 12;
                    iconSize = 20;
                    break;
            }

            Font font = new Font("Arial", fontSize, FontStyle.Regular);
            if (componentProperties.ComponentFontWide != eTextWeight.Normal)
            {
                if (componentProperties.ComponentFontWide == eTextWeight.Bold)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Bold);
                }
                else if (componentProperties.ComponentFontWide == eTextWeight.Normal)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Regular);
                }
            }
            if (componentProperties.ComponentFontItalic == eTextItalic.Italic)
            {
                font = new Font("Arial", fontSize, font.Style | FontStyle.Italic);
            }
            if (componentProperties.ComponentFontDecoration != eTextDecoration.None)
            {
                if (componentProperties.ComponentFontDecoration == eTextDecoration.Underline)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Underline);
                }
                else if (componentProperties.ComponentFontDecoration == eTextDecoration.Through)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Strikeout);
                }
            }

            // Creación de las variables base
            SizeF textSize = e.Graphics.MeasureString(maskText, font); // Tamaño del texto
            RectangleF bounds = e.Bounds; // Rectángulo que se dibujará como fondo
            int yTextBase = (int)(e.Bounds.Y + (25 - textSize.Height) / 2);
            int yIconBase = (int)(e.Bounds.Y + (25 - iconSize) / 2);

            #endregion Variables principales

            #region Dibujado del fondo

            WoColor backgroundColor = new WoColor();

            if (componentProperties.BackgorundColorGroup.ToString().Contains("Border"))
            {
                backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Default");
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties.BackgorundColorGroup.ToString().Replace("Border", "")
                );
            }
            else
            {
                backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties.BackgorundColorGroup.ToString()
                );
                borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Border");
            }

            LinearGradientBrush brush = new LinearGradientBrush(
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
                brush.InterpolationColors = colorBlend;
            }

            e.Graphics.FillRectangle(brush, e.Bounds);

            #endregion Dibujado del fondo

            if (e.Item.Name != "Root")
            {
                #region Dibujado del borde

                LinearGradientBrush borderBrush = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
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
                    borderBrush.InterpolationColors = colorBlend;
                }

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    26
                );

                // Dibujado del borde.
                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderBrush),
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                #endregion Dibujado del borde

                #region Calculo del inicio de icono y texto

                int xBase = e.Bounds.Left + marginBorder;

                int xTextBase = 0;
                int xIconBase = 0;

                if (componentProperties.Icon != eBoostrapIcons.None)
                {
                    xIconBase = xBase;
                    xTextBase = xBase + iconTextMargin + iconSize;
                }
                else if (componentProperties.Icon == eBoostrapIcons.None)
                {
                    xTextBase = xBase;
                }

                #endregion Calculo del inicio de icono y texto

                #region Dibujado

                WoColor fontColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties.ComponentFontColor.ToString()
                );

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

                if (componentProperties.Icon != eBoostrapIcons.None)
                {
                    System.Drawing.Image icon = null;

                    var svgDoc = SvgDocument.Open(
                        $@"{_iconPath}\{_woCommonDesignOptions.BoostrapIcons.Get(componentProperties.Icon.ToString())}"
                    );
                    for (int i = 0; i < svgDoc.Children.Count; i++)
                    {
                        svgDoc.Children[i].Fill = new SvgColourServer(fontColor.SolidColor);
                    }
                    icon = new Bitmap(svgDoc.Draw());

                    e.Graphics.DrawImage(icon, xIconBase, yIconBase, iconSize, iconSize);
                }

                #endregion Dibujado
            }

            #region Indicador de seleccionado

            if (componentProperties.Selected)
            {
                #region Variables principales

                int sizeWidthIndicator = 24;
                int sizeHeightIndicator = 12;

                int sizeIconIndicator = 20;

                Color baseControlsColor = Color.Blue;
                LinearGradientBrush baseControlsBrush = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlsColor,
                    baseControlsColor,
                    180
                );
                Color baseControlColorSecondary = Color.Orange;
                LinearGradientBrush baseControlsBrushSecondary = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlColorSecondary,
                    baseControlColorSecondary,
                    180
                );

                #endregion Variables principales

                #region Indicador principal

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(baseControlsBrush),
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 2
                );

                #endregion Indicador principal

                if (e.Item.Name != "Root" && !_sizeModifyW && !_sizeModifyH)
                {
                    #region Control de eliminado

                    int baseXDeleteControl = e.Bounds.Right - 26;

                    ///Delete
                    _startXButtonDelete = baseXDeleteControl;
                    _endXButtonDelete = _startXButtonDelete + sizeWidthIndicator;

                    _startYButtonDelete = e.Bounds.Y + 1;
                    _endYButtonDelete = _startYButtonDelete + (sizeHeightIndicator * 2);

                    Rectangle btnDelete = new Rectangle(
                        _startXButtonDelete,
                        _startYButtonDelete,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDelete);

                    System.Drawing.Image iconBtnDelete = null;
                    SvgDocument svgIconDelete = SvgDocument.Open(_pathIconDelete);
                    for (int i = 0; i < svgIconDelete.Children.Count; i++)
                    {
                        svgIconDelete.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnDelete = new Bitmap(svgIconDelete.Draw());

                    e.Graphics.DrawImage(
                        iconBtnDelete,
                        _startXButtonDelete + 2,
                        _startYButtonDelete + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    int baseXLabelControl = baseXDeleteControl - 26;

                    ///Label
                    _startXButtonLabel = baseXLabelControl;
                    _endXButtonLabel = _startXButtonLabel + sizeWidthIndicator;

                    _startYButtonLabel = e.Bounds.Y + 1;
                    _endYButtonLabel = _startYButtonLabel + (sizeHeightIndicator * 2);

                    Rectangle btnLabelControl = new Rectangle(
                        _startXButtonLabel,
                        _startYButtonLabel,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLabelControl);

                    System.Drawing.Image iconBtnLabelControl = null;
                    SvgDocument svgIconLabelControl = SvgDocument.Open(_pathIconLabel);
                    for (int i = 0; i < svgIconLabelControl.Children.Count; i++)
                    {
                        svgIconLabelControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnLabelControl = new Bitmap(svgIconLabelControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLabelControl,
                        _startXButtonLabel + 2,
                        _startYButtonLabel + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    int baseXWidthControl = baseXLabelControl - 26;

                    ///Width
                    _startXButtonWidth = baseXWidthControl;
                    _endXButtonWidth = _startXButtonWidth + sizeWidthIndicator;

                    _startYButtonWidth = e.Bounds.Y + 1;
                    _endYButtonWidth = _startYButtonWidth + (sizeHeightIndicator * 2);

                    Rectangle btnWidthControl = new Rectangle(
                        _startXButtonWidth,
                        _startYButtonWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnWidthControl);

                    System.Drawing.Image iconBtnWidthControl = null;
                    SvgDocument svgIconWidthControl = SvgDocument.Open(_buttonWidth);
                    for (int i = 0; i < svgIconWidthControl.Children.Count; i++)
                    {
                        svgIconWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnWidthControl = new Bitmap(svgIconWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnWidthControl,
                        _startXButtonWidth + 2,
                        _startYButtonWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    int baseXHeightControl = baseXWidthControl - 26;

                    ///Width
                    _startXButtonHeight = baseXHeightControl;
                    _endXButtonHeight = _startXButtonHeight + sizeWidthIndicator;

                    _startYButtonHeight = e.Bounds.Y + 1;
                    _endYButtonHeight = _startYButtonHeight + (sizeHeightIndicator * 2);

                    Rectangle btnHeightControl = new Rectangle(
                        _startXButtonHeight,
                        _startYButtonHeight,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnHeightControl);

                    System.Drawing.Image iconBtnHeightControl = null;
                    SvgDocument svgIconHeightControl = SvgDocument.Open(_buttonHeight);
                    for (int i = 0; i < svgIconHeightControl.Children.Count; i++)
                    {
                        svgIconHeightControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnHeightControl = new Bitmap(svgIconHeightControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnHeightControl,
                        _startXButtonHeight + 2,
                        _startYButtonHeight + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño height

                    #region Controles de rows

                    int baseXAddRowCotrol = baseXHeightControl - 26;

                    ///AddRow
                    _startXButtonAddRow = baseXAddRowCotrol;
                    _endXButtonAddRow = _startXButtonAddRow + sizeWidthIndicator;

                    _startYButtonAddRow = e.Bounds.Y + 1;
                    _endYButtonAddRow = _startYButtonAddRow + (sizeHeightIndicator * 2);

                    Rectangle btnAddRow = new Rectangle(
                        _startXButtonAddRow,
                        _startYButtonAddRow,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnAddRow);

                    System.Drawing.Image iconBtnAddRow = null;
                    SvgDocument svgIconAddRow = SvgDocument.Open(_pathRowAdd);
                    for (int i = 0; i < svgIconAddRow.Children.Count; i++)
                    {
                        svgIconAddRow.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnAddRow = new Bitmap(svgIconAddRow.Draw());

                    e.Graphics.DrawImage(
                        iconBtnAddRow,
                        _startXButtonAddRow + 2,
                        _startYButtonAddRow + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    int baseXLessRowCotrol = baseXAddRowCotrol - 26;

                    ///LessRow
                    _startXButtonLessRow = baseXLessRowCotrol;
                    _endXButtonLessRow = _startXButtonLessRow + sizeWidthIndicator;

                    _startYButtonLessRow = e.Bounds.Y + 1;
                    _endYButtonLessRow = _startYButtonLessRow + (sizeHeightIndicator * 2);

                    Rectangle btnLessRow = new Rectangle(
                        _startXButtonLessRow,
                        _startYButtonLessRow,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLessRow);

                    System.Drawing.Image iconBtnLessRow = null;
                    SvgDocument svgIconLessRow = SvgDocument.Open(_pathRowLess);
                    for (int i = 0; i < svgIconLessRow.Children.Count; i++)
                    {
                        svgIconLessRow.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnLessRow = new Bitmap(svgIconLessRow.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLessRow,
                        _startXButtonLessRow + 2,
                        _startYButtonLessRow + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs
                }
                else if (e.Item.Name == "Root")
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    int baseXRowCotrol = e.Bounds.Right - 54;

                    ///LessRow
                    _startXButtonLessRow = baseXRowCotrol;
                    _endXButtonLessRow = _startXButtonLessRow + sizeWidthIndicator;

                    _startYButtonLessRow = e.Bounds.Y + 1;
                    _endYButtonLessRow = _startYButtonLessRow + sizeHeightIndicator;

                    Rectangle btnLessRow = new Rectangle(
                        _startXButtonLessRow,
                        _startYButtonLessRow,
                        sizeWidthIndicator,
                        sizeHeightIndicator
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLessRow);

                    System.Drawing.Image iconBtnLessRow = null;
                    SvgDocument svgIconLessRow = SvgDocument.Open(_pathRowLess);
                    for (int i = 0; i < svgIconLessRow.Children.Count; i++)
                    {
                        svgIconLessRow.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnLessRow = new Bitmap(svgIconLessRow.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLessRow,
                        _startXButtonLessRow + 2,
                        _startYButtonLessRow + 2,
                        sizeIconIndicator,
                        sizeIconIndicator / 2
                    );

                    ///AddRow
                    _startXButtonAddRow = baseXRowCotrol + 26;
                    _endXButtonAddRow = _startXButtonAddRow + sizeWidthIndicator;

                    _startYButtonAddRow = e.Bounds.Y + 1;
                    _endYButtonAddRow = _startYButtonAddRow + sizeHeightIndicator;

                    Rectangle btnAddRow = new Rectangle(
                        _startXButtonAddRow,
                        _startYButtonAddRow,
                        sizeWidthIndicator,
                        sizeHeightIndicator
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnAddRow);

                    System.Drawing.Image iconBtnAddRow = null;
                    SvgDocument svgIconAddRow = SvgDocument.Open(_pathRowAdd);
                    for (int i = 0; i < svgIconAddRow.Children.Count; i++)
                    {
                        svgIconAddRow.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnAddRow = new Bitmap(svgIconAddRow.Draw());

                    e.Graphics.DrawImage(
                        iconBtnAddRow,
                        _startXButtonAddRow + 2,
                        _startYButtonAddRow + 2,
                        sizeIconIndicator,
                        sizeIconIndicator / 2
                    );

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs
                }
                else if (_sizeModifyW)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicCol.Clear();
                    int iLast = 13 - (e.Item.OptionsTableLayoutItem.ColumnIndex);

                    for (int i = 3; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnLeftMove = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.ColumnSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnLeftMove
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnLeftMove);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLeftMove);

                            e.Graphics.FillRectangle(baseControlsBrush, btnLeftMove);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicCol.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
                else if (_sizeModifyH)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 1;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicRow.Clear();
                    int iLast =
                        (e.Item.Parent.OptionsTableLayoutGroup.RowCount)
                        - (e.Item.OptionsTableLayoutItem.RowIndex)
                        + 1;

                    for (int i = 2; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Y + 1;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnDinamicH = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.RowSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnDinamicH
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnDinamicH);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDinamicH);

                            e.Graphics.FillRectangle(baseControlsBrush, btnDinamicH);
                        }

                        Font fontTemp = new Font("Arial", fontSize, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicRow.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
            }

            #endregion Indicador de seleccionado

            e.Handled = true;
        }

        [SupportedOSPlatform("windows")]
        private void TabGroupCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.Handled = false;

            #region Recuperación de la tab actual

            TabbedControlGroup editingTab = (TabbedControlGroup)e.Item;
            LayoutControlGroup controlGroup = (LayoutControlGroup)
                editingTab.TabPages.Where(x => x.Visible).FirstOrDefault();

            // Recuperación del componente.
            WoComponentProperties baseComponentProperties = (WoComponentProperties)controlGroup.Tag;

            #endregion Recuperación de la tab actual

            #region Dibujado del grupo principal

            WoColor baseBackgroundColor = new WoColor();
            WoColor baseBorderColor = new WoColor();

            if (baseComponentProperties.BackgorundColorGroup.ToString().Contains("Border"))
            {
                baseBackgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Default");
                baseBorderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    baseComponentProperties.BackgorundColorGroup.ToString().Replace("Border", "")
                );
            }
            else
            {
                baseBackgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    baseComponentProperties.BackgorundColorGroup.ToString()
                );
                baseBorderColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Border");
            }

            LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                baseBackgroundColor.SolidColor,
                baseBackgroundColor.SolidColor,
                180
            );

            if (baseBackgroundColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    baseBackgroundColor.GradientColor1,
                    baseBackgroundColor.GradientColor2,
                    baseBackgroundColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                backgroundBrush.InterpolationColors = colorBlend;
            }

            e.Graphics.FillRectangle(
                backgroundBrush,
                e.Bounds.X,
                e.Bounds.Y + 25,
                e.Bounds.Width,
                e.Bounds.Height - 25
            );

            LinearGradientBrush borderBrush = new LinearGradientBrush(
                new System.Drawing.Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                ),
                ///Solo para realizar la instancia del brush, el color se asigna luego.
                baseBorderColor.SolidColor,
                baseBorderColor.SolidColor,
                180
            );

            if (baseBorderColor.ColorType == eWoColorType.Gradient)
            {
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = new Color[]
                {
                    baseBorderColor.GradientColor1,
                    baseBorderColor.GradientColor2,
                    baseBorderColor.GradientColor3
                };
                colorBlend.Positions = new float[] { 0f, 0.5f, 1f };
                borderBrush.InterpolationColors = colorBlend;
            }

            e.Graphics.DrawRectangle(
                new System.Drawing.Pen(borderBrush),
                e.Bounds.X,
                e.Bounds.Y + 25,
                e.Bounds.Width,
                e.Bounds.Height - 25
            );

            #endregion Dibujado del grupo principal

            #region Dibujado de las etiquetas

            var labelTabs = editingTab.TabPages.ToList();
            int left = e.Bounds.Left + 8;

            foreach (var tab in labelTabs)
            {
                #region Variables principales

                WoComponentProperties componentProperties = (WoComponentProperties)tab.Tag;
                string maskText = componentProperties.MaskText;

                WoColor backgroundColor = new WoColor();
                WoColor borderColor = new WoColor();

                if (componentProperties.BackgorundColorGroup.ToString().Contains("Border"))
                {
                    backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Default");
                    borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                        componentProperties.BackgorundColorGroup.ToString().Replace("Border", "")
                    );
                }
                else
                {
                    backgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                        componentProperties.BackgorundColorGroup.ToString()
                    );
                    borderColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Border");
                }

                // Variables generales
                int fontSize = 5;
                int iconSize = 10;
                int marginBorder = 4;
                int iconTextMargin = 3;

                switch (componentProperties.ComponentFontSize)
                {
                    case eTextSize.Small:
                        fontSize = 5;
                        iconSize = 10;
                        break;
                    case eTextSize.Normal:
                        fontSize = 10;
                        iconSize = 15;
                        break;
                    case eTextSize.Large:
                        fontSize = 12;
                        iconSize = 20;
                        break;
                }

                Font font = new Font("Arial", fontSize, FontStyle.Regular);
                if (componentProperties.ComponentFontWide != eTextWeight.Normal)
                {
                    if (componentProperties.ComponentFontWide == eTextWeight.Bold)
                    {
                        font = new Font("Arial", fontSize, font.Style | FontStyle.Bold);
                    }
                    else if (componentProperties.ComponentFontWide == eTextWeight.Normal)
                    {
                        font = new Font("Arial", fontSize, font.Style | FontStyle.Regular);
                    }
                }
                if (componentProperties.ComponentFontItalic == eTextItalic.Italic)
                {
                    font = new Font("Arial", fontSize, font.Style | FontStyle.Italic);
                }
                if (componentProperties.ComponentFontDecoration != eTextDecoration.None)
                {
                    if (componentProperties.ComponentFontDecoration == eTextDecoration.Underline)
                    {
                        font = new Font("Arial", fontSize, font.Style | FontStyle.Underline);
                    }
                    else if (componentProperties.ComponentFontDecoration == eTextDecoration.Through)
                    {
                        font = new Font("Arial", fontSize, font.Style | FontStyle.Strikeout);
                    }
                }

                SizeF textSize = e.Graphics.MeasureString(maskText, font);
                int yTextBase = (int)(e.Bounds.Y + (25 - textSize.Height) / 2);
                int yIconBase = (int)(e.Bounds.Y + (25 - iconSize) / 2);

                #endregion Variables principales

                #region Calculo del dibujado de texto e icono

                int xBase = left + marginBorder;

                int xTextBase = 0;
                int xIconBase = 0;

                if (componentProperties.Icon == eBoostrapIcons.None)
                {
                    xTextBase = xBase;
                }
                else
                {
                    xIconBase = xBase;
                    xTextBase = xBase + iconTextMargin + iconSize;
                }

                #endregion Calculo del dibujado de texto e icono

                #region Calculo de dimensiones

                int labelSize = 0;
                int marginLabelControl = 3;
                int borders = 0;

                if (componentProperties.Icon == eBoostrapIcons.None)
                {
                    labelSize = marginBorder + (int)textSize.Width + marginLabelControl;
                    borders = marginBorder + marginLabelControl;
                }
                else if (componentProperties.Icon != eBoostrapIcons.None)
                {
                    labelSize =
                        marginBorder
                        + iconSize
                        + iconTextMargin
                        + (int)textSize.Width
                        + marginLabelControl;
                    borders = marginBorder + iconTextMargin + marginLabelControl;
                }

                #endregion Calculo de dimensiones

                #region Dibujado del fondo

                LinearGradientBrush backgroundLabelBrush = new LinearGradientBrush(
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
                    backgroundLabelBrush.InterpolationColors = colorBlend;
                }

                e.Graphics.FillRectangle(backgroundLabelBrush, left, e.Bounds.Y, labelSize, 25);

                #endregion Dibujado del fondo

                #region Dibujado del texto

                WoColor fontColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                    componentProperties.ComponentFontColor.ToString()
                );

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

                if (componentProperties.Icon != eBoostrapIcons.None)
                {
                    System.Drawing.Image icon = null;

                    var svgDoc = SvgDocument.Open(
                        $@"{_iconPath}\{_woCommonDesignOptions.BoostrapIcons.Get(componentProperties.Icon.ToString())}"
                    );
                    for (int i = 0; i < svgDoc.Children.Count; i++)
                    {
                        svgDoc.Children[i].Fill = new SvgColourServer(fontColor.SolidColor);
                    }
                    icon = new Bitmap(svgDoc.Draw());

                    e.Graphics.DrawImage(icon, xIconBase, yIconBase, iconSize, iconSize);
                }

                #endregion Dibujado del texto

                #region Dibujado del borde

                LinearGradientBrush borderLabelBrush = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
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
                    borderLabelBrush.InterpolationColors = colorBlend;
                }

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(borderLabelBrush),
                    left,
                    e.Bounds.Y,
                    labelSize,
                    26
                );

                #endregion Dibujado del borde

                #region Calculo del tamaño de la tab

                LayoutControlGroup groupTab = (LayoutControlGroup)tab;
                groupTab.TabPageWidth = labelSize - borders - 10;

                left = left + labelSize;

                #endregion Calculo del tamaño de la tab
            }

            #endregion Dibujado de las etiquetas

            #region Indicador de seleccionado

            if (baseComponentProperties.Selected)
            {
                #region Variables principales

                int sizeWidthIndicator = 24;
                int sizeHeightIndicator = 12;

                int sizeIconIndicator = 20;

                Color baseControlsColor = Color.Blue;
                LinearGradientBrush baseControlsBrush = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlsColor,
                    baseControlsColor,
                    180
                );
                Color baseControlColorSecondary = Color.Orange;
                LinearGradientBrush baseControlsBrushSecondary = new LinearGradientBrush(
                    new System.Drawing.Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width,
                        e.Bounds.Height
                    ),
                    baseControlColorSecondary,
                    baseControlColorSecondary,
                    180
                );

                #endregion Variables principales

                #region Indicador principal

                e.Graphics.DrawRectangle(
                    new System.Drawing.Pen(baseControlsBrush),
                    e.Bounds.X - 1,
                    e.Bounds.Y - 1,
                    e.Bounds.Width + 2,
                    e.Bounds.Height + 2
                );

                #endregion Indicador principal

                if (!_sizeModifyW && !_sizeModifyH)
                {
                    #region Control de eliminado

                    int baseXDeleteControl = e.Bounds.Right - 37;

                    ///Delete
                    _startXButtonDelete = baseXDeleteControl;
                    _endXButtonDelete = _startXButtonDelete + sizeWidthIndicator;

                    _startYButtonDelete = e.Bounds.Bottom - 37;
                    _endYButtonDelete = _startYButtonDelete + (sizeHeightIndicator * 2);

                    Rectangle btnDelete = new Rectangle(
                        _startXButtonDelete,
                        _startYButtonDelete,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDelete);

                    System.Drawing.Image iconBtnDelete = null;
                    SvgDocument svgIconDelete = SvgDocument.Open(_pathIconDelete);
                    for (int i = 0; i < svgIconDelete.Children.Count; i++)
                    {
                        svgIconDelete.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnDelete = new Bitmap(svgIconDelete.Draw());

                    e.Graphics.DrawImage(
                        iconBtnDelete,
                        _startXButtonDelete + 2,
                        _startYButtonDelete + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    int baseXLabelControl = baseXDeleteControl - 26;

                    ///Label
                    _startXButtonLabel = baseXLabelControl;
                    _endXButtonLabel = _startXButtonLabel + sizeWidthIndicator;

                    _startYButtonLabel = e.Bounds.Bottom - 37;
                    _endYButtonLabel = _startYButtonLabel + (sizeHeightIndicator * 2);

                    Rectangle btnLabelControl = new Rectangle(
                        _startXButtonLabel,
                        _startYButtonLabel,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLabelControl);

                    System.Drawing.Image iconBtnLabelControl = null;
                    SvgDocument svgIconLabelControl = SvgDocument.Open(_pathIconLabel);
                    for (int i = 0; i < svgIconLabelControl.Children.Count; i++)
                    {
                        svgIconLabelControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnLabelControl = new Bitmap(svgIconLabelControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLabelControl,
                        _startXButtonLabel + 2,
                        _startYButtonLabel + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    int baseXWidthControl = baseXLabelControl - 26;

                    ///Width
                    _startXButtonWidth = baseXWidthControl;
                    _endXButtonWidth = _startXButtonWidth + sizeWidthIndicator;

                    _startYButtonWidth = e.Bounds.Bottom - 37;
                    _endYButtonWidth = _startYButtonWidth + (sizeHeightIndicator * 2);

                    Rectangle btnWidthControl = new Rectangle(
                        _startXButtonWidth,
                        _startYButtonWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnWidthControl);

                    System.Drawing.Image iconBtnWidthControl = null;
                    SvgDocument svgIconWidthControl = SvgDocument.Open(_buttonWidth);
                    for (int i = 0; i < svgIconWidthControl.Children.Count; i++)
                    {
                        svgIconWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnWidthControl = new Bitmap(svgIconWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnWidthControl,
                        _startXButtonWidth + 2,
                        _startYButtonWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    int baseXHeightControl = baseXWidthControl - 26;

                    ///Width
                    _startXButtonHeight = baseXHeightControl;
                    _endXButtonHeight = _startXButtonHeight + sizeWidthIndicator;

                    _startYButtonHeight = e.Bounds.Bottom - 37;
                    _endYButtonHeight = _startYButtonHeight + (sizeHeightIndicator * 2);

                    Rectangle btnHeightControl = new Rectangle(
                        _startXButtonHeight,
                        _startYButtonHeight,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnHeightControl);

                    System.Drawing.Image iconBtnHeightControl = null;
                    SvgDocument svgIconHeightControl = SvgDocument.Open(_buttonHeight);
                    for (int i = 0; i < svgIconHeightControl.Children.Count; i++)
                    {
                        svgIconHeightControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnHeightControl = new Bitmap(svgIconHeightControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnHeightControl,
                        _startXButtonHeight + 2,
                        _startYButtonHeight + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Control de tamaño height

                    #region Controles de rows

                    int baseXAddRowCotrol = baseXHeightControl - 26;

                    ///AddRow
                    _startXButtonAddRow = baseXAddRowCotrol;
                    _endXButtonAddRow = _startXButtonAddRow + sizeWidthIndicator;

                    _startYButtonAddRow = e.Bounds.Bottom - 37;
                    _endYButtonAddRow = _startYButtonAddRow + (sizeHeightIndicator * 2);

                    Rectangle btnAddRow = new Rectangle(
                        _startXButtonAddRow,
                        _startYButtonAddRow,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnAddRow);

                    System.Drawing.Image iconBtnAddRow = null;
                    SvgDocument svgIconAddRow = SvgDocument.Open(_pathRowAdd);
                    for (int i = 0; i < svgIconAddRow.Children.Count; i++)
                    {
                        svgIconAddRow.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnAddRow = new Bitmap(svgIconAddRow.Draw());

                    e.Graphics.DrawImage(
                        iconBtnAddRow,
                        _startXButtonAddRow + 2,
                        _startYButtonAddRow + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    int baseXLessRowCotrol = baseXAddRowCotrol - 26;

                    ///LessRow
                    _startXButtonLessRow = baseXLessRowCotrol;
                    _endXButtonLessRow = _startXButtonLessRow + sizeWidthIndicator;

                    _startYButtonLessRow = e.Bounds.Bottom - 37;
                    _endYButtonLessRow = _startYButtonLessRow + (sizeHeightIndicator * 2);

                    Rectangle btnLessRow = new Rectangle(
                        _startXButtonLessRow,
                        _startYButtonLessRow,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLessRow);

                    System.Drawing.Image iconBtnLessRow = null;
                    SvgDocument svgIconLessRow = SvgDocument.Open(_pathRowLess);
                    for (int i = 0; i < svgIconLessRow.Children.Count; i++)
                    {
                        svgIconLessRow.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnLessRow = new Bitmap(svgIconLessRow.Draw());

                    e.Graphics.DrawImage(
                        iconBtnLessRow,
                        _startXButtonLessRow + 2,
                        _startYButtonLessRow + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de rows

                    #region Controles de tabs

                    int baseXRemoveTab = baseXLessRowCotrol - 26;

                    ///AddTab
                    _startXButtonAddTab = baseXRemoveTab;
                    _endXButtonAddTab = _startXButtonAddTab + sizeWidthIndicator;

                    _startYButtonAddTab = e.Bounds.Bottom - 37;
                    _endYButtonAddTab = _startYButtonAddTab + (sizeHeightIndicator * 2);

                    Rectangle btnAddTab = new Rectangle(
                        _startXButtonAddTab,
                        _startYButtonAddTab,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnAddTab);

                    System.Drawing.Image iconBtnAddTab = null;
                    SvgDocument svgIconAddTab = SvgDocument.Open(_pathIconAddTab);
                    for (int i = 0; i < svgIconAddTab.Children.Count; i++)
                    {
                        svgIconAddTab.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnAddTab = new Bitmap(svgIconAddTab.Draw());

                    e.Graphics.DrawImage(
                        iconBtnAddTab,
                        _startXButtonAddTab + 2,
                        _startYButtonAddTab + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    int baseXAddTab = baseXRemoveTab - 26;

                    ///RemoveTab
                    _startXButtonRemoveTab = baseXAddTab;
                    _endXButtonRemoveTab = _startXButtonRemoveTab + sizeWidthIndicator;

                    _startYButtonRemoveTab = e.Bounds.Bottom - 37;
                    _endYButtonRemoveTab = _startYButtonRemoveTab + (sizeHeightIndicator * 2);

                    Rectangle btnRemoveTab = new Rectangle(
                        _startXButtonRemoveTab,
                        _startYButtonRemoveTab,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnRemoveTab);

                    System.Drawing.Image iconBtnRemoveTab = null;
                    SvgDocument svgIconRemoveTab = SvgDocument.Open(_pathIconRemoveTab);
                    for (int i = 0; i < svgIconRemoveTab.Children.Count; i++)
                    {
                        svgIconRemoveTab.Children[i].Fill = new SvgColourServer(baseControlsColor);
                    }
                    iconBtnRemoveTab = new Bitmap(svgIconRemoveTab.Draw());

                    e.Graphics.DrawImage(
                        iconBtnRemoveTab,
                        _startXButtonRemoveTab + 2,
                        _startYButtonRemoveTab + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de tabs
                }
                else if (_sizeModifyW)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 12;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicCol.Clear();
                    int iLast = 13 - (e.Item.OptionsTableLayoutItem.ColumnIndex);

                    for (int i = 3; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Bottom - 37;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnLeftMove = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.ColumnSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnLeftMove
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnLeftMove);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnLeftMove);

                            e.Graphics.FillRectangle(baseControlsBrush, btnLeftMove);
                        }

                        Font fontTemp = new Font("Arial", 10, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        int yTextBase = (int)(
                            (e.Bounds.Bottom - 37) + (25 - textSizeTemp.Height) / 2
                        );
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicCol.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }
                else if (_sizeModifyH)
                {
                    #region Control de eliminado

                    ///Delete
                    _startXButtonDelete = 0;
                    _endXButtonDelete = 0;

                    _startYButtonDelete = 0;
                    _endYButtonDelete = 0;

                    #endregion Control de eliminado

                    #region Control de etiqueta

                    ///Label
                    _startXButtonLabel = 0;
                    _endXButtonLabel = 0;

                    _startYButtonLabel = 0;
                    _endYButtonLabel = 0;

                    #endregion Control de etiqueta

                    #region Control de tamaño width

                    ///Width
                    _startXButtonWidth = 0;
                    _endXButtonWidth = 0;

                    _startYButtonWidth = 0;
                    _endYButtonWidth = 0;

                    #endregion Control de tamaño width

                    #region Control de tamaño height

                    ///Width
                    _startXButtonHeight = 0;
                    _endXButtonHeight = 0;

                    _startYButtonHeight = 0;
                    _endYButtonHeight = 0;

                    #endregion Control de tamaño height

                    #region Controles de rows

                    _startXButtonAddRow = 0;
                    _endXButtonAddRow = 0;
                    _startYButtonAddRow = 0;
                    _endYButtonAddRow = 0;

                    _startXButtonLessRow = 0;
                    _endXButtonLessRow = 0;
                    _startYButtonLessRow = 0;
                    _endYButtonLessRow = 0;

                    #endregion Controles de rows

                    #region Controles de tabs

                    ///RemoveTab
                    _startXButtonRemoveTab = 0;
                    _endXButtonRemoveTab = 0;
                    _startYButtonRemoveTab = 0;
                    _endYButtonRemoveTab = 0;

                    ///AddTab
                    _startXButtonAddTab = 0;
                    _endXButtonAddTab = 0;
                    _startYButtonAddTab = 0;
                    _endYButtonAddTab = 0;

                    #endregion Controles de tabs

                    #region Opciones de tamaño

                    int baseXMoveControls = e.Bounds.Left + 12;

                    int startXSizeDinamic = 0;
                    int endXSizeDinamic = 0;
                    int startYSizeDinamic = 0;
                    int endYSizeDinamic = 0;

                    _buttonsSizeDynamicRow.Clear();
                    int iLast =
                        (e.Item.Parent.OptionsTableLayoutGroup.RowCount)
                        - (e.Item.OptionsTableLayoutItem.RowIndex)
                        + 1;

                    for (int i = 2; i <= iLast; i++)
                    {
                        startXSizeDinamic =
                            (startXSizeDinamic == 0) ? baseXMoveControls : (endXSizeDinamic + 2);
                        endXSizeDinamic = startXSizeDinamic + sizeWidthIndicator;

                        startYSizeDinamic = e.Bounds.Bottom - 37;
                        endYSizeDinamic = startYSizeDinamic + (sizeHeightIndicator * 2);

                        Rectangle btnDinamicH = new Rectangle(
                            startXSizeDinamic,
                            startYSizeDinamic,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        if (i == e.Item.OptionsTableLayoutItem.RowSpan)
                        {
                            e.Graphics.DrawRectangle(
                                new Pen(baseControlsBrushSecondary),
                                btnDinamicH
                            );

                            e.Graphics.FillRectangle(baseControlsBrushSecondary, btnDinamicH);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnDinamicH);

                            e.Graphics.FillRectangle(baseControlsBrush, btnDinamicH);
                        }

                        Font fontTemp = new Font("Arial", 10, FontStyle.Regular);
                        LinearGradientBrush fontTempBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            Color.White,
                            Color.White,
                            180
                        );
                        SizeF textSizeTemp = e.Graphics.MeasureString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp
                        );
                        float xTextTempBase =
                            startXSizeDinamic + (sizeWidthIndicator / 2) - (textSizeTemp.Width / 2);
                        int yTextBase = (int)(
                            (e.Bounds.Bottom - 37) + (25 - textSizeTemp.Height) / 2
                        );
                        e.Graphics.DrawString(
                            (i == iLast) ? "X" : i.ToString(),
                            fontTemp,
                            fontTempBrush,
                            xTextTempBase,
                            yTextBase
                        );

                        _buttonsSizeDynamicRow.Add(
                            (
                                startX: startXSizeDinamic,
                                endX: endXSizeDinamic,
                                startY: startYSizeDinamic,
                                endY: endYSizeDinamic,
                                size: i,
                                last: (i == iLast)
                            )
                        );
                    }

                    #endregion Opciones de tamaño
                }

                if (_tabAdedd)
                {
                    _itemSelected = null;
                    _groupSelected = _layoutDesigner.Root;
                    _tabGroupSelected = null;
                    _typeSelectedControl = "LayoutControlGroup";

                    UpdateItemSelected("Root");
                }
            }

            #endregion Indicador de seleccionado

            e.Handled = true;
        }

        #endregion Dibujado


        #region Control de eliminado

        //Dimensiones del botón
        private int _startXButtonDelete = 0;
        private int _endXButtonDelete = 0;
        private int _startYButtonDelete = 0;
        private int _endYButtonDelete = 0;

        //Icono
        private string _pathIconDelete = string.Empty;

        #endregion Control de eliminado

        #region Control de etiqueta

        //Dimensiones del botón
        private int _startXButtonLabel = 0;
        private int _endXButtonLabel = 0;
        private int _startYButtonLabel = 0;
        private int _endYButtonLabel = 0;

        //Icono
        private string _pathIconLabel = string.Empty;

        #endregion Control de etiqueta

        #region Controles de tamaño dinámicos width

        //Dimensiones de los botones dinámicos
        //startX, endX, startY, endY, size
        private List<(
            int startX,
            int endX,
            int startY,
            int endY,
            int size,
            bool last
        )> _buttonsSizeDynamicCol =
            new List<(int startX, int endX, int startY, int endY, int size, bool last)>();

        //Dimensiones del botón
        private int _startXButtonWidth = 0;
        private int _endXButtonWidth = 0;
        private int _startYButtonWidth = 0;
        private int _endYButtonWidth = 0;

        //Icono
        private string _buttonWidth = string.Empty;

        #endregion Controles de tamaño dinámicos width

        #region Controles de tamaño dinámicos height

        //Dimensiones de los botones dinámicos
        //startX, endX, startY, endY, size
        private List<(
            int startX,
            int endX,
            int startY,
            int endY,
            int size,
            bool last
        )> _buttonsSizeDynamicRow =
            new List<(int startX, int endX, int startY, int endY, int size, bool last)>();

        //Dimensiones del botón
        private int _startXButtonHeight = 0;
        private int _endXButtonHeight = 0;
        private int _startYButtonHeight = 0;
        private int _endYButtonHeight = 0;

        //Icono
        private string _buttonHeight = string.Empty;

        #endregion Controles de tamaño dinámicos height

        #region Controles de renglones

        //Dimensiones del botón
        private int _startXButtonAddRow = 0;
        private int _endXButtonAddRow = 0;
        private int _startYButtonAddRow = 0;
        private int _endYButtonAddRow = 0;

        //Icono
        private string _pathRowAdd = string.Empty;

        //Dimensiones del botón
        private int _startXButtonLessRow = 0;
        private int _endXButtonLessRow = 0;
        private int _startYButtonLessRow = 0;
        private int _endYButtonLessRow = 0;

        //Icono
        private string _pathRowLess = string.Empty;

        #endregion Controles de renglones

        #region Control para tabs

        //Dimensiones del botón
        private int _startXButtonAddTab = 0;
        private int _endXButtonAddTab = 0;
        private int _startYButtonAddTab = 0;
        private int _endYButtonAddTab = 0;

        //Icono
        private string _pathIconAddTab = string.Empty;

        //Dimensiones del botón
        private int _startXButtonRemoveTab = 0;
        private int _endXButtonRemoveTab = 0;
        private int _startYButtonRemoveTab = 0;
        private int _endYButtonRemoveTab = 0;

        //Icono
        private string _pathIconRemoveTab = string.Empty;

        #endregion Control para tabs


        #region Iconos de los controles

        private void InitializeControlerIcons()
        {
            //Delete
            _pathIconDelete = $@"{_project.DirProyectData}\LayOuts\icons\trash3-fill.svg";
            //Label
            _pathIconLabel = $@"{_project.DirProyectData}\LayOuts\icons\textarea-t.svg";
            //Width
            _buttonWidth = $@"{_project.DirProyectData}\LayOuts\icons\arrow-left-right.svg";
            //Height
            _buttonHeight = $@"{_project.DirProyectData}\LayOuts\icons\arrow-down-up.svg";

            //AddRow
            _pathRowAdd = $@"{_project.DirProyectData}\LayOuts\icons\file-plus.svg";
            //LessRow
            _pathRowLess = $@"{_project.DirProyectData}\LayOuts\icons\file-minus.svg";

            //AddTab
            _pathIconAddTab = $@"{_project.DirProyectData}\LayOuts\icons\window-plus.svg";
            //RemoveTab
            _pathIconRemoveTab = $@"{_project.DirProyectData}\LayOuts\icons\window-x.svg";
        }

        #endregion Iconos de los controles


        #region Eventos de los botones en el dibujado

        [SupportedOSPlatform("windows")]
        private void MouseClick_LcLayoutDesigner(object sender, MouseEventArgs e)
        {
            if (_tabAdedd)
            {
                _tabAdedd = false;
                WoDesignerSerializerHelper woDesignerSerializerHelper =
                    new WoDesignerSerializerHelper(layoutDesigner: _layoutDesigner);

                WoContainer container = woDesignerSerializerHelper.SerilizeFormToJson();
                ChargeJsonLayout(container);
            }
            else
            {
                #region Evento click del botón de eliminado

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonDelete
                        && e.X <= _endXButtonDelete
                        && e.Y >= _startYButtonDelete
                        && e.Y <= _endYButtonDelete
                    )
                    {
                        DeleteControl();
                    }
                }

                #endregion Evento click del botón de eliminado

                #region Evento click del botón de cambiar texto

                if (
                    e.Button == MouseButtons.Left
                    && _startXButtonLabel != 0
                    && _endXButtonLabel != 0
                    && _startYButtonLabel != 0
                    && _endYButtonLabel != 0
                )
                {
                    if (
                        e.X >= _startXButtonLabel
                        && e.X <= _endXButtonLabel
                        && e.Y >= _startYButtonLabel
                        && e.Y <= _endYButtonLabel
                    )
                    {
                        Config.fmLabelsSelector modalEditor = new Config.fmLabelsSelector();

                        if (modalEditor.ShowDialog() == DialogResult.OK)
                        {
                            string text = EtiquetaCol.Get(modalEditor.Id);

                            ChangeTitle(modalEditor.Id, text);
                        }
                    }
                }

                #endregion Evento click del botón de cambiar texto

                #region Evento click de los botones de tamaño dinámico width

                if (_sizeModifyW && e.Button == MouseButtons.Left)
                {
                    (int startX, int endX, int startY, int endY, int size, bool last) newSize =
                        _buttonsSizeDynamicCol
                            .Where(x =>
                                e.X >= x.startX && e.X <= x.endX && e.Y >= x.startY && e.Y <= x.endY
                            )
                            .FirstOrDefault();

                    if (newSize.size != null)
                    {
                        if (newSize.last)
                        {
                            _sizeModifyW = false;
                            _buttonsSizeDynamicCol.Clear();
                        }
                        else
                        {
                            if (
                                _typeSelectedControl == "LayoutControlItem"
                                || _typeSelectedControl == "SimpleLabelItem"
                            )
                            {
                                _itemSelected.OptionsTableLayoutItem.ColumnSpan = newSize.size;
                            }
                            else if (
                                _typeSelectedControl == "LayoutControlGroup"
                                && _tabGroupSelected == null
                            )
                            {
                                _groupSelected.OptionsTableLayoutItem.ColumnSpan = newSize.size;
                            }
                            else if (
                                _typeSelectedControl == "TabbedControlGroup"
                                || _tabGroupSelected != null
                            )
                            {
                                _tabGroupSelected.OptionsTableLayoutItem.ColumnSpan = newSize.size;
                                _layoutDesigner.Refresh();
                                _layoutDesigner.Update();
                            }
                        }
                    }
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonWidth
                        && e.X <= _endXButtonWidth
                        && e.Y >= _startYButtonWidth
                        && e.Y <= _endYButtonWidth
                    )
                    {
                        _sizeModifyW = true;
                    }
                }

                #endregion Evento click de los botones de tamaño dinámico width

                #region Evento click de los botones de tamaño dinámico height

                if (_sizeModifyH && e.Button == MouseButtons.Left)
                {
                    (int startX, int endX, int startY, int endY, int size, bool last) newSize =
                        _buttonsSizeDynamicRow
                            .Where(x =>
                                e.X >= x.startX && e.X <= x.endX && e.Y >= x.startY && e.Y <= x.endY
                            )
                            .FirstOrDefault();

                    if (newSize.size != null)
                    {
                        if (newSize.last)
                        {
                            _sizeModifyW = false;
                            _sizeModifyH = false;
                            _buttonsSizeDynamicRow.Clear();
                        }
                        else
                        {
                            if (
                                _typeSelectedControl == "LayoutControlItem"
                                || _typeSelectedControl == "SimpleLabelItem"
                            )
                            {
                                _itemSelected.OptionsTableLayoutItem.RowSpan = newSize.size;
                            }
                            else if (
                                _typeSelectedControl == "LayoutControlGroup"
                                && _tabGroupSelected == null
                            )
                            {
                                _groupSelected.OptionsTableLayoutItem.RowSpan = newSize.size;
                            }
                            else if (
                                _typeSelectedControl == "TabbedControlGroup"
                                || _tabGroupSelected != null
                            )
                            {
                                _tabGroupSelected.OptionsTableLayoutItem.RowSpan = newSize.size;
                            }
                        }
                    }
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonHeight
                        && e.X <= _endXButtonHeight
                        && e.Y >= _startYButtonHeight
                        && e.Y <= _endYButtonHeight
                    )
                    {
                        _sizeModifyW = false;
                        _sizeModifyH = true;
                    }
                }

                #endregion Evento click de los botones de tamaño dinámico height

                #region Eventos click de los botones de rows

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonAddRow
                        && e.X <= _endXButtonAddRow
                        && e.Y >= _startYButtonAddRow
                        && e.Y <= _endYButtonAddRow
                    )
                    {
                        AddRow();
                    }
                    else if (
                        e.X >= _startXButtonLessRow
                        && e.X <= _endXButtonLessRow
                        && e.Y >= _startYButtonLessRow
                        && e.Y <= _endYButtonLessRow
                    )
                    {
                        DeleteRow();
                    }
                }

                #endregion Eventos click de los botones de rows

                #region Evento click de los botones para las tabs

                if (
                    e.Button == MouseButtons.Left
                    && _startXButtonAddTab != 0
                    && _endXButtonAddTab != 0
                    && _startYButtonAddTab != 0
                    && _endYButtonAddTab != 0
                    && _startXButtonRemoveTab != 0
                    && _endXButtonRemoveTab != 0
                    && _startYButtonRemoveTab != 0
                    && _endYButtonRemoveTab != 0
                )
                {
                    if (
                        e.X >= _startXButtonAddTab
                        && e.X <= _endXButtonAddTab
                        && e.Y >= _startYButtonAddTab
                        && e.Y <= _endYButtonAddTab
                    )
                    {
                        CreateTab(internalRows: 2);
                    }
                    else if (
                        e.X >= _startXButtonRemoveTab
                        && e.X <= _endXButtonRemoveTab
                        && e.Y >= _startYButtonRemoveTab
                        && e.Y <= _endYButtonRemoveTab
                    )
                    {
                        DeleteControl(true);
                    }
                }

                #endregion Evento click de los botones para las tabs
            }

            _layoutDesigner.Root.Update();
            _layoutDesigner.Root.Update();
        }

        #endregion Eventos de los botones en el dibujado
    }
}
