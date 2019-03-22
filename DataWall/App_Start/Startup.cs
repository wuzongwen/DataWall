using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartupAttribute(typeof(DataWall.App_Start.Startup), "Configuration")]
namespace DataWall.App_Start
{
    public class Startup
    {
        public static void ConfigureSignalR(IAppBuilder app)
        {
            app.MapSignalR();
        }

        public static void Configuration(IAppBuilder app)
        {
            Startup.ConfigureSignalR(app);
        }
    }
}