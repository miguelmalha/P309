using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace P309_2.Models
{
    public class Contacts
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Location { get; set; }

        [Display(Name = "Zip-Code")]
        public string ZIP_Code { get; set; }

        public string Country { get; set; }

        [ForeignKey("Company")]
        [Display(Name = "Company")]
        public int Company_Id { get; set; }

        public Companies Company { get; set; }

        public string Observations { get; set; }

        [Display(Name = "Created by")]
        public string UserId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy H:mm:ss}")] 
        public DateTime Created { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy H:mm:ss}")] 
        public DateTime Updated { get; set; }
    }
}