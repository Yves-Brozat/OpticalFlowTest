using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

namespace OpticalFlowTest {

/// <summary>
/// Lecteur automatique de poèmes avec apparition lettre par lettre
/// Contrôle le spawn de particules VFX pour afficher le texte
/// </summary>
public class PoemReader : MonoBehaviour
{
    [Header("Input")]
    [SerializeField, Tooltip("Touche pour démarrer le poème (ex: Digit1 pour touche 1)")]
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

    [Header("Particle Settings")]
    [SerializeField, Tooltip("Position de départ (X, Y)")]
    Vector2 _startPosition = new Vector2(-5f, 3f);

    [SerializeField, Range(0.1f, 2f), Tooltip("Espacement horizontal entre lettres")]
    float _letterSpacing = 0.5f;

    [SerializeField, Range(0.5f, 3f), Tooltip("Espacement vertical entre lignes")]
    float _lineSpacing = 1f;

    [Header("VFX References")]
    [SerializeField, Tooltip("Visual Effect pour spawner les particules")]
    VisualEffect _vfx = null;

    [SerializeField, Tooltip("AlphabetFlipbookGenerator pour obtenir les indices")]
    AlphabetFlipbookGenerator _alphabetGenerator = null;

    [Header("Debug")]
    [SerializeField] bool _showDebugLogs = true;

    // État interne
    bool _isReading = false;
    int _currentLine = 0;
    Coroutine _readingCoroutine = null;

    void Update()
    {
        // Utilisation du nouveau Input System
        if (Keyboard.current != null && Keyboard.current[_startKey].wasPressedThisFrame && !_isReading)
        {
            StartPoem();
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
                Debug.LogWarning("[PoemReader] Poème déjà en cours de lecture !");
            return;
        }

        if (_vfx == null)
        {
            Debug.LogError("[PoemReader] VisualEffect non assigné !");
            return;
        }

        if (_alphabetGenerator == null)
        {
            Debug.LogError("[PoemReader] AlphabetFlipbookGenerator non assigné !");
            return;
        }

        if (_poemLines.Length == 0)
        {
            Debug.LogError("[PoemReader] Aucune ligne de poème définie !");
            return;
        }

        _currentLine = 0;
        _isReading = true;

        if (_showDebugLogs)
            Debug.Log($"[PoemReader] ▶ Démarrage du poème : {_poemLines.Length} lignes");

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
            Debug.Log("[PoemReader] ⏹ Poème arrêté");
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
                Debug.Log($"[PoemReader] Ligne {lineIndex + 1}/{_poemLines.Length}: \"{line}\"");

            // Afficher la ligne lettre par lettre
            yield return StartCoroutine(DisplayLineCoroutine(line, lineIndex));

            // Délai avant la ligne suivante (sauf pour la dernière)
            if (lineIndex < _poemLines.Length - 1)
            {
                if (_showDebugLogs)
                    Debug.Log($"[PoemReader] ⏸ Pause de {_lineDelay}s avant la ligne suivante...");

                yield return new WaitForSeconds(_lineDelay);
            }
        }

        // Fin du poème
        _isReading = false;
        if (_showDebugLogs)
            Debug.Log("[PoemReader] ✅ Poème terminé !");
    }

    /// <summary>
    /// Affiche une ligne lettre par lettre
    /// </summary>
    IEnumerator DisplayLineCoroutine(string line, int lineIndex)
    {
        for (int charIndex = 0; charIndex < line.Length; charIndex++)
        {
            char c = line[charIndex];

            // Ignorer les espaces (ou les afficher comme vide)
            if (c == ' ')
            {
                if (_showDebugLogs)
                    Debug.Log($"[PoemReader]   [espace]");

                yield return new WaitForSeconds(_letterDelay * 0.5f); // Espace plus court
                continue;
            }

            // Calculer la position de la lettre
            Vector3 position = CalculateLetterPosition(charIndex, lineIndex);

            // Spawner la particule avec la bonne lettre
            SpawnLetter(c, position);

            if (_showDebugLogs)
                Debug.Log($"[PoemReader]   '{c}' à {position}");

            // Attendre avant la prochaine lettre
            yield return new WaitForSeconds(_letterDelay);
        }
    }

    /// <summary>
    /// Calcule la position d'une lettre dans l'espace
    /// </summary>
    Vector3 CalculateLetterPosition(int charIndex, int lineIndex)
    {
        float x = _startPosition.x + (charIndex * _letterSpacing);
        float y = _startPosition.y - (lineIndex * _lineSpacing);
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Spawn une particule avec une lettre spécifique
    /// </summary>
    void SpawnLetter(char c, Vector3 position)
    {
        if (_vfx == null) return;

        // Obtenir l'index flipbook pour cette lettre
        float flipbookIndex = _alphabetGenerator.GetFlipbookIndex(char.ToUpper(c));

        // Configurer les attributs de spawn du VFX
        VFXEventAttribute eventAttribute = _vfx.CreateVFXEventAttribute();

        if (eventAttribute != null)
        {
            // Position de spawn
            eventAttribute.SetVector3("position", position);

            // Index de texture (si votre VFX expose cet attribut)
            eventAttribute.SetFloat("texIndex", flipbookIndex * _alphabetGenerator.TotalCells);

            // Envoyer l'événement de spawn
            _vfx.SendEvent("OnSpawn", eventAttribute);
        }
        else
        {
            Debug.LogWarning("[PoemReader] Impossible de créer VFXEventAttribute. Vérifiez la configuration du VFX Graph.");
        }
    }

    /// <summary>
    /// Réinitialise le lecteur
    /// </summary>
    [ContextMenu("Reset Reader")]
    public void ResetReader()
    {
        StopPoem();
        _currentLine = 0;

        if (_showDebugLogs)
            Debug.Log("[PoemReader] 🔄 Lecteur réinitialisé");
    }

    // Getters pour l'état actuel
    public bool IsReading => _isReading;
    public int CurrentLine => _currentLine;
    public int TotalLines => _poemLines.Length;
    public float Progress => _poemLines.Length > 0 ? (float)_currentLine / _poemLines.Length : 0f;
}

} // namespace OpticalFlowTest
