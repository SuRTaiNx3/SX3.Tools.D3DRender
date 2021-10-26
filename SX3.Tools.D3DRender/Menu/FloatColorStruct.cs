using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SX3.Tools.D3DRender.Menu
{
    public struct FloatColorStruct
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public FloatColorStruct(float r, float g, float b, float a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
    }
}
