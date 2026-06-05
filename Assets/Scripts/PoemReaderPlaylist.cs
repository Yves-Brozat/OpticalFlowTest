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

    [Header("VFX Force")]
    [Tooltip("Index de la force VFX à activer pour cette zone (0-7)")]
    [Range(0, 7)]
    public int forceIndex = 0;

    // État interne
    [HideInInspector] public int currentPoemIndex = 0;
    [HideInInspector] public bool isReading = false;
    [HideInInspector] public bool hasStarted = false; // Pour savoir si la zone a déjà démarré au moins une fois
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

    [Header("Global Controls")]
    [SerializeField, Tooltip("Touche pour réinitialiser toutes les zones")]
    Key _resetKey = Key.R;

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

    [Header("VFX Integration")]
    [SerializeField, Tooltip("Référence au Visual Effect pour contrôler les forces")]
    UnityEngine.VFX.VisualEffect _visualEffect = null;

    [SerializeField, Tooltip("Noms des propriétés VFX pour les 8 forces")]
    string[] _forcePropertyNames = new string[8]
    {
        "Force1_Enabled",
        "Force2_Enabled",
        "Force3_Enabled",
        "Force4_Enabled",
        "Force5_Enabled",
        "Force6_Enabled",
        "Force7_Enabled",
        "Force8_Enabled"
    };

    [Header("Debug")]
    [SerializeField] bool _showDebugLogs = true;

    // État des forces VFX (8 forces, une par zone)
    private bool[] _forceStates = new bool[8];

    void Start()
    {
        // Initialiser tous les textes vides et les forces
        for (int i = 0; i < 8; i++)
        {
            _forceStates[i] = false;
        }

        foreach (var zone in _zones)
        {
            if (zone != null && zone.textDisplay != null)
            {
                zone.textDisplay.text = "";
                zone.currentPoemIndex = 0;
                zone.hasStarted = false;
            }
        }
    }

    void OnDestroy()
    {
        // Nettoyer si nécessaire (placeholder pour futures fonctionnalités)
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
                // Arrêter le poème en cours s'il y en a un
                if (zone.isReading)
                {
                    StopZone(i);
                }

                // Si ce n'est pas la première fois, passer au suivant
                if (zone.hasStarted)
                {
                    zone.currentPoemIndex = (zone.currentPoemIndex + 1) % zone.poems.Length;
                }
                else
                {
                    // Premier appui : marquer comme démarré
                    zone.hasStarted = true;
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

        // Touche Reset pour réinitialiser toutes les zones
        if (Keyboard.current[_resetKey].wasPressedThisFrame)
        {
            ResetAllZones();
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

        // Appliquer l'alignement spécifique au poème
        zone.textDisplay.alignment = currentPoem.textAlignment;

        // Activer la force VFX correspondante
        if (zone.forceIndex >= 0 && zone.forceIndex < 8)
        {
            SetForceState(zone.forceIndex, true);
        }

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

        // Désactiver la force VFX correspondante
        if (zone.forceIndex >= 0 && zone.forceIndex < 8)
        {
            SetForceState(zone.forceIndex, false);
        }

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
        zone.hasStarted = false; // Réinitialiser le flag de démarrage

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

        // Désactiver la force VFX correspondante
        if (zone.forceIndex >= 0 && zone.forceIndex < 8)
        {
            SetForceState(zone.forceIndex, false);
        }

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

    /// <summary>
    /// Retourne l'état d'activation d'une force VFX
    /// </summary>
    public bool GetForceState(int forceIndex)
    {
        if (forceIndex < 0 || forceIndex >= 8) return false;
        return _forceStates[forceIndex];
    }

    /// <summary>
    /// Définit l'état d'une force VFX et met à jour immédiatement le Visual Effect
    /// </summary>
    private void SetForceState(int forceIndex, bool enabled)
    {
        if (forceIndex < 0 || forceIndex >= 8) return;

        _forceStates[forceIndex] = enabled;

        // Mettre à jour immédiatement le VFX si la référence existe
        if (_visualEffect != null && forceIndex < _forcePropertyNames.Length)
        {
            string propertyName = _forcePropertyNames[forceIndex];
            if (_visualEffect.HasBool(propertyName))
            {
                _visualEffect.SetBool(propertyName, enabled);

                if (_showDebugLogs)
                {
                    string stateText = enabled ? "activée" : "désactivée";
                    Debug.Log($"[PoemReaderPlaylist] 🌀 Force {forceIndex} ({propertyName}) {stateText}");
                }
            }
            else if (_showDebugLogs)
            {
                Debug.LogWarning($"[PoemReaderPlaylist] ⚠ Propriété VFX '{propertyName}' introuvable !");
            }
        }
        else if (_showDebugLogs && _visualEffect == null)
        {
            Debug.LogWarning($"[PoemReaderPlaylist] ⚠ Visual Effect non assigné. Force {forceIndex} changée en mémoire seulement.");
        }
    }
}

} // namespace OpticalFlowTest
