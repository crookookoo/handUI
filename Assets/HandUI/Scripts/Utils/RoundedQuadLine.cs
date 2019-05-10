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

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class RoundedQuadLine : MonoBehaviour {
 
	private LineRenderer line;
    public RoundedQuadMesh rqm;
    public bool ignoreLastVert;

    private bool hasRqm = false;
    private Mesh mesh;
    
    void Start ()
    {
        if (!GetComponent<LineRenderer>())
        {
            gameObject.AddComponent<LineRenderer>();
        }

        if (!GetComponent<UIColor>())
        {
            gameObject.AddComponent<UIColor>().objectType = UIColor.ObjectType.Line;
        }

        line = GetComponent<LineRenderer>();
        line.startWidth = 0.002f;
        line.endWidth = 0.002f;
        line.loop = true;
        line.useWorldSpace = false;

#if UNITY_EDITOR
        UnityEditorInternal.ComponentUtility.MoveComponentDown (line);
#endif
        
    }
   
    private void OnValidate()
    {
        if (rqm)
        {
            hasRqm = true;
        }
        else
        {
            hasRqm = false;
        }
    }
	
    void UpdatePoints ()
    {
        if (hasRqm)
        {
            var verts = rqm.GetMeshVerts();
            line.positionCount = verts.Length - 1;

            if (!ignoreLastVert)
                for (var i = 1; i < verts.Length; i++)
                    line.SetPosition(i - 1, verts[i]);
            else
                for (var i = 1; i < verts.Length; i++)
                    line.SetPosition(i - 1, verts[i]);

        }
    }

    private void Update()
    {
        if (rqm && rqm.autoUpdate) UpdatePoints();

    }
}
