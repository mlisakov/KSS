using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using HomeProjectWebForms.Helpers;

namespace HomeProjectWebForms.Pages.Main
{
    public partial class Favorites : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void AddFavoritesClicked(object sender, ImageClickEventArgs e)
        {
            var currentUserGuid = new Guid(Session["CurrentUser"].ToString());
            string commandArguents = ((ImageButton) sender).CommandArgument;
            string commandText = "RemoveFavorites";
            var favoriteUserGuid = new Guid(commandArguents);

            SqlConnection connection = DBHelper.GetConnection();

            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.StoredProcedure};
            cmd.Parameters.Add("@ParentUserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@ParentUserGuid"].Value = currentUserGuid;
            cmd.Parameters.Add("@FavoriteUserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@FavoriteUserGuid"].Value = favoriteUserGuid;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка добаления сотрудника в избранное", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected bool CheckVisibility(string data)
        {
            if (string.IsNullOrEmpty(data))
                return false;
            return true;
        }

        protected string GetUserPhoneNumbers(object guid)
        {
            return DBHelper.GetUserPhoneNumbers(new Guid(guid.ToString()));
        }

        protected string GetUserSpecificPhoneNumbers(object guid)
        {
            return DBHelper.GetUserSpecificPhoneNumbers(new Guid(guid.ToString()));
        }
    }
}