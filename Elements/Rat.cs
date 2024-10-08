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
	internal override void Update(Level CurrentLevel)
	{
		throw new NotImplementedException();
	}


	internal override (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView)
	{

		char c = isInView ? Symbol : ' ';
		ConsoleColor fg = isInView ? ForegroundVisibleRat : DiscoveredRat;
		ConsoleColor bg = isInView ? BackroundVisibleRat : DiscoveredRat;

		return (c, fg, bg);
	}
	public static ConsoleColor ForegroundVisibleRat { get; } = ConsoleColor.Red;
	public static ConsoleColor BackroundVisibleRat { get; } = ConsoleColor.Gray;
	public static ConsoleColor DiscoveredRat { get; } = ConsoleColor.DarkGray;

}
