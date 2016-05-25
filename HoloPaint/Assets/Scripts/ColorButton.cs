using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public void OnSelect()
    {
        Color c = Color.cyan; //this.gameObject.GetComponent<Image>().color;
        BrushManager.Instance.SetColor(c);
        BrushColorManager.Instance.ChangeColor(c);
    }
}

