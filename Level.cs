using CommunityToolkit.HighPerformance;
using Labb2_CsProg_ITHS.NET.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET;
internal class Level
{
	public int Width { get; private set; }
	public int Height { get; private set; }

	public Player Player { get; private set; }

	private LevelElement?[,] _elements;
	private LevelElement?[,] _discovered;
	private List<LevelEntity> _enemies;

	internal Level(ReadOnlySpan2D<LevelElement?> levelData, List<LevelEntity> enemies, Player player)
	{
		Width = levelData.Width;
		Height = levelData.Height;
		_elements = levelData.ToArray();
		_discovered = new LevelElement?[Width, Height];
		_enemies = enemies;

		Player = player;

		//ParseLevel(levelData);
	}

	public override string ToString()
	{
		StringBuilder sb = new();
		var span = _elements.AsSpan2D();

		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{

				sb.Append(span[y,x]);
			}
			//sb.Append('|');
			sb.AppendLine();
		}
		return sb.ToString();
	}


	//private void ParseLevel(ReadOnlySpan2D<char> levelData)
	//{
	//	for (int x = 0; x < Width; x++)
	//	{
	//		for (int y = 0; y < Height; y++)
	//		{
	//			switch (levelData[x, y])
	//			{
	//				case '#':
	//					_staticElements[x, y] = new Wall();
	//					break;
	//				case ' ':
	//					continue;
	//				case '@':
	//					_actorElements[x, y] = new Player(x, y);
	//					break;
	//				//case 'E':
	//				//	_staticElements[x, y] = new Exit();
	//				//	break;
	//				case 'r':
	//					_actorElements[x, y] = new Rat(x, y);
	//					break;
	//				case 's':
	//					_actorElements[x, y] = new Snake(x, y);
	//					break;
	//				default:
	//					continue;
	//			}
	//		}
	//	}
	//}
}
