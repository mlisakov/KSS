using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using HomeProjectWebForms.Helpers;

namespace HomeProjectWebForms.Pages.Main
{
    public partial class Employee : Page
    {
        private DropDownList _currentRegionList;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string GetBindingValue(string status)
        {
//            string value = status == "BirthDay" ? Eval(status, "{0:dd.MM.yyyy}") : Eval(status).ToString();
            string value = status == "BirthDay" ? Eval(status, "{0:dd.MM}") : Eval(status).ToString();
            if (string.IsNullOrEmpty(value))
                return "Не указано";
            return value;
        }

        protected void AddFavoritesClicked(object sender, ImageClickEventArgs e)
        {
            var currentUserGuid = new Guid(Session["CurrentUser"].ToString());
            string[] commandArguents = ((ImageButton) sender).CommandArgument.Split(';');
            bool isFavorite = bool.Parse(commandArguents[1]);
            var favoriteUserGuid = new Guid(commandArguents[0]);

            SqlConnection connection = DBHelper.GetConnection();
            string commandText = isFavorite ? "RemoveFavorites" : "AddFavorites";
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.StoredProcedure};
            cmd.Parameters.Add("@ParentUserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@ParentUserGuid"].Value = currentUserGuid;
            cmd.Parameters.Add("@FavoriteUserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@FavoriteUserGuid"].Value = favoriteUserGuid;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                EmployeeFormView.DataBind();
                //string popupScript = "<script language='javascript'>" +"showSuccessToast();" +"</script>";
                //Page.ClientScript.RegisterStartupScript(typeof(Page), "popupScript", popupScript);
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

        public string GetImage(string status)
        {
            bool isFavorite = bool.Parse(status);
            if (isFavorite)
                return "/Images/Premium_Fill.png";
            return "/Images/Premium.png";
        }

        //protected void EmployeeFormView_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        //{
        //    TextBox streetTextBox = (TextBox) EmployeeFormView.FindControl("StreetTextBox");
        //    string currentStreet = streetTextBox.Text;
        //    TextBox edificeTextBox = (TextBox) EmployeeFormView.FindControl("EdificeTextBox");
        //    string currentEdifice = edificeTextBox.Text;
        //    DropDownList regionList = (DropDownList) EmployeeFormView.FindControl("RegionList");
        //    string currentRegion = regionList.SelectedValue;
        //    UpdateEmployeeAddress(new Guid(currentRegion), currentStreet, currentEdifice);
        //}

