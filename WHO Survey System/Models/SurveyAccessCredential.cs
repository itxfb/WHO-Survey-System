//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WHO_Survey_System.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SurveyAccessCredential
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Passcode { get; set; }
        public Nullable<int> IsVerify { get; set; }
        public Nullable<int> IsSubmit { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public Nullable<System.DateTime> DeletedAt { get; set; }
        public Nullable<int> CompanyId { get; set; }
    }
}
