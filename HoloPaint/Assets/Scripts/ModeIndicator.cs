using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class ModeIndicator : Singleton<ModeIndicator> {

    public string text;
    bool active = true;

    public void setActive() {
        active = !active;
        this.gameObject.SetActive(active);
    }
}
