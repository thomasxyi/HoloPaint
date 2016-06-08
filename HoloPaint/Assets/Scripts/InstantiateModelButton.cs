using UnityEngine;
using System.Collections;

public class InstantiateModelButton : MonoBehaviour
{
    public string modelName;
    public void OnSelect()
    {
        ModelsManager.Instance.InstantiateHologram(modelName);
        ModeIndicator.Instance.setText("A model has been placed", true);
        ModeIndicator.Instance.setActive(5.0f, true);
    }
}
