using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    float walkSpeed = 3.0f;
    Vector2 movement = new Vector2(0, 0);

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement = Vector2.zero;
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 position = playerRigidbody.position;
        Vector2 newPos = position;

        newPos.x += walkSpeed * movement.x * Time.deltaTime;
        newPos.y += walkSpeed * movement.y * Time.deltaTime;

        playerRigidbody.MovePosition(newPos);
    }
}
