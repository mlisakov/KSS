using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using HomeProjectWebForms.Consts;
using HomeProjectWebForms.Helpers;

namespace HomeProjectWebForms.Controls
{
    public class CustomTreeView : TreeView
    {
        /// <summary>
        ///     Максимальная глубина загрузки дерева
        /// </summary>
        private const int _maxLoadLevelTree = 2;

        //!+ Избавится от поля Все хранится в сессии!
        private static Guid _selectedDivision;


        private static Dictionary<Guid, string> _selectedNodeParentList;

        /// <summary>
        ///     Изображение для элемента дерева DepartmentState
        /// </summary>
        private readonly string _departmentTreeNodeTypeImageUrl;

        /// <summary>
        ///     Изображение для элемента дерева DivisionState
        /// </summary>
        private readonly string _divisionTreeNodeTypeImageUrl;

        public CustomTreeNode CurrentSelectedNode;

        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        public CustomTreeView()
        {
            _selectedNodeParentList = new Dictionary<Guid, string>();
            _divisionTreeNodeTypeImageUrl = UIConsts.DivisionTreeNodeTypeImageUrl;
            _departmentTreeNodeTypeImageUrl = UIConsts.DepartmentTreeNodeTypeImageUrl;
            TreeNodeExpanded += OnTreeNodeExpanded;
        }

        protected override TreeNode CreateNode()
        {
            return new CustomTreeNode(this, false);
        }

        /// <summary>
        ///     Заполнение дерева подразделений
        /// </summary>
        public void FillTree(Guid selectedDepartment)
        {
            _selectedDivision = selectedDepartment;
            if (selectedDepartment != Guid.Empty)
                _selectedNodeParentList = GetDepartmentStateParents(_selectedDivision);

            Nodes.Clear();
            foreach (var divisionState in GetDivisionState(Guid.Empty))
            {
                int currentDivisionLoadLevel = 1;
                //int currentDepartmentLoadLevel = 2;
                var divisionStateTreeNode = new CustomTreeNode
                {
                    Text = divisionState.Value,
                    Value = divisionState.Key.ToString(),
                    Tag = DepartmentTreeNodeType.DivisionState
                };
                if (!_selectedNodeParentList.ContainsKey(divisionState.Key))
                    divisionStateTreeNode.Collapse();
                Nodes.Add(divisionStateTreeNode);
                //Заполнение дерева DivisionState
                CreateDivisionStateTreeChildNodes(divisionStateTreeNode, divisionState.Key, ref currentDivisionLoadLevel,
                    true);
            }
        }

        /// <summary>
        ///     Заполнение дерева подразделений
        /// </summary>
        public void FillTreeByDivision(Guid selectedDivision)
        {
            _selectedDivision = selectedDivision;
            if (selectedDivision != Guid.Empty)
                _selectedNodeParentList = GetDivisionParents(_selectedDivision);

            Nodes.Clear();
            foreach (var divisionState in GetDivisionState(Guid.Empty))
            {
                int currentDivisionLoadLevel = 1;
                //int currentDepartmentLoadLevel = 2;
                var divisionStateTreeNode = new CustomTreeNode
                {
                    Text = divisionState.Value,
                    Value = divisionState.Key.ToString(),
                    Tag = DepartmentTreeNodeType.DivisionState
                };
                if (!_selectedNodeParentList.ContainsKey(divisionState.Key))
                    divisionStateTreeNode.Collapse();
                Nodes.Add(divisionStateTreeNode);
                //Заполнение дерева DivisionState
                CreateDivisionStateTreeChildNodes(divisionStateTreeNode, divisionState.Key, ref currentDivisionLoadLevel,
                    true);
            }
        }

        /// <summary>
        ///     Заполнение дерева подразделений
        /// </summary>
        public void FillTree(CustomTreeNode rootNode, CustomTreeNode selected)
        {
            Nodes.Clear();
            Nodes.Add(rootNode);
            if (selected != null)
                selected.Selected = true;
        }


