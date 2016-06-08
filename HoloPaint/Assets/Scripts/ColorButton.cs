using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public string color;

    public void OnSelect()
    {
        Color c = this.gameObject.GetComponent<Image>().color;
        BrushManager.Instance.SetColor(c);
        CursorManager.Instance.BrushCursor.GetComponent<SpriteRenderer>().color = c;
        ModeIndicator.Instance.setText(string.Concat("Color changed to " , color), true);
        ModeIndicator.Instance.setActive(5.0f, true);
    }
}

