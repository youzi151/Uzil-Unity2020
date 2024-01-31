using System.Collections.Generic;
using UnityEngine;

using Triangulator = Uzil.OtherLib.Triangulator;

namespace Uzil.Util {

public class MeshUtil {

	public static Vector2[] PixelUV (Vector2[] pixelUV, int textureWidth, int textureHeight) {
		
		Vector2[] cauculateUV = new Vector2[pixelUV.Length];
		
		// 依序計算每一個像素UV 設為 百分比UV
		for (int idx = 0; idx < pixelUV.Length; idx++) {

			Vector2 point_pixel = pixelUV[idx];
			Vector2 point_percent = new Vector2(
				point_pixel.x / textureWidth,
				point_pixel.y / textureHeight
			);

			cauculateUV[idx] = point_percent;
		}
		
		return cauculateUV;
	}
	
	protected static int[] CubeTriangles = new int[]{
		0, 2, 1, //face front
		0, 3, 2,
		2, 3, 4, //face top
		2, 4, 5,
		1, 2, 5, //face right
		1, 5, 6,
		0, 7, 4, //face left
		0, 4, 3,
		5, 4, 7, //face back
		5, 7, 6,
		0, 6, 7, //face bottom
		0, 1, 6
	};
	public static Mesh CubeMesh (Vector3[] vertices, Vector2[] uv = null, Mesh existMesh = null) {
		if (vertices.Length != 8) {
			Debug.Log("[MeshUtil] invalid vertice Count for cube");
			return null;
		}

		Mesh mesh = existMesh == null ? new Mesh() : existMesh;

		mesh.vertices = vertices;
		mesh.triangles = MeshUtil.CubeTriangles;

		if (uv == null && mesh.uv == null) {
			uv = new Vector2[vertices.Length];
			for (int idx = 0; idx < uv.Length; idx++) {
				uv[idx] = new Vector2(vertices[idx].x, vertices[idx].y);
			}
		}

		if (uv != null) {
			mesh.uv = uv;
		}

		mesh.Optimize();
		mesh.RecalculateTangents();

		return mesh;
	}

	public static Mesh SpriteMesh (Vector2[] pointList, Vector2[] uvList = null, Mesh existMesh = null) {

		Mesh mesh = existMesh == null ? new Mesh() : existMesh;


		Triangulator tri = new Triangulator(pointList);
		List<int> indices = tri.Triangulate();

		List<Vector3> vertices = new List<Vector3>();
		for (int i = 0; i < pointList.Length; i++) {
			vertices.Add(new Vector3(pointList[i].x, pointList[i].y, 0));
		}

		// MeshCollider所使用的Mesh一定要能夠讓他可以建立有體積的collider，否則形狀會變成以該Mesh上下左右最大值建立的Box形狀Collider
		// 所以可以把一個vertex的z稍微錯開 
		Vector3 altStart = vertices[0];
		altStart.z = 0.0001f;
		vertices[0] = altStart;

		
		Vector3[] normals = new Vector3[vertices.Count];
		for (int idx = 0; idx < normals.Length; idx++) {
			normals[idx] = new Vector3(0, 0, -1);
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.normals = normals;

		Vector2[] uv;
		if (uvList != null) {
			uv = uvList;
		} else {
			uv = new Vector2[vertices.Count];
			for (int idx = 0; idx < uv.Length; idx++) {
				uv[idx] = new Vector2(vertices[idx].x, vertices[idx].y);
			}
		}

		mesh.uv = uv;

		mesh.Optimize();
		mesh.RecalculateTangents();

		return mesh;
	}

}

}