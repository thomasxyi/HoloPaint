using UnityEngine;

public class P3D_DestroyAfterTime : MonoBehaviour
{
	public float Life = 1.0f;
	
	protected virtual void Update()
	{
		Life -= Time.deltaTime;
		
		if (Life <= 0.0f)
		{
			Destroy(gameObject);
		}
	}
}