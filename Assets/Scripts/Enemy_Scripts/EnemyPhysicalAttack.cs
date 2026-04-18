using UnityEngine;

public class EnemyPhysicalAttack : MonoBehaviour
{
    public Transform ParentTransform;

    public LayerMask PlayerLayer;
    public GameObject KnifeRotationObject;

    public bool PlayerCloseEnough = false;
    public int KnifeSwingDistance = 2;

    public bool HitUp;
    public bool HitDown;

    void Update()
    {
        PlayerCloseEnough = Physics2D.Raycast(ParentTransform.position, ParentTransform.right * KnifeSwingDistance, PlayerLayer);

        if(PlayerCloseEnough == true)
        {
            if(this.transform.rotation.z >= this.transform.rotation.z - 30 && HitDown != true)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, KnifeRotationObject.transform.rotation.z + 1));
                if(KnifeRotationObject.transform.rotation.z >= 30)
                {
                    HitDown = true;
                    HitUp = false;
                }
            }
            else if(this.transform.rotation.z >= this.transform.rotation.z + 30 && HitUp != true)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, KnifeRotationObject.transform.rotation.z - 1));
                if (KnifeRotationObject.transform.rotation.z <= -30)
                {
                    HitDown = false;
                    HitUp = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(ParentTransform.position, ParentTransform.position + (ParentTransform.right * KnifeSwingDistance));
    }
}
