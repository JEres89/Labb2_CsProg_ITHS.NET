using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal abstract class LevelEntity : LevelElement
{
	protected Dictionary<Allegiance, Interaction> relationships = new();

    public int ViewRange { get; set; }
    internal LevelEntity(Position p, char symbol)
	{
		Pos = p;
		Symbol = symbol;
	}

	protected Interaction GetReaction(Allegiance allegiance)
	{
		return relationships[allegiance];
	}
	internal abstract Allegiance GetAllegiance(LevelEntity entity);

	internal enum Allegiance
	{
		None,
		Neutral,
		Enemy,
		Friend
	}
}
