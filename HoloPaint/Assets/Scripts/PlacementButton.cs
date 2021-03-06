﻿using UnityEngine;

public class PlacementButton : MonoBehaviour
{
    public void OnSelect()
    {
        switch (AppStateManager.Instance.CurrentAppState)
        {
            // TODO insert sound effect
            // change app state
            case AppStateManager.AppState.Starting:
                AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Placement;
                break;
            case AppStateManager.AppState.Placement:
                AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Drawing;
                break;
            case AppStateManager.AppState.Drawing:
                AppStateManager.Instance.CurrentAppState = AppStateManager.AppState.Placement;
                break;
        }
    }
}