using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;

namespace Pathfinding
{
    public class BidirectionAstarPathfinder : IPathfinder
    {
        AstarPathfinder _forwardFinder = new AstarPathfinder();
        AstarPathfinder _backwardFinder = new AstarPathfinder();
        private bool _meet;
        int _safeNum = 0;


        public override void Initialize(GridGraph graph, int maxSearchNodes)
        {
            _forwardFinder.Initialize(graph, maxSearchNodes);
            _backwardFinder.Initialize(graph, maxSearchNodes);
            _backwardFinder._channel = 1;
        }

        public override void Begin(Vector2Int start, Vector2Int goal)
        {
            base.Begin(start, goal);
            _meet = false;
            _safeNum = 0;

            _forwardFinder.OnNodeVisited = OnNodeVisited_Forward;
            _forwardFinder.OnNodeAddOpenSet = OnNodeAddOpenSet_Forward;
            _backwardFinder.OnNodeVisited = OnNodeVisited_Backward;
            _backwardFinder.OnNodeAddOpenSet = OnNodeAddOpenSet_Backward;

            _forwardFinder.heuristicDistanceMethod = this.heuristicDistanceMethod;
            _forwardFinder.WeightOfG = WeightOfG;
            _forwardFinder.WeightOfH = WeightOfH;
            _forwardFinder.HScale = HScale;

            _backwardFinder.heuristicDistanceMethod = this.heuristicDistanceMethod;
            _backwardFinder.WeightOfG = WeightOfG;
            _backwardFinder.WeightOfH = WeightOfH;
            _backwardFinder.HScale = HScale;

            _forwardFinder.Begin(start, goal);
            _backwardFinder.Begin(goal, start);
        }

        void OnNodeAddOpenSet_Backward(PathingNode node)
        {
            this.OnNodeAddOpenSet?.Invoke(node);   
        }

        void OnNodeAddOpenSet_Forward(PathingNode node)
        {
            this.OnNodeAddOpenSet?.Invoke(node);
        }

        void OnNodeVisited_Forward(PathingNode node)
        {
            this.OnNodeVisited?.Invoke(node);

            CheckMeet(node, true);
        }

        void OnNodeVisited_Backward(PathingNode node)
        {
            this.OnNodeVisited?.Invoke(node);

            CheckMeet(node, false);
        }


        void CheckMeet( PathingNode node, bool isForward)
        {
            var otherFinder = isForward ? _backwardFinder : _forwardFinder;
            var otherFinderNode = otherFinder.TryGetExpandedNode(node.Location);
            if ( otherFinderNode != null)
            {
                _meet = true;
                if( isForward)
                {
                    pathResult = Trace(node, otherFinderNode);
                }
                else
                {
                    pathResult = Trace(otherFinderNode, node);
                }
                
            }    
        }


        protected override Path Trace(PathingNode fNode, PathingNode bNode = null)
        {
            
            Path path = new Path();
            while (fNode.Parent != null)
            {
                path.AddNode(fNode);
                fNode = fNode.Parent;
            }
            path.AddNode(fNode);
            path.Reverse();

            bNode = bNode.Parent;
            while(bNode.Parent != null)
            {
                path.AddNode(bNode);
                bNode = bNode.Parent;
            }
            path.AddNode(bNode);

            return path;
        }


        public override bool Step()
        {
            if( _meet)
            {
                return true;
            }

            if( _forwardFinder.Step() )
            {
                return true;
            }


            if(_backwardFinder.Step())
            {
                return true;
            }


            if( ++_safeNum > 999999)
            {
                return true;
            }

          
            return false;

        }

        public override UnityEngine.Color GetColor(GridGraph.Node node)
        {
            if( node.isBlock)
            {
                return Utils.BlockColor;
            }
            else
            {
                bool isPath = pathResult != null && pathResult.Contains(node.location);
                var fNode = _forwardFinder.TryGetExpandedNode(node.location);
                var bNode = _backwardFinder.TryGetExpandedNode(node.location);
                if (isPath)
                {
                    if (node.location == _start)
                    {
                        return Utils.startColor;
                    }
                    else if (node.location == _goal)
                    {
                        return Utils.endColor;
                    }
                    else
                    {
                        if (fNode != null)
                        {
                            return Utils.PathColor0;
                        }
                        else
                        {
                            return Utils.PathColor1;
                        }
                    }

                   
                }
                else
                {
                    if (fNode != null && bNode != null)
                    {
                        return Utils.VisitedColor0;
                    }
                    else if (fNode != null)
                    {
                        if(fNode.Closed)
                        {
                            return Utils.VisitedColor0;
                        }
                        else
                        {
                            return Utils.OpenColor0;
                        }
                    }
                    else if (bNode != null)
                    {
                        if (bNode.Closed)
                        {
                            return Utils.VisitedColor1;
                        }
                        else
                        {
                            return Utils.OpenColor1;
                        }
                    }
                    else
                    {
                        return Utils.InitColor;
                    }
                }
            }
            
        }


    }
}