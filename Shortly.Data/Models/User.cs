using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Models
{
    public class User : IdentityUser
    {
        

        public List<Url> Urls { get; set; }

        public User()
        {
            Urls = new List<Url>();
        }
    }
}

