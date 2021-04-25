using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float smoothRate = 0.5f;

    [SerializeField]
    Vector2 position;

    [SerializeField]
    Text displayText;

    [SerializeField]
    bool drawDebugFPS = false;

    float fps = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentFPS = GetComputeFPS();
        fps = Mathf.Lerp(currentFPS, fps, smoothRate);

        if (displayText != null)
            displayText.text = this.ToString();
    }

    public override string ToString()
    {
        return string.Format("FPS: {0}", fps);
    }

    float GetComputeFPS()
    {
        return 1.0f / Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        //Vector3 nPos = Camera.main.WorldToScreenPoint(new Vector3(position.x, position.y, 0.0f));
        //Handles.Label(nPos, string.Format("FPS: {0}", fps));
        if(drawDebugFPS)
            Handles.Label(new Vector3(position.x, position.y, 0.0f), this.ToString());
#endif
    }
}
