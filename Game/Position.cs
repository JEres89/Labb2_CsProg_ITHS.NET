using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET.Game;

/// <summary>
/// Note: The order of the X and Y values are reversed from the usual (X, Y) to (Y, X) to match the order of 2D arrays.
/// </summary>
internal record struct Position
{
    internal int Y { get; set; }
    internal int X { get; set; }

    internal static Position Up = new(-1, 0);
    internal static Position Left = new(0, -1);
    internal static Position Down = new(1, 0);
    internal static Position Right = new(0, 1);

    internal Position(int y, int x)
    {
        Y = y;
        X = x;
    }

    internal Position(Position position)
    {
        Y = position.Y;
        X = position.X;
    }

    internal (int, int) ToTuple() => (Y, X);

    // Non-usable methods when using record struct
    //internal void SetPosition(int y, int x)
    //{
    //	X = x;
    //	Y = y;
    //}

    //internal void SetPosition(Position position)
    //{
    //	X = position.X;
    //	Y = position.Y;
    //}

    //internal void Move(int y, int x)
    //{
    //	X += x;
    //	Y += y;
    //}

    internal Position Move(Position position)
    {
        X += position.X;
        Y += position.Y;
        return this;
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
        return X - position.X <= 1 && Y - position.Y <= 1;
    }

    internal bool IsAdjacent(int y, int x)
    {
        return X - x <= 1 && Y - y <= 1 && X - x != Y - y;
    }

    internal int Distance(Position position)
    {
        return Distance(position.Y, position.X);
    }

    internal int Distance(int y, int x)
    {
        var dist = Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
        var distFloor = Math.Floor(dist);
        return (int)(dist - distFloor > 0.5 ? Math.Ceiling(dist) : distFloor);
    }

    internal Position GetDirection(Position position)
    {
        return new Position(position.Y - Y, position.X - X);
    }

    internal Position GetDirection(int y, int x)
    {
        return new Position(y - Y, x - X);
    }

    internal Position GetDirectionUnit(Position position)
    {
        Position direction = GetDirection(position);
        var absY = Math.Abs(direction.Y);
        var absX = Math.Abs(direction.X);
        if (absY > absX)
        {
            direction = direction with { Y = direction.Y / absY, X = 0 };
        }
        else
        {
            direction = direction with { Y = 0, X = direction.X / absX };
        }
        //var y = direction.Y / Math.Abs(direction.Y) * distance; // > 0 ? Math.Min(direction.Y, distance) : Math.Max(direction.Y, -distance);
        //var x = direction.X / Math.Abs(direction.X) * distance; //> 0 ? Math.Min(direction.X, distance) : Math.Max(direction.X, -distance);
        return direction;
    }

    internal Position GetDirection(int y, int x, int distance)
    {
        Position direction = GetDirection(y, x);
        direction.Y = direction.Y > 0 ? Math.Min(direction.Y, distance) : Math.Max(direction.Y, -distance);
        direction.X = direction.X > 0 ? Math.Min(direction.X, distance) : Math.Max(direction.X, -distance);
        return direction;
    }

    internal Position Invert()
    {
        Y = -Y;
        X = -X;
        return this;
    }

    internal void Deconstruct(out int y, out int x)
    {
        y = Y;
        x = X;
    }
    //public static Position operator -(Position position) => new (-position.X, -position.Y);


}
