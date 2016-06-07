using UnityEngine;
using System.Collections;

public class BrushSizeLarger : MonoBehaviour {
    
    public void OnSelect()
    {
        Vector2 currentSize = BrushManager.Instance.GetLocalBrush().Size;
        if (currentSize.x < 50 && currentSize.y < 50)
        {
            currentSize.x += 7.5f;
            currentSize.y += 7.5f;
            Vector3 scale = CursorManager.Instance.BrushCursor.transform.localScale;
            scale.x += 0.1f;
            scale.y += 0.1f;
            CursorManager.Instance.BrushCursor.transform.localScale = scale;
            BrushManager.Instance.SetSize(currentSize);
        }
    }
}
