using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class MoveButton : MonoBehaviour
{
    public void OnSelect()
    {
        // TODO insert sound effect
        // change app state
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Placement;
      
        
    }
}
