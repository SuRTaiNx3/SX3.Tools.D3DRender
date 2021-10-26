using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class ColorItem : MenuItem
    {
        #region Properties

        public Colors SelectedColor { get; set; }

        #endregion

        #region Constructor

        public ColorItem(string title, Colors color)
        {
            this.Title = title;
            this.SelectedColor = color;
        }

        #endregion

        #region Enums

        public enum Colors
        {
            Green,
            Red,
            Cyan,
            Blue,
            Yellow,
            Pink,
            Orange,
            OrangeRed,
            LightBlue,
            White,
            Black,
            LightGreen,
            Gray
        }

        #endregion

        #region Methods

        public FloatColorStruct GetFloats()
        {
            SD.Color color = GetColor();
            FloatColorStruct floats = new FloatColorStruct(color.R / 255, color.G / 255, color.B / 255, color.A / 255);

            return floats;
        }

        public SD.Color GetColor()
        {
            switch (SelectedColor)
            {
                case Colors.Green:
                    return SD.Color.Green;
                case Colors.Red:
                    return SD.Color.Red;
                case Colors.Cyan:
                    return SD.Color.Cyan;
                case Colors.Blue:
                    return SD.Color.Blue;
                case Colors.Yellow:
                    return SD.Color.Yellow;
                case Colors.Pink:
                    return SD.Color.Pink;
                case Colors.Orange:
                    return SD.Color.FromArgb(255, 178, 0);
                case Colors.OrangeRed:
                    return SD.Color.OrangeRed;
                case Colors.LightBlue:
                    return SD.Color.LightBlue;
                case Colors.White:
                    return SD.Color.White;
                case Colors.Black:
                    return SD.Color.Black;
                case Colors.LightGreen:
                    return SD.Color.LightGreen;
                case Colors.Gray:
                    return SD.Color.DimGray;
            }
            return SD.Color.Green;
        }

        public static Colors GetColor(string name)
        {
            switch (name.ToLower())
            {
                case "green":
                    return Colors.Green;
                case "red":
                    return Colors.Red;
                case "cyan":
                    return Colors.Cyan;
                case "blue":
                    return Colors.Blue;
                case "yellow":
                    return Colors.Yellow;
                case "pink":
                    return Colors.Pink;
                case "orange":
                    return Colors.Orange;
                case "orangered":
                    return Colors.OrangeRed;
                case "lightblue":
                    return Colors.LightBlue;
                case "white":
                    return Colors.White;
                case "black":
                    return Colors.Black;
                case "lightgreen":
                    return Colors.LightGreen;
                case "gray":
                    return Colors.Gray;
                default:
                    return Colors.Green;
            }
        }

        public override void Draw(UIRenderer ui, int valueX, int labelX, int y)
        {
            ui.DrawText("default_font", Title, new SD.Point(labelX, y), SD.Color.FromArgb(20, 20, 20));
            ui.DrawFilledBox(valueX, y + 2, 10, 10, GetColor());
        }

        public override void ProcessKeyInput(int vkCode)
        {
            base.ProcessKeyInput(vkCode);
            if (vkCode == WF.Keys.Right.GetHashCode())
            {
                switch (SelectedColor)
                {
                    case Colors.Green:
                        SelectedColor = Colors.Red;
                        break;
                    case Colors.Red:
                        SelectedColor = Colors.Cyan;
                        break;
                    case Colors.Cyan:
                        SelectedColor = Colors.Blue;
                        break;
                    case Colors.Blue:
                        SelectedColor = Colors.Yellow;
                        break;
                    case Colors.Yellow:
                        SelectedColor = Colors.Pink;
                        break;
                    case Colors.Pink:
                        SelectedColor = Colors.Orange;
                        break;
                    case Colors.Orange:
                        SelectedColor = Colors.OrangeRed;
                        break;
                    case Colors.OrangeRed:
                        SelectedColor = Colors.LightBlue;
                        break;
                    case Colors.LightBlue:
                        SelectedColor = Colors.White;
                        break;
                    case Colors.White:
                        SelectedColor = Colors.Black;
                        break;
                    case Colors.Black:
                        SelectedColor = Colors.LightGreen;
                        break;
                    case Colors.LightGreen:
                        SelectedColor = Colors.Gray;
                        break;
                    case Colors.Gray:
                        SelectedColor = Colors.Green;
                        break;
                    default:
                        break;
                }
            }
            else if (vkCode == WF.Keys.Left.GetHashCode())
            {
                switch (SelectedColor)
                {
                    case Colors.Green:
                        SelectedColor = Colors.Gray;
                        break;
                    case Colors.Red:
                        SelectedColor = Colors.Green;
                        break;
                    case Colors.Cyan:
                        SelectedColor = Colors.Red;
                        break;
                    case Colors.Blue:
                        SelectedColor = Colors.Cyan;
                        break;
                    case Colors.Yellow:
                        SelectedColor = Colors.Blue;
                        break;
                    case Colors.Pink:
                        SelectedColor = Colors.Yellow;
                        break;
                    case Colors.Orange:
                        SelectedColor = Colors.Pink;
                        break;
                    case Colors.OrangeRed:
                        SelectedColor = Colors.Orange;
                        break;
                    case Colors.LightBlue:
                        SelectedColor = Colors.OrangeRed;
                        break;
                    case Colors.White:
                        SelectedColor = Colors.LightBlue;
                        break;
                    case Colors.Black:
                        SelectedColor = Colors.White;
                        break;
                    case Colors.LightGreen:
                        SelectedColor = Colors.Black;
                        break;
                    case Colors.Gray:
                        SelectedColor = Colors.LightGreen;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
