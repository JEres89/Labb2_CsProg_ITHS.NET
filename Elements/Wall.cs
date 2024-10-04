using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Elements;

internal class Wall : LevelElement
{

	internal Wall(Position p, char symbol)
	{
		Pos = p;
		Symbol = symbol;
		Name = "Wall";
		Description = "A solid stone wall.";
	}

	internal override Interaction Collide(LevelElement element)
	{
		return Interaction.Block;
	}

	internal override void Update(Level CurrentLevel)
	{
		throw new NotImplementedException();
	}
	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{
		char c = isDiscovered | isInView ? Symbol : ' ';
		ConsoleColor fg = isInView ? ConsoleColor.Black : ConsoleColor.Gray;
		ConsoleColor bg = isDiscovered ? isInView ? ConsoleColor.Gray : ConsoleColor.DarkGray : ConsoleColor.Black;

		return (c,fg,bg);
	}
}
