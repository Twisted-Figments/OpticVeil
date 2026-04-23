using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RayCastAi : MonoBehaviour
{
    private Player_Movement PlayerLoc;
    private Rigidbody2D RB;

    public LayerMask WallLayer;

    public GameObject ThisGameObject;
    public GameObject[] AllEnemies;
    public List<GameObject> AllEnemiesList;
    public RayCastAi CloestEnemy;

    public float RaycastDistance;
    public float RayCastConeSize;

    [SerializeField] private bool WallToLeft;
    [SerializeField] private bool WallToRight;
    [SerializeField] private bool WallToTop;
    [SerializeField] private bool WallToBot;

    [SerializeField] private bool EnemyInFront;
    [SerializeField] private float EnemyDistanceDetection;
    [SerializeField] private bool HasRotatedAway = false;

    [SerializeField] private bool TempDirectionChosen;

    public bool BehindWall;
    public bool InRange;
    private bool InTrap = false;

    public int SearchDistance;
    public int HP = 10;


    [SerializeField] private bool WallInAngle;
    [SerializeField] private bool AvoidingWall;

    public float Speed;
    public float AvoidingWallMult;

    [SerializeField] private int Turning;

    public float rotationSpeed;

    [SerializeField] private Transform ForWardPos;

    private int directionNum;

    [SerializeField] public enum State
    {
        idle,
        searching,
        chaseing,
    }

    [SerializeField] public State currentAction = State.searching;

    private void Awake()
    {
        ThisGameObject = this.gameObject;
        PlayerLoc = FindAnyObjectByType<Player_Movement>();
        RB = GetComponent<Rigidbody2D>();
        this.gameObject.transform.rotation = UnityEngine.Random.rotation;
        AllEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        for(int i = 0; i < AllEnemies.Length; i++)
        {
            AllEnemiesList.Add(AllEnemies[i]);
            if (AllEnemies[i].gameObject == this.gameObject) { AllEnemiesList.Remove(AllEnemies[i]); }
        }

    }

    void Start()
    {
        this.transform.rotation = quaternion.Euler(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        RayCastSearch();

        switch(currentAction)
        {
            case State.idle:
                break;
            case State.searching:
                LocateNearestEnemy();
                Searching();
                break;
            case State.chaseing:
                InChase();
                break;
        }
    }

    public void LocateNearestEnemy()
    {
        GameObject nearestEnemy = AllEnemiesList[0];
        float distanceToEnemy = Vector2.Distance(ThisGameObject.transform.position, nearestEnemy.transform.position);

        for (int i = 0; i < AllEnemiesList.Count; i++)
        {
            float distanceToCurrent = Vector2.Distance(ThisGameObject.transform.position, AllEnemiesList[i].transform.position);

            if (distanceToCurrent < distanceToEnemy && Vector2.Distance(ThisGameObject.transform.position, nearestEnemy.transform.position) != 0)
            {
                nearestEnemy = AllEnemiesList[i];
                distanceToEnemy = distanceToCurrent;
            }
        }

        CloestEnemy = nearestEnemy.GetComponent<RayCastAi>();

        EnemyInFront = Vector2.Distance(ThisGameObject.transform.position, CloestEnemy.transform.position) < EnemyDistanceDetection;
    }

    private void RayCastSearch()
    {
        BehindWall = Physics2D.Linecast(transform.position, PlayerLoc.transform.position, WallLayer);
        if(BehindWall) { 
            currentAction = State.searching;
            return; 
        }

        InRange = Vector2.Distance(transform.position, PlayerLoc.transform.position) < SearchDistance;
        if(!InRange) {
            currentAction = State.searching;
            return; 
        }
        currentAction = State.chaseing;
    }

    private void InChase()
    {
        if (WallToLeft && WallToTop || WallToRight && WallToBot)
        {
            if (WallToLeft && WallToTop)
            {
                Turning = -1;
            }
            if (WallToRight && WallToBot)
            {
                Turning = 1;
            }

            this.transform.Rotate(0, 0, rotationSpeed * Turning);
        }

        Rotate(PlayerLoc.transform.position);
        RB.linearVelocity = transform.right.normalized * Speed;
    }

    private void Rotate(Vector2 LookAt)
    {
        Vector2 distance = LookAt - (Vector2)transform.position;
        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void RotateAway(int RotDirection)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.z * RotDirection));
    }

    private void Searching()
    {
        WallToLeft = Physics2D.Raycast(transform.position, transform.right + transform.up * RayCastConeSize, RaycastDistance, WallLayer);
        WallToRight = Physics2D.Raycast(transform.position, transform.right - transform.up * RayCastConeSize, RaycastDistance, WallLayer);

        WallToTop = Physics2D.Raycast(transform.position, transform.up + transform.up * RayCastConeSize, RaycastDistance, WallLayer);
        WallToBot = Physics2D.Raycast(transform.position, -transform.up + transform.up * RayCastConeSize, RaycastDistance, WallLayer);

        /*
        if(WallToTop && !WallToBot)
        {
            RotateAway(-1);
        }
        if (!WallToTop && WallToBot)
        {
            RotateAway(1);
        }
        */
        // work on maybe?

        if (WallToLeft && WallToRight)
        {
            AvoidWall();
        }
        if (WallToLeft || WallToRight || !WallToTop || !WallToBot)
        {
            AvoidWall();
        }
        if (!WallToLeft && !WallToRight)
        {
            NoWall();
        }

        Moveing();
    }

    private void Moveing()
    {
        float TempSpeed = Speed;

        if(EnemyInFront && !AvoidingWall && !HasRotatedAway)
        {
            HasRotatedAway = true;
            int TempNum = Random.Range(1, 5);
            //this.transform.rotation.SetLookRotation(CloestEnemy.transform.position);
            //transform.rotation = Quaternion.Euler(new Vector3(0, 0, -transform.rotation.z));
            if(TempNum == 1)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            if (TempNum == 2)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            if (TempNum == 3)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
            if (TempNum == 4)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            /*
            if (transform.rotation.z < 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -180));
            }
            */
            Invoke("RoatateAwayCoolDown", 3);
            //this.transform.Rotate(0, 0, rotationSpeed * Turning);
            return;
        }

        if (AvoidingWall)
        {
            TempSpeed = Speed * AvoidingWallMult; // add var

            if(WallToLeft && WallToRight && WallToTop && WallToBot || WallToLeft && WallToRight)
            {
                if(transform.rotation.z < 0)
                { 
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                else
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, -180));
                }
            }

            if(!WallToTop)
            {
                Turning = -1;
            }
            if(!WallToBot)
            {
                Turning = 1;
            }

            if (WallToLeft && WallToTop)
            {
                Turning = -1;
            }
            if (WallToRight && WallToBot)
            {
                Turning = 1;
            }

            this.transform.Rotate(0, 0, rotationSpeed * Turning);
            return;
        }
        else
        {
            Turning = 0;
        }

        RB.linearVelocity = transform.right.normalized * TempSpeed;
        /*else if(AvoidingWall)
        {
            directionNum = UnityEngine.Random.Range(0,1);

            if(directionNum == 0)
            {
                this.transform.Rotate(0, 0, rotationSpeed);
            }
            else
            {
                this.transform.Rotate(0, 0, -rotationSpeed);
            }
        }*/
    }

    private void RoatateAwayCoolDown()
    {
        HasRotatedAway = false;
    }

    private int RNDDirection()
    {
        int TempDirection = Random.Range(0, 2);
        return TempDirection;
    }

    private void AllowNewDirection()
    {
        TempDirectionChosen = false;
    }

    private void AvoidWall()
    {
        AvoidingWall = true;
    }

    private void NoWall()
    {
        AvoidingWall = false;
    }

    private void OnDrawGizmos()
    {
        if(WallToLeft)
        {
            Gizmos.color = Color.red;
        }
        if (!WallToLeft)
        {
            Gizmos.color = Color.darkGreen;
        }

        Gizmos.DrawLine(transform.position, transform.position + (transform.right * RaycastDistance + transform.up * RayCastConeSize));
        Gizmos.DrawLine(transform.position, transform.position + (transform.right * RaycastDistance - transform.up * RayCastConeSize));

        Gizmos.DrawLine(transform.position, transform.position + (transform.up * RaycastDistance));
        Gizmos.DrawLine(transform.position, transform.position + (-transform.up * RaycastDistance));

        if (!BehindWall && InRange)
        {
            Gizmos.DrawLine(transform.position, PlayerLoc.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Trap")
        {
            if(!InTrap)
            {
                InTrap = true;
                InvokeRepeating("TrapDamage", 0, 1);
            }
        }

        if(collision.tag == "Bullet")
        {
            Destroy(collision.gameObject);
            HP -= 2;
            if(HP <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Trap")
        {
            if (InTrap)
            {
                InTrap = false;
                CancelInvoke();
            }
        }
    }

    private void TrapDamage()
    {
        HP -= 2;
        if(HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }


}
