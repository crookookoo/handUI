/******************************************************************************
 
 Based on Bunny83's answer here:
 https://answers.unity.com/questions/1007222/round-procedural-square-mesh-corners-c.html

 Copyright (C) 2019 Eugene Krivoruchko                                           

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 ******************************************************************************/

using UnityEngine;

[ExecuteInEditMode]
public class RoundedQuadMesh : MonoBehaviour
{
    public enum Align
    {
        Center,
        Top,
        Bottom,
        Left,
        Right
    }

    public Align align = Align.Center;
    public Vector2 size = new Vector2(1,1);
    public bool autoUpdate = true;
    [Space]
    [Range(0,1)] public float roundCorners = 0.5f;
    [Range(0,1)] public float roundTopLeft = 0.0f;
    [Range(0,1)] public float roundTopRight = 0.0f;
    [Range(0,1)] public float roundBottomLeft = 0.0f;
    [Range(0,1)] public float roundBottomRight = 0.0f;
    [Space]
    public int cornerVertexCount = 8;
    [Space]
    public bool createUv = true;
    public bool flipBackFaceUv = false;
    public bool doubleSided = false;
   
    public bool usePercentage = true;
    private Rect rect = new Rect(-0.5f,-0.5f,1f,1f);
    private float scale = 1f;

    private MeshFilter m_MeshFilter;
    private Mesh m_Mesh;
    private Vector3[] m_Vertices;
    private Vector3[] m_Normals;
    private Vector2[] m_UV;
    private int[] m_Triangles;
    
