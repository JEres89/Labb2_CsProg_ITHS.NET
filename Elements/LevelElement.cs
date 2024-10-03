using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Elements;
internal abstract class LevelElement
{
	public int X { get; protected set; }
	public int Y { get; protected set; }
	public char Symbol { get; protected set; }

	public static explicit operator char(LevelElement element) => element == null ? ' ' : element.Symbol;

	internal abstract Interaction Collide(LevelElement element);

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

