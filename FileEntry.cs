using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free_MS_Store_Apps
{
    internal class FileEntry
    {
        public string packagefilename { get; set; }
        public string packagedownloadurl { get; set; }
        public long packagefilesize { get; set; } 
        public string packagemoniker { get; set; }
    }
}
