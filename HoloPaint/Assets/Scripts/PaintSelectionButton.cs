using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class PaintSelectionButton : MonoBehaviour
{
    public bool isSet;
    public int level;

    public void OnSelect()
    {

        PaintSelectionButton[] children  = this.gameObject.GetComponentsInChildren<PaintSelectionButton>();
        foreach (var child in children) 
        {
           if(child.level == this.level + 1)
            {
                child.gameObject.GetComponent<Image>().enabled = !isSet;
                Text t = child.gameObject.GetComponentInChildren<Text>();
                if (t != null)
                {
                    t.enabled = !isSet;
                }              
                child.gameObject.GetComponent<BoxCollider>().enabled = !isSet;
            }
                                   
        }
        isSet = !isSet;
        

    }
}