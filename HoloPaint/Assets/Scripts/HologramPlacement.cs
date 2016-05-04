using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class HologramPlacement : Singleton<HologramPlacement>
{
    /// <summary>
    /// Tracks if we have been sent a tranform for the anchor model.
    /// The anchor model is rendererd relative to the actual anchor.
    /// </summary>
    public bool Placed { get; private set; }

    void Start()
    {
        Placed = true;

        // We care about getting updates for the model transform.
        CustomMessages.Instance.MessageHandlers[CustomMessages.HoloPaintMessageID.BoardTransform] = this.OnStageTransfrom;

        // And when a new user join we will send the model transform we have.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
    }

    void Update()
    {
        if ((AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement) && (Placed == false))
        {
            Vector3 pos;
            RaycastHit hitInfo;
            Quaternion toQuat;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 3, SpatialMappingManager.Instance.LayerMask))
            {
                // have ray that intersects real world
                pos = hitInfo.point;
                // don't make object intersect with detection irregularities
                pos.z += 0.2F;
            }
            else
            {
                // don't have a ray that intersects the real world, just put the model 2m in
                // front of the user.
                pos = Camera.main.transform.position + Camera.main.transform.forward * 2;
            }

            // Reposition the object
            transform.position = Vector3.Lerp(transform.position, pos, 0.2f);
            // Rotate the object
            toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            transform.rotation = toQuat;
        }
    }

    /// <summary>
    /// When a new user joins we want to send them the relative transform for the model if we have it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        if (Placed)
        {
            CustomMessages.Instance.SendBoardTransform(transform.localPosition, transform.localRotation);
        }
    }

    /// <summary>
    /// When a remote system has a transform for us, we'll get it here.
    /// </summary>
    /// <param name="msg"></param>
    void OnStageTransfrom(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        transform.localPosition = CustomMessages.Instance.ReadVector3(msg);
        transform.localRotation = CustomMessages.Instance.ReadQuaternion(msg);

        Placed = true;
    }

    public void OnSelect()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement)
        {
            // if we have placed it, focus the game object for position manipulation
            // if not placed already, place the game object
            GestureManager.Instance.OverrideFocusedObject = (Placed ? this.gameObject : null);
            Placed = !Placed;
            if (Placed)
            {
                CustomMessages.Instance.SendBoardTransform(transform.localPosition, transform.localRotation);
            }
        }
    }
}