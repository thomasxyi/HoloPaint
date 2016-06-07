using UnityEngine;
using System.Collections;

public class BrushSizeSmaller : MonoBehaviour {
    
    public void OnSelect()
    {
        Vector2 currentSize = BrushManager.Instance.GetLocalBrush().Size;
        Vector3 scale = CursorManager.Instance.BrushCursor.transform.localScale;
        if (currentSize.x > 7.5f && currentSize.y > 7.5f && scale.x > 0.1f && scale.y > 0.1f)
        {
            currentSize.x -= 7.5f;
            currentSize.y -= 7.5f;
            scale.x -= 0.1f;
            scale.y -= 0.1f;
            CursorManager.Instance.BrushCursor.transform.localScale = scale;
            BrushManager.Instance.SetSize(currentSize);
        }
    }
}
