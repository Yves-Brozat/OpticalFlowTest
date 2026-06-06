using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Klak.TestTools;

namespace OpticalFlowTest {

public sealed class OpticalFlowEstimator : MonoBehaviour
{
    #region Scene object references

    [SerializeField] ImageSource _source = null;

    #endregion

    #region Editable properties

    [SerializeField, Range(0f, 1f), Tooltip("Seuil de détection de mouvement (filtre le bruit)")]
    float _motionThreshold = 0.001f;

    [SerializeField, Tooltip("Valeur initiale du motion threshold")]
    float _initialMotionThreshold = 0.001f;

    [SerializeField, Range(0f, 1f), Tooltip("Incrément/décrément du motion threshold")]
    float _thresholdStep = 0.001f;

    [SerializeField, Tooltip("Touche pour diminuer le threshold")]
    Key _decreaseThresholdKey = Key.S;

    [SerializeField, Tooltip("Touche pour augmenter le threshold")]
    Key _increaseThresholdKey = Key.D;

    [SerializeField, Tooltip("Touche pour réinitialiser le threshold")]
    Key _resetKey = Key.R;

    [SerializeField, Range(0.1f, 5f), Tooltip("Amplitude du flow (multiplie la force du mouvement)")]
    float _flowAmplitude = 1f;

    [SerializeField, Tooltip("Valeur initiale du flow amplitude")]
    float _initialFlowAmplitude = 1f;

    [SerializeField, Tooltip("Incrément/décrément du flow amplitude")]
    float _amplitudeStep = 0.1f;

    [SerializeField, Tooltip("Touche pour diminuer l'amplitude")]
    Key _decreaseAmplitudeKey = Key.F;

    [SerializeField, Tooltip("Touche pour augmenter l'amplitude")]
    Key _increaseAmplitudeKey = Key.G;

    [SerializeField, Range(0f, 0.95f), Tooltip("Lissage temporel (0 = pas de lissage, 0.9+ = très smooth)")]
    float _smoothness = 0f;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] ComputeShader _diffDetector = null;
    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Public accessors

    public RenderTexture AsRenderTexture => _output.flow;

    #endregion

    #region Private members

    Blitter _blitter;
    (RenderTexture prev, RenderTexture cur) _buffer;
    (RenderTexture grad, RenderTexture flow) _output;
    GraphicsBuffer _diffMask;
    RenderTexture _smoothedFlow; // Pour le lissage temporel

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _blitter = new Blitter(_shader);
        _buffer.prev = RTUtil.AllocColor(Config.FlowDims);
        _buffer.cur  = RTUtil.AllocColor(Config.FlowDims);
        _output.grad = RTUtil.AllocHalf4(Config.FlowDims);
        _output.flow = RTUtil.AllocHalf2(Config.FlowDims);
        _smoothedFlow = RTUtil.AllocHalf2(Config.FlowDims);
        _diffMask = GpuBufferUtil.Alloc<float4>(1);

        // Initialiser les valeurs
        _motionThreshold = _initialMotionThreshold;
        _flowAmplitude = _initialFlowAmplitude;
    }

    void OnDestroy()
    {
        _blitter.Dispose();
        Destroy(_buffer.prev);
        Destroy(_buffer.cur);
        Destroy(_output.grad);
        Destroy(_output.flow);
        Destroy(_smoothedFlow);
        _diffMask.Release();
    }

    void Update()
    {
        // Contrôle du motion threshold et flow amplitude
        if (Keyboard.current != null)
        {
            if (Keyboard.current[_decreaseThresholdKey].wasPressedThisFrame)
            {
                _motionThreshold = Mathf.Max(0f, _motionThreshold - _thresholdStep);
                Debug.Log($"[OpticalFlowEstimator] Motion Threshold: {_motionThreshold:F4}");
            }

            if (Keyboard.current[_increaseThresholdKey].wasPressedThisFrame)
            {
                _motionThreshold = Mathf.Min(1f, _motionThreshold + _thresholdStep);
                Debug.Log($"[OpticalFlowEstimator] Motion Threshold: {_motionThreshold:F4}");
            }

            if (Keyboard.current[_decreaseAmplitudeKey].wasPressedThisFrame)
            {
                _flowAmplitude = Mathf.Max(0.1f, _flowAmplitude - _amplitudeStep);
                Debug.Log($"[OpticalFlowEstimator] Flow Amplitude: {_flowAmplitude:F2}");
            }

            if (Keyboard.current[_increaseAmplitudeKey].wasPressedThisFrame)
            {
                _flowAmplitude = Mathf.Min(5f, _flowAmplitude + _amplitudeStep);
                Debug.Log($"[OpticalFlowEstimator] Flow Amplitude: {_flowAmplitude:F2}");
            }

            if (Keyboard.current[_resetKey].wasPressedThisFrame)
            {
                _motionThreshold = _initialMotionThreshold;
                _flowAmplitude = _initialFlowAmplitude;
                Debug.Log($"[OpticalFlowEstimator] Reset - Threshold: {_motionThreshold:F4}, Amplitude: {_flowAmplitude:F2}");
            }
        }

        Graphics.Blit(_source.AsRenderTexture, _buffer.cur);

        _diffDetector.SetTexture(0, "Previous", _buffer.prev);
        _diffDetector.SetTexture(0, "Current", _buffer.cur);
        _diffDetector.SetBuffer(0, "Output", _diffMask);
        _diffDetector.Dispatch(0, 1, 1, 1);

        _blitter.Material.SetTexture("_PrevTex", _buffer.prev);
        _blitter.Material.SetFloat("_Threshold", _motionThreshold);
        _blitter.Material.SetFloat("_Amplitude", _flowAmplitude);
        _blitter.Run(_buffer.cur, _output.grad, 0);

        _blitter.Material.SetBuffer("_DiffMask", _diffMask);
        _blitter.Run(_output.grad, _output.flow, 1);

        // Appliquer le lissage temporel si activé
        if (_smoothness > 0.001f)
        {
            _blitter.Material.SetTexture("_PrevFlow", _smoothedFlow);
            _blitter.Material.SetFloat("_Smoothness", _smoothness);
            _blitter.Run(_output.flow, _smoothedFlow, 2); // Pass 2 = smoothing
            Graphics.Blit(_smoothedFlow, _output.flow); // Copier le résultat lissé
        }

        _buffer = (_buffer.cur, _buffer.prev);
    }

    #endregion
}

} // namespace OpticalFlowTest
