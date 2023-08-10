
using Newtonsoft.Json;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using System.Web.UI.WebControls;
using WHO_Survey_System.BL;
using WHO_Survey_System.HelpingClasses;
using WHO_Survey_System.Models;
using Newtonsoft.Json;
using System.Data.Entity;

namespace WHO_Survey_System.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly SqlConnection de = new SqlConnection();
        DatabaseEntities db = new DatabaseEntities();

        public AdminController()
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

        //Admin Dashboard
        public ActionResult Index(string msg = "", string color = "black")
        {
            if (ValidateAdminLogin() == false)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }

            ViewBag.UserCount = new UserBL().GetActiveUserList(de).Where(x => x.Role != 1).Count();
            //ViewBag.Recipient = new RecipientBL().GetActiveRecipientList(de).Count();
            ViewBag.SurveyCount = new SurveyBL().SurveysCount(de);
            ViewBag.SurveyResponseCount = new SurveyResponseBL().SurveyResponsesCount(de);
            ViewBag.Attempted = new SurveyAccessBL().SurveyAttemptedCount(de);
           // ViewBag.Submitted = new SurveyAccessBL().SurveySubmittedCount(de);

            //ViewBag.CategoryCount = new CategoryBL().GetActiveCategoryList(de).Count();
            //ViewBag.ScenarioCount = new ScenarioBL().GetActiveScenarioList(de).Count();
            //ViewBag.ShareLinkCount = new ShareLinkBL().GetActiveShareLinkList(de).Count();
            //ViewBag.SurveyResponseCount = new SurveyResponseBL().GetActiveSurveyResponseList(de).Where(x => x.Status == "Complete").Count() / 20;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        #region Manage User 
        public ActionResult AddUser(string msg = "", string color = "black")
        {
            if (ValidateAdminLogin() == false)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }

            //List<Institute> ilist = new InstituteBL().GetActiveInstituteList(de).ToList();

            //ViewBag.institutelist = ilist;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult PostAddUser(User _user)
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                bool checkEmail = gp.ValidateEmail(_user.Email);
                if (checkEmail == false)
                {
                    return RedirectToAction("AddUser", "Admin", new { msg = "Email already exist. Try another!", color = "red" });
                }

                User obj = new User()
                {
                    FirstName = _user.FirstName.Trim(),
                    LastName = _user.LastName.Trim(),
                    Email = _user.Email.Trim(),
                    Password = StringCipher.Encrypt(_user.Password.Trim()),
                    Role = 1,
                    IsActive = 1,
                    CreatedAt = DateTime.Now,
                    //InstituteId = _user.InstituteId
                };

                //if(_user.InstituteId != null)
                //{
                //    obj.Role = 10;
                //}
                //else
                //{
                //    obj.Role = 2;
                //}

                bool check = new UserBL().AddUser(obj, de);
                if (check == true)
                {
                    return RedirectToAction("AddUser", "Admin", new { msg = "Record Inserted Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("AddUser", "Admin", new { msg = "Somethings' Wrong", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }


        public ActionResult ViewUser(string msg = "", string color = "black")
        {
            if (ValidateAdminLogin() == false)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }

            //ViewBag.institutelist = new InstituteBL().GetActiveInstituteList(de).ToList();
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult PostUpdateUser(User _user)
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                bool checkEmail = gp.ValidateEmail(_user.Email, _user.Id);
                if (checkEmail == false)
                {
                    return RedirectToAction("ViewUser", "Admin", new { msg = "Email already exist. Try another!", color = "red" });
                }

                User u = new UserBL().GetUserById(_user.Id, de);
                u.FirstName = _user.FirstName.Trim();
                u.LastName = _user.LastName.Trim();
                u.Email = _user.Email.Trim();
                //u.InstituteId = _user.InstituteId;

                bool check = new UserBL().UpdateUser(u, de);
                if (check == true)
                {
                    return RedirectToAction("ViewUser", "Admin", new { msg = "Record Updated Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("ViewUser", "Admin", new { msg = "Somethings' Wrong", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult DeleteUser(int id)
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                User u = new UserBL().GetUserById(id, de);
                u.IsActive = 0;

                var getsurveyaccess = new SurveyAccessBL().GetSurveyAccessByEmail(StringCipher.Base64Encode(u.Email), de);
                if (getsurveyaccess != null)
                {
                    getsurveyaccess.IsActive = 0;
                    if(!new SurveyAccessBL().UpdateSurveyAccess(getsurveyaccess, de))
                    {
                        return RedirectToAction("ViewUser", "Admin", new { msg = "Something is Wrong", color = "red" });
                    }
                }

                if (new UserBL().UpdateUser(u, de))
                {
                    return RedirectToAction("ViewUser", "Admin", new { msg = "Record Deleted Successfully", color = "red" });
                }
                return RedirectToAction("ViewUser", "Admin", new { msg = "Something is Wrong", color = "red" });
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public ActionResult GetUserList(string Name = "", string Email = "")
        {
            List<User> ulist = new UserBL().GetActiveUserList(de).Where(x => x.Role == 1 && x.Id != gp.ValidateLoggedinUser().Id).OrderByDescending(x => x.Id).ToList();

            if (Name != "")
            {
                ulist = ulist.Where(x => x.FirstName.ToLower().Contains(Name.ToLower()) || x.LastName.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (Email != "")
            {
                ulist = ulist.Where(x => x.Email.ToLower().Contains(Email.ToLower())).ToList();
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
                ulist = ulist.Where(x => x.FirstName.Contains(searchValue) || x.LastName.Contains(searchValue) || x.Email.ToLower().Contains(searchValue.ToLower())).ToList();
            }

            int totalrowsafterfilterinig = ulist.Count();
            //pagination
            ulist = ulist.Skip(start).Take(length).ToList();
            List<UserDTO> udto = new List<UserDTO>();
            foreach (User u in ulist)
            {
                UserDTO obj = new UserDTO()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    //InstituteName = "N/A"
                };

                //if(u.InstituteId != 0 && u.InstituteId != null)
                //{
                //    obj.InstituteName = u.Institute.Name;
                //    obj.InstituteId = (int)u.InstituteId;
                //}

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserById(int id)
        {
            User u = new UserBL().GetUserById(id, de);

            UserDTO obj = new UserDTO()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Manage Survey
        public ActionResult AddSurvey(string msg = "", string color = "black", string companyId = "")
        {
            if (ValidateAdminLogin() == false)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }
            string companyIds = ""; 
            int SurveysOfCompany = 0;
            if (!string.IsNullOrEmpty(companyId))
            {
                companyIds = companyId;
                Company getCompany = new CompanyBL().GetCompanyById(Convert.ToInt32(companyIds), de);
                SurveysOfCompany = (int)getCompany.TotalCompanySurvey;
            }
            ViewBag.companyId = companyIds;
            ViewBag.companyIdSurvey = SurveysOfCompany;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult PostAddSurvey(string Title = "", string Description = "",
            string Cat_n_Scn = "", string companyId = "")
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                //var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(Cat_n_Scn);

                CreateSurvey obj = new CreateSurvey()
                {
                    Title = Title.Trim(),
                    Description = Description,
                    Category_Scenarios = Cat_n_Scn,
                    IsActive = 1,
                    CreatedAt = DateTime.UtcNow,
                    //CompanyId = StringCipher.Base64Decode(companyId)
                    CompanyId = Convert.ToInt32(companyId)
                };
                bool check = true;


                if (obj.Title.Contains("'") || obj.Description.Contains("'")||obj.Category_Scenarios.Contains("'"))
                {
                   var add= db.CreateSurveys.Add(obj);

                    int SaveChanges = db.SaveChanges();
                    if(SaveChanges > 0)
                    {
                        check = true;
                    }
                    else
                    {
                        check=false;
                    }

                }

                else
                {
                   check = new SurveyBL().AddSurvey(obj, de);

                }


                if (check == true)
                {
                    // for create a log of create company 
                    var dt = DateTime.Now.ToString("g");

                    string fileName = "logs.txt";
                    string path = Path.Combine(Server.MapPath("~/Content/assets"), fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        using (StreamWriter writer = System.IO.File.CreateText(path))
                        {
                            writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Id: " + companyId + " ------------------------ Survey Title: " + Title);

                            writer.Close();

                        }

                    }
                    else
                    {
                        using (StreamWriter writer = new StreamWriter(path, true))
                        {
                            writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Id: " + companyId + " ------------------------ Survey Title: " + Title);

                            writer.Close();
                        }
                    }

                    // end log create

                    return RedirectToAction("AddSurvey", "Admin", new { msg = "Record Inserted Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("AddSurvey", "Admin", new { msg = "Somethings' Wrong", color = "red" });
                }
                //return RedirectToAction("AddSurvey", "Admin", new { msg = "Record Inserted Successfully", color = "green" });
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ViewSurvey(string msg = "", string color = "black")
        {
            if (ValidateAdminLogin() == false)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult GetSurveyList(string Title = "", string Description = "")
        {
            List<CreateSurvey> ulist = new SurveyBL().GetActiveSurveyList(de).OrderByDescending(x => x.Id).ToList();
            if (Title != "")
            {
                ulist = ulist.Where(x => x.Title.ToLower().Contains(Title.ToLower())).ToList();
            }
            if (Description != "")
            {
                ulist = ulist.Where(x => x.Description.ToLower().Contains(Description.ToLower())).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            if (sortColumnName != "" && sortColumnName != null)
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
                ulist = ulist.Where(x => x.Title.ToLower().Contains(searchValue.ToLower()) || x.Description.ToLower().Contains(searchValue.ToLower())).ToList();
            }

            int totalrowsafterfilterinig = ulist.Count();
            // pagination
            ulist = ulist.Skip(start).Take(length).ToList();
            List<SurveyDTO> udto = new List<SurveyDTO>();
            foreach (CreateSurvey u in ulist)
            {
                SurveyDTO obj = new SurveyDTO()
                {
                    Id = u.Id,
                    EncId = StringCipher.Base64Encode(Convert.ToString(u.Id)),
                    Title = u.Title,
                    Description = u.Description,
                };
                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSurveyById(int id)
        {
            CreateSurvey u = new SurveyBL().GetSurveyById(id, de);
            //var getCatAndScenarios = JsonConvert.DeserializeObject<Dictionary<string, object>>(u.Category_Scenarios);
            var getCompanyTotalSurveys = new CompanyBL().GetCompanyById((int)u.CompanyId, de).TotalCompanySurvey;
            SurveyDTO obj = new SurveyDTO()
            {
                Id = u.Id,
                Title = u.Title,
                Description = u.Description,
                Category_Scenarios = u.Category_Scenarios,
                CompanyTotalSurveys = (int)getCompanyTotalSurveys,
            };

            //var getval = obj.keyValuePairs["1"];

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSurvey(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var getId = Convert.ToInt32(StringCipher.Base64Decode(Id));
                var getSurvey = new SurveyBL().GetSurveyById(getId, de);
                var getCompany = new CompanyBL().GetCompanyById((int)getSurvey.CompanyId, de);
                ViewBag.Surveys = getCompany.TotalCompanySurvey;
                ViewBag.getSurvey = getSurvey;
                return View();
            }
            return RedirectToAction("ViewSurvey", new { msg = "Something is Wrong, Try Again" });
        }

        [HttpPost]
        public ActionResult PostUpdateSurvey(int Id = -1, string Title = "", string Description = "", string Cat_n_Scn = "")
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                //bool checkTitle = gp.ValidateSurvey(Title, Id);
                //if (checkTitle == false)
                //{
                //    return RedirectToAction("ViewSurvey", "Admin", new { msg = "Survey Title already exist. Try another!", color = "red" });
                //}

                CreateSurvey u = new SurveyBL().GetSurveyById(Id, de);
                u.Title = Title.Trim();
                u.Description = Description.Trim();
                u.Category_Scenarios = Cat_n_Scn;

                bool check = true;


                if (u.Title.Contains("'") || u.Description.Contains("'") || u.Category_Scenarios.Contains("'"))
                {
                    db.Entry(u).State = EntityState.Modified;

                    int SaveChanges = db.SaveChanges();
                    if (SaveChanges > 0)
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }

                }

                else
                {
                 check = new SurveyBL().UpdateSurvey(u, de);


                }



                if (check == true)
                {
                    return RedirectToAction("ViewSurvey", "Admin", new { msg = "Record Updated Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("ViewSurvey", "Admin", new { msg = "Somethings' Wrong", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult DeleteSurvey(int id)
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                CreateSurvey u = new SurveyBL().GetSurveyById(id, de);

                if (u != null)
                {
                    u.IsActive = 0;

                }

                //

                bool check = true;

                if (u.Title.Contains("'") || u.Description.Contains("'") || u.Category_Scenarios.Contains("'"))
                {
                    db.Entry(u).State = EntityState.Modified;

                    int SaveChanges = db.SaveChanges();
                    if (SaveChanges > 0)
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }

                }

                else
                {
                    check = new SurveyBL().UpdateSurvey(u, de);

                }

                //


                if (check == true)
                {
                    return RedirectToAction("ViewSurvey", "Admin", new { msg = "Record Deleted Successfully", color = "red" });
                }
                else
                {
                    return RedirectToAction("ViewSurvey", "Admin", new { msg = "Somethings' Wrong", color = "red" });
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
        #endregion

        #region Manage Survey Response
        public ActionResult ViewSurveyResponsePage(string msg = "", string color = "black")
        {
            if (ValidateAdminLogin() == false)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }
            List<CreateSurvey> survey = new SurveyBL().GetActiveSurveyList(de).ToList();
            List<CreateSurvey> srvy = new List<CreateSurvey>();
            foreach (CreateSurvey surveyItem in survey)
            {
                int count = new SurveyResponseBL().GetActiveSurveyResponseList(de).Where(x => x.SurveyId == surveyItem.Id).Count();
                if (count > 0)
                {
                    srvy.Add(surveyItem);
                }
            }
            var companies = new CompanyBL().GetActiveCompanyList(de);

            //List<string> SurveyTitleList = new List<string>();
            //foreach (var item in srvy)
            //{
            //    var getCompany = new CompanyBL().GetCompanyById(Convert.ToInt32(item.CompanyId),de);
            //    var surveyTitle  = item.Title + "(" + getCompany.CompanyName+ ")";

            //}
            ViewBag.surveylist = srvy;
            ViewBag.companyList = companies;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult GetSurveyResponseList(string surveyTitle = "")
        {
            List<SurveyResponse> ulist = new SurveyResponseBL().GetActiveSurveyResponseList(de).OrderByDescending(x => x.Id).ToList();


            if (!string.IsNullOrEmpty(surveyTitle))
            {
                ulist = ulist.Where(x => x.SurveyTitle.ToLower().Contains(surveyTitle.ToLower())).ToList();
            }
            //if (!string.IsNullOrEmpty(institution))
            //{
            //    ulist = ulist.Where(x => x.Institution == institution).ToList();
            //}

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            //if (sortColumnName == "0")
            //{
            //    sortColumnName = "id";
            //    sortDirection = "desc";
            //}

            //List<SurveyResponse> ulist = new SurveyResponseBL().GetActiveSurveyResponseList(de, start, length, sortColumnName, sortDirection);

            if (sortColumnName != "" && sortColumnName != null)
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
                ulist = ulist.Where(x => x.SurveyTitle.ToLower().Contains(searchValue.ToLower())).ToList();
            }

            int totalrowsafterfilterinig = ulist.Count();
            //pagination
            ulist = ulist.Skip(start).Take(length).ToList();



            List<SurveyResponse> udto = new List<SurveyResponse>();
            foreach (SurveyResponse u in ulist)
            {
                SurveyResponse obj = new SurveyResponse()
                {
                    Id = u.Id,
                    SurveyTitle = u.SurveyTitle,
                    Age = u.Age,
                    Gender = u.Gender,
                    Experience = u.Experience,
                    Work_Place = u.Work_Place,
                    Contract_Category = u.Contract_Category,
                    DemoGraphicDropDown6=u.DemoGraphicDropDown6,
                    DemoGraphicDropDown7=u.DemoGraphicDropDown7
                };

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewSurveyResponse(string msg = "", string color = "black", int surveyResId = -1)
        {
            if (ValidateAdminLogin() == false)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
            }

            var surRes = new SurveyResponseBL().GetSurveyResponseById(surveyResId, de);
            var sur = new SurveyBL().GetActiveSurveyList(de).Where(x => x.Id == surRes.SurveyId).FirstOrDefault();
            ViewBag.Survey = JsonConvert.DeserializeObject<Dictionary<string, object>>(sur.Category_Scenarios);
            var array = JsonConvert.DeserializeObject<Dictionary<string, object>>(StringCipher.Decrypt(surRes.Responses)).Values.ToArray();
            ViewBag.SurveyCounts = new CompanyBL().GetCompanyById((int)sur.CompanyId, de).TotalCompanySurvey;
            ViewBag.surveyRes = array;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        public ActionResult DeleteSurveyResponse(int? id)
        {
            try
            {
                if (ValidateAdminLogin() == false)
                {
                    return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
                }

                SurveyResponse u = new SurveyResponseBL().GetSurveyResponseById(id.Value, de);

                if(u != null)
                {
                    //var surveyAccess = new SurveyAccessBL().GetSurveyAccessByEmail(StringCipher.Base64Encode(u.Email), de);
                    var EncodedEmail=StringCipher.Base64Encode(u.Email);
                    var surveyAccess = db.SurveyAccessCredentials.Where(x => x.Email==EncodedEmail).FirstOrDefault();
                    if (surveyAccess != null)
                    {
                        surveyAccess.IsActive = 0;

                    }

                    u.IsActive = 0;

                    bool checkk = new SurveyAccessBL().UpdateSurveyAccess(surveyAccess, de);
                    bool check = new SurveyResponseBL().UpdateSurveyResponse(u, de);
                    if (check == true && checkk == true)
                    {
                        return RedirectToAction("ViewSurveyResponsePage", "Admin", new { msg = "Record Deleted Successfully", color = "red" });
                    }
                    else
                    {
                        return RedirectToAction("ViewSurveyResponsePage", "Admin", new { msg = "Somethings' Wrong", color = "red" });
                    }
                }

                return RedirectToAction("ViewSurveyResponsePage", "Admin", new { msg = "Somethings' Wrong", color = "red" });


            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        //IronXL
        //public async Task<FileResult> ExportToExcel()
        //{

        //    var ulist = new SurveyResponseBL().GetActiveSurveyResponseList(de).ToList();


        //    IronXL.License.LicenseKey = "IRONXL.ASMDASDFAIZANFAYYAZ.5818-DE94945907-NNYH3AHT4EQL6-SOD2MAFCSO27-LGPOITQGOKCX-ORYT4LC4GM6T-637VITJ774TJ-AFHUHU-TCFD7NEP4CKIUA-DEPLOYMENT.TRIAL-6OX4QD.TRIAL.EXPIRES.28.DEC.2022";

        //    //if (ulist.Count == 0)
        //    //{
        //    //    return RedirectToAction("ViewSurveyResponsePage", "Admin", new { msg = "No Record Found", color = "red" });
        //    //}

        //    WorkBook workbook = WorkBook.Create(ExcelFileFormat.XLSX);

        //    var sheet = workbook.CreateWorkSheet("responses_sheet");


        //    int row = 1, headingcount = 1, temprow = row + 1, hcount = 1;





        //    var cdto = new List<SurveyResponseDTO>();
        //    foreach (var u in ulist)
        //    {

        //        var responsePoints = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(u.Responses);

        //        int alphabetcount = 65, column = 79, hcount3 = 1; ; //A1

        //        string alphacolumn = "";


        //        //speicified row
        //        foreach (var r in responsePoints)
        //        {

        //            var key = r.Key.TrimEnd(':').Trim();

        //            key = r.Key.Substring(2).Trim();

        //            if (column >= 88)
        //            {
        //                alphacolumn = Convert.ToChar(alphabetcount++).ToString();

        //                column = 65;
        //            }

        //            //assigning key value // from to ranges//
        //            var r1 = Convert.ToChar(column).ToString(); //column range from
        //            var r2 = Convert.ToChar(column + 3).ToString(); //column range to








        //            if (headingcount == 1)
        //            {



        //               // Merge Columns

        //                sheet.Merge(alphacolumn + r1 + row + ":" + alphacolumn + r2 + row);

        //                //Assigning Header
        //                sheet[alphacolumn + r1 + row + ":" + alphacolumn + r2 + row].Value = r.Key;
        //            }



        //            //ilterating values from responses

        //            int tempcol = column, cnt = 1;

        //            foreach (var x in r.Value)
        //            {
        //                if (Convert.ToInt32(r.Key.Trim('.')[0]) < 6)
        //                {
        //                    if (headingcount == 1)
        //                    {
        //                        sheet[Convert.ToChar(tempcol).ToString() + (row + 1)].Value = "Q" + cnt;
        //                    }
        //                    sheet[Convert.ToChar(tempcol).ToString() + (row + 3)].Value = x;
        //                }
        //                else
        //                {
        //                    if (headingcount == 1)
        //                    {
        //                        sheet[alphacolumn + Convert.ToChar(tempcol).ToString() + (row + 1)].Value = "Q" + cnt;
        //                    }
        //                    sheet[alphacolumn + Convert.ToChar(tempcol).ToString() + (row + 3)].Value = x;
        //                }

        //                cnt++;
        //                tempcol++;

        //            }




        //            //Email//Gender//Experience//Age//WorkPlace//Contract Category

        //            if (hcount == 1)
        //            {


        //                sheet.Merge("A" + temprow + ":" + "B" + temprow);
        //                sheet.Merge("C" + temprow + ":" + "D" + temprow);
        //                sheet.Merge("E" + temprow + ":" + "F" + temprow);
        //                sheet.Merge("G" + temprow + ":" + "H" + temprow);
        //                sheet.Merge("I" + temprow + ":" + "J" + temprow);
        //                sheet.Merge("K" + temprow + ":" + "L" + temprow);

        //                sheet["A" + temprow + ":" + "B" + temprow].Value = "Id";
        //                sheet["C" + temprow + ":" + "D" + temprow].Value = "Gender";
        //                sheet["E" + temprow + ":" + "F" + temprow].Value = "Experience";
        //                sheet["G" + temprow + ":" + "H" + temprow].Value = "Age";
        //                sheet["I" + temprow + ":" + "J" + temprow].Value = "Work Place";
        //                sheet["K" + temprow + ":" + "L" + temprow].Value = "Contract Category";

        //                temprow += 1;



        //            }
        //            if (hcount3 == 1)
        //            {
        //                sheet.Merge("A" + (temprow + 1) + ":" + "B" + (temprow + 1));
        //                sheet.Merge("C" + (temprow + 1) + ":" + "D" + (temprow + 1));
        //                sheet.Merge("E" + (temprow + 1) + ":" + "F" + (temprow + 1));
        //                sheet.Merge("G" + (temprow + 1) + ":" + "H" + (temprow + 1));
        //                sheet.Merge("I" + (temprow + 1) + ":" + "J" + (temprow + 1));
        //                sheet.Merge("K" + (temprow + 1) + ":" + "L" + (temprow + 1));


        //                sheet["A" + (temprow + 1) + ":" + "B" + (temprow + 1)].Value = u.Id;
        //                sheet["C" + (temprow + 1) + ":" + "D" + (temprow + 1)].Value = u.Gender;
        //                sheet["E" + (temprow + 1) + ":" + "F" + (temprow + 1)].Value = u.Experience;
        //                sheet["G" + (temprow + 1) + ":" + "H" + (temprow + 1)].Value = u.Age;
        //                sheet["I" + (temprow + 1) + ":" + "J" + (temprow + 1)].Value = u.Work_Place;
        //                sheet["K" + (temprow + 1) + ":" + "L" + (temprow + 1)].Value = u.Contract_Category;

        //                temprow += 2;

        //                hcount3++;
        //            }



        //            column += 6;


        //            hcount++;



        //        }

        //        row += 2;
        //        headingcount++;






        //    }


        //    //Styling and aligning

        //    sheet.Style.Font.Bold = true;

        //    sheet.Style.ShrinkToFit = true;

        //    sheet.Style.HorizontalAlignment = IronXL.Styles.HorizontalAlignment.Center;

        //    //Read the File data into Byte Array.
        //    byte[] bytes = workbook.ToByteArray();
        ////Send the File to Download.
        //     return File(bytes, "application/octet-stream", "Survey_Responses.xlsx");

        //}


        public async Task<FileResult> ExportToExcel(int sId = -1)
        {
            //SpreadsheetLight works on the idea of a currently selected worksheet.
            // So there's always a worksheet selected, just like you always only
            // have one worksheet active when in Excel.
            SLDocument sl = new SLDocument();


            SLStyle style1 = sl.CreateStyle();
            style1.Font.Bold = true;
            style1.Alignment.ShrinkToFit = true;
            style1.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);

            var ulist = new SurveyResponseBL().GetActiveSurveyResponseList(de).Where(x => x.SurveyId == sId).ToList();
            int row = 1, headingcount = 1, temprow = row + 1, hcount = 1;
            var cdto = new List<SurveyResponseDTO>();

            foreach (var u in ulist)
            {
                var responsePoints = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(StringCipher.Decrypt(u.Responses));
                int alphabetcount = 65, column = 82, hcount3 = 1; ; //A1
                string alphacolumn = "";

                //speicified row
                foreach (var r in responsePoints)
                {
                    var key = r.Key.TrimEnd(':').Trim();
                    key = r.Key.Substring(2).Trim();

                    if (column >= 88)
                    {
                        alphacolumn = Convert.ToChar(alphabetcount++).ToString();
                        column = 65;
                    }

                    // assigning key value // from to ranges//
                    var r1 = Convert.ToChar(column).ToString(); //column range from
                    var r2 = Convert.ToChar(column + 3).ToString(); //column range to

                    if (headingcount == 1)
                    {
                        //Merge Columns
                        sl.MergeWorksheetCells(alphacolumn + r1 + row, alphacolumn + r2 + row);

                        //  Assigning Header
                        sl.SetCellValue(alphacolumn + r1 + row, r.Key);
                        sl.SetCellStyle(alphacolumn + r1 + row, style1);
                        //  sheet[alphacolumn + r1 + row + ":" + alphacolumn + r2 + row].Value = r.Key;
                    }

                    //  ilterating values from responses

                    int tempcol = column, cnt = 1;
                    foreach (var x in r.Value)
                    {
                        if (Convert.ToInt32(r.Key.Trim('.')[0]) < 6)
                        {
                            if (headingcount == 1)
                            {
                                sl.SetCellValue(Convert.ToChar(tempcol).ToString() + (row + 1), "Q" + cnt);
                                sl.SetCellStyle(Convert.ToChar(tempcol).ToString() + (row + 1), style1);
                                // sheet[Convert.ToChar(tempcol).ToString() + (row + 1)].Value = "Q" + cnt;
                            }

                            sl.SetCellValue(Convert.ToChar(tempcol).ToString() + (row + 3), x);
                            sl.SetCellStyle(Convert.ToChar(tempcol).ToString() + (row + 3), style1);
                            // sheet[Convert.ToChar(tempcol).ToString() + (row + 3)].Value = x;
                        }
                        else
                        {
                            if (headingcount == 1)
                            {
                                sl.SetCellValue(alphacolumn + Convert.ToChar(tempcol).ToString() + (row + 1), "Q" + cnt);
                                sl.SetCellStyle(alphacolumn + Convert.ToChar(tempcol).ToString() + (row + 1), style1);

                            }
                            sl.SetCellValue(alphacolumn + Convert.ToChar(tempcol).ToString() + (row + 3), x);
                            sl.SetCellStyle(alphacolumn + Convert.ToChar(tempcol).ToString() + (row + 3), style1);

                        }

                        cnt++;
                        tempcol++;

                    }




                    // Email//Gender//Experience//Age//WorkPlace//Contract Category

                    // customize headings and values

                    string[] DemoGraphicData1 = new string[2];
                    string[] DemoGraphicData2 = new string[2];
                    string[] DemoGraphicData3 = new string[2];
                    string[] DemoGraphicData4 = new string[2];
                    string[] DemoGraphicData5 = new string[2];
                    string[] DemoGraphicData6 = new string[2];
                    string[] DemoGraphicData7 = new string[2];


                    DemoGraphicData1 = u.Gender.Split(',');
                    DemoGraphicData2 = u.Age.Split(',');
                    DemoGraphicData3 = u.Experience.Split(',');


                    if (!string.IsNullOrEmpty(u.Work_Place))
                    {
                        DemoGraphicData4 = u.Work_Place.Split(',');

                    }
                    else
                    {
                        DemoGraphicData4[0] = "";
                        DemoGraphicData4[1] = "";

                    }
                    if (!string.IsNullOrEmpty(u.Contract_Category))
                    {
                        DemoGraphicData5 = u.Contract_Category.Split(',');

                    }
                    else
                    {
                        DemoGraphicData5[0] = "";
                        DemoGraphicData5[1] = "";

                    }

                    if (!string.IsNullOrEmpty(u.DemoGraphicDropDown6))
                    {
                        DemoGraphicData6 = u.DemoGraphicDropDown6.Split(',');

                    }
                    else
                    {
                        DemoGraphicData6[0] = " ";
                        DemoGraphicData6[1] = " ";

                    }

                    if (!string.IsNullOrEmpty(u.DemoGraphicDropDown7))
                    {
                        DemoGraphicData7 = u.DemoGraphicDropDown6.Split(',');

                    }
                    else
                    {
                        DemoGraphicData7[0] = " ";
                        DemoGraphicData7[1] = "";

                    }

                    if (hcount == 1)
                    {

                        sl.MergeWorksheetCells("A" + temprow, "B" + temprow);
                        sl.MergeWorksheetCells("C" + temprow, "D" + temprow);
                        sl.MergeWorksheetCells("E" + temprow, "F" + temprow);
                        sl.MergeWorksheetCells("G" + temprow, "H" + temprow);
                        sl.MergeWorksheetCells("I" + temprow, "J" + temprow);
                        sl.MergeWorksheetCells("K" + temprow, "L" + temprow);
                        sl.MergeWorksheetCells("M" + temprow, "N" + temprow);
                        sl.MergeWorksheetCells("O" + temprow, "P" + temprow);

                        //sheet.Merge("A" + temprow + ":" + "B" + temprow);
                        //sheet.Merge("C" + temprow + ":" + "D" + temprow);
                        //sheet.Merge("E" + temprow + ":" + "F" + temprow);
                        //sheet.Merge("G" + temprow + ":" + "H" + temprow);
                        //sheet.Merge("I" + temprow + ":" + "J" + temprow);
                        //sheet.Merge("K" + temprow + ":" + "L" + temprow);

                        //sl.SetCellValue("A" + temprow + ":" + "B" + temprow, "Id");  //babar commented
                        //sl.SetCellValue("C" + temprow + ":" + "D" + temprow, "Gender");
                        //sl.SetCellValue("E" + temprow + ":" + "F" + temprow, "Experience");
                        //sl.SetCellValue("G" + temprow + ":" + "H" + temprow, "Age");
                        //sl.SetCellValue("I" + temprow + ":" + "J" + temprow, "Work Place");
                        //sl.SetCellValue("K" + temprow + ":" + "L" + temprow, "Contract Category");




                        //sl.SetCellValue("A" + temprow, "Id");
                        //sl.SetCellValue("C" + temprow, "Gender");
                        //sl.SetCellValue("E" + temprow, "Experience");
                        //sl.SetCellValue("G" + temprow, "Age");
                        //sl.SetCellValue("I" + temprow, "Work Place");
                        //sl.SetCellValue("K" + temprow, "Contract Category");

                        sl.SetCellValue("A" + temprow + ":" + "B" + temprow, "Id");
                        sl.SetCellValue("C" + temprow + ":" + "D" + temprow, "DemoGraphicData1");
                        sl.SetCellValue("E" + temprow + ":" + "F" + temprow, "DemoGraphicData2");
                        sl.SetCellValue("G" + temprow + ":" + "H" + temprow, "DemoGraphicData3");
                        sl.SetCellValue("I" + temprow + ":" + "J" + temprow, "DemoGraphicData4");
                        sl.SetCellValue("K" + temprow + ":" + "L" + temprow, "DemoGraphicData5");
                        sl.SetCellValue("M" + temprow + ":" + "N" + temprow, "DemoGraphicData6");
                        sl.SetCellValue("O" + temprow + ":" + "P" + temprow, "DemoGraphicData7");


                        //headings

                        sl.SetCellValue("A" + temprow, "Id");
                        sl.SetCellValue("C" + temprow, DemoGraphicData1[0]);
                        sl.SetCellValue("E" + temprow, DemoGraphicData2[0]);
                        sl.SetCellValue("G" + temprow, DemoGraphicData3[0]);
                        sl.SetCellValue("I" + temprow, DemoGraphicData4[0]);
                        sl.SetCellValue("K" + temprow, DemoGraphicData5[0]);
                        sl.SetCellValue("M" + temprow, DemoGraphicData6[0]);
                        sl.SetCellValue("O" + temprow, DemoGraphicData7[0]);



                        sl.SetCellStyle("A" + temprow, style1);
                        sl.SetCellStyle("C" + temprow, style1);
                        sl.SetCellStyle("E" + temprow, style1);
                        sl.SetCellStyle("G" + temprow, style1);
                        sl.SetCellStyle("I" + temprow, style1);
                        sl.SetCellStyle("K" + temprow, style1);
                        sl.SetCellStyle("M" + temprow, style1);
                        sl.SetCellStyle("O" + temprow, style1);





                        //sheet["A" + temprow + ":" + "B" + temprow].Value = "Id";
                        //sheet["C" + temprow + ":" + "D" + temprow].Value = "Gender";
                        //sheet["E" + temprow + ":" + "F" + temprow].Value = "Experience";
                        //sheet["G" + temprow + ":" + "H" + temprow].Value = "Age";
                        //sheet["I" + temprow + ":" + "J" + temprow].Value = "Work Place";
                        //sheet["K" + temprow + ":" + "L" + temprow].Value = "Contract Category";

                        temprow += 1;



                    }
                    if (hcount3 == 1)
                    {
                        sl.MergeWorksheetCells("A" + (temprow + 1), "B" + (temprow + 1));
                        sl.MergeWorksheetCells("C" + (temprow + 1), "D" + (temprow + 1));
                        sl.MergeWorksheetCells("E" + (temprow + 1), "F" + (temprow + 1));
                        sl.MergeWorksheetCells("G" + (temprow + 1), "H" + (temprow + 1));
                        sl.MergeWorksheetCells("I" + (temprow + 1), "J" + (temprow + 1));
                        sl.MergeWorksheetCells("K" + (temprow + 1), "L" + (temprow + 1));
                        sl.MergeWorksheetCells("M" + (temprow + 1), "N" + (temprow + 1));
                        sl.MergeWorksheetCells("O" + (temprow + 1), "P" + (temprow + 1));





                        //sheet.Merge("A" + (temprow + 1) + ":" + "B" + (temprow + 1));
                        //sheet.Merge("C" + (temprow + 1) + ":" + "D" + (temprow + 1));
                        //sheet.Merge("E" + (temprow + 1) + ":" + "F" + (temprow + 1));
                        //sheet.Merge("G" + (temprow + 1) + ":" + "H" + (temprow + 1));
                        //sheet.Merge("I" + (temprow + 1) + ":" + "J" + (temprow + 1));
                        //sheet.Merge("K" + (temprow + 1) + ":" + "L" + (temprow + 1));

                        //sl.SetCellValue("A" + (temprow + 1), u.Id);
                        //sl.SetCellValue("C" + (temprow + 1), u.Gender);
                        //sl.SetCellValue("E" + (temprow + 1), u.Age);
                        //sl.SetCellValue("G" + (temprow + 1), u.Experience);
                        //sl.SetCellValue("I" + (temprow + 1), u.Work_Place);
                        //sl.SetCellValue("K" + (temprow + 1), u.Contract_Category);



                        //headings values

                        sl.SetCellValue("A" + (temprow + 1), u.Id);
                        sl.SetCellValue("C" + (temprow + 1), DemoGraphicData1[1]);
                        sl.SetCellValue("E" + (temprow + 1), DemoGraphicData2[1]);
                        sl.SetCellValue("G" + (temprow + 1), DemoGraphicData3[1]);
                        sl.SetCellValue("I" + (temprow + 1), DemoGraphicData4[1]);
                        sl.SetCellValue("K" + (temprow + 1), DemoGraphicData5[1]);
                        sl.SetCellValue("M" + (temprow + 1), DemoGraphicData6[1]);
                        sl.SetCellValue("O" + (temprow + 1), DemoGraphicData7[1]);




                        sl.SetCellStyle("A" + (temprow + 1), style1);
                        sl.SetCellStyle("C" + (temprow + 1), style1);
                        sl.SetCellStyle("E" + (temprow + 1), style1);
                        sl.SetCellStyle("G" + (temprow + 1), style1);
                        sl.SetCellStyle("I" + (temprow + 1), style1);
                        sl.SetCellStyle("K" + (temprow + 1), style1);
                        sl.SetCellStyle("M" + (temprow + 1), style1);
                        sl.SetCellStyle("O" + (temprow + 1), style1);



                        //sheet["A" + (temprow + 1) + ":" + "B" + (temprow + 1)].Value = u.Id;
                        //sheet["C" + (temprow + 1) + ":" + "D" + (temprow + 1)].Value = u.Gender;
                        //sheet["E" + (temprow + 1) + ":" + "F" + (temprow + 1)].Value = u.Experience;
                        //sheet["G" + (temprow + 1) + ":" + "H" + (temprow + 1)].Value = u.Age;
                        //sheet["I" + (temprow + 1) + ":" + "J" + (temprow + 1)].Value = u.Work_Place;
                        //sheet["K" + (temprow + 1) + ":" + "L" + (temprow + 1)].Value = u.Contract_Category;

                        temprow += 2;

                        hcount3++;
                    }



                    column += 7;


                    hcount++;



                }

                row += 2;
                headingcount++;
            }

            //filestream
            var fs = new MemoryStream();

            sl.SaveAs(fs);
            fs.Position = 0;

            //sl.SaveAs(Server.MapPath("~/Content/Excels/SurveyResponses.xlsx"));
            return File(fs.ToArray(), "application/octet-stream", "SurveyResponses.xlsx");
        }

        //public ActionResult ExportToCSV(int sId = -1)
        //{
        //    var survey = new SurveyBL().GetSurveyById(sId, de);
        //    List<SurveyResponse> ulist = new SurveyResponseBL().GetActiveSurveyResponseList(de).Where(x => x.SurveyTitle == survey.Title).ToList();
        //    var cat_n_scn = JsonConvert.DeserializeObject<Dictionary<string, object>>(survey.Category_Scenarios).Values.ToArray();


        //    if (ulist.Count == 0)
        //    {
        //        return RedirectToAction("ViewSurveyResponsePage", "Admin", new { msg = "No Record Found", color = "red" });
        //    }

        //    var cdto = new List<SurveyResponseDTO>();
        //    foreach (SurveyResponse u in ulist)
        //    {
        //        var responsePoints = JsonConvert.DeserializeObject<Dictionary<string, object>>(u.Responses).Values.ToArray();



        //        SurveyResponseDTO obj = new SurveyResponseDTO()
        //        {
        //            Email = u.Email,
        //            Gender = u.Gender,
        //            Age = u.Age,
        //            Experience = u.Experience,
        //            Work_Place = u.Work_Place,
        //            Contract_Category = u.Contract_Category,
        //            SurveyTitle = u.SurveyTitle
        //        };
        //        cdto.Add(obj);
        //    }

        //    StringWriter strw = new StringWriter();
        //    strw.WriteLine("\"Email\",\"Gender\",\"Age\",\"Experience\",\"Work Place\",\"Contractual Status\",\"Survey Title\"");
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition",
        //                        string.Format("attachment;filename=SurveyResponse_File.csv", DateTime.Now));
        //    Response.ContentType = "text/csv";

        //    if (cdto.Count != 0)
        //    {

        //        foreach (var c in cdto)
        //        {
        //            strw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
        //                                        c.Email, c.Gender, c.Age, c.Experience, c.Work_Place, c.Contract_Category, c.SurveyTitle));
        //        }
        //    }

        //    Response.Write(strw.ToString());
        //    if (Response.IsClientConnected)
        //    {
        //        Response.End();
        //    }

        //    return RedirectToAction("ViewInstituteUserSurveyResponse", "Admin", new { msg = "File Exported Successfully", color = "green" });
        //}

        #endregion

        #region Manage Recipient 
        //public ActionResult AddRecipient(string msg = "", string color = "black")
        //{
        //    if (ValidateAdminLogin() == false)
        //    {
        //        return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //    }

        //    ViewBag.Message = msg;
        //    ViewBag.Color = color;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult PostAddRecipient(Recipient _recipient)
        //{
        //    try
        //    {
        //        if (ValidateAdminLogin() == false)
        //        {
        //            return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //        }

        //        bool checkEmail = gp.ValidateEmail(_recipient.Email);
        //        if (checkEmail == false)
        //        {
        //            return RedirectToAction("AddUser", "Admin", new { msg = "Email already exist. Try another!", color = "red" });
        //        }

        //        Recipient obj = new Recipient()
        //        {
        //            Institution = _recipient.Institution,
        //            FirstName = _recipient.FirstName.Trim(),
        //            LastName = _recipient.LastName.Trim(),
        //            Email = _recipient.Email.Trim(),
        //            CountryOfResidence = _recipient.CountryOfResidence,
        //            Position = _recipient.Position,
        //            Unit = _recipient.Unit,
        //            YearsAtOrganization = _recipient.YearsAtOrganization,
        //            Age = _recipient.Age,
        //            Gender = _recipient.Gender,
        //            Race_Ethnicity = _recipient.Race_Ethnicity,
        //            IsActive = 1,
        //            CreatedAt = DateTime.Now,
        //        };

        //        if (_user.InstituteId != null)
        //        {
        //            obj.Role = 10;
        //        }
        //        else
        //        {
        //            obj.Role = 2;
        //        }

        //        bool check = new RecipientBL().AddRecipient(obj, de);
        //        if (check == true)
        //        {
        //            return RedirectToAction("AddRecipient", "Admin", new { msg = "Record Inserted Successfully", color = "green" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("AddRecipient", "Admin", new { msg = "Somethings' Wrong", color = "red" });
        //        }
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index", "Error");
        //    }
        //}


        //[HttpPost]
        //public ActionResult PostImportCsv(HttpPostedFileBase PostedFile)
        //{
        //    if (ValidateAdminLogin() == false)
        //    {
        //        return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //    }
        //    try
        //    {

        //        string fileName = Path.GetFileName(PostedFile.FileName);
        //        string fileExt = Path.GetExtension(PostedFile.FileName);

        //        if (fileExt.ToLower() != ".csv")
        //        {
        //            if (fileExt.ToLower() == ".xlsx" || fileExt.ToLower() == ".xls")
        //            {
        //                ActionResult action = ImportExcelfile(PostedFile);
        //                return action;
        //            }
        //            return RedirectToAction("ImportCsv", "Admin", new { msg = "Only .csv,.xlsx OR xls format allowed", color = "red" });
        //        }
        //        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        //        {
        //            MissingFieldFound = null,
        //            HasHeaderRecord = false,
        //            HeaderValidated = null,
        //            BadDataFound = null
        //        };
        //        List<RecipientDTO> cd_DtoList = new List<RecipientDTO>();
        //        using (var streamReader = new StreamReader(PostedFile.InputStream))
        //        {
        //            using (var csvReader = new CsvReader(streamReader, config))
        //            {
        //                cd_DtoList = csvReader.GetRecords<RecipientDTO>().ToList();
        //            }
        //        }
        //        int totalRow = cd_DtoList.Count();
        //        int flag = 0;
        //        int count = 0;
        //        foreach (RecipientDTO cddto in cd_DtoList)
        //        {
        //            int countt = de.Recipients.Where(x => x.IsActive == 1 && x.Email == cddto.Email).ToList().Count();
        //            if (countt > 0)
        //            {
        //                continue;
        //            }
        //            if (count > 0)
        //            {
        //                Recipient cd = new Recipient()
        //                {
        //                    Institution = cddto.Institution,
        //                    FirstName = cddto.FirstName.Trim(),
        //                    LastName = cddto.LastName.Trim(),
        //                    Email = cddto.Email.Trim(),
        //                    CountryOfResidence = cddto.CountryOfResidence,
        //                    Position = cddto.Position,
        //                    Unit = cddto.Unit,
        //                    YearsAtOrganization = cddto.YearsAtOrganization,
        //                    Age = cddto.Age,
        //                    Gender = cddto.Gender,
        //                    Race_Ethnicity = cddto.Race_Ethnicity,
        //                    IsActive = 1,
        //                    CreatedAt = DateTime.Now,
        //                };

        //                bool chk = false;
        //                if (cd != new Recipient())
        //                {
        //                    chk = new RecipientBL().AddRecipient(cd, de);
        //                }

        //                if (chk == false)
        //                {
        //                    flag = 1;
        //                }
        //            }
        //            count++;
        //        }
        //        if (flag == 0)
        //        {
        //            return RedirectToAction("AddRecipient", "Admin", new { msg = "File imported successfully.", color = "green" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("AddRecipient", "Admin", new { msg = "File imported with errors.", color = "orange" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("AddRecipient", "Admin", new { msg = ex.Message, color = "red" });
        //    }
        //}


        //[HttpPost]
        //public ActionResult ImportExcelfile(HttpPostedFileBase PostedFile)
        //{
        //    if (ValidateAdminLogin() == false)
        //    {
        //        return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //    }
        //    try
        //    {
        //        string fileName = Path.GetFileName(PostedFile.FileName);
        //        string fileExt = Path.GetExtension(PostedFile.FileName);
        //        if (fileExt.ToLower() != ".xlsx")
        //        {
        //            return RedirectToAction("ImportCsv", "Users", new { msg = "Only .csv,.xlsx OR xls format allowed", color = "red" });
        //        }
        //        var recipientList = new List<Recipient>();
        //        using (var stream = new MemoryStream())
        //        {
        //            PostedFile.InputStream.CopyToAsync(stream/*, cancellationToken*/);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //                var rowCount = worksheet.Dimension.Rows;
        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    recipientList.Add(new Recipient
        //                    {
        //                        Institution = worksheet.Cells[row, 1].Value.ToString().Trim(),
        //                        FirstName = worksheet.Cells[row, 2].Value.ToString().Trim(),
        //                        LastName = worksheet.Cells[row, 3].Value.ToString().Trim(),
        //                        Email = worksheet.Cells[row, 4].Value.ToString().Trim(),
        //                        CountryOfResidence = worksheet.Cells[row, 5].Value.ToString().Trim(),
        //                        Position = worksheet.Cells[row, 6].Value.ToString().Trim(),
        //                        Unit = worksheet.Cells[row, 7].Value.ToString().Trim(),
        //                        YearsAtOrganization = worksheet.Cells[row, 8].Value.ToString().Trim(),
        //                        Age = worksheet.Cells[row, 9].Value.ToString().Trim(),
        //                        Gender = worksheet.Cells[row, 10].Value.ToString().Trim(),
        //                        Race_Ethnicity = worksheet.Cells[row, 11].Value.ToString().Trim()
        //                    });
        //                }
        //            }
        //        }
        //        add list to db ..here just read and return
        //                        int totalRow = recipientList.Count();
        //        int flag = 0;
        //        foreach (Recipient rdt in recipientList)
        //        {
        //            int countt = de.Recipients.Where(x => x.IsActive == 1 && x.Email == rdt.Email).ToList().Count();
        //            if (countt > 0)
        //            {
        //                continue;
        //            }

        //            Recipient ud = new Recipient()
        //            {
        //                Institution = rdt.Institution,
        //                FirstName = rdt.FirstName.Trim(),
        //                LastName = rdt.LastName.Trim(),
        //                Email = rdt.Email.Trim(),
        //                CountryOfResidence = rdt.CountryOfResidence,
        //                Position = rdt.Position,
        //                Unit = rdt.Unit,
        //                YearsAtOrganization = rdt.YearsAtOrganization,
        //                Age = rdt.Age,
        //                Gender = rdt.Gender,
        //                Race_Ethnicity = rdt.Race_Ethnicity,
        //                IsActive = 1,
        //                CreatedAt = DateTime.Now,
        //            };

        //            bool chk = false;
        //            if (ud != new Recipient())
        //            {
        //                chk = new RecipientBL().AddRecipient(ud, de);
        //            }

        //            if (chk == false)
        //            {
        //                flag = 1;
        //            }
        //        }

        //        if (flag == 0)
        //        {
        //            return RedirectToAction("AddRecipient", "Admin", new { msg = "File imported successfully.", color = "green" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("AddRecipient", "Admin", new { msg = "File imported with errors.", color = "orange" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("AddRecipient", "Admin", new { msg = ex.Message, color = "red" });
        //    }
        //}


        //public ActionResult ViewRecipient(string msg = "", string color = "black")
        //{
        //    if (ValidateAdminLogin() == false)
        //    {
        //        return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //    }

        //    ViewBag.Message = msg;
        //    ViewBag.Color = color;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult PostUpdateRecipient(Recipient _recipient)
        //{
        //    try
        //    {
        //        if (ValidateAdminLogin() == false)
        //        {
        //            return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //        }

        //        Recipient r = new RecipientBL().GetRecipientById(_recipient.Id, de);
        //        r.Institution = _recipient.Institution;
        //        r.FirstName = _recipient.FirstName.Trim();
        //        r.LastName = _recipient.LastName.Trim();
        //        r.Email = _recipient.Email.Trim();
        //        r.CountryOfResidence = _recipient.CountryOfResidence;
        //        r.Position = _recipient.Position;
        //        r.Unit = _recipient.Unit;
        //        r.YearsAtOrganization = _recipient.YearsAtOrganization;
        //        r.Age = _recipient.Age;
        //        r.Gender = _recipient.Gender;
        //        r.Race_Ethnicity = _recipient.Race_Ethnicity;

        //        bool check = new RecipientBL().UpdateRecipient(r, de);
        //        if (check == true)
        //        {
        //            return RedirectToAction("ViewRecipient", "Admin", new { msg = "Record Updated Successfully", color = "green" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("ViewRecipient", "Admin", new { msg = "Somethings' Wrong", color = "red" });
        //        }
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index", "Error");
        //    }
        //}

        //public ActionResult DeleteRecipient(int id)
        //{
        //    try
        //    {
        //        if (ValidateAdminLogin() == false)
        //        {
        //            return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //        }

        //        Recipient u = new RecipientBL().GetRecipientById(id, de);
        //        u.IsActive = 0;

        //        bool check = new RecipientBL().UpdateRecipient(u, de);
        //        if (check == true)
        //        {
        //            return RedirectToAction("ViewRecipient", "Admin", new { msg = "Record Deleted Successfully", color = "red" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("ViewRecipient", "Admin", new { msg = "Somethings' Wrong", color = "red" });
        //        }
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index", "Error");
        //    }
        //}

        //[HttpPost]
        //public ActionResult GetRecipientList(string Name = "", string Email = "", string institution = "")
        //{
        //    List<Recipient> rlist = new List<Recipient>();
        //    if (!string.IsNullOrEmpty(institution))
        //    {
        //        rlist = new RecipientBL().GetActiveRecipientList(de).Where(x => x.Institution == institution).ToList();
        //        ViewBag.RecipientList = rlist;
        //    }
        //    else
        //    {
        //        rlist = new RecipientBL().GetActiveRecipientList(de).OrderByDescending(x => x.Id).ToList();
        //    }

        //    if (Name != "")
        //    {
        //        rlist = rlist.Where(x => x.FirstName.ToLower().Contains(Name.ToLower()) || x.LastName.ToLower().Contains(Name.ToLower())).ToList();
        //    }
        //    if (Email != "")
        //    {
        //        rlist = rlist.Where(x => x.Email.ToLower().Contains(Email.ToLower())).ToList();
        //    }

        //    int start = Convert.ToInt32(Request["start"]);
        //    int length = Convert.ToInt32(Request["length"]);
        //    int length = rlist.Count();
        //    string searchValue = Request["search[value]"];
        //    string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
        //    string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
        //    string sortDirection = Request["order[0][dir]"];
        //    if (!string.IsNullOrWhiteSpace(sortColumnName))
        //    {
        //        if (sortColumnName != "0")
        //        {
        //            if (sortDirection == "asc")
        //            {
        //                rlist = rlist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
        //            }
        //            else
        //            {
        //                rlist = rlist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
        //            }
        //        }
        //    }
        //    int totalrows = rlist.Count();
        //    filter
        //    if (searchValue != "")
        //    {
        //        rlist = rlist.Where(x => x.Institution.Contains(searchValue) || x.FirstName.Contains(searchValue) || x.LastName.Contains(searchValue) || x.Email.ToLower().Contains(searchValue.ToLower())).ToList();
        //    }


        //    pagination
        //    rlist = rlist.Skip(start).Take(totalrows).ToList();
        //    int totalrowsafterfilterinig = rlist.Count();
        //    List<Recipient> udto = new List<Recipient>();
        //    foreach (Recipient r in rlist)
        //    {
        //        Recipient obj = new Recipient()
        //        {
        //            Id = r.Id,
        //            Institution = r.Institution,
        //            FirstName = r.FirstName,
        //            LastName = r.LastName,
        //            Email = r.Email,
        //            CountryOfResidence = r.CountryOfResidence,
        //            Position = r.Position,
        //            Unit = r.Unit,
        //            YearsAtOrganization = r.YearsAtOrganization,
        //            Age = r.Age,
        //            Gender = r.Gender,
        //            Race_Ethnicity = r.Race_Ethnicity,
        //        };

        //        udto.Add(obj);
        //    }

        //    return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrows }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetRecipientById(int id)
        //{
        //    Recipient r = new RecipientBL().GetRecipientById(id, de);

        //    Recipient obj = new Recipient()
        //    {
        //        Id = r.Id,
        //        Institution = r.Institution,
        //        FirstName = r.FirstName,
        //        LastName = r.LastName,
        //        Email = r.Email,
        //        CountryOfResidence = r.CountryOfResidence,
        //        Position = r.Position,
        //        Unit = r.Unit,
        //        YearsAtOrganization = r.YearsAtOrganization,
        //        Age = r.Age,
        //        Gender = r.Gender,
        //        Race_Ethnicity = r.Race_Ethnicity,
        //    };



        //    return Json(obj, JsonRequestBehavior.AllowGet);
        //}
        #endregion..

        #region Manage Share Survey Link

        //public ActionResult AddShareLink(string msg = "", string color = "black")
        //{
        //    if (ValidateAdminLogin() == false)
        //    {
        //        return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //    }

        //    ViewBag.surveylist = new SurveyBL().GetActiveSurveyList(de).ToList();
        //    ViewBag.institutelist = de.Recipients.Where(x => x.IsActive == 1).Select(x => x.Institution).Distinct().ToList();
        //    ViewBag.userId = gp.ValidateLoggedinUser().Id;
        //    ViewBag.email = "<p>The survey is scenario - based and intended to provide a better understanding of the organizational behaviors of your institution." +
        //                        "There are no right or wrong answers.Instead, you should respond to questions based on your personal experience working at the" +
        //                        "institution.The survey consists of six categories. Each category includes four alternative statements(A, B, C, and D). Please" +
        //                        "rate each statement by dividing 100 points between the four options depending on the similarity to your institution(100 points" +
        //                        "would indicate great similarity, while 0 points would indicate little similarity). The total points for each category must equal" +
        //                        "100.Your responses to the survey will remain anonymous, but will be aggregated to construct a “picture” of the organizational" +
        //                        "culture at your institution." +
        //                        "</ p > ";

        //    ViewBag.Message = msg;
        //    ViewBag.Color = color;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult PostAddShareLink(int SurveyId = -1, string Institution = "", string ids = null, string MailBody = "")
        //{
        //    var idss = ids.Split(',');
        //    try
        //    {
        //        if (ValidateAdminLogin() == false)
        //        {
        //            return RedirectToAction("Login", "Auth", new { msg = "Session Expired! Please Login", color = "red" });
        //        }
        //        CreateSurvey SV = new SurveyBL().GetSurveyById(SurveyId, de);

        //        var recipients = new List<Recipient>();


        //        foreach (var id in idss)
        //        {
        //            var xid = Convert.ToInt32(id);
        //            Recipient rec = new RecipientBL().GetRecipientById(xid, de);
        //            recipients.Add(rec);
        //        }

        //        int checkMailSend = -1;
        //        //var ss = recipients.Select();
        //        if (recipients.Count > 0)
        //        {
        //            if (SurveyId != -1 && Institution != null)
        //            {

        //                _ = Task.Run(async () =>
        //                {
        //                    foreach (var recipient in recipients)
        //                    {
        //                        string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
        //                        bool checkMail = MailSender.SendSurveyLink((int)recipient.Id, recipient.FirstName + " " + recipient.LastName, SV.Title, (int)SurveyId, recipient.Email, gp.ValidateLoggedinUser().Id, BaseUrl, MailBody);
        //                        if (checkMail == true)
        //                        {
        //                            recipient.SendedSurveys += SurveyId.ToString();
        //                            var upd = new RecipientBL().UpdateRecipient(recipient, de);

        //                        }
        //                        else
        //                        {
        //                            checkMailSend = 0;
        //                        }
        //                    }
        //                });
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("AddShareLink", "Admin", new { msg = "No user record found againts the corresponding institute", color = "red" });
        //        }
        //        return RedirectToAction("AddShareLink", "Admin", new { msg = "Survey link Successfully sent to the user(s)", color = "green" });
        //    }
        //    catch
        //    {

        //        return RedirectToAction("AddShareLink", "Admin", new { msg = "Mail sending fail!", color = "red" });

        //    }
        //}
        #endregion

        #region Manage Company
        public int validateCampanyName(string Name)
        {
            int count = new CompanyBL().GetActiveCompanyList(de).Where(x => x.CompanyName == Name).Count();

            if (Name == "")
            {
                return 2;
            }
            if (count > 0)
            {
                return 0;
            }
            return 1;
        }

        public string GenerateUrl(string companyName = "")
        {
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
           
            var charsToRemove = new string[] { " ", "`", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "+", "=", "{", "[", "}", "]", ";", ":", "'", ",", "<", ".", ">", "/", "?", "'\'", "|" };

            foreach (var c in charsToRemove)
            {
                companyName = companyName.Replace(c, string.Empty);
            }
            companyName = companyName.ToLower();
            string url = BaseUrl +"survey/"+ companyName;

            int count = new CompanyBL().GetActiveCompanyList(de).Where(x => x.CompanyUrl == url).Count();

            if (count > 0)
            {
                url = "";
            }

            return JsonConvert.SerializeObject(url, Formatting.Indented, 
                new JsonSerializerSettings(){
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostAddCompany(Company _company,HttpPostedFileBase imageFiles,string[] Questions,string[] values)
        {

            var ImagePath = "";
            if (imageFiles != null && imageFiles.ContentLength > 0)
            {
                var fileName = DateTime.Now.Ticks + "_" + Path.GetFileName(imageFiles.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/assets/images"), fileName);
                imageFiles.SaveAs(path);


                ImagePath = "/Content/assets/images/" + fileName;

            }


            //key value pair data 

            Dictionary<string, string> myDictionary = new Dictionary<string, string>();

            for (int i = 0; i < Questions.Length; i++)
            {
                string key = Questions[i];
                string value = values[i];
                myDictionary.Add(key, value);
            }

            var QuestionAndValues = JsonConvert.SerializeObject(myDictionary);
            //key value pair data 

            Company company = new Company()
            {
                CompanyUrl = _company.CompanyUrl,
                CompanyName = _company.CompanyName.Trim(),
                TotalCompanySurvey = _company.TotalCompanySurvey,
                CompanyLogo = ImagePath,
                Company_LandingPageData = _company.Company_LandingPageData,
                DemoGraphicData = _company.DemoGraphicData,
                DemoGraphicDropDowns = QuestionAndValues, //serialize form 
                IsActive = 1,
                CreatedAt = DateTime.Now
            };

            //var Question = JsonConvert.DeserializeObject(QuestionAndValues);

            if(company.DemoGraphicDropDowns.Contains("'") || company.DemoGraphicData.Contains("'"))
            {
                db.Companies.Add(company);
                int saveChanges = db.SaveChanges();
                if(saveChanges > 0)
                {
                    // for create a log of create company 
                    var dt = DateTime.Now.ToString("g");

                    string fileName = "logs.txt";
                    string path = Path.Combine(Server.MapPath("~/Content/assets"), fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        using (StreamWriter writer = System.IO.File.CreateText(path))
                        {
                            writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Name: " + _company.CompanyName + " ------------------------ Company Url: " + _company.CompanyUrl + " ------------------------ Company Survey Categories: " + _company.TotalCompanySurvey);

                            writer.Close();

                        }

                    }
                    else
                    {
                        using (StreamWriter writer = new StreamWriter(path, true))
                        {
                            writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Name: " + _company.CompanyName + " ------------------------ Company Url: " + _company.CompanyUrl + " ------------------------ Company Survey Categories: " + _company.TotalCompanySurvey);

                            writer.Close();
                        }
                    }

                    // end log create
                    return RedirectToAction("CreateCompany", "Home", new { msg = "Record is Added Successfully", color = "green" });

                }

                else
                {
                    
                    return RedirectToAction("CreateCompany", "Home", new { msg = "Record is not Added Successfully", color = "red" });

                }

            }
            else
            {
                if (!new CompanyBL().AddCompany(company, de))
                {
                    return RedirectToAction("CreateCompany", "Home", new { msg = "Record is not Added Successfully", color = "red" });
                }

                // for creating log file
                var dt = DateTime.Now.ToString("g");

                string fileName = "logs.txt";
                string path = Path.Combine(Server.MapPath("~/Content/assets"), fileName);
                if (!System.IO.File.Exists(path))
                {
                    using (StreamWriter writer = System.IO.File.CreateText(path))
                    {
                        writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Name: " + _company.CompanyName + " ------------------------ Company Url: " + _company.CompanyUrl + " ------------------------ Company Survey Categories: " + _company.TotalCompanySurvey);

                        writer.Close();

                    }

                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.WriteLineAsync("DateTime: " + dt + "---------------- Company Name: " + _company.CompanyName + " ------------------------ Company Url: " + _company.CompanyUrl + " ------------------------ Company Survey Categories: " + _company.TotalCompanySurvey);

                        writer.Close();
                    }
                }

            }


            
            return RedirectToAction("CreateCompany", "Home", new { msg = "Record is Added Successfully", color="green" });
        }

        [HttpGet]
        public ActionResult GetAllCompanies()
        {
            List<Company> list = new List<Company>();
            List<Company> list2 = new List<Company>();
            list2 = new CompanyBL().GetActiveCompanyList(de).Where(x => x.IsActive == 1).OrderByDescending(x=>x.Id).ToList();
            foreach (Company company in list2)
            {
                int count = new SurveyBL().GetAllSurveysByCompanyId(company.Id, de).Count();
                if(count <= 0)
                {
                    list.Add(company);
                }
            }
            List<Company> udto = new List<Company>();
            foreach (Company u in list)
            {
                Company obj = new Company()
                {
                    Id = u.Id,
                    CompanyName = u.CompanyName,
                    CompanyUrl = u.CompanyUrl,
                    TotalCompanySurvey= u.TotalCompanySurvey,
                };

                udto.Add(obj);
            }
            return Json(udto, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllActiveCompanies()
        {
            List<Company> list = new List<Company>();
            list = new CompanyBL().GetActiveCompanyList(de).Where(x => x.IsActive == 1).ToList();

            List<Company> udto = new List<Company>();
            foreach (Company u in list)
            {
                Company obj = new Company()
                {
                    Id = u.Id,
                    CompanyName = u.CompanyName,
                    CompanyUrl = u.CompanyUrl,
                    TotalCompanySurvey = u.TotalCompanySurvey,
                };

                udto.Add(obj);
            }
            return Json(udto, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}