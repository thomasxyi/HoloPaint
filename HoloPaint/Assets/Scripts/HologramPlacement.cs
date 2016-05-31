using UnityEngine;
using HoloToolkit.Unity;

public class HologramPlacement : Singleton<HologramPlacement>
{
    /// <summary>
    /// Tracks if we have been sent a tranform for the anchor model.
    /// The anchor model is rendererd relative to the actual anchor.
    /// </summary>
    public bool Placed { get; private set; }
    public bool Reset { get; private set; }

    Vector3 Manip;

    void Start()
    {
        Placed = true;
        Reset = true;
        Manip = Vector3.zero;
    }

    void Update()
    {
        // make all non paintable objects face the camera
        if (this.gameObject.GetComponent<P3D_Paintable>() == null)
        {
            transform.LookAt(Camera.main.transform);
        }

        if ((AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement) && (Placed == false))
        {
            // if not paintable make it face the user

            if (Reset) {
                transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + Camera.main.transform.forward * 2, 0.2f);
            }
            else if (GestureManager.Instance.IsManipulating)
            {
                Vector3 pos = transform.position;
                // dont make object go behind user or go too far
                Manip.x = System.Math.Min(5, System.Math.Max(-1, Manip.x));
                Manip.y = System.Math.Min(5, System.Math.Max(-1, Manip.y));
                Manip.z = System.Math.Min(5, System.Math.Max(-1, Manip.z));

                pos += Manip;
                // Reposition the object
                transform.position = Vector3.Lerp(transform.position, pos, 0.2f);
            }
        }
    }

    void OnSelect()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement)
        {
            // if we have placed it, focus the game object for position manipulation
            // if not placed already, place the game object
            GestureManager.Instance.OverrideFocusedObject = (Placed ? this.gameObject : null);
            Manip = Vector3.zero;
            Reset = Placed;
            Placed = !Placed;
        }
    }

    void OnManipulation() {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement)
        {
            Reset = false;
            Manip = GestureManager.Instance.ManipulationPosition * 1.5f;
        }
    }

    void OnManipulationEnd()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Rotation)
        {
            Manip = Vector3.zero;
        }
    }
}