using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player_Movement : MonoBehaviour
{
    public Vector2 mousePos;
    private Vector2 movementDirection;
    private Rigidbody2D rb;

    public int runSpeed = 25;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        movementDirection = value.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movementDirection.x * runSpeed * Time.deltaTime, movementDirection.y * runSpeed * Time.deltaTime);
    }

    public void OnMousePosition(InputAction.CallbackContext value)
    {
        mousePos = value.ReadValue<Vector2>();
    }

    private void Update()
    {
        Vector3 mouseScreenPos = Camera.main.ScreenToWorldPoint(mousePos);
        Rotate(mouseScreenPos);
    }

    private void Rotate(Vector2 mouseScreenPos)
    {
        Vector2 distance = mouseScreenPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "HidingSpot")
        {
            this.gameObject.layer = 7;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "HidingSpot")
        {
            this.gameObject.layer = 9;
        }
    }
}
