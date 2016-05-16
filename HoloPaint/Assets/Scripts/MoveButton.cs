using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public void OnSelect()
    {
        // TODO insert sound effect
        // change app state
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Placement;
    }
}
