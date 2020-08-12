using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Optimal_Route_Calculator
{
    abstract class LineObject
    {
        protected double[] line_pos = new double[4];
        protected bool visible = true;

        public virtual double[] LinePos
        {
            get { return line_pos; }
            set { line_pos = value; }
        }

        public virtual bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
    }
}
