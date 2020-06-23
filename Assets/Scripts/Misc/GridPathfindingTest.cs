using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine.AI;

public class GridPathfindingTest : MonoBehaviour
{
    public GridMapData  gridMapData;
    public Vector2Int   startPoint;
    public Vector2Int   endPoint;
    private GridMapViewer gridMapViewer;
    private string testResutls = "";

    public enum Algorithms
    {
       AStar,
       JPS,
       UnityNavigate,
    }
    public  Algorithms algorithms;
    private Algorithms lastTestAlgorithms;

    class TestState
    {
        public string Name;
        public Color PathColor = Color.green;
        public Color OpenNodeColor = Color.white;
        public Color VisitedNodeColor = Color.yellow;
        public List<Vector2Int>  PathResult;
        public List<PathingNode> OpenNodes = new List<PathingNode>();
        public HashSet<PathingNode> VisitedNodes = new HashSet<PathingNode>();
        public double CostTimeMS;
        public string extendMsg = string.Empty;

        public override string ToString()
        {
            if( PathResult != null )
            {
                return string.Format("{0} Found Path: {1} Node, CostTime={2}ms, VisitedNodeCount={3},OpenNodeCount={4},ExtendMsg={5}\n",
                    Name, PathResult.Count, CostTimeMS, VisitedNodes.Count, OpenNodes.Count, extendMsg);
            }
            else
            {
                return string.Format("{0} Not Found Path: CostTime={1}ms, VisitedNodeCount={2},OpenNodeCount={3}, ExtendMsg={4}\n",
                    Name, CostTimeMS, VisitedNodes.Count, OpenNodes.Count, extendMsg);
            }   
        }
    }

    TestState JPSTest()
    {
        TestState state = new TestState() { Name = "JPS" };
        if (gridMapData == null)
        {
            Debug.LogError("Please assign gridMapData!");
            return state;
        }
       

        gridMapViewer = Object.FindObjectOfType<GridMapViewer>();
        if (gridMapViewer != null)
        {
            gridMapViewer.ClearHighlight();
            Debug.LogWarning("Get GridMapViewer!");
        }

        GridGraph testGraph = new JPSGridGraph(Utils.Array1DTo2D(gridMapData.data,
            gridMapData.gridWidth, gridMapData.gridHeight));

        JPSPathfinder pathfinder = new JPSPathfinder();
        pathfinder.Initialize(testGraph, 100000);
        pathfinder.OnNodeAddOpenSet += (node) =>
        {
            state.OpenNodes.Add(node);
        };
        pathfinder.OnNodeVisited += (node) =>
        {
            state.VisitedNodes.Add(node);
        };

        System.DateTime start = System.DateTime.Now;

        state.PathResult = pathfinder.FindPath(new Vector2Int(startPoint.x, startPoint.y),
                   new Vector2Int(endPoint.x, endPoint.y));

        state.CostTimeMS = (int)(System.DateTime.Now - start).TotalMilliseconds;
        state.extendMsg = string.Format("JumpCostTime={0}ms", (int)pathfinder.JumpCostTime);
        return state;
    }


    TestState AStarTest()
    {
        TestState state = new TestState() { Name = "AStar", VisitedNodeColor = Color.yellow*0.6f };
        if (gridMapData == null)
        {
            Debug.LogError("Please assign gridMapData!");
            return state;
        }


        gridMapViewer = Object.FindObjectOfType<GridMapViewer>();
        if (gridMapViewer != null)
        {
            gridMapViewer.ClearHighlight();
            Debug.LogWarning("Get GridMapViewer!");
        }

        GridGraph testGraph = new GridGraph(Utils.Array1DTo2D(gridMapData.data,
            gridMapData.gridWidth, gridMapData.gridHeight));

        IPathfinder pathfinder = new AtarPathfinder();
        pathfinder.Initialize(testGraph, 100000);
        pathfinder.OnNodeAddOpenSet += (node) =>
        {
            state.OpenNodes.Add(node);
        };
        pathfinder.OnNodeVisited += (node) =>
        {
            state.VisitedNodes.Add(node);
        };

        System.DateTime start = System.DateTime.Now;

        state.PathResult = pathfinder.FindPath(new Vector2Int(startPoint.x, startPoint.y),
                   new Vector2Int(endPoint.x, endPoint.y));

        state.CostTimeMS = (int)(System.DateTime.Now - start).TotalMilliseconds;
        return state;
    }


    TestState UnityNavigateTest()
    {
        TestState state = new TestState() { Name = "UnityNavigate" };

        //GameObject agent = new GameObject("UnityNavigateAgent");
        //agent.transform.SetParent(transform);
        //NavMeshAgent navAgent = agent.AddComponent<NavMeshAgent>();
        //navAgent.updatePosition = false;
        //NavMesh.pathfindingIterationsPerFrame = int.MaxValue;
       

        Vector3 startPos = new Vector3(this.startPoint.x + 0.5f, 0, this.startPoint.y + 0.5f);
        Vector3 endPos = new Vector3(this.endPoint.x + 0.5f, 0, this.endPoint.y + 0.5f);
        
        NavMeshPath path = new NavMeshPath();
        //agent.transform.position = startPos;

        System.DateTime start = System.DateTime.Now;
        bool success = false;
        int safeNum = 0;
        while( ++safeNum < 199 )
        {
            path.ClearCorners();
            success = NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);
            if (success)
            {
                if (state.PathResult == null)
                    state.PathResult = new List<Vector2Int>();
                for (int i = 0; i < path.corners.Length; ++i)
                {
                    Vector3 p = path.corners[i];
                    state.PathResult.Add(new Vector2Int((int)p.x, (int)p.z));
                }

                startPos = path.corners[path.corners.Length - 1];
                if (path.status == NavMeshPathStatus.PathComplete)
                    break;
            }
            else
            {
                break;
            }
        }
        
