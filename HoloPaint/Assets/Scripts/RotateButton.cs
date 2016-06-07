using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public void OnSelect()
    {
        // change app state
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Rotation;
        ModeIndicator.Instance.setText("Current Mode: Rotation\nPinch to Select / Unselect\nScroll to change orientation");
    }
}
