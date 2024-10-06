using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET;

/// <summary>
/// Note: The order of the X and Y values are reversed from the usual (X, Y) to (Y, X) to match the order of 2D arrays.
/// </summary>
internal class Position
{
	internal int X { get; set; }
	internal int Y { get; set; }

	internal Position(int y, int x)
	{
		X = x;
		Y = y;
	}

	internal Position(Position position)
	{
		X = position.X;
		Y = position.Y;
	}

	internal (int, int) ToTuple() => (Y, X);

	internal void SetPosition(int y, int x)
	{
		X = x;
		Y = y;
	}

	internal void SetPosition(Position position)
	{
		X = position.X;
		Y = position.Y;
	}

	internal void Move(int y, int x)
	{
		X += x;
		Y += y;
	}

	internal void Move(Position position)
	{
		X += position.X;
		Y += position.Y;
	}

	internal bool IsEqual(Position position)
	{
		return X == position.X && Y == position.Y;
	}

	internal bool IsEqual(int y, int x)
	{
		return X == x && Y == y;
	}

	internal bool IsAdjacent(Position position)
	{
		return (X - position.X) <= 1 && (Y - position.Y) <= 1;
	}

	internal bool IsAdjacent(int y, int x)
	{
		return ((X - x) <= 1 && (Y - y) <= 1) && (X - x) != (Y - y);
	}

	internal int Distance(Position position)
	{
		return Distance(position.X, position.Y);
	}

	internal int Distance(int y, int x)
	{
		var dist = Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
		var distFloor = Math.Floor(dist);
		return (int)(dist-distFloor > 0.5 ? Math.Ceiling(dist):distFloor);
	}

	internal Position GetDirection(Position position)
	{
		return new Position(position.Y - Y, position.X - X);
	}

	internal Position GetDirection(int y, int x)
	{
		return new Position(y - Y, x - X);
	}

	internal Position GetDirection(Position position, int distance)
	{
		Position direction = GetDirection(position);
		direction.X = direction.X > 0 ? Math.Min(direction.X, distance) : Math.Max(direction.X, -distance);
		direction.Y = direction.Y > 0 ? Math.Min(direction.Y, distance) : Math.Max(direction.Y, -distance);
		return direction;
	}

	internal Position GetDirection(int y, int x, int distance)
	{
		Position direction = GetDirection(y, x);
		direction.X = direction.X > 0 ? Math.Min(direction.X, distance) : Math.Max(direction.X, -distance);
		direction.Y = direction.Y > 0 ? Math.Min(direction.Y, distance) : Math.Max(direction.Y, -distance);
		return direction;
	}

	internal Position Invert()
	{
		X = -X;
		Y = -Y;
		return this;
	}
	//public static Position operator -(Position position) => new (-position.X, -position.Y);


}
