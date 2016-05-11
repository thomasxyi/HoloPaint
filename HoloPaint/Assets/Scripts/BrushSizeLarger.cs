using UnityEngine;
using System.Collections;

public class BrushSizeLarger : MonoBehaviour {
    
    public void OnSelect()
    {
        BrushManager.Instance.CurrentBrushSize += 0.1f;
    }
}
