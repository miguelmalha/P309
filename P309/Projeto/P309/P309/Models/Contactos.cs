using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

        namespace P309.Models
    {
        public class Contactos
        {
        /*
          Contem um ID como primary key
          Strings para informações 
          string EmpresaNIF como foreign key para fazer associação à empresa 
          Uma var tipo Empresas para associar a empresa
        */
            public int ID { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Telefone { get; set; }
            public string Notas { get; set; }
            public string EmpresaNIF { get; set; }
            public Empresas Empresa { get; set; } // Cada contacto terá associado uma empresa
        }
    }
