using UnityEngine;
using System.Collections;

public class ResetButton : MonoBehaviour
{
    public void OnSelect()
    {
        ModelsManager.Instance.ResetDefaultModelsTransform();
        ModeIndicator.Instance.setText("All holograms have been reset", true);
        ModeIndicator.Instance.setActive(5.0f, true);
    }
}
