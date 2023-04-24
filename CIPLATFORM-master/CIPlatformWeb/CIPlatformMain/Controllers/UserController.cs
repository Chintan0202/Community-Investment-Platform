using CIPlatformMain.Entities.Data;
using CIPlatformMain.Entities.Models;
using CIPlatformMain.Entities.ViewModel;

using CIPlatformMain.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CIPlatformMain.Controllers
{
    public class UserController : Controller
    {

        private readonly CidatabaseContext _cidatabase;
        private readonly IUser _iuser;


        public UserController(CidatabaseContext cidatabase, IUser iuser)
        {
            _iuser = iuser;
            _cidatabase = cidatabase;
        }

        [HttpGet]
        public IActionResult EditProfile(int Country_id)
        {
           // ViewData["Name"] = _cidatabase.Users.Where(u => u.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).Select(u => u.FirstName).FirstOrDefault();
           // ViewData["UserAvatar"] = _cidatabase.Users.Where(u => u.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).Select(u => u.Avatar).FirstOrDefault();

            var userid = long.Parse(HttpContext.Session.GetString("UserID"));

            UserData data = _iuser.GetUserData(userid);
            if (Country_id!=null)
            {
                data.City = data.City.Where(c=>c.CountryId==Country_id).ToList();
            }
            return View(data);
        }
        
        public JsonResult GetCities(int Country_id)
        {
            try
            {
                var cities = _iuser.GetCities(Country_id);
                return Json(cities);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
               
        }

        [HttpPost]
        public IActionResult EditProfile(UserData _user, int cityid, int countryid, IFormFile Avatar)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            if (userid != null)
            {
                int status = _iuser.UpdateUser(_user, cityid, countryid, userid ,Avatar);
            }

            return RedirectToAction("EditProfile");
        }

        public IActionResult chnagepassword(string NewPassword, string ConformPassword, string oldpassword)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            if (userid != null)
            {
                int status = _iuser.UpdatePassword(NewPassword, ConformPassword, oldpassword, userid);
               
            }

            return RedirectToAction("EditProfile");


        }

        public IActionResult volunteeringTimesheet()
        {
            ViewData["Name"] = _cidatabase.Users.Where(u => u.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).Select(u => u.FirstName).FirstOrDefault();
            ViewData["UserAvatar"] = _cidatabase.Users.Where(u => u.UserId == long.Parse(HttpContext.Session.GetString("UserID"))).Select(u => u.Avatar).FirstOrDefault();
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            
            VolunteeringTimesheet TimesheetData = _iuser.GetVolunteeringTimesheet(userid);

            return View(TimesheetData);
        }

        public JsonResult GetMissionDate(long MissionId)
        {
            var mission = _iuser.GetMission(MissionId);
           
            var Dates = new { startdate=mission.StartDate,enddate = mission.EndDate};
            return Json(Dates);
        }

        public IActionResult AddGoalTimesheet(long MissionId,int Action, DateTime DateVolunteered,string Message,long TimesheetId)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            var result = false;
            if (userid != null)
            {
                if (TimesheetId == 0)
                {
                    result = _iuser.AddGoalTimesheet(MissionId, Action, DateVolunteered, Message, userid);
                }
                else
                {
                    result = _iuser.EditGoalTimesheet( Action, DateVolunteered, Message, TimesheetId);
                }
                
            }
            if (result != false)
            {
                TempData["result"] = 1;
            }
            else
            {
                TempData["result"] = null;
            }
           
            return RedirectToAction("volunteeringTimesheet");
        }
        public IActionResult AddTimeTimesheet(long MissionId, DateTime DateVolunteered,string Message,string Hours,string Minutes,long TimesheetId)
        {
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
            var result = false;
            if (userid != null)
            {
                

                if (TimesheetId == 0)
                {
                    result = _iuser.AddTimeTimesheet(MissionId, DateVolunteered, Message, Hours, Minutes, userid);
                }
                else
                {
                    result = _iuser.EditTimeTimesheet(DateVolunteered, Message, Hours, Minutes, TimesheetId);
                }
            }
           


            if (result != false)
            {
                TempData["result"] = 1;
            }
            else
            {
                TempData["result"] = null;
            }

            return RedirectToAction("volunteeringTimesheet");
            
        }
        public IActionResult DeleteTimesheet(long TimesheetId)
        {
            if (TimesheetId!=null){

                var userid = long.Parse(HttpContext.Session.GetString("UserID"));
                var DeleteStatus = false;
                if (userid != null)
                {
                    DeleteStatus = _iuser.DeleteTimeSheet(TimesheetId);
                }
                if (DeleteStatus != false)
                {
                    TempData["DeleteStatus"] = 1;
                }
                else
                {
                    TempData["DeleteStatus"] = null;
                }
                return RedirectToAction("volunteeringTimesheet");

            }
            else
            {
                TempData["error"] = "Not Deleted";
                return RedirectToAction("volunteeringTimesheet");
            }

           
        }

        public IActionResult GetTimesheetData(long TimesheetId)
        {
            var Timesheet=_iuser.GetTimesheetData(TimesheetId);
       
            return Json(Timesheet);
        }

        public IActionResult Updateskills(long[] Userskills)
        {
            
            var userid = long.Parse(HttpContext.Session.GetString("UserID"));
          
            if (userid != null)
            {
                 var result= _iuser.UpdateSkills(Userskills, userid);
            }
           
            return RedirectToAction("EditProfile");
        }

    }
}
