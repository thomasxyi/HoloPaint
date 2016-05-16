using UnityEngine;
using UnityEditor;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(P3D_Paintable))]
public class P3D_Paintable_Editor : P3D_Editor<P3D_Paintable>
{
	protected override void OnInspector()
	{
		if (Any(InvalidRenderer))
		{
			EditorGUILayout.HelpBox("There is no renderer attached to this GameObject", MessageType.Error);
		}

		var indexOob = Any(IndexOob);

		BeginError(indexOob);
		{
			DrawDefault("MaterialIndex");

			if (indexOob == true)
			{
				EditorGUILayout.HelpBox("There is no material at this index", MessageType.Error);
			}
		}
		EndError();

		var nameOob = Any(NameOob);

		BeginError(nameOob);
		{
			DrawDefault("TextureName");

			if (nameOob == true)
			{
				EditorGUILayout.HelpBox("There is no texture slot with this name on the selected material", MessageType.Error);
			}
		}
		EndError();

		BeginError(Any(t => t.ApplyInterval < 0.0f));
		{
			DrawDefault("ApplyInterval");
		}
		EndError();

		DrawDefault("ForceUpdate");

		DrawDefault("DuplicateOnAwake");

		DrawDefault("CreateOnAwake");

		if (Any(t => t.CreateOnAwake == true))
		{
			BeginIndent();
			{
				DrawDefault("CreateColor");

				BeginError(Any(t => t.CreateWidth < 0));
				{
					DrawDefault("CreateWidth");
				}
				EndError();

				BeginError(Any(t => t.CreateHeight < 0));
				{
					DrawDefault("CreateHeight");
				}
				EndError();

				DrawDefault("CreateKeyword");
			}
			EndIndent();
		}
	}

	private bool InvalidRenderer(P3D_Paintable paintable)
	{
		if (paintable.GetComponent<MeshRenderer>() != null && paintable.GetComponent<MeshFilter>() != null)
		{
			return false;
		}

		if (paintable.GetComponent<SkinnedMeshRenderer>() != null)
		{
			return false;
		}

		return true;
	}

	private bool NameOob(P3D_Paintable paintable)
	{
		var renderer = paintable.GetComponent<Renderer>();

		if (renderer != null)
		{
			if (paintable.MaterialIndex >= 0)
			{
				var materials = renderer.sharedMaterials;

				if (paintable.MaterialIndex < materials.Length)
				{
					var material = materials[paintable.MaterialIndex];
					var names    = P3D_Helper.GetTexEnvNames(material);

					if (names.Contains(paintable.TextureName) == true)
					{
						return false;
					}
				}
            }
		}

		return true;
	}

	private bool IndexOob(P3D_Paintable paintable)
	{
		var renderer = paintable.GetComponent<Renderer>();

		if (renderer != null)
		{
			if (paintable.MaterialIndex >= 0 && paintable.MaterialIndex < renderer.sharedMaterials.Length)
			{
				return false;
			}
		}

		return true;
	}
}
