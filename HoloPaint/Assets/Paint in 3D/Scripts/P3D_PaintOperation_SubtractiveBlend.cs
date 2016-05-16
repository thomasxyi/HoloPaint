using UnityEngine;

public static partial class P3D_PaintOperation
{
	public static P3D_Painter.PaintOperation SubtractiveBlend(Color color, Texture2D shape, Texture2D detail, Vector2 detailScale)
	{
#if UNITY_EDITOR
		P3D_Helper.MakeTextureReadable(shape);
		P3D_Helper.MakeTextureReadable(detail);
#endif
		return (Texture2D canvas, P3D_Matrix matrix, float opacity) =>
		{
			var rect = default(P3D_Rect);

			if (GetRectAndInvertMatrix(ref matrix, ref rect, canvas.width, canvas.height) == true)
			{
				var shapeCoord = default(Vector2);
				var detailX    = P3D_Helper.Reciprocal(canvas.width  * detailScale.x);
				var detailY    = P3D_Helper.Reciprocal(canvas.height * detailScale.y);

				color.a *= opacity;

				for (var x = rect.XMin; x < rect.XMax; x++)
				{
					for (var y = rect.YMin; y < rect.YMax; y++)
					{
						if (IsInsideShape(matrix, x, y, ref shapeCoord) == true)
						{
							var old = canvas.GetPixel(x, y);
							var sub = color;

							if (shape != null) sub *= shape.GetPixelBilinear(shapeCoord.x, shapeCoord.y);

							if (detail != null) sub *= SampleRepeat(detail, detailX * x, detailY * y);
								
							canvas.SetPixel(x, y, SubtractiveBlend(old, sub));
						}
					}
				}
			}
		};
	}
}