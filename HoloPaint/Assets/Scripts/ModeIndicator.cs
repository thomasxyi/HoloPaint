using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class ModeIndicator : Singleton<ModeIndicator> {
    public GameObject help;
    public GameObject temp;

    float helpC = 5.0f;
    float tempC = 0.0f;
    
    public void Start()
    {
        help.GetComponentInChildren<TextMesh>().text = "Current Mode: Painting\nPinch and Drag to start Drawing";
        help.SetActive(false);
        temp.SetActive(false);
    }

    public void Update() {
        if (helpC > 0.0f && !temp.activeInHierarchy)
        {
            helpC -= Time.deltaTime;
            help.SetActive(true);
        }
        else
        {
            help.SetActive(false);
        }

        if (tempC > 0.0f)
        {
            tempC -= Time.deltaTime;
            temp.SetActive(true);
        }
        else
        {
            temp.SetActive(false);
            // no flickering when switching
            if (helpC > 0.0f)
            {
                helpC -= Time.deltaTime;
                help.SetActive(true);
            }
        }
    }

    public void setText(string text)
    {
        help.GetComponentInChildren<TextMesh>().text = text;
        temp.GetComponentInChildren<TextMesh>().text = text;
    }

    public void setText(string text, bool val) {
        if (val)
        {
            temp.GetComponentInChildren<TextMesh>().text = text;
        }
        else
        {
            help.GetComponentInChildren<TextMesh>().text = text;
        }
    }

    public void setActive(float time, bool val)
    {
        if (val)
        {
            tempC = time;
            helpC = 0.0f;
        }
        else
        {
            helpC = time;
            tempC = 0.0f;
        }
    }
}
