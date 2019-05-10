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
using UnityEngine.UI;

public class UIEntity : MonoBehaviour
{
    [Header("Color")]
    [SerializeField] private ObjectType objectType = ObjectType.Auto;
    [SerializeField] private Color overrideTint;
  
    [Space]
    
    [SerializeField] private bool setColorOnStart;  
    [SerializeField] [Range(0, 1)] private float startOpacity = 0.5f;

    [Header("Scale")]
    [SerializeField] private bool hideOnStart = false;

    private LineRenderer line;
    private MeshRenderer mesh;
    private Image image;
    private SpriteRenderer sprite;
    private TextMeshPro text;
    private TextMeshProUGUI textGUI;
    private bool useCustomColor = false;
    private bool guiText = false;
    
    public enum ObjectType
    {
        Auto,
        Mesh,
        Line,
        Sprite,
        Image,
        Text
    }

    private bool renderComponentFound = false;
    private Vector3 initScale, initLocalPos;
    
    private void Start()
    {
        if (objectType == ObjectType.Auto) objectType = DetermineType();
       
        if (overrideTint != Color.clear)
        {
            useCustomColor = true;
        }

        if (setColorOnStart)
            SetColor(startOpacity);
        
        initScale = transform.localScale; 
        initLocalPos = transform.localPosition;
		
        if(hideOnStart)
            transform.localScale = Vector3.zero;

       
    }

    private ObjectType DetermineType()
    {
        if (GetComponent<TextMeshPro>() || GetComponent<TextMeshProUGUI>())
        {
            if (GetComponent<TextMeshPro>())
            {
                text = GetComponent<TextMeshPro>();
                
            }
            else
            {
                textGUI = GetComponent<TextMeshProUGUI>();
                guiText = true;
            }
            
            renderComponentFound = true;
            return ObjectType.Text;
        }

        if (GetComponent<LineRenderer>())
        {
            line = GetComponent<LineRenderer>();
            renderComponentFound = true;
            return ObjectType.Line;
        }

        if (GetComponent<SpriteRenderer>())
        {
            sprite = GetComponent<SpriteRenderer>();
            renderComponentFound = true;
            return ObjectType.Sprite;
        }

        if (GetComponent<Image>())
        {
            image = GetComponent<Image>();
            renderComponentFound = true;
            return ObjectType.Image;
        }

        if (GetComponent<MeshRenderer>())
        {
            mesh = GetComponent<MeshRenderer>();
            renderComponentFound = true;
            return ObjectType.Mesh;
        }


        return ObjectType.Auto;
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
            
            case ObjectType.Image:
                image = GetComponent<Image>();
                break;
            
            case ObjectType.Text:
                text = GetComponent<TextMeshPro>();
                break;
            
            case ObjectType.Auto:
                HandUIManager.Warn("UIEntity color component not found, try setting the object type value", this);
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
            c = overrideTint;
            c.a = a;
        }

        if (!renderComponentFound) DetermineType();

        switch (objectType)
        {
            case ObjectType.Mesh:
                if (mesh.material.HasProperty("_Color"))
                    mesh.material.DOColor(c, t);
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

            case ObjectType.Image:
                image.DOColor(c, t);
                break;
            
            case ObjectType.Text:
                if(!guiText) text.DOColor(c, t);
                else textGUI.DOColor(c, t);
                break;
        }
    }

    public void Scale(float factor, float duration = 0.1f)
    {
        bool scaleUp = factor > transform.localScale.y / initScale.y;
               
        transform.DOScale(initScale * factor, duration)
            .SetEase(scaleUp ? Ease.OutBack : Ease.InQuint);
    }
    
    public void ScaleY(float factor, float duration = 0.1f)
    {
        bool scaleUp = factor > transform.localScale.y / initScale.y;
               
        transform.DOScaleY(initScale.y * factor, duration)
            .SetEase(scaleUp ? Ease.OutBack : Ease.InQuint);
    } 

    public void Show(float delay = 0, float t = 0.2f)
    {
        if(delay > 0 ) transform.DOScale(initScale, t).SetEase(Ease.OutBack).SetDelay(delay);
        else
        {
            transform.DOScale(initScale, t).SetEase(Ease.OutBack);
        }
    } 
	
    public void Hide(float delay = 0, float t = 0.2f)
    {
        if(delay > 0 ) transform.DOScale(Vector3.zero, t).SetEase(Ease.InQuint).SetDelay(delay);
        else
        {
            transform.DOScale(Vector3.zero, t).SetEase(Ease.InQuint);
        }
    }

    public void Punch(float magnitude = 0.15f, float duration = 0.1f)
    {
        transform.DOPunchScale(initScale * magnitude, 0.1f);
    }

    public void MoveOnAxis(string axis, float dist, float t = 0.15f)
    {
        switch (axis)
        {
            case "X":
                transform.DOLocalMoveX(dist, t);
                break;
            case "Y":
                transform.DOLocalMoveY(dist, t);
                break;
            case "Z":
                transform.DOLocalMoveZ(dist, t);
                break;
        }
    }
    
    public MeshRenderer GetMeshRend()
    {
        return mesh;
    }
    public LineRenderer GetLineRend()
    {
        return line;
    }
    public SpriteRenderer GetSpriteRend()
    {
        return sprite;
    }
    public TextMeshPro GetText()
    {
        return text;
    }
    public Image GetImage()
    {
        return image;
    }

    public Vector3 GetInitPos()
    {
        return initLocalPos;
    }
    
    public Vector3 GetInitScale()
    {
        return initScale;
    }

}