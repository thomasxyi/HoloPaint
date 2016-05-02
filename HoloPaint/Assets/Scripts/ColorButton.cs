﻿using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public void OnSelect()
    {
        BrushManager.Instance.CurrentBrushColor = this.gameObject.GetComponent<Image>().color;
    }
}

