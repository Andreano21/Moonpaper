using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonpaper.Models
{
    public class User : IdentityUser
    {
        public List<UserTag> UserTags { get; set; }

        public User()
        {
            UserTags = new List<UserTag>();
        }
    }
}
