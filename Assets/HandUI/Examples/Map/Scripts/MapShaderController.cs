using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapShaderController : MonoBehaviour {

	public Transform scalableParent;
	private MeshRenderer meshRenderer;
	public Shader shader;
	
	ComputeBuffer cbf;
	ComputeBuffer cbfOut;

	private float currentMin;
	Vector3[] arrayToProcess;

	void Start () {
		
		meshRenderer = GetComponent<MeshRenderer>();

	}
	
	void Update () {
		
		if(transform.hasChanged || scalableParent.hasChanged){
			meshRenderer.sharedMaterial.SetVector("_Origin", scalableParent.position);
			meshRenderer.sharedMaterial.SetFloat("_Radius", scalableParent.localScale.x);
		}
	}
}
