using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;

namespace OpticalFlowTest {

/// <summary>
/// Affiche l'ImageSource dans une UI RawImage
/// Parfait pour un preview dans le coin de l'écran
/// </summary>
[RequireComponent(typeof(RawImage))]
public class ImageSourceUIDisplay : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] ImageSource _imageSource = null;

    [Header("Display Options")]
    [SerializeField, Tooltip("Affiche ou masque l'image")]
    bool _showImage = true;

    private RawImage _rawImage;

    void Start()
    {
        _rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (_imageSource == null || _rawImage == null) return;

        // Mettre à jour la texture
        if (_imageSource.AsTexture != null)
        {
            _rawImage.texture = _imageSource.AsTexture;
        }

        // Contrôler la visibilité
        _rawImage.enabled = _showImage;
    }

    public void SetShowImage(bool show)
    {
        _showImage = show;
        if (_rawImage != null)
            _rawImage.enabled = show;
    }
}

} // namespace OpticalFlowTest
