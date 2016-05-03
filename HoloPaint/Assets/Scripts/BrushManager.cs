using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;

public class BrushManager : Singleton<BrushManager>
{
    private Color color;

    public Color CurrentBrushColor { get { return this.color; }
                                     set { this.color = value;
            ((GameObject)Instantiate(Resources.Load("TexturePainter-Instances/SolidBrushEntity"))).GetComponent<Material>().color = value;
        } }

    void Start()
    {
        CurrentBrushColor = Color.red;
    }

    

}     