using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private PhotonView photonView;
    // collider maybe??

    public float movementSpeed;
    public float detectionRadius;

    public GameObject holding = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        photonView = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(movementSpeed * Time.fixedDeltaTime, 0, 0), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject;
        if (player.CompareTag("Player") && player.GetComponent<PlayerMovement>().photonView.IsMine)
        {
            Debug.Log("Mambo no. 5 has walked into a player");
            
            if (holding == null)
            {
                GrabPlayer(player);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }

    private void GrabPlayer(GameObject player)
    {
        Debug.Log($"{player.GetComponent<PlayerID>().GetID()} has entered enemies grasp");
        holding = player;
    }
}
