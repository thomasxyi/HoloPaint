using UnityEngine;
using System.Collections;

public class PaintbucketButton : MonoBehaviour
{
    public void OnSelect()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Paintbucket;
        ModeIndicator.Instance.setText("Current Mode: Paint Bucket\nPinch to paint an object");
        ModeIndicator.Instance.setActive(5.0f, true);
    }
}
