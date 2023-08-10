using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHO_Survey_System.HelpingClasses
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string EncId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyUrl { get; set; }
        public string TotalCompanySurvey { get; set; }
    }
}