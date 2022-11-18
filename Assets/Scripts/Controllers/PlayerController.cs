using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    Animator playerAnimator;
    float walkSpeed = 3.0f;
    Vector2 movement = new Vector2(0, 0);

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerAnimator.SetFloat("LookX", 1.0f);
        playerAnimator.SetFloat("LookY", -1.0f);
    }

    void Update()
    {
        movement = Vector2.zero;
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        if (movement.x > 0.5f || movement.y > 0.5f || movement.x < -0.5f || movement.y < -0.5f) {
            playerAnimator.SetBool("IsWalking", true);

            if (movement.x > 0.5f) {
                playerAnimator.SetFloat("LookX", 1.0f);
                playerAnimator.SetFloat("LookY", 1.0f);
            } else if (movement.x < -0.5f) {
                playerAnimator.SetFloat("LookX", -1.0f);
                playerAnimator.SetFloat("LookY", 1.0f);
            } else {
                playerAnimator.SetFloat("LookX", 0.0f);
            }
            
            if (movement.y > 0.5f) {
                playerAnimator.SetFloat("LookY", 1.0f);
            } else if (movement.y < -0.5f) {
                playerAnimator.SetFloat("LookY", -1.0f);
            } else {
                playerAnimator.SetFloat("LookY", 0.0f);
            }
        } else {
            playerAnimator.SetBool("IsWalking", false);
        }
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
