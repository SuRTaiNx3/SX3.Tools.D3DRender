using SX3.Tools.D3DRender.Menu.Items;
using System.Collections.Generic;

namespace SX3.Tools.D3DRender.Menu
{
    public class SubMenu
    {
        #region Properties

        public string Title { get; set; }

        public bool Enabled { get; set; }

        public List<MenuItem> Items { get; set; }

        public int Layer { get; set; }

        #endregion

        #region Constructor

        public SubMenu(string title, int layer = 0, bool enabled = true)
        {
            this.Title = title;
            this.Enabled = enabled;
            this.Layer = layer;
            Items = new List<MenuItem>();
        }

        public SubMenu() { }

        #endregion
    }
}
