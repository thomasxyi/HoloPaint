using UnityEngine;
using System.Collections;

public class InstantiateModelButton : MonoBehaviour
{
    public void OnSelect()
    {
        ModelsManager.Instance.InstantiateHologram("Whale");
        ModeIndicator.Instance.setText("A model has been placed", true);
        ModeIndicator.Instance.setActive(5.0f, true);
    }
}
