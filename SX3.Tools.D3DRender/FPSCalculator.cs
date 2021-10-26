using System;
using System.Diagnostics;

namespace SX3.Tools.D3DRender
{
    public class FPSCalculator
    {
        #region Globals

        private Stopwatch _sw;
        private int _frames;

        #endregion

        #region Properties

        public float Value { get; private set; }

        public TimeSpan Sample { get; set; }

        #endregion

        #region Constructor

        public FPSCalculator()
        {
            this.Sample = TimeSpan.FromSeconds(1);
            this.Value = 0;
            this._frames = 0;
            this._sw = Stopwatch.StartNew();
        }

        #endregion

        #region Methods

        public float Update()
        {
            this._frames++;
            if (_sw.Elapsed > Sample)
            {
                this.Value = (float)(_frames / _sw.Elapsed.TotalSeconds);
                this._sw.Reset();
                this._sw.Start();
                this._frames = 0;
            }

            return Value;
        }

        #endregion
    }
}
