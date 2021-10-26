using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class ExitItem : MenuItem
    {
        #region Properties

        public bool ExitApplication { get; set; }

        #endregion

        #region Methods

        public override void Draw(UIRenderer ui, int valueX, int labelX, int y)
        {
            ui.DrawShadowText("", "Exit", new SD.Point(labelX, y), SD.Color.DarkRed);
        }

        public override void ProcessKeyInput(int vkCode)
        {
            base.ProcessKeyInput(vkCode);

            if(vkCode == WF.Keys.Right.GetHashCode())
                ExitApplication = true;
        }

        #endregion
    }
}
