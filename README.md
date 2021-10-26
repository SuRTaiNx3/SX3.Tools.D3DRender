# SX3.Tools.D3DRender
Library for rendering on a Windows Form with DirectX Managed Code. Contains basic functions and a keyboard driven menu.

## Usage
The code below shows a basic example. Please take a look at the SX3.Tools.D3DRender.Example project for a more detailed version.
```csharp
using using SX3.Tools.D3DRender;

int width = 1920;
int height = 1080;
var ui = new UIRenderer(this.Handle);
ui.InitializeDevice(width, height);

new Thread(() => 
{
	while (true)
	{
		ui.StartFrame(width, height);

		ui.DrawBox(100, 100, 75, 150, 1, System.Drawing.Color.Red);

		ui.EndFrame();
	}
}).Start();
```

## License
[MIT](https://choosealicense.com/licenses/mit/)
