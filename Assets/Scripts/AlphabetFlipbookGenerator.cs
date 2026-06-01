using UnityEngine;

namespace OpticalFlowTest {

/// <summary>
/// Génère un atlas de textures (FlipBook) pour VFX Graph
/// Compatible avec le système FlipBook natif du VFX Graph
/// </summary>
public class AlphabetFlipbookGenerator : MonoBehaviour
{
    [Header("Alphabet Settings")]
    [SerializeField] string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    [Header("Atlas Settings")]
    [SerializeField] int _columns = 6; // 6 colonnes
    [SerializeField] int _rows = 6;    // 6 lignes = 36 caractères max
    [SerializeField] int _cellSize = 128; // Taille de chaque lettre

    [Header("Font Settings")]
    [SerializeField] Font _font = null;
    [SerializeField, Range(10, 500)] int _fontSize = 100;
    [SerializeField] FontStyle _fontStyle = FontStyle.Bold;
    [SerializeField] Color _textColor = Color.white;
    [SerializeField] Color _backgroundColor = Color.clear;

    [Header("Output")]
    [SerializeField] bool _generateOnStart = false;
    [SerializeField] string _outputName = "AlphabetFlipbook";

    [Header("Result")]
    [SerializeField] Texture2D _generatedTexture = null;

    public Texture2D GeneratedTexture => _generatedTexture;
    public int Columns => _columns;
    public int Rows => _rows;
    public int TotalCells => _columns * _rows;

    void Start()
    {
        if (_generateOnStart)
        {
            GenerateFlipbook();
        }
    }

    [ContextMenu("Generate Flipbook Atlas")]
    public void GenerateFlipbook()
    {
        int atlasWidth = _columns * _cellSize;
        int atlasHeight = _rows * _cellSize;

        Debug.Log($"[AlphabetFlipbook] Génération atlas {atlasWidth}x{atlasHeight} ({_columns}x{_rows} grid)");

        // Créer la texture atlas
        _generatedTexture = new Texture2D(atlasWidth, atlasHeight, TextureFormat.ARGB32, true);
        _generatedTexture.name = _outputName;

        // Remplir avec la couleur de fond
        Color[] bgPixels = new Color[atlasWidth * atlasHeight];
        for (int i = 0; i < bgPixels.Length; i++)
            bgPixels[i] = _backgroundColor;
        _generatedTexture.SetPixels(bgPixels);

        // Générer chaque caractère
        int charCount = Mathf.Min(_alphabet.Length, TotalCells);

        for (int i = 0; i < charCount; i++)
        {
            char c = _alphabet[i];

            // Calculer la position dans la grille
            int col = i % _columns;
            int row = i / _columns;

            // Générer la texture du caractère
            Texture2D charTex = GenerateCharacterTexture(c);

            // Copier dans l'atlas
            int xOffset = col * _cellSize;
            int yOffset = ((_rows - 1) - row) * _cellSize; // Inverser Y

            Color[] pixels = charTex.GetPixels();
            _generatedTexture.SetPixels(xOffset, yOffset, _cellSize, _cellSize, pixels);

            // Nettoyer
            Destroy(charTex);
        }

        // Appliquer et configurer
        _generatedTexture.Apply(true);
        _generatedTexture.filterMode = FilterMode.Bilinear;
        _generatedTexture.wrapMode = TextureWrapMode.Clamp;

        Debug.Log($"[AlphabetFlipbook] ✅ Atlas généré : {charCount} caractères");

#if UNITY_EDITOR
        // Sauvegarder automatiquement
        SaveTextureToFile();
#endif
    }

