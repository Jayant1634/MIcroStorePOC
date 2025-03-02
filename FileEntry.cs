using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free_MS_Store_Apps
{
    internal class FileEntry
    {
        public string File { get; set; }
        public string Expire { get; set; }
        public string SHA1 { get; set; }
        public string DownloadUrl { get; set; }
        public string Size { get; set; }
    }
}
