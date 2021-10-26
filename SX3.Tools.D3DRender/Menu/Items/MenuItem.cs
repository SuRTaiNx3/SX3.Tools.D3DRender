using SD = System.Drawing;

namespace SX3.Tools.D3DRender.Menu.Items
{
    public class MenuItem
    {
        #region Properties

        public string Title { get; set; }

        public bool Enabled { get; set; }

        private int _height = 15;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }


        private bool _drawMe = true;
        public bool DrawMe
        {
            get { return _drawMe; }
            set { _drawMe = value; }
        }

        #endregion

        #region Constructor

        public MenuItem()
        {
            this.Enabled = true;
        }

        #endregion

        #region Methods

        public virtual void Draw(UIRenderer ui, int valueX, int labelX, int y){}

        public virtual void ProcessKeyInput(int vkCode) { }

        #endregion
    }
}
