using UnityEngine;
using System.Collections.Generic;

namespace OpticalFlowTest {

/// <summary>
/// Génère des textures pour chaque lettre de l'alphabet
/// Peut créer une Texture2DArray pour utilisation dans VFX Graph
/// </summary>
public class AlphabetTextureGenerator : MonoBehaviour
{
    [Header("Alphabet Settings")]
    [SerializeField] string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    [Header("Texture Settings")]
    [SerializeField] int _textureSize = 128;
    [SerializeField] Font _font = null;
    [SerializeField] int _fontSize = 100;
    [SerializeField] Color _textColor = Color.white;
    [SerializeField] Color _backgroundColor = Color.clear;
    [SerializeField] TextAnchor _alignment = TextAnchor.MiddleCenter;

    [Header("Output")]
    [SerializeField] bool _generateOnStart = true;
    [SerializeField] string _outputName = "AlphabetTextures";

    private Texture2DArray _textureArray;
    private List<Texture2D> _individualTextures;

    public Texture2DArray TextureArray => _textureArray;
    public List<Texture2D> IndividualTextures => _individualTextures;
    public int CharacterCount => _alphabet.Length;

    void Start()
    {
        if (_generateOnStart)
        {
            GenerateTextures();
        }
    }

    [ContextMenu("Generate Textures")]
    public void GenerateTextures()
    {
        Debug.Log($"[AlphabetGenerator] Génération de {_alphabet.Length} textures...");

        _individualTextures = new List<Texture2D>();

        // Créer une texture pour chaque caractère
        for (int i = 0; i < _alphabet.Length; i++)
        {
            char c = _alphabet[i];
            Texture2D texture = GenerateCharacterTexture(c);
            _individualTextures.Add(texture);
        }

        // Créer la Texture2DArray
        CreateTextureArray();

        Debug.Log($"[AlphabetGenerator] ✅ Généré : {_individualTextures.Count} textures");
    }

    Texture2D GenerateCharacterTexture(char character)
    {
        // Créer une RenderTexture temporaire
        RenderTexture rt = RenderTexture.GetTemporary(_textureSize, _textureSize, 0, RenderTextureFormat.ARGB32);
        RenderTexture.active = rt;

        // Effacer avec la couleur de fond
        GL.Clear(true, true, _backgroundColor);

        // Créer le style de texte
        GUIStyle style = new GUIStyle();
        style.font = _font;
        style.fontSize = _fontSize;
        style.normal.textColor = _textColor;
        style.alignment = _alignment;

        // Dessiner le caractère
        GUI.BeginGroup(new Rect(0, 0, _textureSize, _textureSize));
        GUI.Label(new Rect(0, 0, _textureSize, _textureSize), character.ToString(), style);
        GUI.EndGroup();

        // Copier vers Texture2D
        Texture2D texture = new Texture2D(_textureSize, _textureSize, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, _textureSize, _textureSize), 0, 0);
        texture.Apply();
        texture.name = $"{_outputName}_{character}";

        // Nettoyer
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return texture;
    }

    void CreateTextureArray()
    {
        if (_individualTextures == null || _individualTextures.Count == 0)
        {
            Debug.LogError("[AlphabetGenerator] Aucune texture à convertir en array");
            return;
        }

        // Créer la Texture2DArray
        _textureArray = new Texture2DArray(
            _textureSize, 
            _textureSize, 
            _individualTextures.Count, 
            TextureFormat.ARGB32, 
            false
        );

        // Copier chaque texture dans l'array
        for (int i = 0; i < _individualTextures.Count; i++)
        {
            Graphics.CopyTexture(_individualTextures[i], 0, 0, _textureArray, i, 0);
        }

        _textureArray.Apply();
        _textureArray.name = _outputName;

        Debug.Log($"[AlphabetGenerator] Texture2DArray créée : {_individualTextures.Count} slices");
    }

    /// <summary>
    /// Obtenir l'index d'un caractère dans l'alphabet
    /// </summary>
    public int GetCharacterIndex(char c)
    {
        return _alphabet.IndexOf(c);
    }

    /// <summary>
    /// Obtenir un caractère aléatoire
    /// </summary>
    public char GetRandomCharacter()
    {
        return _alphabet[Random.Range(0, _alphabet.Length)];
    }

    /// <summary>
    /// Obtenir un index aléatoire
    /// </summary>
    public int GetRandomIndex()
    {
        return Random.Range(0, _alphabet.Length);
    }

    void OnDestroy()
    {
        // Nettoyer les textures
        if (_individualTextures != null)
        {
            foreach (var tex in _individualTextures)
            {
                if (tex != null) Destroy(tex);
            }
        }

        if (_textureArray != null)
        {
            Destroy(_textureArray);
        }
    }
}

} // namespace OpticalFlowTest
