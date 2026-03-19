using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class Keycard : MonoBehaviour, IInteractable
{
    public bool IsCollected { get; private set; }
    public string KeycardID { get; private set; }

    public GameObject[] LockedDoor;

    public GameObject[] Lock;

    void Start()
    {
        KeycardID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public bool CanInteract()
    {
        return !IsCollected;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        CollectKeycard();
    }

    private void CollectKeycard()
    {
        SetCollected(true);
    }

    public void SetCollected(bool collected)
    {
        if (IsCollected = collected)
        {
            gameObject.SetActive(false);
            foreach(GameObject locked in Lock)
            {
                locked.SetActive(false);
            }

            foreach (GameObject door in LockedDoor)
            {
             door.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
