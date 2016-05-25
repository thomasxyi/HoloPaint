using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class PaintSelectionButton : MonoBehaviour
{
    bool isSet = false;
    public void OnSelect()
    {
        Image[] images  = this.gameObject.GetComponentsInChildren<Image>();
        for(int i = 1; i < images.Length; i++)
        {
          
            images[i].enabled = !isSet;
            this.gameObject.GetComponentInChildren<Animation>().Play();
        }
        isSet = !isSet;
        

    }
}