using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoonpaperLinux.Models
{
    public class UserSource
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
        public int SourceId { get; set; }
    }
}
