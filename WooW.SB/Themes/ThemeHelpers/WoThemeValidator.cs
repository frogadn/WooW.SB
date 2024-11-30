using System.Collections.Generic;
using System.Linq;
using ExCSS;

namespace WooW.SB.Themes.ThemeHelpers
{
    public class WoThemeValidator
    {
        private bool _correctFormat = true;

        private List<string> colorRootVariables = new List<string>()
        {
            "--bs-blue",
            "--bs-indigo",
            "--bs-purple",
            "--bs-pink",
            "--bs-red",
            "--bs-orange",
            "--bs-yellow",
            "--bs-green",
            "--bs-teal",
            "--bs-cyan",
            "--bs-black",
            "--bs-white",
            "--bs-gray",
            "--bs-gray-dark",
            "--bs-gray-100",
            "--bs-gray-200",
            "--bs-gray-300",
            "--bs-gray-400",
            "--bs-gray-500",
            "--bs-gray-600",
            "--bs-gray-700",
            "--bs-gray-800",
            "--bs-gray-900",
            "--bs-primary",
            "--bs-secondary",
            "--bs-success",
            "--bs-info",
            "--bs-warning",
            "--bs-danger",
            "--bs-light",
            "--bs-dark",
            "--bs-primary-rgb",
            "--bs-secondary-rgb",
            "--bs-success-rgb",
            "--bs-info-rgb",
            "--bs-warning-rgb",
            "--bs-danger-rgb",
            "--bs-light-rgb",
            "--bs-dark-rgb",
            "--bs-white-rgb",
            "--bs-black-rgb",
            "--bs-body-color-rgb",
            "--bs-body-bg-rgb",
            "--bs-font-sans-serif",
            "--bs-font-monospace",
            "--bs-gradient",
            "--bs-body-font-family",
            "--bs-body-font-size",
            "--bs-body-font-weight",
            "--bs-body-line-height",
            "--bs-body-color",
            "--bs-body-bg",
            "--bs-border-width",
            "--bs-border-color",
            "--bs-border-color-translucent",
            "--bs-border-radius",
            "--bs-border-radius-sm",
            "--bs-border-radius-lg",
            "--bs-border-radius-xl",
            "--bs-border-radius-2xl",
            "--bs-border-radius-pill",
            "--bs-link-color",
            "--bs-link-hover-color",
            "--bs-code-color",
            "--bs-highlight-bg"
        };

        private List<string> cssComponentClass = new List<string>()
        {
            ":root",
            ".bg-primary",
            ".bg-secondary",
            ".bg-success",
            ".bg-danger",
            ".bg-warning",
            ".bg-info",
            ".bg-light",
            ".bg-dark",
            ".border-primary",
            ".border-secondary",
            ".border-success",
            ".border-danger",
            ".border-warning",
            ".border-info",
            ".border-light",
            ".border-dark",
            ".btn-primary",
            ".btn-secondary",
            ".btn-success",
            ".btn-info",
            ".btn-warning",
            ".btn-danger",
            ".btn-light",
            ".btn-dark",
            ".btn-link",
            ".btn-outline-primary",
            ".btn-outline-secondary",
            ".btn-outline-success",
            ".btn-outline-info",
            ".btn-outline-warning",
            ".btn-outline-danger",
            ".btn-outline-light",
            ".btn-outline-dark",
            ".text-primary",
            ".text-secondary",
            ".text-warning",
            ".text-danger",
            ".text-success",
            ".text-info",
            ".text-muted",
            ".table-primary",
            ".table-secondary",
            ".table-warning",
            ".table-danger",
            ".table-success",
            ".table-info",
            ".table-dark"
        };

        private Stylesheet _stylesheet = null;

        public bool VerifyBoostrap(string file)
        {
            StylesheetParser stylesheetParser = new StylesheetParser();
            _stylesheet = stylesheetParser.Parse(file);

            _correctFormat = ValidateClass();
            if (_correctFormat)
            {
                var findClass = _stylesheet
                    .StyleRules.Where(x => x.SelectorText == ":root")
                    .FirstOrDefault();
                _correctFormat = ValidateVariables(findClass.StylesheetText.Text);
            }

            return _correctFormat;
        }

        private bool ValidateClass()
        {
            bool correctClass = true;

            foreach (var cssClass in cssComponentClass)
            {
                var findClass = _stylesheet
                    .StyleRules.Where(x => x.SelectorText == cssClass)
                    .FirstOrDefault();
                if (findClass == null)
                {
                    correctClass = false;
                    break;
                }
            }

            return correctClass;
        }

        private bool ValidateVariables(string cssClass)
        {
            bool correctClass = true;

            foreach (var cssVariable in colorRootVariables)
            {
                if (!cssClass.Contains(cssVariable))
                {
                    correctClass = false;
                    break;
                }
            }

            return correctClass;
        }
    }
}
