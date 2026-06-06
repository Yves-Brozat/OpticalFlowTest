using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

namespace OpticalFlowTest {

/// <summary>
/// Contrôleur pour l'intensité des turbulences du FlowVFX
/// Z : Augmenter l'intensité de 1
/// S : Diminuer l'intensité de 1
/// R : Réinitialiser à 1 (synchronisé avec PoemReaderPlaylist)
/// </summary>
public class FlowVFXTurbulenceController : MonoBehaviour
{
    [Header("VFX Reference")]
    [SerializeField, Tooltip("Référence au Visual Effect FlowVFX")]
    VisualEffect _visualEffect = null;

    [Header("Turbulence Settings")]
    [SerializeField, Tooltip("Nom de la propriété VFX pour l'intensité des turbulences")]
    string _turbulencePropertyName = "TurbulenceIntensity";

    [SerializeField, Tooltip("Valeur initiale de l'intensité")]
    float _initialIntensity = 1f;

    [SerializeField, Tooltip("Valeur minimale de l'intensité")]
    float _minIntensity = 0f;

    [SerializeField, Tooltip("Valeur maximale de l'intensité")]
    float _maxIntensity = 100f;

    [SerializeField, Tooltip("Incrément/décrément par appui de touche")]
    float _intensityStep = 1f;

    [Header("Input Keys")]
    [SerializeField, Tooltip("Touche pour augmenter l'intensité")]
    Key _increaseKey = Key.Z;

    [SerializeField, Tooltip("Touche pour diminuer l'intensité")]
    Key _decreaseKey = Key.S;

    [SerializeField, Tooltip("Touche pour réinitialiser l'intensité")]
    Key _resetKey = Key.R;

    [Header("Debug")]
    [SerializeField] bool _showDebugLogs = true;

    // Valeur actuelle
    private float _currentIntensity;

    void Start()
    {
        // Initialiser l'intensité
        _currentIntensity = _initialIntensity;
        UpdateVFXIntensity();

        if (_showDebugLogs)
            Debug.Log($"[FlowVFXTurbulenceController] Initialisé avec intensité = {_currentIntensity}");
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Augmenter l'intensité avec Z
        if (Keyboard.current[_increaseKey].wasPressedThisFrame)
        {
            IncreaseIntensity();
        }

        // Diminuer l'intensité avec S
        if (Keyboard.current[_decreaseKey].wasPressedThisFrame)
        {
            DecreaseIntensity();
        }

        // Réinitialiser avec R
        if (Keyboard.current[_resetKey].wasPressedThisFrame)
        {
            ResetIntensity();
        }
    }

    /// <summary>
    /// Augmente l'intensité des turbulences
    /// </summary>
    public void IncreaseIntensity()
    {
        _currentIntensity = Mathf.Min(_currentIntensity + _intensityStep, _maxIntensity);
        UpdateVFXIntensity();

        if (_showDebugLogs)
            Debug.Log($"[FlowVFXTurbulenceController] ⬆ Intensité augmentée : {_currentIntensity}");
    }

    /// <summary>
    /// Diminue l'intensité des turbulences
    /// </summary>
    public void DecreaseIntensity()
    {
        _currentIntensity = Mathf.Max(_currentIntensity - _intensityStep, _minIntensity);
        UpdateVFXIntensity();

        if (_showDebugLogs)
            Debug.Log($"[FlowVFXTurbulenceController] ⬇ Intensité diminuée : {_currentIntensity}");
    }

    /// <summary>
    /// Réinitialise l'intensité à la valeur initiale
    /// </summary>
    public void ResetIntensity()
    {
        _currentIntensity = _initialIntensity;
        UpdateVFXIntensity();

        if (_showDebugLogs)
            Debug.Log($"[FlowVFXTurbulenceController] 🔄 Intensité réinitialisée : {_currentIntensity}");
    }

    /// <summary>
    /// Met à jour la propriété VFX
    /// </summary>
    void UpdateVFXIntensity()
    {
        if (_visualEffect == null)
        {
            if (_showDebugLogs)
                Debug.LogWarning("[FlowVFXTurbulenceController] VisualEffect non assigné !");
            return;
        }

        if (!_visualEffect.HasFloat(_turbulencePropertyName))
        {
            if (_showDebugLogs)
                Debug.LogWarning($"[FlowVFXTurbulenceController] La propriété '{_turbulencePropertyName}' n'existe pas dans le VFX !");
            return;
        }

        _visualEffect.SetFloat(_turbulencePropertyName, _currentIntensity);
    }

    /// <summary>
    /// Récupère l'intensité actuelle
    /// </summary>
    public float GetCurrentIntensity()
    {
        return _currentIntensity;
    }

    /// <summary>
    /// Définit manuellement l'intensité
    /// </summary>
    public void SetIntensity(float intensity)
    {
        _currentIntensity = Mathf.Clamp(intensity, _minIntensity, _maxIntensity);
        UpdateVFXIntensity();

        if (_showDebugLogs)
            Debug.Log($"[FlowVFXTurbulenceController] 🎚 Intensité définie : {_currentIntensity}");
    }
}

} // namespace OpticalFlowTest
