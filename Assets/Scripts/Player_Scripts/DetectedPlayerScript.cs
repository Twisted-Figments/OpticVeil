using System;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class DetectedPlayerScript : MonoBehaviour
{
    private bool BeenFound;
    private GameObject Enemy;
    public float TempX;
    public float TempY;

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            Enemy = collision.gameObject;
            float TempX = MathF.Round(this.transform.position.x);
            float TempY = MathF.Round(this.transform.position.x);

            Enemy.GetComponent<EnemyPatrolScript>().FoundTarget(new Vector2(TempX, TempY));
            Enemy.GetComponent<EnemyPatrolScript>().StateChange(4); // changes the state of the enemy to chase
        }
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Enemy = collision.gameObject;
            float TempX = MathF.Round(this.transform.position.x);
            float TempY = MathF.Round(this.transform.position.x);

            Enemy.GetComponent<EnemyPatrolScript>().FoundTarget(new Vector2(TempX, TempY));
            Enemy.GetComponent<EnemyPatrolScript>().StateChange(4); // changes the state of the enemy to chase
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Enemy.GetComponent<EnemyPatrolScript>().ReturnToPatrol();
            Enemy.GetComponent<EnemyPatrolScript>().StateChange(2); // changes the state of the enemy to patrol
        }
    }
}
