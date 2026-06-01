using UnityEngine;
using Klak.TestTools;

namespace OpticalFlowTest {

/// <summary>
/// Affiche simplement la texture de l'ImageSource sur un MeshRenderer
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class ImageSourceDisplay : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] ImageSource _imageSource = null;

    [Header("Display Options")]
    [SerializeField, Tooltip("Affiche ou masque l'image")]
    bool _showImage = true;

    private MeshRenderer _meshRenderer;
    private Material _material;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        // Créer un matériau avec le shader Unlit/Texture
        _material = new Material(Shader.Find("Unlit/Texture"));
        _meshRenderer.material = _material;
    }

    void OnDestroy()
    {
        if (_material != null)
            Destroy(_material);
    }

    void Update()
    {
        if (_imageSource == null || _material == null) return;

        // Mettre à jour la texture
        if (_imageSource.AsTexture != null)
        {
            _material.mainTexture = _imageSource.AsTexture;
        }

        // Contrôler la visibilité
        if (_meshRenderer != null)
        {
            _meshRenderer.enabled = _showImage;
        }
    }

    public void SetShowImage(bool show)
    {
        _showImage = show;
        if (_meshRenderer != null)
            _meshRenderer.enabled = show;
    }

    public void ToggleImage()
    {
        SetShowImage(!_showImage);
    }
}

} // namespace OpticalFlowTest
