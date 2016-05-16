using UnityEngine;
using System.Collections;

// This component will paint
public class P3D_PaintParticleCollisions : MonoBehaviour
{
	// The particle system
	public ParticleSystem Particles;

	// The brush settings for the painting
	public P3D_Brush Brush;

	// Not supported in earlier versions
#if UNITY_5
	// The current collision events array
	private static ParticleCollisionEvent[] collisionEvents;

	protected virtual void OnParticleCollision(GameObject paintTarget)
	{
		// Does the particle system exist?
		if (Particles != null)
		{
			var paintable = paintTarget.GetComponent<P3D_Paintable>();

			if (paintable != null)
			{
				// Get the painter for this paintable
				var painter = paintable.GetPainter();

				// Get the collision events array
				var count = Particles.GetSafeCollisionEventSize();

				if (collisionEvents == null || collisionEvents.Length < count)
				{
					collisionEvents = new ParticleCollisionEvent[count];
				}

				count = Particles.GetCollisionEvents(paintTarget, collisionEvents);

				// Begin painting
				painter.SetBrush(Brush);

				// Go through all collision events
				for (var i = 0; i < count; i++)
				{
					var collisionEvent = collisionEvents[i];

					// Paint between the Start and End points
					painter.PaintNearest(collisionEvent.intersection, 0.01f);
				}
			}
		}
	}
#endif
}
