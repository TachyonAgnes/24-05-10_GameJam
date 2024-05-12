//using UnityEngine;

//public class Tool_SetModelToGIProbe : MonoBehaviour
//{
//    [ContextMenu("SetModelToGIProbe")]
//    public void SetModelToGIProbe(){
//        // find all child that has mesh renderer
//        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
//        // set them contribute to GI
//        foreach (MeshRenderer meshRenderer in meshRenderers)
//        {
//            meshRenderer.gameObject.isStatic = true;
//            meshRenderer.receiveGI = ReceiveGI.LightProbes;
//        }
//    }
//}
