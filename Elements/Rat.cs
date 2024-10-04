using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal class Rat : LevelEntity
{
	public Rat(Position p, char symbol) : base(p, symbol)
	{
		Name = "Rat";
		Description = "A ragged, oversized, rabid rat.";
		ViewRange = 2;
	}

	internal override Interaction Collide(LevelElement element)
	{
		throw new NotImplementedException();
	}

	internal override Allegiance GetAllegiance(LevelEntity entity)
	{
		throw new NotImplementedException();
	}

	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{

		return (
			c: isInView ? Symbol:' ', 
			fg: isInView ? ConsoleColor.Red : ConsoleColor.White, 
			bg: isDiscovered ? isInView ? ConsoleColor.Black : ConsoleColor.DarkGray : ConsoleColor.Black);
	}

	internal override void Update(Level CurrentLevel)
	{
		throw new NotImplementedException();
	}
}
