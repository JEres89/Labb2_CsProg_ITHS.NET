using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labb2_CsProg_ITHS.NET.Elements.LevelEntity;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal class PlayerEntity : LevelEntity, IInputEndpoint
{
	public PlayerEntity(Position p, char symbol) : base(p, symbol)
	{
		Name = "";
		Description = "You.";
		ViewRange = 3;
	}
	public void SetName(string name)
	{
		Name = name;
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

	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{
		return (Symbol, ConsoleColor.White, ConsoleColor.Black);
	}

	internal override void Update(Level CurrentLevel)
	{
		throw new NotImplementedException();
	}

	void IInputEndpoint.KeyPressed(ConsoleKeyInfo key)
	{
		throw new NotImplementedException();
	}
}
