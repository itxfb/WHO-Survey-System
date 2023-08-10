
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Configuration;
using System.Data.SqlClient;

namespace WHO_Survey_System.App_Start
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SqlConnection>(new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseEntities"].ConnectionString.Replace("metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=\"", "").Replace("\"", "")));
        }
        public void Configuration(IAppBuilder app)
        {
            
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Auth/Login")
            });
        }
    }
}