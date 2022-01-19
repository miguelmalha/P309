using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using P309_2.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace P309_2.Models
{
    public class Initializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
         {
            var Methods = new List<Payment_Methods>
            {
            new Payment_Methods{Payment_Method="Bank Transfer"},
            new Payment_Methods{Payment_Method="Cash"},
            new Payment_Methods{Payment_Method="Checks"},
            new Payment_Methods{Payment_Method="Credit Card"},
            new Payment_Methods{Payment_Method="Debit Card"}
            };

            Methods.ForEach(s => context.Payment_Method.Add(s));
            context.SaveChanges();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var user = new ApplicationUser
            {
                UserName = "admin@admin.pt",
                Email ="admin@admin.pt",
             };

            userManager.Create(user, "AdminPassword");

            context.Roles.Add(new IdentityRole { Name = "Admin" });
            context.Roles.Add(new IdentityRole { Name = "Colab" });
            context.SaveChanges();

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            userManager.AddToRole(user.Id, "Admin");

            var Areas = new List<Development_Areas>
            {
            new Development_Areas{Development_Area="Accounting"},
            new Development_Areas{Development_Area="Consulting"}, 
            new Development_Areas{Development_Area="Finance"},
            new Development_Areas{Development_Area="Management"},
            new Development_Areas{Development_Area="Marketing"},
            new Development_Areas{Development_Area="Operations"},
            new Development_Areas{Development_Area="Technology"},
            };

            Areas.ForEach(s => context.Development_Area.Add(s));
            context.SaveChanges();

            var Stages = new List<Development_Stages>
            {
            new Development_Stages{Development_Stage="Definition"},
            new Development_Stages{Development_Stage="Initiation"},
            new Development_Stages{Development_Stage="Planning"},
            new Development_Stages{Development_Stage="Execution"},
            new Development_Stages{Development_Stage="Monitoring and Control"},
            new Development_Stages{Development_Stage="Closure"},
            new Development_Stages{Development_Stage="Abandoned"}
            };

            Stages.ForEach(s => context.Development_Stage.Add(s));
            context.SaveChanges();
        }
    }
}