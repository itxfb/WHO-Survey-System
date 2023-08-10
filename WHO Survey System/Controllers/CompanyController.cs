using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHO_Survey_System.BL;
using WHO_Survey_System.HelpingClasses;
using WHO_Survey_System.Models;

namespace WHO_Survey_System.Controllers
{
    public class CompanyController : Controller
    {
        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly SqlConnection de = new SqlConnection();
        DatabaseEntities db = new DatabaseEntities();

        public CompanyController()
        {
            this.de = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseEntities"].ConnectionString.Replace("metadata=res://*/Models.Model.csdl|res://*/Models.Model.ssdl|res://*/Models.Model.msl;provider=System.Data.SqlClient;provider connection string=\"", "").Replace("\"", ""));
        }

        public bool ValidateAdminLogin()
        {
            if (gp.ValidateLoggedinUser() != null)
            {
                if (gp.ValidateLoggedinUser().Role == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // GET: Company
        public ActionResult Index(string msg = "")
        {
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.msg = msg;
            }
            var getUrl = Request.Url.ToString();
            if(getUrl.Contains("?"))
            {
                getUrl=getUrl.Split('?').First();
            }
            var getCompanyName = getUrl.Split('/').Last();
            if (getCompanyName.Contains("?"))
            {
                getCompanyName = getCompanyName.Split('?').First();
            }
            if (string.IsNullOrEmpty(getCompanyName) || getCompanyName == "Index")
            {
                getCompanyName = getUrl.Split('/').Reverse().Take(2).Last();
            }

            Company company = new Company();
            if (!string.IsNullOrEmpty(getCompanyName))
            {
                //getUrl = "https://dsicsurvey.com/survey/psu";
                getCompanyName=getCompanyName.ToLower();
                //var getCompany = new CompanyBL().GetActiveCompanyList(de).Where(x => x.CompanyUrl.ToLower().Contains(getCompanyName)).FirstOrDefault();
                var getCompany = new CompanyBL().GetActiveCompanyList(de).Where(x => x.CompanyUrl.ToLower() == getUrl.ToLower()).FirstOrDefault();
                if(getCompany != null)
                {
                    company = getCompany;
                }
                ViewBag.CompanyId = StringCipher.Base64Encode(Convert.ToString(company.Id));

                // for create a log of  company  url

                var dt = DateTime.Now.ToString("g");

                string fileName = "logs.txt";
                string path = Path.Combine(Server.MapPath("~/Content/assets"), fileName);
                if (!System.IO.File.Exists(path))
                {
                    using (StreamWriter writer = System.IO.File.CreateText(path))
                    {
                        writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Id: " + company.Id + " ------------------------ Company Name: " + company.CompanyName + " ------------------------ Company Url: " + company.CompanyUrl);

                        writer.Close();

                    }

                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        //writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Id: " + company.Id + " ------------------------ Company Name: " + company.CompanyName + " ------------------------ Company Url: " + company.CompanyUrl + " ------------------------ Company Landing Page Data: " + company.Company_LandingPageData + " ------------------------ Company Demo Graphic Data: " + company.DemoGraphicData);
                        writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Id: " + company.Id + " ------------------------ Company Name: " + company.CompanyName + " ------------------------ Company Url: " + company.CompanyUrl);

                        writer.Close();
                    }
                }

                // end log create



                return View(company);
            }
            else
            {
                return RedirectToAction("Index", "Error", new { msg = "Something is Wrong", color = "red" });
            }
        }

        public ActionResult Access(string Id, string Email = "", string msg = "", string Lang = "", string Passcode = "")
        {
            ViewBag.Email = Email;
            ViewBag.msg = msg;
            ViewBag.CompanyId = Id;
            int getCompanySurvey = new SurveyBL().GetAllSurveysByCompanyId(Convert.ToInt32(StringCipher.Base64Decode(Id)), de).Where(x => x.IsActive == 1).Count();
            if (getCompanySurvey > 0)
            {


                if (Email != "")
                {
                    Email = StringCipher.Base64Decode(Email);
                }
                var validateUser = new UserBL().GetUserByEmail(Email, de);
                var ValidateEmail = new SurveyAccessBL().GetSurveyAccessByEmail(StringCipher.Base64Encode(Email), de);


                //Use Case already verified and submitted
                if (ValidateEmail != null && ValidateEmail.IsSubmit == 1 && (validateUser == null || validateUser.Role != 1))
                {
                    return RedirectToAction("Index", new { isSubmit = 1, msg = "You've already submitted this survey!" });
                }

                //Use Case already Verified but not submitted
                if (ValidateEmail != null && ValidateEmail.IsVerify == 1 && validateUser == null)
                {
                    return RedirectToAction("DemographicData", "Home", new {Id=Id,Lang = Lang, Email = StringCipher.Base64Encode(Email) });
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

                        if (ValidateEmail != null)
                        {
                            ValidateEmail.Passcode = sac.Passcode;
                            new SurveyAccessBL().UpdateSurveyAccess(ValidateEmail, de);
                        }
                        else
                        {
                            new SurveyAccessBL().AddSurveyAccess(sac, de);
                        }

                        return RedirectToAction("Access", new { Id = Id, Email = StringCipher.Base64Encode(Email), Passcode = StringCipher.Encrypt("asdasd") });
                    }
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", new { msg = "This Company Don't have any Survey"});
            }
        }

        public ActionResult ViewCompanies(string msg="",string color="")
        {
            ViewBag.msg = msg;
            ViewBag.color = color;
            return View();
        }

        [HttpPost]
        public ActionResult GetCompanyList(string CompanyName = "")
        {
            List<Company> ulist = new CompanyBL().GetActiveCompanyList(de).Where(x =>x.IsActive == 1).OrderByDescending(x => x.Id).ToList();

            if (CompanyName != "")
            {
                ulist = ulist.Where(x => x.CompanyName.ToLower().Contains(CompanyName.ToLower())).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            //string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            if (!string.IsNullOrWhiteSpace(sortColumnName))
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        ulist = ulist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        ulist = ulist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }
            int totalrows = ulist.Count();
            //filter
            if (searchValue != "")
            {
                ulist = ulist.Where(x => x.CompanyName.Contains(searchValue)).ToList();
            }

            int totalrowsafterfilterinig = ulist.Count();
            //pagination
            ulist = ulist.Skip(start).Take(length).ToList();
            List<CompanyDTO> udto = new List<CompanyDTO>();
            foreach (Company u in ulist)
            {
                CompanyDTO obj = new CompanyDTO()
                {
                    Id = u.Id,
                    CompanyName = u.CompanyName,
                    CompanyUrl = u.CompanyUrl == null ? "" : u.CompanyUrl,
                    TotalCompanySurvey = u.TotalCompanySurvey == null ? "" : Convert.ToString(u.TotalCompanySurvey),
                };

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCompany(int id)
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                Company u = new CompanyBL().GetCompanyById(id, de);
                u.IsActive = 0;

                var getSurveys = new SurveyBL().GetAllSurveysByCompanyId(id, de);
                var getSurveysAccess = new SurveyAccessBL().GetSurveyAccessByCompanyId(id, de);
                foreach (var item in getSurveysAccess)
                {
                    var Emails = StringCipher.Base64Decode(item.Email);
                    Emails = Emails.ToLower();
                    var getSurveyResponse = new SurveyResponseBL().GetActiveSurveyResponseList(de).Where(x => x.Email.ToLower().Contains(Emails)).FirstOrDefault();
                    if(getSurveyResponse != null)
                    {
                        getSurveyResponse.IsActive = 0;

                    }
                    if (!new SurveyResponseBL().UpdateSurveyResponse(getSurveyResponse, de))
                    {

                    }
                    item.IsActive = 0;
                    if(!new SurveyAccessBL().UpdateSurveyAccess(item, de))
                    {

                    }
                }

                foreach(var item in getSurveys)
                {
                    item.IsActive = 0;
                    if(!new SurveyBL().UpdateSurvey(item, de))
                    {

                    }
                }
                bool check = new CompanyBL().UpdateCompany(u, de);
                if (check == true)
                {
                    return RedirectToAction("ViewCompanies", new { msg = "Record Deleted Successfully", color = "Gareen" });
                }
                else
                {
                    return RedirectToAction("ViewCompanies", new { msg = "Somethings' Wrong", color = "red" });
                }
            }
            catch (Exception ex) 
            {
                //var Ex = ex.InnerException.Message;
                return RedirectToAction("Index", "Error");
            }
        }
    }
}