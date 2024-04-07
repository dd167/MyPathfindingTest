using UnityEngine;

namespace Pathfinding
{
    public class PathingNode : FastPriorityQueueNode
    {
        public PathingNode(int x, int y)
            : this(new Vector2Int(x, y))
        {
            
        }

        public PathingNode(Vector2Int location)
        {
            Location = location;
        }

        public PathingNode(GridGraph.Node node) 
        {
            Location = node.location;
            IsBlock = node.isBlock;
        }


        public Vector2Int Location { get; private set; }

        public bool IsBlock = false;


        public double? H { get; set; }
        public double F { get; set; }
        public double G { get; set; }
        public bool Opened { get; set; }
        public bool Closed { get; set; }

        public PathingNode Parent { get; set; }

    }
}