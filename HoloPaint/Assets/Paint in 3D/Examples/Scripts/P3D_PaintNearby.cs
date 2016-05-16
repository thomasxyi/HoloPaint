using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PaintNearby))]
public class P3D_PaintNearby_Editor : P3D_Editor<P3D_PaintNearby>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.MaxDistance <= 0.0f));
		{
			DrawDefault("MaxDistance");
		}
		EndError();

		BeginError(Any(t => t.Interval < 0.0f));
		{
			DrawDefault("Interval");
		}
		EndError();

		DrawDefault("NearestOnly");

		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_PaintNearby : MonoBehaviour
{
	[Tooltip("The maximum distance from the current GameObject that can be painted")]
	public float MaxDistance = 1.0f;

	[Tooltip("The amount of seconds between each paint event")]
	public float Interval = 1.0f;

	[Tooltip("Only paint the nearest or all triangles in range?")]
	public bool NearestOnly;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	private float cooldown;

	protected virtual void Update()
	{
		cooldown -= Time.deltaTime;

        if (cooldown <= 0.0f)
		{
			cooldown = Interval;

			for (var i = P3D_Paintable.AllPaintables.Count - 1; i >= 0; i--)
			{
				var paintable = P3D_Paintable.AllPaintables[i];
				var painter   = paintable.GetPainter();

				// Change painter's current brush
				painter.SetBrush(Brush);

				// Paint nearest pixel to the current GameObject's position
				if (NearestOnly == true)
				{
					painter.PaintPerpendicularNearest(transform.position, MaxDistance);
				}
				else
				{
					painter.PaintPerpendicularAll(transform.position, MaxDistance);
				}
            }
        }
    }

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, MaxDistance);
	}
#endif
}
