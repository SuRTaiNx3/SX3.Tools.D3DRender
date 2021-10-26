using SD = System.Drawing;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class BackItem : MenuItem
    {
        #region Constructor

        public BackItem()
        {
            Title = " ";
        }

        #endregion

        #region Methods

        public override void Draw(UIRenderer ui, int valueX, int labelX, int y)
        {
            ui.DrawShadowText("", "< Back", new SD.Point(labelX, y), SD.Color.FromArgb(38, 119, 219));
        }

        #endregion
    }
}
