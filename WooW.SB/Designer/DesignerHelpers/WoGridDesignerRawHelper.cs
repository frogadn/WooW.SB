using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WooW.Core;
using WooW.Core.Common.Exceptions;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.BlazorExtencionsAndPackages;
using WooW.SB.Config;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.ManagerDirectory;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoGridDesignerRawHelper
    {
        #region Instancias singleton

        /// <summary>
        /// Observador general del proyecto.
        /// Para enviar alertas e información al log.
        /// </summary>
        private WoLogObserver _observer = WoLogObserver.GetInstance();

        /// <summary>
        /// Proyecto sobre el que se esta trabajando.
        /// </summary>
        private Proyecto _project = Proyecto.getInstance();

        #endregion Instancias singleton


        #region Atributos

        /// <summary>
        /// Instancia de la clase con la meta data para la generación de la grid
        /// se re instancia en el método principal.
        /// </summary>
        private WoGridProperties _grid;

        /// <summary>
        /// Nombre del modelo del que se elaborara el grid.
        /// </summary>
        private string _modelName = string.Empty;

        /// <summary>
        /// Instancia del modelo del que se elaborara el grid.
        /// </summary>
        private Modelo _baseModel;

        /// <summary>
        /// Lista Solo de las extensiones del modelo
        /// </summary>
        private List<Modelo> _extencionModels = new List<Modelo>();

        /// <summary>
        /// Instancia del helper para la generación de los tipos de columnas.
        /// </summary>
        private WoDesignerTypeHelper _woDesignerTypeHelper;

        /// <summary>
        /// Indicador para cuando es una grid para un lookup
        /// o para cuando se genera una grid completamente fuera del proyecto
        /// (No necesariamente el modelo existe en el proyecto actual)
        /// </summary>
        //private bool _gridOutOfProject = false;

        #endregion Atributos


        #region Método principal

        /// <summary>
        /// Método principal para la generación de la meta data de la grid.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public WoGridProperties GetRawGrid(string modelName, bool isSlave)
        {
            _modelName = modelName;
            //_gridOutOfProject = gridOutOfProyect;

            _grid = new WoGridProperties()
            {
                ModelName = modelName,
                DirectModel = modelName,
                IsSlave = false,
                WoColumnPropertiesCol = new List<WoColumnProperties>(),
                LabelId = Etiqueta.ToId(modelName)
            };

            SearchModel();
            BuildColumns();

            _grid.LabelId = _baseModel.EtiquetaId;

            return _grid;
        }

        /// <summary>
        /// Recuperamos el diseño en raw de la grid con la data del proyecto actual
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="proyecto"></param>
        /// <param name="writeFile"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //public WoGridProperties BuildRawActualModelo(string modelName, bool writeFile = false)
        //{
        //    try
        //    {
        //        _modelName = modelName;

        //        _grid = new WoGridProperties()
        //        {
        //            ModelName = modelName,
        //            DirectModel = modelName,
        //            IsSlave = false,
        //            WoColumnPropertiesCol = new List<WoColumnProperties>(),
        //            LabelId = Etiqueta.ToId(modelName)
        //        };

        //        ChargeModel();

        //        Columns();

        //        _grid.LabelId = _baseModel.EtiquetaId;

        //        if (writeFile)
        //        {
        //            SaveFile(_grid, modelName);
        //        }

        //        return _grid;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(
        //            $@"Error al generar un diseño en raw para el grid {modelName}. {ex.Message}"
        //        );
        //    }
        //}

        /// <summary>
        /// Recuperamos el diseño en raw de la grid en función del proyecto que se recibe por parámetro
        /// </summary>
        /// <param name="project"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public WoGridProperties GetRawGridOfProject(
            Proyecto project,
            string modelName,
            bool blazorIntegral = false
        )
        {
            try
            {
                _modelName = modelName;

                _grid = new WoGridProperties()
                {
                    ModelName = modelName,
                    DirectModel = modelName,
                    IsSlave = false,
                    WoColumnPropertiesCol = new List<WoColumnProperties>(),
                    LabelId = Etiqueta.ToId(modelName)
                };

                ChargeModel(project, blazorIntegral);

                Columns();

                _grid.LabelId = _baseModel.EtiquetaId;

                return _grid;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al recuperar la grid en raw del proyecto. {ex.Message}"
                );
            }
        }

        #endregion Método principal


        #region Modelo

        /// <summary>
        /// Cargamos el modelo y las configuraciones principales
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ChargeModel(Proyecto project = null, bool blazorIntegral = false)
        {
            try
            {
                if (project != null)
                {
                    this._project = project;
                }

                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper(_project);

                if (blazorIntegral)
                {
                    _baseModel = woProjectDataHelper.GetMainModel(_modelName);
                }
                else
                {
                    _baseModel = woProjectDataHelper.GetActualModel(_modelName);
                    _grid.IsExtension = _baseModel.EsPaqueteExterno;
                }
                _grid.Proceso = _baseModel.ProcesoId;
                _grid.TypeModel = _baseModel.TipoModelo;
                _grid.SubTypeModel = _baseModel.SubTipoModelo;
                _grid.GridSelect = (_baseModel.TipoModelo == WoTypeModel.View) ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al cargar el modelo {_modelName}. {ex.Message}");
            }
        }

        /// <summary>
        /// Busca el modelo seleccionado en el proyecto.
        /// </summary>
        /// <exception cref="WoObserverException"></exception>
        private void SearchModel()
        {
            try
            {
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();

                _baseModel = woProjectDataHelper.GetMainModel(_modelName);

                _extencionModels = woProjectDataHelper.GetExtensions(_modelName);

                if (_baseModel == null)
                {
                    throw new WoObserverException(
                        _cantFindModel,
                        $@"El modelo {_modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio."
                    );
                }
                else
                {
                    if (woProjectDataHelper.ModelInProyect(_modelName))
                    {
                        Modelo findModel = woProjectDataHelper.GetActualModel(_modelName);
                        _grid.IsExtension = findModel.SubTipoModelo == WoSubTypeModel.Extension;
                    }

                    _grid.Proceso = _baseModel.ProcesoId;
                    _grid.TypeModel = _baseModel.TipoModelo;
                    _grid.SubTypeModel = _baseModel.SubTipoModelo;
                    _grid.GridSelect = (_baseModel.TipoModelo == WoTypeModel.View) ? false : true;
                }
            }
            catch (Exception ex)
            {
                throw new WoObserverException(
                    _cantFindModel,
                    $@"El modelo {_modelName} no existe en el proyecto, pruebe generar el proyecto o compilarlo en caso de haber realizado algún cambio. {ex.Message}"
                );
            }
        }

        #endregion Modelo

        #region Construcción de grid

        /// <summary>
        /// Recupera la columnas del modelo actual ordenadas desde el modelo y
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void Columns()
        {
            try
            {
                // Index de la columna para el diseño
                int index = 0;

                // Helper para el manejo de tipos de datos
                _woDesignerTypeHelper = new WoDesignerTypeHelper(_modelName);

                // Recuperación de las columnas ordenadas con el orden definido en el modelo
                List<ModeloColumna> columnsCol = _baseModel.Columnas.OrderBy(x => x.Orden).ToList();

                // Recorrido de las columnas
                foreach (ModeloColumna column in columnsCol)
                {
                    // Indicador para cuando es una referencia
                    bool isMasterReference = false;

                    // En caso de ser una columna del tipo de referencia, validamos que
                    // exista la referencia
                    if (column.TipoColumna == Core.WoTypeColumn.Reference)
                    {
                        // Valida la existencia de la referencia y actualiza el indicador
                        isMasterReference = IsMasterReference(column);
                    }

                    // Indicador e si la columna es visible
                    bool isVisible = IsVisuble(column, isMasterReference);

                    // Construimos el diseño de la columna
                    WoColumnProperties columnProperties = BuildColumn(column, index, isVisible);

                    // Actualiza las propiedades de las propiedades de la columna
                    columnProperties = UpdateColumn(columnProperties, column);

                    // Si la columna no es una slave la agrega a la lista de columnas
                    if (column.TipoControl != WoTypeControl.CollectionEditor)
                    {
                        _grid.WoColumnPropertiesCol.Add(columnProperties);
                    }

                    index++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al generar las columnas. {ex.Message}");
            }
        }

        /// <summary>
        /// Validamos si es una inferencia principal
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool IsMasterReference(ModeloColumna column)
        {
            try
            {
                // Indicador de si es o no referencia
                bool result = false;

                // Nombre del modelo al que se hace referencia
                string masterModelName = column.ModeloId;

                // Recuperación del modelo y sus extensiones de la referencia
                WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                List<Modelo> findModels = woProjectDataHelper.GetModelWithExtencions(
                    masterModelName
                );

                // Modelo que contiene el capo al que se realiza la referencia
                Modelo findModel = (
                    from model in findModels
                    where
                        model.Columnas.FirstOrDefault(column => column.Id == $@"{_modelName}Col")
                        != null
                    select model
                ).FirstOrDefault();

                // Columna de la slave a la que se hace referencia
                ModeloColumna findSlave = null;

                // Si encontramos el modelo buscamos la columna de la slave
                if (findModel != null)
                {
                    // Buscamos la columna de la slave
                    findSlave = findModel
                        .Columnas.Where(x => x.Id == $@"{_modelName}Col")
                        .FirstOrDefault();

                    // Si encontramos la slave indicamos el result en true
                    result = findSlave != null;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al validar la referencia de la columna {column.Id}. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Valida la visibilidad de la columna
        /// </summary>
        /// <param name="column"></param>
        /// <param name="isMasterReference"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool IsVisuble(ModeloColumna column, bool isMasterReference)
        {
            try
            {
                // Indicador para cuando la columna es o no visible por default
                bool isVisible = false;

                if (column.Id == "Id")
                {
                    // Si es in Id validamos que no sea catalogo ni transición
                    isVisible = (
                        _grid.TypeModel != WoTypeModel.CatalogSlave
                        && _grid.TypeModel != WoTypeModel.TransactionSlave
                    );
                }
                else if (
                    column.Id == "DeletedBy"
                    || column.Id == "ModifiedBy"
                    || column.Id == "CreatedBy"
                    || column.Id == "SuspendBy"
                    || column.Id == "SuspendInfo"
                    || column.Id == "DeleteInfo"
                    || column.Id == "SuspendDate"
                    || column.Id == "CreatedDate"
                    || column.Id == "ModifiedDate"
                    || column.Id == "DeletedDate"
                    || column.Id == "Nivel"
                    || column.Id == "Contabiliza"
                    || column.Id == "WoState"
                    || column.Id == "RowVersion"
                )
                {
                    // Si es alguno de los campos por default ocultamos la columna
                    isVisible = false;
                }
                else if (column.TipoControl == WoTypeControl.CollectionEditor)
                {
                    // Si es colección ocultamos la columna
                    isVisible = false;
                }
                else
                {
                    if (
                        (
                            _grid.TypeModel == WoTypeModel.CatalogSlave
                            || _grid.TypeModel == WoTypeModel.TransactionSlave
                        ) && isMasterReference
                    )
                    {
                        isVisible = false;
                    }
                    else
                    {
                        isVisible = true;
                    }
                }

                return isVisible;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al validar si la row {column.Id} es visible. {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Construye la columna
        /// </summary>
        /// <returns></returns>
        private WoColumnProperties BuildColumn(ModeloColumna column, int index, bool isVisible)
        {
            try
            {
                // Recupera el identificador de la etiqueta
                string etiqueta = Etiqueta.ToId(column.Grid);

                // Utiliza el helper de tipos para recuperar el tipo de la columna
                string columnType = _woDesignerTypeHelper.DesignerTypeToCodeType(
                    designerType: column.TipoColumna.ToString().ToLower(),
                    isNullable: column.Nulo,
                    attributeName: column.Id
                );

                // Indicador de si es referencia
                bool isReference = (column.TipoColumna == WoTypeColumn.Reference);

                // Instanciamos la columna a retornar con la data recuperada del modelo.
                WoColumnProperties woColumnProperties = new WoColumnProperties()
                {
                    Id = column.Id,
                    Etiqueta = etiqueta,
                    MaskText = column.Grid,
                    ModelName = _modelName,
                    IsSlave = _grid.IsSlave,
                    Process = _grid.Proceso,
                    Size = 100,
                    SizeType = "px",
                    Index = index,
                    IsVisible = isVisible,
                    BindingType = columnType,
                    IsReference = isReference,
                    UrlBaseReference = null,
                    Control = column.TipoControl,
                    IsCustomColumn = false,
                };

                return woColumnProperties;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al construir la columna {column.Id}. {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza las propiedades de la columna con la base ya construida
        /// </summary>
        /// <param name="columnProperties"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private WoColumnProperties UpdateColumn(
            WoColumnProperties columnProperties,
            ModeloColumna column
        )
        {
            try
            {
                // Indica si es una referencia
                if (column.TipoColumna == Core.WoTypeColumn.Reference)
                {
                    // Nombre del modelo al que se hace referencia
                    string referenceModelId = column.ModeloId;

                    // Recuperación del modelo principal
                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    Modelo referenceModel = woProjectDataHelper.GetMainModel(referenceModelId);

                    // Actualización de la referencia
                    columnProperties.UrlBaseReference =
                        $@"{referenceModel.ProcesoId}/{referenceModel.TipoModelo}/{referenceModelId}";
                    columnProperties.TypeModel = referenceModel.TipoModelo;
                    columnProperties.SubTypeModel = referenceModel.SubTipoModelo;
                }

                // Actualiza la referencia para cuando es un campo de urn
                if (column.TipoControl == WoTypeControl.Urn)
                {
                    columnProperties.IsReference = true;
                }

                return columnProperties;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Error al actualizar la columna. {ex.Message}");
            }
        }

        /// <summary>
        /// Construye las columnas de la grid.
        /// </summary>
        private void BuildColumns()
        {
            bool isVisible = false;
            int index = 0;

            _woDesignerTypeHelper = new WoDesignerTypeHelper(_modelName);

            List<ModeloColumna> columnsCol = _baseModel.Columnas.OrderBy(x => x.Orden).ToList();

            if (_extencionModels.Count > 0)
            {
                foreach (Modelo extensionModel in _extencionModels)
                {
                    if (extensionModel.ProcesoId == string.Empty)
                    {
                        columnsCol.AddRange(extensionModel.Columnas);
                    }
                }
            }

            foreach (var column in columnsCol)
            {
                bool isMasterReference = false;

                if (column.TipoColumna == Core.WoTypeColumn.Reference)
                {
                    string masterModelName = column.ModeloId;

                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    List<Modelo> findModels = woProjectDataHelper.GetModelWithExtencions(
                        masterModelName
                    );

                    //Modelo masterModel = woProjectDataHelper.GetMainModel(masterModelName);

                    //ModeloColumna findSlave1 = masterModel
                    //    .Columnas.Where(x => x.Id == $@"{_modelName}Col")
                    //    .FirstOrDefault();

                    Modelo findModel = (
                        from model in findModels
                        where
                            model.Columnas.FirstOrDefault(column =>
                                column.Id == $@"{_modelName}Col"
                            ) != null
                        select model
                    ).FirstOrDefault();

                    //Modelo masterModel = _project
                    //    .ModeloCol.Modelos.Where(x => x.Id == masterModelName)
                    //    .FirstOrDefault();

                    ModeloColumna findSlave = null;

                    if (findModel != null)
                    {
                        findSlave = findModel
                            .Columnas.Where(x => x.Id == $@"{_modelName}Col")
                            .FirstOrDefault();
                    }

                    if (findSlave != null)
                    {
                        isMasterReference = true;
                    }
                }

                if (column.Id == "Id")
                {
                    isVisible = (
                        _grid.TypeModel != WoTypeModel.CatalogSlave
                        && _grid.TypeModel != WoTypeModel.TransactionSlave
                    );
                }
                else if (
                    column.Id == "DeletedBy"
                    || column.Id == "ModifiedBy"
                    || column.Id == "CreatedBy"
                    || column.Id == "SuspendBy"
                    || column.Id == "SuspendInfo"
                    || column.Id == "DeleteInfo"
                    || column.Id == "SuspendDate"
                    || column.Id == "CreatedDate"
                    || column.Id == "ModifiedDate"
                    || column.Id == "DeletedDate"
                    || column.Id == "Nivel"
                    || column.Id == "Contabiliza"
                    || column.Id == "WoState"
                    || column.Id == "RowVersion"
                )
                {
                    isVisible = false;
                }
                else if (column.TipoControl == WoTypeControl.CollectionEditor)
                {
                    isVisible = false;
                }
                else
                {
                    if (
                        (
                            _grid.TypeModel == WoTypeModel.CatalogSlave
                            || _grid.TypeModel == WoTypeModel.TransactionSlave
                        ) && isMasterReference
                    )
                    {
                        isVisible = false;
                    }
                    else
                    {
                        isVisible = true;
                    }
                }

                string etiqueta = Etiqueta.ToId(column.Grid);

                string columnType = _woDesignerTypeHelper.DesignerTypeToCodeType(
                    designerType: column.TipoColumna.ToString().ToLower(),
                    isNullable: column.Nulo,
                    attributeName: column.Id
                );

                bool isReference = (column.TipoColumna == WoTypeColumn.Reference);
                string urlBaseReference = null;

                WoColumnProperties woColumnProperties = new WoColumnProperties()
                {
                    Id = column.Id,
                    Etiqueta = etiqueta,
                    MaskText = column.Grid,
                    ModelName = _modelName,
                    IsSlave = _grid.IsSlave,
                    Process = _grid.Proceso,
                    Size = 100,
                    SizeType = "px",
                    Index = index,
                    IsVisible = isVisible,
                    BindingType = columnType,
                    IsReference = isReference,
                    UrlBaseReference = urlBaseReference,
                    Control = column.TipoControl,
                    IsCustomColumn = false,
                };

                if (isReference)
                {
                    string referenceModelId = column.ModeloId;

                    WoProjectDataHelper woProjectDataHelper = new WoProjectDataHelper();
                    Modelo referenceModel = woProjectDataHelper.GetMainModel(referenceModelId);

                    //Modelo referenceModel = _project
                    //    .ModeloCol.Modelos.Where(x => x.Id == referenceModelId)
                    //    .FirstOrDefault();

                    woColumnProperties.UrlBaseReference =
                        $@"{referenceModel.ProcesoId}/{referenceModel.TipoModelo}/{referenceModelId}";
                    woColumnProperties.TypeModel = referenceModel.TipoModelo;
                    woColumnProperties.SubTypeModel = referenceModel.SubTipoModelo;
                }

                if (column.TipoControl == WoTypeControl.Urn)
                {
                    woColumnProperties.IsReference = true;
                }

                if (column.TipoControl != WoTypeControl.CollectionEditor)
                {
                    _grid.WoColumnPropertiesCol.Add(woColumnProperties);
                }

                index++;
            }
        }

        #endregion Construcción de grid

        #region Creación del fichero

        /// <summary>
        /// Salvado del diseño raw del json de la grid
        /// </summary>
        /// <param name="woGridProperties"></param>
        /// <exception cref="Exception"></exception>
        private void SaveFile(WoGridProperties woGridProperties, string modelName)
        {
            try
            {
                // Construcción del path del grid
                string pathJson = $@"{_project.DirLayOuts}\ListDesign\{modelName}GridList.json";

                // Serialization del objeto del diseño de la grid a json
                string rawJson = JsonConvert.SerializeObject(_grid);

                // Escritura del fichero
                WoDirectory.WriteFile(pathJson, rawJson);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"Error al salvar el fichero raw del diseño {woGridProperties.ModelName}. {ex.Message}"
                );
            }
        }

        #endregion Creación del fichero


        #region Logs

        private WoLog _cantFindModel = new WoLog()
        {
            CodeLog = "000",
            Title = $@"El modelo no se encuentra en el proyecto.",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = "ChargeModel"
            }
        };

        private WoLog _rawReady = new WoLog()
        {
            CodeLog = "000",
            Title = $@"Se creo el formulario en raw para el modelo.",
            LogType = eLogType.Common,
            FileDetails = new WoFileDetails()
            {
                Class = "WoDesignerRawSerializerHelper",
                MethodOrContext = ""
            }
        };

        #endregion Logs
    }
}