        /// <summary>
        ///     Получение списка DepartmentState
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        private Dictionary<Guid, string> GetDepartmentState(Guid parentNode)
        {
            var departments = new Dictionary<Guid, string>();
            string commandText = parentNode == Guid.Empty
                ? "select Id,Department,ParentId from DepartmentState where ExpirationDate IS NULL AND ParentId is null"
                : "select Id,Department,ParentId from DepartmentState where ExpirationDate IS NULL AND ParentId ='" +
                  parentNode + "'";

            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection);
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Guid idDepartment = reader.GetGuid(0);
                        string department = reader.GetString(1);
                        if (!departments.ContainsKey(idDepartment))
                            departments.Add(idDepartment, department);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetDepartmentStateв", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }


            return departments;
        }

        private IEnumerable<KeyValuePair<Guid, string>> GetDivisionDepartmentState(Guid parentDivisionGuid)
        {
            var departments = new Dictionary<Guid, string>();
            string commandText =
                "select Id,Department from DepartmentState where ExpirationDate IS NULL AND ParentId is null and DivisionId='" +
                parentDivisionGuid + "'";

            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection);
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Guid idDepartment = reader.GetGuid(0);
                        string department = reader.GetString(1);
                        departments.Add(idDepartment, department);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetDivisionDepartmentStateв", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }


