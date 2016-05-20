using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;

public class BrushManager : Singleton<BrushManager>
{
    public Color CurrentBrushColor { get; set; }
    public float CurrentBrushSize { get; set; }
    public P3D_Brush Brush;
    public float StepSize = 1.0f;

    void Start()
    {
        Brush = new P3D_Brush();
        Brush.Color = Color.red;
        CurrentBrushColor = Color.red;
        CurrentBrushSize = 1.0f;
    }
}     