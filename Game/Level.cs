using CommunityToolkit.HighPerformance;
using Labb2_CsProg_ITHS.NET.Backend;
using Labb2_CsProg_ITHS.NET.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Game;
internal class Level
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public PlayerEntity Player { get; private set; }

    private LevelElement?[,] _elements;
    private bool[,] _discovered;
    private HashSet<Position> _playerView = new();
    private List<LevelEntity> _enemies;
    private HashSet<Position> _renderUpdateCoordinates = new();

    internal Level(ReadOnlySpan2D<LevelElement?> levelData, List<LevelEntity> enemies, PlayerEntity player)
    {

        Width = levelData.Width;
        Height = levelData.Height;
        _elements = levelData.ToArray();
        _discovered = new bool[Height, Width];
        _enemies = enemies;

        Player = player;
    }

    internal void InitMap()
    {
        UpdateDiscoveredAndPlayerView(false);
        UpdateRendererAll();
    }
    private void UpdateDiscoveredAndPlayerView(bool render)
    {
        var viewRange = Player.ViewRange;
        var pPos = Player.Pos;

        var outOfView = _playerView;
        _playerView = new();

        for (int y = Math.Max(pPos.Y - viewRange, 0); y <= Math.Min(pPos.Y + viewRange, Height); y++)
        {
            for (int x = Math.Max(pPos.X - viewRange, 0); x <= Math.Min(pPos.X + viewRange, Width); x++)
            {
                LevelElement? levelElement = _elements[y, x];
                Position vPos = levelElement?.Pos ?? new(y, x);

                outOfView.Remove(vPos);

                if (pPos.Distance(vPos) <= viewRange)
                {
                    _playerView.Add(vPos);
                    _discovered[y, x] = true;
                }
                if (render)
                {
                    _renderUpdateCoordinates.Add(vPos);
                }
            }
        }
		if (render)
		{
			foreach (var dPos in outOfView)
			{
				_renderUpdateCoordinates.Add(dPos);
			}
		}
	}
    internal void ReRender()
    {
        UpdateDiscoveredAndPlayerView(false);
        UpdateRendererAll();
    }
    internal void UpdateRendererAll()
    {
        _renderUpdateCoordinates.Clear();
        //_renderQueue.Clear();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                RenderPosition(y, x);
            }
        }
    }
    internal void UpdateRenderer()
    {
        foreach (var pos in _renderUpdateCoordinates)
        {
            RenderPosition(pos.Y, pos.X);
        }
        _renderUpdateCoordinates.Clear();
    }

    private void RenderPosition(int y, int x)
    {
        if (_discovered[y, x])
        {
            var e = _elements[y, x];
            if (e != null)
            {
                Renderer.Instance.AddMapUpdate((e.Pos, e.GetRenderData(true, _playerView.Contains(e.Pos))));
            }
            else
            {
                Position pos = new(y, x);
                Renderer.Instance.AddMapUpdate((pos, LevelElement.GetEmptyRenderData(true, _playerView.Contains(pos))));
            }
        }
    }

    internal void Update()
    {
        if (Player.WillAct)
        {
            Player.Update(this);
            // TODO: update player on collidingEntity map
        }
        List<LevelElement> movedElements = new();

        if (Player.HasMoved)
        {
            UpdateDiscoveredAndPlayerView(true);
            Player.HasMoved = false;
            // update elements that are moving
        }
    }

    internal bool TryMove(LevelEntity movingEntity, Position direction, [NotNullWhen(false)] out LevelElement? collision)
    {
        var (y, x) = movingEntity.Pos;
        collision = _elements[y + direction.Y, x + direction.X];
        if (collision != null)//is LevelEntity collidingEntity)
        {
            //bool willMove = movingEntity.ActsIfCollide(collidingEntity) != LevelElement.Reactions.Move;

            //if (willMove)
            //{
            //	//collidingEntity.;
            //	MoveElement(movingEntity.Pos, movingEntity.Pos.Move(direction));
            //	collision = null;
            //	return true;
            //}

            return false;
        }
        //else if (collision is LevelElement collidingElement)
        //{
        //	return false;
        //}
        else
        {
            MoveElement(movingEntity.Pos, movingEntity.Pos.Move(direction));
            collision = null;
            return true;
        }
    }
    private void MoveElement(Position from, Position to)
    {
        _elements[to.Y, to.X] = _elements[from.Y, from.X];
        _elements[from.Y, from.X] = null;
        _renderUpdateCoordinates.Add(from);
        _renderUpdateCoordinates.Add(to);
    }
    public override string ToString()
    {
        StringBuilder sb = new();
        var span = _elements.AsSpan2D();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {

                sb.Append(span[y, x]);
            }
            //sb.Append('|');
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
