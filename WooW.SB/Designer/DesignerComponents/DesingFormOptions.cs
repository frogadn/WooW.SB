using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using ServiceStack.Text;
using Svg;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.Helpers.GeneratorsHelpers;
using WooW.SB.ManagerDirectory;
using WooW.SB.Themes;
using WooW.SB.Themes.ThemeOptions;

namespace WooW.SB.Designer.DesignerComponents
{
    public partial class DesingFormOptions
        : DevExpress.XtraLayout.Customization.UserCustomizationForm
    {
        #region Instancias singleton

        /// <summary>
        /// Observador principal.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Constructor

        /// <summary>
        /// Instancia el selector de temas.
        /// </summary>
        private WoThemeSelector _woThemeSelector = new WoThemeSelector();

        /// <summary>
        /// Constructor principal, Agrega el control de temas y agrega el
        /// controlador de temas al componente.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public DesingFormOptions()
        {
            InitializeComponent();

            _iconsBasePath = $@"{_project.DirLayOuts}\BlazorLibraries\bootstrap-icons\icons";

            pnlThemes.AddControl(_woThemeSelector);
            _woThemeSelector.Dock = DockStyle.Fill;
            _woThemeSelector.ApliedThemeEvt += ApliedTheme;
        }

        #endregion Constructor


        #region Propiedades de un control

        /// <summary>
        /// Objeto bindeado al property grid.
        /// </summary>
        private WoComponentProperties _componentProperties = new WoComponentProperties();

        /// <summary>
        /// Indica las propiedades anteriores de un control.
        /// Principalmente si se requiere recuperar algún dato previo a la modifican.
        /// </summary>
        private WoComponentProperties _oldproperties = new WoComponentProperties();

        /// <summary>
        /// Permite recibir y asignar al property grid las propiedades de un control.
        /// </summary>
        /// <param name="componentProperties"></param>
        [SupportedOSPlatform("windows")]
        public void SetComponentProperties(
            string selectedControl,
            WoComponentProperties componentProperties,
            List<string> showProperties,
            List<string> hideProperties
        )
        {
            lblSelectedControl.Text = $@"Control seleccionado: {selectedControl}";

            HidePropertiesHelper.ModifyBrowsableAttribute(
                _componentProperties,
                showProperties,
                true
            );
            HidePropertiesHelper.ModifyBrowsableAttribute(
                _componentProperties,
                hideProperties,
                false
            );

            propDesigner.Update();

            _componentProperties = componentProperties;
            _oldproperties = _componentProperties.GetCopyInstance();

            propDesigner.SelectedObject = _componentProperties;

            propDesigner.Update();
            propDesigner.Refresh();
        }

        /// <summary>
        /// Controlador de eventos que se detona cuando se realiza una modificación en el property grid.
        /// </summary>
        public EventHandler<WoComponentProperties> PropertyChangedEvt;

        #endregion Propiedades de un control

        #region Temas

        public EventHandler<string> ApliedThemeEvt { get; set; }

        public void ApliedTheme(object sender, string theme)
        {
            if (ApliedThemeEvt != null)
            {
                ApliedThemeEvt.Invoke(this, theme);
            }
        }

        public void SetTheme(string theme)
        {
            _woThemeSelector.SetTheme(theme);
        }

        #endregion Temas

        #region Cambio de la propiedad

        /// <summary>
        /// Se detona al actualizar el valor de una propiedad desde el property grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void propDesigner_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            if (
                e.Row.Properties.FieldName == "Step"
                || e.Row.Properties.FieldName == "Max"
                || e.Row.Properties.FieldName == "Min"
            )
            {
                _oldproperties.ChangedProperty = e.Row.Properties.FieldName;
                PropertyChangedEvt?.Invoke(this, _oldproperties);
            }
            else
            {
                if (e.Row.Properties.FieldName == "InputMask" && e.Value.ToString() != "Custom")
                {
                    _componentProperties.CustomMask = string.Empty;
                }

                if (e.Row.Properties.FieldName == "Id")
                {
                    if (
                        e.Value.ToString() == string.Empty
                        || e.Value.ToString().Contains(" ")
                        || e.Value.ToString().Contains("    ")
                    )
                    {
                        _componentProperties.Id = _oldproperties.Id;
                        _componentProperties.OldValue = _oldproperties.Id;
                        //ToDo: send log
                        XtraMessageBox.Show(
                            "El id ingresado es invalido",
                            "Alerta",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                    else
                    {
                        _componentProperties.Id = e.Value.ToString();
                        _componentProperties.OldValue = _oldproperties.Id;
                    }
                }

                if (e.Row.Properties.FieldName == "Etiqueta")
                {
                    string text = EtiquetaCol.Get(e.Value.ToString());
                    _componentProperties.Etiqueta = e.Value.ToString();
                    _componentProperties.MaskText = text;
                }

                _componentProperties.ChangedProperty = e.Row.Properties.FieldName;

                _oldproperties = _componentProperties.GetCopyInstance();
                propDesigner.Refresh();

                if (PropertyChangedEvt != null)
                {
                    if (e.Row.Properties.FieldName == "Id")
                    {
                        if (_componentProperties.Id == _componentProperties.OldValue)
                        {
                            return;
                        }
                    }
                    PropertyChangedEvt?.Invoke(this, _componentProperties);
                }
            }
        }

        /// <summary>
        /// Método que se ejecuta mientras se realiza la modificación de una
        /// de las propiedades de un control, solo para propiedades donde el usuario
        /// escribe el valor puesto que requieren de mas control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void propDesigner_CellValueChanging(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            if (
                e.Row.Properties.FieldName == "Step"
                || e.Row.Properties.FieldName == "Max"
                || e.Row.Properties.FieldName == "Min"
            )
            {
                if (e.Value.ToString() != string.Empty)
                {
                    if (e.Row.Properties.FieldName == "Step")
                    {
                        _componentProperties.Step = long.Parse(e.Value.ToString());
                    }
                    else if (e.Row.Properties.FieldName == "Max")
                    {
                        _componentProperties.Max = long.Parse(e.Value.ToString());
                    }
                    else if (e.Row.Properties.FieldName == "Min")
                    {
                        _componentProperties.Min = long.Parse(e.Value.ToString());
                    }
                    _componentProperties.ChangedProperty = e.Row.Properties.FieldName;
                    PropertyChangedEvt?.Invoke(this, _componentProperties);
                }
            }
        }

        #endregion Cambio de la propiedad


        #region Ocultar y mostrar propiedades

        public void ChangeVisibleProperties(bool hide)
        {
            if (hide)
            {
                tabProperties.Parent = null;
            }
            else
            {
                tabProperties.Parent = tbgOptions;
            }
        }

        #endregion Ocultar y mostrar propiedades


        #region Bloqueo de la tab de botones

        /// <summary>
        /// Oculta la tab de botones de la ventana de opciones.
        /// </summary>
        public void HideTabButtons()
        {
            tbgOptions.TabPages.Remove(tabPageCustomButtons);
        }

        #endregion Bloqueo de la tab de botones

        #region Lista de los botones

        /// <summary>
        /// Nombre del modelo sobre el que se esta trabajando
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// Lista de los botones custom.
        /// </summary>
        private List<WoCustomButtonProperties> _customButtons =
            new List<WoCustomButtonProperties>();

        /// <summary>
        /// Ruta del archivo json donde se guardan los botones custom.
        /// </summary>
        private string _pathFile = string.Empty;

        /// <summary>
        /// Carga la lista de los botones custom desde el json.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void ChargeButtonList()
        {
            _customButtons.Clear();
            pnlButtons.Controls.Clear();

            _pathFile =
                $@"{_project.DirLayOuts}\UserCode\{ModelName}_proj\{ModelName}CustomButtons.json";
            if (File.Exists(_pathFile))
            {
                string json = WoDirectory.ReadFile(_pathFile);
                _customButtons = JsonConvert.DeserializeObject<List<WoCustomButtonProperties>>(
                    json
                );
            }

            ChargeButtonsToList();

            ChargeControlsNotAssigned();
        }

        /// <summary>
        /// Carga los botones custom al panel con los botones
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeButtonsToList()
        {
            int buttonValidateCount = 0;
            pnlButtons.Controls.Clear();

            foreach (
                WoCustomButtonProperties button in _customButtons.OrderBy(button => button.Index)
            )
            {
                if (button.Index == 0)
                {
                    do
                    {
                        buttonValidateCount++;
                    } while (
                        _customButtons.FirstOrDefault(x => x.Index == buttonValidateCount) != null
                    );

                    button.Index = buttonValidateCount;
                }
            }

            foreach (
                WoCustomButtonProperties button in _customButtons.OrderByDescending(button =>
                    button.Index
                )
            )
            {
                SimpleButton newBtn = new SimpleButton();
                newBtn.Name = button.ButtonId;
                newBtn.Text = button.MaskText;
                newBtn.Dock = DockStyle.Top;
                newBtn.Tag = button;

                if (button.Icon != eBoostrapIcons.None)
                {
                    System.Drawing.Image icon = null;
                    var svgDoc = SvgDocument.Open(
                        $@"{_iconsBasePath}\{_woCommonDesignOptions.BoostrapIcons.Get(button.Icon.ToString())}"
                    );

                    icon = svgDoc.Draw();
                    newBtn.ImageOptions.Image = icon;
                }

                newBtn.Click += SelectButton;
                pnlButtons.Controls.Add(newBtn);
            }
        }

        #endregion Lista de los botones

        #region Creacion de los botones

        /// <summary>
        /// Agrega botones custom para el color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnAddButtons_Click(object sender, EventArgs e)
        {
            string newId = GetNewId();
            int newIndex = 0;

            WoCustomButtonProperties lastButton = _customButtons
                .OrderByDescending(button => button.Index)
                .FirstOrDefault();
            if (lastButton == null)
            {
                newIndex = 1;
            }
            else
            {
                newIndex = lastButton.Index + 1;
            }

            WoCustomButtonProperties newCustomButton = new WoCustomButtonProperties()
            {
                ButtonId = newId,
                Label = "Button",
                MaskText = "Button",
                Icon = eBoostrapIcons.None,
                Index = newIndex
            };
            _customButtons.Add(newCustomButton);

            SimpleButton newBtn = new SimpleButton();
            newBtn.Name = newId;
            newBtn.Text = "Button";
            newBtn.Dock = DockStyle.Top;
            newBtn.Tag = newCustomButton;
            newBtn.Click += SelectButton;
            pnlButtons.Controls.Add(newBtn);

            UpdateJson();

            CreateButton(newCustomButton);

            ChargeButtonsToList();
        }

        /// <summary>
        /// Actualiza el json con la lista de los botones custom.
        /// </summary>
        private void UpdateJson()
        {
            if (File.Exists(_pathFile))
            {
                WoDirectory.DeleteFile(_pathFile);
            }

            WoDirectory.WriteFile(
                _pathFile,
                JsonConvert.SerializeObject(_customButtons.OrderBy(button => button.Index))
            );
        }

        /// <summary>
        /// Genera un identificador nuevo para el sigiente boton custom.
        /// </summary>
        /// <returns></returns>
        private string GetNewId()
        {
            string id = string.Empty;

            WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
            string projectName = woProjectDataHelper.GetWWSBName(_project);
            string baseProject = projectName.Split(".").First().Replace(" ", "");

            int count = 0;
            do
            {
                count++;
                id = $"btnCustom{baseProject}{count}";
            } while ((_customButtons.FirstOrDefault(x => x.ButtonId == id)) != null);

            return id;
        }

        #endregion Creacion de los botones

        #region Creacion del metodo custom

        /// <summary>
        /// Controlador de eventos que se detona cuando se crea un nuevo metodo.
        /// </summary>
        public Action UpdateCodeEditor { get; set; }

        /// <summary>
        /// Creacion del boton
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void CreateButton(WoCustomButtonProperties newCustomButton)
        {
            WoSyntaxManagerUserCode userCode = new WoSyntaxManagerUserCode();

            userCode.InitializeManager(
                pathScript: $@"{_project.DirLayOuts}\UserCode\{ModelName}_proj\{ModelName}ScriptsUser.cs",
                className: ModelName,
                modelName: ModelName
            );

            userCode.CreateNewMethod(
                methodName: $@"{newCustomButton.ButtonId}_OnClick",
                bodyMethod: string.Empty,
                typeMethod: "void"
            );

            UpdateCodeEditor?.Invoke();
        }

        #endregion Creacion del metodo custom

        #region Seleccion de los botones

        /// <summary>
        /// Instancia del bvoton que se esta editando.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private SimpleButton _selectedButton = new SimpleButton();

        /// <summary>
        /// Selecciona el boton que se esta editando.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void SelectButton(object sender, EventArgs e)
        {
            _selectedButton = (SimpleButton)sender;
            _selectedButtonProperties = (WoCustomButtonProperties)_selectedButton.Tag;
            propButton.SelectedObject = _selectedButton.Tag;

            btnDeleteButton.Enabled = true;
            btnUpButton.Enabled = true;
            btnDownButton.Enabled = true;
        }

        #endregion Seleccion de los botones

        #region Edicion de los botones

        /// <summary>
        /// Opciones de diseño comunes para todos los botones.
        /// </summary>
        private WoCommonDesignOptions _woCommonDesignOptions = new WoCommonDesignOptions();

        /// <summary>
        /// Indica cual es el boton que se esta configurando
        /// </summary>
        private WoCustomButtonProperties _selectedButtonProperties = new WoCustomButtonProperties();

        /// <summary>
        /// PathBase de los iconos
        /// Se inicializa en el constructor.
        /// </summary>
        private string _iconsBasePath = string.Empty;

        /// <summary>
        /// Mantiene actualizado el json en funcion de las propiedades realizadas.
        /// </summary>7
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void propButton_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e
        )
        {
            if (e.Row.Properties.FieldName == "Label")
            {
                _selectedButtonProperties.Label = e.Value.ToString();
                _selectedButtonProperties.MaskText = EtiquetaCol.Get(e.Value.ToString());
                _selectedButton.Text = _selectedButtonProperties.MaskText;
            }
            else if (e.Row.Properties.FieldName == "Icon")
            {
                _selectedButtonProperties.Icon = (eBoostrapIcons)e.Value;
                System.Drawing.Image icon = null;

                var svgDoc = SvgDocument.Open(
                    $@"{_iconsBasePath}\{_woCommonDesignOptions.BoostrapIcons.Get(_selectedButtonProperties.Icon.ToString())}"
                );

                icon = svgDoc.Draw();

                _selectedButton.ImageOptions.Image = icon;
            }

            _customButtons.Remove(button => button.ButtonId == _selectedButtonProperties.ButtonId);
            _customButtons.Add(_selectedButtonProperties);

            UpdateJson();

            UpdateCodeEditor?.Invoke();
        }

        #endregion Edicion de los botones

        #region Eliminar boton

        /// <summary>
        /// Elimina el boton seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (_selectedButtonProperties != null)
            {
                DeleteMethodButton(_selectedButtonProperties);

                pnlButtons.Controls.Remove(_selectedButton);
                _customButtons.Remove(button =>
                    button.ButtonId == _selectedButtonProperties.ButtonId
                );
                propButton.SelectedObject = null;

                UpdateJson();

                UpdateCodeEditor?.Invoke();

                _selectedButtonProperties = null;

                btnDeleteButton.Enabled = false;
                btnUpButton.Enabled = false;
                btnDownButton.Enabled = false;
            }
        }

        #endregion Eliminar boton

        #region Eliminar metodo

        /// <summary>
        /// Eliminacion del boton
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void DeleteMethodButton(WoCustomButtonProperties newCustomButton)
        {
            WoSyntaxManagerUserCode userCode = new WoSyntaxManagerUserCode();

            userCode.InitializeManager(
                pathScript: $@"{_project.DirLayOuts}\UserCode\{ModelName}_proj\{ModelName}ScriptsUser.cs",
                className: ModelName,
                modelName: ModelName
            );

            userCode.DeleteMethod(methodName: $@"{newCustomButton.ButtonId}_OnClick");

            UpdateCodeEditor?.Invoke();
        }

        #endregion Eliminar metodo


        #region Cambiar el posicionado de los botones

        /// <summary>
        /// Modifica el orden de los botones modificando la instancia de la clase en el tag
        /// restando 1 a la propiedad de index y bolbiendo a cargar el listado de los botones.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnUpButton_Click(object sender, EventArgs e)
        {
            if (_selectedButtonProperties.Index > 1)
            {
                List<WoCustomButtonProperties> newButtonList = new List<WoCustomButtonProperties>();
                int index = 1;

                foreach (
                    WoCustomButtonProperties buttonProperties in _customButtons.OrderBy(button =>
                        button.Index
                    )
                )
                {
                    if (buttonProperties.ButtonId == _selectedButtonProperties.ButtonId)
                    {
                        newButtonList.Last().Index++;
                        buttonProperties.Index = index - 1;
                        newButtonList.Add(buttonProperties);
                        index++;
                    }
                    else
                    {
                        buttonProperties.Index = index;
                        newButtonList.Add(buttonProperties);
                        index++;
                    }
                }

                _customButtons.Clear();
                _customButtons.AddRange(newButtonList);

                UpdateJson();
                ChargeButtonsToList();

                UpdateCodeEditor?.Invoke();
            }
        }

        /// <summary>
        /// Modifica el orden de los botones modificando la instancia de la clase en el tag
        /// sumando 1 a la propiedad de index y reoranizando el listado de los botones.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [SupportedOSPlatform("windows")]
        private void btnDownButton_Click(object sender, EventArgs e)
        {
            List<WoCustomButtonProperties> newButtonList = new List<WoCustomButtonProperties>();
            int index = 1;

            WoCustomButtonProperties findButtonProperties = null;

            foreach (
                WoCustomButtonProperties buttonProperties in _customButtons.OrderBy(button =>
                    button.Index
                )
            )
            {
                if (buttonProperties.ButtonId == _selectedButtonProperties.ButtonId)
                {
                    findButtonProperties = buttonProperties;
                }
                else if (findButtonProperties != null)
                {
                    buttonProperties.Index = index;
                    newButtonList.Add(buttonProperties);
                    index++;
                    findButtonProperties.Index++;
                    newButtonList.Add(findButtonProperties);
                    index++;
                    findButtonProperties = null;
                }
                else
                {
                    buttonProperties.Index = index;
                    newButtonList.Add(buttonProperties);
                    index++;
                }
            }

            if (findButtonProperties != null)
            {
                newButtonList.Add(findButtonProperties);
                findButtonProperties = null;
            }

            _customButtons.Clear();
            _customButtons.AddRange(newButtonList);

            UpdateJson();
            ChargeButtonsToList();

            UpdateCodeEditor?.Invoke();
        }

        #endregion Cambiar el posicionado de los botones


        #region Controles no asignados

        /// <summary>
        /// Table para la grid de los controles no asignados.
        /// </summary>
        private DataTable _notAssignedCol = null;

        /// <summary>
        /// Carga los controles no asignados en la grid.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeControlsNotAssigned()
        {
            if (ModelName != null && ModelName != string.Empty)
            {
                Modelo model = _project.ModeloCol.Modelos.FirstOrDefault(model =>
                    model.Id == ModelName
                );
                if (model != null)
                {
                    _notAssignedCol = new DataTable();
                    _notAssignedCol.Columns.Add("Id", typeof(string));
                    _notAssignedCol.Columns.Add("Descripción", typeof(string));
                    _notAssignedCol.Columns.Add("Tipo de dato", typeof(string));
                    _notAssignedCol.Columns.Add("Tipo de control", typeof(string));

                    IEnumerable<ModeloColumna> controlsNotAssignedCol = model.Columnas.Where(
                        column =>
                            (
                                column.TipoControl == Core.WoTypeControl.UnKnown
                                || column.TipoControl == Core.WoTypeControl.NA
                            )
                    );

                    if (controlsNotAssignedCol != null)
                    {
                        foreach (ModeloColumna column in controlsNotAssignedCol)
                        {
                            DataRow newRow = _notAssignedCol.NewRow();
                            newRow["Id"] = column.Id;
                            newRow["Descripción"] = column.Descripcion;
                            newRow["Tipo de dato"] = column.TipoDato.ToString();
                            newRow["Tipo de control"] = column.TipoControl.ToString();

                            _notAssignedCol.Rows.Add(newRow);
                        }
                    }

                    grdNotAssignedControls.DataSource = _notAssignedCol;

                    grdViewNotAssignedControls.Columns["Id"].OptionsColumn.AllowEdit = false;
                    grdViewNotAssignedControls.Columns["Descripción"].OptionsColumn.AllowEdit =
                        false;
                    grdViewNotAssignedControls.Columns["Tipo de dato"].OptionsColumn.AllowEdit =
                        false;
                    grdViewNotAssignedControls.Columns["Tipo de control"].OptionsColumn.AllowEdit =
                        false;

                    grdViewNotAssignedControls.ClearSorting();
                    grdViewNotAssignedControls.Columns["Id"].SortOrder = DevExpress
                        .Data
                        .ColumnSortOrder
                        .Descending;
                }
            }
        }

        #endregion Controles no asignados
    }
}
