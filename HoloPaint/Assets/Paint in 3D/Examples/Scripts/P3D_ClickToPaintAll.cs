using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_ClickToPaintAll))]
public class P3D_ClickToPaintAll_Editor : P3D_Editor<P3D_ClickToPaintAll>
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
public class P3D_ClickToPaintAll : MonoBehaviour
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
				var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

				P3D_Paintable.PaintBetweenNearest(ray.GetPoint(mainCamera.nearClipPlane), ray.GetPoint(mainCamera.farClipPlane), Brush, RaycastMask);
			}
		}
	}
}
