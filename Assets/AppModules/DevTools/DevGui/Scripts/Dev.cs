using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.DevGui {
  using Query;

  public class Dev : MonoBehaviour {
    private static bool _enabled = false;
    private static Vector2 _scroll;

    private static Dictionary<object, List<DevElement>> _registered;

    private static HashSet<string> _expandedCategories;
    private static List<CategoryInfo> _categories;

    private class CategoryInfo {
      public string name;
      public List<DevElementInfo> elementInfo = new List<DevElementInfo>();

      public CategoryInfo(string name) {
        this.name = name;
      }
    }

    private class DevElementInfo {
      public List<object> targets;
      public DevElement element;
    }

    static Dev() {
      _registered = new Dictionary<object, List<DevElement>>();
      _expandedCategories = new HashSet<string>();
      _categories = new List<CategoryInfo>();
    }

    private static Dev _devInstance;

    [RuntimeInitializeOnLoadMethod]
    private static void ensureInstanceExists() {
      if (_devInstance == null) {
        _devInstance = FindObjectOfType<Dev>();
        if (_devInstance == null) {
          _devInstance = new GameObject("__Dev").AddComponent<Dev>();
          DontDestroyOnLoad(_devInstance.gameObject);
        }
      }
    }

    private static void onOpenGui() {
      _registered.Clear();

      var objs = FindObjectsOfType<MonoBehaviour>();
      foreach (var obj in objs) {
        List<DevElement> elements;
        if (DevElementBuilder.TryBuildElements(obj, out elements)) {
          _registered.Add(obj, elements);
        }
      }

      rebuildCategoryMap();
    }

    private static void rebuildCategoryMap() {
      _categories.Clear();
      var typeMap = new Dictionary<Type, KeyValuePair<List<object>, List<DevElement>>>();

      foreach (var reg in _registered) {
        var obj = reg.Key;
        var elements = reg.Value;

        KeyValuePair<List<object>, List<DevElement>> pair;
        if (!typeMap.TryGetValue(obj.GetType(), out pair)) {
          pair = new KeyValuePair<List<object>, List<DevElement>>(new List<object>(), elements);
          typeMap[obj.GetType()] = pair;
        }
        pair.Key.Add(obj);
      }

      foreach (var pair in typeMap) {
        var objs = pair.Value.Key;
        var elements = pair.Value.Value;

        foreach (var element in elements) {
          CategoryInfo categoryInfo = _categories.Query().FirstOrDefault(t => t.name == element.category);
          if (categoryInfo == null) {
            categoryInfo = new CategoryInfo(element.category);
            _categories.Add(categoryInfo);
          }

          categoryInfo.elementInfo.Add(new DevElementInfo() {
            targets = new List<object>(objs),
            element = element.Clone()
          });
        }
      }

      _categories.Sort((a, b) => a.name.CompareTo(b.name));
    }

    private void Update() {
      if (Input.GetKeyDown(KeyCode.F12)) {
        _enabled = !_enabled;
        if (_enabled) {
          onOpenGui();
        }
      }
    }

    private void OnGUI() {
      if (!Application.isPlaying) {
        return;
      }

      if (_enabled) {
        GUIStyle boxStyle = new GUIStyle();
        boxStyle.normal.background = Resources.Load<Texture2D>("ImageBackground");
        boxStyle.border = new RectOffset(4, 4, 4, 4);

        GUIStyle overlayStyle = new GUIStyle();
        overlayStyle.normal.background = Resources.Load<Texture2D>("ImageOverlay");
        overlayStyle.border = new RectOffset(4, 4, 4, 4);

        GUIStyle overlayFooterStyle = new GUIStyle();
        overlayFooterStyle.normal.background = Resources.Load<Texture2D>("ImageOverlayFooter");
        overlayFooterStyle.border = new RectOffset(4, 4, 4, 4);

        GUIStyle foldoutStyle = new GUIStyle();
        foldoutStyle.normal.background = Resources.Load<Texture2D>("ImageFoldout");
        foldoutStyle.border = new RectOffset(4, 4, 4, 4);

        GUIStyle blackStyle = new GUIStyle(GUIStyle.none);
        blackStyle.normal.background = Resources.Load<Texture2D>("ImageBlack");
        blackStyle.border = new RectOffset(4, 4, 4, 4);

        GUIStyle barStyle = new GUIStyle(GUIStyle.none);
        barStyle.normal.background = Resources.Load<Texture2D>("ImageBar");
        barStyle.border = new RectOffset(4, 4, 0, 0);

        _scroll = GUILayout.BeginScrollView(_scroll, GUIStyle.none, GUIStyle.none);
        GUILayout.BeginVertical(boxStyle);
        GUILayout.Label("-- Leap Motion Dev Window --");

        for (int i = 0; i < _categories.Count; i++) {
          var category = _categories[i];
          bool isExpanded = _expandedCategories.Contains(category.name);

          if (i == _categories.Count - 1 && !isExpanded) {
            GUILayout.BeginVertical(overlayFooterStyle);
          } else {
            GUILayout.BeginVertical(overlayStyle);
          }

          bool shouldBeExpanded = GUILayout.Toggle(isExpanded, category.name);
          GUILayout.EndVertical();

          if (shouldBeExpanded) {
            _expandedCategories.Add(category.name);
          } else {
            _expandedCategories.Remove(category.name);
          }

          if (shouldBeExpanded) {
            GUILayout.BeginHorizontal();
            GUILayout.Box("", blackStyle, GUILayout.Width(4), GUILayout.ExpandHeight(true));
            GUILayout.BeginVertical(foldoutStyle);

            for (int j = 0; j < category.elementInfo.Count; j++) {
              if (j != 0) {
                GUILayout.Box("", barStyle, GUILayout.Height(1), GUILayout.ExpandWidth(true));
              }

              category.elementInfo[j].element.Draw(category.elementInfo[j].targets);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
          }
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
      }
    }
  }
}
