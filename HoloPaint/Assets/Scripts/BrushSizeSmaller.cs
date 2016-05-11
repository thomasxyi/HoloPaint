using UnityEngine;
using System.Collections;

public class BrushSizeSmaller : MonoBehaviour {
    
    public void OnSelect()
    {
        if (BrushManager.Instance.CurrentBrushSize > 0.1f)
        {
            BrushManager.Instance.CurrentBrushSize -= 0.1f;
        }
    }
}
