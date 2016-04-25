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

public enum Painter_BrushMode{PAINT,DECAL};
public class TexturePainter : MonoBehaviour {
	public GameObject brushCursor,brushContainer; //The cursor that overlaps the model and our container for the brushes painted
	public Camera sceneCamera,canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
	public Sprite cursorPaint,cursorDecal; // Cursor for the differen functions 
	public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
	public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)

	Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)
	float brushSize=1.0f; //The size of our brush
	Color brushColor; //The selected color
	int brushCounter=0,MAX_BRUSH_COUNT=1000; //To avoid having millions of brushes
	bool saving=false; //Flag to check if we are saving the texture


    /// <summary>
    /// To select even when a hologram is not being gazed at,
    /// set the override focused object.
    /// If its null, then the gazed at object will be selected.
    /// </summary>
    public GameObject OverrideFocusedObject
    {
        get; set;
    }
    private GestureRecognizer gestureRecognizer;
    private GameObject focusedObject;

    void Start()
    {
        // Create a new GestureRecognizer. Sign up for tapped events.
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

        gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

        // Start looking for gestures.
        gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (focusedObject != null)
        {
            DoAction();
        }
    }


    void Update ()
    {
        GameObject oldFocusedObject = focusedObject;

        if (GazeManager.Instance.Hit &&
            OverrideFocusedObject == null &&
            GazeManager.Instance.HitInfo.collider != null)
        {
            // If gaze hits a hologram, set the focused object to that game object.
            // Also if the caller has not decided to override the focused object.
            focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
        }
        else
        {
            // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
            focusedObject = OverrideFocusedObject;
        }

        if (focusedObject != oldFocusedObject)
        {
            // If the currently focused object doesn't match the old focused object, cancel the current gesture.
            // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
            gestureRecognizer.CancelGestures();
            gestureRecognizer.StartCapturingGestures();
        }
        brushColor = Color.red;//ColorSelector.GetColor ();	//Updates our painted color with the selected color
		UpdateBrushCursor ();
	}

    void OnDestroy()
    {
        gestureRecognizer.StopCapturingGestures();
        gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    void DoAction(){	
		if (saving)
			return;
		Vector3 uvWorldPosition=Vector3.zero;		
		if(HitTestUVPosition(ref uvWorldPosition)){
			GameObject brushObj;
			if(mode==Painter_BrushMode.PAINT){

				brushObj=(GameObject)Instantiate(Resources.Load("TexturePainter-Instances/BrushEntity")); //Paint a brush
				brushObj.GetComponent<SpriteRenderer>().color=brushColor; //Set the brush color
			}
			else{
				brushObj=(GameObject)Instantiate(Resources.Load("TexturePainter-Instances/DecalEntity")); //Paint a decal
			}
			brushColor.a=brushSize*2.0f; // Brushes have alpha to have a merging effect when painted over.
			brushObj.transform.parent=brushContainer.transform; //Add the brush to our container to be wiped later
			brushObj.transform.localPosition=uvWorldPosition; //The position of the brush (in the UVMap)
			brushObj.transform.localScale=Vector3.one*brushSize;//The size of the brush
		}
		brushCounter++; //Add to the max brushes
		if (brushCounter >= MAX_BRUSH_COUNT) { //If we reach the max brushes available, flatten the texture and clear the brushes
			brushCursor.SetActive (false);
			saving=true;
			Invoke("SaveTexture",0.1f);
			
		}
	}
	//To update at realtime the painting cursor on the mesh
	void UpdateBrushCursor(){
		Vector3 uvWorldPosition=Vector3.zero;
		if (HitTestUVPosition (ref uvWorldPosition) && !saving) {
			brushCursor.SetActive(true);
			brushCursor.transform.position =uvWorldPosition+brushContainer.transform.position;									
		} else {
			brushCursor.SetActive(false);
		}		
	}
	//Returns the position on the texuremap according to a hit in the mesh collider
	bool HitTestUVPosition(ref Vector3 uvWorldPosition){
		RaycastHit hit = GazeManager.Instance.HitInfo;
		if (GazeManager.Instance.Hit){
			Vector2 pixelUV  = new Vector2(hit.textureCoord.x,hit.textureCoord.y);
			uvWorldPosition.x=pixelUV.x-canvasCam.orthographicSize;//To center the UV on X
			uvWorldPosition.y=pixelUV.y-canvasCam.orthographicSize;//To center the UV on Y
			uvWorldPosition.z=0.0f;
			return true;
		}
		else{		
			return false;
		}
		
	}
	//Sets the base material with a our canvas texture, then removes all our brushes
	void SaveTexture(){		
		brushCounter=0;
		System.DateTime date = System.DateTime.Now;
		RenderTexture.active = canvasTexture;
		Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);		
		tex.ReadPixels (new Rect (0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
		tex.Apply ();
		RenderTexture.active = null;
		baseMaterial.mainTexture =tex;	//Put the painted texture as the base
		foreach (Transform child in brushContainer.transform) {//Clear brushes
			Destroy(child.gameObject);
		}
		//StartCoroutine ("SaveTextureToFile"); //Do you want to save the texture? This is your method!
		Invoke ("ShowCursor", 0.1f);
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
	public void SetBrushSize(float newBrushSize){ //Sets the size of the cursor brush or decal
		brushSize = newBrushSize;
		brushCursor.transform.localScale = Vector3.one * brushSize;
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
