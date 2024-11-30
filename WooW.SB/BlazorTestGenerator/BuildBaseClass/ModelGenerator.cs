using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using WooW.SB.Config;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorTestGenerator.BuildBaseClass
{
    public class ModelGenerator
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Método principal

        /// <summary>
        /// Método principal para generar la clase del modelo
        /// </summary>
        /// <param name="model"></param>
        public void BuildModelClass(Modelo model)
        {
            try
            {
                StringBuilder strModelClass = new StringBuilder();

                string pathFormDesign = $@"{_project.DirFormDesign}\{model.Id}.json";
                string rawJson = WoDirectory.ReadFile(pathFormDesign);

                WoContainer woContainer = JsonConvert.DeserializeObject<WoContainer>(rawJson);

                strModelClass.AppendLine(BuildImports(woContainer));
                strModelClass.AppendLine(BuildMainClass(woContainer));
                strModelClass.AppendLine(BuildSubClassModel(woContainer));

                string classPath =
                    $@"{_project.DirProyectData}\Test\TestCafe\Common\{model.Id}Model.ts";

                if (File.Exists(classPath))
                {
                    WoDirectory.DeleteFile(classPath);
                }

                WoDirectory.WriteFile(path: classPath, data: strModelClass.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Método principal


        #region Build Imports

        /// <summary>
        /// Lista de los imports para la clase del modelo para las pruebas
        /// </summary>
        private StringBuilder _strImports = new StringBuilder();

        /// <summary>
        /// Agrega los imports en función de los controles del modelo
        /// </summary>
        private string BuildImports(WoContainer model)
        {
            try
            {
                _strImports.Clear();

                BuildImportsRec(model);

                _strImports.AppendLine($@"import {{ Selector }} from 'testcafe';");

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
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        string import = string.Empty;

                        if (item.Control == "Decimal")
                        {
                            import =
                                "import { WoTextEdit } from '../woow.testcafe.typescript/WoComponents/WoTextEdit';";
                        }
                        else if (
                            item.Control == "Text"
                            || item.Control == "byte[]"
                            || item.Control == "Memo"
                        )
                        {
                            import =
                                "import { WoTextEdit } from '../woow.testcafe.typescript/WoComponents/WoTextEdit';";
                        }
                        else if (item.Control == "Spin")
                        {
                            import =
                                "import { WoSpinEdit } from '../woow.testcafe.typescript/WoComponents/WoSpinEdit';";
                        }
                        else if (item.Control == "LookUp")
                        {
                            import =
                                "import { WoLookUpEdit } from '../woow.testcafe.typescript/WoComponents/WoLookUpEdit';";
                        }
                        else if (item.Control == "LookUpDialog")
                        {
                            import =
                                "import { WoLookUpDialogEdit } from '../woow.testcafe.typescript/WoComponents/WoLookUpDialogEdit';";
                        }
                        else if (
                            item.Control == "WoState"
                            || item.Control == "EnumString"
                            || item.Control == "EnumInt"
                        )
                        {
                            import =
                                "import { WoComboEdit } from '../woow.testcafe.typescript/WoComponents/WoComboEdit';";
                        }
                        else if (item.Control == "Date")
                        {
                            import =
                                "import { WoDateEdit } from '../woow.testcafe.typescript/WoComponents/WoDateEdit';";
                        }
                        else if (item.Control == "Bool")
                        {
                            import =
                                "import { WoCheckEdit } from '../woow.testcafe.typescript/WoComponents/WoCheckEdit';";
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
        /// Codigo con los atributos de la clase principal
        /// </summary>
        private StringBuilder _strMainClassAttributes = new StringBuilder();

        /// <summary>
        /// Codigo del constructor de la clase principal
        /// </summary>
        private StringBuilder _strMainClassConstructor = new StringBuilder();

        /// <summary>
        /// Construye la clase principal del modelo a probar
        /// </summary>
        /// <returns></returns>
        private string BuildMainClass(WoContainer mainContainer)
        {
            try
            {
                _strMainClass.Clear();

                _strMainClass.AppendLine($@"export class {mainContainer.ModelId}Model");
                _strMainClass.AppendLine("{");

                BuildMainClassRec(mainContainer);

                _strMainClass.AppendLine(_strMainClassAttributes.ToString());

                _strMainClass.AppendLine("    constructor()");
                _strMainClass.AppendLine("    {");

                _strMainClass.AppendLine(_strMainClassConstructor.ToString());

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
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        string attribute = string.Empty;
                        string constructorCode = string.Empty;

                        if (
                            item.Control == "Text"
                            || item.Control == "Decimal"
                            || item.Control == "byte[]"
                            || item.Control == "Memo"
                            || item.Control == "Spin"
                            || item.Control == "LookUp"
                            || item.Control == "LookUpDialog"
                            || item.Control == "WoState"
                            || item.Control == "EnumString"
                            || item.Control == "EnumInt"
                            || item.Control == "Date"
                            || item.Control == "Bool"
                        )
                        {
                            attribute = $@"    public {item.Id} = new {item.Id}();";
                        }

                        if (!_strMainClassAttributes.ToString().Contains(attribute))
                        {
                            _strMainClassAttributes.AppendLine(attribute);
                        }
                        if (!_strMainClassConstructor.ToString().Contains(constructorCode))
                        {
                            _strMainClassConstructor.AppendLine(constructorCode);
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
        /// Codigo con las sub clases de la clase principal
        /// </summary>
        private StringBuilder _strSubClassCode = new StringBuilder();

        /// <summary>
        /// Construye las sub clases que usa la clase principal para el test
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
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Recorre el formulario y va construyendo las clases por componente para la clase principal
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
                    }
                }

                if (mainContainer.ItemsCol != null)
                {
                    foreach (WoItem item in mainContainer.ItemsCol)
                    {
                        StringBuilder strSubClassComponent = new StringBuilder();

                        if (
                            item.Control == "Text"
                            || item.Control == "byte[]"
                            || item.Control == "Memo"
                        )
                        {
                            strSubClassComponent.AppendLine($@"export class {item.Id}");
                            strSubClassComponent.AppendLine("{");

                            strSubClassComponent.AppendLine($@"    private _txt{item.Id};");

                            strSubClassComponent.AppendLine("    constructor()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            this._txt{item.Id} = new WoTextEdit('txt{item.Id}');"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al inicializar la clase {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Get()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine($@"            return null;");
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al recuperar la información de {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Set(newValue: string)");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            await this._txt{item.Id}.ReplaceText(newValue);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al asignar el valor ${{newValue}} a {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("}");
                        }
                        else if (item.Control == "LookUp")
                        {
                            strSubClassComponent.AppendLine($@"export class {item.Id}");
                            strSubClassComponent.AppendLine("{");

                            strSubClassComponent.AppendLine($@"    private _lkp{item.Id};");

                            strSubClassComponent.AppendLine("    constructor()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            this._lkp{item.Id} = new WoLookUpEdit('lkp{item.Id}');"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al inicializar la clase {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Get()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            const inputField = Selector('#lkp{item.Id}');"
                            );
                            strSubClassComponent.AppendLine(
                                $@"            const fieldValue = await inputField.value;"
                            );
                            strSubClassComponent.AppendLine($@"            return fieldValue;");
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al recuperar la información de {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Set(newValue: string)");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            await this._lkp{item.Id}.SelectItem(newValue);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al asignar el valor ${{newValue}} a {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("}");
                        }
                        else if (item.Control == "LookUpDialog")
                        {
                            strSubClassComponent.AppendLine($@"export class {item.Id}");
                            strSubClassComponent.AppendLine("{");
                            strSubClassComponent.AppendLine($@"    private _lkd{item.Id};");

                            strSubClassComponent.AppendLine("    constructor()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            this._lkd{item.Id} = new WoLookUpDialogEdit('lkd{item.Id}');"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al inicializar la clase {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Get()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            const inputField = Selector('#lkd{item.Id}');"
                            );
                            strSubClassComponent.AppendLine(
                                $@"            const fieldValue = await inputField.value;"
                            );
                            strSubClassComponent.AppendLine($@"            return fieldValue;");
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al recuperar la información de {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Set(newValue: string)");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            await this._lkd{item.Id}.SelectItem(newValue);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al asignar el valor ${{newValue}} a {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("}");
                        }
                        else if (
                            item.Control == "WoState"
                            || item.Control == "EnumString"
                            || item.Control == "EnumInt"
                        )
                        {
                            strSubClassComponent.AppendLine($@"export class {item.Id}");
                            strSubClassComponent.AppendLine("{");

                            strSubClassComponent.AppendLine($@"    private _cmb{item.Id};");

                            strSubClassComponent.AppendLine("    constructor()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            this._cmb{item.Id} = new WoComboEdit('cmb{item.Id}');"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al inicializar la clase {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Get()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            const inputField = Selector('#cmb{item.Id}');"
                            );
                            strSubClassComponent.AppendLine(
                                $@"            const fieldValue = await inputField.value;"
                            );
                            strSubClassComponent.AppendLine($@"            return fieldValue;");
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al recuperar la información de {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Set(nameItem: string)");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            await this._cmb{item.Id}.SelectItem(nameItem);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al asignar el valor ${{newValue}} a {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("}");
                        }
                        else if (item.Control == "Date")
                        {
                            strSubClassComponent.AppendLine($@"export class {item.Id}");
                            strSubClassComponent.AppendLine("{");

                            strSubClassComponent.AppendLine($@"    private _dte{item.Id};");

                            strSubClassComponent.AppendLine("    constructor()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            this._dte{item.Id} = new WoDateEdit('dte{item.Id}');"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al inicializar la clase {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Get()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            const inputField = Selector('#dte{item.Id}');"
                            );
                            strSubClassComponent.AppendLine(
                                $@"            const fieldValue = await inputField.value;"
                            );
                            strSubClassComponent.AppendLine($@"            return fieldValue;");
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al recuperar la información de {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Set(value: string)");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            await this._dte{item.Id}.SetDate(value);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al asignar el valor ${{newValue}} a {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("}");
                        }
                        else if (item.Control == "Bool")
                        {
                            strSubClassComponent.AppendLine($@"export class {item.Id}");
                            strSubClassComponent.AppendLine("{");

                            strSubClassComponent.AppendLine($@"    private _chk{item.Id};");

                            strSubClassComponent.AppendLine("    constructor()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            this._chk{item.Id} = new WoCheckEdit('chk{item.Id}');"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al inicializar la clase {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Get()");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            const checkbox = Selector('#chk{item.Id}');"
                            );
                            strSubClassComponent.AppendLine(
                                $@"            const isChecked = await checkbox.checked;"
                            );
                            strSubClassComponent.AppendLine($@"            return isChecked;");
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al recuperar la información de {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("    async Set(value: string)");
                            strSubClassComponent.AppendLine("    {");
                            strSubClassComponent.AppendLine("        try");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine($@"            if(value)");
                            strSubClassComponent.AppendLine($@"            {{");
                            strSubClassComponent.AppendLine(
                                $@"                var status = await this.Get();"
                            );
                            strSubClassComponent.AppendLine($@"                if(!status)");
                            strSubClassComponent.AppendLine($@"                {{");
                            strSubClassComponent.AppendLine(
                                $@"                    await this._chk{item.Id}.Toggle();"
                            );
                            strSubClassComponent.AppendLine($@"                }}");
                            strSubClassComponent.AppendLine($@"            }}");
                            strSubClassComponent.AppendLine($@"            else");
                            strSubClassComponent.AppendLine($@"            {{");
                            strSubClassComponent.AppendLine($@"                if(!value)");
                            strSubClassComponent.AppendLine($@"                {{");
                            strSubClassComponent.AppendLine(
                                $@"                    var status = await this.Get();"
                            );
                            strSubClassComponent.AppendLine($@"                    if (status)");
                            strSubClassComponent.AppendLine($@"                    {{");
                            strSubClassComponent.AppendLine(
                                $@"                        await this._chk{item.Id}.UnToggle();"
                            );
                            strSubClassComponent.AppendLine($@"                    }}");
                            strSubClassComponent.AppendLine($@"                }}");
                            strSubClassComponent.AppendLine($@"            }}");
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("        catch (ex: any)");
                            strSubClassComponent.AppendLine("        {");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al asignar el valor ${{newValue}} a {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine("        }");
                            strSubClassComponent.AppendLine("    }");

                            strSubClassComponent.AppendLine("}");
                        }
                        else if (item.Control == "Decimal" || item.Control == "Spin")
                        {
                            strSubClassComponent.AppendLine($@"export class {item.Id}");
                            strSubClassComponent.AppendLine("{");

                            strSubClassComponent.AppendLine($@"    private _spe{item.Id};");

                            strSubClassComponent.AppendLine($@"    constructor()");
                            strSubClassComponent.AppendLine($@"    {{");
                            strSubClassComponent.AppendLine($@"        try");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            this._spe{item.Id} = new WoSpinEdit('spe{item.Id}');"
                            );
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"        catch (ex: any)");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al inicializar la clase {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"    }}");

                            strSubClassComponent.AppendLine($@"    async Get()");
                            strSubClassComponent.AppendLine($@"    {{");
                            strSubClassComponent.AppendLine($@"        try");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            const inputField = Selector('#spe{item.Id}');"
                            );
                            strSubClassComponent.AppendLine(
                                $@"            const fieldValue = await inputField.value;"
                            );
                            strSubClassComponent.AppendLine($@"            return fieldValue;");
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"        catch(ex: any)");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al recuperar el valor de {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"    }}");

                            strSubClassComponent.AppendLine($@"    async Up(value: string)");
                            strSubClassComponent.AppendLine($@"    {{");
                            strSubClassComponent.AppendLine($@"        try");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            for (let i = 0; i < value; i++)"
                            );
                            strSubClassComponent.AppendLine($@"            {{");
                            strSubClassComponent.AppendLine(
                                $@"                await this._spe{item.Id}.Up();"
                            );
                            strSubClassComponent.AppendLine($@"            }}");
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"        catch(ex: any)");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al incrementar el valor en {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"    }}");

                            strSubClassComponent.AppendLine($@"    async Down(value: string)");
                            strSubClassComponent.AppendLine($@"    {{");
                            strSubClassComponent.AppendLine($@"        try");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            for (let i = 0; i < value; i++)"
                            );
                            strSubClassComponent.AppendLine($@"            {{");
                            strSubClassComponent.AppendLine(
                                $@"                await this._spe{item.Id}.Down();"
                            );
                            strSubClassComponent.AppendLine($@"            }}");
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"        catch(ex: any)");
                            strSubClassComponent.AppendLine($@"        {{");
                            strSubClassComponent.AppendLine(
                                $@"            throw new Error(`Error al reducir el valor en {item.Id}. Error: ${{ex.stack}}`);"
                            );
                            strSubClassComponent.AppendLine($@"        }}");
                            strSubClassComponent.AppendLine($@"    }}");

                            strSubClassComponent.AppendLine("}");
                        }

                        if (!_strSubClassCode.ToString().Contains(strSubClassComponent.ToString()))
                        {
                            _strSubClassCode.AppendLine(strSubClassComponent.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Ocurrió un error al recorrer el formulario para la creación de las sub clases para el test. {ex.Message}"
                );
            }
        }

        #endregion Build sub clases del modelo
    }
}
