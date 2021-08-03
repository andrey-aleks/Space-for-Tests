using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
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
            // regex inits
            Regex fbxRegex = new Regex(@"\w*\.fbx", RegexOptions.IgnoreCase);
            Regex meshRegex = new Regex(@"mesh_", RegexOptions.IgnoreCase);
            Regex matRegex = new Regex(@"mat_", RegexOptions.IgnoreCase);
            Regex textureRegex = new Regex(@"tex_\w*\.png", RegexOptions.IgnoreCase);   
            
            
            // fbx import
            var sourceFolderPath = fbxRegex.Replace(sourceFilePath, ""); // get source folder path

            var filename = sourceFilePath.Split('/').Last().Split('.').First(); // get name of fbx
            
            filename = meshRegex.Replace(filename, ""); // remove mesh_ from name

            // create model's folder under Settings.parentFolder
            if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename))
            {
                AssetDatabase.CreateFolder(Settings.parentFolder.Remove(Settings.parentFolder.Length - 1),
                    filename); // -1 because of redundant "/" at the end
            }
            
            // create Materials folder under /Settings.parentFolder/filename/
            if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Materials/"))
            {
                AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Materials");
            }            
            
            var currentDir = Environment.CurrentDirectory + @"\" + Settings.parentFolder;
            Debug.Log("cd: " + currentDir);

            File.Copy(sourceFilePath, $"{currentDir}{filename}\\mesh_{filename}.fbx"); // copy fbx to assets
            Debug.Log($"{NAME}fbx imported to folder {Settings.parentFolder}{filename}\\");
            string currentModelPath = $"{Settings.parentFolder}{filename}\\mesh_{filename}.fbx";
            AssetDatabase.ImportAsset(currentModelPath);

            
            // materials import
            var importedModels = AssetDatabase.LoadAllAssetsAtPath(currentModelPath)
                .Where(x => x.GetType() == typeof(Material)); // get all materials from fbx

            var materialsNames = new Dictionary<string, object>();
            
            foreach (var model in importedModels)
            {
                var message = AssetDatabase.ExtractAsset(model,
                    Settings.parentFolder + filename + @"\Materials\" + model.name + ".mat"); // extract materials
                materialsNames.Add(matRegex.Replace(model.name, ""), model); // add mat.name(w/o mat_) and mat to Dictionary
                if (!string.IsNullOrEmpty(message))
                {
                    Debug.Log(NAME + message); // log if error
                }
            }

            if (materialsNames.Count > 0)
            {
                foreach (var mat in materialsNames)
                {
                    Debug.Log(mat.Key + " ||| " + mat.Value);
                }
            } else Debug.Log("empty");
            
            Material material =
                (Material) AssetDatabase.LoadMainAssetAtPath(
                    $"{Settings.parentFolder}{filename}\\Materials\\mat_{filename}.mat");

            // textures import
            
            var texturePath = sourceFolderPath + @"Textures\";

            foreach (var sourceTexturePath in Directory.GetFiles(texturePath))
            {
                // check if textures exists 
                if (Directory.GetFiles(texturePath).Length < 1)
                {
                    Debug.Log($"{NAME}for {filename} no textures!");
                    return;
                }

                // create Textures folder under /Settings.parentFolder/filename/
                if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Textures/"))
                {
                    AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Textures");
                }
                
                var textureName = textureRegex.Match(sourceTexturePath);
                var currentTexturePath = currentDir + filename + "\\Textures\\" + textureName;
                File.Copy(sourceTexturePath, currentTexturePath);
                var projectTexturePath = Settings.parentFolder + filename + "\\Textures\\" + textureName;
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