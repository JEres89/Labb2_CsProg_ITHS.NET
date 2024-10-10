using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb2_CsProg_ITHS.NET.Backend;
using Labb2_CsProg_ITHS.NET.Game;
using static Labb2_CsProg_ITHS.NET.Game.CombatProvider;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal abstract class LevelEntity : LevelElement
{
	public int ViewRange { get; protected set; }
	public int Health { get; protected set; }
    public abstract int MaxHealth { get; }
    public int AttackDieSize { get; protected set; }
	public int AttackDieNum { get; protected set; }
	public int AttackMod { get; protected set; }
	public int DefenseDieSize { get; protected set; }
	public int DefenseDieNum { get; protected set; }
	public int DefenseMod { get; protected set; }

	public bool HasMoved { get; set; }
	public bool IsDead { get => Health <= 0; }

	protected readonly Alignment alignment;
	internal LevelEntity(Position p, char symbol, Alignment alignment)
	{
		Pos = p;
		Symbol = symbol;
		this.alignment = alignment;
	}

	internal Reactions Collide(Alignment opposingAlignment)
	{
		return (alignment, opposingAlignment) switch
		{
			(Alignment.Neutral, Alignment.Neutral)	=> Reactions.Block,
			(Alignment.Neutral, Alignment.Good)		=> Reactions.Block,
			(Alignment.Neutral, Alignment.Evil)		=> Reactions.Aggressive,
			(Alignment.Good,	Alignment.Neutral)	=> Reactions.Block,
			(Alignment.Good,	Alignment.Good)		=> HasMoved ? Reactions.Block : Reactions.Move,
			(Alignment.Good,	Alignment.Evil)		=> Reactions.Aggressive,
			(Alignment.Evil,	Alignment.Neutral)	=> Reactions.Aggressive,
			(Alignment.Evil,	Alignment.Good)		=> Reactions.Aggressive,
			(Alignment.Evil,	Alignment.Evil)		=> HasMoved ? Reactions.Block : Reactions.Move,
			_ => Reactions.Block
		};
	}

	protected virtual bool ActsIfCollide(LevelElement element, out Reactions reaction)
	{
		switch (element)
		{
			case LevelEntity entity:
				reaction = entity.Collide(alignment);
				return true;
			default:
				reaction = Reactions.Block;
				return false;
		}
	}
	protected bool Act(Level currentLevel, Position direction)
	{
		LevelElement? collisionTarget;
		if (currentLevel.TryMove(this, direction, out collisionTarget))
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
						BlockMove(currentLevel, collisionTarget);
						break;

					case Reactions.Aggressive:
						AttackEnemy(currentLevel, collisionTarget);
						break;

					case Reactions.Move:
						PushFriend(currentLevel, collisionTarget, direction);
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
	protected virtual void BlockMove(Level currentLevel, LevelElement collisionTarget)
	{

	}
	protected virtual void AttackEnemy(Level currentLevel, LevelElement collisionTarget)
	{
		if (collisionTarget is LevelEntity enemy)
		{
			var attack = Attack(this, enemy);
			var counter = enemy.AttackedBy(this, attack);
			currentLevel.Renderer.AddLogLine(attack.GenerateCombatMessage());
			if (counter.defender == this)
			{
				Health -= counter.damage;
				currentLevel.Renderer.AddLogLine(counter.GenerateCombatMessage());
			}
		}
	}
	protected virtual void PushFriend(Level currentLevel, LevelElement collisionTarget, Position direction)
	{
		if (collisionTarget is LevelEntity friend)
		{
			// To avoid circular movement lock, we need to set HasMoved to true before updating the friend.
			HasMoved = true;
			if (friend.UpdateMove(currentLevel, direction.Invert()))
			{
				if(currentLevel.TryMove(this, direction, out var newCollision))
				{
					Pos = Pos.Move(direction);
				}
				else
				{
					throw new Exception("Target position just moved, nothing should be there.");
				}
			}
		}
	}


	internal enum Alignment
	{
		None,
		Neutral,
		Evil,
		Good
	}

	internal CombatResult AttackedBy(LevelEntity attacker, CombatResult attackResult)
	{
		Health -= attackResult.damage;
		if(Health <= 0)
		{
			HasMoved = true;
			return attackResult;
		}

		return Attack(this, attacker);
	}

	internal void Loot(Level currentLevel, LevelEntity entity)
	{
		if(entity is PlayerEntity player)
		{
			currentLevel.Renderer.AddLogLine($"You step in the remains of a fallen {Name}.");
			// Give any loot to the player
		}
		else
		{
			if (currentLevel.IsInview(Pos))
			{
				currentLevel.Renderer.AddLogLine($"You see {entity.Description} consume a fallen {Name} whole, absorbing its power.");
			}
			else
			{
				currentLevel.Renderer.AddLogLine($"You hear a faint crushing of bones and ripping of flesh, somewhere something is having a snack...");

			}
			entity.Consume(this);
		}
	}
	protected virtual void Consume(LevelEntity entity)
	{
		Health += entity.MaxHealth;
		AttackDieSize++;
		AttackMod++;
		DefenseDieSize++;
		DefenseMod++;
	}
	internal virtual bool UpdateMove(Level currentLevel, Position from)
	{
		Position start = Pos;
		List<Position> excludedDir = new List<Position>(4);
		excludedDir.Add(from);
		while (!HasMoved)
		{
			var moveDirection = Position.GetRandomDirection(excludedDir);
			if (Act(currentLevel, moveDirection))
			{
				HasMoved = true;
			}
			else
			{
				excludedDir.Add(moveDirection);
			}
		}

		return Pos != start;
	}

}
