using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;

namespace FileManagerService {
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration { EnableDetailedErrors = true };
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(hubConfiguration);
        }
    }
}