﻿using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// Keeps track of the current state of the experience.
/// </summary>
public class AppStateManager : Singleton<AppStateManager>
{
    /// <summary>
    /// Enum to track progress through the experience.
    /// </summary>
    public enum AppState
    {
        Starting = 0,
        Placement,
        Drawing,
        AddBoard
    }

    /// <summary>
    /// Tracks the current state in the experience.
    /// </summary>
    public AppState CurrentAppState { get; set; }

    void Start()
    {
        CurrentAppState = AppState.Starting;
    }

    void Update()
    {
        bool meshes = SpatialMappingManager.Instance.DrawVisualMeshes;
        switch (CurrentAppState)
        {
            case AppState.Placement:
                if (!meshes)
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;
                }
                break;
            case AppState.Drawing:
                if (meshes)
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = false;
                }
                break;
            case AppState.AddBoard:

                break;
        }
    }

}