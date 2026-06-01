using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace OpticalFlowTest {

/// <summary>
/// Configuration d'une zone de lecture (un TextMeshPro + sa touche + sa playlist de poèmes)
/// </summary>
[System.Serializable]
public class PoemZone
{
    [Header("Input")]
    [Tooltip("Touche pour cette zone")]
    public Key triggerKey = Key.Digit1;

    [Header("UI Reference")]
    [Tooltip("TextMeshProUGUI pour afficher les poèmes")]
    public TextMeshProUGUI textDisplay = null;

    [Header("Poems Playlist")]
    [Tooltip("Liste des poèmes à afficher successivement dans cette zone")]
    public PoemAsset[] poems = new PoemAsset[0];

    // État interne
    [HideInInspector] public int currentPoemIndex = 0;
    [HideInInspector] public bool isReading = false;
    [HideInInspector] public Coroutine readingCoroutine = null;
}

/// <summary>
/// Lecteur de poèmes multiples avec système de playlist
/// Chaque zone (TextMeshPro) a sa propre touche et sa propre playlist de poèmes
/// Réappuyer sur la touche fait passer au poème suivant
/// </summary>
public class PoemReaderPlaylist : MonoBehaviour
{
    [Header("Zones Configuration")]
    [SerializeField, Tooltip("Configuration des 8 zones de lecture")]
    PoemZone[] _zones = new PoemZone[8];

    [Header("Timing")]
    [SerializeField, Range(0.01f, 0.5f), Tooltip("Délai entre chaque lettre (secondes)")]
    float _letterDelay = 0.1f;

    [SerializeField, Range(0.5f, 10f), Tooltip("Délai entre chaque phrase (secondes)")]
    float _lineDelay = 5f;

    [Header("Display Options")]
    [SerializeField, Tooltip("Effacer le texte entre chaque ligne")]
    bool _clearBetweenLines = true;

    [SerializeField, Tooltip("Ajouter un saut de ligne après chaque phrase")]
    bool _addLineBreaks = false;

    [Header("Debug")]
    [SerializeField] bool _showDebugLogs = true;

    void Start()
    {
        // Initialiser tous les textes vides
        foreach (var zone in _zones)
        {
            if (zone != null && zone.textDisplay != null)
            {
                zone.textDisplay.text = "";
                zone.currentPoemIndex = 0;
            }
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Vérifier chaque zone
        for (int i = 0; i < _zones.Length; i++)
        {
            var zone = _zones[i];
            if (zone == null || zone.textDisplay == null || zone.poems.Length == 0) continue;

            // Si la touche est pressée
            if (Keyboard.current[zone.triggerKey].wasPressedThisFrame)
            {
                // Si un poème est déjà en lecture dans cette zone
                if (zone.isReading)
                {
                    // Arrêter le poème actuel et passer au suivant
                    StopZone(i);
                    zone.currentPoemIndex = (zone.currentPoemIndex + 1) % zone.poems.Length;
                }

                // Démarrer le poème actuel
                StartZonePoem(i);
            }
        }

        // Touche Escape pour tout arrêter
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            StopAllZones();
        }
    }

    /// <summary>
    /// Démarre la lecture du poème actuel d'une zone
    /// </summary>
    public void StartZonePoem(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= _zones.Length)
        {
            Debug.LogError($"[PoemReaderPlaylist] Index de zone invalide : {zoneIndex}");
            return;
        }

        var zone = _zones[zoneIndex];

        if (zone == null || zone.textDisplay == null)
        {
            Debug.LogError($"[PoemReaderPlaylist] Zone {zoneIndex} non configurée !");
            return;
        }

        if (zone.poems.Length == 0)
        {
            Debug.LogError($"[PoemReaderPlaylist] Zone {zoneIndex} : Aucun poème défini !");
            return;
        }

        var currentPoem = zone.poems[zone.currentPoemIndex];

        if (currentPoem == null)
        {
            Debug.LogError($"[PoemReaderPlaylist] Zone {zoneIndex}, Poème {zone.currentPoemIndex} : PoemAsset manquant !");
            return;
        }

        if (currentPoem.lines.Length == 0)
        {
            Debug.LogError($"[PoemReaderPlaylist] Zone {zoneIndex}, Poème {zone.currentPoemIndex} : Aucune ligne définie !");
            return;
        }

        zone.isReading = true;
        zone.textDisplay.text = "";

        if (_showDebugLogs)
            Debug.Log($"[PoemReaderPlaylist] ▶ Zone {zoneIndex} - Poème {zone.currentPoemIndex + 1}/{zone.poems.Length}");

        zone.readingCoroutine = StartCoroutine(ReadPoemCoroutine(zone, zoneIndex));
    }

    /// <summary>
    /// Arrête la lecture d'une zone
    /// </summary>
    public void StopZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= _zones.Length) return;

        var zone = _zones[zoneIndex];
        if (zone == null) return;

        if (zone.readingCoroutine != null)
        {
            StopCoroutine(zone.readingCoroutine);
            zone.readingCoroutine = null;
        }

        zone.isReading = false;

        if (_showDebugLogs)
            Debug.Log($"[PoemReaderPlaylist] ⏹ Zone {zoneIndex} arrêtée");
    }

    /// <summary>
    /// Arrête toutes les zones
    /// </summary>
    [ContextMenu("Stop All Zones")]
    public void StopAllZones()
    {
        for (int i = 0; i < _zones.Length; i++)
        {
            StopZone(i);
        }

        if (_showDebugLogs)
            Debug.Log("[PoemReaderPlaylist] ⏹ Toutes les zones arrêtées");
    }

    /// <summary>
    /// Passe au poème suivant dans une zone
    /// </summary>
    public void NextPoem(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= _zones.Length) return;

        var zone = _zones[zoneIndex];
        if (zone == null || zone.poems.Length == 0) return;

        // Arrêter le poème actuel
        StopZone(zoneIndex);

        // Passer au suivant (boucle)
        zone.currentPoemIndex = (zone.currentPoemIndex + 1) % zone.poems.Length;

        // Démarrer le nouveau poème
        StartZonePoem(zoneIndex);
    }

    /// <summary>
    /// Réinitialise une zone (retour au premier poème)
    /// </summary>
    public void ResetZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= _zones.Length) return;

        var zone = _zones[zoneIndex];
        if (zone == null) return;

        StopZone(zoneIndex);
        zone.currentPoemIndex = 0;

        if (zone.textDisplay != null)
            zone.textDisplay.text = "";

        if (_showDebugLogs)
            Debug.Log($"[PoemReaderPlaylist] 🔄 Zone {zoneIndex} réinitialisée");
    }

    /// <summary>
    /// Réinitialise toutes les zones
    /// </summary>
    [ContextMenu("Reset All Zones")]
    public void ResetAllZones()
    {
        for (int i = 0; i < _zones.Length; i++)
        {
            ResetZone(i);
        }

        if (_showDebugLogs)
            Debug.Log("[PoemReaderPlaylist] 🔄 Toutes les zones réinitialisées");
    }

    /// <summary>
    /// Coroutine principale de lecture pour un poème
    /// </summary>
    IEnumerator ReadPoemCoroutine(PoemZone zone, int zoneIndex)
    {
        var currentPoem = zone.poems[zone.currentPoemIndex];

        for (int lineIndex = 0; lineIndex < currentPoem.lines.Length; lineIndex++)
        {
            string line = currentPoem.lines[lineIndex];

            if (_showDebugLogs)
                Debug.Log($"[PoemReaderPlaylist] Zone {zoneIndex} - Ligne {lineIndex + 1}/{currentPoem.lines.Length}: \"{line}\"");

            // Effacer le texte avant chaque ligne
            if (_clearBetweenLines)
            {
                zone.textDisplay.text = "";
            }

            // Afficher la ligne lettre par lettre
            yield return StartCoroutine(DisplayLineCoroutine(zone, line));

            // Ajouter un saut de ligne si demandé
            if (_addLineBreaks && lineIndex < currentPoem.lines.Length - 1 && !_clearBetweenLines)
            {
                zone.textDisplay.text += "\n";
            }

            // Délai avant la ligne suivante (ou avant d'effacer si c'est la dernière)
            if (_showDebugLogs)
            {
                if (lineIndex < currentPoem.lines.Length - 1)
                    Debug.Log($"[PoemReaderPlaylist] Zone {zoneIndex} - ⏸ Pause de {_lineDelay}s...");
                else
                    Debug.Log($"[PoemReaderPlaylist] Zone {zoneIndex} - ⏸ Pause finale de {_lineDelay}s...");
            }

            yield return new WaitForSeconds(_lineDelay);

            // Effacer la dernière phrase si en mode clearBetweenLines
            if (_clearBetweenLines && lineIndex == currentPoem.lines.Length - 1)
            {
                zone.textDisplay.text = "";
            }
        }

        // Fin du poème
        zone.isReading = false;
        if (_showDebugLogs)
            Debug.Log($"[PoemReaderPlaylist] ✅ Zone {zoneIndex} - Poème {zone.currentPoemIndex + 1} terminé !");
    }

    /// <summary>
    /// Affiche une ligne lettre par lettre
    /// </summary>
    IEnumerator DisplayLineCoroutine(PoemZone zone, string line)
    {
        // Diviser la ligne en mots
        string[] words = line.Split(' ');

        for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
        {
            string word = words[wordIndex];

            // Vérifier si le mot va déborder avant de l'écrire
            if (WillWordWrap(zone.textDisplay, word))
            {
                zone.textDisplay.text += "\n";
            }

            // Ajouter le mot lettre par lettre
            foreach (char c in word)
            {
                zone.textDisplay.text += c;
                yield return new WaitForSeconds(_letterDelay);
            }

            // Ajouter l'espace après le mot (sauf pour le dernier)
            if (wordIndex < words.Length - 1)
            {
                zone.textDisplay.text += " ";
                yield return new WaitForSeconds(_letterDelay * 0.5f);
            }
        }
    }

    /// <summary>
    /// Vérifie si un mot va déborder sur la ligne suivante
    /// </summary>
    bool WillWordWrap(TextMeshProUGUI textDisplay, string word)
    {
        if (textDisplay == null || string.IsNullOrEmpty(textDisplay.text)) 
            return false;

        float containerWidth = textDisplay.rectTransform.rect.width;
        string testText = textDisplay.text + word;
        Vector2 textSize = textDisplay.GetPreferredValues(testText, containerWidth, 0);
        Vector2 currentSize = textDisplay.GetPreferredValues(textDisplay.text, containerWidth, 0);

        return textSize.y > currentSize.y;
    }

    // Getters
    public int GetZoneCurrentPoem(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= _zones.Length) return -1;
        return _zones[zoneIndex]?.currentPoemIndex ?? -1;
    }

    public bool IsZoneReading(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= _zones.Length) return false;
        return _zones[zoneIndex]?.isReading ?? false;
    }
}

} // namespace OpticalFlowTest
