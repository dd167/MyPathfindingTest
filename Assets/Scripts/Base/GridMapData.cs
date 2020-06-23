using System;
using UnityEngine;

namespace Pathfinding
{
    public class GridMapData : ScriptableObject
    {
        public Vector3 originPos;

        public Vector2 sampleUnit;

        public int gridWidth;

        public int gridHeight;

        [HideInInspector]
        public byte[] data;

        public Vector3 GridLocationToWorldPos(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x * sampleUnit.x, originPos.y, gridPos.y * sampleUnit.y);
        }

        public Vector2Int WorldPosToGridLocation( Vector3 worldPos )
        {
            Vector3 offsetPos = worldPos - originPos;
            return new Vector2Int(Mathf.FloorToInt(offsetPos.x / sampleUnit.x), Mathf.FloorToInt(offsetPos.z / sampleUnit.y));
        }

        

    }
}
