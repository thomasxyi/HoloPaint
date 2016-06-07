using UnityEngine;
using System.Collections;

public class PaintbucketButton : MonoBehaviour
{
    public void OnSelect()
    {
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Paintbucket;
    }
}
