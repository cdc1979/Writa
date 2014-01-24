using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Writa.Models.Stats
{
    public class WritaStats
    {
        public int NumberOfPosts { get; set; }
        public int NumberOfStaticPages { get; set; }
        public string ActiveTheme { get; set; }
        public DateTime LastPostDate { get; set; }

    }
}
