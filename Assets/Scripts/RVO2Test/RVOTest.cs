using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RVOTest : MonoBehaviour
{

    public int robotCount = 20;
    public float circleRadius = 20;

    public float neighborDist = 10;
    public int maxNeighbors = 1000;
    public float timeHorizon = 0.1f;
    public float timeHorizonObst = 0.1f;
    public float radius = 0.5f;
    public float speed = 2;
    public float maxSpeed = 2;
    public Vector2 velocity = Vector2.zero;

    public bool AddObstacleTest = false;
    public Vector3 ObstacleCenterPos = Vector3.zero;
    public float ObstacleRadius = 1;
  


    private Transform[] robots;
    private int[] robotAgentIds;
    private RVO.Vector2[] goals;

    System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        RVO.Simulator.Instance.setTimeStep(0.03f);
        RVO.Simulator.Instance.setAgentDefaults(neighborDist, maxNeighbors, timeHorizon, timeHorizonObst,
            radius, maxSpeed, new RVO.Vector2(velocity.x, velocity.y));

        if(AddObstacleTest)
        {
            float ObstacleExtend = ObstacleRadius + 0.5f;
            List<RVO.Vector2> obstacleVerts = new List<RVO.Vector2>()
            {
                new RVO.Vector2( ObstacleCenterPos.x - ObstacleExtend, ObstacleCenterPos.z - ObstacleExtend ),
                new RVO.Vector2( ObstacleCenterPos.x - ObstacleExtend, ObstacleCenterPos.z + ObstacleExtend ),
                new RVO.Vector2( ObstacleCenterPos.x + ObstacleExtend, ObstacleCenterPos.z + ObstacleExtend),
                new RVO.Vector2( ObstacleCenterPos.x + ObstacleExtend, ObstacleCenterPos.z - ObstacleExtend),
            };
            RVO.Simulator.Instance.addObstacle(obstacleVerts);
        }
      


        float angleStep = Mathf.PI*2 / robotCount;
        robots = new Transform[robotCount];
        robotAgentIds = new int[robotCount];
        goals = new RVO.Vector2[robotCount];

        for ( int i = 0; i < robotCount; ++i )
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Color clr = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            go.GetComponent<MeshRenderer>().material.SetColor("_Color", clr);
            robots[i] = go.transform;
            robots[i].localScale = new Vector3(radius, radius, radius);
            robots[i].localPosition = Vector3.zero;

            Vector3 initPos = new Vector3(circleRadius*Mathf.Cos(i * angleStep), 0,
                                             circleRadius*Mathf.Sin(i * angleStep));
            robots[i].position = initPos;

            goals[i] = new RVO.Vector2( -initPos.x, -initPos.z) ;

            robotAgentIds[i] = RVO.Simulator.Instance.addAgent(new RVO.Vector2(initPos.x, initPos.z));
            //RVO.Simulator.Instance.setAgentMaxSpeed(robotAgentIds[i], UnityEngine.Random.Range(1f, 2f));
        }

        random = new System.Random();
        Invoke("Run", 2);
    }

    bool isRunning = false;
    void Run()
    {
        isRunning = true;
    }


    // Update is called once per frame
    float fps;
    int fpsCounter;
    float fpsTimer;
    void Update()
    {
      

        fpsTimer += Time.deltaTime;
        fpsCounter++;
        if( fpsTimer >= 1.0f )
        {
            fps = fpsCounter / fpsTimer;
            fpsTimer = 0;
            fpsCounter = 0;
        }

        if (!isRunning)
            return;


        int numAgents = RVO.Simulator.Instance.getNumAgents();

        for ( int  i = 0; i < numAgents; ++i )
        {
            RVO.Vector2 curPos = RVO.Simulator.Instance.getAgentPosition( robotAgentIds[i] );
            robots[i].position = new Vector3(curPos.x(), 0, curPos.y());

           
            RVO.Vector2 goalVector = goals[i] - curPos;
            if (RVO.RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVO.RVOMath.normalize(goalVector)* speed; 
            }
            RVO.Simulator.Instance.setAgentPrefVelocity(robotAgentIds[i], goalVector);

            /* Perturb a little to avoid deadlocks due to perfect symmetry. */
            float angle = (float)random.NextDouble() * 2.0f * (float)Mathf.PI;
            float dist = (float)random.NextDouble() * 0.1f;

            RVO.Simulator.Instance.setAgentPrefVelocity(robotAgentIds[i],
                RVO.Simulator.Instance.getAgentPrefVelocity(robotAgentIds[i]) +
                dist * new RVO.Vector2((float)Mathf.Cos(angle), (float)Mathf.Sin(angle)));
        }

        RVO.Simulator.Instance.doStep();

    }

    private void OnDrawGizmos()
    {
        if(AddObstacleTest)
        {
            Gizmos.DrawSphere(ObstacleCenterPos, ObstacleRadius);
        }
        
        for (int i = 0; i < robotCount; ++i)
        {
           // Gizmos.DrawWireCube(new Vector3(goals[i].x(), 0, goals[i].y()), Vector3.one);
        }
    }


    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width/2, 20, 100, 30), string.Format("FPS: {0:F1}", fps));
    }

}
