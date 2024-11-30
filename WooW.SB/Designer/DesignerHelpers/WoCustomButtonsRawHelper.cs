using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.CodeEditor.SyntaxManager.SyntaxManagerClass;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoCustomButtonsRawHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto permite enviar logs y alertas.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Contiene toda la información del proyecto.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Path del fichero con la información de los botones personalizados.
        /// </summary>
        private string _pathFile = string.Empty;

        /// <summary>
        /// Nombre del modelo sobre del que se esta trabajando.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Modelo del que se generaran los botones
        /// </summary>
        private Modelo _model = null;

        /// <summary>
        /// Lista de los botones custom.
        /// </summary>
        private List<WoCustomButtonProperties> _customButtons =
            new List<WoCustomButtonProperties>();

        #endregion Atributos

        #region Métodos principales

        /// <summary>
        /// Construye el archivo base de los botones personalizados.
        /// (CAMBIAR A PUBLIC CUANDO SE USE #TODO)
        /// </summary>
        /// <param name="modelName"></param>
        private void BuidRawBase(string modelName)
        {
            _modelName = modelName;

            _pathFile =
                $@"{_project.DirLayOuts}\UserCode\{modelName}_proj\{modelName}CustomButtons.json";

            if (!File.Exists(_pathFile))
            {
                _customButtons.Clear();

                //BuildButton();

                WoDirectory.WriteFile(
                    _pathFile,
                    JsonConvert.SerializeObject(_customButtons.OrderBy(button => button.Index))
                );
            }
        }

        /// <summary>
        /// Construye la lista de botones personalizados para la grid.
        /// </summary>
        /// <param name="modelName"></param>
        public void BuildRawGridList(string modelName)
        {
            _modelName = modelName;

            _model = _project.ModeloCol.Modelos.FirstOrDefault(model => model.Id == modelName);

            _pathFile =
                $@"{_project.DirLayOuts}\UserCode\{modelName}GridList_proj\{modelName}GridListCustomButtons.json";

            if (!File.Exists(_pathFile))
            {
                _customButtons.Clear();

                BuildGridButton();

                WoDirectory.WriteFile(
                    _pathFile,
                    JsonConvert.SerializeObject(_customButtons.OrderBy(button => button.Index))
                );
            }
        }

        #endregion Métodos principales


        #region Build Raw Buttons

        /// <summary>
        /// Crea los botones personalizados base en función del modelo
        /// </summary>
        private void BuildGridButton()
        {
            BuildBtnNew();
            BuildBtnEdit();
            BuildBtnCopyToNew();
        }

        #endregion Build Raw Buttons

        #region Button Nuevo

        /// <summary>
        /// Creación del botón para la creación de un nuevo registro
        /// </summary>
        private void BuildBtnNew()
        {
            try
            {
                WoCustomButtonProperties woCustomButtonNewReg = new WoCustomButtonProperties();
                woCustomButtonNewReg.ButtonId = "BtnNew";
                woCustomButtonNewReg.MaskText = "Nuevo";
                woCustomButtonNewReg.Label = "Nuevo";
                woCustomButtonNewReg.Index = 1;
                woCustomButtonNewReg.MethodName = "BtnNew";
                woCustomButtonNewReg.Icon = Themes.ThemeOptions.eBoostrapIcons.pluscircle;

                _customButtons.Add(woCustomButtonNewReg);

                BuildGridCodeButton(woCustomButtonNewReg.MethodName, GetCodeBtnNew());
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al crear el botón para un nuevo registro. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Recuperación del codigo para realizar un nuevo registro
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string GetCodeBtnNew()
        {
            try
            {
                StringBuilder strCode = new StringBuilder();

                strCode.AppendLine($@"///");

                return strCode.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al crear el codigo para la creación de un nuevo registro. {ex.Message}"
                );
            }
        }

        #endregion Button Nuevo

        #region Button editar

        /// <summary>
        /// Creación del botón para la edición
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildBtnEdit()
        {
            try
            {
                WoCustomButtonProperties woCustomButtonEdit = new WoCustomButtonProperties();
                woCustomButtonEdit.ButtonId = "BtnEdit";
                woCustomButtonEdit.MaskText = "Editar";
                woCustomButtonEdit.Label = "Editar";
                woCustomButtonEdit.Index = 2;
                woCustomButtonEdit.MethodName = "BtnEdit";
                woCustomButtonEdit.Icon = Themes.ThemeOptions.eBoostrapIcons.boxarrowupleft;

                _customButtons.Add(woCustomButtonEdit);

                BuildGridCodeButton(woCustomButtonEdit.MethodName, GetCodeBtnEdit());
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al crear el botón de editar. {ex.Message}");
            }
        }

        /// <summary>
        /// Recupera el código para el botón de editar
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string GetCodeBtnEdit()
        {
            try
            {
                StringBuilder bodyMethod = new StringBuilder();

                bodyMethod.AppendLine(
                    $@"
                string route = $@""/{_model.ProcesoId}/{_model.TipoModelo}/{_modelName}"";
                JS.InvokeAsync<object>(""NavigateTo"", [route]);"
                );

                return bodyMethod.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar el codigo para el botón de editar. {ex.Message}"
                );
            }
        }

        #endregion Button editar

        #region Button copy to new

        /// <summary>
        /// Crear el botón para copiar el registro a un nuevo formulario
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildBtnCopyToNew()
        {
            try
            {
                WoCustomButtonProperties woCustomButtonProperties = new WoCustomButtonProperties();
                woCustomButtonProperties.ButtonId = "BtnCopyToNew";
                woCustomButtonProperties.MaskText = "Copiar a nuevo";
                woCustomButtonProperties.Label = "CopyToNew";
                woCustomButtonProperties.MethodName = "BtnCopyToNew";
                woCustomButtonProperties.Index = 3;
                woCustomButtonProperties.Icon = Themes.ThemeOptions.eBoostrapIcons.clipboardplus;

                _customButtons.Add(woCustomButtonProperties);

                BuildGridCodeButton(woCustomButtonProperties.MethodName, GetCodeBtnCopyToNew());
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al crear el botón de copiar a uno nuevo. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Recupera el código para el botón de editar
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string GetCodeBtnCopyToNew()
        {
            try
            {
                StringBuilder bodyMethod = new StringBuilder();

                bodyMethod.AppendLine($@"///");

                return bodyMethod.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar el codigo para el botón de editar. {ex.Message}"
                );
            }
        }

        #endregion Button copy to new


        #region Create new method

        /// <summary>
        /// Genera el método en el fichero de código en función del botón generado.
        /// </summary>
        /// <param name="buttonId"></param>
        private void BuildGridCodeButton(string methodName, string bodyMethod = "")
        {
            WoSyntaxManagerUserCode userCode = new WoSyntaxManagerUserCode();
            userCode.InitializeManager(
                pathScript: $@"{_project.DirLayOuts}\UserCode\{_modelName}GridList_proj\{_modelName}GridListScriptsUser.cs",
                className: $@"{_modelName}GridList",
                modelName: _modelName
            );

            userCode.CreateNewMethod(
                methodName: $@"{methodName}_OnClick",
                bodyMethod: bodyMethod,
                typeMethod: "void"
            );
        }

        #endregion Create new method
    }
}
