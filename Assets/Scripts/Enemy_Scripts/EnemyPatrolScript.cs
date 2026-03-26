using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyPatrolScript : MonoBehaviour
{
    public Transform[] PatrolRoute;
    public int currentPatrolPosition = 0;
    //[SerializeField] private Transform CurrentDestination;

    private Rigidbody2D RB;
    private PathfindingCells PFC;

    [SerializeField] private Vector2 currentTarget;
    [SerializeField] private int currentPosInArray = 0;

    NavMeshAgent NMA;

    private bool behindWall = false;
    public LayerMask WallLayer;
    public int SearchRange;
    private bool InRange = false;

    private bool foundPlayer = false;

    public int runSpeed = 5;
    public float LocationDistance = 0.25f;
    private Player_Movement PlayerPos;

    [SerializeField] Transform playerRef;

    [SerializeField] public enum State
    {
        Idle,
        Patrol,
        Searching,
        Chaseing
    }

    [SerializeField] public State currentState = State.Patrol;

    public void StateChange(int Pick)
    {
        switch(Pick)
        {
            case 1:
                currentState = State.Idle;
                break;
            case 2:
                currentState = State.Patrol;
                break;
            case 3:
                currentState = State.Searching;
                break;
            case 4:
                currentState = State.Chaseing;
                break;
            default:
                return;
        }
    }

    private void Awake()
    {
        NMA = GetComponent<NavMeshAgent>();
        RB = GetComponent<Rigidbody2D>();    
        PFC = FindAnyObjectByType<PathfindingCells>();
        PlayerPos = FindAnyObjectByType<Player_Movement>();
        playerRef = FindAnyObjectByType<Player_Movement>().transform;
    }

    private void Start()
    {
        //Invoke("ResetTarget", 5);

        ReturnToPatrol();
    }

    public void GoToNextPoint(Vector2 GoTo)
    {
        Rotate(GoTo);
    }

    public void FoundTarget(Vector2 TargetPos)
    {
        float TempX = MathF.Round(this.transform.position.x);
        float TempY = MathF.Round(this.transform.position.y);
        currentPosInArray = 0;
        PFC.ClearPath();
        PFC.GenerateGrid();
        PFC.pathsGenerated = false;
        PFC.GeneratePath(new Vector2(TempX, TempY), TargetPos);
    }

    public void ResetTarget(Vector2 targetPos)
    {
        float TempX = MathF.Round(this.transform.position.x);
        float TempY = MathF.Round(this.transform.position.y);
        PFC.ClearPath();
        PFC.GenerateGrid();
        PFC.pathsGenerated = false;
        PFC.GeneratePath(new Vector2(TempX, TempY), targetPos);
        //currentPatrolPosition++;
    }

    public void ReturnToPatrol()
    {
        float TempX = MathF.Round(this.transform.position.x);
        float TempY = MathF.Round(this.transform.position.y);
        PFC.ClearPath();
        PFC.GenerateGrid();
        PFC.pathsGenerated = false;
        PFC.GeneratePath(new Vector2(TempX, TempY), new Vector2(PatrolRoute[currentPatrolPosition].transform.position.x, PatrolRoute[currentPatrolPosition].transform.position.y));
        //currentPatrolPosition++;
    }

    private void FixedUpdate()
    {
        RB.linearVelocity = transform.right.normalized * runSpeed;
    }

    public void Rotate(Vector2 LookAt)
    {
        Vector2 distance = LookAt - (Vector2)transform.position;
        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Update()
    {
        switch(currentState)
        {
            case State.Idle:
                break;
            case State.Patrol:

                RayCastSearch();
                if(foundPlayer == true && InRange) { currentState = State.Chaseing; FoundTarget(PlayerPos.transform.position); break; }

                GoToNextPoint(currentTarget);

                if (!PFC.pathsGenerated) { return; }
                
                OverrideGoToList(PFC.finalPath[currentPosInArray]);

                break;
            case State.Searching:
                break;
            case State.Chaseing:

                //GoToNextPoint(currentTarget);

                if (!PFC.pathsGenerated) { return; }
                RayCastSearch();
                if (!foundPlayer || !InRange) { currentState = State.Patrol; ReturnToPatrol(); break; }

                FoundTarget(playerRef.transform.position);

                OverrideGoToList(playerRef.transform.position);
                GoToNextPoint(currentTarget);

                Debug.Log("Being called");

                // IF WE DON'T WANT THE ENEMY TO CHASE THE PLAYER - 

                // ENEMY HAS RAYCAST TOWARDS PLAYER TO SEE IF VIEW IS HITTING A WALL / DONE

                // ENEMY GETS DIRECTION TO PLAYER = enemy.POS - player.POS / Working on it

                // ENEMY COMPARES IT'S FORWARD DIRECTION TO THE ENEMY -> PLAYER DIRECTION AND MAKES SURE IT'S SMALLER THAN VIEW CONE = IF(VECTOR3.ANGLE(DIRTOPLAYER, ENEMYFORWARD) < VISIONVIEWCONESIZE) // need to add view cone

                // ENEMY CAN SEE PLAYER
                break;
        }
    }

    private void RayCastSearch()
    {
        behindWall = Physics2D.Linecast(transform.position, PlayerPos.transform.position, WallLayer);
        foundPlayer = !behindWall;
        if(behindWall) { return; }

        InRange = Vector2.Distance(transform.position, PlayerPos.transform.position) < SearchRange;
        if(!InRange) { return; }

    }

    void OverrideGoToList(Vector2 target)
    {
        if (Vector2.Distance(this.transform.position, currentTarget) < LocationDistance)
        {
            Debug.Log("Read Distance");
            //currentPosInArray = Mathf.Clamp(currentPosInArray + 1, 0, PFC.finalPath.Count - 1);
            currentPosInArray++;
            currentTarget = target;
            if (currentPosInArray == PFC.finalPath.Count - 1)
            {
                if (currentPatrolPosition == PatrolRoute.Length - 1)
                {
                    currentPatrolPosition = 0;
                    currentPosInArray = 0;
                    ResetTarget(PatrolRoute[currentPatrolPosition].transform.position);
                    return;
                }

                Debug.Log("Read CurrentPosInArray");
                currentPosInArray = 0;
                currentPatrolPosition++;
                ResetTarget(PatrolRoute[currentPatrolPosition].transform.position);
            }
        }

        else if (currentTarget == Vector2.zero)
        {
            currentPosInArray = 0;
            currentTarget = PFC.finalPath[currentPosInArray];
        }
    }

    private void OnDrawGizmos()
    {
        if(!behindWall && InRange)
        {
            Gizmos.DrawLine(transform.position, PlayerPos.transform.position);
        }
    }
}
