 using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace P309_2.Models
{
    public class Companies
    {
        [Key]
        public int Id { get; set; }

        [Range(100000000,999999999, ErrorMessage = "Must have a length of 9 chars")]
        [Required]
        public int NIF { get; set; }

        [Display(Name = "Company Name")]
        [Required]
        public string Name { get; set; }

        [ForeignKey("Payment_Method")]
        [Display(Name = "Payment Method")]
        public int Payment_Method_Id { get; set; }

        public Payment_Methods Payment_Method { get; set; }

        [Range(1, 31, ErrorMessage = "Choose a day between 1 and 31")]
        [Display(Name = "Payment Day")]
        public int Payment_Day { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy H:mm:ss}")] 
        public DateTime Created { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy H:mm:ss}")]
        public DateTime Updated { get; set; }

        [Display(Name = "Created by")]
        public string UserId { get; set; }

        public string Observations { get; set; }

        [Display(Name = "Associated Contacts")]
        public ICollection<Contacts> Contacts { get; set; }
    }
}