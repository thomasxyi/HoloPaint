﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;
using HoloToolkit.Sharing;
using System;

public class TexturePainter : Singleton<TexturePainter>
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

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void PaintWorldCoordinates(Vector3 startPos, Vector3 endPos, P3D_Brush brush)
    {
        // Get painter for this paintable
        var painter = GetComponent<P3D_Paintable>().GetPainter();

        // Change painter's current brush
        painter.SetBrush(brush);

        painter.ModelGUID = this.uid;

        // Paint at the hit coordinate
        // TODO PaintBetweenAll not working, will only draw 2 points
        painter.PaintNearest(endPos, 1.0f);

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
        if (!GestureManager.Instance.IsManipulating)
        {
            navigStart = GazeManager.Instance.HitInfo.point;
            navigating = false;
        }
    }

    //Returns the position on the texuremap according to a hit in the mesh collider
    public bool HitTestPosition(ref Vector3 startPos, ref Vector3 endPos)
    {
        if (!GestureManager.Instance.IsManipulating)
        {
            // user released navigation gesture
            // reset drawing
            navigating = false;
            GestureManager.Instance.OverrideFocusedObject = null;
        }

        if (navigating)
        {
            // user is drawing currently
            // draw based on saved gaze position
            //startPos = lastDrawn;
            endPos = navigStart + GestureManager.Instance.ManipulationPosition * 2.0f;

            //lastDrawn = endPos;
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
            //startPos = hit.point;
            endPos = hit.point;
            //lastDrawn = hit.point;
        }
        else
        {
            // not a valid action, dont do anything
            return false;
        }

        return true;
    }
}
