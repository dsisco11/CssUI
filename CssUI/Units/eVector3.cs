using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public class eVector3
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        #region Constructors
        public eVector3()
        {
        }

        public eVector3(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        #endregion


    }
}
