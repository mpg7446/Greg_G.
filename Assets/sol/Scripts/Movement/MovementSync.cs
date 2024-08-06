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
    public class MovementSync : Photon.Pun.MonoBehaviourPun, IPunObservable
    {
        private Rigidbody2D rb;
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
                stream.SendNext(transform.localScale);
                stream.SendNext(rb.velocity);
            }
            else
            {
                //Network player, receive data
                correctPlayerPos = (Vector3)stream.ReceiveNext();
                correctPlayerScale = (Vector3)stream.ReceiveNext();
                correctVelocity = (Vector2)stream.ReceiveNext();
            }
        }

        private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
        private Vector3 correctPlayerScale = Vector3.zero; //We lerp towards this
        private Vector2 correctVelocity;

        public void Update()
        {
            if (!photonView.IsMine)
            {
                //Update remote player (smooth this, this looks good, at the cost of some accuracy)
                transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
                transform.localScale = Vector3.Lerp(transform.localScale, correctPlayerScale, Time.deltaTime * this.SmoothingDelay);
                rb.velocity = Vector2.Lerp(rb.velocity, correctVelocity, Time.deltaTime * this.SmoothingDelay);
            }
        }

    }
}