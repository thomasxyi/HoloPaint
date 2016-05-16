using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	private static string[] toolNames = new string[] { "Alpha Blend", "Alpha Blend RGB", "Alpha Erase", "Additive Blend", "Subtractive Blend", "Normal Blend", "Alpha Blend Decal" };

	private static int[] toolValues = new int[] { 0, 1, 2, 3, 4, 5, 6 };
	
	[SerializeField]
	private P3D_Painter painter = new P3D_Painter();

	[SerializeField]
	private P3D_Brush currentBrush = new P3D_Brush();
	
	[SerializeField]
	private bool showBrush = true;

	private void DrawBrush()
	{
		EditorGUILayout.Separator();

		BeginGroup(ref showBrush, "Brush"); if (showBrush == true)
		{
			BeginLabelWidth(Mathf.Min(85.0f, position.width * 0.5f));
			{
				currentBrush.Name = EditorGUILayout.TextField("Name", currentBrush.Name);

				currentBrush.Opacity = EditorGUILayout.Slider("Opacity", currentBrush.Opacity, 0.0f, 1.0f);

				currentBrush.Angle = EditorGUILayout.Slider("Angle", currentBrush.Angle, -Mathf.PI, Mathf.PI);

				currentBrush.Size = EditorGUILayout.Vector2Field("Size", currentBrush.Size);

				currentBrush.Blend = (P3D_BlendMode)EditorGUI.IntPopup(P3D_Helper.Reserve(), "Blend", (int)currentBrush.Blend, toolNames, toolValues);

				if (string.IsNullOrEmpty(currentTexEnvName) == false)
				{
					var texEnvLwr = currentTexEnvName.ToLowerInvariant();

					if (currentBrush.Blend != P3D_BlendMode.NormalBlend)
					{
						if (texEnvLwr.Contains("normal") == true || texEnvLwr.Contains("bump") == true)
						{
							EditorGUILayout.HelpBox("This material's texture slot might be a normal map. If so, use the normal blending mode", MessageType.Warning);
						}
					}
					else
					{
						if (texEnvLwr.Contains("normal") == false && texEnvLwr.Contains("bump") == false)
						{
							EditorGUILayout.HelpBox("This material's texture slot might not be a normal map. If so, use a different blending mode", MessageType.Warning);
						}
					}
				}

				EditorGUILayout.Separator();

				switch (currentBrush.Blend)
				{
					case P3D_BlendMode.AlphaBlend:
					{
						DrawColor();
						DrawShapeSize();
						DrawDetailAndTiling();
					}
					break;

					case P3D_BlendMode.AlphaBlendRgb:
					{
						DrawColor();
						DrawShapeSize();
						DrawDetailAndTiling();
					}
					break;

					case P3D_BlendMode.AlphaErase:
					{
						DrawShapeSize();
						DrawDetailAndTiling();
					}
					break;

					case P3D_BlendMode.AdditiveBlend:
					{
						DrawColor();
						DrawShapeSize();
					}
					break;

					case P3D_BlendMode.SubtractiveBlend:
					{
						DrawColor();
						DrawShapeSize();
						DrawDetailAndTiling();
					}
					break;

					case P3D_BlendMode.NormalBlend:
					{
						DrawDirection();
						DrawShapeSize();
						DrawDetailAndTiling();
					}
					break;
				}

				DrawSaveBrush();
			}
			EndLabelWidth();
		}
		EndGroup();
	}

	private void DrawDirection()
	{
		currentBrush.Direction = DirectionField(P3D_Helper.Reserve(32.0f), "Direction", currentBrush.Direction);
	}

	private void DrawColor()
	{
		currentBrush.Color = EditorGUILayout.ColorField("Color", currentBrush.Color);
	}
	
	private void DrawShapeSize()
	{
		EditorGUILayout.Separator();

		currentBrush.Shape = (Texture2D)EditorGUI.ObjectField(P3D_Helper.Reserve(), "Shape", currentBrush.Shape, typeof(Texture2D), true);
	}

	private void DrawDetailAndTiling()
	{
		EditorGUILayout.Separator();

		currentBrush.Detail = (Texture2D)EditorGUI.ObjectField(P3D_Helper.Reserve(), "Detail", currentBrush.Detail, typeof(Texture2D), true);

		currentBrush.DetailScale = EditorGUILayout.Vector2Field("Detail Scale", currentBrush.DetailScale);
	}
	
	private void DrawSaveBrush()
	{
		EditorGUILayout.Separator();
		
		var rect   = P3D_Helper.Reserve(16.0f, true);
		var exists = PresetBrushes.Exists(b => b.Name == currentBrush.Name);
		
		if (GUI.Button(rect, exists == true ? "Overwrite Preset" : "Save Preset") == true)
		{
			var presetBrush = PresetBrushes.Find(b => b.Name == currentBrush.Name);
			
			if (presetBrush == null)
			{
				presetBrush = new P3D_Brush();
			
				PresetBrushes.Add(presetBrush);
			}
		
			presetBrush.Name         = currentBrush.Name.Replace("\n", "");
			presetBrush.Blend        = currentBrush.Blend;
			presetBrush.Color        = currentBrush.Color;
			presetBrush.Direction    = currentBrush.Direction;
			presetBrush.Shape        = currentBrush.Shape;
			presetBrush.Size         = currentBrush.Size;
			presetBrush.Detail       = currentBrush.Detail;
			presetBrush.DetailScale = currentBrush.DetailScale;
		
			SavePresets();
		}
	}

	public void BeginPaint()
	{
		if (lockedMesh != null && lockedRenderer != null)
		{
			painter.SetMesh(lockedGameObject.transform, lockedMesh, currentMaterialIndex);

			painter.SetTexture(currentTexture, CurrentTiling, CurrentOffset);

			painter.SetBrush(currentBrush);
		}
	}

	public void Paint(Vector3 startPosition, Vector3 endPosition)
	{
		if (painter.CanMeshPaint == true)
		{
			var results = default(List<P3D_Result>);

			startPosition = painter.Transform.InverseTransformPoint(startPosition);
			endPosition   = painter.Transform.InverseTransformPoint(  endPosition);

			if (passThrough == true)
			{
				results = painter.Tree.FindBetweenAll(startPosition, endPosition);
			}
			else
			{
				results = painter.Tree.FindBetweenNearest(startPosition, endPosition);
			}

			for (var i = results.Count - 1; i >= 0; i--)
			{
				var result = results[i];

				switch (currentCoord)
				{
					case P3D_CoordType.UV1: Paint(result.UV1); break;
					case P3D_CoordType.UV2: Paint(result.UV2); break;
				}
			}
		}
	}

	private void Paint(Vector2 uv)
	{
		var xy = default(Vector2);

		if (painter.CalculatePixelFromCoord(uv, ref xy) == true)
		{
			var angle = painter.Angle;
			var size  = painter.Size;

			if (scatterPosition != 0.0f)
			{
				var direction = Random.insideUnitCircle;

				xy.x += Mathf.RoundToInt(direction.x * scatterPosition);
				xy.y += Mathf.RoundToInt(direction.y * scatterPosition);
			}

			if (scatterAngle > 0.0f)
			{
				painter.Angle += Random.Range(-scatterAngle, scatterAngle);
			}

			if (scatterScale > 0.0f)
			{
				size *= Random.Range(1.0f - scatterScale, 1.0f);
			}

			painter.Paint(P3D_Helper.CreateMatrix(xy, size, angle));
		}
	}

	public void EndPaint()
	{
		painter.ApplyPaint();
	}

	private static Vector2 DirectionField(Rect rect1, string name, Vector2 direction)
	{
		EditorGUI.LabelField(rect1, name);

		rect1.xMin += EditorGUIUtility.labelWidth;

		var rect3 = P3D_Helper.SplitHorizontal(ref rect1, 2);
		var rect2 = P3D_Helper.SplitVertical(ref rect3, 2);

		direction.x = EditorGUI.FloatField(rect3, "", direction.x);
		direction.y = EditorGUI.FloatField(rect2, "", direction.y);

		GUI.Box(rect1, "", "box");

		var rect4  = new Rect(0.0f, 0.0f, 4.0f, 4.0f);
		var center = default(Vector2);

		center.x = Mathf.Lerp(rect1.xMin, rect1.xMax, direction.x * 0.5f + 0.5f);
		center.y = Mathf.Lerp(rect1.yMin, rect1.yMax, direction.y * 0.5f + 0.5f);

		rect4.center = center;

		GUI.Box(rect4, "", "box");

		if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
		{
			if (Event.current.button == 0 && rect1.Contains(Event.current.mousePosition) == true)
			{
				direction.x = Mathf.InverseLerp(rect1.xMin, rect1.xMax, Event.current.mousePosition.x) * 2.0f - 1.0f;
				direction.y = Mathf.InverseLerp(rect1.yMin, rect1.yMax, Event.current.mousePosition.y) * 2.0f - 1.0f;
			}
		}

		if (direction.magnitude > 1.0f)
		{
			direction = direction.normalized;
		}

		return direction;
	}
}
