using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace P309.Models
{
    public class Empresas
    {

        /*
          Contem um NIF como primary key
          Strings para informações 
          Uma coleção de contactos que estarão associados à empresa
        */
        public string Nome { get; set; }
        [Key]
        public string NIF { get; set; }
        public string Endereço { get; set; }
        public string País { get; set; }
        public string Localidade { get; set; }
        public string CódigoPostal { get; set; }
        public string Site { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string IBAN { get; set; }
        public string MeioDePagamento { get; set; }
        public string PrazoDePagamento { get; set; }
        public string Notas { get; set; }
        public ICollection<Contactos> Contactos { get; set; } // As empresas terão uma coleção de contactos, ou seja, vários contactos relacionados/associados a uma empresa
    }
}