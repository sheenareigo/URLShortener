using Shortly.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Services
{
    public interface IUrlService 
    {
        Task<List<Url>> GetUrls();

        Task<List<Url>> GetUrlbyUser(string userid);
        Task< Url >GetUrlbyId(int id);

       Task <Url> AddUrl(Url url);

        Task<Url> UpdateUrl(int id, Url url);

        Task<bool> DeleteUrl(int id);

       Task< Url > GetOriginalUrl(string shortUrl);

       Task <bool> Incrementclicks(int shortUrlid);

    }
}
