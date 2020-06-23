using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System;

namespace Pathfinding
{
    public class GridGraph
    {
        protected static readonly Vector2Int[] Directions =
        {
            // Cardinal
            new Vector2Int(-1, 0), // W
            new Vector2Int(1, 0), // E
            new Vector2Int(0, 1), // N 
            new Vector2Int(0, -1), // S
            // Diagonal
            new Vector2Int(-1, -1), // NW
            new Vector2Int(-1, 1), // SW
            new Vector2Int(1, -1), // NE
            new Vector2Int(1, 1) // SE
        };

        protected readonly int _boundsMaxX;
        protected readonly int _boundsMaxY;

        protected readonly int _boundsMinX;
        protected readonly int _boundsMinY;

        internal readonly PathingNode[,] _grid;
        internal readonly byte[,] _navigable;

        public GridGraph(byte[,] navigable)
        {
            _boundsMinX = 0;
            _boundsMaxX = navigable.GetUpperBound(0);
            _boundsMinY = 0;
            _boundsMaxY = navigable.GetUpperBound(1);

            _navigable = navigable;

            // Initialise the Grid
            _grid = new PathingNode[_boundsMaxX + 1, _boundsMaxY + 1];
            for (var x = _boundsMinX; x <= _boundsMaxX; x++)
                for (var y = _boundsMinY; y <= _boundsMaxY; y++)
                    _grid[x, y] = new PathingNode(x, y);
        }




        internal PathingNode this[int x, int y] { get { return _grid[x, y]; } }
        internal PathingNode this[Vector2Int location] { get { return _grid[location.x, location.y]; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNavigable(int x, int y)
        {
            return InBounds(x, y) && _navigable[x, y] == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBounds(int x, int y)
        {
            return x >= _boundsMinX && x <= _boundsMaxX &&
                   y >= _boundsMinY && y <= _boundsMaxY;
        }

        internal virtual IEnumerable<PathingNode> Neighbours(PathingNode node)
        {
            for (var i = 0; i < Directions.Length; i++)
            {
                int propX = node.Location.x + Directions[i].x;
                int propY = node.Location.y + Directions[i].y;

                if (IsNavigable(propX, propY))
                {
                    yield return this[propX, propY];
                }
            }
        }

        public Vector2Int GetNearestNavigablePos( Vector2Int grid, int findRadius = 10 )
        {
            int radius = 0;
            while( ++radius <= findRadius)
            {
                for (var i = 0; i < Directions.Length; i++)
                {
                    int propX = grid.x + radius * Directions[i].x;
                    int propY = grid.y + radius * Directions[i].y;
                    if (IsNavigable(propX, propY))
                    {
                        return new Vector2Int(propX, propY);
                    }
                }
            }
            return Utils.InvalidGrid; 
        }
    }
}