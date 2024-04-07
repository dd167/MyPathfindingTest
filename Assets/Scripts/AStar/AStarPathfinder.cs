using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;

namespace Pathfinding
{
    public class AstarPathfinder : IPathfinder
    {
        private GridGraph _graph;
        private FastPriorityQueue<PathingNode> _open;
        private Vector2Int _start;
        private Vector2Int _goal;
        public int _channel = 0;


        public override void Initialize(GridGraph graph, int maxSearchNodes)
        {
            _graph = graph;
            _open = new FastPriorityQueue<PathingNode>(maxSearchNodes);
        }

        private Dictionary<Vector2Int, PathingNode> _nodeManager = new Dictionary<Vector2Int, PathingNode>();
        public PathingNode GetOrCreatePathingNode(Vector2Int location)
        {
            PathingNode node = null;
            if (!_nodeManager.TryGetValue(location, out node))
            {
                node = new PathingNode(location);
                _nodeManager.Add(location, node);
            }
            return node;
        }

        public override PathingNode TryGetExpandedNode(Vector2Int location)
        {
            if (_nodeManager.TryGetValue(location, out var node))
            {
                return node;
            }
            return null;
        }



        public override void Begin(Vector2Int start, Vector2Int goal)
        {
            base.Begin(start, goal);
            _start = start;
            _goal = goal;
            _open.Clear();
            _nodeManager.Clear();
            if (!_graph.IsNavigable(start))
            {
                Debug.LogError($"start {start} is not navigable!");
                return;
            }
            if (!_graph.IsNavigable(goal))
            {
                Debug.LogError($"goal {goal} is not navigable!");
                return;
            }


            var pathNode = GetOrCreatePathingNode(_graph[start].location);
            pathNode.F = 0;
            pathNode.G = 0;
            pathNode.Opened = true;

            _open.Enqueue(pathNode, pathNode.F);
            this.OnNodeAddOpenSet?.Invoke(pathNode);

        }


        public override bool Step()
        {
            if (_open.Count > 0)
            {
                PathingNode curNode = _open.Dequeue();
                curNode.Closed = true;
                this.OnNodeVisited?.Invoke(curNode);

                if (curNode.Location == _goal)
                {
                    pathResult = Trace(curNode);
                    return true;
                }


                foreach (var neighbourNode in _graph.Neighbours(curNode.Location))
                {
                    var nextNode = GetOrCreatePathingNode(neighbourNode.location);
                    if (nextNode.Closed)
                        continue;


                    double d = heuristicFunc(Math.Abs(nextNode.Location.x - curNode.Location.x),
                                        Math.Abs(nextNode.Location.y - curNode.Location.y));
                    double ng = curNode.G + d;

                    if (!nextNode.Opened || ng < nextNode.G)
                    {
                        nextNode.G = ng;
                        if (!nextNode.H.HasValue)
                        {
                            nextNode.H = HScale * heuristicFunc(Math.Abs(nextNode.Location.x - _goal.x),
                                Math.Abs(nextNode.Location.y - _goal.y));
                        }
                        nextNode.F = (WeightOfG * nextNode.G + WeightOfH * nextNode.H.Value);
                        nextNode.Parent = curNode;
                        //Debug.Log($"Expand node: f={neighbour.F}, h={neighbour.H}, g={neighbour.G}");

                        if (!nextNode.Opened)
                        {
                            _open.Enqueue(nextNode, nextNode.F);
                            nextNode.Opened = true;

                            OnNodeAddOpenSet?.Invoke(nextNode);
                        }
                        else
                        {
                            _open.UpdatePriority(nextNode, nextNode.F);
                        }
                    }
                }

                return false;
            }
            else
            {
                return true;
            }

        }
    }
}