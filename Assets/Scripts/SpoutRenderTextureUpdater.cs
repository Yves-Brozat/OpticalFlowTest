using UnityEngine;
#if KLAK_SPOUT
using Klak.Spout;
#endif

namespace OpticalFlowTest {

/// <summary>
/// Updater générique pour n'importe quelle RenderTexture → Spout
/// Utile pour Datamosh, effets personnalisés, etc.
/// </summary>
public class SpoutRenderTextureUpdater : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] RenderTexture _sourceTexture = null;

    [Header("Options")]
    [SerializeField] bool _updateEveryFrame = true;

#if KLAK_SPOUT
    private SpoutSender _spoutSender;
#endif

    void Start()
    {
#if KLAK_SPOUT
        _spoutSender = GetComponent<SpoutSender>();
        if (_spoutSender == null)
        {
            Debug.LogError("SpoutRenderTextureUpdater nécessite SpoutSender !");
        }
#endif
    }

    void LateUpdate()
    {
#if KLAK_SPOUT
        if (_updateEveryFrame && _spoutSender != null && _sourceTexture != null)
        {
            if (_sourceTexture.IsCreated())
            {
                _spoutSender.sourceTexture = _sourceTexture;
            }
        }
#endif
    }

    /// <summary>
    /// Change la source texture dynamiquement
    /// </summary>
    public void SetSourceTexture(RenderTexture texture)
    {
        _sourceTexture = texture;
#if KLAK_SPOUT
        if (_spoutSender != null && texture != null)
        {
            _spoutSender.sourceTexture = texture;
        }
#endif
    }
}

} // namespace OpticalFlowTest
