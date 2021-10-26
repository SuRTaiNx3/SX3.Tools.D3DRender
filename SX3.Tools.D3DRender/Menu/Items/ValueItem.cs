using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class ValueItem : MenuItem
    {
        #region Properties

        public float Value { get; set; }

        public float MaxValue { get; set; }

        public float MinValue { get; set; }

        public float ValueStep { get; set; }

        #endregion

        #region Constructor

        public ValueItem(string title, float value, float maxValue, float minValue, float valueStep)
        {
            this.Title = title;
            this.Value = value;
            this.MaxValue = maxValue;
            this.MinValue = minValue;
            this.ValueStep = valueStep;
        }

        #endregion

        #region Methods

        public override void Draw(UIRenderer ui, int labelX, int valueX, int y)
        {
            ui.DrawText("default_font", Title, new SD.Point(labelX, y), SD.Color.FromArgb(20, 20, 20));
            ui.DrawShadowText("", Value.ToString("0.00"), new SD.Point(valueX, y), SD.Color.LightGreen);
        }

        public override void ProcessKeyInput(int vkCode)
        {
            base.ProcessKeyInput(vkCode);

            if (vkCode == WF.Keys.Right.GetHashCode())
            {
                if (Value < MaxValue)
                    Value += ValueStep;
            }
            else if (vkCode == WF.Keys.Left.GetHashCode())
            {
                if (Value > MinValue)
                    Value -= ValueStep;
            }
        }

        #endregion
    }
}
