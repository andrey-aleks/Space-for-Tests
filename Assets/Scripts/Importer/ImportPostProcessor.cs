using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities.Importer
{
    public class ImportPostProcessor : AssetPostprocessor
    {
        private static string assetPath;
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {

            /*if (!ImportSettings.Instance.doPostProcess)
            {
                return;
            }
            */

            var assets = new List<string>();
            assets.AddRange(importedAssets);
            

            // ImportUtility.Process(assets, assetPath);
        }

        private void OnPreprocessAsset()
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            




        }
    }
}
