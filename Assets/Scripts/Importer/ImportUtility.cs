using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Importer
{
    public static class ImportUtility
    {
        public static void Process(List<string> assets, string assetPath)
        {
            
            foreach (var asset in assets)
            {
                Debug.Log($"asset {assetPath}");
                
            }
        }
    }
}