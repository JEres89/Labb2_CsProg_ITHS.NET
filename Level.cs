using CommunityToolkit.HighPerformance;
using Labb2_CsProg_ITHS.NET.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET;
internal class Level
{
	public int Width { get; private set; }
	public int Height { get; private set; }

	public PlayerEntity Player { get; private set; }

	private LevelElement?[,] _elements;
	private bool[,] _discovered;
	private Dictionary<(int y,int x),bool> _playerView = new();
	private List<LevelEntity> _enemies;
	private List<Position> _updateQueue = new();
	private Dictionary<(int y, int x), (char c, ConsoleColor fg, ConsoleColor bg)> _renderQueue = new();

	internal Level(ReadOnlySpan2D<LevelElement?> levelData, List<LevelEntity> enemies, PlayerEntity player)
	{
		
		Width = levelData.Width;
		Height = levelData.Height;
		_elements = levelData.ToArray();
		_discovered = new bool[Width, Height];
		_enemies = enemies;

		Player = player;

		//ParseLevel(levelData);
	}

	internal void InitMap()
	{
		UpdateDiscoveredAndPlayerView(false);
		RenderAll();
		FlushToRenderer();
	}
	private void UpdateDiscoveredAndPlayerView(bool render)
	{
		var viewRange = Player.ViewRange;
		var pos = Player.Pos;

		if(render)
		{
			var toRemove = new List<(int y, int x)>();
			foreach (var item in _playerView)
			{
				if (pos.Distance(item.Key.y, item.Key.x) <= viewRange)
				{
					continue;
				}
				_renderQueue[item.Key] = _elements[item.Key.y, item.Key.x]?.GetRenderData(true, false) ?? (' ', ConsoleColor.Black, ConsoleColor.DarkGray);
				toRemove.Add(item.Key);
			}
			foreach (var item in toRemove)
			{
				_playerView.Remove(item);
			}
		}
		else
		{
			_playerView.Clear();
		}

		for (int y = Math.Max((pos.Y - viewRange), 0); y < Math.Min((pos.Y + viewRange), Height); y++)
		{
			for (int x = Math.Max((pos.X - viewRange), 0); x < Math.Min((pos.X + viewRange), Width); x++)
			{
				if(_playerView.ContainsKey((y, x)))
				{
					continue;
				}
				if (pos.Distance(y, x) <= viewRange)
				{
					_playerView[(y, x)] = true;
					_discovered[y, x] = true;
					if (render)
					{
						_renderQueue[(y, x)] = _elements[y, x]?.GetRenderData(true, true) ?? (' ', ConsoleColor.Black, ConsoleColor.Black);
					}
				}
			}
		}
	}

	internal void RenderAll()
	{
		_renderQueue.Clear();
		foreach (var item in _elements) {
			if (item != null && _discovered[item.Pos.Y,item.Pos.X])
			{
				_renderQueue[(item.Pos.Y, item.Pos.X)] = item.GetRenderData(true, _playerView.ContainsKey((item.Pos.Y, item.Pos.X)));
			}
		}
	}
	private void FlushToRenderer()
	{
		Renderer.Instance.AddMapUpdate(_renderQueue);
		_renderQueue.Clear();
	}
	internal void Update()
	{
		List<LevelElement> movedElements= new();
		foreach (var pos in _updateQueue)
		{
			var element = _elements[pos.Y,pos.X];

			if(element != null)
			{
				var newPos = element.Pos;
				if (_elements[newPos.Y, newPos.X] != null)
				{
					if(_elements[newPos.Y, newPos.X]!.Pos.IsEqual(newPos))
					{
						throw new InvalidOperationException("Element tried moving to an occupied position.");
					}
					movedElements.Add(element);
				}
				else
				{
					_elements[newPos.Y, newPos.X] = element;
				}
				_elements[pos.Y, pos.X] = null;


			}
		}
		foreach (var element in movedElements)
		{

		}
	}

	internal bool TryMove(LevelEntity entity, Position direction, [NotNullWhen(false)]out LevelElement? collision)
	{
		throw new NotImplementedException();
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
}
