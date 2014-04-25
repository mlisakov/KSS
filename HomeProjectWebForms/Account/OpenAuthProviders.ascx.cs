using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Membership.OpenAuth;

namespace HomeProjectWebForms.Account
{
    public partial class OpenAuthProviders : UserControl
    {
        public string ReturnUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string provider = Request.Form["provider"];
                if (provider == null)
                {
                    return;
                }

                string redirectUrl = "~/Account/RegisterExternalLogin";
                if (!String.IsNullOrEmpty(ReturnUrl))
                {
                    string resolvedReturnUrl = ResolveUrl(ReturnUrl);
                    redirectUrl += "?ReturnUrl=" + HttpUtility.UrlEncode(resolvedReturnUrl);
                }

                OpenAuth.RequestAuthentication(provider, redirectUrl);
            }
        }


        public IEnumerable<ProviderDetails> GetProviderNames()
        {
            return OpenAuth.AuthenticationClients.GetAll();
        }
    }
}