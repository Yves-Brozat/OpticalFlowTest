using UnityEngine;

namespace OpticalFlowTest {

/// <summary>
/// ScriptableObject contenant un poème
/// Peut être réutilisé dans plusieurs zones
/// </summary>
[CreateAssetMenu(fileName = "New Poem", menuName = "OpticalFlowTest/Poem", order = 1)]
public class PoemAsset : ScriptableObject
{
    [Header("Poem Content")]
    [Tooltip("Titre du poème (optionnel, pour identification)")]
    public string title = "Sans titre";

    [Tooltip("Auteur du poème (optionnel)")]
    public string author = "";

    [Tooltip("Lignes du poème")]
    [TextArea(5, 20)]
    public string[] lines = new string[]
    {
        "Première ligne",
        "Deuxième ligne",
        "Troisième ligne"
    };

    [Header("Metadata (optionnel)")]
    [Tooltip("Catégorie ou tag pour organiser")]
    public string category = "";

    // Méthode helper pour avoir un résumé
    public override string ToString()
    {
        string display = title;
        if (!string.IsNullOrEmpty(author))
            display += $" - {author}";
        return display;
    }
}

} // namespace OpticalFlowTest
