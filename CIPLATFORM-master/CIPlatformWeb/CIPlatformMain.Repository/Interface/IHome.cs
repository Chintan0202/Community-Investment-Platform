﻿using CIPlatformMain.Entities.Models;
using CIPlatformMain.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformMain.Repository.Interface
{
    public interface IHome
    {
        IEnumerable<MissionRating> GetRatings();

         Missiondata GetMissions(long UserId);

         MissionDetails GetMissionDetails(int missionid);

        void Recommend(int missionid,long from_userid, int to_userid);

        bool Contact_us(long user_id, string Subject, string Message);

        bool MissionApplication(long MissionId,long UserId);


    }
}
