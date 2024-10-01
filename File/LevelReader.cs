using CommunityToolkit.HighPerformance;
using Labb2_CsProg_ITHS.NET.Elements;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Labb2_CsProg_ITHS.NET.Files;
internal static class LevelReader
{
	internal static async Task<Level> ReadLevel(string path)
	{
		// Read the file and create a Level object
		if (!Path.Exists(path))
		{
			var p2 = Path.GetFullPath(path);
			throw new FileNotFoundException("File not found", path);
		}

		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
			bufferSize: 1024, useAsync: true);
		var length = fileStream.Length;
		//byte[] buffer = new byte[1024];

		//var stream = new MemoryStream(64);
		var queue = new ConcurrentQueue<byte[]>();
		//fileStream.CopyToAsync(stream);
		Task t = CopyTest(fileStream, queue);

		return await ParseLevel(queue, length, t);
		//int numRead;
		//while ((numRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
		//{
		//	var text = Encoding.Unicode.GetChars(buffer, 0, numRead);

		//}


		//Span2D<char> levelData = new Span2D<char>();

	}
	private static async Task CopyTest(FileStream fileStream, ConcurrentQueue<byte[]> queue)
	{
		//await fileStream.CopyToAsync(dataStream);
		int numRead = 0;
		int bytesRead=-1;
		while (bytesRead != 0)
		{
			byte[] buffer = new byte[1024];
			bytesRead = await fileStream.ReadAsync(buffer, 0, 1024);
			numRead++;
            Console.WriteLine($"{DateTime.Now.Ticks}: copying to stream {numRead} times");
			queue.Enqueue(buffer);
			//await Task.Delay(10);
		}
	}
	private static async Task<Level> ParseLevel(ConcurrentQueue<byte[]> queue, long length, Task source)
	{
		LevelElement?[] elements = new LevelElement?[length];
		List<LevelEntity> enemies = new();
		Player? p = null;
		int y = 0;
		int x = 0;
		int width = 0;
		char c;
		int emptyTiles = 0;
		int leastEmptyTiles = int.MaxValue;
		int count = 0;
		int delayCount = 0;

		while (!source.IsCompleted || queue.Count > 0)
		{
			if(!queue.TryDequeue(out var nextChunk))
			{

				delayCount++;
				Console.WriteLine($"{DateTime.Now.Ticks}: Delayed {delayCount} times.");
				//await Task.Delay(10);
				continue;
			}
			delayCount = 0;
			
			ParseChunk(nextChunk);

			Console.WriteLine($"{DateTime.Now.Ticks}: Processed queue count: {count}");
		}
		if(p == null)
			throw new InvalidDataException("No player in level data");

		//Span2D<LevelElement?> grid = new(elements, 0, elements.Length, width - leastEmptyTiles, leastEmptyTiles);
		//LevelElement?[,] grid = Array.;


		return new(new(elements, 0, elements.Length, width - leastEmptyTiles, leastEmptyTiles), enemies, p);

		void ParseChunk(byte[] chunk)
		{
			var span = chunk.AsSpan();
			for (int i = 0; i < span.Length; i++)
			{
				if (count >= length) break;
				c = (char)span[i];
				switch (c)
				{
					case '#':
						elements[count] = new Wall(x, y);
						break;
					case '@':
						elements[count] = p = new Player(x, y);
						break;
					//case 'E':
					//	_staticElements[x, y] = new Exit();
					//	break;
					case 'r':
						var r = new Rat(x, y);
						enemies.Add(r);
						elements[count] = r;
						break;
					case 's':
						var s = new Snake(x, y);
						enemies.Add(s);
						elements[count] = s;
						break;
					case '\n':
						y++;
						if (width > 0)
						{
							if (width != x)
								throw new InvalidDataException("Invalid level data");
						}
						else
						{
							width = x;
						}
						x = 0;
						leastEmptyTiles = Math.Min(leastEmptyTiles, emptyTiles);
						emptyTiles = 0;
						continue;
					case ' ':
					default:
						emptyTiles++;
						elements[count] = null;
						x++;
						count++;
						continue;
				}
				emptyTiles = 0;
				x++;
				count++;
			}
		}
	}
}
