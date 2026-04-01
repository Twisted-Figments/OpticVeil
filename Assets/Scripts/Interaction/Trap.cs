using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject TrapDamageCollider;

    private bool Active = false;   
    private void Awake()
    {
        TrapDamageCollider.SetActive(false);
    }

    public void EngageTrap()
    {
        if(Active) { return; }
        TrapDamageCollider.SetActive(true);
        Active = true;
        Invoke("DisengageTrap", 5);
    }

    private void DisengageTrap()
    {
        TrapDamageCollider.SetActive(false);
        Active = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
