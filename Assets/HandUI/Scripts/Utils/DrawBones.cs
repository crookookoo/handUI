 using UnityEngine;
 
 public class DrawBones : MonoBehaviour
 {
     private SkinnedMeshRenderer m_Renderer;
     
     void Start()
     {
         m_Renderer = GetComponentInChildren<SkinnedMeshRenderer>();
         if (m_Renderer == null)
         {
             Debug.LogWarning("No SkinnedMeshRenderer found, script removed");
             Destroy(this);
         }
     }
     
	 void OnDrawGizmos()
	 {
		if(m_Renderer == null)
			Start();

		var bones = m_Renderer.bones;
         foreach(var B in bones)
         {
            if (B.parent == null)
                continue;
			Gizmos.color = Color.red;
            Gizmos.DrawLine(B.position, B.parent.position);
            Gizmos.DrawSphere(B.position, 0.002f);

         }

	 }

     void LateUpdate()
     {
     }
 }
