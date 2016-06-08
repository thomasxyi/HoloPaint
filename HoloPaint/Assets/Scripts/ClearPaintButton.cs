using UnityEngine;

public class ClearPaintButton : MonoBehaviour
{
    public void OnSelect()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.ClearPaint;
        ModeIndicator.Instance.setText("Current Mode: Clear Paint\nPinch to clear a model");
        ModeIndicator.Instance.setActive(5.0f, true);
    }
}
