using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Importer.Editor
{
    public static class ImporterUtility
    {
        private const string NAME = "[ImporterUtility]: ";
        private static readonly ImporterSettings Settings = ImporterSettings.Instance;

        public static void Import(string sourceFilePath)
        {
            // regex some inits
            Regex fbxRegex = new Regex(@"\w*\.fbx", RegexOptions.IgnoreCase);
            Regex meshRegex = new Regex(@"mesh_", RegexOptions.IgnoreCase);
            Regex matRegex = new Regex(@"mat_", RegexOptions.IgnoreCase);


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
            if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Materials"))
            {
                AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Materials");
            }

            var currentDir = Environment.CurrentDirectory + @"\" + Settings.parentFolder;

            File.Copy(sourceFilePath, $"{currentDir}{filename}\\mesh_{filename}.fbx",
                Settings.enableOverwrite); // copy fbx to assets

            Debug.Log($"{NAME}fbx {filename} imported to folder {Settings.parentFolder}{filename}\\");
            string currentModelPath = $"{Settings.parentFolder}{filename}\\mesh_{filename}.fbx";
            AssetDatabase.ImportAsset(currentModelPath);


            // materials import
            var importedModels = AssetDatabase.LoadAllAssetsAtPath(currentModelPath)
                .Where(x => x.GetType() == typeof(Material)); // get all materials from fbx

            var materialsNames = new Dictionary<string, object>();

            foreach (var model in importedModels)
            {
                if (model.name.Split('_').Last().Equals(Settings.tileMaterialPostfix))
                {
                    // не импортит обычные не тайловые маты
                    var tileMats = AssetDatabase.FindAssets(model.name);
                    if (tileMats.Length > 0)
                    {
                        var modelImporter =
                            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(model)) as ModelImporter;
                        if (modelImporter != null)
                        {
                            modelImporter.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName,
                                ModelImporterMaterialSearch.Everywhere);
                        }
                    }
                    else
                    {
                        var message = AssetDatabase.ExtractAsset(model,
                            Settings.parentFolder + filename + @"\Materials\" + model.name +
                            ".mat"); // extract materials

                        if (!string.IsNullOrEmpty(message))
                        {
                            Debug.Log(NAME + message); // log if error
                        }
                    }
                }
                else
                {
                    var message = AssetDatabase.ExtractAsset(model,
                        Settings.parentFolder + filename + @"\Materials\" + model.name +
                        ".mat"); // extract materials

                    if (!string.IsNullOrEmpty(message))
                    {
                        Debug.Log(NAME + message); // log if error
                    }
                }

                var materialAsset =
                    AssetDatabase.LoadAssetAtPath<Material>(Settings.parentFolder + filename + @"\Materials\" +
                                                            model.name + ".mat");
                materialsNames.Add(matRegex.Replace(model.name, ""),
                    materialAsset); // add mat.name(w/o mat_) and mat to Dictionary
            }


            // textures import
            var texturePath = sourceFolderPath + @"Textures\";
            Material material = null;

            foreach (var sourceTexturePath in Directory.GetFiles(texturePath))
            {
                // check if textures exists 
                if (Directory.GetFiles(texturePath).Length < 1)
                {
                    Debug.Log($"{NAME}for {filename} there are no textures!");
                    return;
                }


                // create Textures folder under /Settings.parentFolder/filename/
                if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Textures"))
                {
                    AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Textures");
                }

                var fullTextureName = sourceTexturePath.Substring(sourceTexturePath.LastIndexOf('\\'));
                fullTextureName = fullTextureName.Replace("\\", ""); // remove first redundant \
                var tileTexs = AssetDatabase.FindAssets(fullTextureName);
                foreach (var tileTex in tileTexs)
                {
                    Debug.Log("tile tex " + tileTex);
                }

                var currentTexturePath = currentDir + filename + "\\Textures\\" + fullTextureName;
                var projectTexturePath = Settings.parentFolder + filename + "\\Textures\\" + fullTextureName;
                Regex formatSubRegex = new Regex("", RegexOptions.IgnoreCase);

                foreach (var textureFormat in Settings.textureFormats)
                {
                    if (sourceTexturePath.Substring(sourceTexturePath.LastIndexOf('.')).Contains(textureFormat))
                    {
                        formatSubRegex = new Regex(textureFormat, RegexOptions.IgnoreCase);
                        File.Copy(sourceTexturePath, currentTexturePath, Settings.enableOverwrite);
                    }
                }

                if (formatSubRegex.ToString() == "")
                {
                    Debug.LogError(
                        $@"{NAME}{fullTextureName} doesn't match available texture formats! It wasn't imported");
                    continue;
                }

                AssetDatabase.ImportAsset(projectTexturePath);

                // textureName mess
                Regex texSubRegex = new Regex(@"tex_", RegexOptions.IgnoreCase);

                var textureName = texSubRegex.Replace(fullTextureName, "");
                textureName = formatSubRegex.Replace(textureName, "");
                var textureType = textureName.Split('_').Last();

                Regex typeSubRegex = new Regex(textureType, RegexOptions.IgnoreCase);
                textureName = typeSubRegex.Replace(textureName, "");
                textureName = textureName.Remove(textureName.Length - 1);

                foreach (var mat in materialsNames)
                {
                    if (textureName.Equals(mat.Key))
                    {
                        material = (Material) mat.Value;
                        break;
                    }
                }

                if (material == null)
                {
                    Debug.Log($"{NAME} texture {textureName}_{textureType} doesn't matches with materials names!");
                    continue;
                }
                else
                {
                    SetTexture(projectTexturePath, textureType, material); // textures setting to material
                }
            }

            AssetDatabase.ImportAsset(currentModelPath);
            AssetDatabase.SaveAssets();
        }

        private static void SetTexture(string texturePath, string textureType, Material targetMaterial)
        {
            if (textureType.Equals(Settings.baseColorPostfix))
            {
                targetMaterial.SetTexture(Settings.baseColorMapName,
                    (Texture) AssetDatabase.LoadMainAssetAtPath(texturePath));
                return;
            }

            if (textureType.Equals(Settings.normalPostfix))
            {
                targetMaterial.SetTexture(Settings.normalMapName,
                    (Texture) AssetDatabase.LoadMainAssetAtPath(texturePath));
                var textureImporter =
                    AssetImporter.GetAtPath(texturePath) as TextureImporter;
                textureImporter.textureType = TextureImporterType.NormalMap;
                return;
            }

            if (textureType.Equals(Settings.metallicPostfix))
            {
                targetMaterial.SetTexture(Settings.metallicMapName,
                    (Texture) AssetDatabase.LoadMainAssetAtPath(texturePath));
                return;
            }

            if (textureType.Equals(Settings.occlusionPostfix))
            {
                targetMaterial.SetTexture(Settings.occlusionMapName,
                    (Texture) AssetDatabase.LoadMainAssetAtPath(texturePath));
            }
        }
    }
}