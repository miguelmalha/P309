//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace P309_2.Diagram
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string ZIP_Code { get; set; }
        public string Country { get; set; }
        public int Company_Id { get; set; }
        public string Observations { get; set; }
        public string UserId { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
    
        public virtual Company Company { get; set; }
    }
}
