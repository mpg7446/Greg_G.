using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientManager;

public class PlayerModelManager : MonoBehaviour
{
    public PlayerVisual playerVisual;
    public List<PlayerModel> models = new List<PlayerModel>();
    private PhotonView photonView;

    // Objects
    [SerializeField] private GameObject cardboard;
    public GameObject stickers;
    private SpriteRenderer sprite;

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
        sprite = stickers.GetComponentInChildren<SpriteRenderer>();

        SetPlayerVisual(ClientManager.Instance.playerVisual);
    }

    public void LoadModel()
    {
        // Get all matching PlayerModels
        List<PlayerModel> matches = new List<PlayerModel>();
        foreach (PlayerModel model in models)
        {
            if (model.playerVisual == playerVisual)
            {
                matches.Add(model);
            }
        }

        // Pick random PlayerModel from matching PlayerModels - MOVE THIS TO CLIENT MANAGER TO SYNC BETWEEN MATCHES
        PlayerModel match = matches[0];
        if (matches.Count > 1)
        {
            match = matches[Random.Range(0, matches.Count)];
        }

        if (match != null)
        {
            sprite.sprite = match.sprite;
            sprite.transform.localPosition += match.offset;
            sprite.transform.localScale = new Vector3(match.scale.x * sprite.transform.localScale.x, match.scale.y * sprite.transform.localScale.y, sprite.transform.localScale.z);
        }
    }

    public void SetPlayerVisual(PlayerVisual playerVisual)
    {
        if (photonView.IsMine)
        {
            this.playerVisual = playerVisual;
            photonView.RPC("SetPlayerVisual", RpcTarget.Others, (int)playerVisual);
            LoadModel();
            Debug.Log("PlayerModelManager: Set Players Visuals and called RPC");
        }
        else
        {
            photonView.RPC("AskForPlayerVisual", RpcTarget.Others);
            Debug.Log("PlayerModelManager: remote user found, requesting player visuals");
        }
    }
    [PunRPC]
    public void SetPlayerVisual(int id)
    {
        playerVisual = (PlayerVisual)id;
        LoadModel();
        Debug.Log("PlayerModelManaher: set player visuals for remote user");
    }
    [PunRPC]
    public void AskForPlayerVisual()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetPlayerVisual", RpcTarget.Others, (int)playerVisual);
        }
    }

    [PunRPC]
    public void Squish(bool rpc = true)
    {
        stickers.transform.localScale = new Vector3(1, 0.5f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y - (stickers.GetComponent<BoxCollider2D>().size.y / 4), 0);
        Squished = true;

        if (rpc)
        {
            photonView.RPC("Squish", RpcTarget.Others, false);
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
