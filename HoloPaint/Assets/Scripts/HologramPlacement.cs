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
    public bool GotTransform { get; private set; }

    void Start()
    {
        // Start by making the model as the cursor.
        // So the user can put the hologram where they want.
        GestureManager.Instance.OverrideFocusedObject = this.gameObject;
    }

    void Update()
    {
        if (GotTransform == false)
        {
            Vector3 pos;
            RaycastHit hitInfo;
            Quaternion toQuat;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 3, SpatialMappingManager.Instance.LayerMask))
            {
                // have ray that intersects real world
                pos = hitInfo.point;
                // TODO: rotate object to align to real world mesh
                toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
            }
            else
            {
                // don't have a ray that intersects the real world, just put the model 2m in
                // front of the user.
                pos = Camera.main.transform.position + Camera.main.transform.forward * 2;
                // make object face the user
                toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
            }

            // Reposition the object
            transform.position = Vector3.Lerp(transform.position, pos, 0.2f);
            // Rotate the object
            transform.rotation = toQuat;
        }
    }

    public void OnSelect()
    {
        // Note that we have a tranform.
        GotTransform = true;

        // The user has now placed the hologram.
        // Route input to gazed at holograms.
        GestureManager.Instance.OverrideFocusedObject = null;
    }

    public void ResetStage()
    {
        // We'll use this later.
    }
}