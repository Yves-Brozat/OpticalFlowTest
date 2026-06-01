using UnityEngine;
#if KLAK_SPOUT
using Klak.Spout;
#endif

namespace OpticalFlowTest {

/// <summary>
/// Updater pour OpticalFlowEstimator → Spout
/// </summary>
public class SpoutOpticalFlowUpdater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] OpticalFlowEstimator _opticalFlow = null;

#if KLAK_SPOUT
    private SpoutSender _spoutSender;
#endif

    void Start()
    {
#if KLAK_SPOUT
        _spoutSender = GetComponent<SpoutSender>();
        if (_spoutSender == null)
        {
            Debug.LogError("SpoutOpticalFlowUpdater nécessite SpoutSender !");
        }
#endif
    }

    void LateUpdate()
    {
#if KLAK_SPOUT
        if (_spoutSender != null && _opticalFlow != null)
        {
            var texture = _opticalFlow.AsRenderTexture;
            if (texture != null && texture.IsCreated())
            {
                _spoutSender.sourceTexture = texture;
            }
        }
#endif
    }
}

} // namespace OpticalFlowTest