        private void UpdateEmployeeAddress(Guid EmployeePlaceId, Guid region, string street, string edifice,
            string phone, Guid phoneType)
        {
            if (region == Guid.Empty)
            {
                string script = "<script type='text/javascript'>showError(";
                script += "'Ошибка выбора региона для сотрудника!'";
                script += ");</script>";
                ScriptManager.RegisterStartupScript(Page, GetType(), "tmp3", script, false);
                return;
            }
            const string commandText = "UpdateEmployeeAddress";
            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.StoredProcedure};
            cmd.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@Guid"].Value = new Guid(Request.QueryString["ID"]);
            cmd.Parameters.Add("@EmployeePlaceId", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@EmployeePlaceId"].Value = EmployeePlaceId;
            cmd.Parameters.Add("@Region", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@Region"].Value = region;
            cmd.Parameters.Add("@Street", SqlDbType.NVarChar);
            cmd.Parameters["@Street"].Value = street;
            cmd.Parameters.Add("@Edifice", SqlDbType.NVarChar);
            cmd.Parameters["@Edifice"].Value = edifice;
            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar);
            cmd.Parameters["@Phone"].Value = phone;
            cmd.Parameters.Add("@PhoneType", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@PhoneType"].Value = phoneType;
            try
            {
                connection.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка обновления адреса сотрудника", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected void EmployeeFormView_ItemCreated(object sender, EventArgs e)
        {
            //EmployeeFormView.FindControl("Edit").Visible = false;
            if (!DBHelper.CheckIfUserIsAdministrator(new Guid(Session["CurrentUser"].ToString())))
            {
                if (EmployeeFormView.CurrentMode == FormViewMode.ReadOnly)
                {
                    EmployeeFormView.FindControl("Edit").Visible = false;
                }
            }
        }

        protected void DivisionDropDownList_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var gridViewRow = (GridViewRow) ((DropDownList) sender).Parent.Parent;

            var ddlist = (DropDownList) gridViewRow.FindControl("RegionList");
            _currentRegionList = ddlist;
            ddlist.DataBind();
        }

        protected void DivisionDropDownList_OnSelectedIndexChanged2(object sender, EventArgs e)
        {
            var ddlist = (DropDownList) EmployeeFormView.FindControl("RegionList2");
            ddlist.DataBind();
        }

        protected void DivisionDropDownList_OnSelectedIndexChanged3(object sender, EventArgs e)
        {
            var ddlist = (DropDownList) EmployeeFormView.FindControl("RegionList3");
            ddlist.DataBind();
        }

        protected string SetSelectedRegion(object seletGuid)
        {
            return seletGuid.ToString();
        }

        protected void EmployeeFormView_OnModeChanged(object sender, EventArgs e)
        {
            //if (EmployeeFormView.CurrentMode == FormViewMode.Edit)
            //{
            //    DropDownList ddlist = (DropDownList) EmployeeFormView.FindControl("RegionList");
            //    DataView dv = (DataView) EmployeeDataSource.Select(DataSourceSelectArguments.Empty);
            //    DataTable t = dv.ToTable();
            //    var y = t.Rows[0]["LocalityId"];
            //    ddlist.SelectedValue = y.ToString();
            //}
        }

        protected void UpdatePhone(object sender, GridViewUpdateEventArgs e)
        {
            var GridView1 = (GridView) EmployeeFormView.FindControl("UserPhonesGridView");
            var EmployeePlaceId = (Guid) e.Keys[0];
            string currentStreet = ((TextBox) GridView1.Rows[e.RowIndex]
                .FindControl("StreetTextBox")).Text;
            string currentEdifice = ((TextBox) GridView1.Rows[e.RowIndex]
                .FindControl("EdificeTextBox")).Text;
            string currentRegion = ((DropDownList) GridView1.Rows[e.RowIndex]
                .FindControl("RegionList")).SelectedValue;

            string phone = ((TextBox) GridView1.Rows[e.RowIndex]
                .FindControl("txtCompanyName1")).Text;

            string phoneType = ((DropDownList) GridView1.Rows[e.RowIndex]
                .FindControl("PhoneTypeDropDownList")).SelectedValue;

            UpdateEmployeeAddress(EmployeePlaceId, new Guid(currentRegion), currentStreet, currentEdifice, phone,
                new Guid(phoneType));
        }

        protected void DeletePhone(object sender, ImageClickEventArgs e)
        {
            var remove = (ImageButton) sender;
            string commandText = "delete from EmployeePlace where Id='" + remove.CommandArgument + "'";
            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.Text};
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка удаления номера", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected void UpdateSelfPhone(object sender, GridViewUpdateEventArgs e)
        {
            var GridView1 = (GridView)EmployeeFormView.FindControl("GridView2");
            var SpecificStaffPlaceId = (Guid)e.Keys[0];
            string currentPhoneNumber = ((TextBox)GridView1.Rows[e.RowIndex]
                .FindControl("txtCompanyName1")).Text;
            string currentPhoneType = ((DropDownList)GridView1.Rows[e.RowIndex]
                .FindControl("PhoneTypeDropDownList5")).SelectedValue;
            string commandText = "update SpecificStaffPlace set PhoneTypeId='" + currentPhoneType + "',PhoneNumber='" + currentPhoneNumber + "' where Id='" + SpecificStaffPlaceId + "'";
            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) { CommandType = CommandType.Text };
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка обновления стационарного номера", ex);
            }
            finally
            {
                connection.Close();
            }

        }

