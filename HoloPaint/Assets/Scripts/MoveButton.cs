using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public void OnSelect()
    {
        // change app state
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Placement;
        ModeIndicator.Instance.setText("Current Mode: Placement\nPinch to Pick up / Place an object\nGaze or Scroll to change position");
        ModeIndicator.Instance.setActive(5.0f, true);
        MenuColliderScript.Instance.opened = false;
    }
}
