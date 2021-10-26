using System;
using System.Collections.Generic;
using SD = System.Drawing;
using WF = System.Windows.Forms;
using System.Linq;
using SX3.Tools.D3DRender.Menu.Items;

namespace SX3.Tools.D3DRender.Menu
{
    public class RootMenu
    {
        #region Globals

        // General
        private UIRenderer _ui;
        private static RootMenu _currentInstance;
        
        // Menu position related
        private static int _selectedSubMenu = -1;
        private static int _selectedSubMenuItemCount = 0;
        private static int _selectedMenuItem = 0;
        private static int _markedSubMenu = 0;
        private static int _menuItemCount { get { return _currentInstance.Items.Count; } }

        private int _textY;

        // Keyboard hook
        

        #endregion

        #region Properties

        public List<SubMenu> Items { get; set; }

        public bool IsVisible { get; set; } = true;

        public string FooterText { get; set; }


        private float _menuWidth = 250;
        public float MenuWidth
        {
            get { return _menuWidth; }
            set { _menuWidth = value; }
        }

        private float _menuPositionX = 7;
        public float MenuPositionX
        {
            get { return _menuPositionX; }
            set { _menuPositionX = value; }
        }

        private float _menuPositionY = 178;
        public float MenuPositionY
        {
            get { return _menuPositionY; }
            set { _menuPositionY = value; }
        }

        private int _titleHeight = 23;
        public int TitleHeight
        {
            get { return _titleHeight; }
            set { _titleHeight = value; }
        }

        private int _menuItemHeight = 18;
        public int MenuItemHeight
        {
            get { return _menuItemHeight; }
            set { _menuItemHeight = value; }
        }

        private int _layerStep = 15;
        public int LayerStep
        {
            get { return _layerStep; }
            set { _layerStep = value; }
        }

        private int _textYStart = 210;
        public int TextYStart
        {
            get { return _textYStart; }
            set { _textYStart = value; }
        }

        // Protected

        protected int TextX
        {
            get { return (int)MenuPositionX + 18; }
        }

        protected int ValueX
        {
            get { return (int)MenuPositionX + 150; }
        }

        protected int ColorX
        {
            get { return (int)MenuPositionX + 180; }
        }

        protected int NavigatorX
        {
            get { return (int)MenuPositionX + 8; }
        }

        #endregion

        #region Constructor

        public RootMenu(UIRenderer uiRenderer)
        {
            _ui = uiRenderer;
            Items = new List<SubMenu>();
            _currentInstance = this;
        }

        #endregion

        #region Methods

