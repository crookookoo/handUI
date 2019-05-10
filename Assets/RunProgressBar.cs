using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RunProgressBar : MonoBehaviour
{
    public float lineLenght = 32;
    public float fullDuration = 200;
    private bool isOn = true;
    
    private LineRenderer line;

    private float currentPosition = 0;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            currentPosition += lineLenght / fullDuration * Time.deltaTime;
            if (currentPosition > lineLenght) currentPosition -= lineLenght;
        
            line.SetPosition(1, currentPosition * Vector3.right);        
        }
    }

    public void TogglePause()
    {
        isOn = !isOn;
    }
}
