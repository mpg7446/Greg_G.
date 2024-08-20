// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmoothSyncMovement.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Smoothed out movement for network gameobjects
// </summary>                                                                                             
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Smoothed out movement for network gameobjects
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PlayerMovementSync : Photon.Pun.MonoBehaviourPun, IPunObservable
    {
        private Rigidbody2D rb;
        private SpriteRenderer sprite;
        private PlayerModelManager playerModel;
        public float SmoothingDelay = 5;
        public void Awake()
        {
            bool observed = false;
            foreach (Component observedComponent in this.photonView.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    observed = true;
                    rb = GetComponent<Rigidbody2D>();
                    sprite = GetComponentInChildren<SpriteRenderer>();
                    break;
                }
            }
            if (!observed)
            {
                Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //We own this player: send the others our data
                stream.SendNext(transform.position);
                stream.SendNext(sprite.flipX);
                stream.SendNext(rb.velocity);
            }
            else
            {
                //Network player, receive data
                correctPlayerPos = (Vector3)stream.ReceiveNext();
                spriteFlip = (bool)stream.ReceiveNext();
                correctVelocity = (Vector2)stream.ReceiveNext();
            }
        }

        // Information for updating remote player
        private Vector3 correctPlayerPos = Vector3.zero;
        private bool spriteFlip = false;
        private Vector2 correctVelocity;

        public void Update()
        {
            if (!photonView.IsMine) // Update remote player (smooth this, this looks good, at the cost of some accuracy)
            {
                // movement
                transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
                sprite.flipX = spriteFlip;
                rb.velocity = correctVelocity;
            }
        }

    }
}