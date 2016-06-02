using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public void OnSelect()
    {
        Color c = this.gameObject.GetComponent<Image>().color;
        BrushManager.Instance.SetColor(c);
    }
}

