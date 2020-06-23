using UnityEngine;
using System.Collections.Generic;
using System;


namespace Pathfinding
{
    public class AtarPathfinder : IPathfinder
    {
        private GridGraph _graph;
        private FastPriorityQueue<PathingNode> _open;
        private Vector2Int _start;
        private Vector2Int _goal;

        public override void Initialize(GridGraph graph, int maxSearchNodes)
        {
            _graph = graph;
            _open = new FastPriorityQueue<PathingNode>(maxSearchNodes);
        }

        public override List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            if (_graph == null)
                return null;

            _start = start;
            _goal = goal;

            var startNode = new PathingNode(_start) { F = 0, G = 0, Opened = true };
            _open.Enqueue(startNode, startNode.F);
            this.OnNodeAddOpenSet?.Invoke(startNode);

            while (_open.Count != 0)
            {
                PathingNode curNode = _open.Dequeue();
                curNode.Closed = true;
                this.OnNodeVisited?.Invoke(curNode);

                if (curNode.Location == _goal)
                    return Trace(curNode);

                foreach( var neighbour in  _graph.Neighbours(curNode) )
                {
                    if (neighbour.Closed)
                        continue;

                    double d = Utils.Heuristic_Diagonal(Math.Abs(neighbour.Location.x - curNode.Location.x),
                                        Math.Abs(neighbour.Location.y - curNode.Location.y));
                    double ng = curNode.G + d;

                    if (!neighbour.Opened || ng < neighbour.G)
                    {
                        neighbour.G = ng;
                        if (!neighbour.H.HasValue)
                        {
                            neighbour.H = Utils.Heuristic_Diagonal(Math.Abs(neighbour.Location.x - _goal.x),
                                Math.Abs(neighbour.Location.y - _goal.y));
                        }
                        neighbour.F = (neighbour.G + neighbour.H.Value);
                        neighbour.Parent = curNode;

                        if (!neighbour.Opened)
                        {
                            _open.Enqueue(neighbour, neighbour.F);
                            neighbour.Opened = true;

                            OnNodeAddOpenSet?.Invoke(neighbour);
                        }
                        else
                        {
                            _open.UpdatePriority(neighbour, neighbour.F);
                        }
                    }
                }
            }

            return null;
        }
    }
}