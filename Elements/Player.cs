using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labb2_CsProg_ITHS.NET.Elements.LevelEntity;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal class Player : LevelEntity
{
	public Player(int x, int y, char symbol) : base(x, y, symbol)
	{

	}

	internal override Interaction Collide(LevelElement element)
	{
		switch (element)
		{
			case LevelEntity entity:
				var al = entity.GetAllegiance(this);
				return relationships[al];
			case Wall wall:
				return Interaction.Move;
			default:
				return Interaction.Block;
		}
	}

	
	internal override Allegiance GetAllegiance(LevelEntity entity)
	{
		throw new NotImplementedException();
	}
}
