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
    
    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            this.Project_Logs = new HashSet<Project_Logs>();
        }
    
        public int ProjectNumber { get; set; }
        public string Name { get; set; }
        public int Company_Id { get; set; }
        public int Development_Stage_Id { get; set; }
        public int Development_Area_Id { get; set; }
        public string Observations { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual Development_Areas Development_Areas { get; set; }
        public virtual Development_Stages Development_Stages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project_Logs> Project_Logs { get; set; }
    }
}
