using System.Collections.Generic;

namespace Nfu.Models
{
    public class WebHook
    {
        public WebHook()
        {
            Files = new List<WebHookFile>();
        }

        public bool Success { get; set; }
        public string Secret { get; set; }
        public string Directory { get; set; }
        public List<WebHookFile> Files { get; set; }
    }
}
