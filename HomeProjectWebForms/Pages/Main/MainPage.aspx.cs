using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using HomeProjectWebForms.Consts;
using HomeProjectWebForms.Controls;
using HomeProjectWebForms.Helpers;
using HomeProjectWebForms.Helpers.GUIHeplers;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HomeProjectWebForms.Pages.Main
{
    public partial class MainPage : Page
    {
        private static bool _isAdditionalSearch;
        private int _birthdaySelector;
        private Guid _lastGroup = Guid.Empty;
        //private static LyncClient _LyncClient=null;
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //InitUserDepartment();

                SetVisibilityReportButtons();
                if (Session["Tree"] != null)
                    if (Session["CurrentSelectedDepartmentNode"] != null)
                        DepartmentTree.FillTree((CustomTreeNode) Session["Tree"],
                            (CustomTreeNode) Session["CurrentSelectedDepartmentNode"]);
                    else
                        DepartmentTree.FillTree((CustomTreeNode) Session["Tree"], null);
                else
                {
                    if (Session["CurrentUserDivision"] != null)
                    {
                        DepartmentTree.FillTreeByDivision(new Guid(Session["CurrentUserDivision"].ToString()));
                        Session["CurrentSelectedDepartment"] = new Guid(Session["CurrentUserDivision"].ToString());
                        Session["CurrentSelectedDepartmentNode"] = DepartmentTree.CurrentSelectedNode;
                    }
                    else
                        DepartmentTree.FillTree(Guid.Empty);
                }

                if (Session["Tree"] == null)
                    Session["Tree"] = DepartmentTree.Nodes[0];


                if (Session["SimpleSearch"] != null && !string.IsNullOrEmpty(Session["SimpleSearch"].ToString()))
                {
                    SearchTextBox.Text = Session["SimpleSearch"].ToString();
                    EmployeesListView.DataSourceID = "FilteredEmployeesDataSource";
                    EmployeesListView.DataBind();
                }
                else if (Session["CustomSearch"] != null && !string.IsNullOrEmpty(Session["CustomSearch"].ToString()))
                {
                    var customSearchParams = (CustomSearchStorage) Session["CustomSearch"];
                    PositionTextBox.Text = customSearchParams.Position;
                    DepartmentTextBox.Text = customSearchParams.Department;
                    HasPhoneCheckBox.Checked = customSearchParams.HasPhone;
                    RestoreBirthdayDates(customSearchParams.StartBirthday, customSearchParams.FinishBirthday);
                    //BirthDayStartTextBox.Text = customSearchParams.StartBirthday.ToShortDateString();
                    //BirthDayFinishTextBox.Text = customSearchParams.FinishBirthday.ToShortDateString();

                    UpdateCustomSearchParametrs();
                    EmployeesListView.DataSourceID = "CustomFilteredEmployeesDataSource";
                    EmployeesListView.DataBind();
                }

                if (Session["SelectedPage"] != null)
                {
                    var pager = ((DataPager) EmployeesListView.FindControl("DataPager"));
                    if (pager != null)
                        pager.SetPageProperties(int.Parse(Session["SelectedPage"].ToString()), pager.MaximumRows, true);
                }
            }
        }

        //private void InitUserDepartment()
        //{
        //    if (Session["CurrentUserDepartment"] != null && Session["CurrentUser"]!=null)
        //        Session["CurrentUserDepartment"] =
        //            DBHelper.GetUserDepartment(new Guid(Session["CurrentUser"].ToString()));
        //}

        protected void DepartmentTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            //Запоминаем выбранный элемент в дереве только для DepartmentState
            if (((DepartmentTreeNodeType) ((CustomTreeNode) DepartmentTree.SelectedNode).Tag) ==
                DepartmentTreeNodeType.DepartmentState)
            {
                Session["CurrentSelectedDepartment"] = DepartmentTree.SelectedValue;
                Session["Tree"] = DepartmentTree.Nodes[0];
                Session["CurrentSelectedDepartmentNode"] = DepartmentTree.SelectedNode;
            }

            SearchTextBox.Text = string.Empty;
            Session["SimpleSearch"] = null;
            Session["CustomSearch"] = null;
            Session["SelectedPage"] = 1;

            EmployeesListView.DataSourceID = "EmployeesDataSource";
            EmployeesListView.DataBind();
        }

        protected void AddFavoritesClicked(object sender, ImageClickEventArgs e)
        {
            //Thread.Sleep(40000);
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
                EmployeesListView.DataBind();
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

        /// <summary>
        ///     Простой поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SimpleSearchClicked(object sender, EventArgs e)
        {
            //обычный поиск
            Session["SelectedPage"] = 1;
            Session["SimpleSearch"] = SearchTextBox.Text;
            Session["CustomSearch"] = null;
            EmployeesListView.DataSourceID = "FilteredEmployeesDataSource";
            EmployeesListView.DataBind();
        }

        /// <summary>
        ///     Расширенный поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AdditionalSearchClicked(object sender, EventArgs e)
        {
            //расширенный поиск
            Session["SelectedPage"] = 1;
            var customSearch = new CustomSearchStorage
            {
                Department = DepartmentTextBox.Text,
                Position = PositionTextBox.Text,
                HasPhone = HasPhoneCheckBox.Checked,
            };

            var birthDayStart = Page.Request.Form["birthDayStartTextBox"];
            var birthDayEnd = Page.Request.Form["birthDayFinishTextBox"];

            var startDate = ParseDate(birthDayStart);
            var endDate = ParseDate(birthDayEnd);

            RestoreBirthdayDates(startDate, endDate);

            if (startDate.HasValue)
                customSearch.StartBirthday = startDate.Value;

            if (endDate.HasValue)
                customSearch.FinishBirthday = endDate.Value;

            Session["SimpleSearch"] = null;
            Session["CustomSearch"] = customSearch;

            UpdateCustomSearchParametrs();
            EmployeesListView.DataSourceID = "CustomFilteredEmployeesDataSource";
            EmployeesListView.DataBind();
        }

        /// <summary>
        /// Parse dateString
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateTime? ParseDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;

            string[] parts = date.Split('.');
            if (parts.Length < 2)
                return null;

            try
            {
                List<Int16> dates = parts.Select(t => Convert.ToInt16(t)).ToList();
                
                return new DateTime(dates[2], dates[1], dates[0]);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void UpdateCustomSearchParametrs()
        {
            var customSearchParams = (CustomSearchStorage) Session["CustomSearch"];
            var oldbirthDayStart = customSearchParams.StartBirthday;
            var oldbirthDayEnd = customSearchParams.FinishBirthday;
            var birthDayStart = Page.Request.Form["birthDayStartTextBox"];
            var birthDayEnd = Page.Request.Form["birthDayFinishTextBox"];

            CustomFilteredEmployeesDataSource.SelectParameters.Clear();
            CustomFilteredEmployeesDataSource.SelectParameters.Add("UserGuid", Session["CurrentUser"].ToString());
            if (!string.IsNullOrEmpty(PositionTextBox.Text))
                CustomFilteredEmployeesDataSource.SelectParameters.Add("Position", PositionTextBox.Text);
            if (!string.IsNullOrEmpty(DepartmentTextBox.Text))
                CustomFilteredEmployeesDataSource.SelectParameters.Add("Department", DepartmentTextBox.Text);
            if (HasPhoneCheckBox.Checked)
                CustomFilteredEmployeesDataSource.SelectParameters.Add("HasPhone", DbType.Boolean, "true");

            if (!string.IsNullOrEmpty(birthDayStart))
            {
                var time = ParseDate(birthDayStart);
                if (time.HasValue)
                    CustomFilteredEmployeesDataSource.SelectParameters.Add("StartBirthdayDate", DbType.DateTime,
                        time.Value.ToShortDateString());
            }
            else if (!string.IsNullOrEmpty(oldbirthDayStart.ToShortDateString()) && oldbirthDayStart>DateTime.MinValue)
            {
                CustomFilteredEmployeesDataSource.SelectParameters.Add("StartBirthdayDate", DbType.DateTime,
                    oldbirthDayStart.ToShortDateString());
            }

            if (!string.IsNullOrEmpty(birthDayEnd))
            {
                var time = ParseDate(birthDayEnd);
                if (time.HasValue)
                    CustomFilteredEmployeesDataSource.SelectParameters.Add("FinishBirthdayDate", DbType.DateTime,
                        time.Value.ToShortDateString());
            }
            else if (!string.IsNullOrEmpty(oldbirthDayEnd.ToShortDateString()) && oldbirthDayEnd > DateTime.MinValue)
            {
                CustomFilteredEmployeesDataSource.SelectParameters.Add("FinishBirthdayDate", DbType.DateTime,
                    oldbirthDayEnd.ToShortDateString());
            }
        }

        protected void ExportToExcel(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=\"Report.xlsx\"");

            using (var memoryStream = new MemoryStream())
            {
                CreateXLWorkbook().SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();
        }

        private void RestoreBirthdayDates(DateTime? startDate, DateTime? finishDate)
        {
            //Page.Request.Form["birthDayStartTextBox"] = startDate.ToShortDateString();
            string script = "<script type='text/javascript'>SetInputValues('";
            
            if (startDate.HasValue)
                script += startDate.Value.ToShortDateString();
            script += "','";

            if (finishDate.HasValue)
                script += finishDate.Value.ToShortDateString();

            script += "');</script>";
            ScriptManager.RegisterStartupScript(Page, GetType(), "tmp", script, false);
        }

        protected void PrintSearchResult(object sender, ImageClickEventArgs e)
        {
            #region output directly in browser

            //Byte[] fileBuffer = CreatePDFDocument().ToArray();
            //Response.Clear();
            //Response.ClearContent();
            //Response.ClearHeaders();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("Content-Disposition", "inline; filename=\"Report.pdf\"");
            //Response.AddHeader("content-length", fileBuffer.Length.ToString());
            //Response.AppendHeader("Accept-Ranges", "bytes");
            //Response.BinaryWrite(fileBuffer);
            //Response.Flush();
            //Response.End();

            #endregion

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=\"Report.pdf\"");
            Byte[] fileBuffer = CreatePDFDocument().ToArray();
            Response.BinaryWrite(fileBuffer);
            Response.End();
        }

        private XLWorkbook CreateXLWorkbook()
        {
            SqlDataSource datasource;
            if (Session["SimpleSearch"] != null && !string.IsNullOrEmpty(Session["SimpleSearch"].ToString()))
                datasource = FilteredEmployeesDataSource;
            else if (Session["CustomSearch"] != null && !string.IsNullOrEmpty(Session["CustomSearch"].ToString()))
                datasource = CustomFilteredEmployeesDataSource;
            else
                datasource = EmployeesDataSource;

            var dv = (DataView) datasource.Select(DataSourceSelectArguments.Empty);
            DataTable t = dv.ToTable();

            // Create the workbook
            var workbook = new XLWorkbook();
            IXLWorksheet ws = workbook.Worksheets.Add("Sample");
            int i = 1;
            ws.Cell(i, 1).SetValue("ФИО");
            ws.Cell(i, 2).SetValue("Подразделение");
            ws.Cell(i, 3).SetValue("Должность");
            ws.Cell(i, 4).SetValue("Рабочий телефон");
            ws.Cell(i, 5).SetValue("Личный телефон");
            ws.Cell(i, 6).SetValue("Почта");
            //ws.Cell(i, 7).SetValue("Адрес");
            i++;
            foreach (DataRow dr in t.Rows)
            {
                string Name = dr["Name"].ToString();
                string Department = dr["Department"].ToString();
                string Title = dr["Title"].ToString();
                //string Region = dr["Region"].ToString();
                //string Street = dr["Street"].ToString();
                //string Edifice = dr["Edifice"].ToString();
                string PhoneNumber = GetUserPhoneNumbers(dr["Id"]); //dr["PhoneNumber"].ToString();
                string SpecificPhoneNumber = GetUserSpecificPhoneNumbers(dr["Id"]);
                string EMail = dr["EMail"].ToString();
                ws.Cell(i, 1).SetValue(Name);
                ws.Cell(i, 2).SetValue(Department);
                ws.Cell(i, 3).SetValue(Title);
                ws.Cell(i, 4).SetValue(SpecificPhoneNumber);
                ws.Cell(i, 5).SetValue(PhoneNumber);
                ws.Cell(i, 6).SetValue(EMail);
                //ws.Cell(i, 7).SetValue(Region + "," + Street + "," + Edifice);
                i++;
            }
            return workbook;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        private void SetVisibilityReportButtons()
        {
            bool isVisible = EmployeesListView.Items.Count > 0;
            ImageButton2.Visible = isVisible;
            ImageButton3.Visible = isVisible;
        }

        protected void EmployeesListView_OnDataBound(object sender, EventArgs e)
        {
            SetVisibilityReportButtons();
        }

        protected void weekButtonClicked(object sender, ImageClickEventArgs e)
        {
            if (_birthdaySelector != 7)
                UpdateBirthdays(7);
        }

        protected void dayButtonClicked(object sender, ImageClickEventArgs e)
        {
            if (_birthdaySelector != 1)
                UpdateBirthdays(1);
        }

        protected void monthButtonClicked(object sender, ImageClickEventArgs e)
        {
            if (_birthdaySelector != 30)
                UpdateBirthdays(30);
        }

        private void UpdateBirthdays(int daysCount)
        {
            _birthdaySelector = daysCount;
            BirthdaysList.DataBind();
        }

        protected void ds_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            e.Command.Parameters[0].Value = _birthdaySelector == 0 ? 1 : _birthdaySelector;
        }

        private MemoryStream CreatePDFDocument()
        {
            SqlDataSource datasource;
            if (Session["SimpleSearch"] != null && !string.IsNullOrEmpty(Session["SimpleSearch"].ToString()))
                datasource = FilteredEmployeesDataSource;
            else if (Session["CustomSearch"] != null && !string.IsNullOrEmpty(Session["CustomSearch"].ToString()))
                datasource = CustomFilteredEmployeesDataSource;
            else
                datasource = EmployeesDataSource;

            var dv = (DataView) datasource.Select(DataSourceSelectArguments.Empty);
            DataTable t = dv.ToTable();

            var pdfDoc = new Document();
            var str = new MemoryStream();
            PdfWriter.GetInstance(pdfDoc, str);
            pdfDoc.Open();
            var table = new PdfPTable(1) {HorizontalAlignment = 0};
            string path = Server.MapPath("~/Fonts/Arial.ttf");
            BaseFont baseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new Font(baseFont, 11, Font.NORMAL);
            var headerFont = new Font(baseFont, 11, Font.BOLD);
            foreach (DataRow dr in t.Rows)
            {
                //string region = dr["Region"].ToString();
                //string street = dr["Street"].ToString();
                //string edifice = dr["Edifice"].ToString();

                var cell = new PdfPCell {HorizontalAlignment = 0};
                //cell.AddElement(new Phrase("ФИО:", headerFont));
                //cell.AddElement(new Phrase(dr["Name"].ToString(), font));
                //cell.AddElement(new Phrase("Подразделение:", headerFont));
                //cell.AddElement(new Phrase(dr["Department"] + Environment.NewLine, font));
                //cell.AddElement(new Phrase("Должность:", headerFont));
                //cell.AddElement(new Phrase(dr["Title"] + Environment.NewLine, font));
                //cell.AddElement(new Phrase("Телефон:", headerFont));
                //cell.AddElement(new Phrase(GetUserPhoneNumbers(dr["Id"]) + Environment.NewLine, font));
                //cell.AddElement(new Phrase("Рабочий телефон:", headerFont));
                //cell.AddElement(new Phrase("(" + dr["CityPhoneCode"] + ")" + dr["SpecificPhoneNumber"] + Environment.NewLine, font));
                //cell.AddElement(new Phrase("Почта:", headerFont));
                //cell.AddElement(new Phrase(dr["EMail"] + Environment.NewLine, font));
                //cell.AddElement(new Phrase("Адрес:", headerFont));
                //cell.AddElement(new Phrase(region + "," + street + "," + edifice + Environment.NewLine, font));
                //table.AddCell(cell);
                table.AddCell(
                    new PdfPCell(
                        new Phrase("ФИО:" + dr["Name"] + Environment.NewLine +
                                   "Подразделение:" + dr["Department"] + Environment.NewLine +
                                   "Должность:" + dr["Title"] + Environment.NewLine +
                                   "Личный телефон:" + GetUserPhoneNumbers(dr["Id"]) + Environment.NewLine +
                                   "Рабочий телефон:" + GetUserSpecificPhoneNumbers(dr["Id"]) +
                                   Environment.NewLine +
                                   "Почта:" + dr["EMail"], font)) {HorizontalAlignment = 0});
            }

            //table.AddCell(new PdfPCell(new Phrase("ФИО", headerFont)) { HorizontalAlignment = 1 });
            //table.AddCell(new PdfPCell(new Phrase("Подразделение", headerFont)) { HorizontalAlignment = 1 });
            //table.AddCell(new PdfPCell(new Phrase("Должность", headerFont)) { HorizontalAlignment = 1 });
            //table.AddCell(new PdfPCell(new Phrase("Рабочий телефон", headerFont)) { HorizontalAlignment = 1 });
            //table.AddCell(new PdfPCell(new Phrase("Телефон", headerFont)) {HorizontalAlignment = 1});
            //table.AddCell(new PdfPCell(new Phrase("Почта", headerFont)) { HorizontalAlignment = 1 });
            //table.AddCell(new PdfPCell(new Phrase("Адрес", headerFont)) { HorizontalAlignment = 1 });

            //foreach (DataRow dr in t.Rows)
            //{
            //    string Name = dr["Name"].ToString();
            //    string Department = dr["Department"].ToString();
            //    string Title = dr["Title"].ToString();
            //    string Region = dr["Region"].ToString();
            //    string Street = dr["Street"].ToString();
            //    string Edifice = dr["Edifice"].ToString();
            //    string PhoneNumber = GetUserPhoneNumbers(dr["Id"]);//dr["PhoneNumber"].ToString();
            //    string SpecificPhoneNumber = "(" + dr["CityPhoneCode"] + ")" + dr["SpecificPhoneNumber"];
            //    string EMail = dr["EMail"].ToString();

            //    table.AddCell(new Phrase(Name, font));
            //    table.AddCell(new Phrase(Department, font));
            //    table.AddCell(new Phrase(Title, font));
            //    table.AddCell(new Phrase(SpecificPhoneNumber, font));
            //    table.AddCell(new Phrase(PhoneNumber, font));
            //    table.AddCell(new Phrase(EMail, font));
            //    table.AddCell(new Phrase(Region + "," + Street + "," + Edifice, font));
            //}
            pdfDoc.Add(table);
            pdfDoc.Close();
            return str;
        }

        /// <summary>
        ///     Формирование заголовка группы
        /// </summary>
        /// <param name="status">Поле биндинга со значением</param>
        /// <returns>Заголовок группы</returns>
        public string GetBirthdayGroupName(string status)
        {
            Guid currentDepartment = Guid.Empty;

            string group = Eval(status).ToString();
            if (!string.IsNullOrEmpty(group))
            {
                Guid.TryParse(group, out currentDepartment);
            }

            if (!_lastGroup.Equals(currentDepartment))
            {
                _lastGroup = currentDepartment;
                return Eval("Department").ToString();
            }
            return string.Empty;
        }

        protected string GetLyncImage()
        {
            return "/Images/lync-icon.png";
        }

        protected bool CheckVisibility(string data)
        {
            if (string.IsNullOrEmpty(data))
                return false;
            return true;
        }

        protected void DataPager_OnPreRender(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (EmployeesListView.Items.Any())
                {
                    var pager = ((DataPager) sender);
                    Session["SelectedPage"] = pager.StartRowIndex; // / pager.MaximumRows;
                }
            }
        }

        protected string GetUserPhoneNumbers(object guid)
        {
            return DBHelper.GetUserPhoneNumbers(new Guid(guid.ToString()));
        }

        protected string GetFullUserPhoneNumbers(object guid)
        {
            return DBHelper.GetFullUserPhoneNumbers(new Guid(guid.ToString()));
        }

        protected string GetUserSpecificPhoneNumbers(object guid)
        {
            return DBHelper.GetUserSpecificPhoneNumbers(new Guid(guid.ToString()));
        }
    }
}