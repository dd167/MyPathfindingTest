using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pathfinding
{
    public abstract class IPathfinder
    {
        public abstract void Initialize(GridGraph grid, int maxSearchNodes );

        public abstract List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal);

        public Action<PathingNode> OnNodeAddOpenSet;

        public Action<PathingNode> OnNodeVisited;


        protected List<Vector2Int> Trace(PathingNode node)
        {
            var path = new List<Vector2Int> { node.Location };
            while (node.Parent != null)
            {
                node = node.Parent;
                path.Add(node.Location);
            }
            path.Reverse();
            return path;
        }
    }   
}