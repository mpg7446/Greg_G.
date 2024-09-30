using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Passable : MonoBehaviour
{
    private Collider2D collider;
    private PlayerManager player;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        player = PlayerManager.Instance;
    }

    private void FixedUpdate()
    {
        if (player.transform.position.y - (player.boxCollider.bounds.size.y / 1.2) > collider.transform.position.y)
            collider.enabled = true;
        else
            collider.enabled = false;
    }
}
