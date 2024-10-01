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
	internal static Level ReadLevel(string path)
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

		var stream = new MemoryStream(64);
		var queue = new ConcurrentQueue<byte>();
		//fileStream.CopyToAsync(stream);
		Task t = CopyTest(fileStream, stream);

		return ParseLevel(stream, queue, length, t);
		//int numRead;
		//while ((numRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
		//{
		//	var text = Encoding.Unicode.GetChars(buffer, 0, numRead);

		//}


		//Span2D<char> levelData = new Span2D<char>();

	}
	private static async Task CopyTest(FileStream fileStream, MemoryStream dataStream)
	{
		//await fileStream.CopyToAsync(dataStream);
		int numRead = 0;
		int bytesRead=-1;
		while (bytesRead != 0)
		{
			byte[] buffer = new byte[64];
			bytesRead = await fileStream.ReadAsync(buffer, 0, 64);
			await Task.Delay(100);
			await dataStream.WriteAsync(buffer);
			numRead++;
            Console.WriteLine($"copying to stream {numRead} times");
		}
	}
	private static Level ParseLevel(MemoryStream dataStream, ConcurrentQueue<byte> queue, long length, Task source)
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
		int i=0;
		int delayCount = 0;
		while (!source.IsCompleted || count < dataStream.Capacity)
		{
			i = dataStream.ReadByte();
			if(i == -1)
			{
				delayCount++;
				Task.Delay(10);
				continue;
			}
            Console.WriteLine($"Delayed {delayCount} times.");
			delayCount = 0;
			c = (char)i;
			Console.WriteLine($"copied stream length: {dataStream.Length} count: {count} char: {c}");
			switch (c)
			{
				case '#':
					elements[count] = new Wall(x,y);
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
					if(width > 0) {
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
		if(p == null)
			throw new InvalidDataException("No player in level data");

		Span2D<LevelElement?> grid = new(elements, 0, elements.Length, width - leastEmptyTiles, leastEmptyTiles);
		//LevelElement?[,] grid = Array.;


		return new(grid, enemies, p);

	}
}
