using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class ManageGenerator : MonoBehaviour
{
    public NNModel modelAsset;
    private Model m_runtimeModel;
    IWorker m_worker;

    const int m_batchSize = 256;

    RenderTexture[] m_textOutput = new RenderTexture[m_batchSize];
    Material m_material;

    // Start is called before the first frame update
    void Start()
    {
        m_runtimeModel = ModelLoader.Load(modelAsset);
        m_worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Compute, m_runtimeModel);

        for(int i = 0; i < m_batchSize; ++i)
        {
            m_textOutput[i] = new RenderTexture(32, 32, 3);
            m_textOutput[i].filterMode = FilterMode.Bilinear;
            m_textOutput[i].wrapMode = TextureWrapMode.Clamp;
        }

        m_material = this.GetComponent<Renderer>().material;
    }

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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Tensor input = new Tensor(256, 1, 1, 100);
            for (int b = 0; b < m_batchSize; ++b)
            {
                for (int i = 0; i < 100; ++i)
                    input[b, 0, 0, i] = RandomGaussian(-2.5f, 2.5f);
            }
            m_worker.Execute(input);
            Tensor output = m_worker.PeekOutput("Y");
            input.Dispose();

            //ConvertOutputToTexture2D(output);
            for (int i = 0; i < m_batchSize; ++i)
                output.ToRenderTexture(m_textOutput[i], i);
            m_material.mainTexture = m_textOutput[Random.Range(0, m_batchSize)];
        }
    }

    //void ConvertOutputToTexture2D(Tensor output)
    //{
    //    //output.
    //    for(int i = 0; i < 32; ++i)
    //    {
    //        for(int j = 0; j < 32; ++j)
    //        {
    //            m_textOutput.SetPixel(i, j, new Color(
    //                output[0, i, j, 0],
    //                output[0, i, j, 1],
    //                output[0, i, j, 2],
    //                1.0f
    //                ));
    //        }
    //    }
    //}
}
