using Labb2_CsProg_ITHS.NET.Files;
using Labb2_CsProg_ITHS.NET.Game;

namespace Labb2_CsProg_ITHS.NET;

internal class Program
{
	static void Main(string[] args)
	{
		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.White;
		var game = new Game(1, null);
		game.GameStart();

		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.White;
		//int i = 0;
		//while (!level.IsCompleted)
		//{
		//	i++;
		//          Console.WriteLine($"{DateTime.Now.Ticks}: Doing things while the level is construction {i} times");
		//	Thread.Sleep(100);
		//}

		//Console.WriteLine(level.Result);
	}
}
