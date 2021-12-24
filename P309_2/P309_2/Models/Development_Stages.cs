using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace P309_2.Models
{
    public class Development_Stages
    {
        public int Id { get; set; }

        [Display(Name = "Project Stage")]
        [Required]
        public string Development_Stage { get; set; }
    }
}