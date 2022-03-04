using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimal_Route_Calculator
{
    class GridNode
    {
        public double F_Cost { get; set; }
        public double G_Cost { get; set; }
        public double H_Cost { get; set; }
        public int Parent { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
