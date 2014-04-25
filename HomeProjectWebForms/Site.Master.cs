//В режиме отладки отключается доменная авторизация

//# define DEBUG
//В режиме релиза включается доменная авторизация

# define RELEASE

using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using HomeProjectWebForms.Helpers;

namespace HomeProjectWebForms
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;        

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            HttpCookie requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string) ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string) ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
#if RELEASE
                if (Context.User.Identity.IsAuthenticated)
                {
//                    string userLogin = Helpers.GUIHelper.GetLogin(Context.User.Identity);
                    string userLogin = Context.User.Identity.Name;
                    Guid userGuid;
                    string divisionName;
                    string userName = Helpers.DBHelper.GetUserFullName(userLogin, out userGuid);
                    Guid userDivision = Helpers.DBHelper.GetUserDivision(userGuid, out divisionName);
                    Guid userDepartment = Helpers.DBHelper.GetUserDepartment(userGuid);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        UserName.Text = divisionName + "," + userName;
                        Session["CurrentUser"] = userGuid;
                        Session["CurrentUserDepartment"] = userDepartment;
                        Session["CurrentUserDivision"] = userDivision;
                    }
                    else
                    {
                        UserName.Text = "Нераспознанное имя:" + Context.User.Identity.Name;
                        Session["CurrentUser"] = Guid.Empty;
                        Session["CurrentUserDepartment"] = Guid.Empty;
                        Session["CurrentUserDivision"] = Guid.Empty;
                        UserName.Text += "Пустой департамент:";
                    }
                }
                else
                {
                    Session["CurrentUser"] = Guid.Empty;
                    Session["CurrentUserDepartment"] = Guid.Empty;
                    Session["CurrentUserDivision"] = Guid.Empty;
                    UserName.Text = "Неавторизованный пользователь:" + Context.User.Identity.Name;
                }
#else
                //Гуид пользователя с логином admin
                //Введен для отладки приложения
                string divisionName;
                Session["CurrentUser"] = "5778A303-AF2C-4579-950F-4370947D0162";
                Session["CurrentUserDepartment"] =
                    DBHelper.GetUserDepartment(new Guid("5778A303-AF2C-4579-950F-4370947D0162"));
                Session["CurrentUserDivision"] =
                    DBHelper.GetUserDivision(new Guid("5778A303-AF2C-4579-950F-4370947D0162"), out divisionName);
                UserName.Text = divisionName + ",admin";

#endif
            }
        }
    }
}