﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace WooW.SB.BlazorGenerator.BlazorTemplates.CommonTemplates.Shared
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\CommonTemplates\Shared\WoTemplateMainLayoutCssBlazor.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class WoTemplateMainLayoutCssBlazor : WoTemplateMainLayoutCssBlazorBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("///------------------------------------------------------------------------------" +
                    "------------------------------------\r\n/// Código autogenerado por la template Wo" +
                    "TemplateGenericClass en el path WooW.SB\\BlazorGenerator\\BlazorTemplates\\CommonTe" +
                    "mplates\\Shared\\WoTemplateMainLayoutCssBlazor.tt\r\n/// Genera código consistente c" +
                    "on la version de la prueba de concepto a dia 05-10-2023 En Blazor Wasm y Server\r" +
                    "\n/// Este código es auto generado y su modificación puede causar que el código n" +
                    "o se comporte como deveria ademas de\r\n/// que se perderan los cambios realizados" +
                    " en el código al momento de la generación.\r\n///---------------------------------" +
                    "--------------------------------------------------------------------------------" +
                    "-\r\n\r\n.page {\r\n    position: relative;\r\n    display: flex;\r\n    flex-direction: c" +
                    "olumn;\r\n}\r\n\r\nmain {\r\n    flex: 1;\r\n}\r\n\r\n.sidebar {\r\n    width: auto;\r\n}\r\n\r\n.top-" +
                    "row {\r\n    background-color: #f7f7f7;\r\n    border-bottom: 1px solid #d6d5d5;\r\n  " +
                    "  justify-content: flex-end;\r\n    height: 3.5rem;\r\n    display: flex;\r\n    align" +
                    "-items: center;\r\n}\r\n\r\n    .top-row ::deep a, .top-row ::deep .btn-link {\r\n      " +
                    "  white-space: nowrap;\r\n        margin-left: 1.5rem;\r\n        text-decoration: n" +
                    "one;\r\n    }\r\n\r\n    .top-row ::deep a:hover, .top-row ::deep .btn-link:hover {\r\n " +
                    "       text-decoration: underline;\r\n    }\r\n\r\n    .top-row ::deep a:first-child {" +
                    "\r\n        overflow: hidden;\r\n        text-overflow: ellipsis;\r\n    }\r\n\r\n@media (" +
                    "max-width: 640.98px) {\r\n    .top-row:not(.auth) {\r\n        display: none;\r\n    }" +
                    "\r\n\r\n    .top-row.auth {\r\n        justify-content: space-between;\r\n    }\r\n\r\n    ." +
                    "top-row ::deep a, .top-row ::deep .btn-link {\r\n        margin-left: 0;\r\n    }\r\n}" +
                    "\r\n\r\n@media (min-width: 641px) {\r\n    .page {\r\n        flex-direction: row;\r\n    " +
                    "}\r\n\r\n    .sidebar {\r\n        width: auto;\r\n        height: 100vh;\r\n        posit" +
                    "ion: sticky;\r\n        top: 0;\r\n    }\r\n\r\n    .top-row {\r\n        position: sticky" +
                    ";\r\n        top: 0;\r\n        z-index: 1;\r\n    }\r\n\r\n    .top-row.auth ::deep a:fir" +
                    "st-child {\r\n        flex: 1;\r\n        text-align: right;\r\n        width: 0;\r\n   " +
                    " }\r\n\r\n    .top-row, article {\r\n        padding-left: 2rem !important;\r\n        p" +
                    "adding-right: 1.5rem !important;\r\n    }\r\n}\r\n\r\nfooter {\r\n    position: fixed;\r\n  " +
                    "  bottom: 0;\r\n    right: 0;\r\n    width: 35%;\r\n    height: auto;\r\n    background-" +
                    "color: Background;\r\n    text-align: center;\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class WoTemplateMainLayoutCssBlazorBase
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