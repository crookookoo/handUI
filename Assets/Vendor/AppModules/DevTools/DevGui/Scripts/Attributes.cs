using System;

namespace Leap.Unity.DevGui {

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class DevCategoryAttribute : Attribute {
    public Maybe<string> category;

    public DevCategoryAttribute() {
      category = Maybe.None;
    }

    public DevCategoryAttribute(string category) {
      this.category = category;
    }
  }

  public abstract class DevAttributeBase : Attribute {
    public string name;
    public string tooltip;
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class DevValueAttribute : DevAttributeBase {
    public DevValueAttribute() { }

    public DevValueAttribute(string name) {
      this.name = name;
    }

    public DevValueAttribute(string name, string tooltip) {
      this.name = name;
      this.tooltip = tooltip;
    }
  }

  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class DevButtonAttribute : DevAttributeBase {
    public DevButtonAttribute() { }

    public DevButtonAttribute(string name) {
      this.name = name;
    }

    public DevButtonAttribute(string name, string tooltip) {
      this.name = name;
      this.tooltip = tooltip;
    }
  }
}
