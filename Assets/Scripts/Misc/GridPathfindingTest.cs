using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Runtime.InteropServices;

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
       BidirectionAstar,
       JPS,
       UnityNavigate,
    }
    public  Algorithms algorithms;
    public bool enableDiagonal;
    public HeuristicDistanceMethod heuristicDistanceMethod;
    public double weightOfG = 1.0;
    public double weightOfH = 1.0;
    public double hScale = 0.999;

    private Algorithms lastTestAlgorithms;

    class TestState
    {
        public string Name;
        public IPathfinder pathfinder;
        public Path pathResult;
        public double CostTimeMS;
        public string extendMsg = string.Empty;
        public  GridGraph.Node[,] AllGraphNodes;
        public int totalOpenNodeCount;
        public int visitedNodeCount;
        
        public override string ToString()
        {
            if (AllGraphNodes != null && pathResult != null && pathResult.HasPath() )
            {
                return $"Map: {AllGraphNodes.GetLength(0)}x{AllGraphNodes.GetLength(1)}\nAlgorithm:{Name}\n" +
                    $"Found Path:{pathResult.NodeCount}\nCostTime={CostTimeMS:F2}ms\nVisitedNodeCount={visitedNodeCount}" +
                    $"\ntotalOpenNodeCount={totalOpenNodeCount}\n";
            }
            else
            {
                if( string.IsNullOrEmpty(extendMsg))
                {
                    return string.Format("{0} Not Found Path: CostTime={1:F2}ms, VisitedNodeCount={2},totalOpenNodeCount={3}\n",
                                       Name, CostTimeMS, visitedNodeCount, totalOpenNodeCount);
                }
                else
                {
                    return string.Format("{0} Not Found Path: CostTime={1:F2}ms, VisitedNodeCount={2},totalOpenNodeCount={3},ext={4}\n",
                                      Name, CostTimeMS, visitedNodeCount, totalOpenNodeCount, extendMsg);
                }
               
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
        state.AllGraphNodes = testGraph._grid;

        JPSPathfinder pathfinder = new JPSPathfinder();
        pathfinder.Initialize(testGraph, 100000);
        pathfinder.OnNodeAddOpenSet += (node) =>
        {
            ++state.totalOpenNodeCount;
        };
        pathfinder.OnNodeVisited += (node) =>
        {
            ++state.visitedNodeCount;
        };
        state.pathfinder = pathfinder;

        System.DateTime start = System.DateTime.Now;

        state.pathResult = pathfinder.FindPath(new Vector2Int(startPoint.x, startPoint.y),
                   new Vector2Int(endPoint.x, endPoint.y));

        state.CostTimeMS = (System.DateTime.Now - start).TotalMilliseconds;
        state.extendMsg = string.Format("JumpCostTime={0:F2}ms", pathfinder.JumpCostTime);
        return state;
    }


    TestState AStarTest()
    {
        TestState state = new TestState() { Name = "AStar" };
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
        testGraph.EnableDiagonal = enableDiagonal;
        state.AllGraphNodes = testGraph._grid;

        AstarPathfinder astarPathfinder = new AstarPathfinder();
        astarPathfinder.heuristicDistanceMethod = this.heuristicDistanceMethod;
        astarPathfinder.WeightOfG = weightOfG;
        astarPathfinder.WeightOfH = weightOfH;
        astarPathfinder.HScale = hScale;
        state.pathfinder = astarPathfinder;

        IPathfinder pathfinder = astarPathfinder;
        pathfinder.Initialize(testGraph, 100000);
        pathfinder.OnNodeAddOpenSet += (node) =>
        {
            ++state.totalOpenNodeCount;
        };
        pathfinder.OnNodeVisited += (node) =>
        {
            ++state.visitedNodeCount;
        };

        System.DateTime start = System.DateTime.Now;

        state.pathResult = pathfinder.FindPath(new Vector2Int(startPoint.x, startPoint.y),
                   new Vector2Int(endPoint.x, endPoint.y));

        state.CostTimeMS = (System.DateTime.Now - start).TotalMilliseconds;
        return state;
    }


    TestState BidirectionAstarTest()
    {

        TestState state = new TestState() { Name = "BidirectionAStar" };
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
        testGraph.EnableDiagonal = enableDiagonal;
        state.AllGraphNodes = testGraph._grid;

        BidirectionAstarPathfinder astarPathfinder = new BidirectionAstarPathfinder();
        astarPathfinder.heuristicDistanceMethod = this.heuristicDistanceMethod;
        astarPathfinder.WeightOfG = weightOfG;
        astarPathfinder.WeightOfH = weightOfH;
        astarPathfinder.HScale = hScale;
        state.pathfinder = astarPathfinder;

        IPathfinder pathfinder = astarPathfinder;
        pathfinder.Initialize(testGraph, 100000);
        pathfinder.OnNodeAddOpenSet += (node) =>
        {
            ++state.totalOpenNodeCount;

        };
        pathfinder.OnNodeVisited += (node) =>
        {
            ++state.visitedNodeCount;
        };

        System.DateTime start = System.DateTime.Now;

        state.pathResult = pathfinder.FindPath(new Vector2Int(startPoint.x, startPoint.y),
                   new Vector2Int(endPoint.x, endPoint.y));

        state.CostTimeMS = (System.DateTime.Now - start).TotalMilliseconds;
        return state;
    }



    TestState UnityNavigateTest()
    {
        TestState state = new TestState() { Name = "UnityNavigate" };


        GameObject agent = new GameObject("UnityNavigateAgent");
        agent.transform.SetParent(transform);
        NavMeshAgent navAgent = agent.AddComponent<NavMeshAgent>();
        navAgent.updatePosition = false;
        NavMesh.pathfindingIterationsPerFrame = int.MaxValue;
       

        Vector3 startPos = new Vector3(this.startPoint.x + 0.5f, 0, this.startPoint.y + 0.5f);
        Vector3 endPos = new Vector3(this.endPoint.x + 0.5f, 0, this.endPoint.y + 0.5f);
        
        NavMeshPath path = new NavMeshPath();
        agent.transform.position = startPos;

        System.DateTime start = System.DateTime.Now;
        bool success = false;
        int safeNum = 0;
        while( ++safeNum < 199 )
        {
            path.ClearCorners();
            success = NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);
            if (success)
            {
                if (state.pathResult == null){
                    state.pathResult = new Path();
                }
                    
                for (int i = 0; i < path.corners.Length; ++i)
                {
                    Vector3 p = path.corners[i];
                    var node = new PathingNode((int)p.x, (int)p.z);
                    state.pathResult.AddNode(node);
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
        
        state.CostTimeMS = (System.DateTime.Now - start).TotalMilliseconds;
        state.extendMsg = string.Format("success={0},status={1},safeNum={2}", success, path.status.ToString(),safeNum);
        return state;
    }


    void DrawPathLine( TestState state )
    {
        string childName = state.Name + "_PathLine";
        Transform child = transform.Find(childName);
        if (child == null)
        {
            child = new GameObject(childName).transform;
            child.SetParent(transform);
            child.localPosition = Vector3.zero;
        }

        LineRenderer lr = child.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = child.gameObject.AddComponent<LineRenderer>();
            lr.startColor = lr.endColor = Color.green;
            lr.useWorldSpace = true;
            lr.startWidth = lr.endWidth = 0.2F;

            Material mat = new Material(Shader.Find("Unlit/Color"));
            mat.SetColor("_Color", Utils.PathColor0);
            lr.material = mat;
        }
        if (state.pathResult != null)
        {
            lr.positionCount = state.pathResult.nodeList.Count;
            for (int i = 0; i < state.pathResult.nodeList.Count; ++i)
            {
                var localtion = state.pathResult.nodeList[i].Location;
                Vector3 pos = new Vector3(localtion.x + 0.5f, 0.5f, localtion.y + 0.5f);
                lr.SetPosition(i, pos);
            }
        }
    }


    void DrawAllNodes(TestState state)
    {
      
        int colSize = state.AllGraphNodes.GetLength(0);
        int rowSize = state.AllGraphNodes.GetLength(1);

        int nodeCount = colSize * rowSize;
        if (nodeCount == 0)
            return;

        Vector3[] verts = new Vector3[nodeCount * 4];
        int[] indecs = new int[nodeCount * 6];
        Color[] colors = new Color[nodeCount * 4];

        int v = 0;
        int i = 0;
        const float padding = 0.1f;
        for( int col = 0; col < colSize; ++col)
        {
            for( int row = 0; row < rowSize; ++row)
            {
                var node = state.AllGraphNodes[col, row];
                var location = node.location;
              

                verts[v] = new Vector3(location.x + padding, 0, location.y + padding);
                verts[v + 1] = new Vector3(location.x + padding, 0, location.y + 1 - padding);
                verts[v + 2] = new Vector3(location.x + 1 - padding, 0, location.y + 1 - padding);
                verts[v + 3] = new Vector3(location.x + 1 - padding, 0, location.y + padding);

                Color color = state.pathfinder.GetColor(node);
                colors[v] = colors[v + 1] = colors[v + 2] = colors[v + 3] = color;


                indecs[i++] = v;
                indecs[i++] = v + 1;
                indecs[i++] = v + 2;

                indecs[i++] = v;
                indecs[i++] = v + 2;
                indecs[i++] = v + 3;

                v += 4;
            }
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
                fontStyle.normal.textColor = Color.blue;
                fontStyle.fontSize = 22;
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
            bool isGrid = true;
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
                    isGrid = false;
                    break;
                case Algorithms.BidirectionAstar:
                    state = BidirectionAstarTest();
                    break;
            }
            lastTestAlgorithms = algorithms;
            testResutls = state.ToString();
            Debug.Log(testResutls);
            if(!isGrid)
            {
                DrawPathLine(state);
            }
            else
            {
                DrawAllNodes(state);
            }
         
            
        }
    }
}
