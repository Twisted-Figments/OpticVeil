using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTrapTrigger : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] Alltraps;
    public Trap ClosestTrap;

    private void Awake()
    {
        Alltraps = GameObject.FindGameObjectsWithTag("TrapBase");
    }

    void Update()
    {
        //ClosestTrap = FindAnyObjectByType<Trap>();

        GameObject nearestTrap = Alltraps[0];
        float distanceToTrap = Vector2.Distance(Player.transform.position, nearestTrap.transform.position);

        for(int i = 0; i < Alltraps.Length; i++)
        {
            float distanceToCurrent = Vector2.Distance(Player.transform.position, Alltraps[i].transform.position);

            if(distanceToCurrent < distanceToTrap)
            {
                nearestTrap = Alltraps[i];
                distanceToTrap = distanceToCurrent;
            }
        }

        ClosestTrap = nearestTrap.GetComponent<Trap>();
    }

    public void TriggerTrap(InputAction.CallbackContext context)
    {
        if (ClosestTrap == null) { return; }
        Debug.Log("TriggerTrap");
        ClosestTrap.EngageTrap();
    }
}
