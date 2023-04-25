using CIPlatformMain.Entities.Data;
using CIPlatformMain.Entities.Models;
using CIPlatformMain.Entities.ViewModel;
using CIPlatformMain.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using MimeKit;
using NuGet.Common;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using X.PagedList;
using System.Drawing.Printing;
using System.Drawing;
using NuGet.Protocol;
using System.Linq;
using CIPlatformMain.Repository.Interface;

namespace CIPlatformMain.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CidatabaseContext _cidatabase;
        private readonly IUser _iuser;
        private readonly IHome _ihome;

        public HomeController(ILogger<HomeController> logger, CidatabaseContext cidatabase, IUser iuser, IHome ihome)
        {
            _iuser = iuser;
            _ihome = ihome;
            _logger = logger;
            _cidatabase = cidatabase;
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.resetstatus = TempData["Resetstatus"];
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserID");
            return RedirectToAction("Login");
        }



        [HttpPost]
        public IActionResult Login(User _user)
        {

            User User = _iuser.Login(_user);

            
            var Deletestatus = _iuser.DeletedUser(_user);
            if (User != null)
            {

                HttpContext.Session.SetString("Username", User.FirstName);
                HttpContext.Session.SetString("UserID", User.UserId.ToString());
                HttpContext.Session.SetString("UserEmail", User.Email);

                if (User.Avatar != null)
                {
                    HttpContext.Session.SetString("UserAvatar", User.Avatar);
                }
                else
                {
                    HttpContext.Session.SetString("UserAvatar", " ");
                }
               


                TempData["logintoast"] = 1;
                return RedirectToAction("LandingPage");

            }
           
            else if (Deletestatus != null && Deletestatus.DeletedAt != null)
            {
                ViewBag.Logincredentials = 1;
                return View();
            }
            else
            {
                ViewBag.Logincredentials = 0;
                return View(_user);
            }
           
        }

        public IActionResult Registration()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Registration(User _user)
        {



            /* var status = _cidatabase.Users.Where(m => m.Email == _user.Email).FirstOrDefault();*/
            var status = _iuser.CheckEmailInDatabase(_user);
            if (status == 1)
            {

                if (ModelState.IsValid)
                {

                    _iuser.Register(_user);
                    TempData["Registeredstatus"] = 1;
                    return RedirectToAction("Login");
                }
            }
            else
            {
                ModelState.AddModelError("Email", "User with same email-id already exists");
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult Forgotpassword()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Forgotpassword(String useremail)
        {
            var registereduser = _cidatabase.Users.Where(u => u.Email == useremail).FirstOrDefault();
            if (registereduser == null)
            {
                ViewBag.forgetstatus = 0;
            }
            else
            {
                var token = Guid.NewGuid().ToString();
                var passwordReset = new PasswordReset
                {

                    Email = useremail,
                    Token = token

                };
                HttpContext.Session.SetString("Token", token);
                HttpContext.Session.SetString("Email", useremail);
                _cidatabase.PasswordResets.Add(passwordReset);
                _cidatabase.SaveChanges();

                var mailbody = "<h1>Click link to reset password</h1><br><h2><a href='" + "https://localhost:7037/Home/ResetPassword?token=" + token + "'>Reset Password</a></h2>";
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("vasudedakiya3@gmail.com"));
                email.To.Add(MailboxAddress.Parse(useremail));
                email.Subject = "Reset Your Password";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailbody };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate("vasudedakiya3@gmail.com", "whdatdbclkgporxj");
                smtp.Send(email);
                smtp.Disconnect(true);


                ViewBag.EmailStatus = 1;
                return View();

            }

            return View();
        }


        public IActionResult LandingPage(string searchin, string themefilter, string cityfilter, string countryfilter, int sortby, int pg = 1)//change here
        {
           
            ViewBag.RatingsAvg = _ihome.GetRatings().GroupBy(r => r.MissionId)
                                     .Select(
                                                g => new
                                                {
                                                    MissionId = g.Key,
                                                    AverageRating = g.Average(r => r.Rating)
                                                })
                                     .ToList();

            if (HttpContext.Session.GetString("UserID") != null)
            {
                var UserId= long.Parse(HttpContext.Session.GetString("UserID"));
                ViewBag.userid = UserId;
                var missiondata = _ihome.GetMissions(UserId);

                

               var fav = missiondata.FavMission.Where(f => f.UserId == int.Parse(HttpContext.Session.GetString("UserID"))).Select(f => f.MissionId).ToList();

                //sort Missions
                if (sortby > 0)
                {
                    switch (sortby)
                    {
                        case 1:
                            missiondata.Missions = missiondata.Missions.OrderByDescending(p => p.CreatedAt).ToList();

                            break;

                        case 2:
                            missiondata.Missions = missiondata.Missions.OrderBy(p => p.CreatedAt).ToList();

                            break;
                        case 3:
                            missiondata.Missions = missiondata.Missions.OrderBy(p => p.StartDate).ToList();

                            break;
                        case 4:
                            missiondata.Missions = missiondata.Missions.OrderByDescending(p => p.StartDate).ToList();

                            break;

                        case 5:
                            missiondata.Missions = missiondata.Missions.Where(m => fav.Contains(m.MissionId)).ToList();

                            break;



                    }
                }
                
                //Filters
                if (countryfilter != null)
                {
                    missiondata.Missions = missiondata.Missions.Where(m => countryfilter.Contains(m.Country.Name)).ToList();
                    ViewData["countryid"] = missiondata.Country.Where(C => countryfilter.Contains(C.Name)).Select(C => C.CountryId).ToList();

                }
                if (searchin != null)
                {
                    missiondata.Missions = missiondata.Missions.Where(m => m.Title.ToLower().Contains(searchin.ToLower()) || m.Theme.Title.Contains(searchin) || m.Country.Name.Contains(searchin) || m.City.Name.Contains(searchin)).ToList();

                }
                if (themefilter != null)
                {
                    missiondata.Missions = missiondata.Missions.Where(m => themefilter.Contains(m.Theme.Title)).ToList();

                }
                if (cityfilter != null)
                {
                    missiondata.Missions = missiondata.Missions.Where(m => cityfilter.Contains(m.City.Name)).ToList();


                }

                ViewBag.Missionskills = (from n in missiondata.MissionSkills
                                         join c in missiondata.Skills on n.SkillId equals c.SkillId
                                         select new
                                         {
                                             c.SkillName,
                                             n.MissionId

                                         }).ToArray();

                ViewData["MissionCount"] = missiondata.Missions.Count();
                const int pageSize = 9;
                if (pg < 1)
                    pg = 1;

                int recsCount = missiondata.Missions.Count();

                var pager = new Pager(recsCount, pg, pageSize);

                int recSkip = (pg - 1) * pageSize;

                var data = missiondata.Missions.Skip(recSkip).Take(pager.PageSize).ToList();

                this.ViewBag.Pager = pager;

                missiondata.Missions = data.ToList();

                return View(missiondata);
            }
            else
            {

                return RedirectToAction("Index");
            }

        }

        public IActionResult PrivacyPolicy()
        {
            long User_id = long.Parse(HttpContext.Session.GetString("UserID"));
            var user = _iuser.GetUser(User_id);
            ViewData["Name"] = user.FirstName;
            
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Volunteering_Mission_Page(int missionid, string commenttext, int favvalue, int user_rating, int to_userid)
        {
          
            
            
            MissionDetails missiondetails = _ihome.GetMissionDetails(missionid);
            
            ViewBag.userid = int.Parse(HttpContext.Session.GetString("UserID"));
           

           
            if (missionid > 0)
            {
                HttpContext.Session.SetString("Mission_ID", missionid.ToString());
            }

            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            if (HttpContext.Session.GetString("Mission_ID") != null)
            {
                missionid = int.Parse(HttpContext.Session.GetString("Mission_ID"));
            }


            var ratingstatus = _cidatabase.MissionRatings.Where(r => r.MissionId == missionid && r.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).FirstOrDefault();

          


            //Add Mission To Fav
            ViewData["missionfav"] = _cidatabase.FavoriteMissions.Where(f => f.MissionId == missionid && f.UserId == userid).Count().ToString();


            ViewBag.Userrating = _cidatabase.MissionRatings.Where(r => r.MissionId == missionid && r.UserId == userid).Select(r => r.Rating).FirstOrDefault();

            
           



            Mission objCategoryList2 = missiondetails.Mission.Where(m => m.MissionId == missionid).FirstOrDefault();


            if (objCategoryList2 != null)
            {
                var title = objCategoryList2.Title;
                var missiontheme = objCategoryList2.Theme.Title;
                var missioncity = objCategoryList2.City.Name;


                //missiondetails.RelatedMission = missiondetails.Mission.Where(m => m.Theme.Title.Contains(missiontheme) || m.Title.Contains(title) || m.City.Name.Contains(missioncity) && m.MissionId != missionid).Take(3).ToList();

            }
            ViewBag.skills = (from n in _cidatabase.MissionSkills
                              join c in _cidatabase.Skills on n.SkillId equals c.SkillId
                              where n.MissionId == missionid
                              select new
                              {
                                  c.SkillName

                              }).ToArray();
            ViewBag.Comment = (from n in _cidatabase.Comments
                               join c in _cidatabase.Users on n.UserId equals c.UserId
                               where n.MissionId == missionid
                               select new
                               {
                                   c.Avatar,
                                   c.FirstName,
                                   n.CommentText,
                                   n.CreatedAt
                               }).ToArray();

            return View(missiondetails);
        }

        //To Add Mission To Favrouite
        public RedirectResult Favrouite(int favvalue,int MissionId)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            var fav_mission_id = MissionId;
           
            ViewData["missionfav"] = _cidatabase.FavoriteMissions.Where(f => f.MissionId == fav_mission_id && f.UserId == userid).Count().ToString();

           
            FavoriteMission favobj = new FavoriteMission();

            if (favvalue == 1)
            {
                if (ViewData["missionfav"] == "0")
                {
                    favobj.MissionId = fav_mission_id;
                    favobj.UserId = long.Parse(HttpContext.Session.GetString("UserID"));
                    _cidatabase.FavoriteMissions.Add(favobj);
                    _cidatabase.SaveChanges();
                }
            }
            if (favvalue == 2)
            {
                var mis = _cidatabase.FavoriteMissions.Where(f => f.MissionId == fav_mission_id && f.UserId == userid).FirstOrDefault();
                _cidatabase.FavoriteMissions.Remove(mis);
                _cidatabase.SaveChanges();
            }
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
            // return RedirectToAction("Volunteering_Mission_Page", new {mission_id= fav_mission_id });
        }

        //To Post Comment
        public RedirectResult AddComment(string commenttext)
        {
            var comment_mission_id = int.Parse(HttpContext.Session.GetString("Mission_ID"));
            if (commenttext != null)
            {
                Comment obj = new Comment();
                obj.CommentText = commenttext;
                obj.MissionId = comment_mission_id;
                obj.UserId = long.Parse(HttpContext.Session.GetString("UserID"));
                obj.CreatedAt = DateTime.Now;

                _cidatabase.Comments.Add(obj);
                _cidatabase.SaveChanges();

            }
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }

        //To Add Ratings
        public RedirectResult Giverating(int user_rating)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            var rating_mission_id = int.Parse(HttpContext.Session.GetString("Mission_ID"));
            var ratingstatus = _cidatabase.MissionRatings.Where(r => r.MissionId == rating_mission_id && r.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).FirstOrDefault();
            if (ratingstatus == null)
            {
                if (user_rating > 0)
                {
                    var ratingobj = new MissionRating();
                    ratingobj.MissionId = rating_mission_id;
                    ratingobj.UserId = long.Parse(HttpContext.Session.GetString("UserID"));
                    ratingobj.Rating = user_rating;
                    _cidatabase.MissionRatings.Add(ratingobj);
                    _cidatabase.SaveChanges();
                }
            }
            if (ratingstatus != null)
            {
                if (user_rating > 0)
                {
                    var mission_rating = _cidatabase.MissionRatings.Where(r => r.MissionId == rating_mission_id && r.UserId == userid).SingleOrDefault();
                    mission_rating.Rating = user_rating;
                    _cidatabase.Update(mission_rating);
                    _cidatabase.SaveChanges();
                }

            }
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }

        //To Recommend To Co-Worker
        public RedirectResult Recommend(int missionid, int to_userid)
        {

            var from_userid = long.Parse(HttpContext.Session.GetString("UserID"));
            if (from_userid != 0 && to_userid != 0 && missionid != 0)
            {
                _ihome.Recommend(missionid, from_userid, to_userid);
            }
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }

        //To Apply To Mission
        public RedirectResult MissionApplication(int missionid)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
           
            MissionApplication missionApplication = new MissionApplication();
            missionApplication.MissionId = missionid;
            missionApplication.UserId = userid;

            if (missionid != 0)
            {
                Mission missionobj = _cidatabase.Missions.Where(m => m.MissionId == missionid).FirstOrDefault();
               
                if(missionobj != null)
                {
                    missionobj.SeatsLeft = missionobj.SeatsLeft - 1;

                    _cidatabase.Update(missionobj);
                    _cidatabase.SaveChanges();
                }
              
            }
            
            _cidatabase.Add(missionApplication);
            _cidatabase.SaveChanges(true);

            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }


        [HttpGet]
        public IActionResult Resetpassword()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Resetpassword(PasswordReset _user)
        {
            var token = HttpContext.Session.GetString("Token");
            var email = HttpContext.Session.GetString("Email");
            var validatetoken = _cidatabase.PasswordResets.FirstOrDefault(m => m.Token == token);
            if (validatetoken != null)
            {
                var userdata = _cidatabase.Users.Where(m => m.Email == validatetoken.Email).FirstOrDefault();
                userdata.Password = _user.Password;
                _cidatabase.Users.Update(userdata);
                _cidatabase.SaveChanges();
                HttpContext.Session.Remove(token);
                TempData["Resetstatus"] = 1;

                return RedirectToAction("Login");
            }
            TempData["Resetstatus"] = 0;
            return RedirectToAction("Forgotpassword");
        }


        public RedirectResult Contactus(string User_id, string Subject, string Message)
        {
            var result = false;
            if (!string.IsNullOrEmpty(User_id))
            {
                var user_id = long.Parse(User_id);
                result = _ihome.Contact_us(user_id, Subject, Message);
            }
            if (result != false)
            {
                TempData["result"] = 1;
            }
            else
            {
                TempData["result"] = null;
            }
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}