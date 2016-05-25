using UnityEngine;
using System.Collections;

public class InstantiateModelButton : MonoBehaviour
{
    public void OnSelect()
    {
        ModelsManager.Instance.InstantiateHologram("Whale");
    }
}
