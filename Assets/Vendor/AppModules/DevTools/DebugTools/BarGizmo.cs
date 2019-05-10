using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.RuntimeGizmos {

  public static partial class BarGizmo {

    public static void Render(float amount, Vector3 position, Vector3 direction, Color color,
                         float scale = 1f) {
      RuntimeGizmoDrawer drawer;

      if (RuntimeGizmoManager.TryGetGizmoDrawer(out drawer)) {
        var thickness = 0.10f;

        drawer.color = color;

        drawer.PushMatrix();

        drawer.matrix = Matrix4x4.TRS(position, Quaternion.LookRotation(direction), Vector3.one * scale);

        var bar = new Vector3(thickness, thickness, amount);
        drawer.DrawWireCube(amount * 0.5f * Vector3.forward, bar + (Vector3.one * thickness));
        drawer.DrawCube(amount * 0.5f * Vector3.forward, bar);

        drawer.PopMatrix();
      }
    }

    public static void Render(float amount, Vector3 position, Vector3 direction,
                         float scale = 1f) {
      Render(amount, position, direction, Color.white, scale);
    }

    public static void Render(float amount, Transform atTransform, Color color,
                         float scale = 1f) {
      Render(amount, atTransform.position, atTransform.forward, color, scale);
    }

    public static void Render(float amount, Transform atTransform, float scale = 1f) {
      Render(amount, atTransform.position, atTransform.forward, Color.white, scale);
    }

  }

}