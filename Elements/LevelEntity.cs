using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labb2_CsProg_ITHS.NET.CombatProvider;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal abstract class LevelEntity : LevelElement
{
    public int ViewRange { get; protected set; }
    public int Health { get; protected set; }
    public int AttackDieSize { get; protected set; }
    public int AttackDieNum { get; protected set; }
    public int AttackMod { get; protected set; }
    public int DefenseDieSize { get; protected set; }
    public int DefenseDieNum { get; protected set; }
    public int DefenseMod { get; protected set; }

    public bool HasMoved { get; set; }
    protected readonly Alignment alignment;
    internal LevelEntity(Position p, char symbol)
	{
		Pos = p;
		Symbol = symbol;
	}

	internal Reactions Collide(Alignment opposingAlignment)
	{
		return (alignment, opposingAlignment) switch
		{
			(Alignment.Neutral, Alignment.Neutral)	=> Reactions.Block,
			(Alignment.Neutral, Alignment.Good)		=> Reactions.Block,
			(Alignment.Neutral, Alignment.Evil)		=> Reactions.Aggressive,
			(Alignment.Good,	Alignment.Neutral)	=> Reactions.Block,
			(Alignment.Good,	Alignment.Good)		=> Reactions.Move,
			(Alignment.Good,	Alignment.Evil)		=> Reactions.Aggressive,
			(Alignment.Evil,	Alignment.Neutral)	=> Reactions.Aggressive,
			(Alignment.Evil,	Alignment.Good)		=> Reactions.Aggressive,
			(Alignment.Evil,	Alignment.Evil)		=> Reactions.Move,
			_ => Reactions.Block
		};
	}
	//internal abstract Alignment GetAllegiance(LevelEntity entity);

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
	internal enum Alignment
	{
		None,
		Neutral,
		Evil,
		Good
	}

	internal CombatResult Attack(LevelEntity attacker, CombatResult attackResult)
	{
		Health -= attackResult.damage;

		return CombatProvider.Attack(this, attacker);
	}
}
