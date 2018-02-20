using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NBodySim2
{
    public class Points
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public SolidColorBrush clr { get; set; }


        public Points()
        {
            width = 8;
            height = 8;
        }
    }
}
