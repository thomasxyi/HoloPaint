using UnityEngine;
using System.Collections;

public class BrushSizeLarger : MonoBehaviour {
    
    public void OnSelect()
    {
        Vector2 currentSize = BrushManager.Instance.GetLocalBrush().Size;
        currentSize.x += 0.1f;
        currentSize.y += 0.1f;
        BrushManager.Instance.SetSize(currentSize);
    }
}
