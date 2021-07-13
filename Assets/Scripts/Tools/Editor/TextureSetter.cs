using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TextureSetter
    {
        public static string nameForDebug = "[TextureSetter]: ";
        
        [MenuItem("Assets/Set textures")]
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
                    if (obj.name.Split('_').First().Equals("mat"))
                    {
                        targetMat = (Material) obj;
                    }
                }

                if (targetMat == null)
                {
                    Debug.Log(nameForDebug + "Material not found");
                    return;                
                }
                
                foreach (var obj in Selection.objects)
                {
                    if (obj.name.Split('_').First().Equals("tex"))
                    {
                        switch (obj.name.Split('_').Last())
                        {
                            case "BC":
                                targetMat.SetTexture("_BaseMap", (Texture)obj);
                                Debug.Log(nameForDebug + obj.name + " is set to " + targetMat.name);
                                break;
                            case "N":
                                targetMat.SetTexture("_BumpMap", (Texture)obj);
                                Debug.Log(nameForDebug + obj.name + " is set to " + targetMat.name);
                                break;
                            case "M":
                                targetMat.SetTexture("_MetallicGlossMap", (Texture)obj);
                                Debug.Log(nameForDebug + obj.name + " is set to " + targetMat.name);
                                break;
                            case "AO":
                                targetMat.SetTexture("_OcclusionMap", (Texture)obj);
                                Debug.Log(nameForDebug + obj.name + " is set to " + targetMat.name);
                                break;
                        }
                        EditorApplication.ExecuteMenuItem("File/Save Project");
                    } 
                }
            }
            else Debug.Log(nameForDebug + " no active objects");
        }
    }
}
