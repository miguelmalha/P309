using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace P309_2.Models
{
    public class Payment_Methods
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Payment Method")]
        [Required]
        public string Payment_Method { get; set; }
    }
}