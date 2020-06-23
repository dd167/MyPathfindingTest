using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{

    public class JPSGridGraph : GridGraph
    {
        public JPSGridGraph(byte[,] navigable) : base(navigable)
        {

        }


        internal override IEnumerable<PathingNode> Neighbours(PathingNode node)
        {
            if (node.Parent != null)
            {
                Vector2Int n = node.Location;
                Vector2Int p = node.Parent.Location;

                Vector2Int dNorm = new Vector2Int(
                    (n.x - p.x) / Math.Max(Math.Abs(n.x - p.x), 1),
                    (n.y - p.y) / Math.Max(Math.Abs(n.y - p.y), 1));

                // Diagonal
                if (dNorm.x != 0 && dNorm.y != 0)
                {
                    if (IsNavigable(n.x, n.y + dNorm.y))
                        yield return _grid[n.x, n.y + dNorm.y];

                    if (IsNavigable(n.x + dNorm.x, n.y))
                        yield return _grid[n.x + dNorm.x, n.y];

                    if ((IsNavigable(n.x, n.y + dNorm.y) || IsNavigable(n.x + dNorm.x, n.y)) && IsNavigable(n.x + dNorm.x, n.y + dNorm.y))
                        yield return _grid[n.x + dNorm.x, n.y + dNorm.y];

                    if (!IsNavigable(n.x - dNorm.x, n.y) && IsNavigable(n.x, n.y + dNorm.y) && IsNavigable(n.x - dNorm.x, n.y + dNorm.y))
                        yield return _grid[n.x - dNorm.x, n.y + dNorm.y];

                    if (!IsNavigable(n.x, n.y - dNorm.y) && IsNavigable(n.x + dNorm.x, n.y) && IsNavigable(n.x + dNorm.x, n.y - dNorm.y))
                        yield return _grid[n.x + dNorm.x, n.y - dNorm.y];
                }
                // Cardinal
                else
                {
                    if (dNorm.x == 0)
                    {
                        if (IsNavigable(n.x, n.y + dNorm.y))
                        {
                            yield return _grid[n.x, n.y + dNorm.y];

                            if (!IsNavigable(n.x + 1, n.y) && IsNavigable(n.x + 1, n.y + dNorm.y))
                                yield return _grid[n.x + 1, n.y + dNorm.y];

                            if (!IsNavigable(n.x - 1, n.y) && IsNavigable(n.x - 1, n.y + dNorm.y))
                                yield return _grid[n.x - 1, n.y + dNorm.y];
                        }
                    }
                    else if (IsNavigable(n.x + dNorm.x, n.y))
                    {
                        yield return _grid[n.x + dNorm.x, n.y];

                        if (!IsNavigable(n.x, n.y + 1) && IsNavigable(n.x + dNorm.x, n.y + 1))
                            yield return _grid[n.x + dNorm.x, n.y + 1];

                        if (!IsNavigable(n.x, n.y - 1) && IsNavigable(n.x + dNorm.x, n.y - 1))
                            yield return _grid[n.x + dNorm.x, n.y - 1];
                    }
                }
            }
            else
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
        }
    }

}