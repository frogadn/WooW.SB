using System;
using System.Drawing;
using System.Linq;
using ExCSS;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.ManagerDirectory;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Themes.ThemeHelpers
{
    public class WoThemeAnalizer
    {
        #region Instancias principales del proyecto

        private Proyecto _project = Proyecto.getInstance();

        private WoLogObserver _log = WoLogObserver.GetInstance();

        #endregion Instancias principales del proyecto

        #region Atributos

        private string _pathThemes = string.Empty;

        private WoThemeOptions _woThemeOption = WoThemeOptions.GetInstance();

        private string _rawCss = string.Empty;
        private Stylesheet _stylesheet = null;
        private string _themeSelected = "Default";

        #endregion Atributos

        #region Constructor

        public WoThemeAnalizer()
        {
            _pathThemes = $@"{_project.DirProyectData}/LayOuts/Themes";
        }

        #endregion Constructor

        #region Análisis y carga del las settings del bootstrap

        public void ReadCss(string themeSelected = "Default")
        {
            _woThemeOption.ColorOptions.Clear();

            _themeSelected = themeSelected;
            if (
                _themeSelected == null
                || _themeSelected == string.Empty
                || _themeSelected == "Seleccione..."
            )
                _themeSelected = "Default";

            _rawCss = WoDirectory.ReadFile($@"{_pathThemes}/{_themeSelected}.css");

            StylesheetParser parser = new StylesheetParser();
            _stylesheet = parser.Parse(_rawCss);

            var rootStyle = _stylesheet
                .StyleRules.Where(x => x.SelectorText == ":root")
                .FirstOrDefault();

            if (rootStyle != null)
            {
                string rawRootStyle = rootStyle.StylesheetText.Text;

                AddToDictionary("FontDefault", "--bs-body-color:", rawRootStyle);

                AddToDictionary("Default", "--bs-body-bg:", rawRootStyle);

                AddToDictionary("Border", "--bs-border-color:", rawRootStyle);

                AddToDictionary("Background", "--bs-body-bg:", rawRootStyle);

                AddToDictionary("Primary", "--bs-primary:", rawRootStyle);

                AddToDictionary("Secondary", "--bs-secondary:", rawRootStyle);

                AddToDictionary("Success", "--bs-success:", rawRootStyle);

                AddToDictionary("Info", "--bs-info:", rawRootStyle);

                AddToDictionary("Warning", "--bs-warning:", rawRootStyle);

                AddToDictionary("Danger", "--bs-danger:", rawRootStyle);

                AddToDictionary("Light", "--bs-light:", rawRootStyle);

                AddToDictionary("Dark", "--bs-dark:", rawRootStyle);

                AddToDictionary("Muted", "--bs-gray:", rawRootStyle);

                AddToDictionary("Black", "--bs-black:", rawRootStyle);

                AddToDictionary("White", "--bs-white:", rawRootStyle);
            }

            var bodyStyle = _stylesheet.StyleRules.Where(x => x.SelectorText == "body");

            if (bodyStyle != null && bodyStyle.Count() > 0)
            {
                foreach (var rule in bodyStyle)
                {
                    if (
                        rule.Text.Contains("background-image")
                        && rule.Text.Contains("linear-gradient")
                    )
                    {
                        AddToDictionary("Background", "background-image:", rule.Text);
                    }
                }
            }
        }

        #endregion Análisis y carga del las settings del bootstrap

        #region Analizador de color

        private void AddToDictionary(string key, string cssClass, string rule)
        {
            WoColor lastColor = new WoColor();

            string property = rule.Split(cssClass)[1]
                .Split(";")[0]
                .Replace(" ", "")
                .Replace(")", "")
                .Replace("}", "");

            if (property.Contains("linear-gradient("))
            {
                WoColor color = new WoColor();

                string gradient = property.Split("linear-gradient(")[1];
                if (gradient.Contains("rgb("))
                {
                    string[] gradientCol = gradient.Split(",rgb(");
                    if (gradientCol.Length == 4 && gradientCol.Contains("deg"))
                    {
                        float angle = float.Parse(gradientCol[0].ToString().Replace("deg", "")) * 2;
                        string[] gradientColor1Col = gradientCol[1].Replace("rgb(", "").Split(",");
                        string[] gradientColor2Col = gradientCol[2].Split(",");
                        string[] gradientColor3Col = gradientCol[3].Split(",");
                        color = new WoColor()
                        {
                            ColorType = eWoColorType.Gradient,
                            GradientColor1 = System.Drawing.Color.FromArgb(
                                255,
                                int.Parse(gradientColor1Col[0]),
                                int.Parse(gradientColor1Col[1]),
                                int.Parse(gradientColor1Col[2])
                            ),
                            GradientColor2 = System.Drawing.Color.FromArgb(
                                255,
                                int.Parse(gradientColor2Col[0]),
                                int.Parse(gradientColor2Col[1]),
                                int.Parse(gradientColor2Col[2])
                            ),
                            GradientColor3 = System.Drawing.Color.FromArgb(
                                255,
                                int.Parse(gradientColor3Col[0]),
                                int.Parse(gradientColor3Col[1]),
                                int.Parse(gradientColor3Col[2])
                            ),
                            GradientAngle = angle
                        };

                        if (_woThemeOption.ColorOptions.TryGetValue(key, out lastColor))
                        {
                            _woThemeOption.ColorOptions.Remove(key);
                        }
                    }
                }

                if (!_woThemeOption.ColorOptions.TryGetValue(key, out lastColor))
                {
                    _woThemeOption.ColorOptions.Add(key, color);
                }
            }
            else if (property.Contains("rgba("))
            {
                double alpha = 255;

                System.Drawing.Color rgbaColor = System.Drawing.Color.White;
                string[] primaryCol = property
                    .Replace("rgba(", "")
                    .Replace(")", "")
                    .Replace(" ", "")
                    .Split(",");

                alpha = ((255 / 100) * double.Parse(primaryCol[3])) * 100;

                rgbaColor = System.Drawing.Color.FromArgb(
                    (int)alpha,
                    int.Parse(primaryCol[0]),
                    int.Parse(primaryCol[1]),
                    int.Parse(primaryCol[2])
                );

                if (!_woThemeOption.ColorOptions.TryGetValue(key, out lastColor))
                {
                    _woThemeOption.ColorOptions.Add(
                        key,
                        new WoColor() { ColorType = eWoColorType.Solid, SolidColor = rgbaColor }
                    );
                }
            }
            else if (property.Contains("#"))
            {
                System.Drawing.Color solidColor = ColorTranslator.FromHtml(property);

                if (!_woThemeOption.ColorOptions.TryGetValue(key, out lastColor))
                {
                    _woThemeOption.ColorOptions.Add(
                        key,
                        new WoColor() { ColorType = eWoColorType.Solid, SolidColor = solidColor }
                    );
                }
            }
        }

        #endregion Analizador de color
    }
}
