using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RVO;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

namespace Pathfinding
{
    public class Path
    {
        private List<PathingNode> nodeList = new List<PathingNode>();
        private HashSet<Vector2Int> nodeSets = new HashSet<Vector2Int>();

        public void AddNode(PathingNode node)
        {
            nodeList.Add(node);
            nodeSets.Add(node.Location);
        }

        public void Reverse()
        {
            nodeList.Reverse();
        }
        

        public bool Contains(Vector2Int location)
        {
            return nodeSets.Contains(location);
        }

        public bool HasPath()
        {
            return nodeList.Count > 1;
        }

        public int NodeCount => nodeList.Count;
       
    }

    public abstract class IPathfinder
    {
        public abstract void Initialize(GridGraph grid, int maxSearchNodes);

        public Action<PathingNode> OnNodeAddOpenSet;

        public Action<PathingNode> OnNodeVisited;

        public HeuristicDistanceMethod heuristicDistanceMethod;
        public double WeightOfG = 1.0;
        public double WeightOfH = 1.0;
        public double HScale = 1.0;

        protected Path pathResult;
        protected Heuristic_Func heuristicFunc;
        public Path GetPath() { return pathResult; }

        public virtual PathingNode TryGetExpandedNode(Vector2Int location)
        {
            return null;
        }


        public virtual void Begin(Vector2Int start, Vector2Int goal)
        {
            pathResult = null;
            switch (heuristicDistanceMethod)
            {
                case HeuristicDistanceMethod.Diagonal:
                    heuristicFunc = Utils.Heuristic_Diagonal;
                    break;
                case HeuristicDistanceMethod.Manhattan:
                    heuristicFunc = Utils.Heuristic_Manhattan;
                    break;
                default:
                    heuristicFunc = Utils.Heuristic_Euclidean;
                    break;

            }
        }
       

        public virtual bool Step()
        {
            return true;
        }
        
        

        protected virtual Path Trace(PathingNode fNode, PathingNode bNode = null)
        {
            Path path = new Path();
            while (fNode.Parent != null)
            {
                path.AddNode(fNode);
                fNode = fNode.Parent;     
            }
            path.AddNode(fNode);
            path.Reverse();
            return path;
        }

        public virtual Path FindPath(Vector2Int start, Vector2Int goal)
        {

            Begin(start, goal);
            while (!Step())
            {
            }
            pathResult = GetPath();
            return pathResult; 
        }


        public virtual UnityEngine.Color GetColor(GridGraph.Node node)
        {
            if (node.isBlock)
            {
                return Utils.BlockColor;
            }
            else
            {
                if (pathResult != null && pathResult.Contains(node.location))
                {
                    return Utils.PathColor0;
                }
                else
                {
                    var pathNode = this.TryGetExpandedNode(node.location);
                    if (pathNode != null)
                    {
                        if (pathNode.Closed)
                        {
                            return Utils.VisitedColor0;
                        }
                        else
                        {
                            return Utils.OpenColor0;
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