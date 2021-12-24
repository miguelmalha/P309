using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace P309_2.Models
{
    public class Development_Areas
    {
        public int Id { get; set; }

        [Display(Name = "Project Area")]
        [Required]
        public string Development_Area { get; set; }
    }
}