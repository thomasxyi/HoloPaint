using UnityEngine;
using System.Collections.Generic;

// This is the main painting class, it stores a tree of mesh data (for fast access), and provides methods to raycast and paint into the data
//[System.Serializable]
public class P3D_Painter
{
	public bool Dirty;

	// The size of the current paint operation
	public Vector2 Size;
	
	// The angle of the current paint operation
	public float Angle;
	
	// The opacity of the current paint operation
	public float Opacity;

	[SerializeField]
	private Transform transform;

	// The current texture we're painting to
	[SerializeField]
	private Texture2D texture;

	// The current texture's tiling values
	[SerializeField]
	private Vector2 tiling = Vector2.one;
	
	// The current texture's offset values
	[SerializeField]
	private Vector2 offset;

	// The current painting operation (brush) used when painting
	//[SerializeField]
	private PaintOperation paintOperation;

	private P3D_Tree tree;

	private Mesh bakedMesh;

	// The paint operation method signature
	public delegate void PaintOperation(Texture2D texture, P3D_Matrix matrix, float opacity);

	public P3D_Tree Tree
	{
		get
		{
			return tree;
		}
	}

	public Transform Transform
	{
		get
		{
			return transform;
        }
	}

	public bool CanMeshPaint
	{
		get
		{
			return tree != null && tree.IsReady == true && transform != null;
		}
	}

	// This allows you to change which mesh is currently used when doing painting, via GameObject
	// NOTE: If you're using MeshCollider raycasting then you don't need to call this, as you can pass the hit UV coordinates directly to the Paint method
	// NOTE: The material index is passed here because it alters the submesh used by the mesh
	public void SetMesh(GameObject newGameObject, int newMaterialIndex = 0, bool forceUpdate = false)
	{
		if (newGameObject != null)
		{
			var mesh = P3D_Helper.GetMesh(newGameObject, ref bakedMesh);

			if (bakedMesh != null)
			{
				forceUpdate = true;
			}

			SetMesh(newGameObject.transform, mesh, newMaterialIndex, forceUpdate);
		}
	}

	// This allows you to change which mesh is currently used when doing painting
	// NOTE: If you're using MeshCollider raycasting then you don't need to call this, as you can pass the hit UV coordinates directly to the Paint method
	// NOTE: The material index is passed here because it alters the submesh used by the mesh
	public void SetMesh(Transform newTransform, Mesh newMesh, int newMaterialIndex = 0, bool forceUpdate = false)
	{
		if (tree == null)
		{
			tree = new P3D_Tree();
        }

		transform = newTransform;

		tree.SetMesh(newMesh, newMaterialIndex, forceUpdate);
    }

	// This allows you to change which texture is currently used when doing painting, via GameObject
	public void SetTexture(GameObject gameObject, string textureName = "_MainTex", int newMaterialIndex = 0)
	{
		var material = P3D_Helper.GetMaterial(gameObject, newMaterialIndex);

		if (material != null)
		{
			SetTexture(material.GetTexture(textureName) as Texture2D, material.GetTextureScale(textureName), material.GetTextureOffset(textureName));
		}
		else
		{
			SetTexture(null, Vector2.zero, Vector2.zero);
		}
	}

	// This allows you to change which texture is currently used when doing painting
	public void SetTexture(Texture newTexture)
	{
		SetTexture(newTexture, Vector2.one, Vector2.zero);
	}

	// This allows you to change which texture is currently used when doing painting
	public void SetTexture(Texture newTexture, Vector2 newTiling, Vector2 newOffset)
	{
		var newTexture2D = newTexture as Texture2D;

		if (newTexture2D != null && tiling.x != 0.0f && tiling.y != 0.0f && P3D_Helper.IsWritableFormat(newTexture2D.format) == true)
		{
			texture = newTexture2D;
			tiling  = newTiling;
			offset  = newOffset;
#if UNITY_EDITOR
			P3D_Helper.MakeTextureReadable(texture);
#endif
		}
		else
		{
			texture        = null;
			paintOperation = null;
		}
	}

	// This method allows you to prepare painting using a brush
	public void SetBrush(P3D_Brush newBrush)
	{
		paintOperation = newBrush.Create();
		Size           = newBrush.Size;
		Angle          = newBrush.Angle;
		Opacity        = newBrush.Opacity;
    }

	// This method allows you to prepare painting using a paint operation
	public void SetOperation(PaintOperation newPaintOperation)
	{
		paintOperation = newPaintOperation;
	}

	// This applys all texture changes after you've finished painting
	public void ApplyPaint()
	{
		if (Dirty == true)
		{
			Dirty = false;

			if (texture != null)
			{
				texture.Apply();
			}
		}
	}