        state.CostTimeMS = (int)(System.DateTime.Now - start).TotalMilliseconds;
        state.extendMsg = string.Format("success={0},status={1},safeNum={2}", success, path.status.ToString(),safeNum);
        return state;
    }


    void DrawPathLine( TestState state )
    {
        string childName = state.Name + "_PathLine";
        Transform child = transform.Find(childName);
        if(child == null)
        {
            child = new GameObject(childName).transform;
            child.SetParent(transform);
            child.localPosition = Vector3.zero;
        }

        LineRenderer lr = child.GetComponent<LineRenderer>();
        if( lr == null )
        {
            lr = child.gameObject.AddComponent<LineRenderer>();
            lr.startColor = lr.endColor = Color.green;
            lr.useWorldSpace = true;
            lr.startWidth = lr.endWidth = 8f;

            Material mat = new Material(Shader.Find("Unlit/Color"));
            mat.SetColor("_Color", state.PathColor);
            lr.material = mat;
        }
        if (state.PathResult != null)
        {
            lr.positionCount = state.PathResult.Count;
            for (int i = 0; i < state.PathResult.Count; ++i)
            { 
                Vector3 pos = new Vector3(state.PathResult[i].x + 0.5f, 0.5f, state.PathResult[i].y + 0.5f);
                lr.SetPosition(i, pos);
            }  
        }
    }


    void DrawOpenNodes(TestState state)
    {
        if (state.OpenNodes == null)
            return;

        int nodeCount = state.OpenNodes.Count;
        if (nodeCount == 0)
            return;

        Vector3[] verts = new Vector3[nodeCount * 4];
        int[] indecs = new int[nodeCount * 6];
        Color[] colors = new Color[nodeCount * 4];

        int v = 0;
        int i = 0;

        for( int k = 0; k < nodeCount; ++k )
        {
            PathingNode node = state.OpenNodes[k];
            verts[v] = new Vector3(node.Location.x, 0, node.Location.y);
            verts[v+1] = new Vector3(node.Location.x, 0, node.Location.y+1);
            verts[v+2] = new Vector3(node.Location.x+1, 0, node.Location.y+1);
            verts[v+3] = new Vector3(node.Location.x+1, 0, node.Location.y);

            Color color = state.VisitedNodes.Contains(node) ? state.VisitedNodeColor : state.OpenNodeColor;
            colors[v] = colors[v + 1] = colors[v + 2] = colors[v + 3] = color;


            indecs[i++] = v;
            indecs[i++] = v + 1;
            indecs[i++] = v + 2;

            indecs[i++] = v;
            indecs[i++] = v + 2;
            indecs[i++] = v + 3;

            v += 4;
        }

        string childName = state.Name + "_OpenNodes";
        Transform child = transform.Find(childName);
        if (child == null)
        {
            child = new GameObject(childName).transform;
            child.SetParent(transform);
            child.localPosition = new Vector3(0, 0.1f, 0);
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = verts;
        mesh.triangles = indecs;
        mesh.colors = colors;
        

        MeshFilter mf = child.gameObject.GetComponent<MeshFilter>();
        if(mf == null)
        {
            mf = child.gameObject.AddComponent<MeshFilter>();
        }

        mf.sharedMesh = mesh;

        MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
        if(mr == null)
        {
            mr = child.gameObject.AddComponent<MeshRenderer>();
            Material mat = new Material(Shader.Find("Unlit/NewUnlitColor"));
            //mat.SetColor("_Color",  state.OpenNodeColor);
            mr.material = mat;
        }     
    }

    // Use this for initialization
    void Start()
    {
        string testCase = string.Empty;
        if( gridMapData != null )
        {
            testCase = string.Format("TestCase: GridSize={0}x{1} ({2})\n",
                gridMapData.gridWidth, gridMapData.gridHeight, gridMapData.gridWidth * gridMapData.gridHeight);

            GridGraph testGraph = new GridGraph(Utils.Array1DTo2D(gridMapData.data,
                gridMapData.gridWidth, gridMapData.gridHeight));

            startPoint = testGraph.GetNearestNavigablePos(this.startPoint);
            endPoint = testGraph.GetNearestNavigablePos(this.endPoint);
        }
    }

    GUIStyle fontStyle;
    private void OnGUI()
    {
        if( !string.IsNullOrEmpty(testResutls) )
        {
            if(fontStyle==null)
            {
                fontStyle = new GUIStyle();
                fontStyle.normal.background = null;
                fontStyle.normal.textColor = Color.white;
                fontStyle.fontSize = 20;
                fontStyle.fontStyle = FontStyle.BoldAndItalic;
            }           
            GUI.Label(new Rect(10,10,600,500), testResutls, fontStyle);
        }

        if( GUI.Button(new Rect(Screen.width*0.5f-50,Screen.height-50f,100,30), "TEST") )
        {
            if(lastTestAlgorithms != this.algorithms )
            {
                for (int i = transform.childCount - 1; i >= 0; --i)
                {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }
            }

            TestState state = null;
            switch (algorithms)
            {
                case Algorithms.AStar:
                    state = AStarTest();
                    break;
                case Algorithms.JPS:
                    state = JPSTest();
                    break;
                case Algorithms.UnityNavigate:
                    state = UnityNavigateTest();
                    break;
            }
            lastTestAlgorithms = algorithms;
            testResutls += state.ToString();
            Debug.Log(testResutls);
            DrawPathLine(state);
            DrawOpenNodes(state);
        }
    }
}
