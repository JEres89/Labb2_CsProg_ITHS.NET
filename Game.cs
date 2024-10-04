using Labb2_CsProg_ITHS.NET.Elements;
using Labb2_CsProg_ITHS.NET.Files;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET;
internal class Game
{
	internal static Game Instance { get; private set; }
	internal Renderer Renderer { get; private set; }

	private int _levelStart;
	internal Level CurrentLevel { get; private set; }
	internal InputHandler input = InputHandler.Instance;

	public Game(int levelStart, string? player)
	{
		Instance = this;
		Renderer = Renderer.Instance;
		_levelStart = levelStart;
		CurrentLevel = LevelReader.GetLevel(levelStart).Result;
		CurrentLevel.Player.SetName(player??string.Empty);
	}

	public void GameStart()
	{
		Initialize();

		var inputTask = Task.Run(input.Start);
		GameLoop();
	}

	// PlayerEntity details, generate level and elements etc
	private void Initialize()
	{
		if (CurrentLevel.Player.Name == string.Empty)
		{
			string? name = null;
			while (string.IsNullOrEmpty(name))
			{
				Console.Write("Please enter your name: ");
				name = Console.ReadLine();
			}
			CurrentLevel.Player.SetName(name);
			Console.Clear();
		}
		Renderer.SetMapCoordinates(5, 2, CurrentLevel.Height, CurrentLevel.Width);
		Renderer.Initialize();
		CurrentLevel.InitMap();
	}

	private void GameLoop()
	{
		int tickTime = 100;
		Stopwatch tickTimer = new();
		while (true)
		{
			tickTimer.Restart();

			Update();

			Render();

			int timeLeft = tickTime - ((int)tickTimer.ElapsedMilliseconds);
			if (timeLeft > 0)
				Thread.Sleep(timeLeft);
		}
	}

	private void Update()
	{

	}

	private void Render()
	{
		Renderer.Render();
	}
}
