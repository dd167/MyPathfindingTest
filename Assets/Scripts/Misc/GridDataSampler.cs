using UnityEngine;
using UnityEditor;
using Pathfinding;

public class GridDataSampler : EditorWindow
{
    [MenuItem("Pathfinding/采样2D格子数据", false, 100)]
    static void Open()
    {
        GetWindow<GridDataSampler>();
    }

    Vector2 _sampleRange = new Vector2(1500, 1500);
    Vector2 _sampleUnit = Vector2.one;

    private void OnGUI()
    {
        _sampleRange = EditorGUILayout.Vector2Field("采样范围", _sampleRange);
        _sampleUnit = EditorGUILayout.Vector2Field("采样单位", _sampleUnit);

        GUILayout.Space(50);

        if (GUILayout.Button("采样2D网格数据"))
        {
            int grid_width = Mathf.CeilToInt(_sampleRange.x / _sampleUnit.x);
            int grid_height = Mathf.CeilToInt(_sampleRange.y / _sampleUnit.y);

            byte[,] data = new byte[grid_width, grid_height];
            Rect cell = new Rect();

            for (int x = 0; x < grid_width; ++x)
            {
                for (int z = 0; z < grid_height; ++z)
                {
                    cell.Set(x * _sampleUnit.x, z * _sampleUnit.y, _sampleUnit.x, _sampleUnit.y);
                    data[x, z] = HasObstacle(ref cell) ? (byte)1 : (byte)0;
                }
            }
       
            GridMapData gridMap = ScriptableObject.CreateInstance<GridMapData>();
            gridMap.data = Utils.Array2DTo1D(data);
            gridMap.gridWidth = grid_width;
            gridMap.gridHeight = grid_height;
            gridMap.originPos = Vector3.zero;
            gridMap.sampleUnit = this._sampleUnit;
            AssetDatabase.CreateAsset(gridMap, "Assets/Model/gridmapdata.asset");
            AssetDatabase.SaveAssets();


            GridMapViewer mapViewer = Object.FindObjectOfType<GridMapViewer>();
            if( mapViewer != null )
            {
                mapViewer._gridData = gridMap;
            }

            GridPathfindingTest tester = Object.FindObjectOfType<GridPathfindingTest>();
            if( tester != null )
            {
                tester.gridMapData = gridMap;
            }
        }       
    }

    RaycastHit[] _hits = new RaycastHit[1];
    bool HasObstacle( ref Rect cell )
    {
        int mask = 1 << Utils.ObstacleLayer;
        int hit = Physics.RaycastNonAlloc(new Vector3(cell.center.x, 10, cell.center.y), Vector3.down,  _hits, 100, mask);
        return hit > 0;
    }
}