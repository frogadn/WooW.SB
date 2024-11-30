using System;
using System.IO;
using System.Text;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.BuildTest
{
    public class WoBuildTest
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Data de la transición seleccionada
        /// </summary>
        private Transicion _transicion;

        /// <summary>
        /// Data del modelo seleccionado
        /// </summary>
        private Modelo _model;

        /// <summary>
        /// Data del diseño del formulario
        /// </summary>
        private WoContainer _mainContainer;

        /// <summary>
        /// Path del test a generar
        /// </summary>
        private string _pathTest = string.Empty;

        #endregion Atributos

        #region Construcción

        /// <summary>
        /// Constructor principal de la clase, recibe el modelo y la transición
        /// </summary>
        /// <param name="model"></param>
        /// <param name="transicion"></param>
        /// <exception cref="Exception"></exception>
        //public WoBuildTest(string modelName)
        //{
        //    try
        //    {
        //        _model = _project.ModeloCol.Modelos.Where(m => m.Id == modelName).First();

        //        string pathFormDesign = $@"{_project.DirFormDesign}\{modelName}.json";
        //        string rawJson = WoDirectory.ReadFile(pathFormDesign);

        //        _mainContainer = JsonConvert.DeserializeObject<WoContainer>(rawJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(
        //            $@"Ocurrió un error en la instancia de la clase que genera la prueba. {ex.Message}"
        //        );
        //    }
        //}

        /// <summary>
        /// Constructor para la generación de la plantilla base para las pruebas de js
        /// </summary>
        /// <exception cref="Exception"></exception>
        //public WoBuildTest()
        //{
        //    try
        //    {
        //        //
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(
        //            $@"Ocurrió un error en la instancia de la clase que genera la prueba. {ex.Message}"
        //        );
        //    }
        //}

        #endregion Construcción


        #region Método principal

        /// <summary>
        /// Método principal para la construcción de un nuevo caso prueba
        /// en función de la transición
        /// </summary>
        public string BuildNewTest(string pathTest)
        {
            try
            {
                _pathTest = pathTest;

                //string fileName = Path.GetFileName(pathTest);

                StringBuilder strTest = new StringBuilder();

                strTest.AppendLine(BuildDescription());

                strTest.AppendLine(BuildImports());
                strTest.AppendLine(BuildHeader());

                strTest.AppendLine(BuildCode());

                strTest.AppendLine(BuildFooter());

                return strTest.ToString();

                //WoDirectory.WriteFile(path: pathTest, data: strTest.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($@"Ocurrió un error al construir la prueba. {ex.Message}");
            }
        }

        #endregion Método principal

        #region Build Descripción

        /// <summary>
        /// Agregamos una descripción con información de la prueba
        /// </summary>
        /// <returns></returns>
        private string BuildDescription()
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(_pathTest);

                StringBuilder strDescription = new StringBuilder();

                strDescription.AppendLine(
                    $@"///----------------------------------------------------"
                );
                strDescription.AppendLine($@"///Modelos: __NombreDelModelo_");
                strDescription.AppendLine($@"///Prueba: {fileName}");
                strDescription.AppendLine(
                    "///Para asegurar el correcto funcionamiento del test evite modificar la estructura del código encerrado marcado como IMPORTANTE"
                );
                strDescription.AppendLine(
                    $@"///----------------------------------------------------"
                );

                return strDescription.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al crear la descripción de la prueba. {ex.Message}");
            }
        }

        #endregion Build Descripción

        #region Build Imports

        /// <summary>
        /// Agrega los imports a la clase de las pruebas
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string BuildImports()
        {
            try
            {
                string[] pathTestCol = _pathTest.Split("\\");
                string escs = string.Empty;
                bool unitTestPath = false;

                foreach (string pathPart in pathTestCol)
                {
                    if (unitTestPath && !pathPart.Contains(".js"))
                    {
                        escs += "../";
                    }
                    unitTestPath = (unitTestPath) ? unitTestPath : pathPart == "UnitTest";
                }

                string baseRefPath = $@"{escs}../TestCafe/Common";

                StringBuilder strImports = new StringBuilder();

                strImports.AppendLine(
                    $@"///IMPORTANTE----------------------------------------------------"
                );
                strImports.AppendLine(
                    $@"///Referencia de los otros ficheros js con clases y funciones para el funcionamiento de la prueba"
                );

                strImports.AppendLine(
                    $@"import {{ Uth }} from '{escs}../TestCafe/wotestcafe/Tools/Helpers/Uth.js';"
                );

                strImports.AppendLine(
                    $@"///--------------------------------------------------------------"
                );

                return strImports.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al crear los imports. {ex.Message}");
            }
        }

        #endregion Build Imports

        #region Build Header

        /// <summary>
        /// Construimos el header genérico para las pruebas
        /// </summary>
        /// <returns></returns>
        private string BuildHeader()
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(_pathTest);

                string launchSettingsPath =
                    $@"{_project.DirApplication}\Temp\ServerUnitModel_proj\Properties\launchSettings.json";
                string lauchSettings = WoDirectory.ReadFile(launchSettingsPath);
                string[] launchSettingsCol = lauchSettings.Split("profiles");
                string[] launchSettingsCol1 = launchSettingsCol[1].Split($@"""applicationUrl"":");
                string[] launchSettingsCol2 = launchSettingsCol1[1].Split(";");
                string url = launchSettingsCol2[0].Replace("\"", "").Replace(" ", "");

                StringBuilder strHeader = new StringBuilder();

                strHeader.AppendLine(
                    $@"///IMPORTANTE----------------------------------------------------"
                );
                strHeader.AppendLine($@"///Creación del contexto para la ejecución de la prueba.");

                strHeader.AppendLine("///Configuración de la url de inicio de la prueba");
                strHeader.AppendLine($@"fixture `{fileName}`.page`{url}/Index`;");

                strHeader.AppendLine(
                    "///Configuración del contexto de la prueba, dentro de una arrow function"
                );
                strHeader.AppendLine($@"test('{fileName}', async t =>");
                strHeader.AppendLine("{");

                strHeader.AppendLine("    ///Try que encapsula el test");
                strHeader.AppendLine("    try");
                strHeader.AppendLine("    {");

                strHeader.AppendLine("        ///Instancia del modelo con utilidades para el test");
                strHeader.AppendLine(
                    $@"        var _uth = new Uth('__NombreDelModelo_', '{fileName}');"
                );

                strHeader.AppendLine("        ///Instancia del modelo de la clase");
                strHeader.AppendLine(
                    $@"        var __NombreDelModelo_ = new Model__NombreDelModelo_();"
                );

                strHeader.AppendLine("        ///Instancia del formulario del modelo");
                strHeader.AppendLine(
                    $@"        var _formulario = new Formulario__NombreDelModelo_();"
                );

                strHeader.AppendLine(
                    $@"///--------------------------------------------------------------"
                );

                return strHeader.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en el header. {ex.Message}");
            }
        }

        #endregion Build Header

        #region Build Code

        /// <summary>
        /// Código default del test
        /// </summary>
        private StringBuilder _strTestDefaultCode = new StringBuilder();

        /// <summary>
        /// Construye el codigo base que se utiliza para realizar una prueba
        /// </summary>
        /// <returns></returns>
        //private string BuildCode()
        //{
        //    try
        //    {
        //        StringBuilder strTestCode = new StringBuilder();

        //        strTestCode.AppendLine(
        //            $@"///IMPORTANTE----------------------------------------------------"
        //        );
        //        strTestCode.AppendLine($@"        ///Autenticación a través del login.");

        //        // Login del formulario
        //        strTestCode.AppendLine($@"        await _uth.Login();");

        //        strTestCode.AppendLine(
        //            $@"///--------------------------------------------------------------"
        //        );
        //        strTestCode.AppendLine("\n");

        //        if (_transicion != null)
        //        {
        //            /// Selección de la transición
        //            strTestCode.AppendLine(
        //                $@"        await _formulario.Controles.cmbTransition.SelectItem('{_transicion.Id}');"
        //            );

        //            /// Creación del codigo
        //            BuildCodeRec(_mainContainer);
        //            strTestCode.AppendLine(_strTestDefaultCode.ToString());

        //            // Hacemos click el botón de ejecutar
        //            strTestCode.AppendLine($@"        await _formulario.Controles.btnNew.Click();");
        //        }
        //        else
        //        {
        //            strTestCode.AppendLine($@"        /// Ingrese su código aquí");
        //        }

        //        // Indicamos que el test se realizo correctamente
        //        strTestCode.AppendLine($@"        await _uth.logSucc();");

        //        strTestCode.AppendLine("\n");

        //        return strTestCode.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        /// <summary>
        /// Construye el codigo base que se utiliza para realizar una prueba
        /// </summary>
        /// <returns></returns>
        private string BuildCode()
        {
            try
            {
                StringBuilder strTestCode = new StringBuilder();

                strTestCode.AppendLine(
                    $@"///IMPORTANTE----------------------------------------------------"
                );
                strTestCode.AppendLine($@"        ///Autenticación a través del login.");

                // Login del formulario
                strTestCode.AppendLine($@"        await _uth.Login();");

                strTestCode.AppendLine(
                    $@"///--------------------------------------------------------------"
                );
                strTestCode.AppendLine("\n");

                strTestCode.AppendLine($@"        /// Ingrese su código aquí");

                return strTestCode.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Construcción del codigo recorriendo el diseño del formulario
        /// </summary>
        /// <param name="container"></param>
        /// <exception cref="Exception"></exception>
        private void BuildCodeRec(WoContainer mainContainer)
        {
            try
            {
                if (mainContainer.ContainersCol != null)
                {
                    foreach (WoContainer subContainer in mainContainer.ContainersCol)
                    {
                        WoContainer container = subContainer;
                        BuildCodeRec(container);
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    if (
                        mainContainer.ItemsCol.Count > 0
                        && mainContainer.TypeContainer == eTypeContainer.FormTab
                    )
                    {
                        _strTestDefaultCode.AppendLine($@"        ///Cambio de la tab");
                    }

                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        if (_transicion.DTO.Columnas.Contains(item.Id))
                        {
                            if (
                                item.Control == "Decimal"
                                || item.Control == "Text"
                                || item.Control == "byte[]"
                                || item.Control == "Memo"
                                || item.Control == "Spin"
                                || item.Control == "LookUp"
                                || item.Control == "LookUpDialog"
                                || item.Control == "EnumString"
                                || item.Control == "EnumInt"
                                || item.Control == "Date"
                                || item.Control == "Bool"
                            )
                            {
                                _strTestDefaultCode.AppendLine(
                                    $@"        await {_model.Id}.{item.Id}.Set(""{item.Id}"");"
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Build Code

        #region Build Footer

        /// <summary>
        /// Construction del footer de las pruebas
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string BuildFooter()
        {
            try
            {
                StringBuilder strFooter = new StringBuilder();

                strFooter.AppendLine(
                    $@"///IMPORTANTE----------------------------------------------------"
                );
                strFooter.AppendLine($@"///Cierre de la prueba");

                strFooter.AppendLine("    }");
                strFooter.AppendLine("    catch (ex)");
                strFooter.AppendLine("    {");
                strFooter.AppendLine("        ///Envió al log, información del error ocurrido");
                strFooter.AppendLine("        await _uth.logErr(ex.message);");
                strFooter.AppendLine("        ///Envió a la consola información del error");
                strFooter.AppendLine("        console.log(ex.message);");
                strFooter.AppendLine("    }");
                strFooter.AppendLine("});");

                strFooter.AppendLine(
                    $@"///--------------------------------------------------------------"
                );

                return strFooter.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error en el footer. {ex.Message}");
            }
        }

        #endregion Build Footer
    }
}
