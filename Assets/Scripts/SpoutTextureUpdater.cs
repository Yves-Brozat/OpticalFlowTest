using UnityEngine;
using Klak.TestTools;
#if KLAK_SPOUT
using Klak.Spout;
#endif

namespace OpticalFlowTest {

/// <summary>
/// Helper simple pour forcer la mise à jour de SpoutSender
/// Utilisez SpoutSender natif + ce helper pour résoudre le freeze
/// </summary>
public class SpoutTextureUpdater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ImageSource _imageSource = null;

#if KLAK_SPOUT
    private SpoutSender _spoutSender;
#endif

    void Start()
    {
#if KLAK_SPOUT
        _spoutSender = GetComponent<SpoutSender>();
        if (_spoutSender == null)
        {
            Debug.LogError("SpoutTextureUpdater nécessite un composant SpoutSender sur le même GameObject !");
        }
#endif
    }

    void LateUpdate()
    {
#if KLAK_SPOUT
        if (_spoutSender != null && _imageSource != null)
        {
            var texture = _imageSource.AsRenderTexture;
            if (texture != null && texture.IsCreated())
            {
                // Force la mise à jour de la texture source
                _spoutSender.sourceTexture = texture;
            }
        }
#endif
    }
}

} // namespace OpticalFlowTest
