using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using P309.Models;
using P309.DAL;

namespace P309.DAL
{
    public class DBInitializer : DropCreateDatabaseIfModelChanges<DB>
    {
        protected override void Seed(DB context)
        {
            //Incicializa a db com uma empresa "SEM EMPRESA". É necessária uma empresa para que se possa adicionar um contacto.
            var empresas = new List<Empresas>
            {
             new Empresas{NIF ="123456789", Nome="Sem empresa"} //empresa que não pode ser eliminada
            };

            empresas.ForEach(s => context.Empresas.Add(s)); //adiciona essa empresa
            context.SaveChanges(); //grava alterações

            // Incicializa a db com uma empresa "Sem informação". Esta está associada à empresa anteriormente inserida através do EmpresaNIF
            var contacto = new List<Contactos>
            {
             new Contactos{ID = 1, Nome="Sem informação", EmpresaNIF = "123456789"}
            };

            contacto.ForEach(s => context.Contactos.Add(s)); //adiciona esse contacto
            context.SaveChanges(); //grava alterações


            //Adiciona 3 contas incialmente, 1 administrador e 2 utilizadores
            var Conta = new List<Conta>
            {
             new Conta{NomeDeUtilizador="admin", PalavraPasse="admin",Role=0},
             new Conta{NomeDeUtilizador="user1", PalavraPasse="user1",Role=1},
             new Conta{NomeDeUtilizador="user2", PalavraPasse="user2",Role=1}
            };

            Conta.ForEach(s => context.Conta.Add(s)); //adiciona as contas
            context.SaveChanges(); //grava alterações
            base.Seed(context);
        }
    }
}