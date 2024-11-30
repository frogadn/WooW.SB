using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using DevExpress.XtraLayout;
using WooW.Core.Common.Observer.LogModel;
using WooW.Core.Common.Observer.LogObserver;
using WooW.SB.Designer.DesignerModels;
using WooW.SB.GeneratorComponentBase.GeneratorComponent;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoDesignerSerializerHelper
    {
        #region Instancias singleton

        private WoLogObserver _observer = WoLogObserver.GetInstance();

        #endregion Instancias singleton

        #region Variables globales

        /// <summary>
        /// Grupo que se serializara del para guardar
        /// el formulario que diseña el usuario en json.
        /// </summary>
        private WoContainer _rootGroup = new WoContainer();

        /// <summary>
        /// Layout que se convertirá en json.
        /// </summary>
        private LayoutControl _layoutDesigner = null;

        /// <summary>
        /// Indica que el item que se esta ordenando ya fue posicionado o aun no.
        /// Principalmente permite validar cuando salir de la recursiva.
        /// </summary>
        private bool _itemIsReady = false;

        #endregion Variables globales

        #region Constructor

        /// <summary>
        /// Constructor principal.
        /// </summary>
        /// <param name="layoutDesigner"></param>
        public WoDesignerSerializerHelper(LayoutControl layoutDesigner)
        {
            _layoutDesigner = layoutDesigner;
        }

        #endregion Constructor

        #region Métodos públicos

        /// <summary>
        /// Método principal que se ocupa de llamar al método base que
        /// crea el grupo con la estructura del layout y lo retorna.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public WoContainer SerilizeFormToJson()
        {
            ChargeGroup();
            return _rootGroup;
        }

        #endregion Métodos públicos

        #region Método principal

        /// <summary>
        /// Recorre cada uno de los items en el layout y en función del tipo
        /// lo pasa a un método que lo ordenara sobre la instancia de group,
        /// al pasarlo realiza una convecino expósita sobre el item del for-each.
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void ChargeGroup()
        {
            foreach (var item in _layoutDesigner.Items)
            {
                string controlType = item.GetType().Name;
                switch (controlType)
                {
                    case "LayoutControlGroup":
                        ChargeDataGroup((LayoutControlGroup)item);
                        break;
                    case "TabbedControlGroup":
                        ChargeDataTab((TabbedControlGroup)item);
                        break;
                    case "LayoutControlItem":
                        ChargeDataItem((LayoutControlItem)item);
                        break;
                }
            }
        }

        #endregion Método principal

        #region Serialization de los grupos

        /// <summary>
        /// Crea una instancia de grupo y en caso de ser root lo pasa como la instancia
        /// base de _groupForSerialize en caso contrario, primero valida que el grupo no tenga
        /// un padre del tipo tab, en ese caso se define como un grupo dentro de un grupo de tabs
        /// y el padre se actualiza con el nombre de la tab, independiente de esa validación
        /// envía el grupo a una función que lo posiciona dentro de la variable de _groupForSerialize
        /// ya sea a ese nivel en subgroups o a un grupo interno como un subgroup de un subgroup,
        /// y el retorno de ese método retornara una instancia de Group ordenada con el grupo que se
        /// paso ya agregado.
        /// </summary>
        /// <param name="layoutControlGroup"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeDataGroup(LayoutControlGroup layoutControlGroup)
        {
            if (layoutControlGroup.Tag != null)
            {
                WoContainer group = (
                    (WoComponentProperties)layoutControlGroup.Tag
                ).ConvertToWoContainer();
                group.Parent =
                    (layoutControlGroup.Parent == null) ? "" : layoutControlGroup.Parent.Name;
                group.Col = layoutControlGroup.OptionsTableLayoutGroup.ColumnCount;
                group.Row = layoutControlGroup.OptionsTableLayoutGroup.RowCount;
                group.ColSpan = layoutControlGroup.OptionsTableLayoutItem.ColumnSpan;
                group.RowSpan = layoutControlGroup.OptionsTableLayoutItem.RowSpan;
                group.ColumnIndex = layoutControlGroup.OptionsTableLayoutItem.ColumnIndex;
                group.RowIndex = layoutControlGroup.OptionsTableLayoutItem.RowIndex;
                group.BeginRow = (layoutControlGroup.OptionsTableLayoutItem.ColumnIndex == 0);

                if (group.TypeContainer == eTypeContainer.FormRoot)
                {
                    _rootGroup = group;
                }
                else
                {
                    if (layoutControlGroup.TabbedGroupParentName != "")
                    {
                        group.TypeContainer = eTypeContainer.FormTab;
                        group.Parent = layoutControlGroup.TabbedGroupParentName;
                    }
                    _rootGroup = OrdenDataGroup(group, _rootGroup);
                    _itemIsReady = false;
                }
            }
        }

        /// <summary>
        /// Método recursiva ocupado de ordenar el grupo que recibe dentro del grupo base
        /// ya sea un nivel abajo como sub grupo o como un sub grupo de un sub grupo N
        /// niveles abajo.
        /// Algoritmo:
        ///     Si el nombre del padre del grupo que se pasa para ordenarse
        ///     coincide con el nombre del grupo padre actual que en
        ///     primera instancia sera Root entones se añadirá como un sub grupo
        ///     de este nivel, se marca la variable de control como true, para indicar que
        ///     el grupo ya fue ordenado y se retorna el grupo padre.
        ///     En caso contrario se recorren los sub grupos y cada uno vuelve a entrar
        ///     en la misma función pero con el grupo padre como cada uno de los sub grupos
        ///     al salir de cada uno de estos se valida si la variable de control ya es true
        ///     en cuyo caso se rompe el for y se deja retornar el padre sacia arriba.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="parentGroup"></param>
        /// <returns></returns>
        private WoContainer OrdenDataGroup(WoContainer group, WoContainer parentGroup)
        {
            if (parentGroup.Id == group.Parent)
            {
                if (parentGroup.ContainersCol == null)
                {
                    parentGroup.ContainersCol = new List<WoContainer>();
                }
                _itemIsReady = true;
                parentGroup.ContainersCol.Add(group);
            }
            else
            {
                if (parentGroup.ContainersCol != null)
                {
                    for (int i = 0; i < parentGroup.ContainersCol.Count; i++)
                    {
                        parentGroup.ContainersCol[i] = OrdenDataGroup(
                            group,
                            parentGroup.ContainersCol[i]
                        );
                        if (_itemIsReady)
                        {
                            i = parentGroup.ContainersCol.Count;
                        }
                    }
                }
            }
            return parentGroup;
        }

        #endregion Serialization de los grupos

        #region Serialization de las tabs

        /// <summary>
        /// Crea una instancia de grupo con todas sus propiedades y se indica que sera un
        /// grupo de tabs.
        /// Una vez creada la instancia se envía a un método de orden recursivo.
        /// </summary>
        /// <param name="tabbedControlGroup"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeDataTab(TabbedControlGroup tabbedControlGroup)
        {
            WoContainer tabbedGroup = (
                (WoComponentProperties)tabbedControlGroup.Tag
            ).ConvertToWoContainer();
            tabbedGroup.Parent =
                (tabbedControlGroup.Parent == null) ? "" : tabbedControlGroup.Parent.Name;
            tabbedGroup.ColSpan = tabbedControlGroup.OptionsTableLayoutItem.ColumnSpan;
            tabbedGroup.RowSpan = tabbedControlGroup.OptionsTableLayoutItem.RowSpan;
            tabbedGroup.ColumnIndex = tabbedControlGroup.OptionsTableLayoutItem.ColumnIndex;
            tabbedGroup.RowIndex = tabbedControlGroup.OptionsTableLayoutItem.RowIndex;
            tabbedGroup.BeginRow = (tabbedControlGroup.OptionsTableLayoutItem.ColumnIndex == 0);

            _rootGroup = OrdenDataTab(tabbedGroup, _rootGroup);
            _itemIsReady = false;
        }

        /// <summary>
        /// Método recursivo que permite ordenar el grupo de tabs de igual forma que se ordena
        /// el grupo.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="parentGroup"></param>
        /// <returns></returns>
        private WoContainer OrdenDataTab(WoContainer group, WoContainer parentGroup)
        {
            if (parentGroup.Id == group.Parent)
            {
                if (parentGroup.ContainersCol == null)
                {
                    parentGroup.ContainersCol = new List<WoContainer>();
                }
                _itemIsReady = true;
                parentGroup.ContainersCol.Add(group);
            }
            else
            {
                if (parentGroup.ContainersCol != null)
                {
                    for (int i = 0; i < parentGroup.ContainersCol.Count; i++)
                    {
                        parentGroup.ContainersCol[i] = OrdenDataTab(
                            group,
                            parentGroup.ContainersCol[i]
                        );
                        if (_itemIsReady)
                        {
                            i = parentGroup.ContainersCol.Count;
                        }
                    }
                }
            }
            return parentGroup;
        }

        #endregion Serialization de las tabs

        #region Serialization de los items

        /// <summary>
        /// Lista de los modelos referenciados en el formulario para validar que no se repitan.
        /// </summary>
        private List<string> _modelReferece = new List<string>();

        /// <summary>
        /// Valida controles custom para tener sus propiedades base como variables
        /// y las usa así como el resto de propiedades de item para generar una instancia de item
        /// que luego se mandara a una función de orden recursiva para acomodarla en la instancia
        /// _groupForSerialize en caso de que el padre no sea nulo.
        /// </summary>
        /// <param name="item"></param>
        [SupportedOSPlatform("windows")]
        private void ChargeDataItem(LayoutControlItem item)
        {
            string itemId = item.Name;
            string itemParent = item.ParentName;

            if (item.Parent != null)
            {
                WoItem woItem = ((WoComponentProperties)item.Tag).ConvertToWoItem();

                int multipleReference = 0;
                IEnumerable<string> referencesCol = _modelReferece.Where(x =>
                    x == woItem.ClassModelType
                );

                if (referencesCol != null)
                {
                    multipleReference = referencesCol.Count();
                }

                woItem.Parent = (item.Parent == null) ? string.Empty : item.Parent.Name;
                woItem.ColSpan = item.OptionsTableLayoutItem.ColumnSpan;
                woItem.RowSpan = item.OptionsTableLayoutItem.RowSpan;
                woItem.ColumnIndex = item.OptionsTableLayoutItem.ColumnIndex;
                woItem.RowIndex = item.OptionsTableLayoutItem.RowIndex;
                woItem.BeginRow = (item.OptionsTableLayoutItem.ColumnIndex == 0);
                woItem.BaseModelName = _rootGroup.ModelId;
                woItem.MultipleReference = multipleReference;

                _modelReferece.Add(woItem.ClassModelType);

                _rootGroup = OrdenDataItem(woItem, _rootGroup);
            }
            else
            {
                _observer.SetLog(_itemOculto);
            }
            _itemIsReady = false;
        }

        /// <summary>
        /// Método recursivo para ordenar en la instancia base de Group el
        /// item que se pase como parámetro funciona igual que los dos métodos anteriores en el
        /// tema del ordenamiento, salvo por un apartado de posicionamiento extra que se agrego.
        /// Usamos linq para determinar si se encuentra en una fila superior a la de los demás
        /// controles ya agregados, en caso de que si lo sea se añade simplemente dentro de los
        /// items del grupo, pero en caso contrario usamos una lista auxiliar para ordenarlo
        /// correctamente y luego simplemente remplazamos la instancia de items por la lista auxiliar.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parentGroup"></param>
        /// <returns></returns>
        private WoContainer OrdenDataItem(WoItem item, WoContainer parentGroup)
        {
            string itemId = item.Id;
            string itemParent = item.Parent;
            string parentGroupId = parentGroup.Id;

            if (parentGroup.Id == item.Parent)
            {
                if (parentGroup.ItemsCol == null)
                {
                    parentGroup.ItemsCol = new List<WoItem>();
                }

                _itemIsReady = true;

                if (parentGroup.ItemsCol.Count > 0)
                {
                    bool isLast =
                        (
                            (
                                from itemGroup in parentGroup.ItemsCol
                                where itemGroup.RowIndex > item.RowIndex
                                select itemGroup
                            ).Count() == 0
                        )
                            ? true
                            : false;
                    if (isLast)
                    {
                        List<WoItem> itemsInRow = parentGroup
                            .ItemsCol.Where(x => x.RowIndex == item.RowIndex)
                            .ToList();
                        if (itemsInRow.Count() > 0)
                        {
                            List<WoItem> itemsOrdenados = new List<WoItem>();
                            bool added = false;
                            foreach (var itemGrupo in parentGroup.ItemsCol)
                            {
                                if (itemGrupo.RowIndex == item.RowIndex && !added)
                                {
                                    itemsInRow.Add(item);
                                    List<WoItem> itemsInRowOrdenados = itemsInRow
                                        .OrderBy(x => x.ColumnIndex)
                                        .ToList();
                                    itemsOrdenados.AddRange(itemsInRowOrdenados);
                                    added = true;
                                }
                                //else if (!added)
                                else
                                {
                                    if (!itemsOrdenados.Contains(itemGrupo))
                                    {
                                        itemsOrdenados.Add(itemGrupo);
                                    }
                                }
                            }
                            parentGroup.ItemsCol = itemsOrdenados;
                        }
                        else
                        {
                            parentGroup.ItemsCol.Add(item);
                        }
                    }
                    else
                    {
                        List<WoItem> itemsOrdenados = new List<WoItem>();
                        bool added = false;
                        foreach (var itemGrupo in parentGroup.ItemsCol)
                        {
                            if (itemGrupo.RowIndex > item.RowIndex && !added)
                            {
                                itemsOrdenados.Add(item);
                                itemsOrdenados.Add(itemGrupo);
                                added = true;
                            }
                            else if (itemGrupo.RowIndex == item.RowIndex && !added)
                            {
                                List<WoItem> itemsInRow = parentGroup
                                    .ItemsCol.Where(x => x.RowIndex == item.RowIndex)
                                    .ToList();
                                itemsInRow.Add(item);

                                List<WoItem> itemsInRowOrdenados = itemsInRow
                                    .OrderBy(x => x.ColumnIndex)
                                    .ToList();
                                itemsOrdenados.AddRange(itemsInRowOrdenados);
                                added = true;
                            }
                            else
                            {
                                if (!itemsOrdenados.Contains(itemGrupo))
                                {
                                    itemsOrdenados.Add(itemGrupo);
                                }
                            }
                        }
                        parentGroup.ItemsCol = itemsOrdenados;
                    }
                }
                else
                {
                    parentGroup.ItemsCol.Add(item);
                }
            }
            else
            {
                if (parentGroup.ContainersCol != null)
                {
                    for (int i = 0; i < parentGroup.ContainersCol.Count; i++)
                    {
                        parentGroup.ContainersCol[i] = OrdenDataItem(
                            item,
                            parentGroup.ContainersCol[i]
                        );
                        if (_itemIsReady)
                        {
                            i = parentGroup.ContainersCol.Count;
                        }
                    }
                }
            }
            return parentGroup;
        }

        #endregion Serialization de los items

        #region Alertas

        private WoLog _itemOculto = new WoLog()
        {
            CodeLog = "000",
            Title = "Se omitió en el grupo un item oculto",
            Details = "Se omitió del grupo para serializar un item oculto.",
            UserMessage = "Se omitió del grupo para serializar un item oculto.",
            LogType = eLogType.Information,
            FileDetails = new WoFileDetails()
            {
                Class = "DesignerSerializerHelper",
                MethodOrContext = "ChargeDataItem"
            }
        };

        #endregion Alertas
    }
}
