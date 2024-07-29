using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // !!!!
    // THIS IS A DEBUG PLAYER MOVEMENT DO NOT USE THIS IN THE FINAL VERSION
    // !!!!

    private InputManager mv;
    public float speed = 10;

    public void Start()
    {
        mv = GetComponent<InputManager>();
    }

    // movement - on physics update
    public void FixedUpdate()
    {
        Vector3 trans = new Vector3(mv.movement.x, mv.movement.y, gameObject.transform.position.z);
        gameObject.transform.position += trans * (speed / 50);
    }
}
