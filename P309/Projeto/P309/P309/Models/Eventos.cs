using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P309.Models
{
    public class Eventos
    {
        /*
          Contem um ID como primary key
          Strings para informações 
          DateTime para datas
        */

        public int ID { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Link { get; set; }
        public string Localizacao { get; set; }
        public string Notas { get; set; } 
    }
}