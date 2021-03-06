﻿using UnityEngine;
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
    bool changed = false;
    Color defColor = Color.white;

    public void Start() {
        if (GetComponent<Image>() != null && GetComponent<ColorButton>() == null)
        {
            defColor = GetComponent<Image>().color;
        }
    }

    public void OnSelect()
    {
        gameObject.GetComponent<AudioSource>().Play();
        if (level >= 0)
        {
            PaintSelectionButton[] children = this.gameObject.GetComponentsInChildren<PaintSelectionButton>();
            foreach (var child in children)
            {
                if ((child.level == this.level + 1) || (isSet && child.level != this.level))
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

    public void highlight()
    {
        if (defColor != Color.white) {
            GetComponent<Image>().color = new Color(255, 50, 0);
            changed = true;
        }
    }

    public void Update()
    {
        if (!CursorManager.Instance.onMenu && changed && defColor != Color.white) {
            GetComponent<Image>().color = defColor;
        }
    }
}