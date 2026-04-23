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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(ParentTransform.position, ParentTransform.position + (ParentTransform.right * KnifeSwingDistance));
    }
}
