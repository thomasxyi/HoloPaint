/// <summary>
/// CodeArtist.mx 2015
/// This is the main class of the project, its in charge of raycasting to a model and place brush prefabs infront of the canvas camera.
/// If you are interested in saving the painted texture you can use the method at the end and should save it to a file.
/// </summary>


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.VR.WSA.Input;
using HoloToolkit.Sharing;

public enum Painter_BrushMode { PAINT, DECAL };
public class TexturePainter : Singleton<TexturePainter>
{
    Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)

    bool drawing = false;
    float gazeX; //Record the gaze position and don't change while navigating
    float gazeY;
    Vector3 startGazeHit; //Record the original gaze hit
    Vector3 prevPos;

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

    void OnSelect()
    {
        if (AppStateManager.Instance.CurrentAppState != AppStateManager.AppState.Drawing)
            return;
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;
        GameObject hologram = null;
        if (HitTestPosition(ref startPos, ref endPos, ref hologram))
        {
            DoAction(startPos, endPos, BrushManager.Instance.Brush, hologram.GetComponent<P3D_Paintable>());
        }
    }

    void Update()
    {
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void DoAction(Vector3 startPos, Vector3 endPos, P3D_Brush brush, P3D_Paintable paintable)
    {
        // Get painter for this paintable
        var painter = paintable.GetPainter();

        // Change painter's current brush
        painter.SetBrush(brush);

        // Paint at the hit coordinate
        painter.PaintNearest(endPos, 1f);

        // Find the ray for this screen position
        //var stepCount = Vector2.Distance(startPos, endPos) / StepSize + 1;

        //for (var i = 0; i < stepCount; i++)
        //{
        //    var subPos = Vector3.Lerp(startPos, endPos, i / stepCount);
        //}
    }

    //Returns the position on the texuremap according to a hit in the mesh collider
    bool HitTestPosition(ref Vector3 startPos, ref Vector3 endPos, ref GameObject hologram)
    {
        RaycastHit hit = GazeManager.Instance.HitInfo;

        if (!GestureManager.Instance.IsNavigating)
        {
            // user released navigation gesture
            // reset drawing
            drawing = false;
            GestureManager.Instance.OverrideFocusedObject = null;
        }

        if (drawing)
        {
            // user is drawing currently
            // draw based on saved gaze position
            startPos = prevPos;
            endPos = startGazeHit + GestureManager.Instance.ManipulationPosition;
            prevPos = endPos;
            hologram = hit.collider.gameObject;
        }
        else if (GazeManager.Instance.Hit)
        {
            if (GestureManager.Instance.IsNavigating)
            {
                // first drawing stroke by user
                // save current gaze focus
                startGazeHit = hit.point;
                prevPos = hit.point;
                drawing = true;
                GestureManager.Instance.OverrideFocusedObject = this.gameObject;
            }
            startPos = hit.point;
            endPos = hit.point;
        }
        else
        {
            // not a valid action, dont do anything
            return false;
        }

        return true;
    }

    //Returns the position on the texuremap according to a hit in the mesh collider
    bool HitTestUVPosition(ref Vector3 pixelUV, ref int paintableNum, ref Vector3 uvEndPosition)
    {
        RaycastHit hit = GazeManager.Instance.HitInfo;

        if (!GestureManager.Instance.IsNavigating)
        {
            // user released navigation gesture
            // reset drawing
            drawing = false;
            GestureManager.Instance.OverrideFocusedObject = null;
        }

        if (drawing)
        {
            // user is drawing currently
            // draw based on saved gaze position
            pixelUV = new Vector2(gazeX, gazeY);

            uvEndPosition = new Vector2(
                GestureManager.Instance.ManipulationPosition.x + gazeX,
                GestureManager.Instance.ManipulationPosition.y + gazeY);
        }
        else if (GazeManager.Instance.Hit)
        {
            if (GestureManager.Instance.IsNavigating)
            {
                // first drawing stroke by user
                // save current gaze focus
                gazeX = hit.textureCoord.x;
                gazeY = hit.textureCoord.y;
                drawing = true;
                GestureManager.Instance.OverrideFocusedObject = this.gameObject;
            }
            pixelUV = new Vector2(
                hit.textureCoord.x,
                hit.textureCoord.y);
        }
        else
        {
            return false;
        }

        return true;
    }

    ////////////////// PUBLIC METHODS //////////////////

    public void SetBrushMode(Painter_BrushMode brushMode)
    { //Sets if we are painting or placing decals
       // mode = brushMode;
        //brushCursor.GetComponent<SpriteRenderer>().sprite = brushMode == Painter_BrushMode.PAINT ? cursorPaint : cursorDecal;
    }

    public void ClearTexture()
    {
       // saving = true;
        //SaveTexture(true);
    }

}