    void Start ()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        if (m_MeshFilter == null)
            m_MeshFilter = gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }
        m_Mesh = new Mesh();
        m_MeshFilter.sharedMesh = m_Mesh;
        UpdateMesh();
    }
 
    public Mesh UpdateMesh()
    {
        rect.width = size.x;
        rect.height = size.y;
        
        switch (align)
        {
            case Align.Center:
                rect.x = -size.x / 2;
                rect.y = -size.y / 2;                
                break;
            case Align.Top:
                rect.x = -size.x / 2;
                rect.y = 0;
                break;
            case Align.Bottom:
                rect.x = -size.x / 2;
                rect.y = -size.y;
                break;
            case Align.Right:
                rect.x = 0;
                rect.y = -size.y / 2;
                break;
            case Align.Left:
                rect.x = -size.x;
                rect.y = -size.y / 2;
                break;
        }
        
        if (cornerVertexCount<2)
            cornerVertexCount = 2;
        int sides = doubleSided ? 2 : 1;
        int vCount = cornerVertexCount * 4 * sides + sides; //+sides for center vertices
        int triCount = (cornerVertexCount * 4) * sides;
        if (m_Vertices == null || m_Vertices.Length != vCount)
        {
            m_Vertices = new Vector3[vCount];
            m_Normals = new Vector3[vCount];
        }
        if (m_Triangles == null || m_Triangles.Length != triCount * 3)
            m_Triangles = new int[triCount * 3];
        if (createUv && (m_UV == null || m_UV.Length != vCount))
        { 
            m_UV = new Vector2[vCount];
        }
        int count = cornerVertexCount * 4;
        if (createUv)
        {
            m_UV[0] = Vector2.one *0.5f;
            if (doubleSided)
                m_UV[count + 1] = m_UV[0];
        }
        float tl = Mathf.Max(0, roundTopLeft + roundCorners);
        float tr = Mathf.Max(0, roundTopRight + roundCorners);
        float bl = Mathf.Max(0, roundBottomLeft + roundCorners);
        float br = Mathf.Max(0, roundBottomRight + roundCorners);
        float f = Mathf.PI * 0.5f / (cornerVertexCount - 1);
        float a1 = 1f;
        float a2 = 1f;
        float x = 1f;
        float y = 1f;
        Vector2 rs = Vector2.one;
        if (usePercentage)
        {
            rs = new Vector2(rect.width, rect.height) * 0.5f;
            if (rect.width > rect.height)
                a1 = rect.height / rect.width;
            else
                a2 = rect.width / rect.height;
            tl = Mathf.Clamp01(tl);
            tr = Mathf.Clamp01(tr);
            bl = Mathf.Clamp01(bl);
            br = Mathf.Clamp01(br);
        }
        else
        {
            x = rect.width * 0.5f;
            y = rect.height * 0.5f;
            if (tl + tr > rect.width)
            {
                float b = rect.width / (tl + tr);
                tl *= b;
                tr *= b;
            }
            if (bl + br > rect.width)
            {
                float b = rect.width / (bl + br);
                bl *= b;
                br *= b;
            }
            if (tl + bl > rect.height)
            {
                float b = rect.height / (tl + bl);
                tl *= b;
                bl *= b;
            }
            if (tr + br > rect.height)
            {
                float b = rect.height / (tr + br);
                tr *= b;
                br *= b;
            }
        }
        m_Vertices[0] = rect.center * scale;
        if (doubleSided)
            m_Vertices[count + 1] = rect.center * scale;
        for (int i = 0; i < cornerVertexCount; i++ )
        {
            float s = Mathf.Sin((float)i * f);
            float c = Mathf.Cos((float)i * f);
            Vector2 v1 = new Vector3(-x + (1f - c) * tl * a1, y - (1f - s) * tl * a2);
            Vector2 v2 = new Vector3(x - (1f - s) * tr * a1, y - (1f - c) * tr * a2);
            Vector2 v3 = new Vector3(x - (1f - c) * br * a1, -y + (1f - s) * br * a2);
            Vector2 v4 = new Vector3(-x + (1f - s) * bl * a1, -y + (1f - c) * bl * a2);
 
            m_Vertices[1 + i] = (Vector2.Scale(v1, rs) + rect.center) * scale;
            m_Vertices[1 + cornerVertexCount + i] = (Vector2.Scale(v2, rs) + rect.center) * scale;
            m_Vertices[1 + cornerVertexCount * 2 + i] = (Vector2.Scale(v3, rs) + rect.center) * scale;
            m_Vertices[1 + cornerVertexCount * 3 + i] = (Vector2.Scale(v4, rs) + rect.center) * scale;
            if (createUv)
            {
                if(!usePercentage)
                {
                    Vector2 adj = new Vector2(2f/rect.width, 2f/rect.height);
                    v1 = Vector2.Scale(v1, adj);
                    v2 = Vector2.Scale(v2, adj);
                    v3 = Vector2.Scale(v3, adj);
                    v4 = Vector2.Scale(v4, adj);
                }
                m_UV[1 + i] = v1 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + cornerVertexCount * 1 + i] = v2 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + cornerVertexCount * 2 + i] = v3 * 0.5f + Vector2.one * 0.5f;
                m_UV[1 + cornerVertexCount * 3 + i] = v4 * 0.5f + Vector2.one * 0.5f;
            }
            if (doubleSided)
            {
                m_Vertices[1 + cornerVertexCount * 8 - i] = m_Vertices[1 + i];
                m_Vertices[1 + cornerVertexCount * 7 - i] = m_Vertices[1 + cornerVertexCount + i];
                m_Vertices[1 + cornerVertexCount * 6 - i] = m_Vertices[1 + cornerVertexCount * 2 + i];
                m_Vertices[1 + cornerVertexCount * 5 - i] = m_Vertices[1 + cornerVertexCount * 3 + i];
                if (createUv)
                {
                    m_UV[1 + cornerVertexCount * 8 - i] = v1 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + cornerVertexCount * 7 - i] = v2 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + cornerVertexCount * 6 - i] = v3 * 0.5f + Vector2.one * 0.5f;
                    m_UV[1 + cornerVertexCount * 5 - i] = v4 * 0.5f + Vector2.one * 0.5f;
                }
            }
        }
        for (int i = 0; i < count + 1;i++ )
        {
            m_Normals[i] = -Vector3.forward;
            if (doubleSided)
            {
                m_Normals[count + 1 + i] = Vector3.forward;
                if (flipBackFaceUv)
                {
                    Vector2 uv = m_UV[count+1+i];
                    uv.x = 1f - uv.x;
                    m_UV[count+1+i] = uv;
                }
            }
        }
        for (int i = 0; i < count; i++)
        {
            m_Triangles[i*3    ] = 0;
            m_Triangles[i*3 + 1] = i + 1;
            m_Triangles[i*3 + 2] = i + 2;
            if (doubleSided)
            {
                m_Triangles[(count + i) * 3] = count+1;
                m_Triangles[(count + i) * 3 + 1] = count+1 +i + 1;
                m_Triangles[(count + i) * 3 + 2] = count+1 +i + 2;
            }
        }
        m_Triangles[count * 3 - 1] = 1;
        if (doubleSided)
            m_Triangles[m_Triangles.Length - 1] = count + 1 + 1;
 
        m_Mesh.Clear();
        m_Mesh.vertices = m_Vertices;
        m_Mesh.normals = m_Normals;
        if (createUv)
            m_Mesh.uv = m_UV;
        m_Mesh.triangles = m_Triangles;
        return m_Mesh;
    }
    
    public Vector3[] GetMeshVerts()
    {
        return m_Vertices;
    }

    void Update ()
    {
        if (autoUpdate)
            UpdateMesh();
    }
}