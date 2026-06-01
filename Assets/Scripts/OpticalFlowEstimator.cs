using Unity.Mathematics;
using UnityEngine;
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

    [SerializeField, Range(0.1f, 5f), Tooltip("Amplitude du flow (multiplie la force du mouvement)")]
    float _flowAmplitude = 1f;

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
