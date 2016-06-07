using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class MenuColliderScript : MonoBehaviour {

    void Update() {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Placement)
        {
            this.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
