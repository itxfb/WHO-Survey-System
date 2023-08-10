using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHO_Survey_System.HelpingClasses
{
    public class RecipientDTO
    {
        public string Institution { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CountryOfResidence { get; set; }
        public string Position { get; set; }
        public string Unit { get; set; }
        public string YearsAtOrganization { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Race_Ethnicity { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}