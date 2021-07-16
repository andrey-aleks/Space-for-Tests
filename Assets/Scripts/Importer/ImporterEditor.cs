using System;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utilities.Importer
{
    public class ImporterEditor : EditorWindow
    {
        [MenuItem("Assets/-- Custom Import")]
        public static void Show()
        {
            Init();
        }

        private static void Init()
        {
            // ImportSettings.Instance.path = EditorUtility.OpenFilePanel("Open File", "", "");
            string path = EditorUtility.OpenFilePanel("Open File", "", "");
            string pattern = @"\w*\.fbx";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            string commonPath = regex.Replace(path, "");
            string filename = path.Split('/').Last().Split('.').First().Split('_')[1];
            //Debug.Log("filename is " + filename);
            string currentDir = Environment.CurrentDirectory + "\\Assets\\";

            if (!AssetDatabase.IsValidFolder("Assets/Models/" + filename))
            {
                AssetDatabase.CreateFolder("Assets/Models", filename);
            }

            File.Copy(path, currentDir + "Models\\" + filename + "\\" + "mesh_" + filename.Split('_').First() + ".fbx");
            string currentModelPath = "Assets\\Models\\" + filename + "\\" + "mesh_" + filename + ".fbx";
            AssetDatabase.ImportAsset(currentModelPath);

/*            var importer = AssetImporter.GetAtPath(currentModelPath) as ModelImporter;
            var success = importer.ExtractTextures("Assets/Textures/");
            if (success) Debug.Log("success");
            */
            var importedModels = AssetDatabase.LoadAllAssetsAtPath(currentModelPath)
                .Where(x => x.GetType() == typeof(Material));
            if (!AssetDatabase.IsValidFolder("Assets/Models/" + filename + "/Materials/"))
            {
                AssetDatabase.CreateFolder("Assets/Models/" + filename, "Materials");
            }

            foreach (var model in importedModels)
            {
                Debug.Log(model.name);
                var message = AssetDatabase.ExtractAsset(model,
                    "Assets/Models/" + filename + "/Materials/" + model.name + ".mat");
                if (!string.IsNullOrEmpty(message))
                {
                    Debug.Log(message);
                }
            }

            //"Custom/SimpleShader"
            //Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            //AssetDatabase.CreateAsset(material, "Assets\\Materials\\" + "mat_" + filename + ".mat");
            Material material =
                (Material) AssetDatabase.LoadMainAssetAtPath("Assets\\Models\\" + filename + "\\Materials\\" + "mat_" +
                                                             filename + ".mat");
            string texturePath = commonPath + "Textures/";
            pattern = @"tex_\w*\.png";
            Regex textureRegex = new Regex(pattern, RegexOptions.IgnoreCase);

            if (!AssetDatabase.IsValidFolder("Assets/Models/" + filename + "/Textures/"))
            {
                AssetDatabase.CreateFolder("Assets/Models/" + filename, "Textures");
            }

            Debug.Log("mat is " + material.name);

            foreach (var sourceTexturePath in Directory.GetFiles(texturePath))
            {
                Match textureName = textureRegex.Match(sourceTexturePath);
                Debug.Log(textureName);
                string currentTexturePath = currentDir + "\\Models\\" + filename + "\\Textures\\" + textureName;
                File.Copy(sourceTexturePath, currentTexturePath);
                string projectTexturePath = "Assets\\Models\\" + filename + "\\Textures\\" + textureName;
                AssetDatabase.ImportAsset(projectTexturePath);
                Debug.Log("split is " + textureName.ToString().Split('_').Last().Split('.').First());
                switch (textureName.ToString().Split('_').Last().Split('.').First())
                {
                    case "BC":
                        material.SetTexture("_BaseMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        //Debug.Log(nameForDebug + obj.name + " is set to " + material.name);
                        break;
                    case "N":
                        material.SetTexture("_BumpMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        TextureImporter textureImporter = AssetImporter.GetAtPath(projectTexturePath) as TextureImporter;
                        textureImporter.textureType = TextureImporterType.NormalMap;
                        //Debug.Log(nameForDebug + obj.name + " is set to " + material.name);
                        break;
                    case "M":
                        material.SetTexture("_MetallicGlossMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        //Debug.Log(nameForDebug + obj.name + " is set to " + material.name);
                        break;
                    case "AO":
                        material.SetTexture("_OcclusionMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        //Debug.Log(nameForDebug + obj.name + " is set to " + material.name);
                        break;
                }
            }
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }
    }
}