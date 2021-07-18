using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utilities.Importer
{
    [CreateAssetMenu(fileName = "CustomImportSettings", menuName = "Custom Import Settings", order = 1010)]
    public class ImportSettings : ScriptableObject
    {
        public string path;
        private static ImportSettings _instance;
        public static ImportSettings Instance => _instance ?? (_instance = LoadAsset());

        private static ImportSettings LoadAsset([CallerFilePath] string callerFilepath = null)
        {
            var folder = Path.GetDirectoryName(callerFilepath);
            folder = folder.Substring(folder.LastIndexOf("/Assets/", StringComparison.Ordinal) + 1);
            return null;
        }
        

        public bool doPostProcess;
        
        


    }
}
