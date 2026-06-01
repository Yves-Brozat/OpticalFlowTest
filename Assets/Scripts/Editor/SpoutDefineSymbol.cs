// Ce fichier définit le symbole KLAK_SPOUT automatiquement
// quand le package KlakSpout est installé

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;

namespace OpticalFlowTest.Editor
{
    [InitializeOnLoad]
    public static class SpoutDefineSymbol
    {
        const string DEFINE_SYMBOL = "KLAK_SPOUT";

        static SpoutDefineSymbol()
        {
            // Vérifier si KlakSpout est installé
            bool isSpoutInstalled = IsPackageInstalled("jp.keijiro.klak.spout");

            // Obtenir les symboles actuels
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

            bool hasDefine = defines.Contains(DEFINE_SYMBOL);

            // Ajouter ou retirer le symbole selon l'installation
            if (isSpoutInstalled && !hasDefine)
            {
                defines += ";" + DEFINE_SYMBOL;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
                UnityEngine.Debug.Log("KlakSpout détecté : symbole KLAK_SPOUT ajouté");
            }
            else if (!isSpoutInstalled && hasDefine)
            {
                defines = defines.Replace(";" + DEFINE_SYMBOL, "").Replace(DEFINE_SYMBOL, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
                UnityEngine.Debug.Log("KlakSpout non détecté : symbole KLAK_SPOUT retiré");
            }
        }

        static bool IsPackageInstalled(string packageId)
        {
            var assemblies = CompilationPipeline.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.name.Contains("Klak.Spout"))
                    return true;
            }
            return false;
        }
    }
}
#endif
