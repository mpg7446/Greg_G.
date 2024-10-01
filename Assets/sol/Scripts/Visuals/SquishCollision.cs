using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishCollision : MonoBehaviour
{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = PlayerManager.Instance.GetComponent<PhotonView>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (photonView == null)
        {
            //Debug.Log("missing photon view");
            return;
        }

        if (photonView.IsMine && collision.gameObject.CompareTag("Player")) // colliding with player/object
        {
            PlayerManager.Instance.CollisionEnter(collision);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (photonView == null)
        {
            //Debug.Log("missing photon view");
            return;
        }

        if (photonView.IsMine && collision.gameObject.CompareTag("Player")) // check if not colliding with player
        {
            PlayerManager.Instance.CollisionExit(collision);
        } 
        //else if (photonView.IsMine)
        //{
        //    PlayerManager.Instance.PassableExit(collision);
        //}
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    PlayerManager.Instance.TriggerEnter(collision);
    //}
}
