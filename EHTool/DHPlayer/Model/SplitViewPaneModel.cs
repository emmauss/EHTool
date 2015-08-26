using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHTool.DHPlayer.Model
{
    public class SplitViewPaneModel
    {
        public SplitViewPaneModel(string icon, string name)
        {
            Icon = icon;
            Name = name;
        }
        public string Icon { get; set; }
        public string Name { get; set; }
    }
}
