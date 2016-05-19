using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private float previewTextureOpacity = 0.0f;

	[SerializeField]
	private float previewBrushOpacity = 1.0f;

	[SerializeField]
	private float previewSize = 128.0f;

	[SerializeField]
	private bool showWireframe = true;

	[SerializeField]
	private bool showPreview;

	private void DrawPreview()
	{
		EditorGUILayout.Separator();

		BeginGroup(ref showPreview, "Preview"); if (showPreview == true)
		{
			BeginLabelWidth(Mathf.Min(85.0f, position.width * 0.5f));
			{
				previewTextureOpacity = EditorGUILayout.Slider("Texture", previewTextureOpacity, 0.0f, 1.0f);

				previewBrushOpacity = EditorGUILayout.Slider("Brush", previewBrushOpacity, 0.0f, 1.0f);

				previewSize = EditorGUILayout.Slider("Size", previewSize, 64.0f, 512.0f);

				showWireframe = EditorGUILayout.Toggle("Wireframe", showWireframe);

				DrawCurrentTexture();
			}
			EndLabelWidth();
		}
		EndGroup();
	}

	private void DrawCurrentTexture()
	{
		if (currentTexture != null)
		{
			var rect1  = P3D_Helper.Reserve(previewSize, true);
			var rect2  = rect1;
			var aspect = currentTexture.width / (float)currentTexture.height;
			var ratio  = rect1.width / rect1.height;

			GUI.Box(rect1, "", "box");

			rect2.xMin += 1;
			rect2.yMin += 1;
			rect2.xMax -= 1;
			rect2.yMax -= 1;

			if (ratio > aspect)
			{
				rect2.width *= aspect / ratio;
			}
			else
			{
				rect2.height *= ratio / aspect;
			}

			rect2.center = rect1.center;

			GUI.DrawTexture(rect2, currentTexture, ScaleMode.StretchToFill);

			rect1.yMax -= 5.0f;

			EditorGUI.DropShadowLabel(rect1, "(" + currentTexture.width + " x " + currentTexture.height + ")");
		}
	}

	private void ShowTexturePreview()
	{
		if (currentTexture != null && previewTextureOpacity > 0.0f)
		{
			var meshFilter = lockedGameObject.GetComponent<MeshFilter>();

			if (meshFilter != null)
			{
				var mesh = meshFilter.sharedMesh;

				if (mesh != null)
				{
					P3D_TexturePreview.Show(mesh, currentMaterialIndex, lockedGameObject.transform, previewTextureOpacity, currentTexture, CurrentTiling, CurrentOffset);
				}
			}
		}
	}

	private void ShowBrushPreview(Camera camera, Vector2 mousePosition)
	{
		if (lockedMesh != null && currentTexture != null && previewBrushOpacity > 0.0f)// && currentTool != ToolType.Fill)
		{
			var ray           = HandleUtility.GUIPointToWorldRay(mousePosition);
			var startPosition = ray.origin + ray.direction * camera.nearClipPlane;
			var endPosition   = ray.origin + ray.direction * camera.farClipPlane;
			var operation     = P3D_PaintOperation.Preview(lockedMesh, currentMaterialIndex, lockedGameObject.transform, currentBrush.Shape, currentBrush.Color, CurrentTiling, CurrentOffset);

			painter.SetMesh(lockedGameObject.transform, lockedMesh, currentMaterialIndex);

			painter.SetTexture(currentTexture, CurrentTiling, CurrentOffset);

			painter.Opacity = previewBrushOpacity;

			painter.Size = currentBrush.Size;

			painter.Angle = currentBrush.Angle;

			painter.SetOperation(operation);

			if (passThrough == true)// && currentTool != ToolType.Fill)
			{
				painter.PaintBetweenAll(startPosition, endPosition);
            }
			else
			{
				painter.PaintBetweenNearest(startPosition, endPosition);
			}
		}
	}
}
