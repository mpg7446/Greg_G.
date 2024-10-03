using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Passable : MonoBehaviour
{
    private new Collider2D collider;
    private Collider2D playerCollider;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        playerCollider = PlayerManager.Instance.gameObject.GetComponentInChildren<Collider2D>();
    }

    private void FixedUpdate()
    {
        Vector3 heading = playerCollider.bounds.min + new Vector3(0, playerCollider.bounds.size.y / 2.1f, 0) - collider.bounds.max;
        if (Vector3.Dot(Vector3.up, heading) > 0){
            collider.enabled = true;
        }
        else
            collider.enabled = false;
    }
}
