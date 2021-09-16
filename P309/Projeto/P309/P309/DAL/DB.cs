using P309.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace P309.DAL
{
    public class DB : DbContext
    {
        public DB() : base("DB")
        {
            Database.SetInitializer(new DBInitializer()); //Inicializa a db

        }

        //define as coleções existentes 
        public System.Data.Entity.DbSet<P309.Models.Contactos> Contactos { get; set; }
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<Conta> Conta { get; set; }
        public DbSet<Projetos> Projetos { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); //Tira o plural dos nomes

        }
        public System.Data.Entity.DbSet<P309.Models.Eventos> Eventos { get; set; }
    }
}