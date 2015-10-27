using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EHTool.Core.Control
{
    public class TextBlock:Label
    {
        public int MaxLine { get; set; } = 100;
    }
}
