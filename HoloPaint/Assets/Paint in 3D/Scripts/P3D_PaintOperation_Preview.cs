using UnityEngine;

public static partial class P3D_PaintOperation
{
	public static P3D_Painter.PaintOperation Preview(Mesh mesh, int submeshIndex, Transform transform, Texture2D shape, Vector2 tiling, Vector2 offset)
	{
		return (Texture2D canvas, P3D_Matrix matrix, float opacity) =>
		{
			if (transform != null)
			{
				var paintMatrix      = matrix.Inverse;
				var canvasResolution = new Vector2(canvas.width, canvas.height);

				P3D_BrushPreview.Show(mesh, submeshIndex, transform, opacity, paintMatrix, canvasResolution, shape, tiling, offset);
			}
		};
	}
}