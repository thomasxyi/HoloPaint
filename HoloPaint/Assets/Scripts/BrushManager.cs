using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;

public class BrushManager : Singleton<BrushManager>
{

    public Color CurrentBrushColor { get; set; }

    void Start()
    {
        CurrentBrushColor = Color.red;
    }

}     