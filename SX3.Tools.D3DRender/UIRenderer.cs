using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SX3.Tools.D3DRender.Menu;
using System;
using SD = System.Drawing;
using WF = System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SX3.Tools.D3DRender.Menu.Items;

namespace SX3.Tools.D3DRender
{
    public class UIRenderer
    {
        #region Globals

        // General
        private IntPtr _hostWindowHandle = IntPtr.Zero;
        private static UIRenderer _currentInstance = null;

        // D3D
        private Device _device = null;
        private PresentParameters _presentParameters = null;
        private Line _2DLine = null;

        // FPS
        private FPSCalculator _fpsCalculator;

        // Fonts
        private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        // Keyboard hook
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc _proc = hookProc;
        private static IntPtr _hhook = IntPtr.Zero;

        private const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x0101;

        #endregion

        #region Events

        public delegate void KeyWasPressedEventHandler(object sender, int keyCode, IntPtr wParam);
        public event KeyWasPressedEventHandler OnKeyPressed;

        #endregion

        #region DllImports

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        #endregion

        #region Properties

        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        private bool _isRendering = false;
        public bool IsRendering
        {
            get { return _isRendering; }
        }

        private int _deviceWidth;
        public int DeviceWidth
        {
            get { return _deviceWidth; }
        }

        private int _deviceHeight;
        public int DeviceHeight
        {
            get { return _deviceHeight; }
        }

        private RootMenu _menu;
        public RootMenu Menu 
        { 
            get { return _menu; } 
        }


        // Input related

        private static bool _controlIsPressed = false;
        public bool ControlIsPressed
        {
            get { return _controlIsPressed; }
        }

        private static bool _altIsPressed = false;
        public bool AltIsPressed
        {
            get { return _altIsPressed; }
        }

        private static bool _shiftIsPressed = false;
        public bool ShiftIsPressed
        {
            get { return _shiftIsPressed; }
        }

        #endregion

        #region Constructor

        public UIRenderer(IntPtr hostWindowHandle)
        {
            _hostWindowHandle = hostWindowHandle;
            _currentInstance = this;
            _menu = new RootMenu(this);
        }

        #endregion

        #region Methods

        #region General

        public void InitializeDevice(int width, int height)
        {
            _presentParameters = new PresentParameters();
            _presentParameters.Windowed = true;
            _presentParameters.SwapEffect = SwapEffect.Discard;
            _presentParameters.BackBufferFormat = Format.A8R8G8B8;
            _presentParameters.BackBufferWidth = width;
            _presentParameters.BackBufferHeight = height;

            _device = new Device(0, DeviceType.Hardware, _hostWindowHandle, CreateFlags.HardwareVertexProcessing, _presentParameters);

            _isInitialized = true;
            CreateResources();
            SetHook();
        }

        private void CreateResources()
        {
            _fpsCalculator = new FPSCalculator();
            RegisterFont("default_font", "Museo", 10);
            RegisterFont("default_bold_font", "Museo", 10, SD.FontStyle.Bold);
            RegisterFont("fps_font", "Arial", 12, SD.FontStyle.Bold);
            RegisterFont("footer_font", "Museo", 8);
            RegisterFont("title_font", "Verdana", 10, SD.FontStyle.Bold);

            _2DLine = new Line(_device);
        }

        public void StartFrame(int width, int height)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("InitializeDevice() must be called first!");

            if(IsRendering)
                throw new InvalidOperationException("Cannot call StartFrame when the previous frame was not finished. Call EndFrame() first!");

            _isRendering = true;

            _device.Clear(ClearFlags.Target, SD.Color.FromArgb(0, 0, 0, 0), 1.0f, 0);
            _device.RenderState.ZBufferEnable = false;
            _device.RenderState.Lighting = false;
            _device.RenderState.CullMode = Cull.None;
            _device.Transform.Projection = Matrix.OrthoOffCenterLH(0, width, height, 0, 0, 1);
            _device.BeginScene();

            GetFontOrDefault("fps_font").DrawText(null, $"{_fpsCalculator.Update().ToString("00")} FPS", width - 110, 3, SD.Color.Red);

            Menu.Draw(this);
        }

        public void EndFrame()
        {
            if (IsRendering)
            {
                _device.EndScene();
                _device.Present();
                _isRendering = false;
            }
        }

        public void Reset(int width, int height)
        {
            if (IsInitialized && !IsRendering)
            {
                _deviceHeight = height;
                _deviceWidth = width;
                _presentParameters.BackBufferWidth = width;
                _presentParameters.BackBufferHeight = height;
                _device.Reset(_presentParameters);
            }
        }

        public void Dispose()
        {
            foreach (var font in _fonts)
                font.Value.Dispose();

            _2DLine.Dispose();
            _device.Dispose();

            UnHook();
        }

