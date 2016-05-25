using UnityEngine;

public class ClearPaint : MonoBehaviour
{
    public void OnSelect()
    {
        //Messages.Instance.SendClearPaint();
        ModelsManager.Instance.InstantiateHologram("Whale");
    }
}
