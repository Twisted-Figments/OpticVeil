using UnityEngine;
using UnityEngine.AI;

public class NavMesh : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform TargetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.destination = TargetPos.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
