using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class MenuColliderScript : MonoBehaviour
{
    public void OnOpenCommand()
    {
        this.gameObject.transform.position = GazeManager.Instance.Position + GazeManager.Instance.Normal * CursorManager.Instance.DistanceFromCollision;
    }

    void Update()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement)
        {
            this.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
