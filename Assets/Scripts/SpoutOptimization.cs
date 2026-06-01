using UnityEngine;

namespace OpticalFlowTest {

/// <summary>
/// Configuration optimale pour Spout en production
/// Gère automatiquement les différences Editor vs Build
/// </summary>
public class SpoutOptimization : MonoBehaviour
{
    [Header("Performance")]
    [SerializeField] int _targetFramerate = 60;

    [Header("Build Settings")]
    [SerializeField, Tooltip("En build, minimiser réduit au lieu de mettre en pause")]
    bool _allowMinimize = true;

    void Awake()
    {
        ConfigureForSpout();
    }

    void ConfigureForSpout()
    {
        // ESSENTIEL : Permet l'exécution en arrière-plan
        Application.runInBackground = true;

        // Désactive VSync pour un framerate stable
        QualitySettings.vSyncCount = 0;

        // Force le framerate
        Application.targetFrameRate = _targetFramerate;

        #if UNITY_EDITOR
        // En Editor : Continue de tourner même si la fenêtre n'est pas focus
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
        Debug.Log("[Spout] Mode Editor : Unity continuera en arrière-plan");
        #else
        // En Build : Gestion spéciale
        if (_allowMinimize)
        {
            // Même minimisé, continue de tourner
            Debug.Log("[Spout] Mode Build : Application continue quand minimisée");
        }
        #endif

        // Log configuration
        Debug.Log($"=== Spout Optimization ===\n" +
                  $"Run In Background: {Application.runInBackground}\n" +
                  $"Target Framerate: {Application.targetFrameRate} FPS\n" +
                  $"VSync: Disabled\n" +
                  $"Platform: {Application.platform}");
    }

    #if UNITY_EDITOR
    void OnPlayModeChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.EnteredPlayMode)
        {
            // Réappliquer les settings au cas où
            Application.runInBackground = true;
            Application.targetFrameRate = _targetFramerate;
        }
    }

    void OnDestroy()
    {
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }
    #endif

    void OnApplicationFocus(bool hasFocus)
    {
        // Log pour debug
        string status = hasFocus ? "a gagné" : "a perdu";
        Debug.Log($"[Spout] Unity {status} le focus - Spout continue à {Application.targetFrameRate} FPS");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // En build, éviter la pause si possible
        if (pauseStatus && !_allowMinimize)
        {
            Debug.LogWarning("[Spout] Application en pause - Spout est arrêté");
        }
    }

    // API publique
    public void SetTargetFramerate(int fps)
    {
        _targetFramerate = Mathf.Clamp(fps, 1, 240);
        Application.targetFrameRate = _targetFramerate;
        Debug.Log($"[Spout] Framerate changé à {_targetFramerate} FPS");
    }
}

} // namespace OpticalFlowTest
