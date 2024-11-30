using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.BlazorExtencionsAndPackages
{
    public partial class WoProjectDataHelper
    {
        #region Recuperación de los botones custom

        /// <summary>
        /// Recupera la lista de botones custom de todos los modelos de extension y
        /// el modelo principal
        /// </summary>
        /// <returns></returns>
        public List<WoCustomButtonProperties> GetCustomButtons(string modelName)
        {
            try
            {
                _customButtons.Clear();
                _readedProyects.Clear();

                SearchCustomButtons(_project, modelName);

                return _customButtons;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al recuperar la lista de botones. {ex.Message}");
            }
        }

        #endregion Recuperación de los botones custom

        #region Búsqueda de los botones custom

        /// <summary>
        /// Lista completa de los botones custom
        /// </summary>
        List<WoCustomButtonProperties> _customButtons = new List<WoCustomButtonProperties>();

        /// <summary>
        /// Busca los botones custom en todas las extensiones y modelos.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SearchCustomButtons(Proyecto project, string modelName)
        {
            try
            {
                string projectName = GetWWSBName(project);
                if (!_readedProyects.Contains(projectName))
                {
                    _readedProyects.Add(projectName);

                    string buttonsPath =
                        $"{project.DirFormDesignUserCode}\\{modelName}_proj\\{modelName}CustomButtons.json";

                    if (File.Exists(buttonsPath))
                    {
                        string jsonRaw = WoDirectory.ReadFile(buttonsPath);
                        List<WoCustomButtonProperties> customButtons =
                            JsonConvert.DeserializeObject<List<WoCustomButtonProperties>>(jsonRaw);

                        _customButtons.AddRange(customButtons.OrderBy(button => button.Index));
                    }

                    foreach (Paquete pack in project.Paquetes)
                    {
                        string[] pathBaseDirCol = project.Dir.Split("\\");
                        string pathPacks = string.Empty;
                        for (int i = 0; i < pathBaseDirCol.Count() - 1; i++)
                        {
                            pathPacks += $"{pathBaseDirCol[i]}\\";
                        }

                        string path = $"{pathPacks}{pack.Archivo}";
                        Proyecto subProyect = new Proyecto();
                        subProyect.Load(path);

                        SearchCustomButtons(subProyect, modelName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al buscar los botones. {ex.Message}");
            }
        }

        #endregion Búsqueda de los botones custom
    }
}