    Texture2D GenerateCharacterTexture(char character)
    {
        // Créer une texture 2D directement avec Graphics
        Texture2D texture = new Texture2D(_cellSize, _cellSize, TextureFormat.ARGB32, false);

        // Remplir avec le fond transparent
        Color[] pixels = new Color[_cellSize * _cellSize];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = _backgroundColor;
        texture.SetPixels(pixels);

        // Créer un RenderTexture pour le rendu du texte
        RenderTexture rt = RenderTexture.GetTemporary(_cellSize, _cellSize, 0, RenderTextureFormat.ARGB32);
        RenderTexture previousRT = RenderTexture.active;
        RenderTexture.active = rt;

        // Clear avec fond
        GL.Clear(true, true, _backgroundColor);

        // Créer un GameObject temporaire dans une position unique très loin
        Vector3 uniquePos = new Vector3(1000 + Random.Range(0f, 100f), 1000 + Random.Range(0f, 100f), 0);

        GameObject tempGO = new GameObject("TempChar_" + character);
        tempGO.hideFlags = HideFlags.HideAndDontSave;
        tempGO.transform.position = uniquePos;

        TextMesh textMesh = tempGO.AddComponent<TextMesh>();
        textMesh.text = character.ToString();
        textMesh.font = _font;
        textMesh.fontSize = _fontSize;
        textMesh.fontStyle = _fontStyle;
        textMesh.color = _textColor;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.characterSize = 0.1f;

        MeshRenderer renderer = tempGO.GetComponent<MeshRenderer>();
        if (_font != null)
            renderer.material = _font.material;

        // Créer une caméra qui regarde UNIQUEMENT ce TextMesh
        GameObject camGO = new GameObject("TempCam_" + character);
        camGO.hideFlags = HideFlags.HideAndDontSave;
        camGO.transform.position = uniquePos + new Vector3(0, 0, -5);
        camGO.transform.LookAt(tempGO.transform);

        Camera cam = camGO.AddComponent<Camera>();
        cam.targetTexture = rt;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = _backgroundColor;
        cam.orthographic = true;
        cam.orthographicSize = 1.1f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 10f;
        cam.cullingMask = 1 << tempGO.layer; // Seul ce layer

        // RENDU IMMÉDIAT
        cam.Render();

        // Copier dans la texture
        texture.ReadPixels(new Rect(0, 0, _cellSize, _cellSize), 0, 0);
        texture.Apply();

        // Nettoyer IMMÉDIATEMENT
        RenderTexture.active = previousRT;
        RenderTexture.ReleaseTemporary(rt);
        DestroyImmediate(tempGO);
        DestroyImmediate(camGO);

        return texture;
    }

    /// <summary>
    /// Obtenir l'index FlipBook d'un caractère
    /// </summary>
    public float GetFlipbookIndex(char c)
    {
        int index = _alphabet.IndexOf(c);
        if (index < 0) index = 0;

        // Normaliser entre 0 et 1
        return (float)index / (float)TotalCells;
    }

    /// <summary>
    /// Obtenir un index FlipBook aléatoire (normalisé 0-1)
    /// </summary>
    public float GetRandomFlipbookIndex()
    {
        int randomIndex = Random.Range(0, Mathf.Min(_alphabet.Length, TotalCells));
        return (float)randomIndex / (float)TotalCells;
    }

#if UNITY_EDITOR
    [ContextMenu("Save Texture to File")]
    void SaveTextureToFile()
    {
        if (_generatedTexture == null)
        {
            Debug.LogError("Générez d'abord la texture !");
            return;
        }

        byte[] bytes = _generatedTexture.EncodeToPNG();
        string path = $"Assets/{_outputName}.png";
        System.IO.File.WriteAllBytes(path, bytes);

        UnityEditor.AssetDatabase.Refresh();

        // Configurer les import settings
        UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
        if (importer != null)
        {
            importer.textureType = UnityEditor.TextureImporterType.Default;
            importer.alphaSource = UnityEditor.TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.maxTextureSize = 4096;

            UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
        }

        Debug.Log($"✅ Texture sauvegardée : {path}");
        Debug.Log($"📊 Configuration FlipBook pour VFX Graph :");
        Debug.Log($"   - Flip Book Size: X={_columns}, Y={_rows}");
        Debug.Log($"   - Texture: {path}");
    }
#endif
}

} // namespace OpticalFlowTest
