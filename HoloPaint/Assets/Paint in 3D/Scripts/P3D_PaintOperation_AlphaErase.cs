using UnityEngine;

public static partial class P3D_PaintOperation
{
	public static P3D_Painter.PaintOperation AlphaErase(Texture2D shape, Texture2D detail, Vector2 detailScale)
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
				
				for (var x = rect.XMin; x < rect.XMax; x++)
				{
					for (var y = rect.YMin; y < rect.YMax; y++)
					{
						if (IsInsideShape(matrix, x, y, ref shapeCoord) == true)
						{
							var old = canvas.GetPixel(x, y);
							var opa = opacity;

							if (shape != null) opa *= shape.GetPixelBilinear(shapeCoord.x, shapeCoord.y).a;

							if (detail != null) opa *= SampleRepeat(detail, detailX * x, detailY * y).a;
								
							canvas.SetPixel(x, y, AlphaErase(old, opa));
						}
					}
				}
			}
		};
	}
}