	// This causes the current paint operation to get applied to the specified matrix in pixel space
	public void Paint(P3D_Matrix matrix)
	{
		if (texture != null && paintOperation != null)
		{
			Dirty = true;

			paintOperation(texture, matrix, Opacity);
		}
	}

	// This causes the current paint operation to get applied to the specified u/v coordinate
	public void Paint(Vector2 uv)
	{
		var xy = default(Vector2);

		if (CalculatePixelFromCoord(uv, ref xy) == true)
		{
			Paint(P3D_Helper.CreateMatrix(xy, Size, Angle));
		}
	}

	// This converts a 0..1 UV coordinate to a pixel coordination for the current texture
	public bool CalculatePixelFromCoord(Vector2 uv, ref Vector2 xy)
	{
		if (texture != null)
		{
			uv.x = Mathf.Repeat(uv.x * tiling.x + offset.x, 1.0f);
			uv.y = Mathf.Repeat(uv.y * tiling.y + offset.y, 1.0f);

			var w = texture.width;
			var h = texture.height;

			xy.x = Mathf.Clamp(Mathf.RoundToInt(uv.x * w), 0, w - 1);
			xy.y = Mathf.Clamp(Mathf.RoundToInt(uv.y * h), 0, h - 1);

			return true;
		}

		return false;
	}

	// This causes the current paint operation to get applied to the specified result
	public void Paint(P3D_Result result, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (result != null)
		{
			switch (coord)
			{
				case P3D_CoordType.UV1: Paint(result.UV1); break;
				case P3D_CoordType.UV2: Paint(result.UV2); break;
			}
		}
	}

	// This causes the current paint operation to get applied to all the specified results
	public void Paint(List<P3D_Result> results, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (results != null)
		{
			for (var i = 0; i < results.Count; i++)
			{
				Paint(results[i], coord);
			}
		}
	}

	// This paints the nearest triangles to the input position within maxDistance
	// NOTE: This method requires you to call SetMesh first
	public void PaintNearest(Vector3 position, float maxDistance, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (CanMeshPaint == true)
		{
			position = transform.InverseTransformPoint(position);

			Paint(tree.FindNearest(position, maxDistance), coord);
		}
	}

	// This paints the nearest triangles between the input positions
	// NOTE: This method requires you to call SetMesh first
	public void PaintBetweenNearest(Vector3 startPosition, Vector3 endPosition, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (CanMeshPaint == true)
		{
			startPosition = transform.InverseTransformPoint(startPosition);
			endPosition   = transform.InverseTransformPoint(  endPosition);

			Paint(tree.FindBetweenNearest(startPosition, endPosition), coord);
		}
	}

	// This paints all triangles between the input positions
	// NOTE: This method requires you to call SetMesh first
	public void PaintBetweenAll(Vector3 startPosition, Vector3 endPosition, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (CanMeshPaint == true)
		{
			startPosition = transform.InverseTransformPoint(startPosition);
			endPosition   = transform.InverseTransformPoint(  endPosition);

			Paint(tree.FindBetweenAll(startPosition, endPosition), coord);
		}
	}

	// This paints the nearest triangle along the ray
	// NOTE: This method requires you to call SetMesh first
	public void PaintBetweenNearest(Ray ray, float maxDistance, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (CanMeshPaint == true)
		{
			var startPosition = transform.InverseTransformPoint(ray.origin);
			var endPosition   = startPosition + transform.InverseTransformDirection(ray.direction) * maxDistance;

			Paint(tree.FindBetweenNearest(startPosition, endPosition), coord);
		}
	}

	// This paints all triangles along the ray
	// NOTE: This method requires you to call SetMesh first
	public void PaintBetweenAll(Ray ray, float maxDistance, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (CanMeshPaint == true)
		{
			var startPosition = transform.InverseTransformPoint(ray.origin);
			var endPosition   = startPosition + transform.InverseTransformDirection(ray.direction) * maxDistance;

			Paint(tree.FindBetweenAll(startPosition, endPosition), coord);
		}
	}

	// This paints the nearest triangle perpendicular to the input position
	// NOTE: This method requires you to call SetMesh first
	public void PaintPerpendicularNearest(Vector3 position, float maxDistance, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (CanMeshPaint == true)
		{
			position = transform.InverseTransformPoint(position);

			Paint(tree.FindPerpendicularNearest(position, maxDistance), coord);
		}
	}

	// This paints all triangles perpendicular to the input position
	// NOTE: This method requires you to call SetMesh first
	public void PaintPerpendicularAll(Vector3 position, float maxDistance, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (CanMeshPaint == true)
		{
			position = transform.InverseTransformPoint(position);

			Paint(tree.FindPerpendicularAll(position, maxDistance), coord);
		}
	}
}
