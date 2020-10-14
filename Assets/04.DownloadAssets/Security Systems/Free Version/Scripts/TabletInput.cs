using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TabletDisplayController))]
public class TabletInput : MonoBehaviour
{
    [SerializeField,HideInInspector] private TabletDisplayController m_tabletController;
    [SerializeField] private KeyCode m_cameraDisplayToggle = KeyCode.F12;

    [ContextMenu("Get References For Script")]
    private void GetReferences()
    {

        if (m_tabletController == null)
        {
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying)
                UnityEditor.Undo.RecordObject(this, "Obtaining TabletDisplayController reference");
#endif
            m_tabletController = GetComponent<TabletDisplayController>();
        }
    }

    private void Reset()
    {
        GetReferences();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(m_cameraDisplayToggle) && m_tabletController != null)
        {
            m_tabletController.ToggleDisplay();
        }
    }
}
