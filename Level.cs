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
	private HashSet<Position> _playerView = new();
	private List<LevelEntity> _enemies;
	private List<Position> _updateQueue = new();
	//private Queue<((int y, int x), (char c, ConsoleColor fg, ConsoleColor bg))> _renderQueue = new();
	//private Queue<(Position, (char c, ConsoleColor fg, ConsoleColor bg))> _renderQueuePos = new();

	internal Level(ReadOnlySpan2D<LevelElement?> levelData, List<LevelEntity> enemies, PlayerEntity player)
	{
		
		Width = levelData.Width;
		Height = levelData.Height;
		_elements = levelData.ToArray();
		_discovered = new bool[Height, Width];
		_enemies = enemies;

		Player = player;

		//ParseLevel(levelData);
	}

	internal void InitMap()
	{
		UpdateDiscoveredAndPlayerView(false);
		RenderAll();
		//FlushToRenderer();
	}
	private void UpdateDiscoveredAndPlayerView(bool render)
	{
		var viewRange = Player.ViewRange;
		var pPos = Player.Pos;

		//if(render)
		//{
		//	var toRemove = new List<Position>();
		//	foreach (var vPos in _playerView)
		//	{
		//		if (pPos.Distance(vPos) <= viewRange)
		//		{
		//			continue;
		//		}
		//		_renderQueue.Enqueue((vPos.ToTuple(),_elements[vPos.Y, vPos.X]?.GetRenderData(true, false) ?? LevelElement.GetEmptyRenderData(true, false)));
		//		_renderQueuePos.Enqueue((vPos, _elements[vPos.Y, vPos.X]?.GetRenderData(true, false) ?? LevelElement.GetEmptyRenderData(true, false)));
		//		toRemove.Add(vPos);
		//	}
		//	foreach (var item in toRemove)
		//	{
		//		_playerView.Remove(item);
		//	}
		//}
		//else
		//{
		//	_playerView.Clear();
		//}
		var toRemove = _playerView;
		_playerView = new();

		for (int y = Math.Max((pPos.Y - viewRange), 0); y <= Math.Min((pPos.Y + viewRange), Height); y++)
		{
			for (int x = Math.Max((pPos.X - viewRange), 0); x <= Math.Min((pPos.X + viewRange), Width); x++)
			{
				//bool inView = false;
				LevelElement? levelElement = _elements[y, x];
				Position vPos = levelElement?.Pos ?? new(y,x);
				
				if (render && toRemove.Contains(vPos))
				{
					toRemove.Remove(vPos);
					_playerView.Add(vPos);
					continue;
				}
				if (pPos.Distance(vPos) <= viewRange)
				{
					_playerView.Add(vPos);
					_discovered[y, x] = true;
					if (render)
					{
						//_renderQueue.Enqueue((vPos.ToTuple(), levelElement?.GetRenderData(true, true) ?? LevelElement.GetEmptyRenderData(true, true)));
						Renderer.Instance.AddMapUpdate((vPos, levelElement?.GetRenderData(true, true) ?? LevelElement.GetEmptyRenderData(true, true)));
					}
				}
			}
		}
		if (render)
		{
			foreach (var dPos in toRemove)
			{
				//_renderQueue.Enqueue((dPos.ToTuple(), _elements[dPos.Y, dPos.X]?.GetRenderData(true, false) ?? LevelElement.GetEmptyRenderData(true, false)));
				Renderer.Instance.AddMapUpdate((dPos, _elements[dPos.Y, dPos.X]?.GetRenderData(true, false) ?? LevelElement.GetEmptyRenderData(true, false)));
			}
		}


		//for (int y = Math.Max((pPos.Y - viewRange), 0); y <= Math.Min((pPos.Y + viewRange), Height); y++)
		//{
		//	for (int x = Math.Max((pPos.X - viewRange), 0); x <= Math.Min((pPos.X + viewRange), Width); x++)
		//	{
		//		bool inView = false;
		//		LevelElement? levelElement = _elements[y, x];
		//		Position? vPos = levelElement?.Pos ?? (render ? _playerView.FirstOrDefault(p => inView = (p.Y == y && p.X == x)) : null);
		//		if (inView)
		//		{
		//			continue;
		//		}
		//		if (pPos.Distance(y, x) <= viewRange)
		//		{
		//			vPos ??= new Position(y, x);

		//			_playerView.Add(vPos);
		//			_discovered[y, x] = true;
		//			if (render)
		//			{
		//				_renderQueue.Enqueue((vPos.ToTuple(), levelElement?.GetRenderData(true, false) ?? LevelElement.GetEmptyRenderData(true, false)));
		//				_renderQueuePos.Enqueue((vPos, levelElement?.GetRenderData(true, false) ?? LevelElement.GetEmptyRenderData(true, false)));
		//			}
		//		}
		//	}
		//}
	}
	internal void ReRender()
	{
		UpdateDiscoveredAndPlayerView(false);
		RenderAll();
	}
	internal void RenderAll()
	{
		//_renderQueue.Clear();
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				if (_discovered[y,x])
				{
					var e = _elements[y,x];

					if (e != null)
					{
						//_renderQueue.Enqueue(((y, x), e.GetRenderData(true, _playerView.Contains(e.Pos))));
						Renderer.Instance.AddMapUpdate((e.Pos, e.GetRenderData(true, _playerView.Contains(e.Pos))));

					}
					else
					{
						Position pos = new(y, x);
						//_renderQueue.Enqueue(((y, x), LevelElement.GetEmptyRenderData(true, _playerView.Contains(e.Pos))));
						Renderer.Instance.AddMapUpdate((pos, LevelElement.GetEmptyRenderData(true, _playerView.Contains(pos))));
					}
				}
			}
		}
	}
	//private void FlushToRenderer()
	//{
	//	Renderer.Instance.AddMapUpdate(_renderQueuePos);
	//	_renderQueue.Clear();
	//	_renderQueuePos.Clear();
	//}
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
