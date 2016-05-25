using UnityEngine;
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
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.Texture2D] = this.OnTexture2DReceived;
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.DrawSprite] = this.OnDrawSprite;
        Messages.Instance.MessageHandlers[Messages.HoloPaintMessageID.ClearPaint] = this.OnClearPaint;
    }

    void OnTexture2DReceived(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        int w = msg.ReadInt32();
        int h = msg.ReadInt32();

        uint len = (uint)msg.ReadInt32();
        byte[] data = new byte[len];

        msg.ReadArray(data, len);

        Texture2D tex = new Texture2D(w, h);

        tex.LoadImage(data);

        //SpriteLayer.mainTexture = tex;
    }

    void OnDrawSprite(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        Vector3 uvWorldPosition = Messages.Instance.ReadVector3(msg);

        float r = msg.ReadFloat();
        float g = msg.ReadFloat();
        float b = msg.ReadFloat();
        float a = msg.ReadFloat();
        float size = msg.ReadFloat();
        int pNum = msg.ReadInt32();

        Vector3 uvEndPosition = Messages.Instance.ReadVector3(msg);
        //DoAction(uvWorldPosition, new Color(r, g, b, a), size, pNum, uvEndPosition);
    }

    void OnClearPaint(NetworkInMessage msg)
    {
        //ClearTexture();
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
            PaintWorldCoordinates(startPos, endPos, BrushManager.Instance.Brush);
        }
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void PaintWorldCoordinates(Vector3 startPos, Vector3 endPos, P3D_Brush brush)
    {
        // Get painter for this paintable
        var painter = GetComponent<P3D_Paintable>().GetPainter();

        // Change painter's current brush
        painter.SetBrush(brush);

        // Paint at the hit coordinate
        // TODO PaintBetweenAll not working, will only draw 2 points
        painter.PaintNearest(endPos, 1.0f);
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
