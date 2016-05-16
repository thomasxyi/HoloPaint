using UnityEngine;

// This stores all the data for a single  This can be passed to P3D painter to construct a paint operation
[System.Serializable]
public class P3D_Brush
{
	public string Name = "Default";

	[Range(0.0f, 1.0f)]
	public float Opacity = 1.0f;

	[Range(-Mathf.PI, Mathf.PI)]
	public float Angle;

	public Vector2 Size = new Vector2(10.0f, 10.0f);

	public P3D_BlendMode Blend = P3D_BlendMode.AlphaBlend;

	public Color Color = Color.white;
	
	public Vector2 Direction;

	public Texture2D Shape;
	
	public Texture2D Detail;

	public Vector2 DetailScale = new Vector2(0.5f, 0.5f);

	// This will create a paint operation based on these brush settings
	public P3D_Painter.PaintOperation Create()
	{
		var paintOperation = default(P3D_Painter.PaintOperation);

		switch (Blend)
		{
			case P3D_BlendMode.AlphaBlend:
			{
				paintOperation = P3D_PaintOperation.AlphaBlend(Color, Shape, Detail, DetailScale);
			}
			break;

			case P3D_BlendMode.AlphaBlendRgb:
			{
				paintOperation = P3D_PaintOperation.AlphaBlendRgb(Color, Shape, Detail, DetailScale);
			}
			break;

			case P3D_BlendMode.AlphaErase:
			{
				paintOperation = P3D_PaintOperation.AlphaErase(Shape, Detail, DetailScale);
			}
			break;

			case P3D_BlendMode.AdditiveBlend:
			{
				paintOperation = P3D_PaintOperation.AdditiveBlend(Color, Shape, Detail, DetailScale);
			}
			break;

			case P3D_BlendMode.SubtractiveBlend:
			{
				paintOperation = P3D_PaintOperation.SubtractiveBlend(Color, Shape, Detail, DetailScale);
			}
			break;

			case P3D_BlendMode.NormalBlend:
			{
				paintOperation = P3D_PaintOperation.NormalBlend(Direction, Shape, Detail, DetailScale);
			}
			break;
		}

		return paintOperation;
	}
}
