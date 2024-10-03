using Labb2_CsProg_ITHS.NET.Files;

namespace Labb2_CsProg_ITHS.NET;

internal class Program
{
	static void Main(string[] args)
	{
		var level = LevelReader.ReadLevel(".\\Levels\\Level1.txt");

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
