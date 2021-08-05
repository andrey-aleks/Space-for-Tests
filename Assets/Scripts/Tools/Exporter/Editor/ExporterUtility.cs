using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Object = UnityEngine.Object;

namespace Exporter.Editor
{
    public class ExporterUtility : MonoBehaviour
    {
        private static readonly ExporterSettings Settings = ExporterSettings.Instance;
        
        private static string NAME = "[ExporterUtility]: ";
        
        public static void Export(string sourcePath, string targetPath, Object targetObject, string[] dependencies)
        {
            Regex fbxSubRegex = new Regex(@".fbx", RegexOptions.IgnoreCase);
            Regex pathRegex = new Regex(@"/", RegexOptions.IgnoreCase);
            Regex texRegex = new Regex(@"tex_", RegexOptions.IgnoreCase);

            sourcePath = pathRegex.Replace(sourcePath, @"\");
            targetPath = pathRegex.Replace(targetPath, @"\");
            var targetDir = Directory.CreateDirectory(fbxSubRegex.Replace(targetPath, ""));
            var targetName = targetPath.Split('\\').Last().Split('.').First();
            if (!targetPath.Split('\\').Last().Contains("mesh_"))
            {
                targetName = "mesh_" + targetName;
            }

            File.Copy(sourcePath, targetDir.FullName + @"\" + targetName + ".fbx");
            var targetTexFolder = Directory.CreateDirectory(targetDir.FullName + @"/Textures");
            foreach (var dep in dependencies)
            {
                if (Settings.exportTextureFormats.Contains(dep.Split('.').Last()))
                {
                    var targetDep = pathRegex.Replace(dep, @"\");

                    var textureName = targetDep.Split('\\').Last();
                    if (texRegex.Matches(textureName).Count < 1 && Settings.addTexturePrefix)
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

            Debug.Log($@"{NAME}{sourcePath.Split('\\').Last()} successfully exported to {targetDir.FullName}\");
        }
    }
}