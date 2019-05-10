using DG.Tweening;
using UnityEngine;

public class MeshVertsToLine : MonoBehaviour
{
    public bool ignoreLastVert;
    private Color initColor, halfWhite, fullWhite;
    private LineRenderer line;

    private Mesh mesh;

    // Use this for initialization
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        line = GetComponent<LineRenderer>();

        line.positionCount = mesh.vertexCount - 1;


        if (!ignoreLastVert)
            for (var i = 1; i < mesh.vertexCount; i++)
                line.SetPosition(i - 1, mesh.vertices[i]);
        else
            for (var i = 1; i < mesh.vertexCount; i++)
                line.SetPosition(i - 1, mesh.vertices[i]);

        initColor = line.material.color;

        halfWhite = Color.white;
        halfWhite.a = 0.5f;

        fullWhite = Color.white;
    }

    public void HalfBright()
    {
        line.material.DOColor(halfWhite, 0.1f);
    }

    public void FullBright()
    {
        line.material.DOColor(fullWhite, 0.1f);
    }

    public void BackToDefault()
    {
        line.material.DOColor(initColor, 0.1f);
    }
}