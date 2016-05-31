﻿using UnityEngine;
using HoloToolkit.Unity;
using System;

public class TexturePainter : MonoBehaviour
{
    public Guid uid { get; set; }
    bool navigating = false;
    Vector3 lastDrawn;
    Vector3 navigStart;

    void Start()
    {
    }

    void OnManipulation()
    {
        OnSelect();
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void PaintWorldCoordinates(Vector3 startPos, Vector3 endPos, P3D_Brush brush)
    {
        // Get painter for this paintable
        var painter = GetComponent<P3D_Paintable>().GetPainter();

        // Change painter's current brush
        painter.SetBrush(brush);

        painter.ModelGUID = this.uid;

        // Paint at the hit coordinate
        if (startPos == endPos)
        {
            var hit = default(RaycastHit);
            Vector3 origin = Camera.main.transform.position;

            // Raycast into the 3D scene
            if (Physics.Raycast(origin, endPos - origin, out hit, 10.0f))
            {
                CursorManager.Instance.brushDirection = hit.normal;
                CursorManager.Instance.onModel = true;
                CursorManager.Instance.brushLocation = hit.point + hit.normal * 0.01f;
                painter.Paint(hit.textureCoord);
            }
            else {
                CursorManager.Instance.onModel = false;
            }
        }
        else
        {
            var startHit = default(RaycastHit);
            var endHit = default(RaycastHit);
            Vector3 origin = Camera.main.transform.position;

            // Raycast into the 3D scene
            if (Physics.Raycast(origin, startPos - origin, out startHit, 10.0f) && Physics.Raycast(origin, endPos - origin, out endHit, 10.0f))
            {
                CursorManager.Instance.brushDirection = endHit.normal;
                CursorManager.Instance.brushLocation = endHit.point + endHit.normal * 0.01f; 
                CursorManager.Instance.onModel = true;
                Vector2 startUV = startHit.textureCoord;
                Vector2 endUV = endHit.textureCoord;
                if (Vector3.Distance(startPos, endPos) > Vector2.Distance(startUV, endUV))
                {
                    var stepCount = Vector2.Distance(startUV, endUV) / BrushManager.Instance.GetStepSize() + 1;
                    
                    for (var i = 0; i < stepCount; i++)
                    {
                        var subUV = Vector2.Lerp(startUV, endUV, i / stepCount);

                        painter.Paint(subUV);
                    }
                }
            }
            else {
                CursorManager.Instance.onModel = false;
            }
        }

        painter.ModelGUID = Guid.Empty; // makes sure there's no more synchronization
    }

    public void PaintUVCoordinates(Vector2 uv, P3D_Brush brush)
    {
        var painter = GetComponent<P3D_Paintable>().GetPainter();

        painter.SetBrush(brush);
        
        painter.ModelGUID = Guid.Empty; // makes sure there's no synchronization

        painter.Paint(uv);
    }

    public void ClearPaint()
    {
        P3D_Paintable paintable = GetComponent<P3D_Paintable>();
        Material material = P3D_Helper.GetMaterial(this.gameObject, paintable.MaterialIndex);

        if (material != null)
        {
            Texture2D texture = material.GetTexture(paintable.TextureName) as Texture2D;

            if (texture != null)
            {
                P3D_Helper.ClearTexture(texture, Color.white, true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Drawing && !GestureManager.Instance.IsManipulating)
        {
            navigStart = GazeManager.Instance.HitInfo.point;
            navigating = false;
            GestureManager.Instance.OverrideFocusedObject = null;
        }
    }

    void OnSelect()
    {
        if (AppStateManager.Instance.CurrentAppState != AppStateManager.AppState.Drawing)
            return;
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;
        if (HitTestPosition(ref startPos, ref endPos))
        {
            PaintWorldCoordinates(startPos, endPos, BrushManager.Instance.GetLocalBrush());
        }
    }

    //Returns the position on the texuremap according to a hit in the mesh collider
    public bool HitTestPosition(ref Vector3 startPos, ref Vector3 endPos)
    {
        if (navigating)
        {
            // user is drawing currently
            // draw based on saved gaze position
            startPos = lastDrawn;
            endPos = navigStart + GestureManager.Instance.ManipulationPosition * 2.0f;
            lastDrawn = endPos;
        }
        else if (GazeManager.Instance.Hit)
        {
            if (GestureManager.Instance.IsManipulating)
            {
                // first drawing stroke by user
                // save current gaze focus
                navigating = true;
                GestureManager.Instance.OverrideFocusedObject = this.gameObject;
            }
            RaycastHit hit = GazeManager.Instance.HitInfo;
            startPos = hit.point;
            endPos = hit.point;
            lastDrawn = hit.point;
        }
        else
        {
            // not a valid action, dont do anything
            return false;
        }

        return true;
    }
}
