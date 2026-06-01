using UnityEngine;

namespace OpticalFlowTest {

/// <summary>
/// Force un framerate stable pour Spout
/// Attachez ce script n'importe où dans la scène
/// </summary>
public class ForceFramerate : MonoBehaviour
{
    [SerializeField, Range(30, 120)]
    int _targetFramerate = 60;

    [SerializeField]
    bool _disableVSync = true;

    void Awake()
    {
        if (_disableVSync)
            QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = _targetFramerate;

        Debug.Log($"Framerate forcé à {_targetFramerate} FPS (VSync: {!_disableVSync})");
    }
}

} // namespace OpticalFlowTest
