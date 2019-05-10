using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Leap.Unity.Attributes;
using System.IO;

public class TextureRampAsset : ScriptableObject {

  [Tooltip("This gradient will be converted to a 1D texture for use in a shader.")]
  [SerializeField]
  private Gradient _gradient;

  [SerializeField]
  private TextureWrapMode _wrapMode = TextureWrapMode.Clamp;

  [Incrementable]
  [MinValue(1)]
  [MaxValue(4096)]
  [SerializeField]
  private int _textureResolution = 128;

  [SerializeField]
  private FilterMode _filterMode = FilterMode.Bilinear;

  [HideInInspector]
  [SerializeField]
  private Texture2D _texture;

  public void UpdateTextureAsset() {
    _texture.Resize(_textureResolution, 1);
#if UNITY_EDITOR
    _texture.name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
#endif

    Color32[] pixels = new Color32[_textureResolution];

    if (_gradient == null) {
      return;
    }

    for (int i = 0; i < pixels.Length; i++) {
      float percent = i / (pixels.Length - 1.0f);
      pixels[i] = _gradient.Evaluate(percent);
    }

    _texture.Resize(_textureResolution, 1);
    _texture.wrapMode = _wrapMode;
    _texture.filterMode = _filterMode;
#if UNITY_EDITOR
    _texture.alphaIsTransparency = true;
#endif
    _texture.SetPixels32(pixels);
    _texture.Apply();
  }

#if UNITY_EDITOR
  private const string DEFAULT_ASSET_NAME = "TextureRamp.asset";

  [MenuItem("Assets/Create/Texture Ramp", priority = 305)]
  private static void createNewTextureRamp(MenuCommand command) {
    string path = "Assets";

    foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
      path = AssetDatabase.GetAssetPath(obj);
      if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
        path = Path.GetDirectoryName(path);
        break;
      }
    }

    path = Path.Combine(path, DEFAULT_ASSET_NAME);
    path = AssetDatabase.GenerateUniqueAssetPath(path);

    TextureRampAsset asset = CreateInstance<TextureRampAsset>();
    AssetDatabase.CreateAsset(asset, path);

    Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
    tex.name = "RampTexture";
    tex.alphaIsTransparency = true;
    asset._texture = tex;
    AssetDatabase.AddObjectToAsset(tex, asset);

    AssetDatabase.SaveAssets();

    Selection.activeObject = asset;
  }
#endif
}
