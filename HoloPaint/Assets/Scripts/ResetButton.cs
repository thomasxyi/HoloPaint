using UnityEngine;
using System.Collections;

public class ResetButton : MonoBehaviour
{
    public void OnSelect()
    {
        ModelsManager.Instance.ResetDefaultModelsTransform();
    }
}
