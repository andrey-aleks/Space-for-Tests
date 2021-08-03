using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Utilities.Importer;
using Object = System.Object;

namespace Importer
{
    public static class ImporterUtility
    {
        // s/^[a-zA-Z0-9]*\_//
        // s/[_][a-zA-Z0-9]*\.[a-zA-Z0-9]*$
        // s/\.[a-zA-Z0-9]*$//
        private static string NAME = "[ImporterUtility]: ";
        private static readonly ImportSettings Settings = ImportSettings.Instance;

        public static void Import(string sourceFilePath)
        {
            // fbx import
            Regex fbxRegex = new Regex(@"\w*\.fbx", RegexOptions.IgnoreCase);
            var sourceFolderPath = fbxRegex.Replace(sourceFilePath, ""); // get source folder path

            var filename = sourceFilePath.Split('/').Last().Split('.').First(); // get name of fbx

            Regex meshRegex = new Regex(@"mesh_", RegexOptions.IgnoreCase);
            filename = meshRegex.Replace(filename, ""); // remove mesh_ from name
            
            var currentDir = Environment.CurrentDirectory + "\\Assets\\";
            
            // create model's folder under Settings.parentFolder
            if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename))
            {
                AssetDatabase.CreateFolder(Settings.parentFolder.Remove(Settings.parentFolder.Length - 1),
                    filename); // -1 because of redundant "/" at the end
            }

            File.Copy(sourceFilePath, $"{currentDir}Models\\{filename}\\mesh_{filename}.fbx"); // copy fbx
            Debug.Log($"{NAME}fbx imported to folder {Settings.parentFolder}{filename}\\");
            string currentModelPath = $"{Settings.parentFolder}{filename}\\mesh_{filename}.fbx";
            AssetDatabase.ImportAsset(currentModelPath);

            // materials import
            var importedModels = AssetDatabase.LoadAllAssetsAtPath(currentModelPath)
                .Where(x => x.GetType() == typeof(Material));

            if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Materials/"))
            {
                AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Materials");
            }

            foreach (var model in importedModels)
            {
                var message = AssetDatabase.ExtractAsset(model,
                    Settings.parentFolder + filename + @"\Materials\" + model.name + ".mat");
                if (!string.IsNullOrEmpty(message))
                {
                    Debug.Log(NAME + message);
                }
            }

            var mats = AssetDatabase.LoadAllAssetsAtPath($"{Settings.parentFolder}{filename}\\Materials\\");
            
            var materials = mats.ToList(); 
            
            Material material =
                (Material) AssetDatabase.LoadMainAssetAtPath(
                    $"{Settings.parentFolder}{filename}\\Materials\\mat_{filename}.mat");

            // textures import
            
            string texturePath = sourceFolderPath + @"Textures\";
            Regex textureRegex = new Regex(@"tex_\w*\.png", RegexOptions.IgnoreCase);

            if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Textures/"))
            {
                AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Textures");
            }

            foreach (var mat in materials)
            {
                
            }
            foreach (var sourceTexturePath in Directory.GetFiles(texturePath))
            {
                Match textureName = textureRegex.Match(sourceTexturePath);
                string currentTexturePath = currentDir + "\\Models\\" + filename + "\\Textures\\" + textureName;
                File.Copy(sourceTexturePath, currentTexturePath);
                string projectTexturePath = Settings.parentFolder + filename + "\\Textures\\" + textureName;
                AssetDatabase.ImportAsset(projectTexturePath);
                
                // textures setting to material
                switch (textureName.ToString().Split('_').Last().Split('.').First())
                {
                    case "BC":
                        material.SetTexture("_BaseMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        break;
                    case "N":
                        material.SetTexture("_BumpMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        TextureImporter textureImporter =
                            AssetImporter.GetAtPath(projectTexturePath) as TextureImporter;
                        textureImporter.textureType = TextureImporterType.NormalMap;
                        break;
                    case "M":
                        material.SetTexture("_MetallicGlossMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        break;
                    case "AO":
                        material.SetTexture("_OcclusionMap",
                            (Texture) AssetDatabase.LoadMainAssetAtPath(projectTexturePath));
                        break;
                }
            }

            EditorApplication.ExecuteMenuItem("File/Save Project");
        }
    }
}