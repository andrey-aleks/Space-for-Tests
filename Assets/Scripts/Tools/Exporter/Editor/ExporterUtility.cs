using System;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Object = UnityEngine.Object;


namespace Exporter.Editor
{
    public static class ExporterUtility
    {
        private static readonly ExporterSettings Settings = ExporterSettings.Instance;

        private static readonly string NAME = "[ExporterUtility]: "; // name for debug

        /// <summary>
        /// Export fbx from sourcePath to targetPath with dependency textures
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static void Export(string sourcePath, string targetPath)
        {
            Regex pathRegex = new Regex(@"/", RegexOptions.IgnoreCase); // regex for replacing / to \

            sourcePath = pathRegex.Replace(sourcePath, @"\"); // replace / to \ in the source path
            targetPath = pathRegex.Replace(targetPath, @"\"); // replace / to \ in the target path

            var sourceAssetPath =
                sourcePath.Substring(sourcePath.LastIndexOf("\\Assets\\", StringComparison.Ordinal) +
                                     1); // get asset path (starts with Assets\)

            var fileFormat = sourceAssetPath.Split('\\').Last().Split('.').Last(); // get file's format (word after .)

            if (!Settings.exportFileFormats.Contains(fileFormat)) // validate file's format 
            {
                Debug.LogError($@"{NAME}trying to export the wrong object (must be fbx or FBX)");
                return;
            }

            Regex fbxSubRegex = new Regex(@".fbx", RegexOptions.IgnoreCase); // regex for removing ".fbx" part

            var targetFolder =
                Directory.CreateDirectory(fbxSubRegex.Replace(targetPath, "")); // create target folder for file
            var targetName = targetPath.Split('\\').Last().Split('.').First(); // get target file's name

            Regex meshRegex = new Regex(@"^mesh_", RegexOptions.IgnoreCase); // regex for validating mesh_ prefix

            if (meshRegex.Matches(targetName).Count == 0) // validate mesh_ prefix
            {
                targetName = "mesh_" + targetName; // add mesh_ prefix
            }

            File.Copy(sourcePath,
                targetFolder.FullName + @"\" + targetName + ".fbx"); // copy file from sourcePath to targetPath

            var targetTexFolder =
                Directory.CreateDirectory(targetFolder.FullName + @"/Textures"); // create Textures folder

            Regex texRegex = new Regex(@"^tex_", RegexOptions.IgnoreCase); // regex for validating tex_ prefix

            var assetDependencies =
                AssetDatabase.GetDependencies(sourceAssetPath); // get file's dependencies (as button "Select Dependencies")

            foreach (var dep in assetDependencies)
            {
                var textureFormat = dep.Split('.').Last(); // get texture format
                if (Settings.exportTextureFormats.Contains(textureFormat)) // validate texture's format
                {
                    var targetDep = pathRegex.Replace(dep, @"\"); // replace / to \ in the dep path

                    var textureName = targetDep.Split('\\').Last(); // get texture name
                    if (texRegex.Matches(textureName).Count == 0 &&
                        Settings.addTexturePrefix) // validate tex_ prefix & settings (should add prefix or not)
                    {
                        textureName = "tex_" + textureName; // add tex_ prefix
                    }

                    var sourceDepPath =
                        Environment.CurrentDirectory + @"\" + targetDep; // get full source texture path
                    
                    try // try - because of chance of double textures in 1 fbx (>1 mats with the same textures)
                    {
                        File.Copy(sourceDepPath, targetTexFolder.FullName + @"\" + textureName); // try to copy texture 
                    }
                    catch (Exception e)
                    {
                        Debug.Log(NAME + e); // log if error
                        continue;
                    }
                }
            }

            Debug.Log($@"{NAME}{sourcePath.Split('\\').Last()} exported to {targetFolder.FullName}\"); // log if ok
        }

        /// <summary>
        /// Get full path of Unity asset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetFullSourcePath(Object asset)
        {
            return Environment.CurrentDirectory + @"\" + AssetDatabase.GetAssetPath(asset);
        }
    }
}