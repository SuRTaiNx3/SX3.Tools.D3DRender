using SD = System.Drawing;
using WF = System.Windows.Forms;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class CrosshairTypeItem : MenuItem
    {
        #region Properties

        public Type SelectedType { get; set; }

        #endregion

        #region Constructor

        public CrosshairTypeItem(string title, Type type, bool enabled = true)
        {
            this.Title = title;
            this.SelectedType = type;
            this.Enabled = enabled;
        }

        #endregion

        #region Enums

        public enum Type
        {
            FullCross,
            Cross,
            Circle,
            CircleAndCross,
            FilledCircle,
            TiltedCross
        }

        #endregion

        #region Methods

        public static Type GetType(string type)
        {
            switch (type)
            {
                case "FullCross":
                    return Type.FullCross;
                case "Cross":
                    return Type.Cross;
                case "Circle":
                    return Type.Circle;
                case "CircleAndCross":
                    return Type.CircleAndCross;
                case "FilledCircle":
                    return Type.FilledCircle;
                case "TiltedCross":
                    return Type.TiltedCross;
                default:
                    return Type.Cross;
            }
        }

        public override void Draw(UIRenderer ui, int valueX, int labelX, int y)
        {
            ui.DrawText("default_font", Title, new SD.Point(labelX, y), SD.Color.FromArgb(20, 20, 20));
            ui.DrawShadowText("", SelectedType.ToString(), new SD.Point(valueX, y), SD.Color.DimGray);
        }

        public override void ProcessKeyInput(int vkCode)
        {
            base.ProcessKeyInput(vkCode);

            if (vkCode == WF.Keys.Right.GetHashCode())
            {
                switch (SelectedType)
                {
                    case Type.Circle:
                        SelectedType = Type.CircleAndCross;
                        break;
                    case Type.CircleAndCross:
                        SelectedType = Type.FilledCircle;
                        break;
                    case Type.FilledCircle:
                        SelectedType = Type.TiltedCross;
                        break;
                    case Type.TiltedCross:
                        SelectedType = Type.FullCross;
                        break;
                    case Type.FullCross:
                        SelectedType = Type.Cross;
                        break;
                    case Type.Cross:
                        SelectedType = Type.Circle;
                        break;
                    default:
                        break;
                }
            }
            else if (vkCode == WF.Keys.Left.GetHashCode())
            {
                switch (SelectedType)
                {
                    case Type.Circle:
                        SelectedType = Type.Cross;
                        break;
                    case Type.Cross:
                        SelectedType = Type.FullCross;
                        break;
                    case Type.FullCross:
                        SelectedType = Type.TiltedCross;
                        break;
                    case Type.TiltedCross:
                        SelectedType = Type.FilledCircle;
                        break;
                    case Type.FilledCircle:
                        SelectedType = Type.CircleAndCross;
                        break;
                    case Type.CircleAndCross:
                        SelectedType = Type.Circle;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
