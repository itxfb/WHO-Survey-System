using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHO_Survey_System.HelpingClasses
{
    public class SurveyResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Experience { get; set; }
        public string Work_Place { get; set; }
        public string Contract_Category { get; set; }
        public string SurveyTitle { get; set; }
        public string Scenario { get; set; }
        public string Category { get; set; }
        public int Point { get; set; }
        public int SurveyId { get; set; }
        public string IsActive { get; set; }
        public string CreatedAt { get; set; }
    }
}