using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Barracuda;
using UnityEngine;

public class _BatchPaintGenerator : MonoBehaviour
{
    #region parmaters

    /* Neural network */
    const int LATENT_SIZE = 100;
    public NNModel modelAsset = null;
    private Model m_runtimeModel;
    IWorker m_worker;

    /* Texuture buffer */
    const int IMAGE_RES = 32;
    const int BATCH_SIZE = 256;
    public Action<RenderTexture[]> onInferenceCallback;
    RenderTexture[] m_textureBuffer = new RenderTexture[BATCH_SIZE];
    public int m_currentTextureBufferCount = 0;
    public int m_currentTextureBufferRequestCount = 0;
    #endregion

    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        modelAsset = Resources.Load<NNModel>("ML/generator_v1");

        m_runtimeModel = ModelLoader.Load(modelAsset);
        m_worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Compute, m_runtimeModel);

        for(int i = 0; i < BATCH_SIZE; ++i)
        {
            m_textureBuffer[i] = new RenderTexture(IMAGE_RES, IMAGE_RES, 3);
            m_textureBuffer[i].filterMode = FilterMode.Bilinear;
            m_textureBuffer[i].wrapMode = TextureWrapMode.Clamp;
        }

        StartCoroutine(InferenceIterator(3.0f));
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    #region texture callback
    public int AddNewTextureCallback(Action<RenderTexture[]> callbackFunc)
    {
        onInferenceCallback += callbackFunc;

        m_currentTextureBufferCount = Mathf.Min(m_currentTextureBufferCount + 1, BATCH_SIZE);
        int returnIndex = m_currentTextureBufferRequestCount % BATCH_SIZE;
        m_currentTextureBufferRequestCount++;

        return returnIndex;
    }

    void OutputToTextureAndCallback(Tensor output)
    {
        for(int i = 0; i < m_currentTextureBufferCount; ++i)
        {
            output.ToRenderTexture(m_textureBuffer[i], i);
        }

        onInferenceCallback?.Invoke(m_textureBuffer);
        m_currentTextureBufferCount = 0;
        m_currentTextureBufferRequestCount = 0;
    }
    #endregion

    #region neural network
    
    IEnumerator InferenceIterator(float delaySeconds)
    {
        while (true)
        {
            if(m_currentTextureBufferCount > 0)
            {
                DoInferenceStep();
            }

            yield return new WaitForSeconds(delaySeconds);
        }
    }

    Tensor GenerateNRandTensor(int batchSize, int latentSize)
    {
        Tensor input = new Tensor(batchSize, 1, 1, latentSize);
        for(int b = 0; b < batchSize; ++b)
        {
            for (int i = 0; i < latentSize; ++i)
                input[b, 0, 0, i] = RandomGaussian(-2.5f, 2.5f);
        }

        return input;
    }

    public void DoInferenceStep()
    {
        print($"Doing inference with {m_currentTextureBufferCount} of {BATCH_SIZE} max images. Number of requests: {m_currentTextureBufferRequestCount}");

        Tensor input = GenerateNRandTensor(m_currentTextureBufferCount, LATENT_SIZE);
        m_worker.Execute(input);
        Tensor output = m_worker.PeekOutput("Y");
        input.Dispose();

        OutputToTextureAndCallback(output);
    }
    #endregion

    #region math helpers
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
    #endregion

}

public class BatchPaintGenerator : Singleton<_BatchPaintGenerator> { }
