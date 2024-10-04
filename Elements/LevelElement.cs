using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal abstract class LevelElement
{
	public Position Pos { get; protected set; }
	public char Symbol { get; protected set; }
	public string Name { get; protected set; }
	public string Description { get; protected set; }


	public static explicit operator char(LevelElement element) => element == null ? ' ' : element.Symbol;

	internal abstract Interaction Collide(LevelElement element);

	internal abstract void Update(Level CurrentLevel);

	internal abstract (char c, ConsoleColor fg, ConsoleColor bg) GetRenderData(bool isDiscovered, bool isInView);

	internal enum Interaction
	{
		Block,		//none, this element is not affected in any way
		Aggressive, //is an enemy 
		Move,		//this element will move out of the way of the element
		Activate,   //performs a function
		Acquire,	//this element can be obtained as an owned object
		Trigger,	//triggers an event elsewhere
		Status,     //Causes a passive lingering effect on the instigator
	}
}

