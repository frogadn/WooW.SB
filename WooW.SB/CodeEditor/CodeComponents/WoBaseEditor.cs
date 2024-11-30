using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;
using ActiproSoftware.Text;
using ActiproSoftware.Text.Languages.CSharp.Implementation;
using ActiproSoftware.Text.Parsing;
using ServiceStack;
using WooW.Core;

namespace WooW.SB.CodeEditor.CodeComponents
{
    public partial class WoBaseEditor : UserControl
    {
        #region Constructores

        /// <summary>
        /// Constructor base y principal,
        /// inicializa el componente e inicializa el lenguaje del syntax.
        /// </summary>
        /// <param name="language"></param>
        [SupportedOSPlatform("windows")]
        public WoBaseEditor(CSharpSyntaxLanguage language)
        {
            InitializeComponent();

            syeCodigo.Document.Language = language;

            syeCodigo.Document.TextChanged += CodeChanged;
        }

        #endregion Constructores


        #region Syntax Editor


        #region Eventos del syntax

        /// <summary>
        /// Action que se detonara al modificar el código del syntax.
        /// no recibe parámetros ni retorna nada.
        /// </summary>
        public Action CodeChangedEvt;

        /// <summary>
        /// Método suscrito al evento de cambio de texto del syntax.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodeChanged(object sender, TextSnapshotChangedEventArgs e)
        {
            if (e.OldSnapshot.Text != e.NewSnapshot.Text && e.OldSnapshot.Text != string.Empty)
            {
                CodeChangedEvt?.Invoke();
            }
        }

        #endregion Eventos del syntax


        #region Definir Header y Footer

        /// <summary>
        /// Define el header y el footer del syntax.
        /// se reciben en una tupla (string header, string footer).
        /// </summary>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        [SupportedOSPlatform("windows")]
        public void AssignHeaderAndFooter((string header, string footer) code)
        {
            syeCodigo.Document.SetHeaderAndFooterText(code.header, code.footer);
        }

        #endregion Definir Header y Footer

        #region Preguntas a la clase

        /// <summary>
        /// Retorna true o false en función del estado del editor de código.
        /// Tienen código : true
        /// No tiene código : false
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public bool HaveCode()
        {
            return !syeCodigo.Text.IsNullOrStringEmpty();
        }

        #endregion Preguntas a la clase

        #region Gestión del código

        /// <summary>
        /// Remplaza el código actual del syntax por el que se recibe por parámetro.
        /// </summary>
        /// <param name="code"></param>
        [SupportedOSPlatform("windows")]
        public void SetCode(string code)
        {
            syeCodigo.Text = string.Empty;
            syeCodigo.Text = code;
        }

        /// <summary>
        /// Retorna el código actual del syntax.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public string GetCode()
        {
            return syeCodigo.Text;
        }

        /// <summary>
        /// Limpia el editor de código.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void CleanEditor()
        {
            syeCodigo.Text = string.Empty;
        }

        #endregion Gestión del código

        #region Eventos del syntax

        /// <summary>
        /// Agrega el texto que se recibe por parametro en la posicion actual.
        /// </summary>
        /// <param name="addText"></param>
        [SupportedOSPlatform("windows")]
        public void SetCodeSnipet(string addText)
        {
            string selectedText = syeCodigo.ActiveView.SelectedText;

            if (selectedText == string.Empty)
            {
                int characterPosition = syeCodigo.ActiveView.CurrentViewLine.EndOffset;

                int lineIndex = syeCodigo.ActiveView.CurrentViewLine.EndPosition.Line;

                syeCodigo.Text = syeCodigo.Text.Insert(
                    characterPosition + lineIndex,
                    $"{addText}\n"
                );
            }
            else
            {
                string line = syeCodigo.ActiveView.CurrentViewLine.Text.ReplaceFirst(
                    selectedText,
                    addText
                );

                syeCodigo.ActiveView.CurrentViewLine.Text = line;
            }

            CodeChangedEvt?.Invoke();
        }

        #endregion Eventos del syntax

        #endregion Syntax Editor

        #region Lista de errores

        #region Variables globales

        /// <summary>
        /// Bandera para saves si hay modificaciones en el código.
        /// </summary>
        private bool _hasPendingParseData = false;

        #endregion Lista de errores

        #region Actualizaciones del editor

        /// <summary>
        /// CUando la información del editor de código es modificada
        /// indica que falta código por verificarse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCodeEditorDocumentParseDataChanged(object sender, EventArgs e)
        {
            _hasPendingParseData = true;
        }

        /// <summary>
        /// Cuando se verifica el código del syntax se detona y actualiza la
        /// bandera que indica si hay código pendiente y actualiza la lista de errores.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void OnCodeEditorUserInterfaceUpdate(object sender, EventArgs e)
        {
            // If there is a pending parse data change...
            if (_hasPendingParseData)
            {
                // Clear flag
                _hasPendingParseData = false;

                var parseData = syeCodigo.Document.ParseData as IParseErrorProvider;
                if (parseData != null)
                {
                    // Output errors
                    RefreshErrorList(parseData.Errors);
                }
                else
                {
                    // Clear UI
                    this.RefreshErrorList(null);
                }
            }
        }

        #endregion Actualizaciones del editor

        #region Gestión de la lista de errores.

        /// <summary>
        /// Actualiza la lista de errores con la colección que se recibe por parámetro.
        /// </summary>
        /// <param name="errors"></param>
        [SupportedOSPlatform("windows")]
        private void RefreshErrorList(IEnumerable<IParseError> errorsCol)
        {
            errorListView.Items.Clear();

            if (errorsCol != null)
            {
                foreach (var error in errorsCol)
                {
                    var item = new ListViewItem(
                        new string[]
                        {
                            error.PositionRange.StartPosition.DisplayLine.ToString(),
                            error.PositionRange.StartPosition.DisplayCharacter.ToString(),
                            error.Description
                        }
                    );
                    item.Tag = error;
                    errorListView.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Actualiza la lista de errores de forma externa
        /// con la lista de errores que se recibe por parámetro.
        /// </summary>
        /// <param name="outputCompilation"></param>
        [SupportedOSPlatform("windows")]
        public void SetErrorList(List<ListViewItem> outputCompilation)
        {
            errorListView.Items.AddRange(outputCompilation.ToArray());
        }

        #endregion Gestión de la lista de errores.

        #endregion Lista de errores
    }
}
