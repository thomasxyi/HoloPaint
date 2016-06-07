using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public void OnSelect()
    {
        // change app state

        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Rotation;
    }
}
