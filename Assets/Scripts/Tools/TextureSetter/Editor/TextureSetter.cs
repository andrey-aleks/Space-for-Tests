using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TextureSetter.Editor
{
    public static class TextureSetter
    {
        private static readonly string NAME = "[TextureSetter]: "; // name for debug

        [MenuItem("Assets/Artist Tools/Set textures to mat")]
        public static void ShowWindow()
        {
            SetTextures();
        }

        private static void SetTextures()
        {
            Material targetMat = null;
            if (Selection.objects.Length != 0)
            {
                foreach (var obj in Selection.objects)
                {
                    if (obj is Material)
                    {
                        targetMat = (Material) obj;
                    }
                }

                if (targetMat == null)
                {
                    Debug.Log(NAME + "Material not found");
                    return;
                }

                foreach (var obj in Selection.objects)
                {
                    if (obj is Texture2D)
                    {
                        switch (obj.name.Split('_').Last())
                        {
                            case "BC":
                                targetMat.SetTexture("_BaseMap", (Texture) obj);
                                Debug.Log(NAME + obj.name + " was set to " + targetMat.name);
                                break;
                            case "N":
                                targetMat.SetTexture("_BumpMap", (Texture) obj);
                                Debug.Log(NAME + obj.name + " was set to " + targetMat.name);
                                break;
                            case "MS":
                                targetMat.SetTexture("_MetallicGlossMap", (Texture) obj);
                                Debug.Log(NAME + obj.name + " was set to " + targetMat.name);
                                break;
                            case "AO":
                                targetMat.SetTexture("_OcclusionMap", (Texture) obj);
                                Debug.Log(NAME + obj.name + " was set to " + targetMat.name);
                                break;
                        }
                    }
                }

                AssetDatabase.SaveAssets();
            }
            else Debug.Log(NAME + " no active objects");
        }
    }
}