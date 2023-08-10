using Dapper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WHO_Survey_System.BL;
using WHO_Survey_System.HelpingClasses;
using WHO_Survey_System.Models;

namespace WHO_Survey_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly SqlConnection de = new SqlConnection();
        DatabaseEntities db = new DatabaseEntities();

        public HomeController()
        {
            
            this.de = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseEntities"].ConnectionString.Replace("metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=\"", "").Replace("\"", ""));
        }

        public string GetBaseUrl()
        {
            var request = HttpContext.ApplicationInstance.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}",
                request.Url.Scheme, //request.Url.Scheme gives https or http
                request.Url.Authority, //request.Url.Authority gives qawithexperts/com
                appUrl); //appUrl = /questions/111/ok-this-is-url

            return baseUrl; //this will return complete url
        }

        [HttpPost]
        public ActionResult CheckLanguage(string lang = "")
        {
            HttpCookie myCookie = Request.Cookies["LanguageCookie"];
            if (myCookie == null)
            {
                myCookie = new HttpCookie("LanguageCookie");
                if (lang == "")
                {
                    myCookie.Values["lang"] = "English";
                }
            }

            

            myCookie.Values["lang"] = lang;


            myCookie.Expires = GeneralPurpose.DateTimeNow().AddDays(30);
            Response.Cookies.Add(myCookie);
            //ProjectVariables.lang = lang;
            return Json(myCookie.Value);
        }

        public ActionResult Index(string Email = "", string msg = "", string Lang = "", string Passcode = "")
        {
            ViewBag.Email = Email;
            //ProjectVariables.lang = Lang;
            //ViewBag.lang = Lang;
            ViewBag.msg = msg;


          

            if (Email != "")
            {
                Email = StringCipher.Base64Decode(Email);
            }
            var validateUser = new UserBL().GetUserByEmail(Email, de);
            var ValidateEmail = new SurveyAccessBL().GetSurveyAccessByEmail(StringCipher.Base64Encode(Email), de);



            //if (Email != "" && !Email.ToLower().Contains("@who.") && !Email.ToLower().Contains("@paho.") && validateUser == null)
            //{
            //    return RedirectToAction("Index", new { Lang = Lang, msg = "You are not authorized to access the survey !" });
            //}
            //if (Lang == "")
            //{
            //    return RedirectToAction("LandingPage", new { msg = msg });
            //}

            //Use Case already verified and submitted
            if (ValidateEmail != null && ValidateEmail.IsSubmit == 1 && (validateUser == null || validateUser.Role != 1))
            {
                return RedirectToAction("LandingPage", new { isSubmit = 1, msg = "You've already submitted this survey!" });
            }

            //Use Case already Verified but not submitted
            if (ValidateEmail != null && ValidateEmail.IsVerify == 1 && validateUser==null)
            {
                return RedirectToAction("DemographicData", new { Lang = Lang, Email = StringCipher.Base64Encode(Email) });
            }


            //Use case neither verified nor submitted
            if ((ValidateEmail == null || ValidateEmail.IsVerify == 0 || validateUser.Role == 1) && Passcode == "")
            {
                if (!String.IsNullOrEmpty(Email))
                {
                    string rnadomPasscode = gp.GetUniqueKeyOriginal_BIASED(5);
                    string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
                    MailSender.SendPasscode(Email, BaseUrl, rnadomPasscode, Lang);

                    SurveyAccessCredential sac = new SurveyAccessCredential();
                    sac.Email = StringCipher.Base64Encode(Email);
                    sac.Passcode = rnadomPasscode;
                    sac.IsVerify = 0;
                    sac.IsSubmit = 0;
                    //var ValidateEmail = new SurveyAccessBL().GetSurveyAccessByEmail(sac.Email,de);

                    //new SurveyAccessBL().AddSurveyAccess(sac, de);

                    if (ValidateEmail != null)
                    {
                        ValidateEmail.Passcode = sac.Passcode;
                        new SurveyAccessBL().UpdateSurveyAccess(ValidateEmail, de);
                    }
                    else
                    {
                        new SurveyAccessBL().AddSurveyAccess(sac, de);
                    }

                    return RedirectToAction("Index", new { Lang = Lang, Email = StringCipher.Base64Encode(Email), Passcode = StringCipher.Encrypt("asdasd") });
                }
            }
            return View();
        }

        public ActionResult LandingPage(string Lang = "", int isSubmit = -1, string msg = "")
        {
            //ProjectVariables.lang = Lang;
            //var getallattempted = new SurveyAccessBL().GetActiveSurveyAccessList(de).DistinctBy(a => StringCipher.Base64Decode(a.Email)).Select(a=>StringCipher.Base64Decode(a.Email)).ToList();

            HttpCookie myCookie = Request.Cookies["LanguageCookie"];

            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
            var enddate = new DateTime(2022, 12, 18);

            //var test = easternTime.ToString("dd-MM-yyyy");

            //if ((easternTime.ToString("dd-MM-yyyy") == "18-12-2022" && easternTime.Hour > 19 && easternTime.DayOfWeek == DayOfWeek.Sunday) || easternTime.Date > enddate.Date)
            //{
            //    msg = "The survey is now closed. Thank you for your participation.";

            //    //if(Lang=="Spanish")
            //    //{
            //    //    endmsg = "L'enquête est maintenant terminée. Merci de votre participation.";
            //    //}
            //    //else if(Lang=="French")
            //    //{
            //    //    endmsg = "La encuesta está cerrada. Gracias por su participación.";
            //    //}  
            //    //else if(Lang== "Portuguese")
            //    //{
            //    //    endmsg = "A sondagem está encerrada. Obrigado pela sua participação.";
            //    //}
            //    //else if (Lang == "Arabic")
            //    //{
            //    //    endmsg = "";
            //    //}
            //    //else if (Lang == "Chinese")
            //    //{
            //    //    endmsg = "";
            //    //}

            //    //return RedirectToAction("LandingPage", new { msg = endmsg });
            //}

            if (myCookie == null)
            {
                CheckLanguage();
            }

            //ViewBag.Lang = Lang;
            ViewBag.isSubmit = isSubmit;
            ViewBag.msg = msg;

            return View();

        }
        
        public ActionResult DemographicData(string Id = "", string Lang = "", string Email = "", string Passcode = "", string msg = "")
        {
            ViewBag.Lang = Lang;
            ViewBag.msg = msg;
            ViewBag.Email = Email;
            ViewBag.Passcode = Passcode;
            var CompanyId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                CompanyId = Convert.ToInt32(StringCipher.Base64Decode(Id));
            }
            ViewBag.CompanyId = Id;

            var validateUser = new UserBL().GetUserByEmail(StringCipher.Base64Decode(Email), de);
            var getpasscode = new SurveyAccessBL().GetSurveyAccessByEmail((Email), de);

            if (getpasscode == null)
            {
                return RedirectToAction("LandingPage");
            }

            if (getpasscode.IsVerify == 1 && getpasscode.IsSubmit != 1)
            {

                var getCompany = new CompanyBL().GetCompanyById(CompanyId, de);
                if (getCompany != null)
                {
                    var CompanyDemographicsDropDowns = JsonConvert.DeserializeObject<Dictionary<string, object>>(getCompany.DemoGraphicDropDowns);

                    if (CompanyDemographicsDropDowns != null)
                    {
                        ViewBag.DemoGraphicDropDowns = CompanyDemographicsDropDowns;
                    }
                    return View(getCompany);

                }

                return View();
            }

            if (StringCipher.Base64Decode(Passcode) == getpasscode.Passcode)
            {
                if (validateUser == null)
                {
                    if (getpasscode.IsVerify == 0 || getpasscode.IsVerify == null)
                    {
                        getpasscode.IsVerify = 1;
                        //new SurveyAccessBL().UpdateSurveyAccess(getpasscode, de);
                    }
                }
                //else
                //{
                //    new SurveyAccessBL().AddSurveyAccess(getpasscode, de);
                //}
                getpasscode.CompanyId= CompanyId;
                new SurveyAccessBL().UpdateSurveyAccess(getpasscode, de);

                var getCompany = new CompanyBL().GetCompanyById(CompanyId, de);
                if(getCompany != null)
                {
                    var CompanyDemographicsDropDowns = JsonConvert.DeserializeObject<Dictionary<string, object>>(getCompany.DemoGraphicDropDowns);

                    //var CompanyDemographicsDropDowns = JsonConvert.DeserializeObject(getCompany.DemoGraphicDropDowns);
                    if(CompanyDemographicsDropDowns!=null)
                    {
                        //ViewBag.DemoGraphicDropDownsCounts = CompanyDemographicsDropDowns;
                        ViewBag.DemoGraphicDropDowns = CompanyDemographicsDropDowns;
                    }
                    return View(getCompany);

                    //ViewBag.CompanyDEMOGraphicData = getCompany;
                }

            }
            else
            {
                return RedirectToAction("Index", new { Email = Email, Passcode = Passcode, Lang = Lang, msg = "Wrong Passcode please try again!" });
            }

            //return RedirectToAction("DemographicData", new { Email = Email, Passcode = Passcode, Lang = Lang });
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Survey(string Id = "", SurveyResponse surveyResponse = null, string Lang = "", string Email = "")
        {
            var testing = Request.Form;

            //Dictionary<string, string> myDictionary = new Dictionary<string, string>();
            string QuestionAndValues = "";
            List<string> DemoGraphics=new List<string>();

            for (int i = 3; i < testing.Count; i++)
            {
                string key = testing.Keys[i];
                string value = testing[i];

                QuestionAndValues = string.Join(",", key, value);

                DemoGraphics.Add(QuestionAndValues);
            }

            var x = DemoGraphics.Count();

            //ProjectVariables.lang = Lang;
            ViewBag.Lang = Lang;
            ViewBag.Email = Email;
            ViewBag.CompanyId = Id;
            var validateUser = new UserBL().GetUserByEmail(StringCipher.Base64Decode(Email), de);
            var submissionCheck = new SurveyAccessBL().GetSurveyAccessByEmail((Email), de);
            var CompanyId = 0;
            if (!string.IsNullOrEmpty(Id))
            {
                CompanyId = Convert.ToInt32(StringCipher.Base64Decode(Id));
                ViewBag.DecodedCompanyId = CompanyId;
                var GetCompany = new CompanyBL().GetCompanyById(CompanyId, de);
                if (GetCompany != null)
                {
                    ViewBag.companyLogo = GetCompany.CompanyLogo;
                    ViewBag.companyName = GetCompany.CompanyName;
                }
            }
            if (submissionCheck.IsSubmit == 1 && validateUser==null)
            {
                return RedirectToAction("LandingPage", new { Email = Email, msg = "Your survey has been submitted!" });
            }

            var list = new SurveyBL().GetAllSurveysByCompanyId(CompanyId, de).ToList();
            ViewBag.SurveyCounts = new CompanyBL().GetCompanyById(CompanyId, de).TotalCompanySurvey;
            var sResponse = list.Last();
            ViewBag.Survey = JsonConvert.DeserializeObject<Dictionary<string, object>>(sResponse.Category_Scenarios);
            ViewBag.surveyTitle = sResponse.Title;
            //surveyResponse.SurveyTitle = sResponse.Title;
            surveyResponse.SurveyId = sResponse.Id;

            //adding demographics dropdown values to survey response

            var counts = DemoGraphics.Count();
            for (int i= 0;i < counts; i++)
            {
                if (i == 0)
                {
                    surveyResponse.Gender = DemoGraphics[i];
                }
                else if(i == 1)
                {
                    surveyResponse.Age = DemoGraphics[i];
                }
                else if (i == 2)
                {
                    surveyResponse.Experience = DemoGraphics[i];
                }
                else if (i == 3)
                {
                    surveyResponse.Work_Place = DemoGraphics[i];

                }
                else if(i==4)
                {
                    surveyResponse.Contract_Category = DemoGraphics[i];

                }
                else if (i == 5)
                {
                    surveyResponse.DemoGraphicDropDown6 = DemoGraphics[i];

                }
                else if (i == 6)
                {
                    surveyResponse.DemoGraphicDropDown7 = DemoGraphics[i];

                }
            }

            var sur = JsonConvert.SerializeObject(surveyResponse);
            ViewBag.surveyResponse = sur;
            return View();
        }

        [HttpPost]
        public ActionResult PostSurvey(string surveyResponse = "", string Responses = "", string sTitle = "", string Lang = "", string Email = "")
        {

            var surveySubmit = new SurveyAccessBL().GetSurveyAccessByEmail(Email, de);
            Email = StringCipher.Base64Decode(Email);
            var validateUser = new UserBL().GetUserByEmail(Email, de);

            if (surveySubmit != null && surveySubmit.IsSubmit == 1 && validateUser == null)
            {
                return RedirectToAction("LandingPage", new { Email = Email, msg = "Your survey has been submitted!" });
            }
            if (surveySubmit != null && (validateUser == null || validateUser.Role != 1))
            {
                surveySubmit.IsSubmit = 1;
                new SurveyAccessBL().UpdateSurveyAccess(surveySubmit, de);
            }
            SurveyResponse getSurvey = JsonConvert.DeserializeObject<SurveyResponse>(surveyResponse);
            getSurvey.SurveyTitle = sTitle.Trim();

            
            getSurvey.Responses = StringCipher.Encrypt(Responses);
           

            getSurvey.Email = Email.Trim();

            getSurvey.Language = Lang.Trim();

            if(!string.IsNullOrEmpty(getSurvey.Gender))
            {
                getSurvey.Gender = getSurvey.Gender;

            }
            if (!string.IsNullOrEmpty(getSurvey.Age))
            {
                getSurvey.Age = getSurvey.Age;

            }
            if (!string.IsNullOrEmpty(getSurvey.Experience))
            {
                getSurvey.Experience = getSurvey.Experience;

            }
            if (!string.IsNullOrEmpty(getSurvey.Work_Place))
            {
                getSurvey.Work_Place = getSurvey.Work_Place;

            }
            if (!string.IsNullOrEmpty(getSurvey.Contract_Category))
            {
                getSurvey.Contract_Category = getSurvey.Contract_Category;

            }
            if (!string.IsNullOrEmpty(getSurvey.DemoGraphicDropDown6))
            {
                getSurvey.DemoGraphicDropDown6 = getSurvey.DemoGraphicDropDown6;

            }
            if (!string.IsNullOrEmpty(getSurvey.DemoGraphicDropDown7))
            {
                getSurvey.DemoGraphicDropDown7 = getSurvey.DemoGraphicDropDown7;

            }




            new SurveyResponseBL().AddSurveyResponse(getSurvey, de);
            return RedirectToAction("Thanks", "Home");
        }

        public ActionResult Thanks()
        {
            return View();
        }

        public ActionResult CreateCompany(string msg = "", string color = "")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;
            return View();
        }
    }
}