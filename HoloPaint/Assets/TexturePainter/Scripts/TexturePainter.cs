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

public enum Painter_BrushMode{PAINT,DECAL};
public class TexturePainter : Singleton<TexturePainter> {
	public GameObject brushCursor,brushContainer; //The cursor that overlaps the model and our container for the brushes painted
	public Camera sceneCamera,canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
	public Sprite cursorPaint,cursorDecal; // Cursor for the differen functions 
	public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
    public Material SpriteLayer; // The material of our base texture (Were we will save the painted texture)
    public Material backgroundLayer; // The material of immutable background image

    Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)
	float brushSize=1.0f; //The size of our brush
	Color brushColor = Color.red; //The selected color
	int brushCounter=0,MAX_BRUSH_COUNT=100; //To avoid having millions of brushes
	bool saving=false; //Flag to check if we are saving the texture

    bool drawing = false;
    float gazeX; //Record the gaze position and don't change while navigating
    float gazeY;

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

        SpriteLayer.mainTexture = tex;
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

        DoAction(uvWorldPosition, new Color(r, g, b, a), size);
    }

    void OnClearPaint(NetworkInMessage msg)
    {
        ClearTexture();
    }

    void OnSelect()
    {
        if (saving || (AppStateManager.Instance.CurrentAppState != AppStateManager.AppState.Drawing))
            return;
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
        {
            DoAction(uvWorldPosition, brushColor, brushSize);
            Messages.Instance.SendDrawSprite(uvWorldPosition, brushColor, brushSize);
        }
    }

    void Update ()
    {
        //To update at realtime the painting cursor on the mesh
        brushColor = BrushManager.Instance.CurrentBrushColor;
        // local brush size changes updates cursor
        if (brushSize != BrushManager.Instance.CurrentBrushSize) {
            brushSize = BrushManager.Instance.CurrentBrushSize;
            brushCursor.transform.localScale = Vector3.one * brushSize;
        }
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition) && !saving && (AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Drawing))
        {
            brushCursor.SetActive(true);
            brushCursor.transform.position = uvWorldPosition + brushContainer.transform.position;
        }
        else {
            brushCursor.SetActive(false);
        }
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void DoAction(Vector3 uvWorldPosition, Color bColor, float size)
    {
        GameObject brushObj;
        if (mode == Painter_BrushMode.PAINT)
        {
            brushObj = (GameObject)Instantiate(Resources.Load("TexturePainter-Instances/SolidBrushEntity")); //Paint a brush
        }
        else
        {
            brushObj = (GameObject)Instantiate(Resources.Load("TexturePainter-Instances/DecalEntity")); //Paint a decal
        }
        bColor.a = size * 2.0f; // Brushes have alpha to have a merging effect when painted over.
        brushObj.GetComponent<SpriteRenderer>().color = bColor; //Set the brush color
        brushObj.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
        brushObj.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
        brushObj.transform.localScale = Vector3.one * size;//The size of the brush
        brushCounter++; //Add to the max brushes
		if (brushCounter >= MAX_BRUSH_COUNT)
        { //If we reach the max brushes available, flatten the texture and clear the brushes
            saving = true;
            brushCursor.SetActive (false);
			Invoke("SaveTexture",0.1f);
		}
	}

	//Returns the position on the texuremap according to a hit in the mesh collider
	bool HitTestUVPosition(ref Vector3 uvWorldPosition){
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

    void SaveTexture() {
        SaveTexture(false);
    }

	//Sets the base material with a our canvas texture, then removes all our brushes
	void SaveTexture(bool clear){
		brushCounter=0;
		System.DateTime date = System.DateTime.Now;
        Texture2D tex;
        if (clear)
        {
            tex = backgroundLayer.mainTexture as Texture2D;
        }
        else
        {
            RenderTexture.active = canvasTexture;
            tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
        }
        SpriteLayer.mainTexture = tex;	//Put the painted texture as the base
        //Messages.Instance.SendTexture2D(tex);
		foreach (Transform child in brushContainer.transform) {//Clear brushes
			Destroy(child.gameObject);
		}
        //StartCoroutine ("SaveTextureToFile"); //Do you want to save the texture? This is your method!
        ShowCursor();
	}
	//Show again the user cursor (To avoid saving it to the texture)
	void ShowCursor(){	
		saving = false;
	}

	////////////////// PUBLIC METHODS //////////////////

	public void SetBrushMode(Painter_BrushMode brushMode){ //Sets if we are painting or placing decals
		mode = brushMode;
		brushCursor.GetComponent<SpriteRenderer> ().sprite = brushMode == Painter_BrushMode.PAINT ? cursorPaint : cursorDecal;
	}

    public void ClearTexture()
    {
        saving = true;
        SaveTexture(true);
    }

	////////////////// OPTIONAL METHODS //////////////////

	#if !UNITY_WEBPLAYER 
		IEnumerator SaveTextureToFile(Texture2D savedTexture){		
			brushCounter=0;
			string fullPath=System.IO.Directory.GetCurrentDirectory()+"\\UserCanvas\\";
			System.DateTime date = System.DateTime.Now;
			string fileName = "CanvasTexture.png";
			if (!System.IO.Directory.Exists(fullPath))		
				System.IO.Directory.CreateDirectory(fullPath);
			var bytes = savedTexture.EncodeToPNG();
			System.IO.File.WriteAllBytes(fullPath+fileName, bytes);
			Debug.Log ("<color=orange>Saved Successfully!</color>"+fullPath+fileName);
			yield return null;
		}
	#endif
}
