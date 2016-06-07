using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class ModeIndicator : Singleton<ModeIndicator> {
    float time = 10.0f;
    
    public void Start()
    {
        this.gameObject.GetComponentInChildren<TextMesh>().text = "Current Mode: Painting\nPinch and Drag to start Drawing";
    }

    public void setActive() {
        time = 10.0f;
    }

    public void Update() {
        float val = Time.deltaTime;
        if (time >= 0.0f)
        {
            time -= val;
            this.gameObject.SetActive(true);
        }
        else
        {
            time = 0.0f;
            this.gameObject.SetActive(false);
        }
    }

    public void setText(string text) {
        this.gameObject.GetComponentInChildren<TextMesh>().text = text;
    }
}
