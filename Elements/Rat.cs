using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb2_CsProg_ITHS.NET.Game;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal class Rat : LevelEntity
{
	public const int RatHealth = 20;

	public const int RatAttackDieSize = 8;
	public const int RatAttackDieNum = 1;
	public const int RatAttackMod = 0;

	public const int RatDefenseDieSize = 4;
	public const int RatDefenseDieNum = 1;
	public const int RatDefenseMod = 0;

	public override int MaxHealth => RatHealth;

	public Rat(Position p, char symbol) : base(p, symbol, Alignment.Evil)
	{
		Name = "Rat";
		Description = "A ragged, oversized, rabid rat.";
		ViewRange = 2;
		Health = RatHealth;
		AttackDieSize = RatAttackDieSize;
		AttackDieNum = RatAttackDieNum;
		AttackMod = RatAttackMod;
		DefenseDieSize = RatDefenseDieSize;
		DefenseDieNum = RatDefenseDieNum;
		DefenseMod = RatDefenseMod;
	}

	internal override void Update(Level currentLevel)
	{
		Position direction;
		if (currentLevel.Player.Pos.Distance(Pos) <= ViewRange)
		{
			direction = Pos.GetDirectionUnit(currentLevel.Player.Pos);
		}
		else
		{
			direction = Position.GetRandomDirection();
		}
		HasMoved = Act(currentLevel, direction);
	}

	internal override bool UpdateMove(Level currentLevel, Position from)
	{
		Position start = Pos;

		if (currentLevel.Player.Pos.Distance(Pos) <= ViewRange)
		{
			if (Act(currentLevel, Pos.GetDirectionUnit(currentLevel.Player.Pos)))
			{
				HasMoved = true;
			}
		}

		if (!HasMoved)
		{
			return base.UpdateMove(currentLevel, from);
		}
		return Pos != start;
	}

	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{

		char c = isInView ? IsDead ? '¤' : Symbol : ' ';
		ConsoleColor fg = isInView ? ForegroundVisibleRat : DiscoveredRat;
		ConsoleColor bg = isInView ? BackroundVisibleRat : DiscoveredRat;

		return (c, fg, bg);
	}

	public static ConsoleColor ForegroundVisibleRat { get; } = ConsoleColor.Red;
	public static ConsoleColor BackroundVisibleRat { get; } = ConsoleColor.Gray;
	public static ConsoleColor DiscoveredRat { get; } = ConsoleColor.DarkGray;
	protected override void Consume(LevelEntity entity)
	{
		if(entity is Rat)
		{
			Health += MaxHealth;
			AttackDieNum++;
			AttackMod+=2;
			DefenseDieSize++;
			Symbol = 'R';
		}
		else base.Consume(entity);
	}
}
