using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Importer.Editor
{
    public static class ImporterUtility
    {
        private const string NAME = "[ImporterUtility]: ";
        private static Dictionary<string, object> materialsNames = new Dictionary<string, object>();

        private static readonly ImporterSettings Settings = ImporterSettings.Instance;

        public static void Import(string sourceFilePath)
        {
            // inits some regex 
            Regex fbxRegex = new Regex(@"\w*\.fbx", RegexOptions.IgnoreCase);
            Regex meshRegex = new Regex("^(mesh_)", RegexOptions.IgnoreCase);


            // fbx import
            var sourceFolderPath = fbxRegex.Replace(sourceFilePath, ""); // get source folder path

            var filename = Path.GetFileNameWithoutExtension(sourceFilePath); // get name of fbx

            filename = meshRegex.Replace(filename, ""); // remove mesh_ from name


            // create model's folder under Settings.parentFolder
            if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename))
            {
                AssetDatabase.CreateFolder(Settings.parentFolder.Remove(Settings.parentFolder.Length - 1),
                    filename); // -1 because of redundant "/" at the end
            }

            var currentDir = Path.GetFullPath(Settings.parentFolder);

            File.Copy(sourceFilePath, $"{currentDir}{filename}\\mesh_{filename}.fbx",
                Settings.enableOverwrite); // copy fbx to assets

            Debug.Log($"{NAME}fbx {filename} imported to folder {Settings.parentFolder}{filename}\\");
            
            string currentModelPath = $"{Settings.parentFolder}{filename}\\mesh_{filename}.fbx";
            AssetDatabase.ImportAsset(currentModelPath);


            // materials import
            materialsNames.Clear();
//            var importedMaterials = AssetDatabase.LoadAllAssetsAtPath(currentModelPath)
//                .Where(x => x.GetType() == typeof(Material)); // get all materials from fbx
            
            var importedMaterials = AssetDatabase.LoadAllAssetsAtPath(currentModelPath)
                .Where(x => x is Material); // get all materials from fbx
            
            if (importedMaterials.Any())
            {
                // create Materials folder under /Settings.parentFolder/filename/
                if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Materials"))
                {
                    AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Materials");
                }
            }

            foreach (var mat in importedMaterials)
            {
                if (mat.name.Split('_').Last().Equals(Settings.tileMaterialPostfix)) // check tile mat postfix
                {
                    var tileMats = AssetDatabase.FindAssets($"{mat.name} t:Material");
                    if (tileMats.Length > 1) // first is embed mat itself, second will be the existing tile material
                    {
                        var modelImporter =
                            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mat)) as ModelImporter;
                        if (modelImporter != null)
                        {
                            modelImporter.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName,
                                ModelImporterMaterialSearch.Everywhere);
                            Debug.Log(
                                $"{NAME}materials were remapped because of existing tile material. Check if other model's materials were not changed");
                        }
                    }
                    else
                    {
                        ExtractMaterial(mat, filename);
                    }
                }
                else
                {
                    ExtractMaterial(mat, filename);
                }
            }


            // textures import
            var texturePath = sourceFolderPath + @"Textures\";

            foreach (var sourceTexturePath in Directory.GetFiles(texturePath))
            {
                Material material = null;

                // check if textures exists 
                if (Directory.GetFiles(texturePath).Length < 1)
                {
                    Debug.Log($"{NAME}for {filename} there are no textures!");
                    break;
                }

                var fullTextureName = sourceTexturePath.Substring(sourceTexturePath.LastIndexOf('\\'));
                fullTextureName = fullTextureName.Replace("\\", ""); // remove first redundant \


                //Regex formatSubRegex = new Regex("", RegexOptions.IgnoreCase);
                var allowTextureImport = false;
                foreach (var textureFormat in Settings.textureFormats)
                {
                    if (Path.GetExtension(sourceTexturePath).Contains(textureFormat))
                    {
                        //formatSubRegex = new Regex($"{textureFormat}$", RegexOptions.IgnoreCase);
                        allowTextureImport = true;
                        break;
                    }
                }

                /*if (formatSubRegex.ToString() == "")
                {
                    Debug.LogError(
                        $@"{NAME}{fullTextureName} doesn't match available texture formats! It wasn't imported");
                    continue;
                }
                */

                if (!allowTextureImport)
                {
                    Debug.LogError(
                        $@"{NAME}{fullTextureName} doesn't match available texture formats! It wasn't imported");
                    continue;
                }

                // textureName mess
                Regex texSubRegex = new Regex("^(tex_)", RegexOptions.IgnoreCase);

                var textureName = texSubRegex.Replace(fullTextureName, "");

                //textureName = formatSubRegex.Replace(textureName, "");
                textureName = Path.GetFileNameWithoutExtension(textureName);
                var textureNameWithoutExtension = textureName;
                
                var textureType = textureName.Split('_').Last();

                textureName = textureName.Remove(textureName.LastIndexOf('_'));

                if (textureName.Split('_').Last().Equals($@"{Settings.tileMaterialPostfix}"))
                {
                    var tileTexs = AssetDatabase.FindAssets($"{textureNameWithoutExtension} t:Texture2D");
                    if (tileTexs.Any())
                    {
                        continue;
                    }
                }

                // create Textures folder under /Settings.parentFolder/filename/
                if (!AssetDatabase.IsValidFolder(Settings.parentFolder + filename + "/Textures"))
                {
                    AssetDatabase.CreateFolder(Settings.parentFolder + filename, "Textures");
                }

                var targetTexturePath = currentDir + filename + "\\Textures\\" + fullTextureName;
                var assetTexturePath = Settings.parentFolder + filename + "\\Textures\\" + fullTextureName;

                File.Copy(sourceTexturePath, targetTexturePath, Settings.enableOverwrite);
                AssetDatabase.ImportAsset(assetTexturePath);

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
                    Debug.Log(
                        $"{NAME}texture {textureName}_{textureType} doesn't match with the imported materials names!");
                }
                else
                {
                    SetTexture(assetTexturePath, textureType, material); // set textures to material
                }
            }

            AssetDatabase.ImportAsset(currentModelPath); // reimport fbx
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(currentModelPath); // select fbx as active
            EditorGUIUtility.PingObject(Selection.activeObject); // ping (highlight) fbx in assets folder
            AssetDatabase.SaveAssets();
        }

        private static void ExtractMaterial(Object mat, string filename)
        {
            Regex matRegex = new Regex(@"^(mat_)", RegexOptions.IgnoreCase);

            var message = AssetDatabase.ExtractAsset(mat,
                Settings.parentFolder + filename + @"\Materials\" + mat.name +
                ".mat"); // extract materials

            if (!string.IsNullOrEmpty(message))
            {
                Debug.Log(NAME + message); // log if error
            }

            var materialAsset =
                AssetDatabase.LoadAssetAtPath<Material>(Settings.parentFolder + filename + @"\Materials\" +
                                                        mat.name + ".mat");
            materialsNames.Add(matRegex.Replace(mat.name, ""),
                materialAsset); // add mat.name(w/o mat_) and mat to Dictionary 
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
                textureImporter.SaveAndReimport();
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