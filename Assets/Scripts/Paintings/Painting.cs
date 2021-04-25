using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] float width = 0.1f;
    [SerializeField] float height = 0.1f;
    #endregion

    #region parameters
    public static float RESOLUTION = 100.0f;
    public static float DEPTH = 0.01f;
    public static float FRAME_WIDTH = 0.01f;

    private static GameObject baseObject = null;
    private static Painting baseObjectPainting = null;
    GameObject canvas = null;
    GameObject frameTop = null;
    GameObject painting = null;
    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        CreateProceduralPainting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region create
    public static Painting Init(Vector3 position, Quaternion orientation, float width, float height)
    {
        if(baseObject == null)
        {
            baseObject = new GameObject("painting");
            baseObjectPainting = baseObject.AddComponent<Painting>();
            //baseObjectPainting.canvas = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //baseObjectPainting.frameTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //baseObjectPainting.painting = GameObject.CreatePrimitive(PrimitiveType.Cube);

            baseObject.AddComponent<Rigidbody>();

            baseObject.SetActive(false);
        }

        GameObject obj = GameObject.Instantiate(baseObject);
        obj.transform.position = position;
        obj.transform.rotation = orientation;
        Painting painting = obj.GetComponent<Painting>();
        painting.width = width;
        painting.height = height;
        //painting.canvas = GameObject.Instantiate(baseObjectPainting.canvas);
        //painting.frameTop = GameObject.Instantiate(baseObjectPainting.frameTop);
        //painting.painting = GameObject.Instantiate(baseObjectPainting.painting);
        painting.canvas = GameObject.CreatePrimitive(PrimitiveType.Cube);
        painting.frameTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        painting.painting = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.SetActive(true);

        return painting;
    }

    private void CreateProceduralPainting()
    {
        float canvasWidth = 0.2f * DEPTH;

        UnityEngine.Profiling.Profiler.BeginSample("Painting::CreateCanvas");
        // canvas
        canvas.transform.localScale = new Vector3(width, height, canvasWidth);
        canvas.transform.SetParent(this.transform);
        canvas.transform.localPosition = new Vector3(0.0f, 0.0f, (DEPTH + canvasWidth * 0.5f) / 2.0f);
        canvas.transform.localRotation = Quaternion.identity;

        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Painting::CreateFrame");
        // frame.
            // top.
        frameTop.transform.localScale = new Vector3(width, FRAME_WIDTH, FRAME_WIDTH);
        frameTop.transform.SetParent(this.transform);
        frameTop.transform.localPosition = new Vector3(0.0f, (height - FRAME_WIDTH) / 2.0f, 0.0f);
        frameTop.transform.localRotation = Quaternion.identity;

        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Painting::CreatePainting");
        // painting.
        painting.transform.localScale = new Vector3(width * 0.99f, height * 0.99f, canvasWidth);
        painting.transform.SetParent(this.transform);
        painting.transform.localPosition = new Vector3(0.0f, 0.0f, (DEPTH + canvasWidth) / 2.0f);
        painting.transform.localRotation = Quaternion.identity;
        Renderer paintRenderer = painting.GetComponent<Renderer>();
        paintRenderer.material.mainTexture = SimplePaintGenerator.Generate((int)(width * RESOLUTION), (int)(height * RESOLUTION));

        UnityEngine.Profiling.Profiler.EndSample();
    }
    #endregion
}
