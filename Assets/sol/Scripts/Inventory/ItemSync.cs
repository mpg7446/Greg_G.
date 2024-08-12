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
    public class ItemSync : Photon.Pun.MonoBehaviourPun, IPunObservable
    {
        private Item item;
        public float SmoothingDelay = 5;
        public void Awake()
        {
            bool observed = false;
            foreach (Component observedComponent in this.photonView.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    observed = true;
                    item = gameObject.GetComponent<Item>();

                    if (item == null)
                    {
                        Debug.LogError("ItemSync failed to access Item script");
                    }

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
                stream.SendNext(item.countdown);
            }
            else
            {
                //Network player, receive data
                correctPlayerPos = (Vector3)stream.ReceiveNext();
                sliderValue = (float)stream.ReceiveNext();
            }
        }

        // Information for updating remote player
        private Vector3 correctPlayerPos = Vector3.zero;
        private float sliderValue = 0f;

        public void Update()
        {
            if (!photonView.IsMine) // Update remote player (smooth this, this looks good, at the cost of some accuracy)
            {
                transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
                item.countdown = Mathf.Lerp(item.countdown, sliderValue, Time.deltaTime * this.SmoothingDelay);
            }
        }

    }
}