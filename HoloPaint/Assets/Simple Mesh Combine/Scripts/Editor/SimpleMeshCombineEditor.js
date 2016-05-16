/****************************************
	Simple Mesh Combine v1.61
	Copyright 2015 Unluck Software	
 	www.chemicalbliss.com
 	
 	Change Log
 		v1.1
 		Added naming and prefab save option	
 		
 		v1.2
 		Added lightmap support		
 		
 		v1.3
 		Added multiple material support
 			v1.301
 			Fixed compile error trying to unwrap UV in game mode	
 		
 		v1.4
 		Added C# scripts
 		
 		v1.41 - 22.01.2015
 		Changed from using SharedMaterial.Name to SharedMaterial directly to identify different materials
 		Fixed error when combining meshes with more submeshes than materials
 		
 		v1.5 -24.01.2015
 		Improved editor layout, added more info and tips
 		Lightmap option as own function
 		Now sets UV2 to null to reduce mesh size
 				 		
 		v1.53 -31.03.2015
 		Fixed lightmapping for Unity 5
 		
 		v1.54 -01.05.2015
 		Fixed build error Unity 5
 		
 		v1.6 & v1.61 - 28.05.2015
 		Added Export to OBJ (Beta)
 			- Used to fix/optimize combined meshes in external 3D sofware
 				- Submesh/mulitple material support limited
 		Improved Save Mesh asset
 			- Save to custom folder 
 			- Overwrite keeps prefab
 		Added Copy functionions
 			- Duplicates gameObject then removes all components and empty gameObjects exept Colliders
 			- Used to create prefabs with combined mesh + colliders
 		Fixed: Deleting combined gameObject no longer gives null error
 		
*****************************************/
import System.IO;
import System.Text;
import System.Collections.Generic;
@CustomEditor(SimpleMeshCombine)
public class SimpleMeshCombineEditor extends Editor {
	function ExportMesh(meshFilter: MeshFilter, folder: String, filename: String) {
		var path: String = SaveFile(folder, filename, "obj");
		if (path != null) {
			var sw: StreamWriter = new StreamWriter(path);
			sw.Write(MeshToString(meshFilter));
			sw.Flush();
			sw.Close();
			AssetDatabase.Refresh();
			Debug.Log("Exported OBJ file to folder: " + path);
		}
	}

