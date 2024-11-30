using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.BuildBaseClass
{
    public class FormModelGenerator
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Método principal

        /// <summary>
        /// Método principal para la creación de la clase de formulario
        /// Orquesta todo el apartado de la creación
        /// </summary>
        public void BuildFormModelClass(Modelo model)
        {
            try
            {
                StringBuilder strFormClass = new StringBuilder();

                string pathFormDesign = $@"{_project.DirFormDesign}\{model.Id}.json";
                string rawJson = WoDirectory.ReadFile(pathFormDesign);

                WoContainer woContainer = JsonConvert.DeserializeObject<WoContainer>(rawJson);

                strFormClass.AppendLine(BuildImports(woContainer));
                strFormClass.AppendLine(BuildMainClass(woContainer));
                strFormClass.AppendLine(BuildSubClassModel(woContainer));

                string classPath =
                    $@"{_project.DirProyectData}\Test\TestCafe\Common\{model.Id}Form.ts";

                if (File.Exists(classPath))
                {
                    WoDirectory.DeleteFile(classPath);
                }

                WoDirectory.WriteFile(path: classPath, data: strFormClass.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al momento de ejecutar la creación del la clase del formulario para pruebas. {ex.Message}"
                );
            }
        }

        #endregion Método principal


        #region Build Imports

        /// <summary>
        /// Lista de los imports para la clase del modelo para las pruebas
        /// </summary>
        private StringBuilder _strImports = new StringBuilder();

        /// <summary>
        /// Import de los formularios de las slaves
        /// </summary>
        private StringBuilder _strImportsSlave = new StringBuilder();

        /// <summary>
        /// Agrega los imports en función de los controles del modelo
        /// </summary>
        private string BuildImports(WoContainer model)
        {
            try
            {
                _strImports.Clear();
                _strImportsSlave.Clear();

                _strImports.AppendLine(
                    "import { WoButtonEdit } from '../woow.testcafe.typescript/WoComponents/WoButtonEdit.js';"
                );
                BuildImportsRec(model);

                _strImports.AppendLine(_strImportsSlave.ToString());

                return _strImports.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Recorremos el diseño del formulario para poder agregar los imports para los
        /// componentes presentes
        /// </summary>
        /// <param name="mainContainer"></param>
        private void BuildImportsRec(WoContainer mainContainer)
        {
            try
            {
                if (mainContainer.ContainersCol != null)
                {
                    foreach (WoContainer subContainer in mainContainer.ContainersCol)
                    {
                        WoContainer container = subContainer;
                        BuildImportsRec(container);

                        if (container.TypeContainer == eTypeContainer.FormTab)
                        {
                            string import =
                                "import { WoTabsEdit } from '../wotestcafe/WoComponents/WoTabsEdit.js';";
                            if (!_strImports.ToString().Contains(import))
                            {
                                _strImports.AppendLine(import);
                            }
                        }
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        string import = string.Empty;

                        if (item.Control == "Label" && item.Id == "Controles")
                        {
                            import =
                                "import { WoToolBar } from '../wotestcafe/WoComponents/WoToolBar.js';";
                        }
                        else if (item.Control == "Slave")
                        {
                            import =
                                "import { WoRowSlave } from '../wotestcafe/WoComponents/WoRowSlaveEdit.js';"
                                + "\r\nimport { WoSlave } from '../wotestcafe/WoComponents/WoSlave.js';";

                            string importModelSlave =
                                $@"import {{ {item.SlaveModelId}Model }} from './{item.SlaveModelId}Model.js';";
                            string importFormSlave =
                                $@"import {{ {item.SlaveModelId}Form }} from './{item.SlaveModelId}Form.js';";

                            if (!_strImportsSlave.ToString().Contains(importModelSlave))
                            {
                                _strImportsSlave.AppendLine(importModelSlave);
                            }
                            if (!_strImportsSlave.ToString().Contains(importFormSlave))
                            {
                                _strImportsSlave.AppendLine(importFormSlave);
                            }
                        }

                        if (!_strImports.ToString().Contains(import))
                        {
                            _strImports.AppendLine(import);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al recorrer el formulario para la creación de los imports. {ex.Message}"
                );
            }
        }

        #endregion Build Imports

        #region Build clase principal del modelo

        /// <summary>
        /// Codigo de la clase principal del fichero
        /// </summary>
        private StringBuilder _strMainClass = new StringBuilder();

        /// <summary>
        /// Codigo de los atributos para los contenedores
        /// </summary>
        private StringBuilder _strMainClassContainers = new StringBuilder();

        /// <summary>
        /// Codigo con los atributos de la clase principal
        /// </summary>
        private StringBuilder _strMainClassAttributes = new StringBuilder();

        /// <summary>
        /// Codigo del constructor de la clase principal
        /// </summary>
        private StringBuilder _strmainClassConstructor = new StringBuilder();

        /// <summary>
        /// Construye la clase principal del modelo a probar
        /// </summary>
        /// <returns></returns>
        private string BuildMainClass(WoContainer mainContainer)
        {
            try
            {
                _strMainClass.Clear();

                _strMainClass.AppendLine($@"export class {mainContainer.ModelId}Form");
                _strMainClass.AppendLine("{");

                BuildMainClassRec(mainContainer);

                _strMainClass.AppendLine(_strMainClassAttributes.ToString());

                _strMainClass.AppendLine("    constructor()");
                _strMainClass.AppendLine("    {");

                _strMainClass.AppendLine(_strmainClassConstructor.ToString());

                _strMainClass.AppendLine("    }");

                _strMainClass.AppendLine("}");

                return _strMainClass.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Recorre el formulario para ir construyendo la clase
        /// </summary>
        private void BuildMainClassRec(WoContainer mainContainer)
        {
            try
            {
                if (mainContainer.ContainersCol != null)
                {
                    foreach (WoContainer subContainer in mainContainer.ContainersCol)
                    {
                        WoContainer container = subContainer;
                        BuildMainClassRec(container);

                        if (container.TypeContainer == eTypeContainer.FormTab)
                        {
                            string attribute = $@"    {container.Id};";
                            string constructorCode =
                                $@"        this.{container.Id} = new Tab{container.Id}();";

                            if (!_strMainClassAttributes.ToString().Contains(attribute))
                            {
                                _strMainClassAttributes.AppendLine(attribute);
                            }
                            if (!_strmainClassConstructor.ToString().Contains(constructorCode))
                            {
                                _strmainClassConstructor.AppendLine(constructorCode);
                            }
                        }
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        string attribute = string.Empty;
                        string constructorCode = string.Empty;

                        if (item.Control == "Label" && item.Id == "Controles")
                        {
                            attribute = $@"    {item.Id};";
                            constructorCode = $@"        this.{item.Id} = new WoToolBar();";
                        }
                        else if (item.Control == "Slave")
                        {
                            attribute = $@"    {item.SlaveModelId}Slave;";
                            constructorCode =
                                $@"        this.{item.SlaveModelId}Slave = new {item.SlaveModelId}Slave();";
                        }

                        if (!_strMainClassAttributes.ToString().Contains(attribute))
                        {
                            _strMainClassAttributes.AppendLine(attribute);
                        }
                        if (!_strmainClassConstructor.ToString().Contains(constructorCode))
                        {
                            _strmainClassConstructor.AppendLine(constructorCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al recorrer el formulario para crear la clase principal. {ex.Message}"
                );
            }
        }

        #endregion Build clase principal del modelo

        #region Build sub clases del modelo

        /// <summary>
        /// Código con las sub clases del modelo
        /// </summary>
        private StringBuilder _strSubClassCode = new StringBuilder();

        /// <summary>
        /// Construye las sub clases para el modelo del formulario
        /// </summary>
        /// <returns></returns>
        private string BuildSubClassModel(WoContainer mainContainer)
        {
            try
            {
                _strSubClassCode.Clear();

                BuildSubClassModelRec(mainContainer);

                return _strSubClassCode.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error en la creación de las sub clases del formulario. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Recorre recursiva mente el contenido del formulario y arma el código para la creación de clases
        /// </summary>
        /// <param name="mainContainer"></param>
        /// <exception cref="Exception"></exception>
        private void BuildSubClassModelRec(WoContainer mainContainer)
        {
            try
            {
                if (mainContainer.ContainersCol != null)
                {
                    foreach (WoContainer subContainer in mainContainer.ContainersCol)
                    {
                        WoContainer container = subContainer;
                        BuildSubClassModelRec(container);

                        if (container.TypeContainer == eTypeContainer.FormTab)
                        {
                            StringBuilder strInternalClass = new StringBuilder();

                            strInternalClass.AppendLine($@"export class Tab{container.Id}");
                            strInternalClass.AppendLine("{");
                            strInternalClass.AppendLine($@"    _tab{container.Id};");
                            strInternalClass.AppendLine($@"    constructor()");
                            strInternalClass.AppendLine("    {");
                            strInternalClass.AppendLine(
                                $@"        this._tab{container.Id} = new WoTabsEdit('{container.Id}');"
                            );
                            strInternalClass.AppendLine("    }");
                            strInternalClass.AppendLine("    async Select()");
                            strInternalClass.AppendLine("    {");
                            strInternalClass.AppendLine(
                                $@"        await this._tab{container.Id}.Click('{container.Etiqueta}');"
                            );
                            strInternalClass.AppendLine("    }");
                            strInternalClass.AppendLine("}");

                            _strSubClassCode.AppendLine(strInternalClass.ToString());
                        }
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        if (item.Control == "Slave")
                        {
                            StringBuilder strSlaveClass = new StringBuilder();

                            strSlaveClass.AppendLine($@"export class {item.SlaveModelId}Slave");
                            strSlaveClass.AppendLine("{");
                            strSlaveClass.AppendLine("    _btnNewSlave;");
                            strSlaveClass.AppendLine("    constructor()");
                            strSlaveClass.AppendLine("    {");
                            strSlaveClass.AppendLine(
                                $@"        this._btnNewSlave = new WoButtonEdit(""{item.SlaveModelId}-btnAdd"");"
                            );
                            strSlaveClass.AppendLine("    }");

                            strSlaveClass.AppendLine("    async NewSlave()");
                            strSlaveClass.AppendLine("    {");
                            strSlaveClass.AppendLine($@"        await this._btnNewSlave.Click();");
                            strSlaveClass.AppendLine(
                                $@"        return new WoSlave(new {item.SlaveModelId}Model, new {item.SlaveModelId}Form);"
                            );
                            strSlaveClass.AppendLine("    }");

                            strSlaveClass.AppendLine("    async GetRow(renglon)");
                            strSlaveClass.AppendLine("    {");
                            strSlaveClass.AppendLine(
                                $@"        return new WoRowSlave(""Departamento"", renglon, new {item.SlaveModelId}Model, new {item.SlaveModelId}Form);"
                            );
                            strSlaveClass.AppendLine("    }");
                            strSlaveClass.AppendLine("}");

                            _strSubClassCode.AppendLine(strSlaveClass.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recorrer el diseño del formulario. {ex.Message}");
            }
        }

        #endregion Build sub clases del modelo
    }
}
