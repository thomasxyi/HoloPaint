using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public void OnSelect()
    {
        // change app state
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Placement;
    }
}
