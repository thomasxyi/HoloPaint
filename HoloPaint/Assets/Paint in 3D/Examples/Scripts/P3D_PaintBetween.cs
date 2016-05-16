using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PaintBetween))]
public class P3D_PaintBetween_Editor : P3D_Editor<P3D_PaintBetween>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Start == null));
		{
			DrawDefault("Start");
		}
		EndError();

		BeginError(Any(t => t.End == null));
		{
			DrawDefault("End");
		}
		EndError();

		DrawDefault("Paint");

		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint onto the target object. Any pixels between the Start and End points will be painted red. It also allows you to control the rotation of the target object.
public class P3D_PaintBetween : MonoBehaviour
{
	public enum NearestOrAll
	{
		Nearest,
		All
	}

	[Tooltip("This transform marks the start point of the painting ray")]
	public Transform Start;

	[Tooltip("This transform marks the end point of the painting ray")]
	public Transform End;

	[Tooltip("Which triangles it should hit")]
	public NearestOrAll Paint;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	// This will paint the target object every frame
	protected virtual void Update()
	{
		if (Start != null && End != null)
		{
			for (var i = P3D_Paintable.AllPaintables.Count - 1; i >= 0; i--)
			{
				var paintable = P3D_Paintable.AllPaintables[i];
				var painter   = paintable.GetPainter();

				// Change painter's current brush
				painter.SetBrush(Brush);

				// Paint between the start and end positions
				switch (Paint)
				{
					case NearestOrAll.Nearest:
					{
						painter.PaintBetweenNearest(Start.position, End.position, P3D_CoordType.UV1);
					}
					break;

					case NearestOrAll.All:
					{
						painter.PaintBetweenAll(Start.position, End.position, P3D_CoordType.UV1);
					}
					break;
				}
			}
		}
	}
}
