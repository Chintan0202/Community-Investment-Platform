using CIPlatformMain.Entities.Data;
using Microsoft.AspNetCore.Mvc;
using CIPlatformMain.Entities.ViewModel;
using CIPlatformMain.Entities.Models;
using System.Text.RegularExpressions;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using CIPlatformMain.Repository.Interface;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CIPlatformMain.Controllers
{
    public class StoryController : Controller
    {
        private readonly CidatabaseContext _cidatabase;
        private readonly IStory _istory;

        public StoryController( CidatabaseContext cidatabase, IStory istory)
        {
            _cidatabase = cidatabase;
            _istory = istory;
        }
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetStory(long MissionId)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            Story story=_cidatabase.Stories.Where(s=>s.MissionId == MissionId && s.UserId==userid && s.Status=="DRAFT").FirstOrDefault();
            
            
            if (story != null)
            {
                var images = _cidatabase.StoryMedia.Where(m => m.StoryId == story.StoryId).ToList();
               
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 1024
                };
                var json = System.Text.Json.JsonSerializer.Serialize(story, options);


                return Json(json);
            }
            else
            {
                return Json("NoStoryFound");
            }
        }
        public IActionResult AddEditStory()
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            var storydata = _istory.GetStoryData();
            storydata.Applications = storydata.Applications.Where(a => a.UserId == userid && a.DeletedAt==null).ToList();
            //ViewData["Mis"] = _cidatabase.Missions.ToList();
            //ViewData["Missions"] = _cidatabase.MissionApplications.Where(m => m.UserId == userid && m.DeletedAt == null ).ToList();
            return View(storydata);
        }

        [HttpPost]
    
        public IActionResult AddEditStoryPost(long StoryId,long MissionId,string StoryDescription,DateTime StoryDate,string StoryTitle,List<IFormFile> StoryImages,string StoryVideoURL ,List<string> savedImages)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            var storydata = _istory.GetStoryData();
            var story = storydata.story.Where(s => s.MissionId == MissionId && s.UserId == userid && s.Status == "DRAFT").FirstOrDefault();
            if (story == null) {
                if (StoryId == 0)
                {
                    var status = _istory.AddStory(userid, MissionId, StoryDescription, StoryDate, StoryTitle, StoryImages, StoryVideoURL);
                }
                else
                {
                    var status = _istory.EditStory(StoryId,userid, MissionId, StoryDescription, StoryDate, StoryTitle, StoryImages, StoryVideoURL,savedImages);

                }
            }
            else
            {
                var status = _istory.EditStory(StoryId, userid, MissionId, StoryDescription, StoryDate, StoryTitle, StoryImages, StoryVideoURL, savedImages);
            }
            
            return View();
        }
        //To Show Stories
        public IActionResult StoryListPage(int pg = 1)
        {
          //  ViewData["Name"] = HttpContext.Session.GetString("Username");

            var storydata = _istory.GetStoryData();
         

            //To Pagination
            const int pageSize = 3;
            if (pg < 1)
                pg = 1;

            int recsCount = storydata.story.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = storydata.story.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            storydata.story = data.ToList();

            
            return View(storydata);
        }

        [HttpGet]
        public IActionResult ShareYourStory()
        {

            var userid = long.Parse(HttpContext.Session.GetString("UserID"));

           // ViewData["UserAvatar"] = _cidatabase.Users.Where(u => u.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).Select(u => u.Avatar).FirstOrDefault();

          //  ViewData["Name"] = HttpContext.Session.GetString("Username");
            
            ViewData["Mis"]=_cidatabase.Missions.ToList();  
            ViewData["Missions"] = _cidatabase.MissionApplications.Where(m=> m.UserId==userid && m.DeletedAt==null).ToList();
         
            return View();
        }

        //To Add StoryData 
        public IActionResult ShareYourStoryAdd(string missionId, string storyTitle, string storyDiscription, string storyDate, List<IFormFile> formFile,string storyUrl)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));

            var storyexists = _cidatabase.Stories.FirstOrDefault(s => s.MissionId == long.Parse(missionId) && s.UserId == userid && s.Status == "DRAFT");

            if (storyexists != null)
            {
                
                storyexists.MissionId = int.Parse(missionId);
                storyexists.UserId = userid;
                storyexists.Title = storyTitle;
                storyexists.Description = storyDiscription;
                storyexists.Status = "DRAFT";
                storyexists.UpdatedAt = DateTime.Now;
                
                _cidatabase.Update(storyexists);
                _cidatabase.SaveChanges();

                var storymedia = _cidatabase.StoryMedia.Where(m => m.StoryId == storyexists.StoryId).ToList();


                if (storymedia.Count() > 0)
                {
                    foreach (var item in storymedia)
                    {
                        _cidatabase.StoryMedia.Remove(item);
                        _cidatabase.SaveChanges();

                    }

                    if (formFile.Count() > 0)
                    {
                        foreach (var file in formFile)
                        {

                            FileStream FileStream = new(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads", Path.GetFileName(file.FileName)), FileMode.Create);

                            file.CopyToAsync(FileStream);
                            var ImageURL = "\\Uploads\\" + Path.GetFileName(file.FileName);
                            StoryMedium medias = new StoryMedium();

                            medias.StoryId = storyexists.StoryId;
                            medias.Path = ImageURL;
                            medias.Type = "IMG";

                            _cidatabase.StoryMedia.Add(medias);
                            _cidatabase.SaveChanges();
                            FileStream.Close();
                        }
                    }

                    

                    if (storyUrl != null)
                    {
                        Match regexMatch = Regex.Match(storyUrl, "^[^v]+v=(.{11}).*",
                                RegexOptions.IgnoreCase);

                        if (regexMatch.Success)
                        {
                            string embededurl = storyUrl.Replace("watch?v=", "embed/");

                            StoryMedium storyMedium = new StoryMedium();

                            storyMedium.StoryId = storyexists.StoryId;
                            storyMedium.Path = embededurl;
                            storyMedium.Type = "URL";

                            _cidatabase.StoryMedia.Add(storyMedium);
                            _cidatabase.SaveChanges();

                        }
                        
                    }
                }
                else
                {
                    foreach (var file in formFile)
                    {
                        FileStream FileStream = new(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads", Path.GetFileName(file.FileName)), FileMode.Create);
                        file.CopyToAsync(FileStream);
                        var ImageURL = "\\Uploads\\" + Path.GetFileName(file.FileName);
                       
                        StoryMedium medias = new StoryMedium();

                        medias.StoryId = storyexists.StoryId;
                        medias.Path = ImageURL;
                        medias.Type = "IMG";

                        _cidatabase.StoryMedia.Add(medias);
                        _cidatabase.SaveChanges();

                    }
                }

                return RedirectToAction("StoryListPage");

            }

            else
            {
                Story StoryData = new Story()
                {
                    PublishedAt = DateTime.Parse(storyDate),
                    MissionId = int.Parse(missionId),
                    UserId = userid,
                    Title = storyTitle,
                    Description = storyDiscription,
                    Status = "DRAFT"
                };

                _cidatabase.Stories.Add(StoryData);
                _cidatabase.SaveChanges();

                
                //To Story Media n Storymedia table
                var story = _cidatabase.Stories.Where(s => s.UserId == userid && s.MissionId == int.Parse(missionId) && s.Status=="DRAFT").OrderBy(s => s.StoryId).LastOrDefault();

                if (story != null)
                {
                    //to add videourl
                    if (storyUrl != null)
                    {
                        Match regexMatch = Regex.Match(storyUrl, "^[^v]+v=(.{11}).*",
                                RegexOptions.IgnoreCase);

                        if (regexMatch.Success)
                        {




                            StoryMedium storyMedium = new StoryMedium();

                            storyMedium.StoryId = story.StoryId;
                            storyMedium.Path = storyUrl;
                            storyMedium.Type = "URL";

                            _cidatabase.StoryMedia.Add(storyMedium);
                            _cidatabase.SaveChanges();
                        }

                    }

                    if (formFile.Count() > 0)
                    {
                        //to add photos
                        string fileName;

                        FileStream stream;

                        string uploadpath;


                        foreach (var file in formFile)
                        {
                            fileName = Path.GetFileName(file.FileName);

                            uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads", fileName);

                            stream = new FileStream(uploadpath, FileMode.Create);

                            file.CopyToAsync(stream);



                            var ImageURL = "\\Uploads\\" + fileName;

                            StoryMedium storyMedium = new StoryMedium();
                            storyMedium.StoryId = story.StoryId;
                            storyMedium.Path = ImageURL;
                            storyMedium.Type = "IMG";

                            _cidatabase.StoryMedia.Add(storyMedium);
                            _cidatabase.SaveChanges();

                        }
                    }
                }
              

                

                return RedirectToAction("StoryListPage");
            }

            

        }


        public IActionResult StoryDetails(int storyid)
        {
            var story = _cidatabase.Stories.Where(s=>s.StoryId==storyid).FirstOrDefault();
            if (story != null)
            {
                story.Views = story.Views + 1;
                _cidatabase.Update(story);
                _cidatabase.SaveChanges();
            }
           
            var storydata = new StoryData
            {
                story = _cidatabase.Stories.Where(s=>s.StoryId==storyid).ToList(),
                storyMedia = _cidatabase.StoryMedia.Where(s => s.StoryId == storyid).ToList(),
                user = _cidatabase.Users.ToList(),
                mission=_cidatabase.Missions.ToList()
            };
            ViewData["UserAvatar"] = storydata.user.Where(u => u.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).Select(u => u.Avatar).FirstOrDefault();



            return View(storydata);
        }

        public JsonResult GetDraftStory(int missionid)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));

            var Story = _cidatabase.Stories.Where(s => s.UserId == userid && s.MissionId == missionid && s.Status == "DRAFT").FirstOrDefault();
            if (Story == null)
            {
                RedirectToAction("ShareYourStoryAdd");
                var draftDetails = new { title = "", description = "", date = ""};
                return Json(draftDetails);

            }
            else
            {
                var storyMedia = _cidatabase.StoryMedia.Where(m => m.StoryId == Story.StoryId).ToList();
                
                
                IEnumerable<string> paths = storyMedia.Select(m => m.Path).ToList();

                var draftDetails = new { title = Story.Title, description = Story.Description, path = paths, date = Story.PublishedAt };
                return Json(draftDetails);
            }



        }

        public RedirectResult Recommend(int StoryId, int to_userid)
        {
            var to_useremail = _cidatabase.Users.Where(u => u.UserId == to_userid).Select(u => u.Email).SingleOrDefault();

            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            if (to_useremail != null)
            {
            

                var mailbody = "<h1>Watch Story</h1><br><h2><a href='" + "https://localhost:7037/Story/StoryDetails?storyid=" + StoryId + "'></a></h2>";
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("ciplatformmailsender@gmail.com"));
                email.To.Add(MailboxAddress.Parse(to_useremail));
                email.Subject = "Story Invite";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailbody };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate("ciplatformmailsender@gmail.com", "muarmclnmmtdzxqh");
                smtp.Send(email);
                smtp.Disconnect(true);


              StoryInvite storyInvite = new StoryInvite();
                storyInvite.StoryId = StoryId;
                storyInvite.CreatedAt = DateTime.Now;
                storyInvite.FromUserId = userid;
                storyInvite.ToUserId = to_userid;


                _cidatabase.Add(storyInvite);
                _cidatabase.SaveChanges();



            }


            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }

    }
}
