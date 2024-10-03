using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Elements;

internal class Wall : LevelElement
{

	internal Wall(int x, int y, char symbol)
	{
		X = x;
		Y = y;
		Symbol = symbol;
	}
}
