using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pathfinding
{
    public class JPSPathfinder : IPathfinder
    {     
        private JPSGridGraph _graph;
        private FastPriorityQueue<PathingNode> _open;
        private Vector2Int _start;
        private Vector2Int _goal;

        public double JumpCostTime { get; private set; }

        public override void Initialize(GridGraph graph, int maxSearchNodes = 40960)
        {
            _graph = graph as JPSGridGraph;
            if( _graph == null )
            {
                throw new System.Exception("[JPSPathfinder.Initialize] graph must a JPSGridGraph!");
            }
            _open = new FastPriorityQueue<PathingNode>(maxSearchNodes);
        }

        public override List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            if (_graph == null)
                return null;

            _start = start;
            _goal = goal;
            JumpCostTime = 0;

            var startNode = new PathingNode(_start) { F = 0, G = 0, Opened = true };

            _open.Enqueue(startNode, startNode.F);
            this.OnNodeAddOpenSet?.Invoke(startNode);

            while (_open.Count != 0)
            {
                PathingNode node = _open.Dequeue();

                node.Closed = true;
                this.OnNodeVisited?.Invoke(node);

                if (node.Location == _goal)
                    return Trace(node);

                IdentitySuccessors(node);
            }

            return null;
        }

  

        private void IdentitySuccessors(PathingNode node)
        {
            foreach (PathingNode neighbour in _graph.Neighbours(node))
            {
                System.DateTime s = System.DateTime.Now;
                Vector2Int jumpPoint = Jump(neighbour.Location, node.Location);
                JumpCostTime += ((System.DateTime.Now - s).TotalMilliseconds);

                if ( jumpPoint != Utils.InvalidGrid )
                {
                    PathingNode jumpNode = _graph[jumpPoint];

                    if (jumpNode.Closed)
                        continue;

                    double d = Utils.Heuristic_Diagonal(Math.Abs(jumpPoint.x - node.Location.x), Math.Abs(jumpPoint.y - node.Location.y));
                    double ng = node.G + d;

                    if (!jumpNode.Opened || ng < jumpNode.G)
                    {
                        jumpNode.G = ng;
                        if (!jumpNode.H.HasValue)
                            jumpNode.H = Utils.Heuristic_Diagonal(Math.Abs(jumpPoint.x - _goal.x), Math.Abs(jumpPoint.y - _goal.y));
                        jumpNode.F = jumpNode.G + jumpNode.H.Value;
                        jumpNode.Parent = node;

                        if (!jumpNode.Opened)
                        {
                            _open.Enqueue(jumpNode, jumpNode.F);
                            jumpNode.Opened = true;

                            OnNodeAddOpenSet?.Invoke(jumpNode);
                        }
                        else
                        {
                            _open.UpdatePriority(jumpNode, jumpNode.F);
                        }
                    }
                }
            }
        }

        private Vector2Int Jump(Vector2Int current, Vector2Int proposed)
        {
            int x = current.x;
            int y = current.y;
            int dx = current.x - proposed.x;
            int dy = current.y - proposed.y;

            if (!_graph.IsNavigable(x, y))
                return Utils.InvalidGrid;

            if (_goal == current)
                return current;

            // Diagonal
            if (dx != 0 && dy != 0)
            {
                if ((_graph.IsNavigable(x - dx, y + dy) && !_graph.IsNavigable(x - dx, y)) ||
                    (_graph.IsNavigable(x + dx, y - dy) && !_graph.IsNavigable(x, y - dy)))
                    return current;

                if (Jump(new Vector2Int(x + dx, y), current) != Utils.InvalidGrid ||
                    Jump(new Vector2Int(x, y + dy), current) != Utils.InvalidGrid )
                    return current;
            }
            // Cardinal
            else
            {
                if (dx != 0)
                {
                    // Horizontal
                    if ((_graph.IsNavigable(x + dx, y + 1) && !_graph.IsNavigable(x, y + 1)) ||
                        (_graph.IsNavigable(x + dx, y - 1) && !_graph.IsNavigable(x, y - 1)))
                        return current;
                }
                else
                {
                    // Vertical
                    if ((_graph.IsNavigable(x + 1, y + dy) && !_graph.IsNavigable(x + 1, y)) ||
                        (_graph.IsNavigable(x - 1, y + dy) && !_graph.IsNavigable(x - 1, y)))
                        return current;
                }
            }

            if (_graph.IsNavigable(x + dx, y) || _graph.IsNavigable(x, y + dy))
                return Jump(new Vector2Int(x + dx, y + dy), current);

            return Utils.InvalidGrid;
        }
    }
}
