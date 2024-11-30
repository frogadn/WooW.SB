using System;
using System.Text;
using WooW.SB.Config;

namespace WooW.SB.BlazorTestGenerator.Helpers
{
    public class BuildTestContextJs
    {
        #region Instancias singleton

        /// <summary>
        /// Instancia con toda la información del proyecto sobre del que se esta trabajando
        /// </summary>
        public Proyecto proyecto { get; set; }

        #endregion Instancias singleton

        #region Atributos

        /// <summary>
        /// Código del header para el contexto del syntax
        /// </summary>
        private StringBuilder _strContextHeader = new StringBuilder();

        /// <summary>
        /// Código del footer para el contexto del syntax
        /// </summary>
        private StringBuilder _strContextFooter = new StringBuilder();

        /// <summary>
        /// Nombre del modelo del que se va a generar el contexto
        /// </summary>
        private string _modelName = string.Empty;

        #endregion Atributos


        #region Constructor

        /// <summary>
        /// Constructor principal de la clase
        /// </summary>
        public BuildTestContextJs(string modelName)
        {
            _modelName = modelName;
        }

        #endregion Constructor


        #region Método principal

        /// <summary>
        /// Método principal que se ocupa de orquestar la creación del contexto del syntax
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (string header, string footer) GetContext()
        {
            try
            {
                _strContextHeader.Clear();
                _strContextFooter.Clear();

                BuildHeader();

                return (header: _strContextHeader.ToString(), footer: _strContextFooter.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al construir el contexto para el syntax. {ex.Message}"
                );
            }
        }

        #endregion Método principal


        #region Construcción del header

        /// <summary>
        /// Construimos el header
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildHeader()
        {
            try
            {
                _strContextHeader.AppendLine(
                    $@"
import {{ {_modelName} }} from '../../../TestCafe/{_modelName}.js';
import {{ Formulario{_modelName} }} from '../../../TestCafe/Formulario{_modelName}.js';
import {{ Uth }} from '../../../Test/Libreria/WoTestCafe/Tools/Helpers/Uth.js';
"
                );

                _strContextHeader.AppendLine(
                    $@"fixture `CfgMoneda`.page`https://localhost:7208/Index`;"
                );
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al construir el header. {ex.Message}");
            }
        }

        #endregion Construcción del header
    }
}
