using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tool_WireCreator : EditorWindow
{
    bool m_pollCreateNewWire = false;

    [MenuItem("Tools/Wire Builder")]
    static void OpenWindow()
    {
        Tool_WireCreator window = EditorWindow.GetWindow<Tool_WireCreator>();
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate -= OnScene;
        SceneView.onSceneGUIDelegate += OnScene;
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Create New Wire"))
        {
            m_pollCreateNewWire = true;
        }
    }

    void OnScene(SceneView sceneView)
    {
        Event e = Event.current;
        if(e.type == EventType.MouseDown)
        {
            Vector3 mCenter = Camera.current.ScreenToWorldPoint(
            new Vector2(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y));
            Ray r = Camera.current.ScreenPointToRay(//e.mousePosition);
            new Vector2(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y));
            //new Vector3(e.mousePosition.x, e.mousePosition.y));

            if (m_pollCreateNewWire)
            {
                if(Physics.Raycast(r, out RaycastHit hit))
                {
                    //Debug.DrawLine
                    Debug.DrawLine(r.origin, hit.point, Color.red, 1.0f);
                    //Gizmos.DrawCube(hit.point, Vector3.one);
                }
                //Debug.DrawRay(r.origin, r.direction * 100.0f, Color.green, 1.0f);
                m_pollCreateNewWire = false;
            }
        }
    }
}
