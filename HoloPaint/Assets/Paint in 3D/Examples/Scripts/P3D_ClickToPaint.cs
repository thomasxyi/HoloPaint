using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_ClickToPaint))]
public class P3D_ClickToPaint_Editor : P3D_Editor<P3D_ClickToPaint>
{
	protected override void OnInspector()
	{
		DrawDefault("Requires");

		DrawDefault("RaycastMask");

		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_ClickToPaint : MonoBehaviour
{
	[Tooltip("The key that must be held down to mouse look")]
	public KeyCode Requires = KeyCode.Mouse0;

	[Tooltip("The layer mask used when raycasting into the scene")]
	public LayerMask RaycastMask = -1;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	private Camera mainCamera;

	// Called every frame
	protected virtual void Update()
	{
		if (mainCamera == null) mainCamera = Camera.main;

		if (mainCamera != null)
		{
			// The required key is down?
			if (Input.GetKey(Requires) == true)
			{
				// Find the ray for this screen position
				var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
				var hit = default(RaycastHit);

				// Raycast into the 3D scene
				if (Physics.Raycast(ray, out hit, float.PositiveInfinity, RaycastMask) == true)
				{
					// See if the object the raycast hit is paintable
					var paintable = hit.collider.GetComponent<P3D_Paintable>();

					if (paintable != null)
					{
						// Get painter for this paintable
						var painter = paintable.GetPainter();

						// Change painter's current brush
						painter.SetBrush(Brush);

						// Paint at the hit coordinate
						painter.Paint(hit.textureCoord);
					}
				}
			}
		}
	}
}
