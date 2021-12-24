using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using P309_2.Models;
using System;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(P309_2.Startup))]
namespace P309_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
