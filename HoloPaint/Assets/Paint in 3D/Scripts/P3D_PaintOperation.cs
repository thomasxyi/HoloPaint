using UnityEngine;

// This class contains the basic code for painting operations, the other partial classes contain the optimized code for each specific blend type
public static partial class P3D_PaintOperation
{
	private static Color AlphaBlend(Color old, Color add)
	{
		if (add.a > 0.0f)
		{
			var add_a = add.a;
			var add_i = 1.0f - add_a;
			var old_a = old.a;
			var old_n = add_a + old_a * add_i;
			
			old.r = (add.r * add_a + old.r * old_a * add_i) / old_n;
			old.g = (add.g * add_a + old.g * old_a * add_i) / old_n;
			old.b = (add.b * add_a + old.b * old_a * add_i) / old_n;
			old.a = old_n;
		}
		
		return old;
	}
	
	
	private static Color AlphaBlendRgb(Color old, Color add)
	{
		if (old.a > 0.0f && add.a > 0.0f)
		{
			var add_a = add.a;
			var add_i = 1.0f - add_a;
			var old_a = old.a;
			var old_n = add_a + old_a * add_i;
			
			old.r = (add.r * add_a + old.r * old_a * add_i) / old_n;
			old.g = (add.g * add_a + old.g * old_a * add_i) / old_n;
			old.b = (add.b * add_a + old.b * old_a * add_i) / old_n;
		}
		
		return old;
	}
	
	private static Color AlphaErase(Color old, float sub)
	{
		old.a -= sub;
		
		return old;
	}
	
	private static Color AdditiveBlend(Color old, Color add)
	{
		old.r += add.r;
		old.g += add.g;
		old.b += add.b;
		old.a += add.a;
		
		return old;
	}
	
	private static Color SubtractiveBlend(Color old, Color sub)
	{
		old.r -= sub.r;
		old.g -= sub.g;
		old.b -= sub.b;
		old.a -= sub.a;
		
		return old;
	}
	
	private static Color NormalBlend(Color old, Color add, float a)
	{
		var i = 1.0f - a;
		
		old.g = old.g * i + add.g * a;
		old.a = old.a * i + add.a * a;
		
		return old;
	}
	
	private static Color NormalErase(Color old, float sub)
	{
		return NormalBlend(old, new Color(0.5f, 0.5f, 1.0f, 0.5f), sub);
	}
	
	private static Color DirectionToColor(Vector2 direction)
	{
		var color = default(Color);
		
		color.r = direction.x * 0.5f + 0.5f;
		color.g = direction.y * 0.5f + 0.5f;
		color.b = Mathf.Sqrt(1.0f - direction.x * direction.x + direction.y * direction.y) * 0.5f + 0.5f;
		color.a = color.r;
		
		return color;
	}

	private static bool GetRectAndInvertMatrix(ref P3D_Matrix matrix, ref P3D_Rect rect, int canvasW, int canvasH)
	{
		// Grab transformed corners
		var a = matrix.MultiplyPoint(0.0f, 0.0f);
		var b = matrix.MultiplyPoint(1.0f, 0.0f);
		var c = matrix.MultiplyPoint(0.0f, 1.0f);
		var d = matrix.MultiplyPoint(1.0f, 1.0f);

		// Find min/max x/y
		var xMin = Mathf.Min(Mathf.Min(a.x, b.x), Mathf.Min(c.x, d.x));
		var xMax = Mathf.Max(Mathf.Max(a.x, b.x), Mathf.Max(c.x, d.x));
		var yMin = Mathf.Min(Mathf.Min(a.y, b.y), Mathf.Min(c.y, d.y));
		var yMax = Mathf.Max(Mathf.Max(a.y, b.y), Mathf.Max(c.y, d.y));
		
		// Has volume?
		if (xMin < xMax && yMin < yMax)
		{
			// Make sure rect doesn't go outside canvas
			rect.XMin = Mathf.Clamp(Mathf.FloorToInt(xMin), 0, canvasW);
			rect.XMax = Mathf.Clamp(Mathf. CeilToInt(xMax), 0, canvasW);
			rect.YMin = Mathf.Clamp(Mathf.FloorToInt(yMin), 0, canvasH);
			rect.YMax = Mathf.Clamp(Mathf. CeilToInt(yMax), 0, canvasH);

			matrix = matrix.Inverse;

			return true;
		}

		return false;
	}

	private static bool IsInsideShape(P3D_Matrix inverseMatrix, int x, int y, ref Vector2 shapeCoord)
	{
		shapeCoord = inverseMatrix.MultiplyPoint(x, y);

		if (shapeCoord.x >= 0.0f && shapeCoord.x < 1.0f)
		{
			if (shapeCoord.y >= 0.0f && shapeCoord.y < 1.0f)
			{
				return true;
			}
		}

		return false;
	}

	private static Color SampleRepeat(Texture2D texture, float u, float v)
	{
		return texture.GetPixelBilinear(u % 1.0f, v % 1.0f);
	}
}