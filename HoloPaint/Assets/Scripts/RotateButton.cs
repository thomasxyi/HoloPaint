using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public void OnSelect()
    {
        // TODO insert sound effect
        // change app state
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Rotation;
    }
}