        protected void DeleteSelfPhone(object sender, ImageClickEventArgs e)
        {
            var remove = (ImageButton)sender;
            string commandText = "delete from SpecificStaffPlace where Id='" + remove.CommandArgument + "'";
            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) { CommandType = CommandType.Text };
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка удаления стационарного номера", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected void AddNewPhone(object sender, EventArgs e)
        {
            var PhoneTypeDDL = (DropDownList) EmployeeFormView.FindControl("PhoneTypeDDL");
            var regionList2 = (DropDownList) EmployeeFormView.FindControl("RegionList2");
            var txtContactName = (TextBox) EmployeeFormView.FindControl("txtContactName");
            var locationId = new Guid(regionList2.SelectedValue);
            var phoneTypeId = new Guid(PhoneTypeDDL.SelectedValue);
            string phone = txtContactName.Text;
            var currentUserGuid = new Guid(Request.QueryString["ID"]);

            string commandText =
                "insert into EmployeePlace(Id,EmployeeId,LocationId,PhoneTypeId,PhoneNumber) values ('" + Guid.NewGuid() +
                "','" + currentUserGuid + "','" + locationId + "','" + phoneTypeId + "','" + phone + "')";
            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.Text};
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                EmployeeFormView.DataBind();
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка добавления номера", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected void AddNewSelfPhone(object sender, EventArgs e)
        {
            var PhoneTypeDDL = (DropDownList) EmployeeFormView.FindControl("DropDownList2");
            var regionList2 = (DropDownList) EmployeeFormView.FindControl("RegionList3");
            var SpecificStaffDDL = (DropDownList)EmployeeFormView.FindControl("SpecificStaffDDL");
            var txtContactName = (TextBox) EmployeeFormView.FindControl("TextBox1");
            var locationId = new Guid(regionList2.SelectedValue);
            var phoneTypeId = new Guid(PhoneTypeDDL.SelectedValue);
            var specificStaffId = new Guid(SpecificStaffDDL.SelectedValue);
            string phone = txtContactName.Text;

            string commandText =
                "insert into SpecificStaffPlace(Id,SpecificStaffId,LocationId,PhoneTypeId,PhoneNumber) values ('" + Guid.NewGuid() +
                "','" + specificStaffId + "','" + locationId + "','" + phoneTypeId + "','" + phone + "')";
            SqlConnection connection = DBHelper.GetConnection();
            var cmd = new SqlCommand(commandText, connection) {CommandType = CommandType.Text};
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                EmployeeFormView.DataBind();
            }
            catch (Exception ex)
            {
                ShellLogger.WriteLog("DB.log", "Ошибка добавления стационарного номера", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected bool IsAdminMode()
        {
            return DBHelper.CheckIfUserIsAdministrator(new Guid(Session["CurrentUser"].ToString()));
        }

        protected void EmployeeFormView_OnItemCreated(object sender, EventArgs e)
        {
            bool isAdmin = IsAdminMode();
            string script = "<script type='text/javascript'>hideAddingButtons(";
            script += isAdmin ? "1" : "0";
            script += ");</script>";
            ScriptManager.RegisterStartupScript(Page, GetType(), "tmp", script, false);
        }

        protected string UpdateSelectedValue(object eval)
        {
            if (_currentRegionList != null)
            {
                ListItemCollection items = _currentRegionList.Items;
                if (eval == null || items.FindByValue(eval.ToString()) == null)
                {
                    _currentRegionList = null;
                    return Guid.Empty.ToString();
                }
            }
            return eval.ToString();
        }

        protected string UpdateDivisionDDL2SelectedValue(object eval)
        {
            if (_currentRegionList != null)
            {
                ListItemCollection items = _currentRegionList.Items;
                if (eval == null || items.FindByValue(eval.ToString()) == null)
                {
                    _currentRegionList = null;
                    return Guid.Empty.ToString();
                }
            }
            return eval.ToString();
        }
    }
}