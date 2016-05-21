using UnityEngine;

public class ClearPaint : MonoBehaviour
{
    public void OnSelect()
    {
        Messages.Instance.SendClearPaint();
    }
}
