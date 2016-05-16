using UnityEngine;
using System.Collections.Generic;

public class P3D_Paintable : MonoBehaviour
{
	public static List<P3D_Paintable> AllPaintables = new List<P3D_Paintable>();

	[Tooltip("The material index we want to paint")]
	public int MaterialIndex;

	[Tooltip("The texture we want to paint")]
	public string TextureName = "_MainTex";

	[Tooltip("How many seconds between each time paint modifications get applied")]
	public float ApplyInterval = 0.01f;

	[Tooltip("Should the mesh data always update?")]
	public bool ForceUpdate;

	[Tooltip("Should the material and texture get duplicated on awake? (useful for prefab clones)")]
	public bool DuplicateOnAwake;

	[Tooltip("Should the texture get created on awake? (useful for saving scene file size)")]
	public bool CreateOnAwake;

	[Tooltip("The color of the created texture")]
	public P3D_Helper.BaseColors CreateColor;

	[Tooltip("The width of the created texture")]
	public int CreateWidth = 512;

	[Tooltip("The height of the created texture")]
	public int CreateHeight = 512;

	[Tooltip("Some shaders (e.g. Standard Shader) require you to enable keywords when adding new textures, you can specify that keyword here")]
	public string CreateKeyword;

	// This class stores mesh data, and allows you to paint onto textures using it
	[SerializeField]
	private P3D_Painter painter = new P3D_Painter();

	private float cooldown;

	public static void PaintBetweenNearest(Vector3 startPosition, Vector3 endPosition, P3D_Brush brush, int raycastMask = -1, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		var maxDistance = Vector3.Distance(startPosition, endPosition);

		if (maxDistance > 0.0f)
		{
			var bestPaintable  = default(P3D_Paintable);
			var bestDistance01 = 1.0f;
			var bestUV         = default(Vector2);

			for (var i = AllPaintables.Count - 1; i >= 0; i--)
			{
				var paintable = AllPaintables[i];
				var painter   = paintable.GetPainter();

				if (painter.CanMeshPaint == true)
				{
					var startPoint = painter.Transform.InverseTransformPoint(startPosition);
					var endPoint   = painter.Transform.InverseTransformPoint(endPosition);
					var results    = painter.Tree.FindBetweenNearest(startPoint, startPoint + (endPoint - startPoint) * bestDistance01);

					if (results.Count > 0)
					{
						var result = results[0];

						if (result.Distance01 < bestDistance01)
						{
							bestPaintable  = paintable;
							bestDistance01 = result.Distance01;
							bestUV         = GetUV(result, coord);
						}
					}
				}
			}

			var ray = new Ray(startPosition, endPosition - startPosition);
			var hit = default(RaycastHit);

			if (Physics.Raycast(ray, out hit, maxDistance * bestDistance01, raycastMask) == true)
			{
				bestPaintable = hit.collider.GetComponent<P3D_Paintable>();
				bestUV        = GetUV(hit, coord);
			}

			if (bestPaintable != null)
			{
				var painter = bestPaintable.GetPainter();

				painter.SetBrush(brush);

				painter.Paint(bestUV);
			}
		}
    }

	public static Vector2 GetUV(P3D_Result result, P3D_CoordType coord)
	{
		var uv = default(Vector2);

		if (result != null)
		{
			switch (coord)
			{
				case P3D_CoordType.UV1: uv = result.UV1; break;
				case P3D_CoordType.UV2: uv = result.UV2; break;
			}
		}

		return uv;
	}

	public static Vector2 GetUV(RaycastHit hit, P3D_CoordType coord)
	{
		var uv = default(Vector2);

		switch (coord)
		{
			case P3D_CoordType.UV1: uv = hit.textureCoord;  break;
			case P3D_CoordType.UV2: uv = hit.textureCoord2; break;
		}

		return uv;
	}

	public P3D_Painter GetPainter()
	{
		painter.SetMesh(gameObject, MaterialIndex, ForceUpdate);

		painter.SetTexture(gameObject, TextureName, MaterialIndex);

		return painter;
	}

	protected virtual void OnEnable()
	{
		AllPaintables.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllPaintables.Remove(this);
	}

	protected virtual void Awake()
	{
		if (DuplicateOnAwake == true)
		{
			// Get cloned material
			var material = P3D_Helper.CloneMaterial(gameObject, MaterialIndex);

			if (material != null)
			{
				// Get texture
				var texture = material.GetTexture(TextureName);

				if (texture != null)
				{
					// Clone material
					texture = P3D_Helper.Clone(texture);

					// Update material
					material.SetTexture(TextureName, texture);
				}
			}
		}

		if (CreateOnAwake == true && CreateWidth > 0 && CreateHeight > 0)
		{
			var material = P3D_Helper.GetMaterial(gameObject, MaterialIndex);

			if (material != null)
			{
				var texture = material.GetTexture(TextureName);

				if (texture != null)
				{
					Debug.LogWarning("There is already a texture in this texture slot, maybe set it to null to save memory?");
				}

				texture = P3D_Helper.CreateTexture(CreateColor, CreateWidth, CreateHeight);

				material.SetTexture(TextureName, texture);

				// Enable a keyword?
				if (string.IsNullOrEmpty(CreateKeyword) == false)
				{
					material.EnableKeyword(CreateKeyword);
				}
            }
        }
	}

	protected virtual void Update()
	{
		cooldown -= Time.deltaTime;

		if (cooldown <= 0.0f)
		{
			cooldown = ApplyInterval;

			if (painter != null && painter.Dirty == true)
			{
				painter.ApplyPaint();
			}
		}
	}
}
