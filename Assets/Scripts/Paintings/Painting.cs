using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    #region serialized parameters
    [SerializeField] public float width = 0.1f;
    [SerializeField] public float height = 0.1f;
    #endregion

    #region parameters
    public static float RESOLUTION = 100.0f;
    public static float DEPTH = 0.01f;
    public static float FRAME_WIDTH = 0.01f;

    private static GameObject baseObject = null;
    private static Painting baseObjectPainting = null;
    GameObject canvas = null;
    GameObject frameTop = null;
    GameObject frameBottom = null;
    GameObject painting = null;
    public RenderTexture paintingTexture { get; private set; }

    /* generator inference */
    int generatorRenderBufferIndex = -1;
    #endregion

    #region unity methods
    // Start is called before the first frame update
    private void OnDestroy()
    {
        if (paintingTexture != null)
            paintingTexture.Release();
    }
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
            baseObject.AddComponent<Grabbable>();
            baseObjectPainting = baseObject.AddComponent<Painting>();

            baseObject.AddComponent<Rigidbody>();

            baseObject.SetActive(false);
        }

        GameObject obj = GameObject.Instantiate(baseObject);
        obj.transform.position = position;
        obj.transform.rotation = orientation;
        Painting painting = obj.GetComponent<Painting>();
        painting.width = width;
        painting.height = height;
        painting.canvas = GameObject.CreatePrimitive(PrimitiveType.Cube);
        painting.frameTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        painting.frameBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
        painting.painting = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.SetActive(true);

        return painting;
    }

    private void CreateProceduralPainting()
    {
        float canvasWidth = 0.3f * DEPTH;
        // canvas

        UnityEngine.Profiling.Profiler.BeginSample("Painting::CreateCanvas");

        canvas.transform.localScale = new Vector3(width, height, canvasWidth);
        canvas.transform.SetParent(this.transform);
        canvas.transform.localPosition = new Vector3(0.0f, 0.0f, (DEPTH + canvasWidth * 0.5f) / 2.0f);
        canvas.transform.localRotation = Quaternion.identity;

        UnityEngine.Profiling.Profiler.EndSample();

        // frame.

        UnityEngine.Profiling.Profiler.BeginSample("Painting::CreateFrame");

            // top.
        frameTop.transform.localScale = new Vector3(width, FRAME_WIDTH, FRAME_WIDTH);
        frameTop.transform.SetParent(this.transform);
        frameTop.transform.localPosition = new Vector3(0.0f, (height - FRAME_WIDTH) / 2.0f, 0.0f);
        frameTop.transform.localRotation = Quaternion.identity;

            // bottom.
        frameBottom.transform.localScale = new Vector3(width, FRAME_WIDTH, FRAME_WIDTH);
        frameBottom.transform.SetParent(this.transform);
        frameBottom.transform.localPosition = new Vector3(0.0f, -(height - FRAME_WIDTH) / 2.0f, 0.0f);
        frameBottom.transform.localRotation = Quaternion.identity;

        UnityEngine.Profiling.Profiler.EndSample();

        // painting.

        UnityEngine.Profiling.Profiler.BeginSample("Painting::CreatePainting");

        painting.transform.localScale = new Vector3(width * 0.99f, height * 0.99f, canvasWidth);
        painting.transform.SetParent(this.transform);
        painting.transform.localPosition = new Vector3(0.0f, 0.0f, (DEPTH + canvasWidth) / 2.0f);
        painting.transform.localRotation = Quaternion.identity;

        UnityEngine.Profiling.Profiler.EndSample();

        // add new entry in generator buffer.
        generatorRenderBufferIndex = BatchPaintGenerator.Instance.AddNewTextureCallback(OnGeneratorInference);
    }
    #endregion

    #region generator inference
    public void OnGeneratorInference(RenderTexture[] renderTextureBuffer)
    {
        BatchPaintGenerator.Instance.onInferenceCallback -= OnGeneratorInference;

        paintingTexture = new RenderTexture(renderTextureBuffer[generatorRenderBufferIndex]);
        Graphics.CopyTexture(renderTextureBuffer[generatorRenderBufferIndex], paintingTexture);

        Renderer paintRenderer = painting.GetComponent<Renderer>();
        paintRenderer.material.mainTexture = paintingTexture;
    }
    #endregion
}
