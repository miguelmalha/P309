using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace P309_2.Models
{
    public class Projects
    {

        [Display(Name = "Project Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Project Number")]
        [Key]
        public int ProjectNumber { get; set; }

        [ForeignKey("Company")]
        [Display(Name = "Project Owner")]
        public int Company_Id { get; set; }

        public Companies Company { get; set; }

        [Display(Name = "Current Stage")]
        [ForeignKey("Development_Stage")]
        public int Development_Stage_Id { get; set; }

        public Development_Stages Development_Stage { get; set; }

        [Display(Name = "Project Area")]
        [ForeignKey("Development_Area")]
        public int Development_Area_Id { get; set; }

        public Development_Areas Development_Area { get; set; }

        public string Observations { get; set; }

        [Display(Name = "Associated Contacts")]
        public ICollection<Project_Logs> Logs { get; set; }
    }
}