using Labb2_CsProg_ITHS.NET.Files;
using Labb2_CsProg_ITHS.NET.Game;

namespace Labb2_CsProg_ITHS.NET;

internal class Program
{
	static void Main(string[] args)
	{
		ResetConsoleColors();
		var game = new GameLoop(1, null);
		game.GameStart();


	}
	public static void ResetConsoleColors()
	{
		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.White;
	}
}
