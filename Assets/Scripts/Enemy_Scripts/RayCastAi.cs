using System;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class RayCastAi : MonoBehaviour
{
    private Player_Movement PlayerLoc;
    private Rigidbody2D RB;

    public LayerMask WallLayer;

    public float RaycastDistance;
    public float RayCastConeSize;

    [SerializeField] private bool WallToLeft;
    [SerializeField] private bool WallToRight;
    [SerializeField] private bool WallToTop;
    [SerializeField] private bool WallToBot;

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

        PlayerLoc = FindAnyObjectByType<Player_Movement>();
        RB = GetComponent<Rigidbody2D>();
        this.gameObject.transform.rotation = UnityEngine.Random.rotation;
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
                Searching();
                break;
            case State.chaseing:
                InChase();
                break;
        }
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
        Rotate(PlayerLoc.transform.position);
        RB.linearVelocity = transform.right.normalized * Speed;
    }

    private void Rotate(Vector2 LookAt)
    {
        Vector2 distance = LookAt - (Vector2)transform.position;
        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Searching()
    {
        WallToLeft = Physics2D.Raycast(transform.position, transform.right + transform.up * RayCastConeSize, RaycastDistance, WallLayer);
        WallToRight = Physics2D.Raycast(transform.position, transform.right - transform.up * RayCastConeSize, RaycastDistance, WallLayer);

        WallToTop = Physics2D.Raycast(transform.position, transform.up + transform.up * RayCastConeSize, RaycastDistance, WallLayer);
        WallToBot = Physics2D.Raycast(transform.position, -transform.up + transform.up * RayCastConeSize, RaycastDistance, WallLayer);



        if(WallToLeft && WallToRight)
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
