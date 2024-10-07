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

	private readonly Queue<(Position, (char c, ConsoleColor fg, ConsoleColor bg) gfx)> _mapUpdateQueue = new();
	private readonly Queue<((int y, int x) pos, (string s, ConsoleColor fg, ConsoleColor bg) gfx)> _uiUpdateQueue = new();
	private readonly List<string> _log = new();
	private readonly Queue<string> _logUpdateQueue = new();

	
	public int MapXoffset { get; set; }
	public int MapYoffset { get ; set; }
	public int MapWidth { get; set; }
	public int MapHeight { get; set; }
    //public int LogLength { get; set; }
    private int logWidth => bufferWidth - MapXoffset - MapWidth;
	private int logHeight = 0;

    private int logMinWidth = 15;
	private int topBarHeight = 4;

	private int bufferWidth;
	private int bufferHeight;

	private int minWidth => MapXoffset + MapWidth + logMinWidth;
	private int minHeight => MapYoffset + MapHeight + topBarHeight;
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
		while (_mapUpdateQueue.TryDequeue(out var data))
		{
			var (pos, gfx) = data;
			Console.SetCursorPosition(pos.X+MapXoffset, pos.Y+MapYoffset);
			Console.ForegroundColor = gfx.fg;
			Console.BackgroundColor = gfx.bg;
			Console.Write(gfx.c);
		}
		while (_uiUpdateQueue.TryDequeue(out var data))
		{
			var (pos, gfx) = data;
			Console.SetCursorPosition(pos.x, pos.y);
			Console.ForegroundColor = gfx.fg;
			Console.BackgroundColor = gfx.bg;
			Console.Write(gfx.s);
		}

		RenderLog();
	}
	private void RenderLog()
	{
		if (_logUpdateQueue.Count == 0)
		{
			return;
		}
		List<string> logLines = new();
		int logStartX = MapWidth + MapXoffset;
		while (_logUpdateQueue.TryDequeue(out var line))
		{
			_log.Add(line);
			var lineChars = 0;

			while(lineChars < line.Length)
			{
				int takeChars = Math.Min(logWidth, line.Length-lineChars);
				logLines.Add(line.Substring(lineChars, takeChars));
				lineChars += takeChars;
			}
		}
		if(logLines.Count > bufferHeight)
		{
			logLines.RemoveRange(0, logLines.Count - bufferHeight);
		}

		int logOverflow = logHeight + logLines.Count - bufferHeight;
		if (logOverflow > 0)
		{
			Console.MoveBufferArea(logStartX, logOverflow, logWidth, logHeight-logOverflow, logStartX, 0);
			logHeight -= logOverflow;
		}
		Console.ForegroundColor = ConsoleColor.White;
		Console.BackgroundColor = ConsoleColor.Black;
		foreach (var line in logLines)
		{
			Console.SetCursorPosition(logStartX, logHeight);
			Console.Write(line);
			logHeight++;
		}
	}
	internal void SetMapCoordinates(int mapStartTop, int mapStartLeft, int height, int width)
	{
		MapYoffset = Math.Max(mapStartTop, topBarHeight);
		MapXoffset = Math.Max(mapStartLeft, 0);
		MapHeight = height;
		MapWidth = width;
	}

	internal void AddMapUpdate((Position pos, (char c, ConsoleColor fg, ConsoleColor bg) gfx) renderData)
	{
		_mapUpdateQueue.Enqueue(renderData);
	}
	//internal void AddMapUpdate(Queue<(Position, (char c, ConsoleColor fg, ConsoleColor bg))> renderData)
	//{
	//	foreach (var item in renderData)
	//	{
	//		_mapUpdateQueue.Enqueue(item);
	//	}
	//}
	internal void AddUiUpdate(((int y, int x) pos, (string s, ConsoleColor fg, ConsoleColor bg) gfx) renderData)
	{
		_uiUpdateQueue.Enqueue(renderData);
	}
	internal void AddLogLine(string line)
	{
		_logUpdateQueue.Enqueue(line);
	}

	private void CheckConsoleBounds()
	{
		if (Console.WindowWidth != bufferWidth || Console.WindowHeight != bufferHeight)
		{
			Console.Clear();
			PauseMessage("Window is being resized");
			bufferWidth = Console.WindowWidth;
			bufferHeight = Console.WindowHeight;
			WindowResize();
			Console.Clear();
		}
	}

	private void WindowResize()
	{

		while (bufferWidth < minWidth || bufferHeight < minHeight)
		{
			Console.Clear();
			PauseMessage($"Window size is too small: w:{bufferWidth}/min:{minWidth}, h:{bufferHeight}/min:{minHeight}");

			bufferWidth = Console.WindowWidth;
			bufferHeight = Console.WindowHeight;
		}
		Game.Instance.CurrentLevel.ReRender();
		logHeight = 0;
		 _log[Math.Max(_log.Count-bufferHeight, 0)..].ForEach(s => AddLogLine(s));
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
		
		_ = InputHandler.Instance.AwaitNextKey();
	}
}
