using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Exporter.Editor
{
    public class ExporterUtility : MonoBehaviour
    {
        private static readonly ExporterSettings Settings = ExporterSettings.Instance;
        
        private static string NAME = "[ExporterUtility]: ";
        
        public static void Export(string sourcePath, string targetPath)
        {
            Regex pathRegex = new Regex(@"/", RegexOptions.IgnoreCase);

            sourcePath = pathRegex.Replace(sourcePath, @"\");
            targetPath = pathRegex.Replace(targetPath, @"\");

            var sourceAssetPath = sourcePath.Substring(sourcePath.LastIndexOf("\\Assets\\", StringComparison.Ordinal) + 1);

            var fileFormat = sourceAssetPath.Split('\\').Last().Split('.').Last();

            if (!Settings.exportFileFormats.Contains(fileFormat))
            {
                Debug.LogError($@"{NAME}trying to export the wrong object (must be fbx or FBX)");
                return;
            }

            var assetDependencies = AssetDatabase.GetDependencies(sourceAssetPath);

            Regex fbxSubRegex = new Regex(@".fbx", RegexOptions.IgnoreCase);

            var targetFolder = Directory.CreateDirectory(fbxSubRegex.Replace(targetPath, ""));
            var targetName = targetPath.Split('\\').Last().Split('.').First();
            
            Regex meshRegex = new Regex(@"^mesh_", RegexOptions.IgnoreCase);
            
            if (meshRegex.Matches(targetName).Count == 0)
            {
                targetName = "mesh_" + targetName;
            }

            File.Copy(sourcePath, targetFolder.FullName + @"\" + targetName + ".fbx");
            var targetTexFolder = Directory.CreateDirectory(targetFolder.FullName + @"/Textures");
            
            Regex texRegex = new Regex(@"^tex_", RegexOptions.IgnoreCase);

            foreach (var dep in assetDependencies)
            {
                var textureFormat = dep.Split('.').Last();
                if (Settings.exportTextureFormats.Contains(textureFormat))
                {
                    var targetDep = pathRegex.Replace(dep, @"\");

                    var textureName = targetDep.Split('\\').Last();
                    if (texRegex.Matches(textureName).Count == 0 && Settings.addTexturePrefix)
                    {
                        textureName = "tex_" + textureName;
                    }

                    var sourceDepPath = Environment.CurrentDirectory + @"\" + targetDep;
                    try
                    {
                        File.Copy(sourceDepPath, targetTexFolder.FullName + @"\" + textureName);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(NAME + e);
                        continue;
                    }
                }
            }

            Debug.Log($@"{NAME}{sourcePath.Split('\\').Last()} successfully exported to {targetFolder.FullName}\");
        }
    }
}