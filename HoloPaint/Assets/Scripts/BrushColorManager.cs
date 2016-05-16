using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public class BrushColorManager : Singleton<BrushColorManager>
{
    public void ChangeColor(Color  c)
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = c;
    }
}
