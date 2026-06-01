using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace OpticalFlowTest {

/// <summary>
/// Configuration d'un poème individuel
/// </summary>
[System.Serializable]
public class PoemConfig
{
    [Header("Input")]
    [Tooltip("Touche pour démarrer ce poème")]
    public Key startKey = Key.Digit1;

    [Header("Poem Content")]
    [Tooltip("Phrases du poème (une ligne par phrase)")]
    [TextArea(3, 10)]
    public string[] lines = new string[]
    {
        "Poème par défaut",
        "Ligne 2",
        "Ligne 3"
    };

    [Header("UI Reference")]
    [Tooltip("TextMeshProUGUI pour afficher ce poème")]
    public TextMeshProUGUI textDisplay = null;

    // État interne (ne pas modifier dans l'Inspector)
    [HideInInspector] public bool isReading = false;
    [HideInInspector] public int currentLine = 0;
    [HideInInspector] public Coroutine readingCoroutine = null;
}

/// <summary>
/// Lecteur automatique de poèmes multiples avec apparition lettre par lettre
/// Supporte jusqu'à 8 poèmes simultanés avec leurs propres touches et UI
/// </summary>
public class PoemReaderMulti : MonoBehaviour
{
    [Header("Poems Configuration")]
    [SerializeField, Tooltip("Configuration des 8 poèmes")]
    PoemConfig[] _poems = new PoemConfig[8];

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
        foreach (var poem in _poems)
        {
            if (poem != null && poem.textDisplay != null)
                poem.textDisplay.text = "";
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Vérifier chaque poème
        for (int i = 0; i < _poems.Length; i++)
        {
            var poem = _poems[i];
            if (poem == null || poem.textDisplay == null) continue;

            // Démarrer le poème si la touche est pressée
            if (Keyboard.current[poem.startKey].wasPressedThisFrame && !poem.isReading)
            {
                StartPoem(i);
            }
        }

        // Touche Escape pour tout arrêter
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            StopAllPoems();
        }
    }

    /// <summary>
    /// Démarre la lecture d'un poème spécifique
    /// </summary>
    public void StartPoem(int poemIndex)
    {
        if (poemIndex < 0 || poemIndex >= _poems.Length)
        {
            Debug.LogError($"[PoemReaderMulti] Index de poème invalide : {poemIndex}");
            return;
        }

        var poem = _poems[poemIndex];

        if (poem == null)
        {
            Debug.LogError($"[PoemReaderMulti] Poème {poemIndex} non configuré !");
            return;
        }

        if (poem.isReading)
        {
            if (_showDebugLogs)
                Debug.LogWarning($"[PoemReaderMulti] Poème {poemIndex} déjà en cours de lecture !");
            return;
        }

        if (poem.textDisplay == null)
        {
            Debug.LogError($"[PoemReaderMulti] Poème {poemIndex} : TextMeshProUGUI non assigné !");
            return;
        }

        if (poem.lines.Length == 0)
        {
            Debug.LogError($"[PoemReaderMulti] Poème {poemIndex} : Aucune ligne définie !");
            return;
        }

        poem.currentLine = 0;
        poem.isReading = true;
        poem.textDisplay.text = "";

        if (_showDebugLogs)
            Debug.Log($"[PoemReaderMulti] ▶ Démarrage poème {poemIndex} : {poem.lines.Length} lignes");

        poem.readingCoroutine = StartCoroutine(ReadPoemCoroutine(poem, poemIndex));
    }

    /// <summary>
    /// Arrête la lecture d'un poème spécifique
    /// </summary>
    public void StopPoem(int poemIndex)
    {
        if (poemIndex < 0 || poemIndex >= _poems.Length) return;

        var poem = _poems[poemIndex];
        if (poem == null) return;

        if (poem.readingCoroutine != null)
        {
            StopCoroutine(poem.readingCoroutine);
            poem.readingCoroutine = null;
        }

        poem.isReading = false;
        poem.currentLine = 0;

        if (_showDebugLogs)
            Debug.Log($"[PoemReaderMulti] ⏹ Poème {poemIndex} arrêté");
    }

    /// <summary>
    /// Arrête tous les poèmes en cours
    /// </summary>
    [ContextMenu("Stop All Poems")]
    public void StopAllPoems()
    {
        for (int i = 0; i < _poems.Length; i++)
        {
            StopPoem(i);
        }

        if (_showDebugLogs)
            Debug.Log("[PoemReaderMulti] ⏹ Tous les poèmes arrêtés");
    }

    /// <summary>
    /// Efface le texte d'un poème spécifique
    /// </summary>
    public void ClearText(int poemIndex)
    {
        if (poemIndex < 0 || poemIndex >= _poems.Length) return;

        var poem = _poems[poemIndex];
        if (poem != null && poem.textDisplay != null)
            poem.textDisplay.text = "";
    }

    /// <summary>
    /// Efface tous les textes
    /// </summary>
    [ContextMenu("Clear All Texts")]
    public void ClearAllTexts()
    {
        for (int i = 0; i < _poems.Length; i++)
        {
            ClearText(i);
        }
    }

    /// <summary>
    /// Coroutine principale de lecture pour un poème
    /// </summary>
    IEnumerator ReadPoemCoroutine(PoemConfig poem, int poemIndex)
    {
        for (int lineIndex = 0; lineIndex < poem.lines.Length; lineIndex++)
        {
            poem.currentLine = lineIndex;
            string line = poem.lines[lineIndex];

            if (_showDebugLogs)
                Debug.Log($"[PoemReaderMulti] Poème {poemIndex} - Ligne {lineIndex + 1}/{poem.lines.Length}: \"{line}\"");

            // Effacer le texte avant chaque ligne
            if (_clearBetweenLines)
            {
                poem.textDisplay.text = "";
            }

            // Afficher la ligne lettre par lettre
            yield return StartCoroutine(DisplayLineCoroutine(poem, line));

            // Ajouter un saut de ligne si demandé
            if (_addLineBreaks && lineIndex < poem.lines.Length - 1 && !_clearBetweenLines)
            {
                poem.textDisplay.text += "\n";
            }

            // Délai avant la ligne suivante (ou avant d'effacer si c'est la dernière)
            if (_showDebugLogs)
            {
                if (lineIndex < poem.lines.Length - 1)
                    Debug.Log($"[PoemReaderMulti] Poème {poemIndex} - ⏸ Pause de {_lineDelay}s avant la ligne suivante...");
                else
                    Debug.Log($"[PoemReaderMulti] Poème {poemIndex} - ⏸ Pause de {_lineDelay}s avant effacement final...");
            }

            yield return new WaitForSeconds(_lineDelay);

            // Effacer la dernière phrase si en mode clearBetweenLines
            if (_clearBetweenLines && lineIndex == poem.lines.Length - 1)
            {
                poem.textDisplay.text = "";
            }
        }

        // Fin du poème
        poem.isReading = false;
        if (_showDebugLogs)
            Debug.Log($"[PoemReaderMulti] ✅ Poème {poemIndex} terminé !");
    }

    /// <summary>
    /// Affiche une ligne lettre par lettre
    /// </summary>
    IEnumerator DisplayLineCoroutine(PoemConfig poem, string line)
    {
        // Diviser la ligne en mots
        string[] words = line.Split(' ');

        for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
        {
            string word = words[wordIndex];

            // Vérifier si le mot va déborder avant de l'écrire
            if (WillWordWrap(poem.textDisplay, word))
            {
                // Ajouter un saut de ligne avant le mot
                poem.textDisplay.text += "\n";
            }

            // Ajouter le mot lettre par lettre
            foreach (char c in word)
            {
                poem.textDisplay.text += c;

                if (_showDebugLogs)
                    Debug.Log($"[PoemReaderMulti]   '{c}'");

                yield return new WaitForSeconds(_letterDelay);
            }

            // Ajouter l'espace après le mot (sauf pour le dernier)
            if (wordIndex < words.Length - 1)
            {
                poem.textDisplay.text += " ";
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

        // Obtenir la largeur du conteneur
        float containerWidth = textDisplay.rectTransform.rect.width;

        // Calculer la largeur du texte actuel + le mot à ajouter
        string testText = textDisplay.text + word;

        // Utiliser GetPreferredValues pour calculer la taille sans modifier le texte
        Vector2 textSize = textDisplay.GetPreferredValues(testText, containerWidth, 0);

        // Calculer la largeur du texte actuel seul
        Vector2 currentSize = textDisplay.GetPreferredValues(textDisplay.text, containerWidth, 0);

        // Si ajouter le mot augmente la hauteur, c'est qu'il va wrap
        return textSize.y > currentSize.y;
    }

    /// <summary>
    /// Réinitialise tous les lecteurs
    /// </summary>
    [ContextMenu("Reset All Readers")]
    public void ResetAllReaders()
    {
        StopAllPoems();
        ClearAllTexts();

        foreach (var poem in _poems)
        {
            if (poem != null)
                poem.currentLine = 0;
        }

        if (_showDebugLogs)
            Debug.Log("[PoemReaderMulti] 🔄 Tous les lecteurs réinitialisés");
    }

    // Getters pour l'état
    public bool IsAnyReading()
    {
        foreach (var poem in _poems)
        {
            if (poem != null && poem.isReading)
                return true;
        }
        return false;
    }

    public int GetReadingCount()
    {
        int count = 0;
        foreach (var poem in _poems)
        {
            if (poem != null && poem.isReading)
                count++;
        }
        return count;
    }
}

} // namespace OpticalFlowTest
