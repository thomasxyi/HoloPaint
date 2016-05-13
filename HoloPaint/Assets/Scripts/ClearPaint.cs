using UnityEngine;

public class ClearPaint : MonoBehaviour
{
    public void OnSelect()
    {
        TexturePainter.Instance.ClearTexture();
        Messages.Instance.SendClearPaint();
    }
}
