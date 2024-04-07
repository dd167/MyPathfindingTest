using UnityEngine;
using UnityEditor;

public class TestSceneGenerator : EditorWindow
{
    [MenuItem("Pathfinding/生成测试场景", false, 50)]
    static void Open()
    {
        GetWindow<TestSceneGenerator>();
    }

    Vector2Int  _randomeRange = new Vector2Int(1500, 1500);
    int         _obstacleNum = 10000;
    Vector2Int  _obstacleSizeRange = new Vector2Int(1, 5);
    bool        _trouble1 = true;
    bool        _trouble2 = true;

    private void Awake()
    {
        _randomeRange.x = EditorPrefs.GetInt("_randomeRange_x", 100);
        _randomeRange.y = EditorPrefs.GetInt("_randomeRange_y", 100);
        _obstacleNum = EditorPrefs.GetInt("_obstacleNum", 100);
        _obstacleSizeRange.x = EditorPrefs.GetInt("_obstacleSizeRange_x", 1);
        _obstacleSizeRange.y = EditorPrefs.GetInt("_obstacleSizeRange_y", 5);
        _trouble1 = EditorPrefs.GetBool("_trouble1", false);
        _trouble2 = EditorPrefs.GetBool("_trouble2", false);

    }

    private void OnDestroy()
    {
        EditorPrefs.SetInt("_randomeRange_x", _randomeRange.x);
        EditorPrefs.SetInt("_randomeRange_y", _randomeRange.y);
        EditorPrefs.SetInt("_obstacleNum", _obstacleNum);
        EditorPrefs.SetInt("_obstacleSizeRange_x", _obstacleSizeRange.x);
        EditorPrefs.SetInt("_obstacleSizeRange_y", _obstacleSizeRange.y);
        EditorPrefs.SetBool("_trouble1", _trouble1);
        EditorPrefs.SetBool("_trouble2", _trouble2);
    }


    void OnGUI()
    {
        _randomeRange = EditorGUILayout.Vector2IntField("随机范围", _randomeRange);
        _obstacleNum = EditorGUILayout.IntField("阻挡数量", _obstacleNum);
        _obstacleSizeRange = EditorGUILayout.Vector2IntField("阻挡大小范围", _obstacleSizeRange);
        _trouble1 = EditorGUILayout.Toggle("添加难度1", _trouble1);
        _trouble2 = EditorGUILayout.Toggle("添加难度2", _trouble2);

        GUILayout.Space(50);

        if( GUILayout.Button("生成") )
        {
            Generate();
        }
    }

    void Generate()
    {
        GameObject cube = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Model/Cube.prefab");
        if( cube == null )
        {
            EditorUtility.DisplayDialog("Error","Assets/Model/Cube.prefab not found!","OK");
            return;
        }

        GameObject.DestroyImmediate(GameObject.Find("ObstacleRoot"));

        GameObject testRoot = new GameObject("ObstacleRoot");
        Transform root = testRoot.transform;
        root.localPosition = Vector3.zero;

        int genObstacleCount = _obstacleNum;

        while( genObstacleCount > 0 )
        {
            GameObject obj = GameObject.Instantiate<GameObject>(cube);
            obj.transform.SetParent(root);
            float x = UnityEngine.Random.Range(0, _randomeRange.x);
            float z = UnityEngine.Random.Range(0, _randomeRange.y);
            int scale = UnityEngine.Random.Range(_obstacleSizeRange.x, _obstacleSizeRange.y);
            obj.transform.localPosition = new Vector3(x + ((float)scale*0.5f), 0, z + ((float)scale * 0.5f));
            obj.transform.localScale = new Vector3(scale, 1, scale);
            obj.layer = Pathfinding.Utils.ObstacleLayer;
            obj.isStatic = true;

            --genObstacleCount;
        }


        int middleX = _randomeRange.x / 2;
        int middleY = _randomeRange.y / 2;

        if(_trouble1)
        {
            GameObject av1 = GameObject.Instantiate<GameObject>(cube);
            av1.transform.SetParent(root);
            av1.transform.localPosition = new Vector3(middleX, 0, middleY);
            av1.transform.localScale = new Vector3(1, 1, _randomeRange.y * 0.8f);
            av1.layer = Pathfinding.Utils.ObstacleLayer;
            av1.isStatic = true;

            GameObject ah1 = GameObject.Instantiate<GameObject>(cube);
            ah1.transform.SetParent(root);
            ah1.transform.localPosition = new Vector3(middleX - middleX*0.25f, 0, middleY+_randomeRange.y * 0.4f);
            ah1.transform.localScale = new Vector3(middleX * 0.5f, 1, 1);
            ah1.layer = Pathfinding.Utils.ObstacleLayer;
            ah1.isStatic = true;

            GameObject ah2 = GameObject.Instantiate<GameObject>(cube);
            ah2.transform.SetParent(root);
            ah2.transform.localPosition = new Vector3(middleX + middleX * 0.25f, 0, middleY - _randomeRange.y * 0.4f);
            ah2.transform.localScale = new Vector3(middleX * 0.5f, 1, 1);
            ah2.layer = Pathfinding.Utils.ObstacleLayer;
            ah2.isStatic = true;        
        }

        if( _trouble2 )
        {
            GameObject bh1 = GameObject.Instantiate<GameObject>(cube);
            bh1.transform.SetParent(root);
            bh1.transform.localPosition = new Vector3(middleX, 0, middleY);
            bh1.transform.localScale = new Vector3(_randomeRange.y * 0.8f, 1, 1);
            bh1.layer = Pathfinding.Utils.ObstacleLayer;
            bh1.isStatic = true;

            GameObject bv1 = GameObject.Instantiate<GameObject>(cube);
            bv1.transform.SetParent(root);
            bv1.transform.localPosition = new Vector3(middleX - _randomeRange.y * 0.4f, 0, middleY - middleX * 0.25f);
            bv1.transform.localScale = new Vector3(1, 1, middleX * 0.5f);
            bv1.layer = Pathfinding.Utils.ObstacleLayer;
            bv1.isStatic = true;

            GameObject bv2 = GameObject.Instantiate<GameObject>(cube);
            bv2.transform.SetParent(root);
            bv2.transform.localPosition = new Vector3(middleX + _randomeRange.y * 0.4f, 0, middleY + middleX * 0.25f);
            bv2.transform.localScale = new Vector3(1, 1, middleX * 0.5f);
            bv2.layer = Pathfinding.Utils.ObstacleLayer;
            bv2.isStatic = true;

        }

    }
}