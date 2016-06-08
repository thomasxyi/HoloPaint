// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;

/// <summary>
/// CursorManager class takes Cursor GameObjects.
/// One that is on Holograms and another off Holograms.
/// 1. Shows the appropriate Cursor when a Hologram is hit.
/// 2. Places the appropriate Cursor at the hit position.
/// 3. Matches the Cursor normal to the hit surface.
/// </summary>
public class CursorManager : Singleton<CursorManager>
{

    Vector3 navigStart;
    bool isNavigating;

    [Tooltip("Drag the Cursor object to show when it paints on a hologram.")]
    public GameObject BrushCursor;

    [Tooltip("Drag the Cursor object to show when it hits a hologram.")]
    public GameObject CursorOnHolograms;

    [Tooltip("Drag the Cursor object to show when it does not hit a hologram.")]
    public GameObject CursorOffHolograms;

    [Tooltip("Distance, in meters, to offset the cursor from the collision point.")]
    public float DistanceFromCollision = 0.01f;

    public Vector3 brushLocation;
    public Vector3 brushDirection;
    public bool onModel;

    void Awake()
    {
        if (CursorOnHolograms == null || CursorOffHolograms == null || BrushCursor == null)
        {
            return;
        }

        brushLocation = Vector3.zero;
        brushLocation = Vector3.zero;

        // Hide the Cursors to begin with.
        CursorOnHolograms.SetActive(false);
        CursorOffHolograms.SetActive(false);
        BrushCursor.SetActive(false);
        onModel = false;
    }

    void LateUpdate()
    {
        if (GazeManager.Instance == null || CursorOnHolograms == null || CursorOffHolograms == null || BrushCursor == null)
        {
            return;
        }

        // Decide which cursor to show up
        if (((GazeManager.Instance.Hit && GazeManager.Instance.HitInfo.collider.gameObject != null &&
            GazeManager.Instance.HitInfo.collider.gameObject.GetComponent<P3D_Paintable>() != null) ||
            (GestureManager.Instance.getFocusedObject() != null && GestureManager.Instance.IsManipulating) ||
            (onModel && GestureManager.Instance.IsManipulating)) &&
            AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Drawing)
        {
            BrushCursor.SetActive(true);
            CursorOffHolograms.SetActive(false);
            CursorOnHolograms.SetActive(false);
        }
        else
        {
            if (GazeManager.Instance.Hit)
            {
                CursorOnHolograms.SetActive(true);
                CursorOffHolograms.SetActive(false);
                BrushCursor.SetActive(false);
            }
            else
            {
                CursorOffHolograms.SetActive(true);
                CursorOnHolograms.SetActive(false);
                BrushCursor.SetActive(false);
            }
        }

        // Place the cursor at the calculated position.
        if (!GestureManager.Instance.IsManipulating)
        {
            //navigStart = GazeManager.Instance.HitInfo.point;
            //isNavigating = false;

            if (GazeManager.Instance.HitInfo.collider.gameObject != null && GazeManager.Instance.HitInfo.collider.gameObject.GetComponent<P3D_Paintable>() == null)
            {
                this.gameObject.transform.position = GazeManager.Instance.Position + GazeManager.Instance.Normal * 0.1f;
            }
            else
            {
                this.gameObject.transform.position = GazeManager.Instance.Position + GazeManager.Instance.Normal * DistanceFromCollision;
            }
            // Orient the cursor to match the surface being gazed at.
            this.gameObject.transform.up = GazeManager.Instance.Normal;
        }
        else if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Drawing && onModel)
        {
            this.gameObject.transform.position = this.brushLocation;
            this.gameObject.transform.up = this.brushDirection;
        }
        
    }
}