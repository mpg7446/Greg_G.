using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClientManager;

public class PlayerModelManager : MonoBehaviour
{
    public PlayerVisual playerVisual;
    [SerializeField] private int playerVariation = -1;
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

        SetPlayerVisual();
    }

    public void LoadModel()
    {
        PlayerModel match = null;
        List<PlayerModel> matches = new List<PlayerModel>();

        // Get all matching PlayerModels
        foreach (PlayerModel model in models)
        {
            if (model.playerVisual == playerVisual)
            {
                matches.Add(model);
            }
        }

        // Pick random PlayerModel from matching PlayerModels
        if (photonView.IsMine && playerVariation < 0)
        {
            playerVariation = UnityEngine.Random.Range(0, matches.Count - 1);
            ClientManager.Instance.playerVariation = playerVariation;
            Debug.Log("PlayerModelManager: No player variable selected, choosing random variation");
        }

        match = matches[playerVariation];
        Debug.Log($"PlayerModelManager: Player variable {playerVariation} selected");

        sprite.sprite = match.sprite;
        sprite.transform.localPosition += match.offset;
        sprite.transform.localScale = new Vector3(match.scale.x * sprite.transform.localScale.x, match.scale.y * sprite.transform.localScale.y, sprite.transform.localScale.z);
    }

    public void SetPlayerVisual()
    {
        if (photonView.IsMine)
        {
            playerVisual = ClientManager.Instance.playerVisual;
            playerVariation = ClientManager.Instance.playerVariation;
            LoadModel();
            photonView.RPC("SetPlayerVisual", RpcTarget.Others, (int)playerVisual, playerVariation);
            Debug.Log("PlayerModelManager: Set Players Visuals and called RPC");
        }
        else
        {
            photonView.RPC("AskForPlayerVisual", RpcTarget.Others);
            Debug.Log("PlayerModelManager: remote user found, requesting player visuals");
        }
    }
    [PunRPC]
    public void SetPlayerVisual(int id, int variation)
    {
        playerVisual = (PlayerVisual)id;
        playerVariation = variation;
        Debug.Log($"Recieved from remote user: PlayerVisual: {(PlayerVisual)id} ({id}) | PlayerVariation: {variation}");
        LoadModel();
        Debug.Log("PlayerModelManaher: set player visuals for remote user");
    }
    [PunRPC]
    public void AskForPlayerVisual()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetPlayerVisual", RpcTarget.Others, (int)playerVisual, playerVariation);
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
