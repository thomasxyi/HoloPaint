using UnityEngine;
using System.Collections;

public class BrushSizeSmaller : MonoBehaviour {

    public void OnSelect()
    {
        Vector2 currentSize = BrushManager.Instance.GetLocalBrush().Size;
        Vector3 scale = CursorManager.Instance.BrushCursor.transform.localScale;
        if (currentSize.x > BrushManager.Instance.minBrushSize && currentSize.y > BrushManager.Instance.minBrushSize
            && scale.x > BrushManager.Instance.minCursorScale && scale.y > BrushManager.Instance.minCursorScale)
        {
            currentSize.x -= 7.5f;
            currentSize.y -= 7.5f;
            scale.x -= 0.1f;
            scale.y -= 0.1f;
            CursorManager.Instance.BrushCursor.transform.localScale = scale;
            BrushManager.Instance.SetSize(currentSize);

            ModeIndicator.Instance.setText("Brush Size--!", true);
            ModeIndicator.Instance.setActive(5.0f, true);
        }
        else
        {
            ModeIndicator.Instance.setText("Brush Size already at minimum", true);
            ModeIndicator.Instance.setActive(5.0f, true);
        }
    }
}
