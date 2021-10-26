using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class BoolItem : MenuItem
    {
        #region Properties

        public bool State { get; set; }

        #endregion

        #region Constructor

        public BoolItem(string title, bool state, bool enbaled = true)
        {
            this.Title = title;
            this.State = state;
            this.Enabled = enbaled;
        }

        #endregion

        #region Methods

        public override void Draw(UIRenderer ui, int valueX, int labelX, int y)
        {
            ui.DrawText("default_font", Title, new SD.Point(labelX, y), SD.Color.FromArgb(20, 20, 20));
            if (Enabled)
            {
                if (State)
                    ui.DrawShadowText("", "ON", new SD.Point(valueX, y), SD.Color.LightGreen);
                else
                    ui.DrawShadowText("", "OFF", new SD.Point(valueX, y), SD.Color.Red);
            }
            else
            {
                ui.DrawShadowText("", "DISABLED", new SD.Point(valueX, y), SD.Color.Gray);
            }
        }

        public override void ProcessKeyInput(int vkCode)
        {
            base.ProcessKeyInput(vkCode);
            if(vkCode == WF.Keys.Right.GetHashCode() || vkCode == WF.Keys.Left.GetHashCode())
                State = !State;
        }

        #endregion
    }
}
