using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using Svg;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerComponents;
using WooW.SB.Designer.DesignerHelpers;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer
{
    public partial class WoFormDesigner : UserControl
    {
        #region Actualización y re dibujado base

        /// <summary>
        /// Controlador de eventos que se detonara cuando se cambie el identificador del item.
        /// </summary>
        public Func<WoComponentProperties, List<(string oldName, string newName)>> UpdateIdEvt;

        /// <summary>
        /// Permite actualizar el componente seleccionado con la información en la
        /// instancia de WoComponentPorperties que se recibe por parámetro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="woComponentProperties"></param>
        [SupportedOSPlatform("windows")]
        private void UpdateComponent(object sender, WoComponentProperties woComponentProperties)
        {
            if (woComponentProperties.ChangedProperty == "Step")
            {
                WoComponentProperties properties = (WoComponentProperties)_itemSelected.Tag;

                properties.Step = woComponentProperties.Step;
                _itemSelected.Tag = properties;
            }
            else if (woComponentProperties.ChangedProperty == "Max")
            {
                WoComponentProperties properties = (WoComponentProperties)_itemSelected.Tag;

                properties.Max = woComponentProperties.Max;
                _itemSelected.Tag = properties;
            }
            else if (woComponentProperties.ChangedProperty == "Min")
            {
                WoComponentProperties properties = (WoComponentProperties)_itemSelected.Tag;

                properties.Min = woComponentProperties.Min;
                _itemSelected.Tag = properties;
            }

            if (
                woComponentProperties.ChangedProperty == "InputString"
                || woComponentProperties.ChangedProperty == "InputNumeric"
                || woComponentProperties.ChangedProperty == "InputDate"
            )
            {
                DesingFormOptions designerFormOptions = (DesingFormOptions)
                    _layoutDesigner.CustomizationForm;

                WoComponentProperties properties = (WoComponentProperties)_itemSelected.Tag;

                if (woComponentProperties.InputString != eInputString.None)
                {
                    designerFormOptions.SetComponentProperties(
                        selectedControl: $@"{properties.Id}",
                        componentProperties: properties,
                        showProperties: new List<string>()
                        {
                            "ItemSize",
                            "BackgorundColorContainerItem",
                            "CaptionColor",
                            "CaptionItalic",
                            "CaptionWide",
                            "CaptionDecoration",
                            "PlaceHolder",
                            "CustomMask",
                            "InputString",
                        },
                        hideProperties: new List<string>()
                        {
                            "BackgorundColorGroup",
                            "ComponentFontSize",
                            "Visible",
                            "WidthSlave",
                            "HeightSlave",
                            "Icon",
                            "Password",
                            "LookUpInputSize",
                            "Max",
                            "Min",
                            "Step",
                            "InputNumeric",
                            "InputDate",
                        }
                    );
                }
                else if (woComponentProperties.InputNumeric == eInputNumeric.Custom)
                {
                    designerFormOptions.SetComponentProperties(
                        selectedControl: $@"{properties.Id}",
                        componentProperties: properties,
                        showProperties: new List<string>()
                        {
                            "ItemSize",
                            "BackgorundColorContainerItem",
                            "CaptionColor",
                            "CaptionItalic",
                            "CaptionWide",
                            "CaptionDecoration",
                            "PlaceHolder",
                            "CustomMask",
                            "InputNumeric"
                        },
                        hideProperties: new List<string>()
                        {
                            "BackgorundColorGroup",
                            "ComponentFontSize",
                            "Visible",
                            "WidthSlave",
                            "HeightSlave",
                            "Icon",
                            "Password",
                            "LookUpInputSize",
                            "Max",
                            "Min",
                            "Step",
                            "InputString",
                            "InputDate",
                        }
                    );
                }
                else if (woComponentProperties.InputDate == eInputDate.Custom)
                {
                    designerFormOptions.SetComponentProperties(
                        selectedControl: $@"{properties.Id}",
                        componentProperties: properties,
                        showProperties: new List<string>()
                        {
                            "ItemSize",
                            "BackgorundColorContainerItem",
                            "CaptionColor",
                            "CaptionItalic",
                            "CaptionWide",
                            "CaptionDecoration",
                            "PlaceHolder",
                            "CustomMask",
                            "InputDate",
                        },
                        hideProperties: new List<string>()
                        {
                            "BackgorundColorGroup",
                            "ComponentFontSize",
                            "Visible",
                            "WidthSlave",
                            "HeightSlave",
                            "Icon",
                            "Password",
                            "LookUpInputSize",
                            "Max",
                            "Min",
                            "Step",
                            "InputNumeric",
                            "InputString",
                        }
                    );
                }
                else
                {
                    List<string> showProperties = new List<string>()
                    {
                        "ItemSize",
                        "BackgorundColorContainerItem",
                        "CaptionColor",
                        "CaptionItalic",
                        "CaptionWide",
                        "CaptionDecoration",
                        "PlaceHolder",
                    };

                    List<string> hideProperties = new List<string>()
                    {
                        "BackgorundColorGroup",
                        "ComponentFontSize",
                        "Visible",
                        "WidthSlave",
                        "HeightSlave",
                        "Icon",
                        "Password",
                        "LookUpInputSize",
                        "Max",
                        "Min",
                        "Step",
                        "CustomMask",
                    };

                    if (
                        properties.BindingType == "string"
                        || properties.BindingType == "string?"
                        || properties.BindingType == "String"
                        || properties.BindingType == "String?"
                    )
                    {
                        showProperties.Add("InputString");

                        hideProperties.Add("InputNumeric");
                        hideProperties.Add("InputDate");
                    }
                    else if (
                        properties.BindingType == "DateTime"
                        || properties.BindingType == "DateTime?"
                    )
                    {
                        showProperties.Add("InputDate");

                        hideProperties.Add("InputString");
                        hideProperties.Add("InputNumeric");
                    }
                    else if (
                        properties.BindingType == "int"
                        || properties.BindingType == "int?"
                        || properties.BindingType == "Decimal"
                        || properties.BindingType == "Decimal?"
                        || properties.BindingType == "double"
                        || properties.BindingType == "double?"
                        || properties.BindingType == "long"
                        || properties.BindingType == "long?"
                    )
                    {
                        showProperties.Add("InputNumeric");

                        hideProperties.Add("InputString");
                        hideProperties.Add("InputDate");
                    }

                    designerFormOptions.SetComponentProperties(
                        selectedControl: $@"{properties.Id}",
                        componentProperties: properties,
                        showProperties: showProperties,
                        hideProperties: hideProperties
                    );
                }
            }

            if (woComponentProperties.ChangedProperty == "Etiqueta")
            {
                woComponentProperties.ChangedProperty = string.Empty;
                ChangeTitle(woComponentProperties.Etiqueta, woComponentProperties.MaskText);
            }

            List<(string oldName, string newName)> changedMethods = UpdateIdEvt?.Invoke(
                woComponentProperties
            );
            UpdateIdSaveData(woComponentProperties, changedMethods);

            if (
                _typeSelectedControl == "LayoutControlItem"
                || _typeSelectedControl == "SimpleLabelItem"
            )
            {
                RePaintItem(woComponentProperties);
            }
            else if (_typeSelectedControl == "LayoutControlGroup")
            {
                if (woComponentProperties.TypeContainer == eTypeContainer.FormTab)
                {
                    RePaintTab(woComponentProperties);
                }
                else
                {
                    RePaintContainer(woComponentProperties);
                }
            }
            else if (_typeSelectedControl == "TabbedControlGroup")
            {
                RePaintTab(woComponentProperties);
            }

            if (EditingEvt != null)
            {
                EditingEvt.Invoke(this, false);
            }
        }

        /// <summary>
        /// Actualiza el nombre de los métodos que cambian con el nombre de los ids de los controles.
        /// </summary>
        /// <param name="woComponentProperties"></param>
        /// <param name="changedMethods"></param>
        [SupportedOSPlatform("windows")]
        private void UpdateIdSaveData(
            WoComponentProperties woComponentProperties,
            List<(string oldName, string newName)> changedMethods
        )
        {
            if (woComponentProperties.ChangedProperty == "Id")
            {
                if (
                    File.Exists(
                        $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs"
                    )
                )
                {
                    //Actualización de la clase ScriptsUser
                    WoSyntaxManagerUserCode woSyntaxManagerUserCode = new WoSyntaxManagerUserCode();
                    woSyntaxManagerUserCode.InitializeManager(
                        pathScript: $@"{_project.DirLayOuts}\UserCode\{_modelName}_proj\{_modelName}ScriptsUser.cs",
                        className: _modelName,
                        modelName: _modelName
                    );

                    foreach ((string oldName, string newName) method in changedMethods)
                    {
                        woSyntaxManagerUserCode.RenameMethod(
                            oldName: method.oldName,
                            newName: method.newName
                        );
                    }

                    //Actualización del control models
                    //WoBlazorSave woBlazorSave = new WoBlazorSave(
                    //    modelName: _modelName,
                    //    className: _className,
                    //    isSlave: false,
                    //    woContainer: GetLastVersionGroup()
                    //);
                    //woBlazorSave.BuildControlModels(rewrite: true);
                }
            }
        }

        /// <summary>
        /// Re dibuja el item seleccionado en función del parámetro que se recibe y actualiza
        /// el tag del item con la instancia de WoComponentProperties que se recibe por parámetro.
        /// </summary>
        /// <param name="woComponentPropertiesStyle"></param>
        [SupportedOSPlatform("windows")]
        private void RePaintItem(WoComponentProperties woComponentPropertiesStyle)
        {
            if (woComponentPropertiesStyle.Id != _itemSelected.Name)
            {
                var response = _layoutDesigner
                    .Items.Where(x => x.Name == woComponentPropertiesStyle.Id)
                    .FirstOrDefault();
                if (response == null)
                {
                    _itemSelected.Name = woComponentPropertiesStyle.Id;
                }
                else
                {
                    MessageBox.Show(
                        "EL id no se puede duplicar para dos componentes iguales",
                        "Alerta"
                    );
                    woComponentPropertiesStyle.Id = _itemSelected.Name;
                }
            }
            _itemSelected.Tag = woComponentPropertiesStyle;
            _itemSelected.Update();
        }

        /// <summary>
        /// Re dibuja el contenedor en función del parámetro que se recibe y actualiza
        /// el tag del contenedor con la instancia de WoComponentProperties que se recibe por parámetro.
        /// </summary>
        /// <param name="woComponentPropertiesStyle"></param>
        [SupportedOSPlatform("windows")]
        private void RePaintContainer(WoComponentProperties woComponentPropertiesStyle)
        {
            if (woComponentPropertiesStyle.Id != _groupSelected.Name)
            {
                var response = _layoutDesigner
                    .Items.Where(x => x.Name == woComponentPropertiesStyle.Id)
                    .FirstOrDefault();
                if (response == null)
                {
                    _groupSelected.Name = woComponentPropertiesStyle.Id;
                }
                else
                {
                    MessageBox.Show(
                        "EL id no se puede duplicar para dos componentes iguales",
                        "Alerta"
                    );
                    woComponentPropertiesStyle.Id = _groupSelected.Name;
                }
            }
            _groupSelected.Tag = woComponentPropertiesStyle;
            _groupSelected.Update();
        }

        /// <summary>
        /// Re dibuja el tab en función del parámetro que se recibe y actualiza
        /// el tag del contenedor con la instancia de WoComponentProperties que se recibe por parámetro.
        /// </summary>
        /// <param name="woComponentPropertiesStyle"></param>
        [SupportedOSPlatform("windows")]
        private void RePaintTab(WoComponentProperties woComponentPropertiesStyle)
        {
            if (woComponentPropertiesStyle.Id != _groupSelected.Name)
            {
                var response = _layoutDesigner
                    .Items.Where(x => x.Name == woComponentPropertiesStyle.Id)
                    .FirstOrDefault();
                if (response == null)
                {
                    _groupSelected.Name = woComponentPropertiesStyle.Id;
                }
                else
                {
                    MessageBox.Show(
                        "EL id no se puede duplicar para dos componentes iguales",
                        "Alerta"
                    );
                    woComponentPropertiesStyle.Id = _groupSelected.Name;
                }
            }
            _groupSelected.Tag = woComponentPropertiesStyle;
            _tabGroupSelected.Update();
        }

        #endregion Actualización y re dibujado base


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
        private void ItemCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.Handled = false;

            #region Variables principales

            Modelo findModel = _project.ModeloCol.Modelos.Where(x => x.Id == _modelName).First();

            // Recuperación del componente.
            WoComponentProperties componentProperties = (WoComponentProperties)e.Item.Tag;

            string maskText = string.Empty;

            if (componentProperties.Etiqueta != null)
            {
                maskText = componentProperties.MaskText;
            }
            else
            {
                ModeloColumna column = findModel
                    .Columnas.Where(x => x.Id == componentProperties.BindedProperty)
                    .FirstOrDefault();
                if (column != null)
                {
                    maskText = column.Formulario;
                }
            }

            string parentName = e.Item.Parent.Name;

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
            WoComponentProperties parentComponentProperties = (WoComponentProperties)
                e.Item.Parent.Tag;
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

            if (componentProperties.Id != "Controles" && componentProperties.Id != "Alertas")
            {
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

            if (
                componentProperties.Control == "Text"
                || componentProperties.Control == "File"
                || componentProperties.Control == "MaskText"
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
            else if (componentProperties.Control == "Custom")
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
                    $@"{componentProperties.BindingType}",
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
                int unitComponent = componentSize;
                int widthComponent = e.Bounds.Width - labelSize + e.Bounds.Left + unitComponent;
                int sizeComponent1 = (widthComponent / 2) - 2 - unitComponent;
                int sizeComponent2 = (widthComponent / 2) - 2;

                if (componentProperties.LookUpInputSize == eLookupInputWidth.Small)
                {
                    sizeComponent1 = (widthComponent / 4) - 2 - unitComponent;
                    sizeComponent2 = ((widthComponent / 4) * 3) - 2;
                }
                else if (componentProperties.LookUpInputSize == eLookupInputWidth.Large)
                {
                    sizeComponent1 = ((widthComponent / 4) * 3) - unitComponent - 2;
                    sizeComponent2 = (widthComponent / 4) - 2;
                }

                int secondLabel = labelSize + sizeComponent1 + 4;

                e.Graphics.FillRectangle(
                    backgroundControlBrush,
                    labelSize,
                    e.Bounds.Y,
                    sizeComponent1,
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
                    labelSize + sizeComponent1 - componentSize,
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
                    secondLabel + 4,
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
                    labelSize + sizeComponent1 - (unitComponent / 2) - (iconSize / 2),
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
            else if (componentProperties.Control == "Label")
            {
                if (componentProperties.Id == "Controles")
                {
                    int yBaseArrow = yTextBase;

                    //Flechita 1
                    int xBaseArrow1 = xTextBase;

                    System.Drawing.Image iconChevronDoubleLeft = null;

                    var svgChevronDoubleLeft = SvgDocument.Open(
                        $@"{_iconPath}\chevron-double-left.svg"
                    );
                    for (int i = 0; i < svgChevronDoubleLeft.Children.Count; i++)
                    {
                        svgChevronDoubleLeft.Children[i].Fill = new SvgColourServer(Color.Black);
                    }
                    iconChevronDoubleLeft = new Bitmap(svgChevronDoubleLeft.Draw());

                    e.Graphics.DrawImage(
                        iconChevronDoubleLeft,
                        xBaseArrow1,
                        yBaseArrow,
                        iconSize,
                        iconSize
                    );

                    //Flechita 2
                    int xBaseArrow2 = xBaseArrow1 + iconSize + 2;

                    System.Drawing.Image iconChevronLeft = null;

                    var svgChevronLeft = SvgDocument.Open($@"{_iconPath}\chevron-left.svg");
                    for (int i = 0; i < svgChevronLeft.Children.Count; i++)
                    {
                        svgChevronLeft.Children[i].Fill = new SvgColourServer(Color.Black);
                    }
                    iconChevronLeft = new Bitmap(svgChevronLeft.Draw());

                    e.Graphics.DrawImage(
                        iconChevronLeft,
                        xBaseArrow2,
                        yBaseArrow,
                        iconSize,
                        iconSize
                    );

                    //Flechita 3
                    int xBaseArrow3 = xBaseArrow2 + iconSize + 4;

                    System.Drawing.Image iconChevronRight = null;

                    var svgChevronRight = SvgDocument.Open($@"{_iconPath}\chevron-right.svg");
                    for (int i = 0; i < svgChevronRight.Children.Count; i++)
                    {
                        svgChevronRight.Children[i].Fill = new SvgColourServer(Color.Black);
                    }
                    iconChevronRight = new Bitmap(svgChevronRight.Draw());

                    e.Graphics.DrawImage(
                        iconChevronRight,
                        xBaseArrow3,
                        yBaseArrow,
                        iconSize,
                        iconSize
                    );

                    //Flechita 4
                    int xBaseArrow4 = xBaseArrow3 + iconSize + 2;

                    System.Drawing.Image iconChevronDoubleRight = null;

                    var svgChevronDoubleRight = SvgDocument.Open(
                        $@"{_iconPath}\chevron-double-right.svg"
                    );
                    for (int i = 0; i < svgChevronDoubleRight.Children.Count; i++)
                    {
                        svgChevronDoubleRight.Children[i].Fill = new SvgColourServer(Color.Black);
                    }
                    iconChevronDoubleRight = new Bitmap(svgChevronDoubleRight.Draw());

                    e.Graphics.DrawImage(
                        iconChevronDoubleRight,
                        xBaseArrow4,
                        yBaseArrow,
                        iconSize,
                        iconSize
                    );

                    //Separador

                    int xBaseDivider = xBaseArrow4 + iconSize + 6;
                    int yBase1 = e.Bounds.Y;
                    int yBase2 = e.Bounds.Y + componentSize + 1;

                    e.Graphics.DrawLine(
                        new System.Drawing.Pen(borderBrush),
                        xBaseDivider,
                        yBase1,
                        xBaseDivider,
                        yBase2
                    );

                    //Dropdown rectangle
                    int xBaseTransition = xBaseDivider + 6;
                    int yBaseTransition = e.Bounds.Y;
                    int baseTransitionWidth = iconSize * 10;
                    int baseTransitionHeight = componentSize;

                    e.Graphics.DrawRectangle(
                        new System.Drawing.Pen(borderBrush),
                        xBaseTransition,
                        yBaseTransition,
                        baseTransitionWidth,
                        baseTransitionHeight
                    );

                    //Dropdown text
                    e.Graphics.DrawString(
                        $@"Transición...",
                        componentFont,
                        componentFontBrush,
                        xBaseTransition + 2,
                        yTextBase
                    );

                    //Dropdown icon rectangle
                    int xBaseDropdown = xBaseTransition + baseTransitionWidth;
                    e.Graphics.DrawRectangle(
                        new System.Drawing.Pen(borderBrush),
                        xBaseDropdown,
                        yBaseTransition,
                        baseTransitionHeight,
                        baseTransitionHeight
                    );

                    //Dropdown icon
                    int xBaseDropdownIcon = xBaseDropdown + iconSize / 2;

                    System.Drawing.Image iconCaretDownFill = null;
                    var svgCaretDownFill = SvgDocument.Open($@"{_iconPath}\caret-down-fill.svg");
                    for (int i = 0; i < svgCaretDownFill.Children.Count; i++)
                    {
                        svgCaretDownFill.Children[i].Fill = new SvgColourServer(Color.Black);
                    }
                    iconCaretDownFill = new Bitmap(svgCaretDownFill.Draw());

                    e.Graphics.DrawImage(
                        iconCaretDownFill,
                        xBaseDropdownIcon,
                        yTextBase + 2,
                        iconSize,
                        iconSize
                    );

                    //Ejecutar button icon
                    int xBaseRunIcon = xBaseDropdown + baseTransitionHeight + 10;

                    System.Drawing.Image iconCheck2 = null;
                    var svgCheck2 = SvgDocument.Open($@"{_iconPath}\check2.svg");
                    for (int i = 0; i < svgCheck2.Children.Count; i++)
                    {
                        svgCheck2.Children[i].Fill = new SvgColourServer(Color.Green);
                    }
                    iconCheck2 = new Bitmap(svgCheck2.Draw());

                    e.Graphics.DrawImage(iconCheck2, xBaseRunIcon, yTextBase, iconSize, iconSize);

                    //Ejecutar button text
                    int xBaseRunText = xBaseRunIcon + iconSize;
                    SizeF sizeRunText = e.Graphics.MeasureString($@"Ejecutar", componentFont);

                    LinearGradientBrush runTextComponentFontBrush = new LinearGradientBrush(
                        new System.Drawing.Rectangle(
                            e.Bounds.X,
                            e.Bounds.Y,
                            e.Bounds.Width,
                            e.Bounds.Height
                        ),
                        ///Solo para realizar la instancia del brush, el color se asigna luego.
                        Color.Green,
                        Color.Green,
                        180
                    );

                    e.Graphics.DrawString(
                        $@"Ejecutar",
                        componentFont,
                        runTextComponentFontBrush,
                        xBaseRunText,
                        yTextBase
                    );

                    //Cancelar button icon
                    int xBaseCancelIcon = (int)(xBaseRunText + sizeRunText.Width + 2);

                    System.Drawing.Image iconX = null;
                    var svgX = SvgDocument.Open($@"{_iconPath}\x-lg.svg");
                    for (int i = 0; i < svgX.Children.Count; i++)
                    {
                        svgX.Children[i].Fill = new SvgColourServer(Color.Red);
                    }
                    iconX = new Bitmap(svgX.Draw());

                    e.Graphics.DrawImage(iconX, xBaseCancelIcon, yTextBase, iconSize, iconSize);

                    //Cancelar button text
                    int xBaseCancelText = xBaseCancelIcon + iconSize;
                    SizeF sizeCancelText = e.Graphics.MeasureString($@"Cancelar", componentFont);

                    LinearGradientBrush cancelTextComponentFontBrush = new LinearGradientBrush(
                        new System.Drawing.Rectangle(
                            e.Bounds.X,
                            e.Bounds.Y,
                            e.Bounds.Width,
                            e.Bounds.Height
                        ),
                        ///Solo para realizar la instancia del brush, el color se asigna luego.
                        Color.Red,
                        Color.Red,
                        180
                    );

                    e.Graphics.DrawString(
                        $@"Cancelar",
                        componentFont,
                        cancelTextComponentFontBrush,
                        xBaseCancelText,
                        yTextBase
                    );

                    //Eliminar button icon
                    int xBaseDeleteIcon = (int)(xBaseCancelText + sizeCancelText.Width + 2);

                    System.Drawing.Image iconTrash = null;
                    var svgTrash = SvgDocument.Open($@"{_iconPath}\trash.svg");
                    for (int i = 0; i < svgTrash.Children.Count; i++)
                    {
                        svgTrash.Children[i].Fill = new SvgColourServer(Color.Red);
                    }
                    iconTrash = new Bitmap(svgTrash.Draw());

                    e.Graphics.DrawImage(iconTrash, xBaseDeleteIcon, yTextBase, iconSize, iconSize);

                    //Eliminar button text
                    int xBaseDeleteText = xBaseDeleteIcon + iconSize;
                    SizeF sizeDeleteText = e.Graphics.MeasureString($@"Eliminar", componentFont);

                    LinearGradientBrush deleteTextComponentFontBrush = new LinearGradientBrush(
                        new System.Drawing.Rectangle(
                            e.Bounds.X,
                            e.Bounds.Y,
                            e.Bounds.Width,
                            e.Bounds.Height
                        ),
                        ///Solo para realizar la instancia del brush, el color se asigna luego.
                        Color.Red,
                        Color.Red,
                        180
                    );

                    e.Graphics.DrawString(
                        $@"Eliminar",
                        componentFont,
                        deleteTextComponentFontBrush,
                        xBaseDeleteText,
                        yTextBase
                    );

                    //Compartir button icon
                    int xBaseShareIcon = (int)(xBaseDeleteText + sizeDeleteText.Width + 2);

                    System.Drawing.Image iconShare = null;
                    var svgShare = SvgDocument.Open($@"{_iconPath}\share-fill.svg");
                    for (int i = 0; i < svgShare.Children.Count; i++)
                    {
                        svgShare.Children[i].Fill = new SvgColourServer(Color.Blue);
                    }
                    iconShare = new Bitmap(svgShare.Draw());

                    e.Graphics.DrawImage(iconShare, xBaseShareIcon, yTextBase, iconSize, iconSize);

                    //Compartir button text
                    int xBaseShareText = xBaseShareIcon + iconSize;
                    SizeF sizeShareText = e.Graphics.MeasureString($@"Compartir", componentFont);

                    LinearGradientBrush shareTextComponentFontBrush = new LinearGradientBrush(
                        new System.Drawing.Rectangle(
                            e.Bounds.X,
                            e.Bounds.Y,
                            e.Bounds.Width,
                            e.Bounds.Height
                        ),
                        ///Solo para realizar la instancia del brush, el color se asigna luego.
                        Color.Blue,
                        Color.Blue,
                        180
                    );

                    e.Graphics.DrawString(
                        $@"Compartir",
                        componentFont,
                        shareTextComponentFontBrush,
                        xBaseShareText,
                        yTextBase
                    );

                    //Indicador de botones custom

                    //Recuperación de los botones custom
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

                    List<WoCustomButtonProperties> customButtons =
                        woProjectDataHelper.GetCustomButtons(_modelName);

                    if (customButtons.Count > 0)
                    {
                        int buttonsCount = customButtons.Count;

                        //Indicador de botones custom
                        int xBaseCustomIcon = (int)(xBaseShareText + sizeShareText.Width + 2);

                        System.Drawing.Image iconCustom = null;
                        var svgCustom = SvgDocument.Open($@"{_iconPath}\node-plus.svg");
                        for (int i = 0; i < svgCustom.Children.Count; i++)
                        {
                            svgCustom.Children[i].Fill = new SvgColourServer(Color.Black);
                        }
                        iconCustom = new Bitmap(svgCustom.Draw());

                        e.Graphics.DrawImage(
                            iconCustom,
                            xBaseCustomIcon,
                            yTextBase,
                            iconSize,
                            iconSize
                        );

                        //botones custom text
                        int xBaseCustomText = xBaseCustomIcon + iconSize;
                        SizeF sizeCustomText = e.Graphics.MeasureString($@"00", componentFont);

                        LinearGradientBrush customTextComponentFontBrush = new LinearGradientBrush(
                            new System.Drawing.Rectangle(
                                e.Bounds.X,
                                e.Bounds.Y,
                                e.Bounds.Width,
                                e.Bounds.Height
                            ),
                            ///Solo para realizar la instancia del brush, el color se asigna luego.
                            Color.Black,
                            Color.Black,
                            180
                        );

                        e.Graphics.DrawString(
                            buttonsCount.ToString(),
                            componentFont,
                            customTextComponentFontBrush,
                            xBaseCustomText,
                            yTextBase
                        );
                    }
                }
                if (componentProperties.Id == "Alertas")
                {
                    LinearGradientBrush alertPrimaryComponentFontBrush = new LinearGradientBrush(
                        new System.Drawing.Rectangle(
                            e.Bounds.X,
                            e.Bounds.Y,
                            e.Bounds.Width,
                            e.Bounds.Height
                        ),
                        ///Solo para realizar la instancia del brush, el color se asigna luego.
                        Color.DarkBlue,
                        Color.DarkBlue,
                        180
                    );
                    LinearGradientBrush alertSecondaryComponentFontBrush = new LinearGradientBrush(
                        new System.Drawing.Rectangle(
                            e.Bounds.X,
                            e.Bounds.Y,
                            e.Bounds.Width,
                            e.Bounds.Height
                        ),
                        ///Solo para realizar la instancia del brush, el color se asigna luego.
                        Color.LightBlue,
                        Color.LightBlue,
                        180
                    );
                    LinearGradientBrush alertTertiaryComponentFontBrush = new LinearGradientBrush(
                        new System.Drawing.Rectangle(
                            e.Bounds.X,
                            e.Bounds.Y,
                            e.Bounds.Width,
                            e.Bounds.Height
                        ),
                        ///Solo para realizar la instancia del brush, el color se asigna luego.
                        Color.CornflowerBlue,
                        Color.CornflowerBlue,
                        180
                    );

                    //Alert border rectangle
                    int xBaseBRectangle = e.Bounds.X + 2;
                    int yBaseBRectangle = e.Bounds.Y;
                    int baseBRectangleWidth = iconSize * 10;
                    int baseBRectangleHeight = componentSize;
                    int widthComponent = e.Bounds.Width;

                    e.Graphics.FillRectangle(
                        alertSecondaryComponentFontBrush,
                        xBaseBRectangle,
                        yBaseBRectangle,
                        widthComponent,
                        baseBRectangleHeight
                    );

                    e.Graphics.DrawRectangle(
                        new System.Drawing.Pen(alertTertiaryComponentFontBrush),
                        xBaseBRectangle,
                        yBaseBRectangle,
                        widthComponent,
                        baseBRectangleHeight
                    );

                    //Alert Warning icon

                    int xBaseAlertIcon = xBaseBRectangle + 6;

                    System.Drawing.Image iconExclamationCircle = null;
                    var svgExclamationCircle = SvgDocument.Open(
                        $@"{_iconPath}\exclamation-circle.svg"
                    );
                    for (int i = 0; i < svgExclamationCircle.Children.Count; i++)
                    {
                        svgExclamationCircle.Children[i].Fill = new SvgColourServer(Color.DarkBlue);
                    }
                    iconExclamationCircle = new Bitmap(svgExclamationCircle.Draw());

                    e.Graphics.DrawImage(
                        iconExclamationCircle,
                        xBaseAlertIcon,
                        yTextBase,
                        iconSize,
                        iconSize
                    );

                    //Alert text

                    int xBaseAlertText = xBaseAlertIcon + iconSize + 6;

                    e.Graphics.DrawString(
                        $@"Alertas...",
                        componentFont,
                        alertPrimaryComponentFontBrush,
                        xBaseAlertText,
                        yTextBase
                    );
                    //Alert Close icon

                    int xBaseAlertCloseIcon = xBaseBRectangle + 6;

                    System.Drawing.Image iconX = null;
                    var svgX = SvgDocument.Open($@"{_iconPath}\x-lg.svg");
                    for (int i = 0; i < svgX.Children.Count; i++)
                    {
                        svgX.Children[i].Fill = new SvgColourServer(Color.Black);
                    }
                    iconX = new Bitmap(svgX.Draw());

                    e.Graphics.DrawImage(
                        iconX,
                        widthComponent - iconSize,
                        yTextBase,
                        iconSize,
                        iconSize
                    );
                }
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

                    #region Mover la posicion de las tabs

                    //Mover la tab a la derecha en el grupo de tabs
                    _startXButtonMoveTabRight = 0;
                    _endXButtonMoveTabRight = 0;
                    _startYButtonMoveTabRight = 0;
                    _endYButtonMoveTabRight = 0;

                    //Mover la tab a la izquierda en el grupo de tabs
                    _startXButtonMoveTabLeft = 0;
                    _endXButtonMoveTabLeft = 0;
                    _startYButtonMoveTabLeft = 0;
                    _endYButtonMoveTabLeft = 0;

                    #endregion Mover la posicion de las tabs

                    #region Controles de Mover a la derecha

                    int baseXMoveRightControl = baseXHeightControl - 26;

                    ///Width
                    _startXButtonMoveRight = baseXMoveRightControl;
                    _endXButtonMoveRight = _startXButtonMoveRight + sizeWidthIndicator;

                    _startYButtonMoveRight = e.Bounds.Y + 1;
                    _endYButtonMoveRight = _startYButtonMoveRight + (sizeHeightIndicator * 2);

                    Rectangle btnMoveRightControl = new Rectangle(
                        _startXButtonMoveRight,
                        _startYButtonMoveRight,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveRightControl);

                    System.Drawing.Image iconBtnMoveRightControl = null;
                    SvgDocument svgIconMoveRightControl = SvgDocument.Open(_pathIconMoveRight);
                    for (int i = 0; i < svgIconMoveRightControl.Children.Count; i++)
                    {
                        svgIconMoveRightControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveRightControl = new Bitmap(svgIconMoveRightControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveRightControl,
                        _startXButtonMoveRight + 2,
                        _startYButtonMoveRight + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de Mover a la derecha

                    #region Controles de Mover arriba

                    int baseXMoveUpControl = baseXMoveRightControl - 26;

                    ///Width
                    _startXButtonMoveUp = baseXMoveUpControl;
                    _endXButtonMoveUp = _startXButtonMoveUp + sizeWidthIndicator;

                    _startYButtonMoveUp = e.Bounds.Y + 1;
                    _endYButtonMoveUp = _startYButtonMoveUp + (sizeHeightIndicator * 1);

                    Rectangle btnMoveUpControl = new Rectangle(
                        _startXButtonMoveUp,
                        _startYButtonMoveUp,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 1
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveUpControl);

                    System.Drawing.Image iconBtnMoveUpControl = null;
                    SvgDocument svgIconMoveUpControl = SvgDocument.Open(_pathIconMoveUp);
                    for (int i = 0; i < svgIconMoveUpControl.Children.Count; i++)
                    {
                        svgIconMoveUpControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveUpControl = new Bitmap(svgIconMoveUpControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveUpControl,
                        _startXButtonMoveUp + 2,
                        _startYButtonMoveUp + 2,
                        sizeIconIndicator,
                        sizeIconIndicator / 2
                    );

                    #endregion Controles de Mover arriba

                    #region Controles de Mover abajo

                    int baseXMoveDownControl = baseXMoveRightControl - 26;

                    ///Width
                    _startXButtonMoveDown = baseXMoveDownControl;
                    _endXButtonMoveDown = _startXButtonMoveDown + sizeWidthIndicator;

                    _startYButtonMoveDown = _endYButtonMoveUp;
                    _endYButtonMoveDown = _startYButtonMoveDown + (sizeHeightIndicator * 2);

                    Rectangle btnMoveDownControl = new Rectangle(
                        _startXButtonMoveDown,
                        _startYButtonMoveDown,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 1
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveDownControl);

                    System.Drawing.Image iconBtnMoveDownControl = null;
                    SvgDocument svgIconMoveDownControl = SvgDocument.Open(_pathIconMoveDown);
                    for (int i = 0; i < svgIconMoveDownControl.Children.Count; i++)
                    {
                        svgIconMoveDownControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveDownControl = new Bitmap(svgIconMoveDownControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveDownControl,
                        _startXButtonMoveDown + 2,
                        _startYButtonMoveDown + 2,
                        sizeIconIndicator,
                        sizeIconIndicator / 2
                    );

                    #endregion Controles de Mover abajo

                    #region Controles de Mover a la izquierda

                    int baseXMoveLeftControl = baseXMoveDownControl - 26;

                    ///Width
                    _startXButtonMoveLeft = baseXMoveLeftControl;
                    _endXButtonMoveLeft = _startXButtonMoveLeft + sizeWidthIndicator;

                    _startYButtonMoveLeft = e.Bounds.Y + 1;
                    _endYButtonMoveLeft = _startYButtonMoveLeft + (sizeHeightIndicator * 2);

                    Rectangle btnMoveLeftControl = new Rectangle(
                        _startXButtonMoveLeft,
                        _startYButtonMoveLeft,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveLeftControl);

                    System.Drawing.Image iconBtnMoveLeftControl = null;
                    SvgDocument svgIconMoveLeftControl = SvgDocument.Open(_pathIconMoveLeft);
                    for (int i = 0; i < svgIconMoveLeftControl.Children.Count; i++)
                    {
                        svgIconMoveLeftControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveLeftControl = new Bitmap(svgIconMoveLeftControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveLeftControl,
                        _startXButtonMoveLeft + 2,
                        _startYButtonMoveLeft + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de Mover a la izquierda


                    #region Controles de aumentar ancho

                    int baseXIncreaseWidthControl = baseXMoveLeftControl - 26;

                    ///Width
                    _startXButtonIncreaseWidth = baseXIncreaseWidthControl;
                    _endXButtonIncreaseWidth = _startXButtonIncreaseWidth + sizeWidthIndicator;

                    _startYButtonIncreaseWidth = e.Bounds.Y + 1;
                    _endYButtonIncreaseWidth =
                        _startYButtonIncreaseWidth + (sizeHeightIndicator * 2);

                    Rectangle btnIncreaseWidthControl = new Rectangle(
                        _startXButtonIncreaseWidth,
                        _startYButtonIncreaseWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnIncreaseWidthControl);

                    System.Drawing.Image iconBtnIncreaseWidthControl = null;
                    SvgDocument svgIconIncreaseWidthControl = SvgDocument.Open(
                        _pathIconIncreaseWidth
                    );
                    for (int i = 0; i < svgIconIncreaseWidthControl.Children.Count; i++)
                    {
                        svgIconIncreaseWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnIncreaseWidthControl = new Bitmap(svgIconIncreaseWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnIncreaseWidthControl,
                        _startXButtonIncreaseWidth + 2,
                        _startYButtonIncreaseWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );
                    #endregion Controles de aumentar ancho

                    #region Controles de reducir ancho

                    int baseXReduceWidthControl = baseXIncreaseWidthControl - 26;

                    ///Width
                    _startXButtonReduceWidth = baseXReduceWidthControl;
                    _endXButtonReduceWidth = _startXButtonReduceWidth + sizeWidthIndicator;

                    _startYButtonReduceWidth = e.Bounds.Y + 1;
                    _endYButtonReduceWidth = _startYButtonReduceWidth + (sizeHeightIndicator * 2);

                    Rectangle btnReduceWidthControl = new Rectangle(
                        _startXButtonReduceWidth,
                        _startYButtonReduceWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnReduceWidthControl);

                    System.Drawing.Image iconBtnReduceWidthControl = null;
                    SvgDocument svgIconReduceWidthControl = SvgDocument.Open(_pathIconReduceWidth);
                    for (int i = 0; i < svgIconReduceWidthControl.Children.Count; i++)
                    {
                        svgIconReduceWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnReduceWidthControl = new Bitmap(svgIconReduceWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnReduceWidthControl,
                        _startXButtonReduceWidth + 2,
                        _startYButtonReduceWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de reducir ancho
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

                    #region Mover la posicion de las tabs

                    //Mover la tab a la derecha en el grupo de tabs
                    _startXButtonMoveTabRight = 0;
                    _endXButtonMoveTabRight = 0;
                    _startYButtonMoveTabRight = 0;
                    _endYButtonMoveTabRight = 0;

                    //Mover la tab a la izquierda en el grupo de tabs
                    _startXButtonMoveTabLeft = 0;
                    _endXButtonMoveTabLeft = 0;
                    _startYButtonMoveTabLeft = 0;
                    _endYButtonMoveTabLeft = 0;

                    #endregion Mover la posicion de las tabs

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

                    #region Control de mover a la derecha

                    ///MoveRight
                    _startXButtonMoveRight = 0;
                    _endXButtonMoveRight = 0;

                    _startYButtonMoveRight = 0;
                    _endYButtonMoveRight = 0;

                    #endregion Control de mover a la derecha

                    #region Control de mover a la izquierda

                    ///MoveLeft
                    _startXButtonMoveLeft = 0;
                    _endXButtonMoveLeft = 0;

                    _startYButtonMoveLeft = 0;
                    _endYButtonMoveLeft = 0;

                    #endregion Control de mover a la izquierda

                    #region Control de mover arriba

                    ///MoveUp
                    _startXButtonMoveUp = 0;
                    _endXButtonMoveUp = 0;

                    _startYButtonMoveUp = 0;
                    _endYButtonMoveUp = 0;

                    #endregion Control de mover arriba

                    #region Control de mover abajo

                    ///MoveDown
                    _startXButtonMoveDown = 0;
                    _endXButtonMoveDown = 0;

                    _startYButtonMoveDown = 0;
                    _endYButtonMoveDown = 0;

                    #endregion Control de mover abajo

                    #region Control de aumentar ancho

                    ///IncreaseWidth
                    _startXButtonIncreaseWidth = 0;
                    _endXButtonIncreaseWidth = 0;

                    _startYButtonIncreaseWidth = 0;
                    _endYButtonIncreaseWidth = 0;

                    #endregion Control de aumentar ancho

                    #region Control de reducir ancho

                    ///ReduceWidth
                    _startXButtonReduceWidth = 0;
                    _endXButtonReduceWidth = 0;

                    _startYButtonReduceWidth = 0;
                    _endYButtonReduceWidth = 0;

                    #endregion Control de reducir ancho
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

                    #region Mover la posicion de las tabs

                    //Mover la tab a la derecha en el grupo de tabs
                    _startXButtonMoveTabRight = 0;
                    _endXButtonMoveTabRight = 0;
                    _startYButtonMoveTabRight = 0;
                    _endYButtonMoveTabRight = 0;

                    //Mover la tab a la izquierda en el grupo de tabs
                    _startXButtonMoveTabLeft = 0;
                    _endXButtonMoveTabLeft = 0;
                    _startYButtonMoveTabLeft = 0;
                    _endYButtonMoveTabLeft = 0;

                    #endregion Mover la posicion de las tabs

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

                    #region Control de mover a la derecha

                    ///MoveRight
                    _startXButtonMoveRight = 0;
                    _endXButtonMoveRight = 0;

                    _startYButtonMoveRight = 0;
                    _endYButtonMoveRight = 0;

                    #endregion Control de mover a la derecha

                    #region Control de mover a la izquierda

                    ///MoveLeft
                    _startXButtonMoveLeft = 0;
                    _endXButtonMoveLeft = 0;

                    _startYButtonMoveLeft = 0;
                    _endYButtonMoveLeft = 0;

                    #endregion Control de mover a la izquierda

                    #region Control de mover arriba

                    ///MoveUp
                    _startXButtonMoveUp = 0;
                    _endXButtonMoveUp = 0;

                    _startYButtonMoveUp = 0;
                    _endYButtonMoveUp = 0;

                    #endregion Control de mover arriba

                    #region Control de mover abajo

                    ///MoveDown
                    _startXButtonMoveDown = 0;
                    _endXButtonMoveDown = 0;

                    _startYButtonMoveDown = 0;
                    _endYButtonMoveDown = 0;

                    #endregion Control de mover abajo

                    #region Control de aumentar ancho

                    ///IncreaseWidth
                    _startXButtonIncreaseWidth = 0;
                    _endXButtonIncreaseWidth = 0;

                    _startYButtonIncreaseWidth = 0;
                    _endYButtonIncreaseWidth = 0;

                    #endregion Control de aumentar ancho

                    #region Control de reducir ancho

                    ///ReduceWidth
                    _startXButtonReduceWidth = 0;
                    _endXButtonReduceWidth = 0;

                    _startYButtonReduceWidth = 0;
                    _endYButtonReduceWidth = 0;

                    #endregion Control de reducir ancho
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

                    #region Mover la posicion de las tabs

                    //Mover la tab a la derecha en el grupo de tabs
                    _startXButtonMoveTabRight = 0;
                    _endXButtonMoveTabRight = 0;
                    _startYButtonMoveTabRight = 0;
                    _endYButtonMoveTabRight = 0;

                    //Mover la tab a la izquierda en el grupo de tabs
                    _startXButtonMoveTabLeft = 0;
                    _endXButtonMoveTabLeft = 0;
                    _startYButtonMoveTabLeft = 0;
                    _endYButtonMoveTabLeft = 0;

                    #endregion Mover la posicion de las tabs

                    #region Controles de Mover a la derecha

                    int baseXMoveRightControl = baseXLessRowCotrol - 26;

                    ///Width
                    _startXButtonMoveRight = baseXMoveRightControl;
                    _endXButtonMoveRight = _startXButtonMoveRight + sizeWidthIndicator;

                    _startYButtonMoveRight = e.Bounds.Y + 1;
                    _endYButtonMoveRight = _startYButtonMoveRight + (sizeHeightIndicator * 2);

                    Rectangle btnMoveRightControl = new Rectangle(
                        _startXButtonMoveRight,
                        _startYButtonMoveRight,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveRightControl);

                    System.Drawing.Image iconBtnMoveRightControl = null;
                    SvgDocument svgIconMoveRightControl = SvgDocument.Open(_pathIconMoveRight);
                    for (int i = 0; i < svgIconMoveRightControl.Children.Count; i++)
                    {
                        svgIconMoveRightControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveRightControl = new Bitmap(svgIconMoveRightControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveRightControl,
                        _startXButtonMoveRight + 2,
                        _startYButtonMoveRight + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de Mover a la derecha

                    #region Controles de Mover arriba

                    int baseXMoveUpControl = baseXMoveRightControl - 26;

                    ///Width
                    _startXButtonMoveUp = baseXMoveUpControl;
                    _endXButtonMoveUp = _startXButtonMoveUp + sizeWidthIndicator;

                    _startYButtonMoveUp = e.Bounds.Y + 1;
                    _endYButtonMoveUp = _startYButtonMoveUp + (sizeHeightIndicator * 1);

                    Rectangle btnMoveUpControl = new Rectangle(
                        _startXButtonMoveUp,
                        _startYButtonMoveUp,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 1
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveUpControl);

                    System.Drawing.Image iconBtnMoveUpControl = null;
                    SvgDocument svgIconMoveUpControl = SvgDocument.Open(_pathIconMoveUp);
                    for (int i = 0; i < svgIconMoveUpControl.Children.Count; i++)
                    {
                        svgIconMoveUpControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveUpControl = new Bitmap(svgIconMoveUpControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveUpControl,
                        _startXButtonMoveUp + 2,
                        _startYButtonMoveUp + 2,
                        sizeIconIndicator,
                        sizeIconIndicator / 2
                    );

                    #endregion Controles de Mover arriba

                    #region Controles de Mover abajo

                    int baseXMoveDownControl = baseXMoveRightControl - 26;

                    ///Width
                    _startXButtonMoveDown = baseXMoveDownControl;
                    _endXButtonMoveDown = _startXButtonMoveDown + sizeWidthIndicator;

                    _startYButtonMoveDown = _endYButtonMoveUp;
                    _endYButtonMoveDown = _startYButtonMoveDown + (sizeHeightIndicator * 2);

                    Rectangle btnMoveDownControl = new Rectangle(
                        _startXButtonMoveDown,
                        _startYButtonMoveDown,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 1
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveDownControl);

                    System.Drawing.Image iconBtnMoveDownControl = null;
                    SvgDocument svgIconMoveDownControl = SvgDocument.Open(_pathIconMoveDown);
                    for (int i = 0; i < svgIconMoveDownControl.Children.Count; i++)
                    {
                        svgIconMoveDownControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveDownControl = new Bitmap(svgIconMoveDownControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveDownControl,
                        _startXButtonMoveDown + 2,
                        _startYButtonMoveDown + 2,
                        sizeIconIndicator,
                        sizeIconIndicator / 2
                    );

                    #endregion Controles de Mover abajo

                    #region Controles de Mover a la izquierda

                    int baseXMoveLeftControl = baseXMoveDownControl - 26;

                    ///Width
                    _startXButtonMoveLeft = baseXMoveLeftControl;
                    _endXButtonMoveLeft = _startXButtonMoveLeft + sizeWidthIndicator;

                    _startYButtonMoveLeft = e.Bounds.Y + 1;
                    _endYButtonMoveLeft = _startYButtonMoveLeft + (sizeHeightIndicator * 2);

                    Rectangle btnMoveLeftControl = new Rectangle(
                        _startXButtonMoveLeft,
                        _startYButtonMoveLeft,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveLeftControl);

                    System.Drawing.Image iconBtnMoveLeftControl = null;
                    SvgDocument svgIconMoveLeftControl = SvgDocument.Open(_pathIconMoveLeft);
                    for (int i = 0; i < svgIconMoveLeftControl.Children.Count; i++)
                    {
                        svgIconMoveLeftControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnMoveLeftControl = new Bitmap(svgIconMoveLeftControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnMoveLeftControl,
                        _startXButtonMoveLeft + 2,
                        _startYButtonMoveLeft + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de Mover a la izquierda

                    #region Controles de aumentar ancho

                    int baseXIncreaseWidthControl = baseXMoveLeftControl - 26;

                    ///Width
                    _startXButtonIncreaseWidth = baseXIncreaseWidthControl;
                    _endXButtonIncreaseWidth = _startXButtonIncreaseWidth + sizeWidthIndicator;

                    _startYButtonIncreaseWidth = e.Bounds.Y + 1;
                    _endYButtonIncreaseWidth =
                        _startYButtonIncreaseWidth + (sizeHeightIndicator * 2);

                    Rectangle btnIncreaseWidthControl = new Rectangle(
                        _startXButtonIncreaseWidth,
                        _startYButtonIncreaseWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnIncreaseWidthControl);

                    System.Drawing.Image iconBtnIncreaseWidthControl = null;
                    SvgDocument svgIconIncreaseWidthControl = SvgDocument.Open(
                        _pathIconIncreaseWidth
                    );
                    for (int i = 0; i < svgIconIncreaseWidthControl.Children.Count; i++)
                    {
                        svgIconIncreaseWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnIncreaseWidthControl = new Bitmap(svgIconIncreaseWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnIncreaseWidthControl,
                        _startXButtonIncreaseWidth + 2,
                        _startYButtonIncreaseWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );
                    #endregion Controles de aumentar ancho

                    #region Controles de reducir ancho

                    int baseXReduceWidthControl = baseXIncreaseWidthControl - 26;

                    ///Width
                    _startXButtonReduceWidth = baseXReduceWidthControl;
                    _endXButtonReduceWidth = _startXButtonReduceWidth + sizeWidthIndicator;

                    _startYButtonReduceWidth = e.Bounds.Y + 1;
                    _endYButtonReduceWidth = _startYButtonReduceWidth + (sizeHeightIndicator * 2);

                    Rectangle btnReduceWidthControl = new Rectangle(
                        _startXButtonReduceWidth,
                        _startYButtonReduceWidth,
                        sizeWidthIndicator,
                        sizeHeightIndicator * 2
                    );

                    //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                    e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnReduceWidthControl);

                    System.Drawing.Image iconBtnReduceWidthControl = null;
                    SvgDocument svgIconReduceWidthControl = SvgDocument.Open(_pathIconReduceWidth);
                    for (int i = 0; i < svgIconReduceWidthControl.Children.Count; i++)
                    {
                        svgIconReduceWidthControl.Children[i].Fill = new SvgColourServer(
                            baseControlsColor
                        );
                    }
                    iconBtnReduceWidthControl = new Bitmap(svgIconReduceWidthControl.Draw());

                    e.Graphics.DrawImage(
                        iconBtnReduceWidthControl,
                        _startXButtonReduceWidth + 2,
                        _startYButtonReduceWidth + 2,
                        sizeIconIndicator,
                        sizeIconIndicator
                    );

                    #endregion Controles de reducir ancho
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

                    #region Mover la posicion de las tabs

                    //Mover la tab a la derecha en el grupo de tabs
                    _startXButtonMoveTabRight = 0;
                    _endXButtonMoveTabRight = 0;
                    _startYButtonMoveTabRight = 0;
                    _endYButtonMoveTabRight = 0;

                    //Mover la tab a la izquierda en el grupo de tabs
                    _startXButtonMoveTabLeft = 0;
                    _endXButtonMoveTabLeft = 0;
                    _startYButtonMoveTabLeft = 0;
                    _endYButtonMoveTabLeft = 0;

                    #endregion Mover la posicion de las tabs

                    #region Control de mover a la derecha

                    ///MoveRight
                    _startXButtonMoveRight = 0;
                    _endXButtonMoveRight = 0;

                    _startYButtonMoveRight = 0;
                    _endYButtonMoveRight = 0;

                    #endregion Control de mover a la derecha

                    #region Control de mover a la izquierda

                    ///MoveLeft
                    _startXButtonMoveLeft = 0;
                    _endXButtonMoveLeft = 0;

                    _startYButtonMoveLeft = 0;
                    _endYButtonMoveLeft = 0;

                    #endregion Control de mover a la izquierda

                    #region Control de mover arriba

                    ///MoveUp
                    _startXButtonMoveUp = 0;
                    _endXButtonMoveUp = 0;

                    _startYButtonMoveUp = 0;
                    _endYButtonMoveUp = 0;

                    #endregion Control de mover arriba

                    #region Control de mover abajo

                    ///MoveDown
                    _startXButtonMoveDown = 0;
                    _endXButtonMoveDown = 0;

                    _startYButtonMoveDown = 0;
                    _endYButtonMoveDown = 0;

                    #endregion Control de mover abajo
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

                    #region Mover la posicion de las tabs

                    //Mover la tab a la derecha en el grupo de tabs
                    _startXButtonMoveTabRight = 0;
                    _endXButtonMoveTabRight = 0;
                    _startYButtonMoveTabRight = 0;
                    _endYButtonMoveTabRight = 0;

                    //Mover la tab a la izquierda en el grupo de tabs
                    _startXButtonMoveTabLeft = 0;
                    _endXButtonMoveTabLeft = 0;
                    _startYButtonMoveTabLeft = 0;
                    _endYButtonMoveTabLeft = 0;

                    #endregion Mover la posicion de las tabs

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

                    #region Control de mover a la derecha

                    ///MoveRight
                    _startXButtonMoveRight = 0;
                    _endXButtonMoveRight = 0;

                    _startYButtonMoveRight = 0;
                    _endYButtonMoveRight = 0;

                    #endregion Control de mover a la derecha

                    #region Control de mover a la izquierda

                    ///MoveLeft
                    _startXButtonMoveLeft = 0;
                    _endXButtonMoveLeft = 0;

                    _startYButtonMoveLeft = 0;
                    _endYButtonMoveLeft = 0;

                    #endregion Control de mover a la izquierda

                    #region Control de mover arriba

                    ///MoveUp
                    _startXButtonMoveUp = 0;
                    _endXButtonMoveUp = 0;

                    _startYButtonMoveUp = 0;
                    _endYButtonMoveUp = 0;

                    #endregion Control de mover arriba

                    #region Control de mover abajo

                    ///MoveDown
                    _startXButtonMoveDown = 0;
                    _endXButtonMoveDown = 0;

                    _startYButtonMoveDown = 0;
                    _endYButtonMoveDown = 0;

                    #endregion Control de mover abajo

                    #region Control de aumentar ancho

                    ///IncreaseWidth
                    _startXButtonIncreaseWidth = 0;
                    _endXButtonIncreaseWidth = 0;

                    _startYButtonIncreaseWidth = 0;
                    _endYButtonIncreaseWidth = 0;

                    #endregion Control de aumentar ancho

                    #region Control de reducir ancho

                    ///ReduceWidth
                    _startXButtonReduceWidth = 0;
                    _endXButtonReduceWidth = 0;

                    _startYButtonReduceWidth = 0;
                    _endYButtonReduceWidth = 0;

                    #endregion Control de reducir ancho
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

                    #region Mover la posicion de las tabs

                    //Mover la tab a la derecha en el grupo de tabs
                    _startXButtonMoveTabRight = 0;
                    _endXButtonMoveTabRight = 0;
                    _startYButtonMoveTabRight = 0;
                    _endYButtonMoveTabRight = 0;

                    //Mover la tab a la izquierda en el grupo de tabs
                    _startXButtonMoveTabLeft = 0;
                    _endXButtonMoveTabLeft = 0;
                    _startYButtonMoveTabLeft = 0;
                    _endYButtonMoveTabLeft = 0;

                    #endregion Mover la posicion de las tabs

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

                    #region Control de mover a la derecha

                    ///MoveRight
                    _startXButtonMoveRight = 0;
                    _endXButtonMoveRight = 0;

                    _startYButtonMoveRight = 0;
                    _endYButtonMoveRight = 0;

                    #endregion Control de mover a la derecha

                    #region Control de mover a la izquierda

                    ///MoveLeft
                    _startXButtonMoveLeft = 0;
                    _endXButtonMoveLeft = 0;

                    _startYButtonMoveLeft = 0;
                    _endYButtonMoveLeft = 0;

                    #endregion Control de mover a la izquierda

                    #region Control de mover arriba

                    ///MoveUp
                    _startXButtonMoveUp = 0;
                    _endXButtonMoveUp = 0;

                    _startYButtonMoveUp = 0;
                    _endYButtonMoveUp = 0;

                    #endregion Control de mover arriba

                    #region Control de mover abajo

                    ///MoveDown
                    _startXButtonMoveDown = 0;
                    _endXButtonMoveDown = 0;

                    _startYButtonMoveDown = 0;
                    _endYButtonMoveDown = 0;

                    #endregion Control de mover abajo

                    #region Control de aumentar ancho

                    ///IncreaseWidth
                    _startXButtonIncreaseWidth = 0;
                    _endXButtonIncreaseWidth = 0;

                    _startYButtonIncreaseWidth = 0;
                    _endYButtonIncreaseWidth = 0;

                    #endregion Control de aumentar ancho

                    #region Control de reducir ancho

                    ///ReduceWidth
                    _startXButtonReduceWidth = 0;
                    _endXButtonReduceWidth = 0;

                    _startYButtonReduceWidth = 0;
                    _endYButtonReduceWidth = 0;

                    #endregion Control de reducir ancho
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

            #endregion Recuperación de la tab actual

            if (controlGroup != null)
            {
                // Recuperación del componente.
                WoComponentProperties baseComponentProperties = (WoComponentProperties)
                    controlGroup.Tag;

                #region Dibujado del grupo principal

                WoColor baseBackgroundColor = new WoColor();
                WoColor baseBorderColor = new WoColor();

                if (baseComponentProperties.BackgorundColorGroup.ToString().Contains("Border"))
                {
                    baseBackgroundColor = _woThemeOptions.ColorOptions.GetValueOrDefault("Default");
                    baseBorderColor = _woThemeOptions.ColorOptions.GetValueOrDefault(
                        baseComponentProperties
                            .BackgorundColorGroup.ToString()
                            .Replace("Border", "")
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
                            componentProperties
                                .BackgorundColorGroup.ToString()
                                .Replace("Border", "")
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
                        if (
                            componentProperties.ComponentFontDecoration == eTextDecoration.Underline
                        )
                        {
                            font = new Font("Arial", fontSize, font.Style | FontStyle.Underline);
                        }
                        else if (
                            componentProperties.ComponentFontDecoration == eTextDecoration.Through
                        )
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
                            svgIconLessRow.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
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
                            svgIconRemoveTab.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
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

                        #region Controles de Mover a la derecha

                        int baseXMoveRightControl = baseXAddTab - 26;

                        ///Width
                        _startXButtonMoveRight = baseXMoveRightControl;
                        _endXButtonMoveRight = _startXButtonMoveRight + sizeWidthIndicator;

                        _startYButtonMoveRight = e.Bounds.Bottom - 37;
                        _endYButtonMoveRight = _startYButtonMoveRight + (sizeHeightIndicator * 2);

                        Rectangle btnMoveRightControl = new Rectangle(
                            _startXButtonMoveRight,
                            _startYButtonMoveRight,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveRightControl);

                        System.Drawing.Image iconBtnMoveRightControl = null;
                        SvgDocument svgIconMoveRightControl = SvgDocument.Open(_pathIconMoveRight);
                        for (int i = 0; i < svgIconMoveRightControl.Children.Count; i++)
                        {
                            svgIconMoveRightControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveRightControl = new Bitmap(svgIconMoveRightControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveRightControl,
                            _startXButtonMoveRight + 2,
                            _startYButtonMoveRight + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        #endregion Controles de Mover a la derecha

                        #region Controles de Mover arriba

                        int baseXMoveUpControl = baseXMoveRightControl - 26;

                        ///Width
                        _startXButtonMoveUp = baseXMoveUpControl;
                        _endXButtonMoveUp = _startXButtonMoveUp + sizeWidthIndicator;

                        _startYButtonMoveUp = e.Bounds.Bottom - 37;
                        _endYButtonMoveUp = _startYButtonMoveUp + (sizeHeightIndicator * 1);

                        Rectangle btnMoveUpControl = new Rectangle(
                            _startXButtonMoveUp,
                            _startYButtonMoveUp,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 1
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveUpControl);

                        System.Drawing.Image iconBtnMoveUpControl = null;
                        SvgDocument svgIconMoveUpControl = SvgDocument.Open(_pathIconMoveUp);
                        for (int i = 0; i < svgIconMoveUpControl.Children.Count; i++)
                        {
                            svgIconMoveUpControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveUpControl = new Bitmap(svgIconMoveUpControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveUpControl,
                            _startXButtonMoveUp + 2,
                            _startYButtonMoveUp + 2,
                            sizeIconIndicator,
                            sizeIconIndicator / 2
                        );

                        #endregion Controles de Mover arriba

                        #region Controles de Mover abajo

                        int baseXMoveDownControl = baseXMoveRightControl - 26;

                        ///Width
                        _startXButtonMoveDown = baseXMoveDownControl;
                        _endXButtonMoveDown = _startXButtonMoveDown + sizeWidthIndicator;

                        _startYButtonMoveDown = _endYButtonMoveUp;
                        _endYButtonMoveDown = _startYButtonMoveDown + (sizeHeightIndicator * 2);

                        Rectangle btnMoveDownControl = new Rectangle(
                            _startXButtonMoveDown,
                            _startYButtonMoveDown,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 1
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveDownControl);

                        System.Drawing.Image iconBtnMoveDownControl = null;
                        SvgDocument svgIconMoveDownControl = SvgDocument.Open(_pathIconMoveDown);
                        for (int i = 0; i < svgIconMoveDownControl.Children.Count; i++)
                        {
                            svgIconMoveDownControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveDownControl = new Bitmap(svgIconMoveDownControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveDownControl,
                            _startXButtonMoveDown + 2,
                            _startYButtonMoveDown + 2,
                            sizeIconIndicator,
                            sizeIconIndicator / 2
                        );

                        #endregion Controles de Mover abajo

                        #region Controles de Mover a la izquierda

                        int baseXMoveLeftControl = baseXMoveDownControl - 26;

                        ///Width
                        _startXButtonMoveLeft = baseXMoveLeftControl;
                        _endXButtonMoveLeft = _startXButtonMoveLeft + sizeWidthIndicator;

                        _startYButtonMoveLeft = e.Bounds.Bottom - 37;
                        _endYButtonMoveLeft = _startYButtonMoveLeft + (sizeHeightIndicator * 2);

                        Rectangle btnMoveLeftControl = new Rectangle(
                            _startXButtonMoveLeft,
                            _startYButtonMoveLeft,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveLeftControl);

                        System.Drawing.Image iconBtnMoveLeftControl = null;
                        SvgDocument svgIconMoveLeftControl = SvgDocument.Open(_pathIconMoveLeft);
                        for (int i = 0; i < svgIconMoveLeftControl.Children.Count; i++)
                        {
                            svgIconMoveLeftControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveLeftControl = new Bitmap(svgIconMoveLeftControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveLeftControl,
                            _startXButtonMoveLeft + 2,
                            _startYButtonMoveLeft + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        #endregion Controles de Mover a la izquierda

                        #region Controles de aumentar ancho

                        int baseXIncreaseWidthControl = baseXMoveLeftControl - 26;

                        ///Width
                        _startXButtonIncreaseWidth = baseXIncreaseWidthControl;
                        _endXButtonIncreaseWidth = _startXButtonIncreaseWidth + sizeWidthIndicator;

                        _startYButtonIncreaseWidth = e.Bounds.Bottom - 37;
                        _endYButtonIncreaseWidth =
                            _startYButtonIncreaseWidth + (sizeHeightIndicator * 2);

                        Rectangle btnIncreaseWidthControl = new Rectangle(
                            _startXButtonIncreaseWidth,
                            _startYButtonIncreaseWidth,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(
                            new Pen(baseControlsBrush),
                            btnIncreaseWidthControl
                        );

                        System.Drawing.Image iconBtnIncreaseWidthControl = null;
                        SvgDocument svgIconIncreaseWidthControl = SvgDocument.Open(
                            _pathIconIncreaseWidth
                        );
                        for (int i = 0; i < svgIconIncreaseWidthControl.Children.Count; i++)
                        {
                            svgIconIncreaseWidthControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnIncreaseWidthControl = new Bitmap(
                            svgIconIncreaseWidthControl.Draw()
                        );

                        e.Graphics.DrawImage(
                            iconBtnIncreaseWidthControl,
                            _startXButtonIncreaseWidth + 2,
                            _startYButtonIncreaseWidth + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );
                        #endregion Controles de aumentar ancho

                        #region Controles de reducir ancho

                        int baseXReduceWidthControl = baseXIncreaseWidthControl - 26;

                        ///Width
                        _startXButtonReduceWidth = baseXReduceWidthControl;
                        _endXButtonReduceWidth = _startXButtonReduceWidth + sizeWidthIndicator;

                        _startYButtonReduceWidth = e.Bounds.Bottom - 37;
                        _endYButtonReduceWidth =
                            _startYButtonReduceWidth + (sizeHeightIndicator * 2);

                        Rectangle btnReduceWidthControl = new Rectangle(
                            _startXButtonReduceWidth,
                            _startYButtonReduceWidth,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnReduceWidthControl);

                        System.Drawing.Image iconBtnReduceWidthControl = null;
                        SvgDocument svgIconReduceWidthControl = SvgDocument.Open(
                            _pathIconReduceWidth
                        );
                        for (int i = 0; i < svgIconReduceWidthControl.Children.Count; i++)
                        {
                            svgIconReduceWidthControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnReduceWidthControl = new Bitmap(svgIconReduceWidthControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnReduceWidthControl,
                            _startXButtonReduceWidth + 2,
                            _startYButtonReduceWidth + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        #endregion Controles de reducir ancho

                        #region Mover la posicion de las tabs

                        int baseXMoveRight = baseXReduceWidthControl - 26;

                        ///Right
                        _startXButtonMoveTabRight = baseXMoveRight;
                        _endXButtonMoveTabRight = _startXButtonMoveTabRight + sizeWidthIndicator;

                        _startYButtonMoveTabRight = e.Bounds.Bottom - 37;
                        _endYButtonMoveTabRight =
                            _startYButtonMoveTabRight + (sizeHeightIndicator * 2);

                        Rectangle btnMoveTabRight = new Rectangle(
                            _startXButtonMoveTabRight,
                            _startYButtonMoveTabRight,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveTabRight);

                        System.Drawing.Image iconBtnMoveTabRight = null;
                        SvgDocument svgIconMoveTabRight = SvgDocument.Open(_pathIconMoveTabRight);
                        for (int i = 0; i < svgIconMoveTabRight.Children.Count; i++)
                        {
                            svgIconMoveTabRight.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveTabRight = new Bitmap(svgIconMoveTabRight.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveTabRight,
                            _startXButtonMoveTabRight + 2,
                            _startYButtonMoveTabRight + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        //Mover la tab a la izquierda en el grupo de tabs
                        int baseXMoveLeft = baseXMoveRight - 26;

                        ///Left
                        _startXButtonMoveTabLeft = baseXMoveLeft;
                        _endXButtonMoveTabLeft = _startXButtonMoveTabLeft + sizeWidthIndicator;

                        _startYButtonMoveTabLeft = e.Bounds.Bottom - 37;
                        _endYButtonMoveTabLeft =
                            _startYButtonMoveTabLeft + (sizeHeightIndicator * 2);

                        Rectangle btnMoveTabLeft = new Rectangle(
                            _startXButtonMoveTabLeft,
                            _startYButtonMoveTabLeft,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveTabLeft);

                        System.Drawing.Image iconBtnMoveTabLeft = null;
                        SvgDocument svgIconMoveTabLeft = SvgDocument.Open(_pathIconMoveTabLeft);
                        for (int i = 0; i < svgIconMoveTabLeft.Children.Count; i++)
                        {
                            svgIconMoveTabLeft.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveTabLeft = new Bitmap(svgIconMoveTabLeft.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveTabLeft,
                            _startXButtonMoveTabLeft + 2,
                            _startYButtonMoveTabLeft + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        #endregion Mover la posicion de las tabs
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

                        #region Mover la posicion de las tabs

                        //Mover la tab a la derecha en el grupo de tabs
                        _startXButtonMoveTabRight = 0;
                        _endXButtonMoveTabRight = 0;
                        _startYButtonMoveTabRight = 0;
                        _endYButtonMoveTabRight = 0;

                        //Mover la tab a la izquierda en el grupo de tabs
                        _startXButtonMoveTabLeft = 0;
                        _endXButtonMoveTabLeft = 0;
                        _startYButtonMoveTabLeft = 0;
                        _endYButtonMoveTabLeft = 0;

                        #endregion Mover la posicion de las tabs

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
                                (startXSizeDinamic == 0)
                                    ? baseXMoveControls
                                    : (endXSizeDinamic + 2);
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
                                startXSizeDinamic
                                + (sizeWidthIndicator / 2)
                                - (textSizeTemp.Width / 2);
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

                        #region Control de mover a la derecha

                        ///MoveRight
                        _startXButtonMoveRight = 0;
                        _endXButtonMoveRight = 0;

                        _startYButtonMoveRight = 0;
                        _endYButtonMoveRight = 0;

                        #endregion Control de mover a la derecha

                        #region Control de mover a la izquierda

                        ///MoveLeft
                        _startXButtonMoveLeft = 0;
                        _endXButtonMoveLeft = 0;

                        _startYButtonMoveLeft = 0;
                        _endYButtonMoveLeft = 0;

                        #endregion Control de mover a la izquierda

                        #region Control de mover arriba

                        ///MoveUp
                        _startXButtonMoveUp = 0;
                        _endXButtonMoveUp = 0;

                        _startYButtonMoveUp = 0;
                        _endYButtonMoveUp = 0;

                        #endregion Control de mover arriba

                        #region Control de mover abajo

                        ///MoveDown
                        _startXButtonMoveDown = 0;
                        _endXButtonMoveDown = 0;

                        _startYButtonMoveDown = 0;
                        _endYButtonMoveDown = 0;

                        #endregion Control de mover abajo

                        #region Control de aumentar ancho

                        ///IncreaseWidth
                        _startXButtonIncreaseWidth = 0;
                        _endXButtonIncreaseWidth = 0;

                        _startYButtonIncreaseWidth = 0;
                        _endYButtonIncreaseWidth = 0;

                        #endregion Control de aumentar ancho

                        #region Control de reducir ancho

                        ///ReduceWidth
                        _startXButtonReduceWidth = 0;
                        _endXButtonReduceWidth = 0;

                        _startYButtonReduceWidth = 0;
                        _endYButtonReduceWidth = 0;

                        #endregion Control de reducir ancho
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

                        #region Mover la posicion de las tabs

                        //Mover la tab a la derecha en el grupo de tabs
                        _startXButtonMoveTabRight = 0;
                        _endXButtonMoveTabRight = 0;
                        _startYButtonMoveTabRight = 0;
                        _endYButtonMoveTabRight = 0;

                        //Mover la tab a la izquierda en el grupo de tabs
                        _startXButtonMoveTabLeft = 0;
                        _endXButtonMoveTabLeft = 0;
                        _startYButtonMoveTabLeft = 0;
                        _endYButtonMoveTabLeft = 0;

                        #endregion Mover la posicion de las tabs

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
                                (startXSizeDinamic == 0)
                                    ? baseXMoveControls
                                    : (endXSizeDinamic + 2);
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
                                startXSizeDinamic
                                + (sizeWidthIndicator / 2)
                                - (textSizeTemp.Width / 2);
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

                        #region Control de mover a la derecha

                        ///MoveRight
                        _startXButtonMoveRight = 0;
                        _endXButtonMoveRight = 0;

                        _startYButtonMoveRight = 0;
                        _endYButtonMoveRight = 0;

                        #endregion Control de mover a la derecha

                        #region Control de mover a la izquierda

                        ///MoveLeft
                        _startXButtonMoveLeft = 0;
                        _endXButtonMoveLeft = 0;

                        _startYButtonMoveLeft = 0;
                        _endYButtonMoveLeft = 0;

                        #endregion Control de mover a la izquierda

                        #region Control de mover arriba

                        ///MoveUp
                        _startXButtonMoveUp = 0;
                        _endXButtonMoveUp = 0;

                        _startYButtonMoveUp = 0;
                        _endYButtonMoveUp = 0;

                        #endregion Control de mover arriba

                        #region Control de mover abajo

                        ///MoveDown
                        _startXButtonMoveDown = 0;
                        _endXButtonMoveDown = 0;

                        _startYButtonMoveDown = 0;
                        _endYButtonMoveDown = 0;

                        #endregion Control de mover abajo

                        #region Control de aumentar ancho

                        ///IncreaseWidth
                        _startXButtonIncreaseWidth = 0;
                        _endXButtonIncreaseWidth = 0;

                        _startYButtonIncreaseWidth = 0;
                        _endYButtonIncreaseWidth = 0;

                        #endregion Control de aumentar ancho

                        #region Control de reducir ancho

                        ///ReduceWidth
                        _startXButtonReduceWidth = 0;
                        _endXButtonReduceWidth = 0;

                        _startYButtonReduceWidth = 0;
                        _endYButtonReduceWidth = 0;

                        #endregion Control de reducir ancho
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
            }

            e.Handled = true;
        }

        [SupportedOSPlatform("windows")]
        private void SlaveCustomDraw(object sender, ItemCustomDrawEventArgs e)
        {
            e.Handled = false;

            try
            {
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

                #region Dibujado de las slaves

                int fontSizeSlave = 8;
                Font fontSlave = new Font("Arial", fontSizeSlave, FontStyle.Regular);

                WoGridProperties woGrid = new WoGridProperties();
                string gridPath =
                    $@"{_project.DirLayOuts}\Grids\{componentProperties.SlaveModelId}GridList.json";
                if (File.Exists(gridPath))
                {
                    woGrid = JsonConvert.DeserializeObject<WoGridProperties>(
                        File.ReadAllText(gridPath)
                    );
                }
                else
                {
                    WoGridDesignerRawHelper woGridDesignerRawHelper = new WoGridDesignerRawHelper();
                    woGrid = woGridDesignerRawHelper.GetRawGrid(
                        componentProperties.SlaveModelId,
                        isSlave: true
                    );
                }

                List<string> columnsSlaveCol = new List<string>();

                foreach (WoColumnProperties column in woGrid.WoColumnPropertiesCol)
                {
                    if (column.IsVisible)
                    {
                        if (column.Etiqueta != null)
                        {
                            columnsSlaveCol.Add(column.MaskText);
                        }
                        else
                        {
                            Modelo model = _project
                                .ModeloCol.Modelos.Where(m => m.Id == woGrid.ModelName)
                                .FirstOrDefault();
                            var modelColumn = model
                                .Columnas.Where(x => x.Id == column.Id)
                                .FirstOrDefault();
                            columnsSlaveCol.Add(modelColumn.Grid);
                        }
                    }
                }

                int columnSize = e.Bounds.Width / woGrid.WoColumnPropertiesCol.Count;
                int xColumn = e.Bounds.X;

                foreach (string column in columnsSlaveCol)
                {
                    SizeF textSizeSlave = e.Graphics.MeasureString(column, fontSlave);
                    int ySlaveTextBase = (int)(e.Bounds.Y + (25 - textSizeSlave.Height) / 2);
                    int ySlaveIconBase = (int)(e.Bounds.Y + (25 - iconSize) / 2);

                    e.Graphics.DrawRectangle(
                        new System.Drawing.Pen(borderBrush),
                        xColumn,
                        e.Bounds.Y + 24,
                        columnSize,
                        24
                    );

                    e.Graphics.DrawRectangle(
                        new System.Drawing.Pen(borderBrush),
                        xColumn,
                        e.Bounds.Y + 48,
                        columnSize,
                        e.Bounds.Height - 48
                    );

                    e.Graphics.DrawString(
                        column,
                        fontSlave,
                        fontBrush,
                        xColumn + 2,
                        e.Bounds.Y + 36 - (textSizeSlave.Height / 2)
                    );

                    e.Graphics.DrawString(
                        "Data",
                        fontSlave,
                        fontBrush,
                        xColumn + 2,
                        e.Bounds.Y + 48 + ((e.Bounds.Height - 48) / 2) - (textSizeSlave.Height / 2)
                    );

                    xColumn += columnSize;
                }

                #endregion Dibujado de las slaves

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

                        #region Mover la posicion de las tabs

                        //Mover la tab a la derecha en el grupo de tabs
                        _startXButtonMoveTabRight = 0;
                        _endXButtonMoveTabRight = 0;
                        _startYButtonMoveTabRight = 0;
                        _endYButtonMoveTabRight = 0;

                        //Mover la tab a la izquierda en el grupo de tabs
                        _startXButtonMoveTabLeft = 0;
                        _endXButtonMoveTabLeft = 0;
                        _startYButtonMoveTabLeft = 0;
                        _endYButtonMoveTabLeft = 0;

                        #endregion Mover la posicion de las tabs

                        #region Controles de Mover a la derecha

                        int baseXMoveRightControl = baseXHeightControl - 26;

                        ///Width
                        _startXButtonMoveRight = baseXMoveRightControl;
                        _endXButtonMoveRight = _startXButtonMoveRight + sizeWidthIndicator;

                        _startYButtonMoveRight = e.Bounds.Y + 1;
                        _endYButtonMoveRight = _startYButtonMoveRight + (sizeHeightIndicator * 2);

                        Rectangle btnMoveRightControl = new Rectangle(
                            _startXButtonMoveRight,
                            _startYButtonMoveRight,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveRightControl);

                        System.Drawing.Image iconBtnMoveRightControl = null;
                        SvgDocument svgIconMoveRightControl = SvgDocument.Open(_pathIconMoveRight);
                        for (int i = 0; i < svgIconMoveRightControl.Children.Count; i++)
                        {
                            svgIconMoveRightControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveRightControl = new Bitmap(svgIconMoveRightControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveRightControl,
                            _startXButtonMoveRight + 2,
                            _startYButtonMoveRight + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        #endregion Controles de Mover a la derecha

                        #region Controles de Mover arriba

                        int baseXMoveUpControl = baseXMoveRightControl - 26;

                        ///Width
                        _startXButtonMoveUp = baseXMoveUpControl;
                        _endXButtonMoveUp = _startXButtonMoveUp + sizeWidthIndicator;

                        _startYButtonMoveUp = e.Bounds.Y + 1;
                        _endYButtonMoveUp = _startYButtonMoveUp + (sizeHeightIndicator * 1);

                        Rectangle btnMoveUpControl = new Rectangle(
                            _startXButtonMoveUp,
                            _startYButtonMoveUp,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 1
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveUpControl);

                        System.Drawing.Image iconBtnMoveUpControl = null;
                        SvgDocument svgIconMoveUpControl = SvgDocument.Open(_pathIconMoveUp);
                        for (int i = 0; i < svgIconMoveUpControl.Children.Count; i++)
                        {
                            svgIconMoveUpControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveUpControl = new Bitmap(svgIconMoveUpControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveUpControl,
                            _startXButtonMoveUp + 2,
                            _startYButtonMoveUp + 2,
                            sizeIconIndicator,
                            sizeIconIndicator / 2
                        );

                        #endregion Controles de Mover arriba

                        #region Controles de Mover abajo

                        int baseXMoveDownControl = baseXMoveRightControl - 26;

                        ///Width
                        _startXButtonMoveDown = baseXMoveDownControl;
                        _endXButtonMoveDown = _startXButtonMoveDown + sizeWidthIndicator;

                        _startYButtonMoveDown = _endYButtonMoveUp;
                        _endYButtonMoveDown = _startYButtonMoveDown + (sizeHeightIndicator * 2);

                        Rectangle btnMoveDownControl = new Rectangle(
                            _startXButtonMoveDown,
                            _startYButtonMoveDown,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 1
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveDownControl);

                        System.Drawing.Image iconBtnMoveDownControl = null;
                        SvgDocument svgIconMoveDownControl = SvgDocument.Open(_pathIconMoveDown);
                        for (int i = 0; i < svgIconMoveDownControl.Children.Count; i++)
                        {
                            svgIconMoveDownControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveDownControl = new Bitmap(svgIconMoveDownControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveDownControl,
                            _startXButtonMoveDown + 2,
                            _startYButtonMoveDown + 2,
                            sizeIconIndicator,
                            sizeIconIndicator / 2
                        );

                        #endregion Controles de Mover abajo

                        #region Controles de Mover a la izquierda

                        int baseXMoveLeftControl = baseXMoveDownControl - 26;

                        ///Width
                        _startXButtonMoveLeft = baseXMoveLeftControl;
                        _endXButtonMoveLeft = _startXButtonMoveLeft + sizeWidthIndicator;

                        _startYButtonMoveLeft = e.Bounds.Y + 1;
                        _endYButtonMoveLeft = _startYButtonMoveLeft + (sizeHeightIndicator * 2);

                        Rectangle btnMoveLeftControl = new Rectangle(
                            _startXButtonMoveLeft,
                            _startYButtonMoveLeft,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnMoveLeftControl);

                        System.Drawing.Image iconBtnMoveLeftControl = null;
                        SvgDocument svgIconMoveLeftControl = SvgDocument.Open(_pathIconMoveLeft);
                        for (int i = 0; i < svgIconMoveLeftControl.Children.Count; i++)
                        {
                            svgIconMoveLeftControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnMoveLeftControl = new Bitmap(svgIconMoveLeftControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnMoveLeftControl,
                            _startXButtonMoveLeft + 2,
                            _startYButtonMoveLeft + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        #endregion Controles de Mover a la izquierda

                        #region Controles de aumentar ancho

                        int baseXIncreaseWidthControl = baseXMoveLeftControl - 26;

                        ///Width
                        _startXButtonIncreaseWidth = baseXIncreaseWidthControl;
                        _endXButtonIncreaseWidth = _startXButtonIncreaseWidth + sizeWidthIndicator;

                        _startYButtonIncreaseWidth = e.Bounds.Y + 1;
                        _endYButtonIncreaseWidth =
                            _startYButtonIncreaseWidth + (sizeHeightIndicator * 2);

                        Rectangle btnIncreaseWidthControl = new Rectangle(
                            _startXButtonIncreaseWidth,
                            _startYButtonIncreaseWidth,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(
                            new Pen(baseControlsBrush),
                            btnIncreaseWidthControl
                        );

                        System.Drawing.Image iconBtnIncreaseWidthControl = null;
                        SvgDocument svgIconIncreaseWidthControl = SvgDocument.Open(
                            _pathIconIncreaseWidth
                        );
                        for (int i = 0; i < svgIconIncreaseWidthControl.Children.Count; i++)
                        {
                            svgIconIncreaseWidthControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnIncreaseWidthControl = new Bitmap(
                            svgIconIncreaseWidthControl.Draw()
                        );

                        e.Graphics.DrawImage(
                            iconBtnIncreaseWidthControl,
                            _startXButtonIncreaseWidth + 2,
                            _startYButtonIncreaseWidth + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );
                        #endregion Controles de aumentar ancho

                        #region Controles de reducir ancho

                        int baseXReduceWidthControl = baseXIncreaseWidthControl - 26;

                        ///Width
                        _startXButtonReduceWidth = baseXReduceWidthControl;
                        _endXButtonReduceWidth = _startXButtonReduceWidth + sizeWidthIndicator;

                        _startYButtonReduceWidth = e.Bounds.Y + 1;
                        _endYButtonReduceWidth =
                            _startYButtonReduceWidth + (sizeHeightIndicator * 2);

                        Rectangle btnReduceWidthControl = new Rectangle(
                            _startXButtonReduceWidth,
                            _startYButtonReduceWidth,
                            sizeWidthIndicator,
                            sizeHeightIndicator * 2
                        );

                        //e.Graphics.FillRectangle(baseControlsBrush, btnLabelControl);
                        e.Graphics.DrawRectangle(new Pen(baseControlsBrush), btnReduceWidthControl);

                        System.Drawing.Image iconBtnReduceWidthControl = null;
                        SvgDocument svgIconReduceWidthControl = SvgDocument.Open(
                            _pathIconReduceWidth
                        );
                        for (int i = 0; i < svgIconReduceWidthControl.Children.Count; i++)
                        {
                            svgIconReduceWidthControl.Children[i].Fill = new SvgColourServer(
                                baseControlsColor
                            );
                        }
                        iconBtnReduceWidthControl = new Bitmap(svgIconReduceWidthControl.Draw());

                        e.Graphics.DrawImage(
                            iconBtnReduceWidthControl,
                            _startXButtonReduceWidth + 2,
                            _startYButtonReduceWidth + 2,
                            sizeIconIndicator,
                            sizeIconIndicator
                        );

                        #endregion Controles de reducir ancho
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

                        #region Mover la posicion de las tabs

                        //Mover la tab a la derecha en el grupo de tabs
                        _startXButtonMoveTabRight = 0;
                        _endXButtonMoveTabRight = 0;
                        _startYButtonMoveTabRight = 0;
                        _endYButtonMoveTabRight = 0;

                        //Mover la tab a la izquierda en el grupo de tabs
                        _startXButtonMoveTabLeft = 0;
                        _endXButtonMoveTabLeft = 0;
                        _startYButtonMoveTabLeft = 0;
                        _endYButtonMoveTabLeft = 0;

                        #endregion Mover la posicion de las tabs

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
                                (startXSizeDinamic == 0)
                                    ? baseXMoveControls
                                    : (endXSizeDinamic + 2);
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
                                startXSizeDinamic
                                + (sizeWidthIndicator / 2)
                                - (textSizeTemp.Width / 2);
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

                        #region Control de mover a la derecha

                        ///MoveRight
                        _startXButtonMoveRight = 0;
                        _endXButtonMoveRight = 0;

                        _startYButtonMoveRight = 0;
                        _endYButtonMoveRight = 0;

                        #endregion Control de mover a la derecha

                        #region Control de mover a la izquierda

                        ///MoveLeft
                        _startXButtonMoveLeft = 0;
                        _endXButtonMoveLeft = 0;

                        _startYButtonMoveLeft = 0;
                        _endYButtonMoveLeft = 0;

                        #endregion Control de mover a la izquierda

                        #region Control de mover arriba

                        ///MoveUp
                        _startXButtonMoveUp = 0;
                        _endXButtonMoveUp = 0;

                        _startYButtonMoveUp = 0;
                        _endYButtonMoveUp = 0;

                        #endregion Control de mover arriba

                        #region Control de mover abajo

                        ///MoveDown
                        _startXButtonMoveDown = 0;
                        _endXButtonMoveDown = 0;

                        _startYButtonMoveDown = 0;
                        _endYButtonMoveDown = 0;

                        #endregion Control de mover abajo

                        #region Control de aumentar ancho

                        ///IncreaseWidth
                        _startXButtonIncreaseWidth = 0;
                        _endXButtonIncreaseWidth = 0;

                        _startYButtonIncreaseWidth = 0;
                        _endYButtonIncreaseWidth = 0;

                        #endregion Control de aumentar ancho

                        #region Control de reducir ancho

                        ///ReduceWidth
                        _startXButtonReduceWidth = 0;
                        _endXButtonReduceWidth = 0;

                        _startYButtonReduceWidth = 0;
                        _endYButtonReduceWidth = 0;

                        #endregion Control de reducir ancho
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

                        #region Mover la posicion de las tabs

                        //Mover la tab a la derecha en el grupo de tabs
                        _startXButtonMoveTabRight = 0;
                        _endXButtonMoveTabRight = 0;
                        _startYButtonMoveTabRight = 0;
                        _endYButtonMoveTabRight = 0;

                        //Mover la tab a la izquierda en el grupo de tabs
                        _startXButtonMoveTabLeft = 0;
                        _endXButtonMoveTabLeft = 0;
                        _startYButtonMoveTabLeft = 0;
                        _endYButtonMoveTabLeft = 0;

                        #endregion Mover la posicion de las tabs

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
                                (startXSizeDinamic == 0)
                                    ? baseXMoveControls
                                    : (endXSizeDinamic + 2);
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
                                startXSizeDinamic
                                + (sizeWidthIndicator / 2)
                                - (textSizeTemp.Width / 2);
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

                        #region Control de mover a la derecha

                        ///MoveRight
                        _startXButtonMoveRight = 0;
                        _endXButtonMoveRight = 0;

                        _startYButtonMoveRight = 0;
                        _endYButtonMoveRight = 0;

                        #endregion Control de mover a la derecha

                        #region Control de mover a la izquierda

                        ///MoveLeft
                        _startXButtonMoveLeft = 0;
                        _endXButtonMoveLeft = 0;

                        _startYButtonMoveLeft = 0;
                        _endYButtonMoveLeft = 0;

                        #endregion Control de mover a la izquierda

                        #region Control de mover arriba

                        ///MoveUp
                        _startXButtonMoveUp = 0;
                        _endXButtonMoveUp = 0;

                        _startYButtonMoveUp = 0;
                        _endYButtonMoveUp = 0;

                        #endregion Control de mover arriba

                        #region Control de mover abajo

                        ///MoveDown
                        _startXButtonMoveDown = 0;
                        _endXButtonMoveDown = 0;

                        _startYButtonMoveDown = 0;
                        _endYButtonMoveDown = 0;

                        #endregion Control de mover abajo

                        #region Control de aumentar ancho

                        ///IncreaseWidth
                        _startXButtonIncreaseWidth = 0;
                        _endXButtonIncreaseWidth = 0;

                        _startYButtonIncreaseWidth = 0;
                        _endYButtonIncreaseWidth = 0;

                        #endregion Control de aumentar ancho

                        #region Control de reducir ancho

                        ///ReduceWidth
                        _startXButtonReduceWidth = 0;
                        _endXButtonReduceWidth = 0;

                        _startYButtonReduceWidth = 0;
                        _endYButtonReduceWidth = 0;

                        #endregion Control de reducir ancho
                    }
                }

                #endregion Indicador de seleccionado
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    text: $"El modelo que se usaba como referencia, ya no existe en el proyecto",
                    caption: "Alerta",
                    buttons: System.Windows.Forms.MessageBoxButtons.OK,
                    icon: System.Windows.Forms.MessageBoxIcon.Information
                );
            }

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

        #region Mover la posicion de las tabs

        //Mover la tab a la derecha en el grupo de tabs
        private int _startXButtonMoveTabRight = 0;
        private int _endXButtonMoveTabRight = 0;
        private int _startYButtonMoveTabRight = 0;
        private int _endYButtonMoveTabRight = 0;

        private string _pathIconMoveTabRight = string.Empty;

        //Mover la tab a la izquierda en el grupo de tabs
        private int _startXButtonMoveTabLeft = 0;
        private int _endXButtonMoveTabLeft = 0;
        private int _startYButtonMoveTabLeft = 0;
        private int _endYButtonMoveTabLeft = 0;

        private string _pathIconMoveTabLeft = string.Empty;

        #endregion Mover la posicion de las tabs

        #region Controles de Mover a la derecha

        //Dimensiones del botón
        private int _startXButtonMoveRight = 0;
        private int _endXButtonMoveRight = 0;
        private int _startYButtonMoveRight = 0;
        private int _endYButtonMoveRight = 0;

        //Icono
        private string _pathIconMoveRight = string.Empty;

        #endregion Controles de Mover a la derecha

        #region Controles de Mover a la izquierda

        //Dimensiones del botón
        private int _startXButtonMoveLeft = 0;
        private int _endXButtonMoveLeft = 0;
        private int _startYButtonMoveLeft = 0;
        private int _endYButtonMoveLeft = 0;

        //Icono
        private string _pathIconMoveLeft = string.Empty;

        #endregion Controles de Mover a la izquierda

        #region Controles de Mover arriba

        //Dimensiones del botón
        private int _startXButtonMoveUp = 0;
        private int _endXButtonMoveUp = 0;
        private int _startYButtonMoveUp = 0;
        private int _endYButtonMoveUp = 0;

        //Icono
        private string _pathIconMoveUp = string.Empty;

        #endregion Controles de Moverarriba

        #region Controles de Mover arriba

        //Dimensiones del botón
        private int _startXButtonMoveDown = 0;
        private int _endXButtonMoveDown = 0;
        private int _startYButtonMoveDown = 0;
        private int _endYButtonMoveDown = 0;

        //Icono
        private string _pathIconMoveDown = string.Empty;

        #endregion Controles de Moverarriba

        #region Controles de Aumentar ancho

        //Dimensiones del botón
        private int _startXButtonIncreaseWidth = 0;
        private int _endXButtonIncreaseWidth = 0;
        private int _startYButtonIncreaseWidth = 0;
        private int _endYButtonIncreaseWidth = 0;

        //Icono
        private string _pathIconIncreaseWidth = string.Empty;

        #endregion Controles de Aumentar ancho

        #region Controles de reducir ancho

        //Dimensiones del botón
        private int _startXButtonReduceWidth = 0;
        private int _endXButtonReduceWidth = 0;
        private int _startYButtonReduceWidth = 0;
        private int _endYButtonReduceWidth = 0;

        //Icono
        private string _pathIconReduceWidth = string.Empty;

        #endregion Controles de reducir ancho


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
            _pathRowAdd = $@"{_project.DirProyectData}\LayOuts\icons\plus.svg";
            //LessRow
            _pathRowLess = $@"{_project.DirProyectData}\LayOuts\icons\dash.svg";

            //AddTab
            _pathIconAddTab = $@"{_project.DirProyectData}\LayOuts\icons\window-plus.svg";
            //RemoveTab
            _pathIconRemoveTab = $@"{_project.DirProyectData}\LayOuts\icons\window-x.svg";

            //MoveRight
            _pathIconMoveRight = $@"{_project.DirProyectData}\LayOuts\icons\arrow-right.svg";
            //MoveLeft
            _pathIconMoveLeft = $@"{_project.DirProyectData}\LayOuts\icons\arrow-left.svg";
            //MoveUp
            _pathIconMoveUp = $@"{_project.DirProyectData}\LayOuts\icons\arrow-Up-Short.svg";
            //MoveDown
            _pathIconMoveDown = $@"{_project.DirProyectData}\LayOuts\icons\arrow-Down-short.svg";

            //IncreaseWidth
            _pathIconIncreaseWidth =
                $@"{_project.DirProyectData}\LayOuts\icons\arrow-bar-right.svg";
            //ReduceWidth
            _pathIconReduceWidth = $@"{_project.DirProyectData}\LayOuts\icons\arrow-bar-left.svg";

            //Move tab right
            _pathIconMoveTabRight =
                $@"{_project.DirProyectData}\LayOuts\icons\box-arrow-in-right.svg";
            //Move tab left
            _pathIconMoveTabLeft =
                $@"{_project.DirProyectData}\LayOuts\icons\box-arrow-in-left.svg";
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
                                && _tabGroupSelected != null
                            )
                            {
                                _tabGroupSelected.OptionsTableLayoutItem.ColumnSpan = newSize.size;
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
                                && _tabGroupSelected != null
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

                #region Evento click de los botones para mover el orden de las tabs

                if (
                    e.Button == MouseButtons.Left
                    && _startXButtonMoveTabRight != 0
                    && _endXButtonMoveTabRight != 0
                    && _startYButtonMoveTabRight != 0
                    && _endYButtonMoveTabRight != 0
                    && _startXButtonMoveTabLeft != 0
                    && _endXButtonMoveTabLeft != 0
                    && _startYButtonMoveTabLeft != 0
                    && _endYButtonMoveTabLeft != 0
                )
                {
                    if (
                        e.X >= _startXButtonMoveTabRight
                        && e.X <= _endXButtonMoveTabRight
                        && e.Y >= _startYButtonMoveTabRight
                        && e.Y <= _endYButtonMoveTabRight
                    )
                    {
                        MoveTabRight();
                    }
                    else if (
                        e.X >= _startXButtonMoveTabLeft
                        && e.X <= _endXButtonMoveTabLeft
                        && e.Y >= _startYButtonMoveTabLeft
                        && e.Y <= _endYButtonMoveTabLeft
                    )
                    {
                        MoveTabLeft();
                    }
                }

                #endregion Evento click de los botones para mover el orden de las tabs

                #region Disparar el indicador de que se modifico algo

                if (EditingEvt != null)
                {
                    EditingEvt.Invoke(this, false);
                }

                #endregion Disparar el indicador de que se modifico algo

                #region Evento click del botón de Mover a la derecha

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonMoveRight
                        && e.X <= _endXButtonMoveRight
                        && e.Y >= _startYButtonMoveRight
                        && e.Y <= _endYButtonMoveRight
                    )
                    {
                        MoveRight();
                    }
                }

                #endregion Evento click del botón de Mover a la derecha

                #region Evento click del botón de Mover a la izquierda

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonMoveLeft
                        && e.X <= _endXButtonMoveLeft
                        && e.Y >= _startYButtonMoveLeft
                        && e.Y <= _endYButtonMoveLeft
                    )
                    {
                        MoveLeft();
                    }
                }

                #endregion Evento click del botón de Mover a la izquierda

                #region Evento click del botón de Mover arriba

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonMoveUp
                        && e.X <= _endXButtonMoveUp
                        && e.Y >= _startYButtonMoveUp
                        && e.Y <= _endYButtonMoveUp
                    )
                    {
                        MoveTop();
                    }
                }

                #endregion Evento click del botón de Mover arriba

                #region Evento click del botón de Mover abajo

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonMoveDown
                        && e.X <= _endXButtonMoveDown
                        && e.Y >= _startYButtonMoveDown
                        && e.Y <= _endYButtonMoveDown
                    )
                    {
                        MoveDown();
                    }
                }

                #endregion Evento click del botón de Mover abajo

                #region Evento click del botón de aumentar ancho

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonIncreaseWidth
                        && e.X <= _endXButtonIncreaseWidth
                        && e.Y >= _startYButtonIncreaseWidth
                        && e.Y <= _endYButtonIncreaseWidth
                    )
                    {
                        IncreaseWidth();
                    }
                }

                #endregion Evento click del botón de aumentar ancho

                #region Evento click del botón de disminuir ancho

                if (e.Button == MouseButtons.Left)
                {
                    if (
                        e.X >= _startXButtonReduceWidth
                        && e.X <= _endXButtonReduceWidth
                        && e.Y >= _startYButtonReduceWidth
                        && e.Y <= _endYButtonReduceWidth
                    )
                    {
                        ReduceWidth();
                    }
                }

                #endregion Evento click del botón de disminuir ancho
            }

            _layoutDesigner.Root.Update();
            _layoutDesigner.Root.Update();
        }

        #endregion Eventos de los botones en el dibujado
    }
}
