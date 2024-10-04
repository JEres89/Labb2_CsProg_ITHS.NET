using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labb2_CsProg_ITHS.NET.Game;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal class Snake : LevelEntity
{
	public Snake(Position p, char symbol) : base(p, symbol)
	{
		Name = "Snake";
		Description = "A slithering, scary reptile.";
		ViewRange = 1;
	}

	internal override Interaction Collide(LevelElement element)
	{
		throw new NotImplementedException();
	}

	internal override Allegiance GetAllegiance(LevelEntity entity)
	{
		throw new NotImplementedException();
	}

	internal override void Update(Level CurrentLevel)
	{
		if (CurrentLevel.Player.Pos.IsAdjacent(Pos))
		{
			if(!CurrentLevel.TryMove(this, Pos.GetDirection(CurrentLevel.Player.Pos).Invert(), out var collision))
			{
				var result = collision.Collide(this);
				switch (result)
				{
					case Interaction.Move:
						break;
					case Interaction.Block:
						break;
					case Interaction.Aggressive:
					case Interaction.Activate:
					case Interaction.Acquire:
					case Interaction.Trigger:
					case Interaction.Status:
					default:

						break;
				}
			}
		}
	}
	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{

		return (
			c: isInView ? Symbol : ' ',
			fg: isInView ? ConsoleColor.Green : ConsoleColor.White,
			bg: isDiscovered ? isInView ? ConsoleColor.Black : ConsoleColor.DarkGray : ConsoleColor.Black);
	}
}
