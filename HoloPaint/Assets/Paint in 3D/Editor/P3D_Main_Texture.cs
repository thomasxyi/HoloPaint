 using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private int newWidth = 256;

	[SerializeField]
	private int newHeight = 256;

	[SerializeField]
	private bool showTexture = true;

	private void DrawTexture()
	{
		if (currentMaterial != null && texEnvNames.Length > 0)
		{
			EditorGUILayout.Separator();

			BeginGroup(ref showTexture, "Texture"); if (showTexture == true)
			{
				currentTexEnvIndex = EditorGUILayout.Popup(currentTexEnvIndex, texEnvNames);
				currentTexEnvName  = P3D_Helper.GetIndexOrDefault(texEnvNames, currentTexEnvIndex);
				
				UpdateState();

				EditorGUI.BeginDisabledGroup(true);
				{
					EditorGUI.ObjectField(P3D_Helper.Reserve(), "", currentTexture, typeof(Texture2D), false);
				}
				EditorGUI.EndDisabledGroup();

				if (currentTexture == null)
				{
					DrawCreateTexture();
				}
				else
				{
					if (currentTexture.hideFlags == HideFlags.None)
					{
						if (P3D_Helper.IsWritableFormat(currentTexture.format) == true)
						{
							if (P3D_Helper.IsAsset(currentTexture) == true)
							{
								var path = AssetDatabase.GetAssetPath(currentTexture);

								// Directly writable Texture2D?
								if (path.EndsWith(".asset") == true)
								{
									DrawDuplicateTexture();
								}
								// png/psd/etc?
								else
								{
									DrawDuplicateTexture("This texture asset isn't directly writable, duplicate it to convert it to a writable format");
								}
							}
							else
							{
								DrawDuplicateTexture();

								// Both in scene?
								if (P3D_Helper.IsAsset(currentMaterial) == false)
								{
									DrawSaveTexture();
								}
								// Texture in scene, but material in asset?
								else
								{
									DrawSaveTexture("This texture isn't an asset, but it belongs to a material which is an asset, so you must save the texture to prevent data loss");
								}
							}

							DrawExportTexture();
						}
						else
						{
							DrawDuplicateTexture("This texture's format (" + currentTexture.format + ") isn't directly writable, duplicate it to convert it to a writable format");
						}
					}
					// Bad hide flags?
					else
					{
						DrawDuplicateTexture("This texture's hideFlags indicate it shouldn't be modified, duplicate it to fix this");
					}
				}
			}
			EndGroup();
		}
	}

	private void DrawCreateTexture()
	{
		BeginError();
		{
			EditorGUILayout.HelpBox("There is no texture in this texture slot", MessageType.Error);

			BeginColor(Color.green);
			{
				var rect1 = P3D_Helper.Reserve();
				var rect2 = P3D_Helper.SplitHorizontal(ref rect1, 2);
				var rect3 = P3D_Helper.Reserve(16.0f, true);

				newWidth  = P3D_Helper.DrawSize(rect1, newWidth );
				newHeight = P3D_Helper.DrawSize(rect2, newHeight);

				if (GUI.Button(rect3, "Create") == true)
				{
					var menu = new GenericMenu();

					menu.AddItem(new GUIContent("Transparent"), false, () => { CreateTexture(P3D_Helper.GetColor(P3D_Helper.BaseColors.Transparent)); if (currentBrush.Blend == P3D_BlendMode.NormalBlend) currentBrush.Blend = P3D_BlendMode.AlphaBlend; });
					menu.AddItem(new GUIContent("Black")      , false, () => { CreateTexture(P3D_Helper.GetColor(P3D_Helper.BaseColors.Black      )); if (currentBrush.Blend == P3D_BlendMode.NormalBlend) currentBrush.Blend = P3D_BlendMode.AlphaBlend; });
					menu.AddItem(new GUIContent("White")      , false, () => { CreateTexture(P3D_Helper.GetColor(P3D_Helper.BaseColors.White      )); if (currentBrush.Blend == P3D_BlendMode.NormalBlend) currentBrush.Blend = P3D_BlendMode.AlphaBlend; });
					menu.AddItem(new GUIContent("Normal")     , false, () => { CreateTexture(P3D_Helper.GetColor(P3D_Helper.BaseColors.Normal     )); currentBrush.Blend = P3D_BlendMode.NormalBlend; });

					menu.DropDown(rect3);
				}
			}
			EndColor();
		}
		EndError();
	}

	private void CreateTexture(Color newColor)
	{
		currentTexture = new Texture2D(newWidth, newHeight);
		currentTexture.name = "New Texture";

		ClearTexture(newColor, false);

		ClearUndo();

		currentMaterial.SetTexture(currentTexEnvName, currentTexture);

		P3D_Helper.SetDirty(this);
	}

	private void DrawDuplicateTexture(string errorMessage = null)
	{
		var showError = string.IsNullOrEmpty(errorMessage) == false;

		BeginError(showError);
		{
			if (showError == true)
			{
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}

			BeginColor(showError == true ? Color.green : GUI.color);
			{
				if (Button("Duplicate") == true)
				{
					P3D_Helper.MakeTextureReadable(currentTexture);

					var pixels     = currentTexture.GetPixels32();
					var newTexture = new Texture2D(currentTexture.width, currentTexture.height, GetClosestFormat(currentTexture.format), currentTexture.mipmapCount > 1);

					newTexture.name = "New Texture";
					newTexture.SetPixels32(pixels);
					newTexture.Apply();

					ClearUndo();

					currentTexture = newTexture;

					currentMaterial.SetTexture(currentTexEnvName, newTexture);

					P3D_Helper.SetDirty(this);
				}
			}
			EndColor();
		}
		EndError();
	}

	private void DrawSaveTexture(string errorMessage = null)
	{
		var showError = string.IsNullOrEmpty(errorMessage) == false;

		BeginError(showError);
		{
			if (showError == true)
			{
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}

			BeginColor(showError == true ? Color.green : GUI.color);
			{
				if (Button("Save") == true)
				{
					var path = P3D_Helper.SaveDialog("Save Texture", "Assets", currentTexture.name, "asset");

					if (string.IsNullOrEmpty(path) == false)
					{
						AssetDatabase.CreateAsset(currentTexture, path);
					}
				}
			}
			EndColor();
		}
		EndError();
	}

	private void DrawExportTexture()
	{
		if (Button("Export") == true)
		{
			var path = P3D_Helper.SaveDialog("Export Texture", "Assets", currentTexture.name, "png");

			if (string.IsNullOrEmpty(path) == false)
			{
				P3D_Helper.SaveTextureAsset(currentTexture, path, true);

				var newTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

				if (newTexture != null)
				{
					ClearUndo();

					currentTexture = newTexture;
					currentMaterial.SetTexture(currentTexEnvName, newTexture);

					P3D_Helper.SetDirty(this);
				}
			}
		}
	}

	private TextureFormat GetClosestFormat(TextureFormat format)
	{
		switch (format)
		{
			case TextureFormat.Alpha8:         return TextureFormat.Alpha8;
			case TextureFormat.EAC_R:          return TextureFormat.Alpha8;
			case TextureFormat.EAC_R_SIGNED:   return TextureFormat.Alpha8;
			case TextureFormat.EAC_RG:         return TextureFormat.RGB24;
			case TextureFormat.EAC_RG_SIGNED:  return TextureFormat.RGB24;
			case TextureFormat.RGB565:         return TextureFormat.RGB24;
			case TextureFormat.DXT1:           return TextureFormat.RGB24;
			case TextureFormat.PVRTC_RGB2:     return TextureFormat.RGB24;
			case TextureFormat.PVRTC_RGB4:     return TextureFormat.RGB24;
			case TextureFormat.ETC_RGB4:       return TextureFormat.RGB24;
			case TextureFormat.ATC_RGB4:       return TextureFormat.RGB24;
			//case TextureFormat.ATF_RGB_DXT1:   return TextureFormat.RGB24;
			//case TextureFormat.ATF_RGB_JPG:    return TextureFormat.RGB24;
			case TextureFormat.ETC2_RGB:       return TextureFormat.RGB24;
			case TextureFormat.ASTC_RGB_4x4:   return TextureFormat.RGB24;
			case TextureFormat.ASTC_RGB_5x5:   return TextureFormat.RGB24;
			case TextureFormat.ASTC_RGB_6x6:   return TextureFormat.RGB24;
			case TextureFormat.ASTC_RGB_8x8:   return TextureFormat.RGB24;
			case TextureFormat.ASTC_RGB_10x10: return TextureFormat.RGB24;
			case TextureFormat.ASTC_RGB_12x12: return TextureFormat.RGB24;
		}

		return TextureFormat.ARGB32;
	}
}
