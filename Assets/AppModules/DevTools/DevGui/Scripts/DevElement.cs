using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.DevGui {

  public abstract class DevElement {
    public string name;
    public string tooltip;
    public string category;

    public GUIContent guiContent {
      get {
        return new GUIContent(name, tooltip);
      }
    }

    public abstract void Draw(List<object> targets);
    public abstract void ApplyState(List<object> currTargets, List<object> newTargets);

    public virtual DevElement Clone() {
      var clone = (DevElement)Activator.CreateInstance(GetType());
      clone.name = name;
      clone.tooltip = tooltip;
      clone.category = category;
      return clone;
    }
  }

  public abstract class DevElementValue<T> : DevElement {
    public Func<object, T> getValue;
    public Action<object, T> setValue;

    public sealed override void Draw(List<object> targets) {
      T currValue = getValue(targets[0]);
      T newValue = DrawValue(currValue);
      if (!newValue.Equals(currValue)) {
        foreach (var obj in targets) {
          setValue(obj, newValue);
        }
      }
    }

    public sealed override void ApplyState(List<object> currTargets, List<object> newTargets) {
      T currValue = getValue(currTargets[0]);
      foreach (var newTarget in newTargets) {
        setValue(newTargets, currValue);
      }
    }

    protected abstract T DrawValue(T curr);

    public override DevElement Clone() {
      var clone = (DevElementValue<T>)base.Clone();
      clone.getValue = getValue;
      clone.setValue = setValue;
      return clone;
    }
  }

  public class DevElementBool : DevElementValue<bool> {

    protected override bool DrawValue(bool curr) {
      return GUILayout.Toggle(curr, guiContent);
    }
  }

  public class DevElementInt : DevElementValue<int> {
    public Maybe<int> min;
    public Maybe<int> max;

    private string _currString = "";

    protected override int DrawValue(int curr) {
      GUILayout.BeginHorizontal();
      GUILayout.Label(guiContent, GUILayout.ExpandWidth(true));

      float textWidth = GUIStyle.none.CalcSize(new GUIContent("0123456789")).x;
      int textControlId = GUIUtility.GetControlID(FocusType.Passive) + 1;
      _currString = GUILayout.TextField(_currString, GUILayout.Width(textWidth));

      string filteredString = "";
      foreach (var character in _currString) {
        if (char.IsDigit(character) ||
            character == '-') {
          filteredString = filteredString + character;
        }
      }
      _currString = filteredString;

      if (Event.current.type != EventType.Repaint &&
          Event.current.type != EventType.Layout) {
        if (GUIUtility.keyboardControl == textControlId && textControlId != 0) {
          if (GUIUtility.hotControl != 0 && GUIUtility.hotControl != GUIUtility.keyboardControl) {
            GUIUtility.keyboardControl = 0;
          }

          int parsedResult;
          if (int.TryParse(_currString, out parsedResult)) {
            curr = parsedResult;

            if (!min.hasValue || !max.hasValue) {
              curr = min.Match(min => Mathf.Min(min, curr),
                               () => curr);

              curr = max.Match(max => Mathf.Max(max, curr),
                               () => curr);
            }
          }
        } else {
          _currString = curr.ToString();
        }
      }

      GUILayout.EndHorizontal();

      min.Match(min => {
        max.Match(max => {
          curr = Mathf.RoundToInt(GUILayout.HorizontalSlider(curr, min, max));
        });
      });

      return curr;
    }

    public override DevElement Clone() {
      var clone = (DevElementInt)base.Clone();
      clone.min = min;
      clone.max = max;
      return clone;
    }
  }

  public class DevElementFloat : DevElementValue<float> {
    public Maybe<float> min;
    public Maybe<float> max;

    private string _currString = "";

    protected override float DrawValue(float curr) {
      GUILayout.BeginHorizontal();
      GUILayout.Label(guiContent, GUILayout.ExpandWidth(true));

      float textWidth = GUIStyle.none.CalcSize(new GUIContent("0.123456789")).x;
      int textControlId = GUIUtility.GetControlID(FocusType.Passive) + 1;
      _currString = GUILayout.TextField(_currString, GUILayout.Width(textWidth));

      string filteredString = "";
      foreach (var character in _currString) {
        if (char.IsDigit(character) ||
            character == '-' ||
            character == '.') {
          filteredString = filteredString + character;
        }
      }
      _currString = filteredString;

      if (Event.current.type != EventType.Repaint &&
          Event.current.type != EventType.Layout) {
        if (GUIUtility.keyboardControl == textControlId && textControlId != 0) {
          if (GUIUtility.hotControl != 0 && GUIUtility.hotControl != GUIUtility.keyboardControl) {
            GUIUtility.keyboardControl = 0;
          }

          float parsedResult;
          if (float.TryParse(_currString, out parsedResult)) {
            curr = parsedResult;

            if (!min.hasValue || !max.hasValue) {
              curr = min.Match(min => Mathf.Max(min, curr),
                 () => curr);

              curr = max.Match(max => Mathf.Min(max, curr),
                               () => curr);
            }
          }
        } else {
          _currString = curr.ToString("0.#########");
        }
      }

      GUILayout.EndHorizontal();

      min.Match(min => {
        max.Match(max => {
          curr = GUILayout.HorizontalSlider(curr, min, max);
        });
      });

      return curr;
    }

    public override DevElement Clone() {
      var clone = (DevElementFloat)base.Clone();
      clone.min = min;
      clone.max = max;
      return clone;
    }
  }

  public class DevElementString : DevElementValue<string> {

    protected override string DrawValue(string curr) {
      GUILayout.Label(guiContent, GUILayout.ExpandWidth(true));
      float widtgh = GUILayoutUtility.GetLastRect().width;

      curr = GUILayout.TextArea(curr, GUILayout.MinWidth(widtgh));

      if (string.IsNullOrEmpty(curr)) {
        curr = "";
      }

      return curr;
    }
  }

  public class DevElementEnum : DevElementValue<Enum> {
    public Type enumType;

    public string[] names;
    public Array values;

    public DevElementEnum() { }

    public DevElementEnum(Type enumType) {
      this.enumType = enumType;

      names = Enum.GetNames(enumType);
      values = Enum.GetValues(enumType);
    }

    protected override Enum DrawValue(Enum curr) {
      int index = Array.IndexOf(values, curr);

      GUILayout.Label(guiContent);
      Color color = GUI.color;

      for (int i = 0; i < names.Length; i++) {
        if (i == index) {
          GUI.color = Color.green;
        } else {
          GUI.color = color;
        }

        if (GUILayout.Button(names[i])) {
          curr = (Enum)values.GetValue(i);
        }
      }

      GUI.color = color;
      return curr;
    }

    public override DevElement Clone() {
      var clone = (DevElementEnum)base.Clone();
      clone.enumType = enumType;
      clone.names = names;
      clone.values = values;
      return clone;
    }
  }

  public class DevElementButton : DevElement {
    public Action<object> onPress;

    public override void Draw(List<object> targets) {
      if (GUILayout.Button(guiContent)) {
        foreach (var target in targets) {
          onPress(target);
        }
      }
    }

    public override void ApplyState(List<object> currTargets, List<object> newTargets) { }

    public override DevElement Clone() {
      var clone = (DevElementButton)base.Clone();
      clone.onPress = onPress;
      return clone;
    }
  }
}
