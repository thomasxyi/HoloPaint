using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class PaintButton : MonoBehaviour
{
	public void OnSelect()
	{
        this.gameObject.GetComponent<AudioSource>().Play();
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Drawing;
    }
}
