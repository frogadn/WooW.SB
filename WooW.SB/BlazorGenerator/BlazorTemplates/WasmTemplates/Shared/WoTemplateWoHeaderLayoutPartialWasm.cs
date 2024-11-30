﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión del runtime: 17.0.0.0
//  
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace WooW.SB.BlazorGenerator.BlazorTemplates.WasmTemplates.Shared
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\WasmTemplates\Shared\WoTemplateWoHeaderLayoutPartialWasm.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class WoTemplateWoHeaderLayoutPartialWasm : WoTemplateWoHeaderLayoutPartialWasmBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write(@"
///------------------------------------------------------------------------------------------------------------------
/// Código autogenerado por la template WoTemplateGenericClass en el path WooW.SB\BlazorGenerator\BlazorTemplates\CommonTemplates\Shared\WoTemplateMainLayoutPartialBlazor.tt
/// Genera código consistente con la version de la prueba de concepto a dia 05-10-2023 En Blazor Wasm y Server
/// Este código es auto generado y su modificación puede causar que el código no se comporte como deveria ademas de
/// que se perderan los cambios realizados en el código al momento de la generación.
///------------------------------------------------------------------------------------------------------------------

using ");
            
            #line 11 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\WasmTemplates\Shared\WoTemplateWoHeaderLayoutPartialWasm.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project));
            
            #line default
            #line hidden
            this.Write(".Auth;\r\nusing Microsoft.AspNetCore.Components;\r\nusing Newtonsoft.Json;\r\nusing Ser" +
                    "viceStack;\r\nusing WooW.Model;\r\nusing WooW.Core;\r\nusing DevExpress.CodeParser;\r\nu" +
                    "sing System.Globalization;\r\nusing ");
            
            #line 19 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\WasmTemplates\Shared\WoTemplateWoHeaderLayoutPartialWasm.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project));
            
            #line default
            #line hidden
            this.Write(".Localizer;\r\nusing System.Net.Http.Json;\r\nusing WooW.Blazor;\r\nusing WooW.Blazor.S" +
                    "ervices;\r\n\r\nnamespace ");
            
            #line 24 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\WasmTemplates\Shared\WoTemplateWoHeaderLayoutPartialWasm.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project));
            
            #line default
            #line hidden
            this.Write(".Shared\r\n{\r\n    public partial class WoHeaderLayout : AWoComponentBase\r\n    {\r\n  " +
                    "      #region Cliente\r\n\r\n        /// <summary>\r\n        /// Inicializa el client" +
                    "e.\r\n        /// </summary>\r\n        /// <returns></returns>\r\n        private asy" +
                    "nc Task LoadClient()\r\n        {\r\n            try\r\n            {\r\n               " +
                    " Client = await GetClientAsync();\r\n\r\n                if (Client != null)\r\n      " +
                    "          {\r\n                    _reqRol = new WoRequestService<WoRolePermission" +
                    "Req, WoRolePermissionRes>(Client);\r\n                    _reqMenu = new WoRequest" +
                    "Service<WoMenusReq, WoMenusRes>(Client);\r\n                }\r\n            }\r\n    " +
                    "        catch (Exception ex)\r\n            {\r\n                /// error al cargar" +
                    " el cliente\r\n            }\r\n        }\r\n\r\n        #endregion Cliente\r\n\r\n        #" +
                    "region Ciclo de vida\r\n\r\n        /// <summary>\r\n        /// Método que se ejecuta" +
                    " al inicializar el componente.\r\n        /// </summary>\r\n        /// <param name=" +
                    "\"firstRender\"></param>\r\n        /// <returns></returns>\r\n        protected overr" +
                    "ide async Task OnAfterRenderAsync(bool firstRender)\r\n        {\r\n            awai" +
                    "t base.OnAfterRenderAsync(firstRender);\r\n\r\n            await LoadClient();\r\n\r\n  " +
                    "          try\r\n            {\r\n                await LoadCultures();\r\n           " +
                    " }\r\n            catch (Exception ex)\r\n            {\r\n                /// error a" +
                    "l cargar las culturas\r\n            }\r\n\r\n            try\r\n            {\r\n        " +
                    "        if (AuthStateProvider != null)\r\n                {\r\n                    A" +
                    "uthResult = await AuthStateProvider.LocalStorage.GetItemAsync<AuthenticateRespon" +
                    "se>(\"A\");\r\n                    UdnResult = await AuthStateProvider.LocalStorage." +
                    "GetItemAsync<WoInstanciaUdnResponse>(\"U\");\r\n                    SessionData = aw" +
                    "ait AuthStateProvider.LocalStorage.GetItemAsync<SessionData>(\"S\");\r\n            " +
                    "    }\r\n            }\r\n            catch (Exception ex)\r\n            {\r\n         " +
                    "       /// error al recuperar la data de la autenticación\r\n            }\r\n\r\n    " +
                    "        if (firstRender)\r\n            {\r\n                try\r\n                {\r" +
                    "\n                    _selectedRol = await LocalStorage.GetItemAsync<string>(\"Rol" +
                    "\");\r\n                    _roles = await LocalStorage.GetItemAsync<List<string>>(" +
                    "\"Roles\") ?? new List<string>();\r\n\r\n                    if (_selectedRol != null)" +
                    "\r\n                    {\r\n                        await SelectRol(_selectedRol);\r" +
                    "\n\r\n                        _selectedMenu = await LocalStorage.GetItemAsync<strin" +
                    "g>(\"Menu\");\r\n\r\n                        if (!_selectedMenu.IsNullOrEmpty())\r\n    " +
                    "                    {\r\n                            await SelectMenu(_selectedMen" +
                    "u);\r\n                        }\r\n                    }\r\n                }\r\n      " +
                    "          catch (Exception ex)\r\n                {\r\n                    /// error" +
                    " al recuperar el rol del local storage\r\n                }\r\n            }\r\n      " +
                    "  }\r\n\r\n        #endregion Ciclo de vida\r\n\r\n        #region Roles\r\n\r\n        /// " +
                    "<summary>\r\n        /// Lista de los roles del usuario.\r\n        /// </summary>\r\n" +
                    "        private List<string> _roles = new List<string>();\r\n\r\n        /// <summar" +
                    "y>\r\n        /// Rol seleccionado del cual se recuperaran los menus.\r\n        ///" +
                    " </summary>\r\n        private string? _selectedRol;\r\n\r\n        /// <summary>\r\n   " +
                    "     /// Servicio que permite realizar peticiones de los roles.\r\n        /// </s" +
                    "ummary>\r\n        private WoRequestService<WoRolePermissionReq, WoRolePermissionR" +
                    "es>? _reqRol;\r\n\r\n        /// <summary>\r\n        /// Evento que se detona al dar " +
                    "click en el dropdown de roles.\r\n        /// </summary>\r\n        /// <returns></r" +
                    "eturns>\r\n        private async Task RoleDropDownClick()\r\n        {\r\n            " +
                    "await LoadRoles();\r\n        }\r\n\r\n        /// <summary>\r\n        /// Carga el cli" +
                    "ente y la lista de roles.\r\n        /// </summary>\r\n        /// <returns></return" +
                    "s>\r\n        private async Task LoadRoles()\r\n        {\r\n            try\r\n        " +
                    "    {\r\n                if (_reqRol != null)\r\n                {\r\n                " +
                    "    IList<WoRolePermissionRes> result = await _reqRol.GetListResponse();\r\n      " +
                    "              if (result != null)\r\n                    {\r\n                      " +
                    "  _roles.Clear();\r\n\r\n                        foreach (WoRolePermissionRes role i" +
                    "n result)\r\n                        {\r\n                            _roles.AddRang" +
                    "e(role.RoleCol);\r\n                        }\r\n\r\n                        await Loc" +
                    "alStorage.SetItemAsync(\"Roles\", _roles);\r\n                    }\r\n\r\n             " +
                    "       StateHasChanged();\r\n                }\r\n            }\r\n            catch (" +
                    "Exception ex)\r\n            {\r\n                /// Fallo la recuperacion de los r" +
                    "oles\r\n            }\r\n        }\r\n\r\n        #region Seleccion de rol\r\n\r\n        //" +
                    "/ <summary>\r\n        /// Selecciona el rol y carga los menus.\r\n        /// </sum" +
                    "mary>\r\n        /// <param name=\"newRol\"></param>\r\n        /// <returns></returns" +
                    ">\r\n        private async Task SelectRol(string newRol)\r\n        {\r\n            _" +
                    "menusCol.Clear();\r\n            _menus.Clear();\r\n            _rawMenusCol.Clear()" +
                    ";\r\n            _selectedRol = newRol;\r\n\r\n            await LoadMenus();\r\n\r\n     " +
                    "       await LocalStorage.SetItemAsync(\"Rol\", newRol);\r\n        }\r\n\r\n        #en" +
                    "dregion Seleccion de rol\r\n\r\n        #endregion Roles\r\n\r\n        #region Menus\r\n\r" +
                    "\n        /// <summary>\r\n        /// Nombre del menu seleccionado.\r\n        /// <" +
                    "/summary>\r\n        private string? _selectedMenu;\r\n\r\n        /// <summary>\r\n    " +
                    "    /// Lista de los nombres de los menus que se muestran en el select.\r\n       " +
                    " /// </summary>\r\n        private List<string> _menus = new List<string>();\r\n\r\n  " +
                    "      /// <summary>\r\n        /// Lista de los menus, son los json completos del " +
                    "menu.\r\n        /// </summary>\r\n        private List<string> _rawMenusCol = new L" +
                    "ist<string>();\r\n\r\n        /// <summary>\r\n        /// Lista de los menus deserial" +
                    "izados a WoMenuProperties.\r\n        /// </summary>\r\n        private List<WoMenuP" +
                    "roperties> _menusCol = new List<WoMenuProperties>();\r\n\r\n        /// <summary>\r\n " +
                    "       /// Servicio que permite la petición de los menus.\r\n        /// </summary" +
                    ">\r\n        private WoRequestService<WoMenusReq, WoMenusRes>? _reqMenu;\r\n\r\n      " +
                    "  /// <summary>\r\n        /// Carga los menus.\r\n        /// </summary>\r\n        /" +
                    "// <returns></returns>\r\n        private async Task LoadMenus()\r\n        {\r\n     " +
                    "       try\r\n            {\r\n                if (_reqMenu != null)\r\n              " +
                    "  {\r\n                    IList<WoMenusRes> resultMenusCol = await _reqMenu.GetLi" +
                    "stResponse();\r\n\r\n                    _menusCol.Clear();\r\n                    _me" +
                    "nus.Clear();\r\n\r\n                    foreach (var item in resultMenusCol)\r\n      " +
                    "              {\r\n                        if (item.Rol == _selectedRol)\r\n        " +
                    "                {\r\n                            string menu = item.Menu ?? string" +
                    ".Empty;\r\n                            if (menu != string.Empty)\r\n                " +
                    "            {\r\n                                WoMenuProperties? woContainer = J" +
                    "sonConvert.DeserializeObject<WoMenuProperties>(menu);\r\n                         " +
                    "       if (woContainer != null)\r\n                                {\r\n            " +
                    "                        _menusCol.Add(woContainer);\r\n                           " +
                    "         _menus.Add(woContainer.Id);\r\n                                }\r\n       " +
                    "                     }\r\n                        }\r\n                    }\r\n      " +
                    "          }\r\n            }\r\n            catch (Exception ex)\r\n            {\r\n   " +
                    "             /// error al consultar los menus\r\n            }\r\n        }\r\n\r\n     " +
                    "   #region Seleccion del menu\r\n\r\n        /// <summary>\r\n        /// Evento que s" +
                    "e detona al seleccionar un menu\r\n        /// </summary>\r\n        /// <param name" +
                    "=\"newMenu\"></param>\r\n        /// <returns></returns>\r\n        private async Task" +
                    " SelectMenu(string newMenu)\r\n        {\r\n            _selectedMenu = newMenu;\r\n  " +
                    "          WoMenuProperties? woContainer = _menusCol.Where(x => x.Id == newMenu)." +
                    "FirstOrDefault();\r\n\r\n            if (woContainer != null)\r\n            {\r\n      " +
                    "          _rawMenusCol.Clear();\r\n                _rawMenusCol.Add(JsonConvert.Se" +
                    "rializeObject(woContainer));\r\n            }\r\n            else\r\n            {\r\n  " +
                    "              _rawMenusCol.Clear();\r\n                _rawMenusCol.Add(JsonConver" +
                    "t.SerializeObject(new WoMenuProperties()));\r\n            }\r\n\r\n            await " +
                    "LocalStorage.SetItemAsync(\"Menu\", newMenu);\r\n\r\n            StateHasChanged();\r\n " +
                    "       }\r\n\r\n        #endregion Seleccion del menu\r\n\r\n        #endregion Menus\r\n\r" +
                    "\n        #region Selector de culturas\r\n\r\n        /// <summary>\r\n        /// Indi" +
                    "ca el nombre de la region seleccionada.\r\n        /// </summary>\r\n        private" +
                    " string _selectedCulture = CultureInfo.CurrentCulture.Name;\r\n\r\n        /// <summ" +
                    "ary>\r\n        /// lista de las posibles culturas\r\n        /// </summary>\r\n      " +
                    "  private List<string> _cultures = new List<string>();\r\n\r\n        /// <summary>\r" +
                    "\n        /// Carga la cultura la iniciar desde el local storage\r\n        /// </s" +
                    "ummary>\r\n        /// <returns></returns>\r\n        private async Task LoadCulture" +
                    "s()\r\n        {\r\n            string cultureStorage = await LocalStorage.GetItemAs" +
                    "ync<string>(\"culture\") ?? LocalizerSettings.NeutralCulture.Culture;\r\n           " +
                    " _cultures.Clear();\r\n            foreach (var culture in LocalizerSettings.Suppo" +
                    "rtedCulturesWithName)\r\n            {\r\n                _cultures.Add(culture.Cult" +
                    "ure);\r\n            }\r\n            _selectedCulture = cultureStorage;\r\n        }\r" +
                    "\n\r\n        /// <summary>\r\n        /// Cambia la cultura seleccionada\r\n        //" +
                    "/ </summary>\r\n        /// <param name=\"newCulture\"></param>\r\n        private asy" +
                    "nc void ChangeCulture(string newCulture)\r\n        {\r\n            var uri = new U" +
                    "ri(Navigation.Uri)\r\n                        .GetComponents(UriComponents.PathAnd" +
                    "Query, UriFormat.Unescaped);\r\n            var cultureEscaped = Uri.EscapeDataStr" +
                    "ing(newCulture);\r\n            var uriEscaped = Uri.EscapeDataString(uri);\r\n\r\n   " +
                    "         await LocalStorage.SetItemAsStringAsync(\"culture\", newCulture);\r\n\r\n    " +
                    "        Navigation.NavigateTo(\r\n                $\"Culture/Set?culture={cultureEs" +
                    "caped}&redirectUri={uriEscaped}\",\r\n                forceLoad: true);\r\n        }\r" +
                    "\n\r\n        #endregion Selector de culturas\r\n\r\n        #region LogOut\r\n\r\n        " +
                    "/// <summary>\r\n        /// Evento que se detona al dar click en el boton de logo" +
                    "ut.\r\n        /// </summary>\r\n        private async Task LogOut_Click()\r\n        " +
                    "{\r\n\r\n            await LocalStorage.RemoveItemAsync(\"Rol\");\r\n            await L" +
                    "ocalStorage.RemoveItemAsync(\"Roles\");\r\n\r\n            _menus.Clear();\r\n          " +
                    "  _menusCol.Clear();\r\n            _rawMenusCol.Clear();\r\n\r\n            _roles.Cl" +
                    "ear();\r\n\r\n            AuthStateProvider?.Logout();\r\n\r\n            StateHasChange" +
                    "d();\r\n        }\r\n\r\n        #endregion LogOut\r\n        \r\n    }\r\n}\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 371 "C:\Frog\WooW.SB\WooW.SB\BlazorGenerator\BlazorTemplates\WasmTemplates\Shared\WoTemplateWoHeaderLayoutPartialWasm.tt"

public string Project { get; set; } = "";

        
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
    public class WoTemplateWoHeaderLayoutPartialWasmBase
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