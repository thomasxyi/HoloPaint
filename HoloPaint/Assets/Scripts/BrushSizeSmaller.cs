using UnityEngine;
using System.Collections;

public class BrushSizeSmaller : MonoBehaviour {
    
    public void OnSelect()
    {
        Vector2 currentSize = BrushManager.Instance.GetLocalBrush().Size;
        if (currentSize.x > 0.1f && currentSize.y > 0.1f)
        {
            currentSize.x -= 0.1f;
            currentSize.y -= 0.1f;
            BrushManager.Instance.SetSize(currentSize);
        }
    }
}
