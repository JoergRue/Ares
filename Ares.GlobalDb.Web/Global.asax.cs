using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Ares.GlobalDb.Web
{
    public class Global : System.Web.HttpApplication
    {
        public class AresDbAppHost : ServiceStack.WebHost.Endpoints.AppHostBase
        {
            public AresDbAppHost()
                : base("RPG MusicTags", typeof(Ares.GlobalDB.Services.DownloadService).Assembly)
            {
            }

            public override void Configure(Funq.Container container)
            {
                Plugins.Add(new ServiceStack.Razor.RazorFormat());
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            new AresDbAppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}