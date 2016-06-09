using UnityEngine;
using System.Collections;

public class BrushSizeLarger : MonoBehaviour {
    
    public void OnSelect()
    {
        Vector2 currentSize = BrushManager.Instance.GetLocalBrush().Size;
        if (currentSize.x < BrushManager.Instance.maxBrushSize && currentSize.y < BrushManager.Instance.maxBrushSize)
        {
            currentSize.x += BrushManager.Instance.minBrushSize;
            currentSize.y += BrushManager.Instance.minBrushSize;
            Vector3 scale = CursorManager.Instance.BrushCursor.transform.localScale;
            scale.x += BrushManager.Instance.minCursorScale;
            scale.y += BrushManager.Instance.minCursorScale;
            CursorManager.Instance.BrushCursor.transform.localScale = scale;
            BrushManager.Instance.SetSize(currentSize);

            ModeIndicator.Instance.setText("Brush Size++!", true);
            ModeIndicator.Instance.setActive(5.0f, true);
        }
        else
        {
            ModeIndicator.Instance.setText("Brush Size already at max", true);
            ModeIndicator.Instance.setActive(5.0f, true);
        }
    }
}
