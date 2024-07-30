using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // !!!!
    // THIS IS A DEBUG PLAYER MOVEMENT DO NOT USE THIS IN THE FINAL VERSION
    // !!!!

    public static int idCounter;
    public int id;

    private Rigidbody2D rb;
    private InputManager mv;

    // movement settings
    public float speed = 1;
    public float crouchMultiplier = 0.5f;
    private float moving = 0;

    // jump settings
    private bool jumped = false;
    public float jumpHeight = 200;

    public void Start()
    {
        mv = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody2D>();

        id = idCounter;
        idCounter++;

        gameObject.name += " " + id;
    }

    // movement - on physics update
    public void FixedUpdate()
    {
        // start acceleration
        if (mv.movement[id].x != 0 && moving < 10)
        {
            moving += .5f;
        } else if (mv.movement[id].x == 0)
        {
            moving = 0;
        }
        float finalSpeed = speed * moving;

        // crouch
        if (mv.movement[id].y < 0)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
            finalSpeed *= crouchMultiplier;
        } else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // jump
        if (!jumped && mv.movement[id].y > 0 && rb.velocity.y == 0)
        {
            Jump();
        } 
        else
        {
            jumped = false;
        }

        Vector2 trans = new Vector3(mv.movement[id].x * finalSpeed * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = trans;
    }

    void Jump()
    {
        jumped = true;
        rb.AddForce(Vector2.up * jumpHeight);
    }
    void Crouch()
    {
        transform.localScale = new Vector3(1, 0.5f, 1);
    }
}
