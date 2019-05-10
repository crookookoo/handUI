using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapBgCircle : MonoBehaviour {

    public Turntable turntable;
    private UIColor uIColor;
    // private HandUIObject handUI;

    private void OnEnable() {
        uIColor = GetComponent<UIColor>();
        // handUI = GetComponent<HandUIObject>();

        // handUI.OnHover += OnTouch;
        // handUI.OnUnhover += OnRelease;
        turntable = transform.parent.GetComponent<Turntable>();
    
        turntable.OnTouch += OnTouch;
        turntable.OnRelease += OnRelease;
    }

    void OnTouch(){
        uIColor.SetColor(0.04f);
    }

    void OnRelease(){
        uIColor.SetColor(0.01f);
    }

}