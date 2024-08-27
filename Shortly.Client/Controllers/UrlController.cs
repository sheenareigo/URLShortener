using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shortly.Client.Data.ViewModels;
using Shortly.Client.Helpers.Roles;
using Shortly.Data;
using Shortly.Data.Models;
using Shortly.Data.Services;
using System.Security.Claims;

namespace Shortly.Client.Controllers
{
    //[Route("Url")]
    public class UrlController : Controller
    {
        public IUrlService _urlService;
        private readonly IMapper mapper;
        private UserManager<User> _userManager;
        private IUserService _userService;

        public UrlController(IUrlService urlService, IMapper _mapper, IUserService userService, UserManager<User> userManager)
        {
            _urlService = urlService;
             mapper = _mapper;
            _userService = userService;
            _userManager = userManager;

        }
        public async Task< IActionResult> Index()
        {
            var alldata = new List<Url>();
           var user_id =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole(Role.Admin);
            if (isAdmin)
            {
                alldata = await _urlService.GetUrls();
            }
            else
            {
                alldata = await _urlService.GetUrlbyUser(user_id);
            }
            //Include(y => y.User).
            //Select(url => new GetUrlVM()
            //{
            //    Id = url.Id,
            //    OriginalLink = url.OriginalLink,
            //    ShortLink = url.ShortLink,
            //    NoOfClicks = url.NoOfClicks,
            //    UserId = url.UserId,

            //    UserVM = url.User!=null ? new GetUserVM() {

            //        Id = url.User.Id,
            //        Name = url.User.Name
            //    } :null


            //}).ToList();

            var mappedUrls = mapper.Map<List<Url>,List<GetUrlVM>>(alldata);
            //var alldata = new List<GetUrlVM>() {

            //    new GetUrlVM()
            //    {
            //    Id=1,
            //    OriginalLink="abc",
            //    ShortLink="def",
            //    NoOfClicks=1,
            //    UserId=90,

            //    },
            //     new GetUrlVM()
            //    {
            //    Id=2,
            //    OriginalLink="xyz",
            //    ShortLink="rst",
            //    NoOfClicks=1,
            //    UserId=91,

            //    }
            //};
           
            
            //ViewData["AllUrls"] = new List<string>() {"Url1","Url2","Url3" };
            //ViewData["ShortenedURL"] = "http://link1";
            //ViewBag.AllUrls = new List<string>() { "Url1", "Url2", "Url3" };
            //ViewBag.ShortendUrl = "http://link1";
            //if (TempData["Sucess"] != null)
            //{
            //    ViewBag.Successmessage = TempData["Sucess"].ToString();
            //}
            //var url = new Url() { 
            //Id=1,
            //OriginalLink="ABC",
            //ShortLink="a",
            //NoOfClicks=2,
            //UserId=56,
            ////DateCreated=Convert.ToDateTime("24-04-1995:00-00-00"),
            ////DateUpdated= Convert.ToDateTime("24-04-1995")
            //};
            //var listlinks = new List<Url>();
            //listlinks.Add(url);
            return View(mappedUrls);
        }
        public IActionResult Create()
        {
            //var shortURl = "short";
            //TempData["Sucess"] = "Success";
            //ViewBag.ShortenedURL = "short";
            //ViewData["ShortenedURL"] = "short";
            return RedirectToAction("Index");
        }

        //[HttpPost("Remove/user/{userid}/link/{linkid}")]


        //public IActionResult Remove(int userid, int linkid)
        //{
        //    return View();

        //}


        public async Task<IActionResult> Remove(int id)
        {
            // Attempt to delete the URL using the service
            var urldel = await _urlService.DeleteUrl(id);

            // Check if the deletion was successful
            if (urldel)
            {
                TempData["RemoveSuccess"] = "The short link has been removed successfully.";
            }
            else
            {
                TempData["RemoveError"] = "The short link could not be found or was not removed.";
            }

            // Redirect to the Index action
            return RedirectToAction("Index");
        }


    }
}
