using SX3.Tools.D3DRender.Menu;
using SX3.Tools.D3DRender.Menu.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SX3.Tools.D3DRender.Example
{
    public partial class MainForm : Form
    {
        #region Globals

        // Form related

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;
        private Margins marg;
        
        internal struct Margins
        {
            public int Left, Right, Top, Bottom;
        }


        // DirectX related

        private UIRenderer _d3dUI;

        private int _width = 0;
        private int _height = 0;

        // General Menu
        private BackItem generalBackOption;
        private CrosshairTypeItem generalCrosshairOption;
        private ExitItem exitOption;

        // ESP
        private BackItem espBackOption;
        private BoolItem espEnemiesBoxOption;
        private BoolItem espEnemiesLineOption;
        private VerticalPositionItem espEnemiesLinePositionOption;
        private BoolItem espTeamBoxOption;
        private BoolItem espTeamLineOption;
        private VerticalPositionItem espTeamLinePositionOption;

        // Color
        private BackItem colorsBackOption;
        private ColorItem colorEnemiesOption;
        private ColorItem colorEnemiesLineOption;
        private ColorItem colorTeamOption;
        private ColorItem colorTeamLineOption;

        #endregion

        #region Constructor

        public MainForm()
        {
            if (RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully)
                Console.WriteLine("LegacyV2RuntimeEnabledSuccessfully!");

            InitializeComponent();

            // Make the window's border completely transparant
            SetWindowLong(this.Handle, GWL_EXSTYLE, (IntPtr)(GetWindowLong(this.Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED ^ WS_EX_TRANSPARENT));

            // Set the Alpha on the Whole Window to 255 (solid)
            SetLayeredWindowAttributes(this.Handle, 0, 255, LWA_ALPHA);

            // Misc
            this.Resize += MainForm_Resize;
            _width = (int)this.Width;
            _height = (int)this.Height;

            // Prepare DirectX drawing
            _d3dUI = new UIRenderer(this.Handle);
            _d3dUI.Menu.FooterText = "Example";
            //_d3dUI.KeyWasPressed += d3dUI_KeyWasPressed;
            CreateDefaultMenu();
            _d3dUI.InitializeDevice(_width, _height);

            // Create a separate thread for the core functions
            Thread dx = new Thread(new ThreadStart(this.RenderLoop));
            dx.Name = "MainThread";
            dx.IsBackground = true;
            dx.Start();
        }

        #endregion

        #region DLLImports

        //Dll Imports (Form stuff)
        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        #endregion

        #region Methods

        private void RenderLoop()
        {
            int animX = _width - 300;
            int animY = _height / 2 + 50;
            while (!exitOption.ExitApplication)
            {
                _d3dUI.StartFrame(_width, _height);

                _d3dUI.DrawCrosshair(_width / 2, _height / 2, 10, 1, Color.Red, generalCrosshairOption.SelectedType);
                _d3dUI.DrawList(new HashSet<string>() { "first", "second", "third", "fourth" }, "Items", "Total: 4", _width - 200, 100, 190);
                _d3dUI.DrawProgressbar("default_font", "WeeeWoo", 100, 500, 200, 20, 100, 33, true);

                if (espEnemiesBoxOption.State)
                {
                    animX += 2;
                    animY -= 1;
                    if (animX > 1500)
                    {
                        animX = _width / 2 - 300;
                        animY = _height / 2 + 50;
                    }

                    _d3dUI.DrawBox(animX, animY, 75, 150, 1, colorEnemiesOption.GetColor());

                    if (espEnemiesLineOption.State)
                    {
                        if (espEnemiesLinePositionOption.SelectedPosition == VerticalPositionItem.Position.Top)
                            _d3dUI.DrawLine(animX + 32, animY, _width / 2, 0, 1, colorEnemiesLineOption.GetColor());
                        else
                            _d3dUI.DrawLine(animX + 32, animY + 150, _width / 2, _height, 1, colorEnemiesLineOption.GetColor());
                    }
                }
                _d3dUI.EndFrame();
            }

            _d3dUI.Dispose();
            Application.Exit();
        }

        private void CreateDefaultMenu()
        {
            _d3dUI.Menu.Items.Clear();

            // General
            SubMenu generalMenu = new SubMenu("General");
            generalBackOption = new BackItem();
            generalCrosshairOption = new CrosshairTypeItem("Crosshair", CrosshairTypeItem.Type.Cross);
            exitOption = new ExitItem();
            generalMenu.Items.Add(generalBackOption);
            generalMenu.Items.Add(generalCrosshairOption);
            generalMenu.Items.Add(exitOption);
            _d3dUI.Menu.Items.Add(generalMenu);

            // ESP
            SubMenu espMenu = new SubMenu("ESP");
            espBackOption = new BackItem();
            espEnemiesBoxOption = new BoolItem("Enemies Box", true);
            espEnemiesLineOption = new BoolItem("Enemies Line", false);
            espEnemiesLinePositionOption = new VerticalPositionItem("Enemies Line pos", VerticalPositionItem.Position.Bottom);
            espTeamBoxOption = new BoolItem("Team Box", false);
            espTeamLineOption = new BoolItem("Team Line", false);
            espTeamLinePositionOption = new VerticalPositionItem("Team Line pos", VerticalPositionItem.Position.Bottom);

            espMenu.Items.Add(espBackOption);
            espMenu.Items.Add(espEnemiesBoxOption);
            espMenu.Items.Add(espEnemiesLineOption);
            espMenu.Items.Add(espEnemiesLinePositionOption);
            espMenu.Items.Add(new SeparatorItem());
            espMenu.Items.Add(espTeamBoxOption);
            espMenu.Items.Add(espTeamLineOption);
            espMenu.Items.Add(espTeamLinePositionOption);

            _d3dUI.Menu.Items.Add(espMenu);

            // ESP Colors
            SubMenu colorsMenu = new SubMenu("Colors");
            colorsBackOption = new BackItem();
            colorEnemiesOption = new ColorItem("Enemy Color", ColorItem.Colors.Red);
            colorEnemiesLineOption = new ColorItem("Enemy Line Color", ColorItem.Colors.Red);
            colorTeamOption = new ColorItem("Team Color", ColorItem.Colors.Cyan);
            colorTeamLineOption = new ColorItem("Team Line Color", ColorItem.Colors.Cyan);

            colorsMenu.Items.Add(colorsBackOption);
            colorsMenu.Items.Add(colorEnemiesOption);
            colorsMenu.Items.Add(colorEnemiesLineOption);
            colorsMenu.Items.Add(new SeparatorItem());
            colorsMenu.Items.Add(colorTeamOption);
            colorsMenu.Items.Add(colorTeamLineOption);
            _d3dUI.Menu.Items.Add(colorsMenu);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            _width = this.Width;
            _height = this.Height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Create a margin (the whole form)
            marg.Left = 0;
            marg.Top = 0;
            marg.Right = this.Width;
            marg.Bottom = this.Height;

            //Expand the Aero Glass Effect Border to the WHOLE form.
            // since we have already had the border invisible we now
            // have a completely invisible window - apart from the DirectX
            // renders NOT in black.
            DwmExtendFrameIntoClientArea(this.Handle, ref marg);
        }

        #endregion
    }
}
