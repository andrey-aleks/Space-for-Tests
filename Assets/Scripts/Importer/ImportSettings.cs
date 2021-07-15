using UnityEngine;

namespace Utilities.Importer
{
    [CreateAssetMenu(fileName = "CustomImportSettings", menuName = "Custom Import Settings", order = 1010)]
    public class ImportSettings : ScriptableObject
    {
        public string path;
        //public static ImportSettings Instance;

        public bool doPostProcess;
        
        


    }
}
