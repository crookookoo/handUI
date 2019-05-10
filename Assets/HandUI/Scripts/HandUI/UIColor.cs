/******************************************************************************
 
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

using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIColor : MonoBehaviour
{
    public enum ObjectType
    {
        Mesh,
        Line,
        Sprite,
        Text
    }

//    [HideInInspector] public float brightnessMult = 1;

    private readonly bool componentFound = false;

    public bool useCustomColor = false;
    public Color customColor;

    private LineRenderer line;
    private MeshRenderer mesh;

    public ObjectType objectType;

    public bool setColorOnStart;
    private SpriteRenderer sprite;

    [Range(0, 1)] public float startOpacity = 0.5f;

    private TextMeshPro text;

    private void Start()
    {
        if (!componentFound) FindComponent();

        if (setColorOnStart)
            SetColor(startOpacity);
    }

    private void FindComponent()
    {
        switch (objectType)
        {
            case ObjectType.Mesh:
                mesh = GetComponent<MeshRenderer>();
                break;

            case ObjectType.Line:
                line = GetComponent<LineRenderer>();
                break;

            case ObjectType.Sprite:
                sprite = GetComponent<SpriteRenderer>();
                break;

            case ObjectType.Text:
                text = GetComponent<TextMeshPro>();
                break;
        }
    }

    public void SetInitialColor(float t = 0.15f)
    {
        SetColor(startOpacity, t);
    }

    public void SetColor(float a, float t = 0.15f)
    {
        var c = HandUIManager.GetColor(a);

        if (useCustomColor)
        {
            c = customColor;
            c.a = a;
        }

        if (!componentFound) FindComponent();

        switch (objectType)
        {
            case ObjectType.Mesh:
                if (mesh.material.HasProperty("_Color"))
                    mesh.material.DOColor(c, t).SetId("");
                else
                    mesh.material.SetColor("_TintColor", c);
                break;

            case ObjectType.Line:
                if (line.material.HasProperty("_Color"))
                    line.material.DOColor(c, t);
                else if (line.material.HasProperty("_TintColor"))
                    line.material.SetColor("_TintColor", c);
                break;

            case ObjectType.Sprite:
                sprite.DOColor(c, t);
                break;

            case ObjectType.Text:
                text.DOColor(c, t);
                break;
        }
    }
    private Color EvaluateColor(float alpha)
    {
        var c = HandUIManager.whitePoint;
        
        if (useCustomColor) c = customColor;
        c.a = 1;

        if (HandUIManager.uiColorMode == HandUIManager.UIColorMode.Transparent)
        {
            c.a = alpha;
            return c;
        }

        if (HandUIManager.uiColorMode == HandUIManager.UIColorMode.Gradient) c = Color.Lerp(Color.black, c, alpha);

        return c;
    }
}