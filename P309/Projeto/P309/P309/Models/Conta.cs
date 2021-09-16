using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P309.Models
{
    public class Conta
    {
        /*
         Conta contem:
        um ID como primary key
        duas strings Nome de utilizador e palavra passe, ambos required
        Um int Role
        */
        public int ID { get; set; }
        [Required(ErrorMessage = "*Nome de utilizador necessário")]
        public string NomeDeUtilizador { get; set; }
        [Required(ErrorMessage = "*Palavra-Passe necessária")]
        public string PalavraPasse { get; set; }
        public int Role { get; set; }
    }
}
