using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameGeometryDraw
{
	/// <summary>
	/// Draw some useful debug info for running in the build.
	/// </summary>
	static Material drawMaterial;
	static void CreateDebugMaterial()
	{
		if (drawMaterial)
			return;

		drawMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
		drawMaterial.hideFlags = HideFlags.HideAndDontSave;
		drawMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		drawMaterial.SetInt("_ZWrite", 0);
	}
	public static void DrawIndividualLines(Vector3[] points, Color color, Transform transform=null)
	{
		if(points.Length % 2 != 0)
		{
			Debug.LogError("Number of points must be divisible by two.");
			return;
		}
		CreateDebugMaterial();
		drawMaterial.SetPass(0);

		GL.PushMatrix();
		if(transform != null)
            GL.MultMatrix(transform.localToWorldMatrix);

		GL.Begin(GL.LINES);
		GL.Color(color);
		for(int i = 0; i < points.Length; i += 2)
		{
            GL.Vertex3(points[i].x, points[i].y, points[i].z);
            GL.Vertex3(points[i + 1].x, points[i + 1].y, points[i + 1].z);
		}
		GL.End();
		GL.PopMatrix();
	}
}