        public void Draw(UIRenderer ui)
        {
            if (!IsVisible)
                return;

            int menuBottom = 0;
            if (_selectedSubMenu == -1)
            {
                // Renders the main menu
                _ui.DrawTransparentBox(MenuPositionX, MenuPositionY, MenuWidth, 45 + (Items.Count * MenuItemHeight), 238, SD.Color.FromArgb(216, 216, 216));
                _ui.DrawBox(MenuPositionX, MenuPositionY, MenuWidth, 60 + (Items.Count * MenuItemHeight), 1, SD.Color.FromArgb(60, 60, 60));
                _ui.DrawTransparentBox(MenuPositionX, MenuPositionY, MenuWidth, TitleHeight, 248, SD.Color.FromArgb(255, 40, 40, 40));
                _ui.DrawBox(MenuPositionX, MenuPositionY, MenuWidth, TitleHeight, 1, SD.Color.Black);
                _ui.DrawShadowText("title_font", "Main", new SD.Point((int)MenuPositionX + 5, (int)MenuPositionY + 3), SD.Color.White);

                foreach (SubMenu menuItem in Items)
                {
                    int x = TextX + (menuItem.Layer * LayerStep);

                    if (menuItem.Enabled)
                        _ui.DrawText("default_bold_font", "[ " + menuItem.Title + " ]", new SD.Point(x, _textY), SD.Color.FromArgb(53, 53, 53));
                    else
                        _ui.DrawText("default_bold_font", "[ " + menuItem.Title + "] DISABLED", new SD.Point(x, _textY), SD.Color.FromArgb(112, 112, 112));
                    _textY = _textY + MenuItemHeight;
                }

                _textY = TextYStart;
                menuBottom = TextYStart + (Items.Count * MenuItemHeight) + 11;
            }
            else
            {
                // Renders the sub menu
                _selectedSubMenuItemCount = Items[_selectedSubMenu].Items.Where(sub => sub.DrawMe).Count();

                SubMenu subMenu = Items[_selectedSubMenu];
                _ui.DrawTransparentBox(MenuPositionX, MenuPositionY, MenuWidth, 45 + (_selectedSubMenuItemCount * MenuItemHeight), 238, SD.Color.FromArgb(216, 216, 216));
                _ui.DrawBox(MenuPositionX, MenuPositionY, MenuWidth, 60 + (_selectedSubMenuItemCount * MenuItemHeight), 1, SD.Color.FromArgb(60, 60, 60));
                _ui.DrawTransparentBox(MenuPositionX, MenuPositionY, MenuWidth, TitleHeight, 248, SD.Color.FromArgb(255, 40, 40, 40));
                _ui.DrawBox(MenuPositionX, MenuPositionY, MenuWidth, TitleHeight, 1, SD.Color.Black);
                _ui.DrawShadowText("title_font", subMenu.Title, new SD.Point((int)MenuPositionX + 5, (int)MenuPositionY + 3), SD.Color.White);

                foreach (MenuItem menuItem in subMenu.Items)
                {
                    if (menuItem.DrawMe)
                        _ui.DrawText("default_font", menuItem.Title, new SD.Point(TextX, _textY), SD.Color.FromArgb(20, 20, 20));

                    menuItem.Draw(ui, ValueX, TextX, _textY);

                    if (menuItem.DrawMe)
                        _textY = _textY + MenuItemHeight;
                }

                _textY = TextYStart;
                menuBottom = TextYStart + (_selectedSubMenuItemCount * MenuItemHeight) + 11;
            }

            int navigatorX = NavigatorX;

            int menuItemIndex = 0;
            //Main menu
            if (_selectedSubMenu < 0)
            {
                menuItemIndex = _markedSubMenu;
                navigatorX = (NavigatorX) + (Items[_markedSubMenu].Layer * LayerStep);
            }
            else //Submenu
            {
                menuItemIndex = _selectedMenuItem;
            }

            ui.DrawShadowText("", ">", new SD.Point(navigatorX, TextYStart + (MenuItemHeight * menuItemIndex)), SD.Color.Red);

            // Footer
            ui.DrawBox(MenuPositionX, menuBottom, MenuWidth, 20, 1, SD.Color.Black);
            ui.DrawFilledBox(MenuPositionX, menuBottom, MenuWidth, 20, SD.Color.FromArgb(60, 60, 60));

            SD.Point footerPoint = new SD.Point((int)MenuPositionX, menuBottom);
            SD.Rectangle rect = new SD.Rectangle((int)MenuPositionX, menuBottom + 1, (int)MenuWidth, 55);
            ui.DrawShadowText("footer_font", FooterText, rect, Microsoft.DirectX.Direct3D.DrawTextFormat.Center, SD.Color.Gray);
        }

        public void ProcessKeyInput(int vkCode)
        {
            int menuItemCount = 0;
            if (_selectedSubMenu < 0)
                menuItemCount = _menuItemCount;
            else
                menuItemCount = _selectedSubMenuItemCount;

            if (_selectedSubMenu < 0)
            {
                //Main menu

                if (vkCode == WF.Keys.Up.GetHashCode() && _markedSubMenu > 0)
                    _markedSubMenu--;
                else if (vkCode == WF.Keys.Up.GetHashCode() && _markedSubMenu == 0)
                    _markedSubMenu = menuItemCount - 1;

                if (vkCode == WF.Keys.Down.GetHashCode() && _markedSubMenu < menuItemCount - 1)
                    _markedSubMenu++;
                else if (vkCode == WF.Keys.Down.GetHashCode() && _markedSubMenu == menuItemCount - 1)
                    _markedSubMenu = 0;

                if (vkCode == WF.Keys.Right.GetHashCode() && _currentInstance.Items[_markedSubMenu].Enabled)
                    _selectedSubMenu = _markedSubMenu;
            }
            else
            {
                //Sub Menu

                if (vkCode == WF.Keys.Up.GetHashCode() && _selectedMenuItem > 0)
                    _selectedMenuItem--;
                else if (vkCode == WF.Keys.Up.GetHashCode() && _selectedMenuItem == 0)
                    _selectedMenuItem = menuItemCount - 1;

                if (vkCode == WF.Keys.Down.GetHashCode() && _selectedMenuItem < menuItemCount - 1)
                    _selectedMenuItem++;
                else if (vkCode == WF.Keys.Down.GetHashCode() && _selectedMenuItem == menuItemCount - 1)
                    _selectedMenuItem = 0;

                MenuItem menuitem = _currentInstance.Items[_selectedSubMenu].Items.Where(sub => sub.DrawMe).ElementAt(_selectedMenuItem);
                menuitem.ProcessKeyInput(vkCode);
                if (menuitem.GetType() == typeof(BackItem) && vkCode == WF.Keys.Left.GetHashCode())
                {
                    _selectedMenuItem = 0;
                    _selectedSubMenu = -1;
                }
            }
        }

        #endregion
    }
}
