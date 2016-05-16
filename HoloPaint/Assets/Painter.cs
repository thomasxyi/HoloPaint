using UnityEngine;
using System.Collections;

public class Painter : MonoBehaviour {
    [Tooltip("The layer mask used when raycasting into the scene")]
    public LayerMask RaycastMask = -1;

    [Tooltip("The settings for the brush we will paint with")]
    public P3D_Brush Brush;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //Returns the position on the texuremap according to a hit in the mesh collider
    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit = GazeManager.Instance.HitInfo;

        Vector2 pixelUV;

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
            pixelUV = new Vector2(
                GestureManager.Instance.NavigationPosition.x + gazeX,
                GestureManager.Instance.NavigationPosition.y + gazeY);
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
            // not a valid action, dont do anything
            return false;
        }

        uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
        uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;//To center the UV on Y
        uvWorldPosition.z = 0.0f;
        return true;
    }
}
