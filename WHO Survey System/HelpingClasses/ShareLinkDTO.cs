using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHO_Survey_System.HelpingClasses
{
    public class ShareLinkDTO
    {
        public int Id { get; set; }
        public string Institute { get; set; }
        public int SurveyId { get; set; }
        public string Survey { get; set; }
        public int SendTo { get; set; }
        public int CompletedBy { get; set; }
        public string LinkSendAt { get; set; }
    }
}