using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHO_Survey_System.HelpingClasses
{
    public class SurveyDTO
    {
        public int Id { get; set; }
        public string EncId { get; set; }
        public int CategoryCount { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }
        public string CreatedAt { get; set; }
        public string Category_Scenarios { get; set; }
        public int CompanyTotalSurveys { get; set; }
    }
}