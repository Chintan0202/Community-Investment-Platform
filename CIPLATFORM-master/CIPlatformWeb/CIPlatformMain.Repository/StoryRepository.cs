using CIPlatformMain.Entities.Data;
using CIPlatformMain.Entities.Models;
using CIPlatformMain.Entities.ViewModel;
using CIPlatformMain.Repository.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CIPlatformMain.Repository
{
    public class StoryRepository : IStory
    {
        private readonly CidatabaseContext _cidatabase;

        public StoryRepository()
        {
            _cidatabase = new CidatabaseContext();
        }

        public StoryData GetStoryData()
        {
            var storydata = new StoryData
            {
                story = _cidatabase.Stories.ToList(),
                storyMedia = _cidatabase.StoryMedia.ToList(),
                user = _cidatabase.Users.ToList(),
                mission=_cidatabase.Missions.ToList(),
                Theme=_cidatabase.MissionThemes.ToList(),

            };

            return storydata;
}

    }
}
