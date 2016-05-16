using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_ClickToPaintSubstep))]
public class P3D_ClickToPaintSubstep_Editor : P3D_Editor<P3D_ClickToPaintSubstep>
{
	protected override void OnInspector()
	{
		DrawDefault("Requires");

		DrawDefault("RaycastMask");

		DrawDefault("StepSize");

		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_ClickToPaintSubstep : MonoBehaviour
{
	[Tooltip("The key that must be held down to mouse look")]
	public KeyCode Requires = KeyCode.Mouse0;

	[Tooltip("The layer mask used when raycasting into the scene")]
	public LayerMask RaycastMask = -1;

	[Tooltip("The maximum amount of pixels between ")]
	public float StepSize = 1.0f;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	private Camera mainCamera;

	private Vector2 oldMousePosition;

	// Called every frame
	protected virtual void Update()
	{
		if (mainCamera == null) mainCamera = Camera.main;

		if (mainCamera != null && StepSize > 0.0f)
		{
			// The required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				oldMousePosition = Input.mousePosition;
            }

			// The required key is set?
			if (Input.GetKey(Requires) == true)
			{
				// Find the ray for this screen position
				var newMousePosition = (Vector2)Input.mousePosition;
				var stepCount        = Vector2.Distance(oldMousePosition, newMousePosition) / StepSize + 1;

				for (var i = 0; i < stepCount; i++)
				{
					var subMousePosition = Vector2.Lerp(oldMousePosition, newMousePosition, i / stepCount);
					var ray              = mainCamera.ScreenPointToRay(subMousePosition);
					var hit              = default(RaycastHit);

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

				oldMousePosition = newMousePosition;
			}
		}
	}
}
