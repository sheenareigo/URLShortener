using Microsoft.EntityFrameworkCore;
using Shortly.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Services
{
    public class UrlService : IUrlService
    {
        private AppDbContext _context;

        public UrlService(AppDbContext context)
        {

            _context = context;

        }

        public async Task<List<Url>> GetUrlbyUser(string userid)
        {
            var allurls = await _context.Urls.Include(y => y.User).Where(n => n.User.Id == userid).ToListAsync(); ;
            return allurls;

        }

        public async Task< List<Url> > GetUrls()
        {
            var allurls = await _context.Urls.Include(y => y.User).ToListAsync();
            return allurls;
        
        }

        public async Task<Url> GetUrlbyId(int id)
        {

            var url = await _context.Urls.FirstOrDefaultAsync(x => x.Id == id);
            return url;
        }

        public async Task<Url> AddUrl(Url url)
        {

            await _context.Urls.AddAsync(url);
            await _context.SaveChangesAsync();
            return url;
        
        }

        public async Task<Url> UpdateUrl(int id, Url url)
        {

            var urldb = await _context.Urls.FirstOrDefaultAsync(x => x.Id == id);
            if (urldb != null)
            {
                urldb.ShortLink = url.ShortLink;
                urldb.OriginalLink = url.OriginalLink;
                urldb.DateUpdated = DateTime.UtcNow;
                _context.SaveChanges();
            
            }
            return urldb;

        }

        public async Task<bool> DeleteUrl(int id )
        {

            var urldb = await _context.Urls.FirstOrDefaultAsync(x => x.Id == id);
            bool del = false;
        
            if (urldb != null)
            {
                del = true;
                _context.Urls.Remove(urldb);

                // Save changes asynchronously
                await _context.SaveChangesAsync();
                

            }
            return del;
           

        }

        public async Task<Url> GetOriginalUrl(string shorturl)
        {

            var original = await _context.Urls.FirstOrDefaultAsync(x => x.ShortLink == shorturl);
            return original;

        }

        public async Task<bool> Incrementclicks(int shortUrlid)
        {

           var url= await _context.Urls.FirstOrDefaultAsync(x=>x.Id==shortUrlid);
            url.NoOfClicks++;
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
