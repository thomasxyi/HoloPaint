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

    public void OnSelect()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement)
        {
            // if we have placed it, focus the game object for position manipulation
            // if not placed already, place the game object
            GestureManager.Instance.OverrideFocusedObject = (Placed ? this.gameObject : null);
            Placed = !Placed;
        }
    }
}