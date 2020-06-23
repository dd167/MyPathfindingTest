using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding
{
    [ExecuteInEditMode]
    public class GridMapViewer : MonoBehaviour
    {
        public bool ShowObstacle = true;
        

        public GridMapData _gridData;

        private void OnDrawGizmos()
        {
            if (_gridData != null && ShowObstacle )
            {
                int gridWidth = _gridData.gridWidth;
                int gridHeight = _gridData.gridHeight;

                Vector3 up = new Vector3(0, 2, 0);

                for (int x = 0; x < gridWidth; ++x)
                {
                    for (int z = 0; z < gridHeight; ++z)
                    {
                        Vector3 worldPos = _gridData.GridLocationToWorldPos(new Vector2Int(x, z));

                        bool isObstacle = _gridData.data[x * gridHeight + z] > 0;
                        Gizmos.color = isObstacle ? Color.red : Color.green;
                        Gizmos.DrawLine(worldPos, worldPos + up);
                    }
                }
            }

            if(_hightLightGrid.Count >0)
            {
                Gizmos.color = Color.yellow;
                for( int i = 0; i < _hightLightGrid.Count; ++i)
                {
                    Gizmos.DrawCube(_hightLightGrid[i], Vector3.one);
                } 
            }
        }

        private List<Vector3> _hightLightGrid = new List<Vector3>();

        public void ClearHighlight()
        {
            _hightLightGrid.Clear();
        }

        public void HighlightGrid( Vector2Int pos )
        {
            _hightLightGrid.Add(new Vector3(pos.x + 0.5f, 0.1f, pos.y + 0.5f));
            //Debug.LogWarning("open node:" + pos.ToString());
        }
    }
}


