using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebManage.Models
{
    public class TemplateMenuModel
    {
        public List<TeamplateMeunButton> button { get; set; }

    }
    public class TeamplateMeunButton {
        public string name { get; set; }

        public string type { get; set; }

        public string key { get; set; }

        public List<TeamplateMeunChildButton> sub_button { get; set; }
    }

    public class TeamplateMeunChildButton {
        public string name { get; set; }

        public string type { get; set; }

        public string key { get; set; }

        public string url { get; set; }

        public string pagepath { get; set; }

        public List<TeamplateMeunChildButton> sub_button { get; set; }
    }
}
