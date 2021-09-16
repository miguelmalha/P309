using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P309.Models
{
    public class Projetos
    {
        /*
          Contem um NumeroProjeto como primary key
          Strings para informações 
          DateTime para datas
          uma var tipo Contactos para referenciar um contacto ao projeto
        */
        public string Nome { get; set; }
        [Key]
        public int NumeroProjeto { get; set; }
        public string Area { get; set; }
        public string Estado { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")] //formatação da variavel DateTime
        public DateTime DataDeCriaçao { get; set; }
        public string Notas { get; set; }
        public int ContactoID { get; set; }
        public virtual Contactos Contacto { get; set; } // Cada projeto estará associado a um contacto 
    }
}