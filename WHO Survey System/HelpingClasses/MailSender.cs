using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace WHO_Survey_System.HelpingClasses
{
    public class MailSender
    {
        public static bool SendForgotPasswordEmail(string email, string BaseUrl = "")
        {
            try
            {
                string MailBody = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'>" +
                                  "</div> </nav><center><div><h1 class='text-center'>Password Reset!</h1>" +
                                  "<p class='text-center'>To reset your password click on the button below. (Note: This link expires after 24 hours)</p><br>" +
                                  "<button style='background-color: rgb(0,174,239);'>" +
                                  "<a href='" + BaseUrl + "Auth/ResetPassword?email=" + StringCipher.Base64Encode(email) + "&time=" + StringCipher.Base64Encode(DateTime.Now.ToString("MM/dd/yyyy")) + "' style='text-decoration:none;font-size:15px;color:white;'>Reset Password</a>" +
                                  "</button></div></center>" +
                                  "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                MailBody += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";

                RestClient client = new RestClient(); //intializing Rest client object 
                client.BaseUrl = new Uri("https://api.mailgun.net/v3"); // this is base url (remains same)
                client.Authenticator = new HttpBasicAuthenticator("api", "0b6d34ca5bbaafa8ce4eb81c91999d40-7b8c9ba8-7aac14d4");//copy Private Api Key from Api security (https://app.mailgun.com/app/account/security/api_keys)
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "deiready.com", ParameterType.UrlSegment);  //Create a new domain from side bar "Sending -> Domains -> Add New Domain"
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "survey@deiready.com"); //Can be used any email here, without any password requirements
                request.AddParameter("to", email); //email where you want to send mail
                request.AddParameter("subject", "Survey System | Password Reset"); //subject of mail
                request.AddParameter("html", MailBody); //send html code generated above
                request.Method = Method.POST;
                string response = client.Execute(request).Content.ToString();
                if (response != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // To Admin
        public static bool SendEmailToAdmin(string Name = "", string Subject = "", string Email = "", string adminEmail = "", string Message = "", string BaseUrl = "")
        {
            try
            {
                string MailBody;
                string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
                text += "  font-family: 'Bree Serif', serif; }";
                text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
                if (Message == null)
                {
                    text = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'>" +
                                 "</div></nav><center><div><h1 class='text-center'>Message From End User!</h1>" +
                                 "<p class='text-center'>" + "Name of the user: " + (Name) + "Send you an Empty message" + "</p><br>" +
                                 "<p class='text-center'>" + "Email of the user: " + (Email) + "</p><br>" +
                                 "<button style='background-color: rgb(0,174,239);'>" + "<a href='" + BaseUrl + "Home/Index" + "' style='text-decoration:none;font-size:15px;color:white;'>Go Back TO Home page</a>" + "</button>" +
                                 "<p style='color:red;'>Please move this mail into your inbox.</p></div></center>";
                    text += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                    text += " .line {       margin-bottom: 20px; }";

                    MailBody = text;
                }
                else
                {
                    text = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'>" +
                           "</div></nav><center><div><h1 class='text-center'>Message From End User!</h1>" +
                           "<p class='text-center'>" + "Name of the user: " + (Name) + "</p>" +
                           "<p class='text-center'>" + "Email of the user: " + (Email) + "</p>" +
                           "<p class='text-left'>" + "Message from the user: " + (Message) + "</p><br>" +
                           "<button style='background-color: rgb(0,174,239);'>" + "<a href='" + BaseUrl + "Home/Index" + "' style='text-decoration:none;font-size:15px;color:white;'>Go Back TO Home page</a>" + "</button>" +
                           "<p style='color:red;'>Please move this mail into your inbox.</p></div></center>";
                    text += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                    MailBody = text;
                }
                RestClient client = new RestClient(); //intializing Rest client object 
                client.BaseUrl = new Uri("https://api.mailgun.net/v3"); // this is base url (remains same)
                client.Authenticator = new HttpBasicAuthenticator("api", "0b6d34ca5bbaafa8ce4eb81c91999d40-7b8c9ba8-7aac14d4"); //copy Private Api Key from Api security (https://app.mailgun.com/app/account/security/api_keys)
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "deiready.com", ParameterType.UrlSegment);  //Create a new domain from side bar "Sending -> Domains -> Add New Domain"
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "survey@deiready.com"); //Can be used any email here, without any password requirements
                request.AddParameter("to", adminEmail); //email where you want to send mail
                request.AddParameter("subject", Subject); //subject of mail
                request.AddParameter("html", MailBody); //send html code generated above
                request.Method = Method.POST;
                string response = client.Execute(request).Content.ToString();
                if (response != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //To User
        public static bool SendEmailToUser(string email, string adminName, string adminEmail, string BaseUrl = "")
        {
            try
            {
                string MailBody;
                string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style> * {";
                text += "  font-family: 'Bree Serif', serif; }";
                text += " .list-group-item {       border: none;  }  .hor {border-bottom: 5px solid black;}";
                text += " .line {       margin-bottom: 20px; }";
                text = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'>" +
                             "</div></nav><center><div><h1 class='text-center'>Message From Admin!</h1>" +
                             "<p class='text-bold'>" + "Thank you so much for your interest!" + "</p>" +
                             "<p class='text-bold'>" + "Please, do contact us if you have additional queries.Thanks again!" + "</p><br>" +
                             "<p class='text-right'>" + "Best regards," + "</p>" +
                             "<p class='text-right'>" + adminName + "</p>" +
                             "<p class='text-right'>" + "For further queries, hit this email address: " + adminEmail + "</p>" +
                             "<p class='text-right'>" + "Weapon Store" + "</p>" +
                             "<button style='background-color: rgb(0,174,239);'>" + "<a href='" + BaseUrl + "Home/Index" + "' style='text-decoration:none;font-size:15px;color:white;'>Go Back TO Home page</a>" + "</button>" +
                             "<p style='color:red;'>Please move this mail into your inbox.</p></div></center>";
                text += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                MailBody = text;
                MailBody = text;
                RestClient client = new RestClient(); //intializing Rest client object 
                client.BaseUrl = new Uri("https://api.mailgun.net/v3"); // this is base url (remains same)
                client.Authenticator = new HttpBasicAuthenticator("api", "0b6d34ca5bbaafa8ce4eb81c91999d40-7b8c9ba8-7aac14d4"); //copy Private Api Key from Api security (https://app.mailgun.com/app/account/security/api_keys)
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "deiready.com", ParameterType.UrlSegment);  //Create a new domain from side bar "Sending -> Domains -> Add New Domain"
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "survey@deiready.com"); //Can be used any email here, without any password requirements
                request.AddParameter("to", email); //email where you want to send mail
                request.AddParameter("subject", "Survey System"); //subject of mail
                request.AddParameter("html", MailBody); //send html code generated above
                request.Method = Method.POST;
                string response = client.Execute(request).Content.ToString();
                if (response != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendPasscode(string email, string BaseUrl = "", string Passcode = "", string Lang = "") /// save pointle
        {
            try
            {

                string path = HttpContext.Current.Server.MapPath("~/App_Data/emailTemp.txt");
                string text = "";
                if (Lang == "" || Lang == "English")
                {
                    text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                              "<h2>Welcome to the SURVEY.</h2>" +
                                              "<span style = 'font-size: 30px;'> YOUR PASSCODE:</span>" +
                                              "<p style = 'margin-top: 20px;'><p class='btn btn-primary' style='font-size:30px;'>" + Passcode + "</p></p>" +

                                            "</div>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p> If you have any questions as you take the survey, please contact<a href='mailto:survey@deiready.com'> survey@deiready.com</a></p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";
                }
                if (Lang == "Portuguese")
                {
                    text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                              "<h2>Bem-vindo ao INQUÉRITO DE DIAGNÓSTICO DA CULTURA INSTITUCIONAL NA OMS.</h2>" +
                                              "<span style = 'font-size: 30px;'>O SEU CÓDIGO DE ACESSO:</span>" +
                                              "<p style = 'margin-top: 20px;'><p class='btn btn-primary' style='font-size:30px;'>" + Passcode + "</p></p>" +

                                            "</div>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p>Se tiver alguma dúvida enquanto responde ao inquérito, contacte<a href='mailto:survey@deiready.com'> survey@deiready.com</a></p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";
                }
                if (Lang == "Spanish")
                {
                    text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                              "<h2>Bienvenido al ESTUDIO DIAGNÓSTICO DE LA CULTURA INSTITUCIONAL DE LA OMS.</h2>" +
                                              "<span style = 'font-size: 30px;'>SU CÓDIGO DE ACCESO:</span>" +
                                              "<p style = 'margin-top: 20px;'><p class='btn btn-primary' style='font-size:30px;'>" + Passcode + "</p></p>" +

                                            "</div>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p>Si tiene alguna duda cuando esté haciendo la encuesta, póngase en contacto con<a href='mailto:survey@deiready.com'> survey@deiready.com</a></p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";
                }
                if (Lang == "French")
                {
                    text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                              "<h2>Bienvenue dans l’ENQUÊTE DIAGNOSTIQUE DE L’OMS SUR LA CULTURE INSTITUTIONNELLE.</h2>" +
                                              "<span style = 'font-size: 30px;'>VOTRE CODE D’ACCÈS:</span>" +
                                              "<p style = 'margin-top: 20px;'><p class='btn btn-primary' style='font-size:30px;'>" + Passcode + "</p></p>" +

                                            "</div>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p>Si vous avez des questions pendant que vous participez à l’enquête, veuillez contacter<a href='mailto:survey@deiready.com'> survey@deiready.com</a></p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";
                }
                if (Lang == "Arabic")
                {
                    text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                              "<h2>مرحبًا بك في الاستبيان الاستقصائي للثقافة المؤسسية في منظمة الصحة العالمية</h2>" +
                                              "<span style = 'font-size: 30px;'>:رمز المرور الخاص بك</span>" +
                                              "<p style = 'margin-top: 20px;'><p class='btn btn-primary' style='font-size:30px;'>" + Passcode + "</p></p>" +

                                            "</div>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p><a href='mailto:survey@deiready.com'> survey@deiready.com</a>:إذا كانت لديك أي أسئلة أثناء المشاركة في الاستبيان، يُرجى التواصل عبر</p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";
                }
                if (Lang == "Chinese")
                {
                    text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                              "<h2>欢迎参加世界卫生组织的制度文化诊断调查。</h2>" +
                                              "<span style = 'font-size: 30px;'>您的密码：</span>" +
                                              "<p style = 'margin-top: 20px;'><p class='btn btn-primary' style='font-size:30px;'>" + Passcode + "</p></p>" +

                                            "</div>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p>如果您在参加调查时有任何疑问，请联系<a href='mailto:survey@deiready.com'> survey@deiready.com</a></p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";
                }
                if (Lang == "Russian")
                {
                    text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                              "<h2>Диагностический опрос о корпоративной культуре в ВОЗ.</h2>" +
                                              "<span style = 'font-size: 30px;'> ВАШ ПАРОЛЬ:</span>" +
                                              "<p style = 'margin-top: 20px;'><p class='btn btn-primary' style='font-size:30px;'>" + Passcode + "</p></p>" +

                                            "</div>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p> Если у вас возникнут вопросы при заполнении этой анкеты, свяжитесь с нами по адресу<a href='mailto:survey@deiready.com'> survey@deiready.com</a></p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";
                }

                RestClient client = new RestClient(); //intializing Rest client object 
                client.BaseUrl = new Uri("https://api.mailgun.net/v3"); // this is base url (remains same)
                client.Authenticator = new HttpBasicAuthenticator("api", "0b6d34ca5bbaafa8ce4eb81c91999d40-7b8c9ba8-7aac14d4"); //copy Private Api Key from Api security (https://app.mailgun.com/app/account/security/api_keys)
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "deiready.com", ParameterType.UrlSegment);  //Create a new domain from side bar "Sending -> Domains -> Add New Domain"
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "survey@deiready.com"); //Can be used any email here, without any password requirements
                request.AddParameter("to", email); //email where you want to send mail
                request.AddParameter("subject", "DEI Ready Survey Passcode"); //subject of mail
                request.AddParameter("html", System.Web.HttpUtility.HtmlDecode(text)); //send html code generated above
                request.Method = Method.POST;
                string response = client.Execute(request).Content.ToString();
                if (response != null)
                {
                    return true;
                }
                return false;


            }
            catch (Exception e)
            {
                return e.Message.Any();
            }
        }


        public static bool SendSurveyLink(int IDNumber, string name, string SurveyTitle, int SurveyId, string email, int UserId, string BaseUrl = "", string CustomMail = "") /// save pointle
        {
            try
            {
                

                    string path = HttpContext.Current.Server.MapPath("~/App_Data/emailTemp.txt");
                    string text = System.IO.File.ReadAllText(path) +
                                "<tr>" +
                                  "<td class='bg_white'>" +
                                    "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                      "<tr>" +
                                        "<td class='bg_dark email-section' style='text-align:center; background-color: #008DC9 !important;'>" +
                                            "<div class='heading -section heading-section-white' style='padding-bottom: 10px;'>" +
                                                "<span class='subheading'>Dear <b>" + name.ToUpper() + ",</b></span>" +
                                              "<h2>Welcome to the " + SurveyTitle.ToUpper() + ".</h2>" +
                                                CustomMail +
                                                "<p>" +
                                                    "By clicking the<i style='font-weight:600;'> Click Here to Start the Survey</i> " +
                                                    " button below, you indicate that you have read the description of the survey and agree to participate in the study." +
                                                "</p>" +
                                            "</div>" +
                                            "<p><a href='" + BaseUrl + "/Home/Index?RecpID=" + IDNumber + "&SurveyId=" + SurveyId + "' class='btn btn-primary'>Click Here to Start the Survey</a></p>" +
                                        "</td>" +
                                    "</table>" +
                                  "</td>" +
                                "</tr>" +
                            "</table>" +
                            "<table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:auto;'>" +
                            "<tr>" +
                                "<td valign='middle' class='bg_black footer email-section'>" +
                                    "<table>" +
                                    "<tr> " +
                                    "<td valign = 'top' width='100%'>" +
                                        "<table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%'>" +
                                        "<tr> " +
                                            "<td style='text-align:center;'>" +

                                                "<p> If you have any questions as you take the survey, please contact<a href='mailto:survey@deiready.com'> survey@deiready.com</a></p>" +
                                            "</td>" +
                                        "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                    "</tr>" +
                                "</table>" +
                                "</td>" +
                            "</tr>" +
                            "</table>" +
                        "</div>" +
                        "</center>" +
                    "</body>" +
                    "</html>";


                    RestClient client = new RestClient(); //intializing Rest client object 
                    client.BaseUrl = new Uri("https://api.mailgun.net/v3"); // this is base url (remains same)
                    client.Authenticator = new HttpBasicAuthenticator("api", "0b6d34ca5bbaafa8ce4eb81c91999d40-7b8c9ba8-7aac14d4"); //copy Private Api Key from Api security (https://app.mailgun.com/app/account/security/api_keys)
                    RestRequest request = new RestRequest();
                    request.AddParameter("domain", "deiready.com", ParameterType.UrlSegment);  //Create a new domain from side bar "Sending -> Domains -> Add New Domain"
                    request.Resource = "{domain}/messages";
                    request.AddParameter("from", "survey@deiready.com"); //Can be used any email here, without any password requirements
                    request.AddParameter("to", email); //email where you want to send mail

                    //request.AddParameter("to", "hafizmassamtabraizkhan@gmail.com"); //email where you want to send mail
                    //request.AddParameter("to", "masim_khan1996@hotmail.com"); //email where you want to send mail
                    //request.AddParameter("to", "itxdaxh@gmail.com"); //email where you want to send mail
                    //request.AddParameter("to", "faizan_fiaz2011@yahoo.com"); //email where you want to send mail
                    //request.AddParameter("to", "FaisalBhatti54@yahoo.com"); //email where you want to send mail
                    //request.AddParameter("to", "JamesMiller11147@gmail.com"); //email where you want to send mail
                    //request.AddParameter("to", "Sherdil5362@gmail.com"); //email where you want to send mail
                    //request.AddParameter("to", "FaisalManzoor11147@gmail.com"); //email where you want to send mail
                    //request.AddParameter("to", "massam@nodlays.com"); //email where you want to send mail

                    request.AddParameter("subject", "Survey System"); //subject of mail
                    request.AddParameter("html", System.Web.HttpUtility.HtmlDecode(text)); //send html code generated above
                    request.Method = Method.POST;
                    string response = client.Execute(request).Content.ToString();
                    if (response != null)
                    {
                        return true;
                    }
                    return false;
                
                
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}