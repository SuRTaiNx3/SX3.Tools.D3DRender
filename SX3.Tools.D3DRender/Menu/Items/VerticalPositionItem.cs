using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class VerticalPositionItem : MenuItem
    {
        #region Properties

        public Position SelectedPosition { get; set; }

        #endregion

        #region Constructor

        public VerticalPositionItem(string title, Position position, bool enabled = true)
        {
            this.Title = title;
            this.SelectedPosition = position;
            this.Enabled = enabled;
        }

        #endregion

        #region Enums

        public enum Position
        {
            Top,
            Bottom
        }

        #endregion

        #region Methods

        public static Position GetPosition(string position)
        {
            switch (position)
            {
                case "Top":
                    return Position.Top;
                case "Bottom":
                    return Position.Bottom;
                default:
                    return Position.Bottom;
            }
        }

        public override void Draw(UIRenderer ui, int valueX, int labelX, int y)
        {
            ui.DrawText("default_font", Title, new SD.Point(labelX, y), SD.Color.FromArgb(20, 20, 20));
            ui.DrawShadowText("", SelectedPosition.ToString(), new SD.Point(valueX, y), SD.Color.DimGray);
        }

        public override void ProcessKeyInput(int vkCode)
        {
            base.ProcessKeyInput(vkCode);

            if (vkCode == WF.Keys.Right.GetHashCode())
            {
                switch (SelectedPosition)
                {
                    case Position.Bottom:
                        SelectedPosition = Position.Top;
                        break;
                    case Position.Top:
                        SelectedPosition = Position.Bottom;
                        break;
                    default:
                        break;
                }
            }
            else if (vkCode == WF.Keys.Left.GetHashCode())
            {
                switch (SelectedPosition)
                {
                    case Position.Top:
                        SelectedPosition = Position.Bottom;
                        break;
                    case Position.Bottom:
                        SelectedPosition = Position.Top;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
