using UnityEngine;
using System.Collections;

public class BrushSizeLarger : MonoBehaviour {
    
    public void OnSelect()
    {
        Vector2 currentSize = BrushManager.Instance.GetLocalBrush().Size;
        if (currentSize.x < BrushManager.Instance.maxBrushSize && currentSize.y < BrushManager.Instance.maxBrushSize)
        {
            currentSize.x += 7.5f;
            currentSize.y += 7.5f;
            Vector3 scale = CursorManager.Instance.BrushCursor.transform.localScale;
            scale.x += 0.1f;
            scale.y += 0.1f;
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
