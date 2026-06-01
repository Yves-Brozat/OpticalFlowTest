using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace OpticalFlowTest {

/// <summary>
/// Lecteur automatique de poèmes avec apparition lettre par lettre
/// Affiche le texte dans un TextMeshPro UI
/// </summary>
public class PoemReaderUI : MonoBehaviour
{
    [Header("Input")]
    [SerializeField, Tooltip("Touche pour démarrer le poème")]
    Key _startKey = Key.Digit1;

    [Header("Poem Content")]
    [SerializeField, Tooltip("Phrases du poème (une ligne par phrase)")]
    [TextArea(3, 10)]
    string[] _poemLines = new string[]
    {
        "Les sanglots longs",
        "Des violons",
        "De l'automne",
        "Blessent mon coeur",
        "D'une langueur",
        "Monotone"
    };

    [Header("Timing")]
    [SerializeField, Range(0.01f, 0.5f), Tooltip("Délai entre chaque lettre (secondes)")]
    float _letterDelay = 0.1f;

    [SerializeField, Range(0.5f, 10f), Tooltip("Délai entre chaque phrase (secondes)")]
    float _lineDelay = 5f;

    [Header("UI References")]
    [SerializeField, Tooltip("TextMeshProUGUI pour afficher le poème")]
    TextMeshProUGUI _textDisplay = null;

    [Header("Display Options")]
    [SerializeField, Tooltip("Effacer le texte entre chaque ligne")]
    bool _clearBetweenLines = true;

    [SerializeField, Tooltip("Ajouter un saut de ligne après chaque phrase")]
    bool _addLineBreaks = false;

    [Header("Debug")]
    [SerializeField] bool _showDebugLogs = true;

    // État interne
    bool _isReading = false;
    int _currentLine = 0;
    Coroutine _readingCoroutine = null;

    void Start()
    {
        // Initialiser le texte vide
        if (_textDisplay != null)
            _textDisplay.text = "";
    }

    void Update()
    {
        // Utilisation du nouveau Input System
        if (Keyboard.current != null && Keyboard.current[_startKey].wasPressedThisFrame && !_isReading)
        {
            StartPoem();
        }

        // Touche Escape pour arrêter
        if (Keyboard.current != null && Keyboard.current[Key.Escape].wasPressedThisFrame && _isReading)
        {
            StopPoem();
        }
    }

    /// <summary>
    /// Démarre la lecture du poème depuis le début
    /// </summary>
    [ContextMenu("Start Poem")]
    public void StartPoem()
    {
        if (_isReading)
        {
            if (_showDebugLogs)
                Debug.LogWarning("[PoemReaderUI] Poème déjà en cours de lecture !");
            return;
        }

        if (_textDisplay == null)
        {
            Debug.LogError("[PoemReaderUI] TextMeshProUGUI non assigné !");
            return;
        }

        if (_poemLines.Length == 0)
        {
            Debug.LogError("[PoemReaderUI] Aucune ligne de poème définie !");
            return;
        }

        _currentLine = 0;
        _isReading = true;
        _textDisplay.text = "";

        if (_showDebugLogs)
            Debug.Log($"[PoemReaderUI] ▶ Démarrage du poème : {_poemLines.Length} lignes");

        _readingCoroutine = StartCoroutine(ReadPoemCoroutine());
    }

    /// <summary>
    /// Arrête la lecture du poème
    /// </summary>
    [ContextMenu("Stop Poem")]
    public void StopPoem()
    {
        if (_readingCoroutine != null)
        {
            StopCoroutine(_readingCoroutine);
            _readingCoroutine = null;
        }

        _isReading = false;
        _currentLine = 0;

        if (_showDebugLogs)
            Debug.Log("[PoemReaderUI] ⏹ Poème arrêté");
    }

    /// <summary>
    /// Efface le texte affiché
    /// </summary>
    [ContextMenu("Clear Text")]
    public void ClearText()
    {
        if (_textDisplay != null)
            _textDisplay.text = "";
    }

    /// <summary>
    /// Coroutine principale de lecture
    /// </summary>
    IEnumerator ReadPoemCoroutine()
    {
        for (int lineIndex = 0; lineIndex < _poemLines.Length; lineIndex++)
        {
            _currentLine = lineIndex;
            string line = _poemLines[lineIndex];

            if (_showDebugLogs)
                Debug.Log($"[PoemReaderUI] Ligne {lineIndex + 1}/{_poemLines.Length}: \"{line}\"");

            // Effacer le texte avant chaque ligne (sauf la première si clearBetweenLines)
            if (_clearBetweenLines)
            {
                _textDisplay.text = "";
            }

            // Afficher la ligne lettre par lettre
            yield return StartCoroutine(DisplayLineCoroutine(line));

            // Ajouter un saut de ligne si demandé (sauf pour la dernière ligne)
            if (_addLineBreaks && lineIndex < _poemLines.Length - 1 && !_clearBetweenLines)
            {
                _textDisplay.text += "\n";
            }

            // Délai avant la ligne suivante (ou avant d'effacer si c'est la dernière)
            if (_showDebugLogs)
            {
                if (lineIndex < _poemLines.Length - 1)
                    Debug.Log($"[PoemReaderUI] ⏸ Pause de {_lineDelay}s avant la ligne suivante...");
                else
                    Debug.Log($"[PoemReaderUI] ⏸ Pause de {_lineDelay}s avant effacement final...");
            }

            yield return new WaitForSeconds(_lineDelay);

            // Effacer la dernière phrase si en mode clearBetweenLines
            if (_clearBetweenLines && lineIndex == _poemLines.Length - 1)
            {
                _textDisplay.text = "";
            }
        }

        // Fin du poème
        _isReading = false;
        if (_showDebugLogs)
            Debug.Log("[PoemReaderUI] ✅ Poème terminé !");
    }

    /// <summary>
    /// Affiche une ligne lettre par lettre
    /// </summary>
    IEnumerator DisplayLineCoroutine(string line)
    {
        // Diviser la ligne en mots
        string[] words = line.Split(' ');

        for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
        {
            string word = words[wordIndex];

            // Vérifier si le mot va déborder avant de l'écrire
            if (WillWordWrap(word))
            {
                // Ajouter un saut de ligne avant le mot
                _textDisplay.text += "\n";
            }

            // Ajouter le mot lettre par lettre
            foreach (char c in word)
            {
                _textDisplay.text += c;

                if (_showDebugLogs)
                    Debug.Log($"[PoemReaderUI]   '{c}'");

                yield return new WaitForSeconds(_letterDelay);
            }

            // Ajouter l'espace après le mot (sauf pour le dernier)
            if (wordIndex < words.Length - 1)
            {
                _textDisplay.text += " ";
                yield return new WaitForSeconds(_letterDelay * 0.5f);
            }
        }
    }

    /// <summary>
    /// Vérifie si un mot va déborder sur la ligne suivante
    /// </summary>
    bool WillWordWrap(string word)
    {
        if (_textDisplay == null || string.IsNullOrEmpty(_textDisplay.text)) 
            return false;

        // Obtenir la largeur du conteneur
        float containerWidth = _textDisplay.rectTransform.rect.width;

        // Calculer la largeur du texte actuel + le mot à ajouter
        string testText = _textDisplay.text + word;

        // Utiliser GetPreferredValues pour calculer la taille sans modifier le texte
        Vector2 textSize = _textDisplay.GetPreferredValues(testText, containerWidth, 0);

        // Calculer la largeur du texte actuel seul
        Vector2 currentSize = _textDisplay.GetPreferredValues(_textDisplay.text, containerWidth, 0);

        // Si ajouter le mot augmente la hauteur, c'est qu'il va wrap
        return textSize.y > currentSize.y;
    }

    /// <summary>
    /// Réinitialise le lecteur
    /// </summary>
    [ContextMenu("Reset Reader")]
    public void ResetReader()
    {
        StopPoem();
        ClearText();
        _currentLine = 0;

        if (_showDebugLogs)
            Debug.Log("[PoemReaderUI] 🔄 Lecteur réinitialisé");
    }

    // Getters pour l'état actuel
    public bool IsReading => _isReading;
    public int CurrentLine => _currentLine;
    public int TotalLines => _poemLines.Length;
    public float Progress => _poemLines.Length > 0 ? (float)_currentLine / _poemLines.Length : 0f;
    public string CurrentText => _textDisplay != null ? _textDisplay.text : "";
}

} // namespace OpticalFlowTest
