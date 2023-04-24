using CIPlatformMain.Entities.Models;
using CIPlatformMain.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIPlatformMain.Repository.Interface
{
    public interface IStory
    {
        StoryData GetStoryData();

    }
}
