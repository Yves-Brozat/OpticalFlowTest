using UnityEngine;
using System.Collections.Generic;

namespace OpticalFlowTest {

/// <summary>
/// Version améliorée avec rendu via Material et RenderTexture
/// Meilleure qualité pour les lettres
/// </summary>
public class AlphabetTextureGeneratorPro : MonoBehaviour
{
    [Header("Alphabet Settings")]
    [SerializeField] string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    [SerializeField, Tooltip("Inclure les minuscules")]
    bool _includeLowercase = false;

    [Header("Texture Settings")]
    [SerializeField] int _textureSize = 256;
    [SerializeField] Font _font = null;
    [SerializeField, Range(10, 500)] int _fontSize = 200;
    [SerializeField] FontStyle _fontStyle = FontStyle.Bold;

    [Header("Colors")]
    [SerializeField] Color _textColor = Color.white;
    [SerializeField] Color _backgroundColor = Color.clear;
    [SerializeField, Range(0f, 1f)] float _antiAliasing = 1f;

    [Header("Padding & Margins")]
    [SerializeField, Range(0f, 0.5f)] float _padding = 0.1f;

    [Header("Output")]
    [SerializeField] bool _generateOnStart = true;
    [SerializeField] string _outputName = "AlphabetTextures";

    [Header("Debug")]
    [SerializeField] bool _showDebugInfo = true;

    private Texture2DArray _textureArray;
    private Dictionary<char, int> _charToIndex;
    private string _fullAlphabet;

    public Texture2DArray TextureArray => _textureArray;
    public int CharacterCount => _fullAlphabet?.Length ?? 0;

    void Start()
    {
        if (_generateOnStart)
        {
            GenerateTextures();
        }
    }

    [ContextMenu("Generate Alphabet Textures")]
    public void GenerateTextures()
    {
        // Construire l'alphabet complet
        _fullAlphabet = _alphabet;
        if (_includeLowercase)
        {
            _fullAlphabet += _alphabet.ToLower();
        }

        if (_showDebugInfo)
            Debug.Log($"[AlphabetPro] Génération de {_fullAlphabet.Length} caractères : {_fullAlphabet}");

        // Générer les textures
        List<Texture2D> textures = new List<Texture2D>();
        _charToIndex = new Dictionary<char, int>();

        for (int i = 0; i < _fullAlphabet.Length; i++)
        {
            char c = _fullAlphabet[i];
            Texture2D tex = GenerateCharacterTexture(c);
            textures.Add(tex);
            _charToIndex[c] = i;
        }

        // Créer la Texture2DArray
        CreateTextureArray(textures);

        if (_showDebugInfo)
        {
            Debug.Log($"[AlphabetPro] ✅ Généré {textures.Count} textures");
            Debug.Log($"[AlphabetPro] Texture2DArray : {_textureSize}x{_textureSize} x {textures.Count}");
            Debug.Log($"[AlphabetPro] Format : {_textureArray.format}");
        }

        // Nettoyer les textures individuelles
        foreach (var tex in textures)
        {
            Destroy(tex);
        }
    }

    Texture2D GenerateCharacterTexture(char character)
    {
        // Créer une RenderTexture avec anti-aliasing
        int aaLevel = Mathf.RoundToInt(_antiAliasing * 8);
        RenderTexture rt = new RenderTexture(_textureSize, _textureSize, 0, RenderTextureFormat.ARGB32);
        rt.antiAliasing = Mathf.Max(1, aaLevel);

        RenderTexture previousRT = RenderTexture.active;
        RenderTexture.active = rt;

        // Effacer avec la couleur de fond
        GL.Clear(true, true, _backgroundColor);

        // Dessiner le texte avec TextMesh
        GameObject tempGO = new GameObject("TempChar");
        tempGO.hideFlags = HideFlags.HideAndDontSave;

        TextMesh textMesh = tempGO.AddComponent<TextMesh>();
        textMesh.text = character.ToString();
        textMesh.font = _font;
        textMesh.fontSize = _fontSize;
        textMesh.fontStyle = _fontStyle;
        textMesh.color = _textColor;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.characterSize = 0.1f;

        // Obtenir le renderer
        MeshRenderer renderer = tempGO.GetComponent<MeshRenderer>();
        if (_font != null)
        {
            renderer.material = _font.material;
        }

        // Positionner la caméra
        GameObject camGO = new GameObject("TempCam");
        camGO.hideFlags = HideFlags.HideAndDontSave;
        Camera cam = camGO.AddComponent<Camera>();
        cam.targetTexture = rt;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = _backgroundColor;
        cam.orthographic = true;
        cam.orthographicSize = 1f + _padding;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 10f;

        tempGO.transform.position = new Vector3(0, 0, 1);
        camGO.transform.position = Vector3.zero;
        camGO.transform.LookAt(tempGO.transform);

        // Rendre
        cam.Render();

        // Copier vers Texture2D
        Texture2D texture = new Texture2D(_textureSize, _textureSize, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, _textureSize, _textureSize), 0, 0);
        texture.Apply();
        texture.name = $"{_outputName}_{character}";

        // Nettoyer
        RenderTexture.active = previousRT;
        Destroy(tempGO);
        Destroy(camGO);
        Destroy(rt);

        return texture;
    }

    void CreateTextureArray(List<Texture2D> textures)
    {
        if (textures == null || textures.Count == 0)
        {
            Debug.LogError("[AlphabetPro] Aucune texture à convertir");
            return;
        }

        _textureArray = new Texture2DArray(
            _textureSize,
            _textureSize,
            textures.Count,
            TextureFormat.ARGB32,
            true // mipmaps
        );

        _textureArray.filterMode = FilterMode.Bilinear;
        _textureArray.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < textures.Count; i++)
        {
            Graphics.CopyTexture(textures[i], 0, 0, _textureArray, i, 0);
        }

        _textureArray.Apply(updateMipmaps: true, makeNoLongerReadable: false);
        _textureArray.name = _outputName;
    }

    /// <summary>
    /// Obtenir l'index d'un caractère
    /// </summary>
    public int GetCharacterIndex(char c)
    {
        return _charToIndex != null && _charToIndex.ContainsKey(c) ? _charToIndex[c] : 0;
    }

    /// <summary>
    /// Obtenir un index aléatoire
    /// </summary>
    public int GetRandomIndex()
    {
        return Random.Range(0, CharacterCount);
    }

    /// <summary>
    /// Obtenir un caractère aléatoire
    /// </summary>
    public char GetRandomCharacter()
    {
        return _fullAlphabet[GetRandomIndex()];
    }

    void OnDestroy()
    {
        if (_textureArray != null)
        {
            Destroy(_textureArray);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Save Texture Array to Asset")]
    void SaveTextureArrayToAsset()
    {
        if (_textureArray == null)
        {
            Debug.LogError("Générez d'abord les textures !");
            return;
        }

        string path = $"Assets/{_outputName}.asset";
        UnityEditor.AssetDatabase.CreateAsset(_textureArray, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"Texture2DArray sauvegardée : {path}");
    }
#endif
}

} // namespace OpticalFlowTest
