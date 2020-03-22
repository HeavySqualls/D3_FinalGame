using UnityEngine;
using UnityEngine.UI;
//using TMPro;

[ExecuteInEditMode]
public class CustomRenderQueue_Text : MonoBehaviour
{
    public UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;

    public bool apply = false;

    private void Update()
    {
        if (apply)
        {
            apply = false;
            Debug.Log("Updated material val");
            Graphic text = GetComponent<Graphic>();
            Material existingGlobalMat = text.materialForRendering;
            Material updatedMaterial = new Material(existingGlobalMat);
            updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
            text.material = updatedMaterial;
        }
    }
}
