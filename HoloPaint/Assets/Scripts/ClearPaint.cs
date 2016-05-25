using UnityEngine;

public class ClearPaint : MonoBehaviour
{
    public void OnSelect()
    {
        ModelsManager.Instance.ClearAllPaint();
    }
}
