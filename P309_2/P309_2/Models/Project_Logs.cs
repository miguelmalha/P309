using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace P309_2.Models
{
    public class Project_Logs
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Project")]
        [ForeignKey ("Project")]
        public int Project_Id { get; set; }

        public Projects Project { get; set; }

        public string Description { get; set; }

        [Display(Name = "Created by")]
        public string UserId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy H:mm:ss}")] 
        [Display(Name = "Changed at")]
        public DateTime Created { get; set; }
    }
}