using UnityEngine;
using HoloToolkit.Unity;

public class HologramRotation : Singleton<HologramRotation>
{
    /// <summary>
    /// Tracks if we have been sent a tranform for the anchor model.
    /// The anchor model is rendererd relative to the actual anchor.
    /// </summary>
    public bool Placed { get; private set; }

    Vector3 rotation;
    Vector3 lastRotate;

    void Start()
    {
        Placed = true;
        rotation = Vector3.zero;
    }

    void Update()
    {
        if ((AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Rotation) && (Placed == false))
        {
            if (GestureManager.Instance.IsManipulating) {
                Vector3 angles = transform.rotation.eulerAngles;
                float x = System.Math.Abs(rotation.x);
                float y = System.Math.Abs(rotation.y);
                if (x > y)
                {
                    if (rotation.x > 0)
                    {
                        angles.y += 5;
                    }
                    else if (rotation.x < 0)
                    {
                        angles.y -= 5;
                    }
                }
                else if (y > x)
                {
                    //TODO need to fix model not being able to go 180 degrees when rotating around x axis
                    if (rotation.y > 0)
                    {
                        angles.x += 5;
                    }
                    else if (rotation.y < 0)
                    {
                        angles.x -= 5;
                    }
                }
                transform.eulerAngles = angles;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(angles), Time.deltaTime);
            }
        }
    }

    void OnSelect()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Rotation)
        {
            // if we have placed it, focus the game object for rotation manipulation
            // if not placed already, place the game object
            GestureManager.Instance.OverrideFocusedObject = (Placed ? this.gameObject : null);
            rotation = Vector3.zero;
            Placed = !Placed;
        }
    }

    void OnManipulation()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Rotation)
        {
            rotation = GestureManager.Instance.ManipulationPosition;
        }
    }

    void OnManipulationEnd()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Rotation)
        {
            rotation = Vector3.zero;
        }
    }
}