using Microsoft.AspNetCore.Mvc;
using Shortly.Client.Data.ViewModels;
using Shortly.Data.Models;
using Shortly.Data;
using System;
using System.Linq;
using System.Security.Claims;

namespace Shortly.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var newURL = new PostUrlVM();
            return View(newURL);
        }

        [HttpPost]
        public IActionResult ShortenURL(PostUrlVM postURL) { 
            var user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user_id == null)
            {
                TempData["NotRegistered"] = "Log into the application to continue using Shortly";
                return RedirectToAction("Index");


            }
            if (!ModelState.IsValid)
            {
                return View("Index", postURL);
            }

            var newUrl = new Url()
            {
                OriginalLink = postURL.Url,
                ShortLink = GenerateShortLink(6),
                DateCreated = DateTime.UtcNow,
                UserId = user_id
            };

            _dbContext.Urls.Add(newUrl);
            _dbContext.SaveChanges();

            //tempdata
            TempData["SuccessMessage"] = $"Your URL has been shortened to <a href='https://localhost:7007/{newUrl.ShortLink}' target='_blank'>{newUrl.ShortLink}</a>";

            // TempData["SuccessMessage"] = "Your url has been shortened to https://localhost:7007/" + newUrl.ShortLink;
            return RedirectToAction("Index");
        }

        private string GenerateShortLink(int urllength)
        {
            var random = new Random();
            const string shortchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var shorturl = new string(Enumerable.Repeat(shortchars, urllength)
                            .Select(s => s[random.Next(shortchars.Length)]).ToArray());

            return shorturl;
        }
    }
}
