using CommunityToolkit.HighPerformance;
using Labb2_CsProg_ITHS.NET.Elements;
using Labb2_CsProg_ITHS.NET.Game;
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
	internal static async Task<Level> GetLevel(int level)
	{
		return level switch
		{
			1 => await ReadLevel(".\\Levels\\Level1.txt"),
			2 => await ReadLevel(".\\Levels\\Level2.txt"),
			3 => await ReadLevel(".\\Levels\\Level3.txt"),
			_ => throw new ArgumentException("Invalid level number", nameof(level))
		};
	}
	internal static async Task<Level> ReadLevel(string path)
	{
		// Read the file and create a Level object
		if (!Path.Exists(path))
		{
			var p2 = Path.GetFullPath(path);
			throw new FileNotFoundException("File not found", path);
		}

		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1024, useAsync: true);
		var length = fileStream.Length;

		if (length > 81920) // MaxShadowBufferSize
		{
			var queue = new ConcurrentQueue<byte[]>();
			Task t = QueueFileChunks(fileStream, queue);
			return await ParseLevel(queue, (int)length, t);
		}
		else
		{
			IAsyncEnumerable<byte[]> fileBuffer = EnumerateFile(fileStream);
			return await ParseByEnumeration(fileBuffer, (int)length);
		}
	}

	private static async IAsyncEnumerable<byte[]> EnumerateFile(FileStream fileStream)
	{
		byte[] buffer = new byte[255];
		int bytesRead;
		while ((bytesRead = await fileStream.ReadAsync(buffer, 0, 255)) != 0)
		{
			yield return buffer;
			buffer = new byte[255];
			//Console.WriteLine("Fileposition: " + fileStream.Position);
		}

	}
	private static async Task<Level> ParseByEnumeration(IAsyncEnumerable<byte[]> stream, int length)
	{
		LevelElement?[] elements = new LevelElement?[length];
		List<LevelEntity> enemies = new();

		PlayerEntity? p = null;
		int y = 0;
		int x = 0;
		int width = 0;
		int emptyTiles = 0;
		int leastEmptyTiles = int.MaxValue;
		int emptyRows = 0;
		int count = 0;

		while (count < length)
		{
			await foreach (var chunk in stream)
			{
				ParseChunk(chunk, length, elements, enemies, ref p, ref y, ref x, ref width, ref emptyTiles, ref leastEmptyTiles, ref emptyRows, ref count);
			}
		}

		if (p == null)
			throw new InvalidDataException("No player in level data");

		return new(new(elements, 0, y - emptyRows, width - leastEmptyTiles, leastEmptyTiles), enemies, p);
	}

	private static void ParseChunk(byte[] chunk, int length, LevelElement?[] elements, List<LevelEntity> enemies, ref PlayerEntity? p, ref int y, ref int x, ref int width, ref int emptyTiles, ref int leastEmptyTiles, ref int emptyRows, ref int count)
	{
		var span = chunk.AsSpan();
		for (int i = 0; i < span.Length; i++)
		{
			if (count >= length) break;
			char c = (char)span[i];
			switch (c)
			{
				case '#':
					elements[count] = new Wall(new(y, x), c);
					break;
				case '@':
					elements[count] = p = new PlayerEntity(new(y, x), c);
					break;
				//case 'E':
				//	elements[count] = new Exit(new(y, x), c);
				//	break;
				case 'r':
					var r = new Rat(new(y, x), c);
					enemies.Add(r);
					elements[count] = r;
					break;
				case 's':
					var s = new Snake(new(y, x), c);
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
					if (emptyTiles == width)
					{
						emptyRows++;
					}
					else
					{
						leastEmptyTiles = Math.Min(leastEmptyTiles, emptyTiles);
						emptyRows = 0;
					}
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

	private static async Task QueueFileChunks(FileStream fileStream, ConcurrentQueue<byte[]> queue)
	{
		int numRead = 0;
		int bytesRead = -1;
		while (bytesRead != 0)
		{
			fileStream.ReadByte();
			byte[] buffer = new byte[1024];
			bytesRead = await fileStream.ReadAsync(buffer, 0, 1024);
			numRead++;
			//Console.WriteLine($"{DateTime.Now.Ticks}: copying to stream {numRead} times");
			queue.Enqueue(buffer);
			//await Task.Delay(10);
		}
	}
	private static async Task<Level> ParseLevel(ConcurrentQueue<byte[]> queue, int length, Task source)
	{
		LevelElement?[] elements = new LevelElement?[length];
		List<LevelEntity> enemies = new();

		PlayerEntity? p = null;
		int y = 0;
		int x = 0;
		int width = 0;
		int emptyTiles = 0;
		int leastEmptyTiles = int.MaxValue;
		int emptyRows = 0;
		int count = 0;
		int delayCount = 0;

		while (!source.IsCompleted || queue.Count > 0)
		{
			if (!queue.TryDequeue(out var chunk))
			{
				delayCount++;
				await Task.Delay(10);
				//Console.WriteLine($"{DateTime.Now.Ticks}: Delayed {delayCount} times.");
				continue;
			}
			delayCount = 0;

			ParseChunk(chunk, length, elements, enemies, ref p, ref y, ref x, ref width, ref emptyTiles, ref leastEmptyTiles, ref emptyRows, ref count);

			//Console.WriteLine($"{DateTime.Now.Ticks}: Processed queue count: {count}");
		}

		if (p == null)
			throw new InvalidDataException("No player in level data");

		return new(new(elements, 0, y - emptyRows, width - leastEmptyTiles, leastEmptyTiles), enemies, p);

	}
}
