﻿using UnityEngine;

public class PaintButton : MonoBehaviour
{
	public void OnSelect()
	{
        AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Drawing;
        ModeIndicator.Instance.setText("Current Mode: Painting\nPinch and Drag to start Drawing");
        ModeIndicator.Instance.setActive(5.0f, true);
    }
}