        private SD.Color SetColorTransparency(int A, SD.Color color)
        {
            return SD.Color.FromArgb(A, color.R, color.G, color.B);
        }

        #endregion

        #region Fonts

        public void RegisterFont(string fontKey, string fontFamily, int size, SD.FontStyle style = SD.FontStyle.Regular)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("InitializeDevice() must be called first!");

            _fonts.Add(fontKey, new Font(_device, new SD.Font(fontFamily, 10, style)));
        }

        private Font GetFontOrDefault(string fontKey)
        {
            return _fonts.ContainsKey(fontKey) ? _fonts[fontKey] : _fonts["default_font"];
        }

        public SD.Rectangle MeasureString(string fontKey, string text, DrawTextFormat format, SD.Color color)
        {
            Font ton = GetFontOrDefault(fontKey);
            return ton.MeasureString(null, text, format, color);
        }

        #endregion

        #region Basic drawing

        public void DrawText(string fontKey, string text, SD.Point Position, SD.Color color)
        {
            GetFontOrDefault(fontKey).DrawText(null, text, Position.X, Position.Y, color);
        }

        public void DrawText(string fontKey, string text, SD.Rectangle rect, DrawTextFormat textFormat, SD.Color color)
        {
            GetFontOrDefault(fontKey).DrawText(null, text, rect, textFormat, color);
        }

        public void DrawShadowText(string fontKey, string text, SD.Point Position, SD.Color color)
        {
            GetFontOrDefault(fontKey).DrawText(null, text, Position.X + 1, Position.Y + 1, SD.Color.Black);
            GetFontOrDefault(fontKey).DrawText(null, text, Position.X, Position.Y, color);
        }

        public void DrawShadowText(string fontKey, string text, SD.Rectangle rect, DrawTextFormat textFormat, SD.Color color)
        {
            SD.Rectangle rect2 = new SD.Rectangle(rect.X + 1, rect.Y + 2, rect.Width, rect.Height);

            GetFontOrDefault(fontKey).DrawText(null, text, rect2, textFormat, SD.Color.Black);
            GetFontOrDefault(fontKey).DrawText(null, text, rect, textFormat, color);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, float w, SD.Color Color)
        {
            Vector2[] vLine = new Vector2[2] { new Vector2(x1, y1), new Vector2(x2, y2) };

            _2DLine.GlLines = true;
            _2DLine.Antialias = true;
            _2DLine.Width = w;

            _2DLine.Begin();
            _2DLine.Draw(vLine, Color);
            _2DLine.End();
        }

        public void DrawCircle(float centerX, float centerY, float radius, float w, SD.Color color)
        {
            const int numpoints = 60;
            Vector2[] vLine = new Vector2[numpoints + 1];
            float wedgeAngle = (2 * (float)Math.PI) / numpoints;

            for (int i = 0; i <= numpoints; i++)
            {
                float theta = (float)(i * wedgeAngle);
                vLine[i].X = (centerX + radius * (float)Math.Cos(theta));
                vLine[i].Y = (centerY - radius * (float)Math.Sin(theta));
            }

            _2DLine.GlLines = false;
            _2DLine.Antialias = true;
            _2DLine.Width = w;

            _2DLine.Begin();
            _2DLine.Draw(vLine, color);
            _2DLine.End();
        }

        private void DrawFilledCircle(float x, float y, float rad, SD.Color color)
        {
            CustomVertex.TransformedColoredTextured[] circle = new CustomVertex.TransformedColoredTextured[400];

            float xPos = x;
            float yPos = y;

            float x1 = x;
            float y1 = y;

            for (int i = 0; i <= 363; i += 3)
            {
                float angle = (i / 57.3f);
                float x2 = xPos + (rad * (float)Math.Sin(angle));
                float y2 = yPos + (rad * (float)Math.Cos(angle));
                circle[i] = CreateD3DTLVertex(xPos, yPos, 0, 1, color, 0, 0);
                circle[i + 1] = CreateD3DTLVertex(x1, y1, 0, 1, color, 0, 0);
                circle[i + 2] = CreateD3DTLVertex(x2, y2, 0, 1, color, 0, 0);

                y1 = y2;
                x1 = x2;
            }

            _device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
            _device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 363, circle);
        }

        private CustomVertex.TransformedColoredTextured CreateD3DTLVertex(float x, float y, float z, float rhw, SD.Color color, float tu, float tv)
        {
            CustomVertex.TransformedColoredTextured vertex = new CustomVertex.TransformedColoredTextured();
            vertex.Color = color.ToArgb();
            vertex.Rhw = rhw;
            vertex.Tu = tu;
            vertex.Tv = tv;
            vertex.X = x;
            vertex.Y = y;
            vertex.Z = z;
            return vertex;
        }


        public void DrawFilledBox(float x, float y, float w, float h, SD.Color Color)
        {
            Vector2[] vLine = new Vector2[2];

            _2DLine.GlLines = true;
            _2DLine.Antialias = false;
            _2DLine.Width = w;

            vLine[0].X = x + w / 2;
            vLine[0].Y = y;
            vLine[1].X = x + w / 2;
            vLine[1].Y = y + h;

            _2DLine.Begin();
            _2DLine.Draw(vLine, Color);
            _2DLine.End();
        }

        public void DrawTransparentBox(float x, float y, float w, float h, int transparency, SD.Color Color)
        {
            Vector2[] vLine = new Vector2[2];

            _2DLine.GlLines = true;
            _2DLine.Antialias = false;
            _2DLine.Width = w;

            vLine[0].X = x + w / 2;
            vLine[0].Y = y;
            vLine[1].X = x + w / 2;
            vLine[1].Y = y + h;
            SD.Color halfTransparent = SetColorTransparency(transparency, Color);
            _2DLine.Begin();
            _2DLine.Draw(vLine, halfTransparent);
            _2DLine.End();
        }

        public void DrawBox(float x, float y, float w, float h, float px, SD.Color Color)
        {
            DrawFilledBox(x, y + h, w, px, Color);
            DrawFilledBox(x - px, y, px, h, Color);
            DrawFilledBox(x, y - px, w, px, Color);
            DrawFilledBox(x + w, y, px, h, Color);
        }

        #endregion

        #region Advanced drawing

        public void DrawList(HashSet<string> items, string title, string footer, float x, float y, int width)
        {
            const int titleHeight = 23;

            const int lineHeight = 16;
            int playerTextHeight = items.Count * lineHeight;
            int totalHeight = playerTextHeight + titleHeight + 23;

            //Content box
            DrawFilledBox(x, y, width, totalHeight, SD.Color.FromArgb(216, 216, 216));
            DrawBox(x, y, width, totalHeight, 1, SD.Color.FromArgb(60, 60, 60));

            //Title
            DrawTransparentBox(x, y, width, titleHeight, 248, SD.Color.FromArgb(255, 40, 40, 40));
            DrawBox(x, y, width, titleHeight, 1, SD.Color.Black);
            DrawShadowText("title_font", title, new SD.Point((int)x + 5, (int)y + 3), SD.Color.White);

            string text = "";
            foreach (string item in items)
                text += item + "\n";

            DrawText("default_bold_font", text, new SD.Point((int)x + 3, (int)y + 27), SD.Color.FromArgb(10, 10, 10));

            DrawBox(x, y + playerTextHeight + 28, width, 20, 1, SD.Color.Black);
            DrawFilledBox(x, y + playerTextHeight + 28, width, 20, SD.Color.FromArgb(60, 60, 60));

            SD.Rectangle rect = new SD.Rectangle((int)x, (int)y + 1, width, 55);
            DrawShadowText("footer_font", footer, rect, DrawTextFormat.Center, SD.Color.White);
        }

        public void DrawProgressbar(string fontKey, string title, float x, float y, float width, float height, double maxValue, double value, bool drawText)
        {
            float totalHeight = height;
            float totalWidth = width;
            int textFieldWidth = 50;
            float textX = x + 5;
            float textY = y + 2;

            // Left box with label
            if (!string.IsNullOrEmpty(title))
            {
                DrawFilledBox(x, y, textFieldWidth, totalHeight, SD.Color.FromArgb(40, 40, 40));
                GetFontOrDefault("default_font").DrawText(null, title, (int)textX, (int)textY, SD.Color.White);
            }
            else
            {
                textFieldWidth = 0;
            }

            // MainBox
            float mainBoxX = x + textFieldWidth;
            float mainBoxY = y;
            float mainBoxWidth = totalWidth - textFieldWidth;
            double percentageValue = Math.Ceiling(((value * 100) / maxValue));

            DrawFilledBox(mainBoxX, mainBoxY, mainBoxWidth, totalHeight, SD.Color.FromArgb(104, 104, 104));

            // Calculate how much the progressbar is filled
            int coloredWidth = (int)((percentageValue / 100) * mainBoxWidth);

            SD.Color fillColor = SD.Color.DimGray;
            if (percentageValue > 80)
                fillColor = SD.Color.DarkGreen;
            else if (percentageValue > 50)
                fillColor = SD.Color.Gold;
            else if (percentageValue > 20)
                fillColor = SD.Color.Orange;
            else if (percentageValue > 0)
                fillColor = SD.Color.DarkRed;

            if (coloredWidth > 0)
                DrawFilledBox(mainBoxX, mainBoxY, coloredWidth, totalHeight, fillColor);

            // Draw Text
            if (drawText)
            {
                string valueText = value.ToString("0");
                SD.Rectangle valueTextRect = new SD.Rectangle((int)mainBoxX, (int)mainBoxY + 2, (int)mainBoxWidth, (int)totalHeight);
                GetFontOrDefault("default_font").DrawText(null, valueText, valueTextRect, DrawTextFormat.Center, SD.Color.White);
            }
        }

        public void DrawCrosshair(float centerX, float centerY, float size, float thickness, SD.Color color, CrosshairTypeItem.Type type)
        {
            switch (type)
            {
                case CrosshairTypeItem.Type.FullCross:
                    DrawLine(centerX - size, centerY, centerX + size, centerY, thickness, color);
                    DrawLine(centerX, centerY - size, centerX, centerY + size, thickness, color);
                    break;
                case CrosshairTypeItem.Type.Cross:
                    DrawLine(centerX - size, centerY, centerX - 3, centerY, thickness, color);
                    DrawLine(centerX + size, centerY, centerX + 3, centerY, thickness, color);
                    DrawLine(centerX, centerY - size, centerX, centerY - 3, thickness, color);
                    DrawLine(centerX, centerY + size, centerX, centerY + 3, thickness, color);
                    break;
                case CrosshairTypeItem.Type.Circle:
                    DrawCircle(centerX, centerY, size, 1, color);
                    break;
                case CrosshairTypeItem.Type.CircleAndCross:
                    DrawLine(centerX - size, centerY, centerX + size, centerY, thickness, color);
                    DrawLine(centerX, centerY - size, centerX, centerY + size, thickness, color);
                    DrawCircle(centerX, centerY, size - 5, thickness, color);
                    break;
                case CrosshairTypeItem.Type.FilledCircle:
                    DrawFilledCircle(centerX, centerY, size / 5, color);
                    break;
                case CrosshairTypeItem.Type.TiltedCross:
                    DrawLine(centerX + size, centerY + size, centerX + 3, centerY + 3, thickness, color);
                    DrawLine(centerX - size, centerY + size, centerX - 3, centerY + 3, thickness, color);
                    DrawLine(centerX + size, centerY - size, centerX + 3, centerY - 3, thickness, color);
                    DrawLine(centerX - size, centerY - size, centerX - 3, centerY - 3, thickness, color);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Input

        private void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            _hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        private void UnHook()
        {
            UnhookWindowsHookEx(_hhook);
        }

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);

            if (_currentInstance.OnKeyPressed != null)
                _currentInstance.OnKeyPressed(_currentInstance, vkCode, wParam);

            // Control Pressed
            if (vkCode == WF.Keys.LControlKey.GetHashCode() && wParam == (IntPtr)WM_KEYUP)
                _controlIsPressed = false;
            if (vkCode == WF.Keys.LControlKey.GetHashCode() && wParam == (IntPtr)WM_KEYDOWN)
                _controlIsPressed = true;

            // ALt pressed
            if (vkCode == WF.Keys.Alt.GetHashCode() && wParam == (IntPtr)WM_KEYUP)
                _altIsPressed = false;
            if (vkCode == WF.Keys.Alt.GetHashCode() && wParam == (IntPtr)WM_KEYDOWN)
                _altIsPressed = true;

            // Shift Pressed
            if (vkCode == WF.Keys.LShiftKey.GetHashCode() && wParam == (IntPtr)WM_KEYUP)
                _shiftIsPressed = false;
            if (vkCode == WF.Keys.LShiftKey.GetHashCode() && wParam == (IntPtr)WM_KEYDOWN)
                _shiftIsPressed = true;

            bool ValidKeyDown = false;
            if (vkCode == WF.Keys.Up.GetHashCode()
                || vkCode == WF.Keys.Down.GetHashCode() || vkCode == WF.Keys.Right.GetHashCode()
                || vkCode == WF.Keys.Left.GetHashCode() || vkCode == WF.Keys.Insert.GetHashCode()
                || vkCode == WF.Keys.NumPad2.GetHashCode() || vkCode == WF.Keys.NumPad8.GetHashCode()
                || vkCode == WF.Keys.NumPad5.GetHashCode() || vkCode == WF.Keys.NumPad9.GetHashCode()
                || vkCode == WF.Keys.NumPad3.GetHashCode())
                ValidKeyDown = true;

            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN && ValidKeyDown)
            {
                // Hide/Show Menu
                if (vkCode == WF.Keys.Insert.GetHashCode())
                    _currentInstance.Menu.IsVisible = !_currentInstance.Menu.IsVisible;

                if (!_currentInstance.Menu.IsVisible)
                    return CallNextHookEx(_hhook, code, (int)wParam, lParam);

                _currentInstance.Menu.ProcessKeyInput(vkCode);

                return (IntPtr)1;
            }
            else
                return CallNextHookEx(_hhook, code, (int)wParam, lParam);
        }

        #endregion

        #endregion
    }
}
