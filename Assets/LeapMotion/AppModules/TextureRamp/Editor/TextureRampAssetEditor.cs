using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TextureRampAsset))]
public class TextureRampAssetEditor : Editor {

  public override void OnInspectorGUI() {
    EditorGUI.BeginChangeCheck();

    SerializedProperty iterator = serializedObject.GetIterator();
    bool isFirst = true;

    while (iterator.NextVisible(isFirst)) {
      EditorGUILayout.PropertyField(iterator, true);
      isFirst = false;
    }

    if (EditorGUI.EndChangeCheck()) {
      serializedObject.ApplyModifiedProperties();
      TextureRampAsset asset = target as TextureRampAsset;
      asset.UpdateTextureAsset();
    }
  }

}
