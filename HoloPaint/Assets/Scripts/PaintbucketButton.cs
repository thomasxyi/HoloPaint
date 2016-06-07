using UnityEngine;
using System.Collections;

public class PaintbucketButton : MonoBehaviour
{
    public void OnSelect()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Paintbucket;
    }
}
