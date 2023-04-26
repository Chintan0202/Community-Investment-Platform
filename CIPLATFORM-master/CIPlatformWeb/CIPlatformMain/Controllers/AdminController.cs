using CIPlatformMain.Entities.Models;
using CIPlatformMain.Entities.ViewModel;
using CIPlatformMain.Models;
using CIPlatformMain.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;
using System.Diagnostics;

namespace CIPlatformMain.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdmin _iadmin;
        public AdminController(IAdmin iadmin)
        {
            _iadmin = iadmin;

        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(Admin admin)
        {

            var result = _iadmin.AdminLogin(admin);
            if (result == true)
            {
                return RedirectToAction("AdminPage");
            }
            
            else
            {
                ViewBag.Logincredentials = 0;
                return View(admin);
            }

        }

        public IActionResult AdminPage()
        {
          
           
                var Data = _iadmin.GetData();
            return View(Data);
            
        }

        public IActionResult UserData()
        {
            var Data = _iadmin.GetData();
            if (TempData["CMSPageRedirect"] != null)
            {
                return PartialView("_CMSPage", Data);
            }
            else
            {
                return PartialView("_User", Data);
            }
           

            
        }

        public IActionResult CMSList()
        {
            var Data = _iadmin.GetData();
            return PartialView("_CMSPage", Data);
        }
        public IActionResult AddEditCMSPage(long CMSId)
        {
            var Data = _iadmin.GetData();
            var CMSData=Data.CmsPage;
            if (CMSId != null)
            {
                CMSData = Data.cmsPages.Where(c => c.CmsPageId == CMSId).FirstOrDefault();
            }
            
            return PartialView("_AddEditCMSPage", CMSData);
        }
       
        [HttpPost]
        public IActionResult AddEditCMSPage(CmsPage cmsPage)
        {
           
                if (cmsPage.CmsPageId !=0)
                {
                    _iadmin.EditCMSPage(cmsPage, cmsPage.CmsPageId);
                }
                else
                {
                    _iadmin.AddCMSPage(cmsPage);
                }


            
            TempData["CMSPageRedirect"] = "CMSRedirect";
            return RedirectToAction("AdminPage");


        }

        public IActionResult DeleteCMSPage(long CMSId)
        {
            var status = _iadmin.DeleteCMSPage(CMSId);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }
        }

        public IActionResult MissionApplicationList()
        {
            var Data = _iadmin.GetData();
            return PartialView("_MissionApplicationList", Data);
        }

        public IActionResult MissionThemeList()
        {
            var Data = _iadmin.GetData();
            return PartialView("_MissionTheme", Data);
        }

        public IActionResult MissionSkillList()
        {
            var Data = _iadmin.GetData();
            return PartialView("_MissionSkills", Data);
        }
       
       public IActionResult DraftSkill(long SKillId)
        {
            var skill=_iadmin.GetSkill(SKillId);
            return Json(skill);
        }

        public IActionResult EditSkill(long SkillId,string SkillName,int SkillStatus)
        {
            var status=_iadmin.EditSkill(SkillId, SkillName, SkillStatus);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }
        }
        public IActionResult DeleteSkill(long SkillId)
        {
            var status=_iadmin.DeleteSkill(SkillId);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }
        }
        
        public IActionResult StoryList()
        {
            var Data = _iadmin.GetData();
            return PartialView("_StoryList", Data);
        }
      
        public IActionResult StoryDetails(long StoryId)
        {
            var Data = _iadmin.GetData();
            Data.Story=Data.Stories.Where(s=>s.StoryId==StoryId).FirstOrDefault();
            Data.storyMedia=Data.AllstoryMedia.Where(m=>m.StoryId==StoryId).ToList();
            return PartialView("_viewStory", Data);
        }
        

        public IActionResult StoryDecline(long StoryId)
        {
           var status=_iadmin.StoryDecline(StoryId);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }
            
        }
        public IActionResult StoryApprove(long StoryId)
        {
            var status=_iadmin.StoryApprove(StoryId);
            if (status == true)
            {
                TempData["ApproveStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["ApproveStatus"] = null;
                return RedirectToAction("AdminPage");
            }
            
        }
        public IActionResult StoryDelete(long StoryId) {

            var status=_iadmin.StoryDelete(StoryId);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }
            
        }
        public IActionResult MissionList()
        {
            var Data = _iadmin.GetData();
            return PartialView("_MissionList", Data);
        }

        public IActionResult DeleteMission(long MissionId)
        {
            var status=_iadmin.DeleteMission(MissionId);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }
        }
        
        public IActionResult AddEditMission(long MissionId)
        {
            AddMission addMission = _iadmin.getMissionData();
            if (MissionId != 0)
            {
                addMission.Mission=addMission.Missions.Where(m=>m.MissionId==MissionId).FirstOrDefault();
                addMission.MissionMediums = addMission.MissionMediums.Where(m => m.MissionId == MissionId).ToList();
                addMission.MissionsDocuments = addMission.MissionsDocuments.Where(m => m.MissionId == MissionId).ToList();
                addMission.MissionSkills = addMission.MissionSkills.Where(m => m.MissionId == MissionId).ToList();
            }
            
            return PartialView("_AddEditMission", addMission);
        }
        [HttpPost]
        public IActionResult AddEditMission(Mission mission,List<IFormFile> MissionDocument, List<IFormFile> MissionPhotos, IFormFile DefualtMissionPhotos, String MissionVideoURL,List<int> SkillList)
        {

            if (mission.MissionId != 0)
            {
                var status=_iadmin.EditMission(mission, MissionPhotos, DefualtMissionPhotos, MissionDocument, MissionVideoURL, SkillList);
            }
            else
            {


                var status = _iadmin.AddMission(mission, MissionPhotos, DefualtMissionPhotos, MissionDocument, MissionVideoURL, SkillList);
                TempData["MissionListRedirect"] = 1;
            }
                return RedirectToAction("AdminPage");
        }


        public IActionResult DraftTheme(long MissionThemeId)
        {

            if (MissionThemeId!=null)
            {
                MissionTheme missionTheme = _iadmin.GetMissionTheme(MissionThemeId);
                return Json(missionTheme);
            }
            else
            {
                return Json("Add");
            }
          
        }
        
        public IActionResult AddEditTheme(string ThemeName, int Status, long ThemeId)
        {
            if (ThemeId == 0)
            {
                var status=_iadmin.AddTheme(ThemeName, Status);
                if (status == true)
                {
                    TempData["Addtheme"] = 1;
                }
                else
                {
                    TempData["Addtheme"] = null;
                }

            }
            else
            {
                var status = _iadmin.EditTheme(ThemeName, Status, ThemeId);
                if (status == true)
                {
                    TempData["Edittheme"] = 1;
                }
                else
                {
                    TempData["Edittheme"] = null;
                }
                
            }
            return RedirectToAction("AdminPage");
        }
        public IActionResult DeleteTheme(long MissionThemeId)
        {
            var status = _iadmin.DeleteTheme(MissionThemeId);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }
        }
        public IActionResult AddUser()
        {

            var Data = _iadmin.GetData();
            ViewData["countries"] = Data.Country;
            ViewData["cities"] = Data.City;
            return View();
        }

        [HttpPost]
        public IActionResult AddUserData(User _user,IFormFile Avatar)
        {
            var emailstatus= _iadmin.CheckEmailInDatabase(_user);
            if (emailstatus == 1)
            {
                
                   
                TempData["EmailStatus"] = null;
                
                
            }
            else
            {
                TempData["EmailStatus"] = 1 ;
            }

            
            return RedirectToAction("AdminPage");
        }

        public IActionResult AddEditUserPage(long UserId)
        {
            var Data = _iadmin.GetData();
           
            if (UserId != null)
            {
                Data.User = Data.Users.Where(c => c.UserId == UserId).FirstOrDefault();
            }

            return PartialView("_AddEditUserPage", Data);
        }

        [HttpPost]
        public IActionResult AddEditUserPage(User user,IFormFile Avatar)
        {

            
            if (user.UserId != 0)
            {
                _iadmin.EditUser(user,user.UserId,Avatar);
            }
            else
            {
                var emailstatus = _iadmin.CheckEmailInDatabase(user);
                if (emailstatus == 1)
                {
                    _iadmin.AddUser(user, Avatar);
                    TempData["EmailStatus"] = null;
                }
                else
                {
                    TempData["EmailStatus"] = 1;
                }
                
            }
            

           
            return RedirectToAction("AdminPage");


        }
        public IActionResult DeleteUser(long UserId)
        {
            var DeleteStatus=_iadmin.DeleteUser(UserId);
            if(DeleteStatus == true)
            {
                TempData["DeleteStatus"] = 1;
            }
            else
            {
                TempData["DeleteStatus"] = null;
            }
            return RedirectToAction("AdminPage");
        }

        public IActionResult ApproveMissionApplication(long MissionId,long UserId)
        {

            var status = _iadmin.ApproveMissionApplication( MissionId,UserId);
            if(status == true)
            {
                TempData["MissionApplicationStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["MissionApplicationStatus"] = null;
                return RedirectToAction("AdminPage");
            }
            
        }
        public IActionResult DeclineMissionApplication(long MissionId, long UserId)
        {

            var status = _iadmin.ApproveMissionApplication(MissionId, UserId);
            if (status == true)
            {
                TempData["DeleteStatus"] = 1;
                return RedirectToAction("AdminPage");
            }
            else
            {
                TempData["DeleteStatus"] = null;
                return RedirectToAction("AdminPage");
            }

        }


       public IActionResult GetBannerList()
        {
            var Data = _iadmin.GetData();
            return PartialView("_BannerManagment", Data);
        }
        public JsonResult DraftBanner(long BannerId)
        {
            try
            {
                if (BannerId != null)
                {
                    Banner banner = _iadmin.GetBanner(BannerId);
                    if (banner != null)
                    {
                        return Json(banner);
                    }
                    else
                    {
                        return Json("Not Found");
                    }
                }
                else
                {
                    return Json("NotFound");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Processing failed: {ex.Message}");
                return Json("NotFound");
            }
        }

        public IActionResult AddEditBanner(Banner BannerData, IFormFile BannerImage)
        {
            if (BannerData.BannerId != 0)
            {
                var BannerId = BannerData.BannerId;
                var status=_iadmin.EditBanner(BannerId,BannerData, BannerImage);
                if (status == true)
                {

                    TempData["BannerManagmentRediect"] = 1;
                    return RedirectToAction("AdminPage");

                }
                else
                {
                    TempData["BannerManagmentRediect"] = null;
                    return RedirectToAction("AdminPage");
                }
            }
            else
            {

                var status = _iadmin.AddBanner(BannerData, BannerImage);
                if (status == true)
                {

                    TempData["BannerManagmentRediect"] = 1;
                    return RedirectToAction("AdminPage");

                }
                else
                {
                    TempData["BannerManagmentRediect"] = null;
                    return RedirectToAction("AdminPage");
                }
            }

          
        }
    }
}
