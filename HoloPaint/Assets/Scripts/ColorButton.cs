using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class ColorButton : MonoBehaviour
{
    public void OnSelect()
    {
        BrushManager.Instance.CurrentBrushColor = gameObject.GetComponent<Renderer>().material.color;
    }
}

