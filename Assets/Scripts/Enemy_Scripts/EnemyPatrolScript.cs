using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolScript : MonoBehaviour
{
    public Transform[] PatrolRoute;
    [SerializeField] private Transform CurrentDestination;

    private Rigidbody2D RB;
    private int currentPosInArray = 0;

    NavMeshAgent NMA;

    private bool foundPlayer = false;


    [SerializeField] private enum State
    {
        Idle,
        Patrol,
        Searching,
        Chaseing
    }

    [SerializeField] private State currentState = State.Patrol;

    private void Awake()
    {
        CurrentDestination = PatrolRoute[0];
        NMA = GetComponent<NavMeshAgent>();
        RB = GetComponent<Rigidbody2D>();        
    }
    public void GoToNextPoint(Vector2 StartPoint, Vector2 EndPoint)
    {
        RB.SetRotation(transform.rotation);
    }

    private void Update()
    {
        if (!foundPlayer)
        {

        }
        switch(currentState)
        {
            case State.Idle:
                break;
            case State.Patrol:
                NMA.destination = CurrentDestination.transform.position;
                break;
            case State.Searching:
                break;
            case State.Chaseing:
                break;
        }

        if(RB.transform.position == CurrentDestination.transform.position)
        {
            CurrentDestination.transform.position = PatrolRoute[currentPosInArray].transform.position;
            currentPosInArray++;
        }
    }
}
