using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadFrame : MonoBehaviour
{
    public float width { get; private set; }
    public float height { get; private set; }
    public float thickness { get; private set; }

    public static QuadFrame Create(Vector3 position, Quaternion orientation, float width, float height, float thickness)
    {
        GameObject gameObject = new GameObject("QuadFrame");
        gameObject.transform.position = position;
        gameObject.transform.rotation = orientation;
        QuadFrame quadFrame = gameObject.AddComponent<QuadFrame>();
        quadFrame.width = width;
        quadFrame.height = height;
        quadFrame.thickness = thickness;

        quadFrame.Build();

        return quadFrame;
    }

    private GameObject NewQuadNoCollider()
    {
        GameObject quad = new GameObject("Quad");
        Destroy(quad.GetComponent<MeshCollider>());
        return quad;
    }

    private void Build()
    {
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        GameObject quadTop = NewQuadNoCollider();
        quadTop.transform.position = this.transform.position - this.transform.up;
    }
}