            return departments;
        }

        private Dictionary<Guid, string> GetDepartmentStateParents(Guid departmentStateGuid)
        {
            var departments = new Dictionary<Guid, string>();
            const string commandText = "GetDepartmentStatesAllParents";

            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.StoredProcedure};
            cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@Guid"].Value = departmentStateGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Guid idItem = reader.GetGuid(0);
                        string itemType = reader.GetString(1);
                        departments.Add(idItem, itemType);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetDepartmentStatesAllParents", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }


            return departments;
        }

        private Dictionary<Guid, string> GetDivisionParents(Guid divisionGuid)
        {
            var departments = new Dictionary<Guid, string>();
            const string commandText = "GetDivisionAllParents";

            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.StoredProcedure};
            cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@Guid"].Value = divisionGuid;
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Guid idItem = reader.GetGuid(0);
                        string itemType = reader.GetString(1);
                        departments.Add(idItem, itemType);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка GetDivisionParentsв", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }


            return departments;
        }


        /// <summary>
        ///     Признак наличия у элемента потомков
        /// </summary>
        /// <param name="treeNodeType">Тип элемента дерева</param>
        /// <param name="parent">Идентификатор родительского элемента</param>
        /// <returns>true-у родителя есть потомки</returns>
        private bool IsHasChilds(DepartmentTreeNodeType treeNodeType, Guid parent)
        {
            string commandText = string.Empty;
            switch (treeNodeType)
            {
                case DepartmentTreeNodeType.DivisionState:
                    commandText = "select count(Id) as ChildsCount from DivisionState where ParentId='" + parent + "'";
                    break;
                case DepartmentTreeNodeType.DepartmentState:
                    commandText =
                        "select count(Id) as ChildsCount from DepartmentState where ExpirationDate IS NULL AND ParentId='" +
                        parent + "'";
                    break;
            }
            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection);
            try
            {
                connection.Open();
                string childCount = cmd.ExecuteScalar().ToString();
                int nodeChildsCount;
                if (int.TryParse(childCount, out nodeChildsCount))
                    if (nodeChildsCount > 0)
                        return true;
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка проверки наличия дочерних элементов", ex);
            }
            finally
            {
                connection.Close();
            }
            return false;
        }

        /// <summary>
        ///     Получение списка DivisionState
        /// </summary>
        /// <param name="parentNode">Родительский элемент</param>
        /// <returns>Список DivisionState</returns>
        private Dictionary<Guid, string> GetDivisionState(Guid parentNode)
        {
            var divisionStates = new Dictionary<Guid, string>();
            string commandText = parentNode == Guid.Empty
                ? "select Id,Division,ParentId from DivisionState where ParentId is null"
                : "select Id,Division,ParentId from DivisionState where ParentId ='" + parentNode + "'";

            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection);
            SqlDataReader reader = null;
            try
            {
                connection.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        Guid idDepartment = reader.GetGuid(0);
                        string department = reader.GetString(1);
                        if (!divisionStates.ContainsKey(idDepartment))
                            divisionStates.Add(idDepartment, department);
                    }
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка получения DivisionState", ex);
                //Response.Write("<script>window.alert('Ошибка приложения');</script>");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                connection.Close();
            }


            return divisionStates;
        }

        /// <summary>
        ///     Создание элемента дерева
        /// </summary>
        /// <param name="treeNodeType">Тип элемента</param>
        /// <param name="Text">Текст</param>
        /// <param name="Value">Значение</param>
        /// <returns>Элемент дерева</returns>
        private CustomTreeNode CreateCustomTreeNode(DepartmentTreeNodeType treeNodeType, String Text, Guid Value)
        {
            var node = new CustomTreeNode
            {
                Text = Text,
                Value = Value.ToString(),
                Tag = treeNodeType,
                ImageUrl =
                    treeNodeType == DepartmentTreeNodeType.DivisionState
                        ? _divisionTreeNodeTypeImageUrl
                        : _departmentTreeNodeTypeImageUrl
            };
            if ((_selectedNodeParentList.ContainsKey(Value)) && Value != _selectedDivision)
                node.Expand();

            else
                //Если не вызвать то сработает событие Expand и загрузятся лишние элементы
                node.Collapse();

            if (_selectedDivision != Guid.Empty && Value == _selectedDivision)
            {
                node.Selected = true;
                CurrentSelectedNode = node;
            }
            return node;
        }

        /// <summary>
        ///     Создание дерева для DivisionState
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        /// <param name="parentDivisionStateGuid">Идентификатор родительского элемента</param>
        /// <param name="currentLoadLevel">Текущий уровень загрузки</param>
        private void CreateDivisionStateTreeChildNodes(CustomTreeNode parent, Guid parentDivisionStateGuid,
            ref int currentLoadLevel, bool isLoadFirst)
        {
            //Ограничиваем глубину загрузки дерева
            if (!(_selectedNodeParentList.ContainsKey(parentDivisionStateGuid)) && currentLoadLevel > _maxLoadLevelTree)
                return;

            //Формируем дерево DivisionState для текущего DivisionState
            List<CustomTreeNode> childNodesList = parent.ChildNodes.Cast<CustomTreeNode>().ToList();
            foreach (var divisionStateNode in GetDivisionState(parentDivisionStateGuid))
            {
                CustomTreeNode node =
                    childNodesList.FirstOrDefault(n => n.Value.Equals(divisionStateNode.Key.ToString()));
                if (node == null)
                {
                    node = CreateCustomTreeNode(DepartmentTreeNodeType.DivisionState, divisionStateNode.Value,
                        divisionStateNode.Key);
                    parent.ChildNodes.Add(node);
                }
                bool hasChild = IsHasChilds(DepartmentTreeNodeType.DivisionState, divisionStateNode.Key);

                if (hasChild)
                {
                    currentLoadLevel++;
                    CreateDivisionStateTreeChildNodes(node, divisionStateNode.Key, ref currentLoadLevel, isLoadFirst);
                    currentLoadLevel--;
                }
                else
                {
                    //Листовой элемент в дереве DivisionState
                    int currentDepartmentStateLoadLevel = 1;
                    //Формируем дерево DepartmentState для текущего DivisionState
                    CreateDivisionStateLeafDepartmentStateTreeChildNodes(node, divisionStateNode.Key,
                        ref currentDepartmentStateLoadLevel, isLoadFirst);
                }
            }
        }

        private void CreateDivisionStateLeafDepartmentStateTreeChildNodes(CustomTreeNode parent, Guid parentDivisionGuid,
            ref int currentLoadLevel, bool isLoadFirst)
        {
            //Ограничиваем глубину загрузки дерева
            if (!(_selectedNodeParentList.ContainsKey(parentDivisionGuid)) && currentLoadLevel > _maxLoadLevelTree)
                return;

            List<CustomTreeNode> treeNodeChildNodesList = parent.ChildNodes.Cast<CustomTreeNode>().ToList();

            foreach (var departmentNode in GetDivisionDepartmentState(parentDivisionGuid))
            {
                CustomTreeNode node =
                    treeNodeChildNodesList.FirstOrDefault(n => n.Value.Equals(departmentNode.Key.ToString()));
                if (node == null)
                {
                    node = CreateCustomTreeNode(DepartmentTreeNodeType.DepartmentState,
                        departmentNode.Value,
                        departmentNode.Key);
                    parent.ChildNodes.Add(node);
                }
                if (isLoadFirst && !_selectedNodeParentList.ContainsKey(parentDivisionGuid)) return;

                bool hasChild = IsHasChilds(DepartmentTreeNodeType.DepartmentState, departmentNode.Key);

                if (hasChild)
                {
                    currentLoadLevel++;
                    CreateDepartmentStateTreeChildNodes(node, departmentNode.Key, ref currentLoadLevel, true);
                    currentLoadLevel--;
                }
            }
        }

        /// <summary>
        ///     Создание дерева для DepartmentState
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        /// <param name="parentDepartmentGuid">Идентификатор родительского элемента</param>
        /// <param name="currentLoadLevel">Текущий уровень загрузки</param>
        /// <param name="isLoadFirst"></param>
        private void CreateDepartmentStateTreeChildNodes(CustomTreeNode parent, Guid parentDepartmentGuid,
            ref int currentLoadLevel, bool isLoadFirst)
        {
            //Ограничиваем глубину загрузки дерева
            if (!(_selectedNodeParentList.ContainsKey(parentDepartmentGuid)) && currentLoadLevel > _maxLoadLevelTree)
                return;

            //if (parent.isChildsLoaded==null || !(bool) parent.isChildsLoaded)
            //{
            List<CustomTreeNode> treeNodeChildNodesList = parent.ChildNodes.Cast<CustomTreeNode>().ToList();

            foreach (var departmentNode in GetDepartmentState(parentDepartmentGuid))
            {
                CustomTreeNode node =
                    treeNodeChildNodesList.FirstOrDefault(n => n.Value.Equals(departmentNode.Key.ToString()));
                if (node == null)
                {
                    node = CreateCustomTreeNode(DepartmentTreeNodeType.DepartmentState,
                        departmentNode.Value,
                        departmentNode.Key);
                    parent.ChildNodes.Add(node);
                }
                if (isLoadFirst && !_selectedNodeParentList.ContainsKey(parentDepartmentGuid)) return;

                bool hasChild = IsHasChilds(DepartmentTreeNodeType.DepartmentState, departmentNode.Key);

                if (hasChild)
                {
                    if ((_selectedNodeParentList.ContainsKey(parentDepartmentGuid)))
                    {
                        CreateDepartmentStateTreeChildNodes(node, departmentNode.Key, ref currentLoadLevel, false);
                    }
                    else
                    {
                        currentLoadLevel++;
                        CreateDepartmentStateTreeChildNodes(node, departmentNode.Key, ref currentLoadLevel, true);
                        currentLoadLevel--;
                    }
                }
            }
            //}
            //parent.isChildsLoaded = true;
        }

        /// <summary>
        ///     Обработчик события клика по элементу дерева
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeNodeExpanded(object sender, TreeNodeEventArgs e)
        {
            int currentLoadLevel = 1;
            if (((DepartmentTreeNodeType) ((CustomTreeNode) e.Node).Tag) == DepartmentTreeNodeType.DivisionState)
                CreateDivisionStateTreeChildNodes((CustomTreeNode) e.Node, new Guid(e.Node.Value), ref currentLoadLevel,
                    false);
            else
                CreateDepartmentStateTreeChildNodes((CustomTreeNode) e.Node, new Guid(e.Node.Value),
                    ref currentLoadLevel, false);
        }
    }
}