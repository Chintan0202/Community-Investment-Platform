using CIPlatformMain.Entities.Models;
using CIPlatformMain.Entities.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformMain.Repository.Interface
{
    public interface IAdmin
    {
        bool AdminLogin(Admin admin);

        AdminData GetData();

        bool DeleteUser(long UserId);



        UserData GetUser(long UserId);


        int CheckEmailInDatabase(User _user);
        void AddUser(User user, IFormFile Avatar);

        void EditUser(User user, long UserId,IFormFile Avatar);

        bool ApproveMissionApplication(long MissionId, long UserId);

        bool DeclineMissionApplication(long MissionId, long UserId);

        bool StoryDecline(long StoryId);
        bool StoryApprove(long StoryId);
        bool StoryDelete(long StoryId);

        AddMission getMissionData();

        bool DeleteMission(long MissionId);

        bool DeleteSkill(long SkillId);

        bool EditSkill(long SkillId,string SkillName,int SkillStatus);

        Skill GetSkill(long SkillId);

        void AddCMSPage(CmsPage cmsPage);

        void EditCMSPage(CmsPage cmsPage,long cmsId);

        bool DeleteCMSPage(long CMSPageId);

        bool AddMission(Mission mission, List<IFormFile> Images,IFormFile DefaultImage, List<IFormFile> Documents, string MissionVideoURL, List<int> SkillList);

        bool EditMission(Mission mission, List<IFormFile> Images, IFormFile DefaultImage, List<IFormFile> Documents, string MissionVideoURL, List<int> SkillList);


        MissionTheme GetMissionTheme(long MissionThemeId);
        bool AddTheme(string ThemeName, int Status);

        bool EditTheme(string ThemeName, int Status, long ThemeId);

    }
}
