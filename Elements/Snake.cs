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

	internal override void Update(Level CurrentLevel)
	{
		if (CurrentLevel.Player.Pos.IsAdjacent(Pos))
		{
			//if(!CurrentLevel.TryMove(this, Pos.GetDirection(CurrentLevel.Player.Pos).Invert(), out var collision))
			//{
			//	var result = collision.ActsIfCollide(this);
			//	switch (result)
			//	{
			//		case Reactions.Move:
			//			break;
			//		case Reactions.Block:
			//			break;
			//		case Reactions.Aggressive:
			//		case Reactions.Activate:
			//		case Reactions.Acquire:
			//		case Reactions.Trigger:
			//		case Reactions.Status:
			//		default:

			//			break;
			//	}
			//}
		}
	}


	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{
		char c = isInView ? Symbol : ' ';
		ConsoleColor fg = isInView ? ForegroundVisibleSnake : DiscoveredSnake;
		ConsoleColor bg = isInView ? BackroundVisibleSnake : DiscoveredSnake;

		return (c, fg, bg);
	}
	public static ConsoleColor BackroundVisibleSnake { get; } = ConsoleColor.Gray;
	public static ConsoleColor ForegroundVisibleSnake { get; } = ConsoleColor.Green;
	public static ConsoleColor DiscoveredSnake { get; } = ConsoleColor.DarkGray;
}
