using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb2_CsProg_ITHS.NET.Backend;
using Labb2_CsProg_ITHS.NET.Game;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal class PlayerEntity : LevelEntity, IInputEndpoint
{
	public bool WillAct { get; set; }
	private ConsoleKeyInfo pressedKey;
	public PlayerEntity(Position p, char symbol) : base(p, symbol)
	{
		Name = "";
		Description = "You.";
		ViewRange = 3;
	}
	public void SetName(string name)
	{
		Name = name;
	}


	internal override void Update(Level CurrentLevel)
	{
		WillAct = false;
		Position direction = pressedKey.Key switch
		{
			ConsoleKey.A or ConsoleKey.LeftArrow => Position.Left,
			ConsoleKey.W or ConsoleKey.UpArrow => Position.Up,
			ConsoleKey.D or ConsoleKey.RightArrow => Position.Right,
			ConsoleKey.S or ConsoleKey.DownArrow => Position.Down,
			_ => default
		};

		if (direction == default)
		{
			HasMoved = false;
		}
		else
		{
			HasMoved = Act(CurrentLevel, direction);
		}
		pressedKey = default;
	}

	private bool Act(Level CurrentLevel, Position direction)
	{
		LevelElement? collisionTarget;
		if (CurrentLevel.TryMove(this, direction, out collisionTarget))
		{
			Pos = Pos.Move(direction);
			return true;
		}
		else
		{
			if (ActsIfCollide(collisionTarget, out var reaction))
			{
				switch (reaction)
				{
					case Reactions.Block:
						break;

					case Reactions.Aggressive:
						if (collisionTarget is LevelEntity entity)
						{
							var attack = CombatProvider.Attack(this, entity);
							var counter = entity.Attack(this, attack);
							Health -= counter.damage;
							CurrentLevel.Renderer.AddLogLine(attack.GenerateCombatMessage(this, entity));
							CurrentLevel.Renderer.AddLogLine(counter.GenerateCombatMessage(entity, this));
						}
						break;

					case Reactions.Move:
						break;

					case Reactions.Activate:
						break;

					case Reactions.Acquire:
						break;

					case Reactions.Trigger:
						break;

					case Reactions.Status:
						break;

					default:
						break;
				}
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	protected override bool ActsIfCollide(LevelElement element, out Reactions reaction)
	{
		switch (element)
		{
			case LevelEntity entity:
				reaction = entity.Collide(alignment);
				return reaction != Reactions.Block;

			case Wall wall:
				Health -= 1;
				Renderer.Instance.AddLogLine("You bump your nose into a wall, taking 1 damage.");
				reaction = Reactions.Block;
				return true;

			//case Obstacle obstacle:

			//	break;

			default:
				reaction = Reactions.Block;
				return false;
		}
	}



	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{
		return (Symbol, ConsoleColor.Blue, ConsoleColor.Gray);
	}

	public void KeyPressed(ConsoleKeyInfo key)
	{
		if (WillAct || HasMoved)
			return;
		else
		{
			WillAct = true;
			pressedKey = key;
		}
	}

	public void RegisterKeys(InputHandler handler)
	{
		handler.AddKeyListener(ConsoleKey.W, this);
		handler.AddKeyListener(ConsoleKey.A, this);
		handler.AddKeyListener(ConsoleKey.S, this);
		handler.AddKeyListener(ConsoleKey.D, this);
		handler.AddKeyListener(ConsoleKey.UpArrow, this);
		handler.AddKeyListener(ConsoleKey.LeftArrow, this);
		handler.AddKeyListener(ConsoleKey.DownArrow, this);
		handler.AddKeyListener(ConsoleKey.RightArrow, this);
	}
}
