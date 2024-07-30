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
    public float speed = 40;
    public float maxSpeed = 300;
    public float crouchMultiplier = 0.2f;
    private float moving = 0;

    public float weight = 50; // TODO add weight to players, weight and speed changes depending on items that have been picked up.
    // possibly have every player start with (slightly) different weights
    // when u start moving it applies a slight weight boost, allowing button mashing to overpower someone holding down a movement key

    // jump settings
    private bool jumped = false;
    public float jumpHeight = 4.75f;

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
        // start acceleration - TODO change to apply motion input onto movement vector
        // movement vector always tries to return to zero motion
        if (mv.movement[id].x != 0)
        {
            moving += speed * mv.movement[id].x;
            if (moving > maxSpeed)
            {
                moving = maxSpeed;
            } else if (moving < -maxSpeed)
            {
                moving = -maxSpeed;
            }
        } 
        else if (mv.movement[id].x == 0)
        {
            if (moving < -maxSpeed/10)
            {
                moving += maxSpeed / 10;
            }
            else if (moving > maxSpeed/10)
            {
                moving -= maxSpeed / 10;
            } else
            {
                moving = 0;
            }
        }
        float finalSpeed = moving;

        // crouch
        if (mv.movement[id].y < 0)
        {
            if (transform.localScale == new Vector3(1, 1, 1))
            {
                transform.localScale = new Vector3(1, 0.5f, 1);
                transform.position = new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D>().size.y/4), 0);
            }
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

        Vector2 trans = new Vector3(finalSpeed * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = trans;
    }

    void Jump()
    {
        jumped = true;
        rb.AddForce(Vector2.up * jumpHeight * 100);
    }
    void Crouch()
    {
        transform.localScale = new Vector3(1, 0.5f, 1);
    }
}
