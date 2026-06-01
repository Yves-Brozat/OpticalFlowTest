using UnityEngine;

namespace OpticalFlowTest {

/// <summary>
/// Force Unity à continuer de tourner en arrière-plan
/// Essentiel pour Spout quand Unity n'est pas au premier plan
/// </summary>
public class RunInBackground : MonoBehaviour
{
    [Header("Background Execution")]
    [SerializeField, Tooltip("Permet à Unity de tourner en arrière-plan")]
    bool _runInBackground = true;

    [SerializeField, Tooltip("Framerate cible quand en arrière-plan")]
    [Range(30, 120)]
    int _backgroundFramerate = 60;

    [Header("Optimisation")]
    [SerializeField, Tooltip("Désactive VSync pour un framerate stable")]
    bool _disableVSync = true;

    void Awake()
    {
        // Force Unity à continuer en arrière-plan
        Application.runInBackground = _runInBackground;

        // Désactive VSync si demandé
        if (_disableVSync)
        {
            QualitySettings.vSyncCount = 0;
        }

        // Force le framerate
        Application.targetFrameRate = _backgroundFramerate;

        Debug.Log($"[RunInBackground] Configuration :\n" +
                  $"- Run In Background: {_runInBackground}\n" +
                  $"- Target Framerate: {_backgroundFramerate} FPS\n" +
                  $"- VSync: {!_disableVSync}");
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Log quand Unity perd/gagne le focus
        Debug.Log($"[RunInBackground] Application focus: {hasFocus} - Spout devrait continuer à fonctionner");
    }

    public void SetBackgroundFramerate(int framerate)
    {
        _backgroundFramerate = Mathf.Clamp(framerate, 1, 120);
        Application.targetFrameRate = _backgroundFramerate;
    }
}

} // namespace OpticalFlowTest
