using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFU.Models
{
    public class WebHook
    {
        public WebHook()
        {
            Files = new List<WebHookFile>();
        }

        public bool Success { get; set; }
        public string Directory { get; set; }
        public List<WebHookFile> Files { get; set; }
    }
}
