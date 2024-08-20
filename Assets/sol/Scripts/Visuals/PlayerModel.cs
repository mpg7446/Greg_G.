using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientManager;

//[CreateAssetMenu(menuName = "Player Model", fileName = "New Player Model")]
public class PlayerModel : MonoBehaviour
{
    public PlayerVisual playerVisual;
    private PhotonView photonView;

    // Objects
    [SerializeField] private GameObject cardboard;
    [SerializeField] private GameObject stickers;

    public bool Squished {  get; private set; }

    public void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            enabled = false;
        }

        if (stickers == null)
        {
            stickers = GameObject.Find("Stickers");
        }
    }

    [PunRPC]
    public void Squish(bool rpc = true)
    {
        stickers.transform.localScale = new Vector3(1, 0.5f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D>().size.y / 4), 0);
        Squished = true;

        if (rpc)
        {
            photonView.RPC("UnSquish", RpcTarget.Others, false);
        }
    }
    [PunRPC]
    public void UnSquish(bool rpc = true)
    {
        stickers.transform.localScale = Vector3.one;
        Squished = false;

        if (rpc)
        {
            photonView.RPC("UnSquish", RpcTarget.Others, false);
        }
    }
}
