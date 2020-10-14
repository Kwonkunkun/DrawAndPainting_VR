using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletDisplayController : MonoBehaviour
{
    [SerializeField] private bool toggleCamera = false;

    [SerializeField/*, HideInInspector*/] private MeshRenderer m_displayRenderer = null;
    [SerializeField/*, HideInInspector*/] private Material m_cameraRenderMaterial = null;
    [SerializeField/*, HideInInspector*/] private Material m_disabledMaterial = null;
    [SerializeField/*, HideInInspector*/] private List<Material> m_currentMaterials = null;
    [SerializeField] private Camera m_securityCamera = null;

    private readonly string m_instanceString = " (Instance)";
    private string m_cameraRenderMaterialAdjustedName = "";
    private string m_disabledMaterialAdjustedName = "";
    private int m_rendererMaterialIndex = 0;

    private bool m_displayOn = true;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (m_securityCamera == null)
            m_securityCamera = FindObjectOfType<FreebieCameraHookupScript>().SecurityCamera;

        if (m_cameraRenderMaterial == null || m_disabledMaterial == null)
        {
            Debug.LogError(this + "THERE ARE MATERIALS (Resources) MISSING!!!");
            return;
        }

        // Create search strings for the list of materials.
        m_cameraRenderMaterialAdjustedName = m_cameraRenderMaterial.name + m_instanceString;
        m_disabledMaterialAdjustedName = m_disabledMaterial.name + m_instanceString;

        // Now get the list of materials from the renderer
        if (m_displayRenderer != null)
        {
            if (m_currentMaterials == null) m_currentMaterials = new List<Material>();
            m_displayRenderer.GetSharedMaterials(m_currentMaterials);
        }
        Debug.Log("There are " + m_currentMaterials.Count + " materials in the renderer");
        bool matFound = false;
        // Search for the location of the Material used for the display
        for (int i = 0; i < m_currentMaterials.Count; i++)
        {
            if (m_currentMaterials[i].name == m_cameraRenderMaterial.name
                || m_currentMaterials[i].name == m_cameraRenderMaterialAdjustedName
                || m_currentMaterials[i].name == m_disabledMaterial.name
                || m_currentMaterials[i].name == m_disabledMaterialAdjustedName)
            {
                // ... and store that index for future references as the materials have
                // to be put back in the correct order.
                m_rendererMaterialIndex = i;
                matFound = true;
            }
        }

        // If none of the materials to be switched were found in the renderer, then add
        // the most relevant now, and store that position (end of the list) as the corrent
        // index.
        if (!matFound)
        {
            m_rendererMaterialIndex = m_currentMaterials.Count;

            if (m_displayOn)
                m_currentMaterials.Add(m_cameraRenderMaterial);
            else
                m_currentMaterials.Add(m_disabledMaterial);
        }


    }

    public void ToggleDisplay()
    {
        // Invert the 'on' status
        m_displayOn = !m_displayOn;

        // Switch the camera on or off to stop rendering, thus increasing performance.
        if (toggleCamera && m_securityCamera != null)
            m_securityCamera.enabled = m_displayOn;

        // Switch out the material to the new correct one for the display
        if (m_displayOn)
            m_currentMaterials[m_rendererMaterialIndex] = m_cameraRenderMaterial;
        else
            m_currentMaterials[m_rendererMaterialIndex] = m_disabledMaterial;

        // Now update the list of materials for the renderer
        m_displayRenderer.sharedMaterials = m_currentMaterials.ToArray();
    }
}
