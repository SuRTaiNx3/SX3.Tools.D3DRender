using Microsoft.DirectX.Direct3D;
using SD = System.Drawing;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class SeparatorItem : MenuItem
    {
        #region Properties

        private int _seperatorPadding = 4;
        public int SeperatorPadding
        {
            get { return _seperatorPadding; }
            set { _seperatorPadding = value; }
        }

        private int _seperatorHeight = 1;
        public int SeperatorHeight
        {
            get { return _seperatorHeight; }
            set { _seperatorHeight = value; }
        }

        #endregion

        #region Constructor

        public SeparatorItem(string title = "separator")
        {
            this.Title = title;
            this.DrawMe = false;
            this.Height = 4;
        }

        #endregion

        #region Methods

        public override void Draw(UIRenderer ui, int valueX, int labelX, int y)
        {
            if (Title != "separator" && Title != "")
            {
                SD.Rectangle titleRect = ui.MeasureString(null, Title, DrawTextFormat.Center, SD.Color.Gray);

                float totalWidth = ui.Menu.MenuWidth - SeperatorPadding;
                float singleLineWidth = (totalWidth / 2) - (titleRect.Width / 2) - SeperatorPadding;

                float firstLineX1 = labelX;
                float firstLineY1 = y;
                float firstLineX2 = singleLineWidth;
                float firstLineY2 = y;

                float secondLineX1 = labelX + (totalWidth / 2) + (titleRect.Width / 2) - SeperatorPadding;
                float secondLineY1 = y;
                float secondLineX2 = ui.Menu.MenuWidth - SeperatorPadding;
                float secondLineY2 = y;

                ui.DrawLine(firstLineX1, firstLineY1 - 1, firstLineX2, firstLineY2 - 1, SeperatorHeight, SD.Color.Gray);
                ui.DrawLine(secondLineX1, secondLineY1 - 1, secondLineX2, secondLineY2 - 1, SeperatorHeight, SD.Color.Gray);
                ui.DrawShadowText("", Title, new SD.Point((int)firstLineX2 + SeperatorPadding, y - 10), SD.Color.Gray);
            }
            else
            {
                ui.DrawLine(labelX, y - 1, ui.Menu.MenuWidth - SeperatorPadding, y - 1, SeperatorHeight, SD.Color.Gray);
            }
        }

        #endregion
    }
}