	function MeshToString(meshFilter: MeshFilter): String {
		var sMesh: Mesh = meshFilter.sharedMesh;
		var stringBuilder: StringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(meshFilter.name).Append("\n");
		for (var vert: Vector3 in sMesh.vertices) {
			var tPoint: Vector3 = meshFilter.transform.TransformPoint(vert);
			stringBuilder.Append(String.Format("v {0} {1} {2}\n", -tPoint.x, tPoint.y, tPoint.z));
		}
		stringBuilder.Append("\n");
		for (var norm: Vector3 in sMesh.normals) {
			var tDir: Vector3 = meshFilter.transform.TransformDirection(norm);
			stringBuilder.Append(String.Format("vn {0} {1} {2}\n", -tDir.x, tDir.y, tDir.z));
		}
		stringBuilder.Append("\n");
		for (var uv: Vector3 in sMesh.uv) {
			stringBuilder.Append(String.Format("vt {0} {1}\n", uv.x, uv.y));
		}
		for (var material: int = 0; material < sMesh.subMeshCount; material++) {
			stringBuilder.Append("\n");
			var tris: int[] = sMesh.GetTriangles(material);
			for (var i: int = 0; i < tris.Length; i += 3) {
				stringBuilder.Append(String.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", tris[i] + 1, tris[i + 1] + 1, tris[i + 2] + 1));
			}
		}
		return stringBuilder.ToString();
	}
	
	function SaveFile(folder: String, name: String, type: String):String {
		var newPath:String = "";
		var path = EditorUtility.SaveFilePanel("Select Folder ", folder, name, type);
		if (path.Length > 0) {
			if (path.Contains("" + Application.dataPath)) {
				var s: String = "" + path + "";
				var d: String = "" + Application.dataPath + "/";
				var p: String = "Assets/" + s.Remove(0, d.Length);
				var cancel: boolean;
				newPath = p;
			} else {
				Debug.LogError("Prefab Save Failed: Can't save outside project: " + path);
			}
		}
		return newPath;
	}
	
	var tex: Texture = Resources.Load("SMC_Title");
	override function OnInspectorGUI() {
		//
		//	STYLE AND COLOR
		//
		var color2: Color = Color.yellow;
		var color1: Color = Color.cyan;
		var buttonStyle = new GUIStyle(GUI.skin.button);
		var buttonStyle2 = new GUIStyle(GUI.skin.button);
		var titleStyle = new GUIStyle(GUI.skin.label);
		buttonStyle.fontStyle = FontStyle.Bold;
		buttonStyle.fixedWidth = 150;
		buttonStyle.fixedHeight = 35;
		buttonStyle.fontSize = 15;
		buttonStyle2.fixedWidth = 200;
		buttonStyle2.fixedHeight = 25;
		buttonStyle2.margin = RectOffset((Screen.width - 200) * .5, (Screen.width - 200) * .5, 0, 0);
		buttonStyle.margin = RectOffset((Screen.width - 150) * .5, (Screen.width - 150) * .5, 0, 0);
		titleStyle.fixedWidth = 256;
		titleStyle.fixedHeight = 64;
		titleStyle.margin = RectOffset((Screen.width - 256) * .5, (Screen.width - 256) * .5, 0, 0);
		var infoStyle = new GUIStyle(GUI.skin.label);
		infoStyle.fontSize = 10;
		infoStyle.margin.top = 0;
		infoStyle.margin.bottom = 0;
		GUILayout.Label(tex, titleStyle);
		
		if (!Application.isPlaying) {
			GUI.enabled = true;
		} else {
			GUILayout.Label("Editor can't combine in play-mode", infoStyle);
			GUILayout.Label("Use SimpleMeshCombine.CombineMeshes();", infoStyle);
			GUI.enabled = false;
		}
		GUILayout.Space(15);
		//
		//	COMBINE MESH AREA
		//
		GUI.color = color1;
		if (target.combinedGameOjects == null || target.combinedGameOjects.Length == 0) {
			if (GUILayout.Button("Combine", buttonStyle)) {
				if (target.transform.childCount > 1) target.CombineMeshes();
				target.combined.isStatic = true;
			}
		} else {
			if (GUILayout.Button("Release", buttonStyle)) {
				target.EnableRenderers(true);		
				if (target.combined) DestroyImmediate(target.combined);
				target.combinedGameOjects = null;
			}		
		}
		GUILayout.Space(5);
		//
		//	SAVE MESH AREA
		//
		if (target.combined) {
			if (!target._canGenerateLightmapUV) {
				GUILayout.Label("Warning: Mesh has too high vertex count", EditorStyles.boldLabel);
				GUI.enabled = false;
			}
			
			if (target.combined.GetComponent(MeshFilter).sharedMesh.name != "") {			
				GUI.enabled = false;
			} else if(!Application.isPlaying){
				GUI.enabled = true;
			}
			
			if (GUILayout.Button("Save Mesh", buttonStyle2)) {
				var path: String = SaveFile("Assets/", target.transform.name + " [SMC Asset]", "asset");
				if (path != null) {
					var asset = AssetDatabase.LoadAssetAtPath(path, Object);
					if (asset == null) {
						AssetDatabase.CreateAsset(target.combined.GetComponent(MeshFilter).sharedMesh, path);
					} else {
						asset.Clear();
						EditorUtility.CopySerialized(target.combined.GetComponent(MeshFilter).sharedMesh, asset);
						AssetDatabase.SaveAssets();
					}
					target.combined.GetComponent("MeshFilter").sharedMesh = AssetDatabase.LoadAssetAtPath(path, Object);
					Debug.Log("Saved mesh asset: " + path);
				}
			}
			GUILayout.Space(5);
		}
		
		if(!Application.isPlaying){
				GUI.enabled = true;
			}
		
		if (target.combined) {		
			if (GUILayout.Button("Export OBJ", buttonStyle2)) {
				if (target.combined) {
					ExportMesh(target.combined.GetComponent(MeshFilter), "Assets/", target.transform.name + " [SMC Mesh]" + ".obj");
				}
			}
			GUILayout.Space(15);		
			//
			// COPY
			//
			GUI.color = color2;
			var bText:String = "Create Copy";
			if (target.combined.GetComponent(MeshFilter).sharedMesh.name == "") {			
				bText = bText + " (Saved mesh)";
				GUI.enabled = false;
			} else if(!Application.isPlaying){
				GUI.enabled = true;
			}
			
			if (GUILayout.Button(bText, buttonStyle2)) {
				var newCopy = new GameObject();
				var newCopy2 = new GameObject();
				newCopy2.transform.parent = newCopy.transform;
				newCopy2.transform.localPosition = target.combined.transform.localPosition;
				newCopy2.transform.localRotation = target.combined.transform.localRotation;
				newCopy.name = target.name + " [SMC Copy]";		
				newCopy2.name = "Mesh [SMC]";	
				newCopy.transform.position = target.transform.position;
				newCopy.transform.rotation = target.transform.rotation;
				
				
				var mf = newCopy2.AddComponent(MeshFilter);
				newCopy2.AddComponent(MeshRenderer);
				mf.sharedMesh = target.combined.GetComponent(MeshFilter).sharedMesh;
				
				target.copyTarget = newCopy;
				CopyMaterials(newCopy2.transform);
				CopyColliders();
				Selection.activeTransform = newCopy.transform;
				
			}
			
			
			GUILayout.Space(5);
			if (!target.copyTarget) {
				GUI.enabled = false;
			} else if(!Application.isPlaying){
				GUI.enabled = true;
			}
			if (GUILayout.Button("Copy Colliders", buttonStyle2)) {
					CopyColliders();
			}
			GUILayout.Space(5);
			if (GUILayout.Button("Copy Materials", buttonStyle2)) {
				CopyMaterials(target.copyTarget.transform.FindChild("Mesh [SMC]"));
			}
			
			if (!Application.isPlaying) {
				GUI.enabled = true;
			}
			target.destroyOldColliders = EditorGUILayout.Toggle("Destroy old colliders", target.destroyOldColliders);
			target.keepStructure = EditorGUILayout.Toggle("Keep collider structure", target.keepStructure);
			target.copyTarget = EditorGUILayout.ObjectField("Copy to: ", target.copyTarget, GameObject, true);
		}
		
		if(!target.combined){
			target.generateLightmapUV = EditorGUILayout.Toggle("Create Lightmap UV", target.generateLightmapUV);
		}
		
		GUILayout.Space(5);
		GUI.color = color1;
		EditorGUILayout.BeginVertical("Box");
		if (target.combined) {
			GUILayout.Label("Vertex count: " + target.vCount + " / 65536", infoStyle);
			GUILayout.Label("Material count: " + target.combined.GetComponent(Renderer).sharedMaterials.length, infoStyle);
		}else{
			GUILayout.Label("Vertex count: - / 65536", infoStyle);
			GUILayout.Label("Material count: -", infoStyle);
			
		}
		EditorGUILayout.EndVertical();
		
		
		
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
	
	function DestroyComponentsExeptColliders(t:Transform){
		var transforms = t.GetComponentsInChildren(typeof(Transform));
		for(var trans : Transform in transforms){ 
			if(!target.keepStructure && trans.transform.parent != t && trans.transform != t && trans.GetComponent(Collider)){
	        	trans.transform.name = ""+ GetParentStructure(t, trans.transform);
	         	trans.transform.parent = t;      
	        }
		}
		var components = t.GetComponentsInChildren(typeof(Component));
        for(var comp : Component in components){      
	        if( !( comp instanceof Collider) && !( comp instanceof GameObject) && !( comp instanceof Transform) ){    				
	             DestroyImmediate(comp);  
			}
        }
	}
	
	function GetParentStructure(root:Transform, t:Transform):String{
		var ct:Transform = t;
		var s:String = "";
		while(ct !=root ){	
			s = s.Insert(0, ct.name + " - ");	
			ct = ct.parent;
			
		}
		s = s.Remove(s.Length-3, 3);
		return s;
	}
	
	function DestroyEmptyGameObjects(t:Transform){
		var components = t.GetComponentsInChildren(typeof(Transform));
		  for(var comp : Transform in components){
		  	if(comp && (comp.childCount == 0 || !CheckChildrenForColliders(comp))){
		  		var col:Collider = comp.GetComponent(Collider);
		  		if(!col){
		  			DestroyImmediate(comp.gameObject);
		  		}
		  	}
		}
	}
	function CheckChildrenForColliders(t:Transform):boolean{		
		var components = t.GetComponentsInChildren(typeof(Collider));
		if(components.Length > 0){
			return true;
		}
		return false;
	}
	
	function CopyMaterials(t:Transform){
		var r = t.GetComponent(Renderer);
		r.sharedMaterials = target.combined.transform.GetComponent(Renderer).sharedMaterials;
	}
	
	function CopyColliders(){
		var clone:GameObject = Instantiate(target.gameObject,  target.copyTarget.transform.position,  target.copyTarget.transform.rotation);
		if(target.destroyOldColliders){
			var o = target.copyTarget.transform.FindChild("Colliders [SMC]");
			if(o){
				DestroyImmediate(o.gameObject);
			}
		}				
		clone.transform.name = "Colliders [SMC]";
		clone.transform.parent = target.copyTarget.transform;				    
		DestroyComponentsExeptColliders(clone.transform);
		DestroyEmptyGameObjects(clone.transform);
	}
}