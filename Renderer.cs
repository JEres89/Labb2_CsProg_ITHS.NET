using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET;
internal class Renderer
{
	public static Renderer Instance { get; private set; } = new();
	private Renderer() { }

	private readonly Queue<((int y, int x) pos, (char c, ConsoleColor fg, ConsoleColor bg) gfx)> _updateQueue = new();
    public int MapXoffset { get; set; }
	public int MapYoffset { get; set; }
	public int MapWidth { get; set; }
	public int MapHeight { get; set; }

	private int logMinWidth = 15;
	private int topBarHeigth = 4;

	private int bufferWidth;
	private int bufferHeight;

	private int minWidth => MapXoffset + MapWidth + logMinWidth;
	private int minHeight => MapYoffset + MapHeight + topBarHeigth;
	internal void Initialize()
	{
		Console.CursorVisible = false;
		bufferWidth = Console.WindowWidth;
		bufferHeight = Console.WindowHeight;
		if (bufferWidth < minWidth || bufferHeight < minHeight)
		{
			WindowResize();
		}
		Console.BufferHeight = bufferHeight;
		Console.BufferWidth = bufferWidth;
	}
	internal void Render()
	{
		CheckConsoleBounds();
		while (_updateQueue.TryDequeue(out var data))
		{
			var (pos, gfx) = data;
			Console.SetCursorPosition(pos.x, pos.y);
			Console.ForegroundColor = gfx.fg;
			Console.BackgroundColor = gfx.bg;
			Console.Write(gfx.c);
		}
	}
	internal void SetMapCoordinates(int mapStartTop, int mapStartLeft, int height, int width)
	{
		MapYoffset = mapStartTop;
		MapXoffset = mapStartLeft;
		MapHeight = height;
		MapWidth = width;
	}

	internal void AddMapUpdate(((int y, int x) pos, (char c, ConsoleColor fg, ConsoleColor bg) gfx) renderData)
	{
		_updateQueue.Append(renderData);
	}
	internal void AddMapUpdate(Dictionary<(int y, int x), (char c, ConsoleColor fg, ConsoleColor bg)> renderData)
	{
		foreach (var item in renderData)
		{
			_updateQueue.Enqueue((item.Key, item.Value));
		}
	}

	private void CheckConsoleBounds()
	{
		if (Console.WindowWidth != bufferWidth || Console.WindowHeight != bufferHeight)
		{
			Console.Clear();
			InputHandler.Instance.Stop();
			PauseMessage("Window is being resized");
			bufferWidth = Console.WindowWidth;
			bufferHeight = Console.WindowHeight;
			WindowResize();
			Console.Clear();
			InputHandler.Instance.Start();
		}
	}

	private void WindowResize()
	{
		Console.Clear();

		while (bufferWidth < minWidth || bufferHeight < minHeight)
		{
			PauseMessage($"Window size is too small: w:{bufferWidth}/min:{minWidth}, h:{bufferHeight}/min:{minHeight}");

			bufferWidth = Console.WindowWidth;
			bufferHeight = Console.WindowHeight;
		}
	}

	private void PauseMessage(string reason)
	{
		int width = Console.WindowWidth;
		int height = Console.WindowHeight;
		//Console.SetBufferSize(width, height);
		string pause = "GAME IS PAUSED";
		Console.SetCursorPosition(Math.Max((width - pause.Length) / 2, 0), height / 2 - 1);
		Console.Write(pause);
		Console.SetCursorPosition(Math.Max((width - reason.Length) / 2,0), height / 2 + 1);
		Console.Write(reason);
		string resume = "Press any key to resume";
		Console.SetCursorPosition(Math.Max((width - resume.Length) / 2, 0), height / 2 + 3);
		Console.Write(resume);

		Console.ReadKey(true);
	}
}
