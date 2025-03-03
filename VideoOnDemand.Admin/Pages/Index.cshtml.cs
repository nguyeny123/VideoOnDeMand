﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using VideoOnDemand.Admin.Models;
using VideoOnDemand.Data.Services;

namespace VideoOnDemand.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private IDbReadService _db;
        public (CardViewModel instructors, CardViewModel users,
        CardViewModel courses, CardViewModel modules,
        CardViewModel videos, CardViewModel downloads,
        CardViewModel userCourses) Cards;
        public IndexModel(IDbReadService db)
        {
            _db = db;
        }
        public void OnGet()
        {
            var count = _db.Count();
            Cards = (
            instructors: new CardViewModel
            {
                BackgroundColor = "#9c27b0",
                Count = count.instructors,
                Description = "Instructors",
                Icon = "user",
                Url = "./Instructors/Index"
            },
            users: new CardViewModel
            {
                BackgroundColor = "#414141",
                Count = count.users,
                Description = "Users",
                Icon = "graduation-cap",
                Url = "./Users/Index"
            },
            courses: new CardViewModel
            {
                BackgroundColor = "#009688",
                Count = count.courses,
                Description = "Courses",
                Icon = "chalkboard",
                Url = "./Courses/Index"
            },
            modules: new CardViewModel
            {
                BackgroundColor = "#f44336",
                Count = count.modules,
                Description = "Modules",
                Icon = "list",
                Url = "./Modules/Index"
            },
            videos: new CardViewModel
            {
                BackgroundColor = "#3f51b5",
                Count = count.videos,
                Description = "Videos",
                Icon = "film",
                Url = "./Videos/Index"
            },
            downloads: new CardViewModel
            {
                BackgroundColor = "#ffcc00",
                Count = count.downloads,
                Description = "Downloads",
                Icon = "file",
                Url = "./Downloads/Index"
            },
            userCourses: new CardViewModel
            {
                BackgroundColor = "#176c37",
                Count = count.userCourses,
                Description = "User Courses",
                Icon = "file",
                Url = "./UserCourses/Index"
            }
            );
        }
    }
}
