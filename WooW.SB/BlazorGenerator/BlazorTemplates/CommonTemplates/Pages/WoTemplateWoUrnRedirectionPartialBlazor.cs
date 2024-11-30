﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión del runtime: 17.0.0.0
//  
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Pages
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\CommonTemplates\Pages\WoTemplateWoUrnRedirectionPartialBlazor.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class WoTemplateWoUrnRedirectionPartialBlazor : WoTemplateWoUrnRedirectionPartialBlazorBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("\r\nusing Microsoft.AspNetCore.WebUtilities;\r\n \r\nnamespace ");
            
            #line 6 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\CommonTemplates\Pages\WoTemplateWoUrnRedirectionPartialBlazor.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project));
            
            #line default
            #line hidden
            this.Write(".Pages\r\n{\r\n\tpublic partial class WoUrnRedirection\r\n    {\r\n        /// <summary>\r\n" +
                    "        /// Parámetro con la urn recuperada a través de la URL.\r\n        /// </s" +
                    "ummary>\r\n        private string _urn = string.Empty;\r\n\r\n        /// <summary>\r\n " +
                    "       /// Parámetro con la transición a realizar.\r\n        /// </summary>\r\n    " +
                    "    private string __transition = string.Empty;\r\n\r\n        /// <summary>\r\n      " +
                    "  /// Indica si se pasa el parámetro para copiar a un nuevo registro.\r\n        /" +
                    "// </summary>\r\n        private bool __copyToNew = false;\r\n\r\n        /// <summary" +
                    ">\r\n        /// Método de inicialización de la página.\r\n        /// </summary>\r\n " +
                    "       /// <returns></returns>\r\n        protected override async Task OnInitiali" +
                    "zedAsync()\r\n        {\r\n            await base.OnInitializedAsync();\r\n\r\n         " +
                    "   /// Recuperación del parámetro de la urn desde la pagina.\r\n            Uri? u" +
                    "ri = NavigationManager?.ToAbsoluteUri(NavigationManager.Uri);\r\n\r\n            if " +
                    "(uri != null)\r\n            {\r\n                var queryStrings = QueryHelpers.Pa" +
                    "rseQuery(uri.Query);\r\n\r\n                if (queryStrings.TryGetValue(\"urn\", out " +
                    "var urn))\r\n                {\r\n                    _urn = urn;\r\n\r\n               " +
                    "     if (queryStrings.TryGetValue(\"transition\", out var transition))\r\n          " +
                    "          {\r\n                        __transition = transition;\r\n               " +
                    "     }\r\n\r\n                    if (queryStrings.TryGetValue(\"copyToNew\", out var " +
                    "copyToNew))\r\n                    {\r\n                        __copyToNew = Conver" +
                    "t.ToBoolean(copyToNew);\r\n                    }\r\n                }\r\n\r\n           " +
                    "     await MoveTo();\r\n            }\r\n            else\r\n            {\r\n          " +
                    "      await MoveTo();\r\n            }\r\n        }\r\n\r\n        /// <summary>\r\n      " +
                    "  /// Redirige a la página correspondiente en función de la información de la ur" +
                    "n.\r\n        /// </summary>\r\n        /// <returns></returns>\r\n        private asy" +
                    "nc Task MoveTo()\r\n        {\r\n            if (NavigationManager != null)\r\n       " +
                    "     {\r\n                if (_urn != string.Empty)\r\n                {\r\n          " +
                    "          string[] urnCol = _urn.Split(\':\');\r\n                    NavigationMana" +
                    "ger.NavigateTo(\"/\");\r\n\r\n                    if (urnCol.Length > 1)\r\n            " +
                    "        {\r\n                        string model = urnCol[1];\r\n                  " +
                    "      string id = urnCol[2];\r\n\r\n                        (string type, string pro" +
                    "cess)? modelData = _modelData.GetValueOrDefault(model.ToLower());\r\n\r\n           " +
                    "             if (modelData != null && modelData.Value.process != null)\r\n        " +
                    "                {\r\n                            if (__transition != string.Empty)" +
                    "\r\n                            {\r\n                                if (__copyToNew" +
                    ")\r\n                                {\r\n                                    Naviga" +
                    "tionManager.NavigateTo($\"/{modelData.Value.process}/{modelData.Value.type}/{mode" +
                    "l}?__Id={id}&__Transition={__transition}&__CopyToNew=true\");\r\n                  " +
                    "              }\r\n                                else\r\n                         " +
                    "       {\r\n                                    NavigationManager.NavigateTo($\"/{m" +
                    "odelData.Value.process}/{modelData.Value.type}/{model}?__Id={id}&__Transition={_" +
                    "_transition}\");\r\n                                }\r\n                            " +
                    "}\r\n                            else\r\n                            {\r\n            " +
                    "                    NavigationManager.NavigateTo($\"/{modelData.Value.process}/{m" +
                    "odelData.Value.type}/{model}?Id={id}\");\r\n                            }\r\n        " +
                    "                }\r\n                        else\r\n                        {\r\n    " +
                    "                        NavigationManager.NavigateTo(\"/\");\r\n                    " +
                    "    }\r\n\r\n                    }\r\n                    else\r\n                    {\r" +
                    "\n                        NavigationManager.NavigateTo(\"/\");\r\n                   " +
                    " }\r\n                }\r\n                else\r\n                {\r\n                " +
                    "    NavigationManager.NavigateTo(\"/\");\r\n                }\r\n            }\r\n      " +
                    "  }\r\n\r\n        /// <summary>\r\n        /// Diccionario con la relación de los mod" +
                    "elos con sus procesos\r\n        /// </summary>\r\n        private Dictionary<string" +
                    ", (string type, string process)> _modelData = new Dictionary<string, (string typ" +
                    "e, string process)>()\r\n        {\r\n            ");
            
            #line 124 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\CommonTemplates\Pages\WoTemplateWoUrnRedirectionPartialBlazor.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Models));
            
            #line default
            #line hidden
            this.Write("\r\n        };\r\n    }\r\n\r\n}\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 130 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\CommonTemplates\Pages\WoTemplateWoUrnRedirectionPartialBlazor.tt"

public string Project { get; set; } = "";
public string Models { get; set; } = "";

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class WoTemplateWoUrnRedirectionPartialBlazorBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        public System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}