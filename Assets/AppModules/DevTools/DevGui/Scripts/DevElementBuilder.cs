using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.DevGui {
  using Attributes;
  using Query;

  public static class DevElementBuilder {

    private static Dictionary<Type, List<DevElement>> _typeToElements = new Dictionary<Type, List<DevElement>>();

    public static bool TryBuildElements(object obj, out List<DevElement> elements) {
      var type = obj.GetType();

      if (!_typeToElements.TryGetValue(type, out elements)) {
        string category = type.GetCustomAttributes(typeof(DevCategoryAttribute), inherit: true).
                       Query().
                       Cast<DevCategoryAttribute>().
                       FirstOrNone().
                       Match(a => a.category.Match(c => c,
                                                   () => type.Name),
                             () => type.Name);

        elements = new List<DevElement>();
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        buildElementsFromMembers(category, type.GetMethods(flags), elements);
        buildElementsFromMembers(category, type.GetFields(flags), elements);
        buildElementsFromMembers(category, type.GetProperties(flags), elements);

        _typeToElements[type] = elements;
      }

      if (elements != null) {
        elements = elements.Query().Select(t => t.Clone()).ToList();
        return true;
      } else {
        return false;
      }
    }

    private static void buildElementsFromMembers(string category, MemberInfo[] infos, List<DevElement> elements) {
      string overridingCategory = null;

      foreach (var info in infos) {
        overridingCategory = info.GetCustomAttributes(typeof(DevCategoryAttribute), inherit: true).
                             Query().
                             Cast<DevCategoryAttribute>().
                             FirstOrNone().
                             Match(a => a.category.Match(c => c,
                                                         () => info.GetCustomAttributes(typeof(HeaderAttribute), inherit: true).
                                                                    Query().
                                                                    Cast<HeaderAttribute>().
                                                                    FirstOrNone().
                                                                    Match(h => h.header,
                                                                          () => overridingCategory)),
                                   () => overridingCategory);

        var attributes = info.GetCustomAttributes(typeof(DevAttributeBase), inherit: true);
        foreach (var attribute in attributes) {
          var devAttribute = attribute as DevAttributeBase;

          var element = buildMemberElement(info, devAttribute);
          if (element != null) {

            if (string.IsNullOrEmpty(devAttribute.name)) {
              element.name = Utils.GenerateNiceName(info.Name);
            } else {
              element.name = devAttribute.name;
            }

            if (string.IsNullOrEmpty(devAttribute.tooltip)) {
              element.tooltip = info.GetCustomAttributes(typeof(TooltipAttribute), inherit: true).
                                     Query().
                                     Cast<TooltipAttribute>().
                                     FirstOrNone().
                                     Match(a => a.tooltip,
                                           () => "");
            } else {
              element.tooltip = devAttribute.tooltip;
            }

            element.category = overridingCategory == null ? category : overridingCategory;
            elements.Add(element);
          }
        }
      }
    }

    private static DevElement buildMemberElement(MemberInfo info, DevAttributeBase attribute) {
      if (attribute is DevValueAttribute) {
        return buildDevValue(info, attribute as DevValueAttribute);
      } else if (attribute is DevButtonAttribute) {
        return buildDevButton(info, attribute as DevButtonAttribute);
      } else {
        Debug.LogError("Unsupported attribute " + attribute);
        return null;
      }
    }

    private static DevElement buildDevValue(MemberInfo info, DevValueAttribute attribute) {
      Type type;
      Action<object, object> setter;
      Func<object, object> getter;

      if (info is FieldInfo) {
        var field = info as FieldInfo;
        type = field.FieldType;
        setter = (o, v) => field.SetValue(o, v);
        getter = o => field.GetValue(o);
      } else if (info is PropertyInfo) {
        var property = info as PropertyInfo;
        type = property.PropertyType;
        setter = (o, v) => property.SetValue(o, v, null);
        getter = o => property.GetValue(o, null);
      } else {
        Debug.LogError("DevValue used on unsupported member type " + info);
        return null;
      }

      var rangeAtt = info.GetCustomAttributes(typeof(RangeAttribute), inherit: true).Query().
                                                                               Cast<RangeAttribute>().
                                                                               FirstOrNone();

      var minAtt = info.GetCustomAttributes(typeof(MinValue), inherit: true).Query().
                                                                             Cast<MinValue>().
                                                                             FirstOrNone();

      var maxAtt = info.GetCustomAttributes(typeof(MaxValue), inherit: true).Query().
                                                                             Cast<MaxValue>().
                                                                             FirstOrNone();

      var minValue = minAtt.Query().
                            Select(t => t.minValue).
                            Concat(rangeAtt.Query().
                                            Select(t => t.min)).
                            FirstOrNone();

      var maxValue = maxAtt.Query().
                            Select(t => t.maxValue).
                            Concat(rangeAtt.Query().
                                            Select(t => t.max)).
                            FirstOrNone();

      if (type == typeof(int)) {
        return new DevElementInt() {
          min = minValue.Query().Select(Mathf.RoundToInt).FirstOrNone(),
          max = maxValue.Query().Select(Mathf.RoundToInt).FirstOrNone(),
          getValue = (o) => (int)getter(o),
          setValue = (o, i) => setter(o, i)
        };
      } else if (type == typeof(float)) {
        return new DevElementFloat() {
          min = minValue,
          max = maxValue,
          getValue = (o) => (float)getter(o),
          setValue = (o, f) => setter(o, f)
        };
      } else if (type == typeof(string)) {
        return new DevElementString() {
          getValue = (o) => (string)getter(o),
          setValue = (o, s) => setter(o, s)
        };
      } else if (type == typeof(bool)) {
        return new DevElementBool() {
          getValue = (o) => (bool)getter(o),
          setValue = (o, b) => setter(o, b)
        };
      } else if (type.IsEnum) {
        return new DevElementEnum(type) {
          getValue = (o) => (Enum)getter(o),
          setValue = (o, e) => setter(o, e)
        };
      }

      Debug.LogError("DevValue only supports properties of type int, float, string, or bool.");
      return null;
    }

    private static DevElement buildDevButton(MemberInfo info, DevButtonAttribute attribute) {
      MethodInfo method = info as MethodInfo;
      if (method == null) {
        Debug.LogError("DevButton can only be used on Methods.");
        return null;
      }

      if (method.GetParameters().Length != 0) {
        Debug.LogError("A method tagged with DevButton must have zero parameters.");
        return null;
      }

      if (method.ContainsGenericParameters) {
        Debug.LogError("A method tagged with DevButton must not have any unbound generic parameters.");
        return null;
      }

      return new DevElementButton() {
        onPress = o => method.Invoke(o, null)
      };
    }
  }
}
