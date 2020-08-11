using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoonpaperLinux.Models
{
    public class UserTag
    {
        public int Rating { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
