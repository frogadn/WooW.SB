using System;
using System.Collections.Generic;
using System.Text;
using WooW.Core;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Helpers.GeneratorsHelpers;

namespace WooW.SB.BlazorGenerator.BlazorProjectFiles.ProjectFiles
{
    public class WoProgramClassGenerator
    {
        #region Instancias singleton

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Contiene la clase generada.
        /// </summary>
        private StringBuilder _finalClass = new StringBuilder();

        /// <summary>
        /// bandera que indica si la clase generada es para una prueba unitaria o una integral.
        /// </summary>
        private bool _isUnit = false;

        #endregion Variables globales


        #region Contructores

        /// <summary>
        /// Constructor proincipal de la clase.
        /// </summary>
        public WoProgramClassGenerator(bool isUnit)
        {
            _isUnit = isUnit;
        }

        #endregion Contructores


        #region Identación

        /// <summary>
        /// Nivel de identación para grupos internos y controles.
        /// </summary>
        private int _identLevel = 0;

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de la clase;
        /// </summary>
        private String _identClass = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado de las propiedades;
        /// </summary>
        private String _identMethodsAndProperties = "";

        /// <summary>
        /// Tiene los tabs requeridos para ajustar el apartado del código dentro de los métodos;
        /// </summary>
        private String _identCode = "";

        /// <summary>
        /// Calcula los niveles de identación con ayuda del helper.
        /// </summary>
        private void CalculateIdentSpaces()
        {
            _identLevel++;
            _identClass = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identMethodsAndProperties = FormatClassHelper.Ident(_identLevel);
            _identLevel++;
            _identCode = FormatClassHelper.Ident(_identLevel);
        }

        #endregion Identación


        #region Metodo principal

        /// <summary>
        /// Metodo principal que retorna el codigo para la clase de program
        /// </summary>
        /// <returns></returns>
        public string GetCode(
            List<string> models,
            List<string> reports,
            List<string> lists,
            List<string> slaves,
            List<string> oDataReports,
            string blazorType,
            bool onlyList,
            bool isReport
        )
        {
            _finalClass.Clear();

            _finalClass.AppendLine(GetCodeForLogin());

            if (_isUnit)
            {
                if (isReport)
                {
                    _finalClass.AppendLine(GetCodeForUnit(blazorType));
                }
                else
                {
                    _finalClass.AppendLine(GetCodeForUnit(blazorType, onlyList));
                }
            }
            else
            {
                _finalClass.AppendLine(GetCodeForIntegral(models, reports, lists));
            }

            if (slaves != null)
            {
                _finalClass.AppendLine($@"/// Dependencias de las slaves");
                foreach (var slave in slaves)
                {
                    _finalClass.AppendLine(
                        $@"
builder.Services.AddScoped<{slave}Controls>();
builder.Services.AddScoped<{slave}ScriptsUser>();
builder.Services.AddScoped<{slave}TransitionSettings>();"
                    );
                }
            }

            if (oDataReports.Count > 0)
            {
                _finalClass.AppendLine($@"/// Dependencias de los reportes oData");
                foreach (var oDataReport in oDataReports)
                {
                    _finalClass.AppendLine(
                        $@"
builder.Services.AddScoped<{oDataReport}ReportControls>();
builder.Services.AddScoped<{oDataReport}ReportLayoutControls>();
builder.Services.AddScoped<{oDataReport}ReportLayoutScriptsUser>();"
                    );
                }
            }

            return _finalClass.ToString();
        }

        #endregion Metodo principal

        #region Build code

        /// <summary>
        /// Construlle el codigo para la inyeccion de dependencias del login
        /// </summary>
        /// <returns></returns>
        private string GetCodeForLogin()
        {
            return $@"
/// Dependencias para el login
builder.Services.AddScoped<WoLoginControls>();
builder.Services.AddScoped<LoginScriptsUser>();";
        }

        private string GetCodeForUnit(string blazorType, bool onlyList)
        {
            StringBuilder strDependencies = new StringBuilder();

            strDependencies.AppendLine($@"/// Dependencias para el modelo de la prueba unitaria");

            if (!onlyList)
            {
                strDependencies.AppendLine(
                    $@"
/// Formulario
builder.Services.AddScoped<{blazorType}UnitModelControls>();
builder.Services.AddScoped<{blazorType}UnitModelScriptsUser>();
builder.Services.AddScoped<{blazorType}UnitModelTransitionSettings>();"
                );
            }

            strDependencies.AppendLine(
                $@"
/// Lista
builder.Services.AddScoped<{blazorType}UnitModelGridListControls>();
builder.Services.AddScoped<{blazorType}UnitModelGridListScriptsUser>();"
            );

            return strDependencies.ToString();
        }

        private string GetCodeForIntegral(
            List<string> models,
            List<string> reports,
            List<string> lists
        )
        {
            StringBuilder strIntancesForModels = new StringBuilder();

            if (models != null)
            {
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                foreach (string modelName in models)
                {
                    Modelo findModel = woProjectDataHelper.GetMainModel(modelName);

                    if (findModel != null)
                    {
                        if (
                            findModel.TipoModelo != WoTypeModel.Kardex
                            && findModel.TipoModelo != WoTypeModel.Control
                            && findModel.TipoModelo != WoTypeModel.Configuration
                            && findModel.TipoModelo != WoTypeModel.View
                        )
                        {
                            strIntancesForModels.AppendLine(
                                $@"
/// Formulario
builder.Services.AddScoped<{modelName}Controls>();
builder.Services.AddScoped<{modelName}ScriptsUser>();
builder.Services.AddScoped<{modelName}TransitionSettings>();"
                            );
                        }

                        strIntancesForModels.AppendLine(
                            $@"
/// Lista
builder.Services.AddScoped<{modelName}GridListControls>();
builder.Services.AddScoped<{modelName}GridListScriptsUser>();"
                        );
                    }
                }
            }

            if (lists != null)
            {
                foreach (string list in lists)
                {
                    strIntancesForModels.AppendLine(
                        $@"
/// Lista
builder.Services.AddScoped<{list}GridListControls>();
builder.Services.AddScoped<{list}GridListScriptsUser>();"
                    );
                }
            }

            if (reports != null)
            {
                foreach (string report in reports)
                {
                    strIntancesForModels.AppendLine(
                        $@"
/// Reporte
builder.Services.AddScoped<{report}ReportControls>();
builder.Services.AddScoped<{report}LayoutControls>();
builder.Services.AddScoped<{report}LayoutScriptsUser>();"
                    );
                }
            }

            return strIntancesForModels.ToString();
        }

        #endregion Build code

        #region Build report code

        private string GetCodeForUnit(string blazorType)
        {
            StringBuilder strDependencies = new StringBuilder();

            strDependencies.AppendLine($@"/// Dependencias para el modelo de la prueba unitaria");

            strDependencies.AppendLine(
                $@"
/// Reporte
builder.Services.AddScoped<{blazorType}UnitReportControls>();
builder.Services.AddScoped<Report0LayoutControls>();
builder.Services.AddScoped<Report0LayoutScriptsUser>();"
            );

            return strDependencies.ToString();
        }

        #endregion Build report code
    }
}
