using System.Web.UI.WebControls;

namespace HomeProjectWebForms.Controls
{
    /// <summary>
    ///     Переопределенный элемент дерева с возможностью записи в поле Tag
    /// </summary>
    public class CustomTreeNode : TreeNode
    {
        public CustomTreeNode()
        {
        }

        public CustomTreeNode(TreeView owner, bool isRoot)
            : base(owner, isRoot)
        {
        }

        /// <summary>
        ///     Gets or sets the object that contains data about the tree node.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        ///     Restores view-state information from a previous page request that
        ///     was saved by the SaveViewState method.
        /// </summary>
        /// <param name="state">
        ///     An Object that represents the control state to be restored.
        /// </param>
        protected override void LoadViewState(object state)
        {
            var arrState = state as object[];

            Tag = arrState[0];
            base.LoadViewState(arrState[1]);
        }

        /// <summary>
        ///     Saves any server control view-state changes that have occurred
        ///     since the time the page was posted back to the server.
        /// </summary>
        /// <returns>
        ///     Returns the server control's current view state. If there is no
        ///     view state associated with the control, this method returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            var arrState = new object[2];
            arrState[1] = base.SaveViewState();
            arrState[0] = Tag;

            return arrState;
        }
    }
}