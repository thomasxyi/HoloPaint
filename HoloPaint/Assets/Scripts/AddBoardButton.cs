using UnityEngine;
using System.Collections;

public class AddBoardButton : MonoBehaviour {

	void OnSelect () {
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.AddBoard;
    }
}
