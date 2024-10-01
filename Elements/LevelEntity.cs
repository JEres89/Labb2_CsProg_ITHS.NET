using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal abstract class LevelEntity : LevelElement
{
	public int X { get; private set; }
	public int Y { get; private set; }

	internal LevelEntity(int x, int y)
	{
		X = x;
		Y = y;
	}

}
