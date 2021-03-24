using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquares {

	int numValuesX;
	int numValuesY;

	List<Vector3> vertices;
	List<int> triangles;
	List<int> outlineVertexIndices;
	int[, ] vertexShareMap;

	public MarchingSquares (int numValuesX, int numValuesY) {
		this.numValuesX = numValuesX;
		this.numValuesY = numValuesY;

		vertices = new List<Vector3> ();
		triangles = new List<int> ();
		vertexShareMap = new int[(numValuesX + 1) * 2, (numValuesY + 1) * 2];
		outlineVertexIndices = new List<int> ();
	}

	public MeshData GenerateMesh (float[, ] map, float iso, float scale, bool smooth = false) {
		vertices.Clear ();
		triangles.Clear ();
		outlineVertexIndices.Clear ();
		System.Array.Clear (vertexShareMap, 0, vertexShareMap.Length);

		float halfScale = scale * 0.5f;

		for (int y = 0; y < numValuesY - 1; y++) {
			for (int x = 0; x < numValuesX - 1; x++) {

				int configuration = 0;
				configuration |= ((map[x, y] < iso) ? 1 : 0) << 0;
				configuration |= ((map[x + 1, y] < iso) ? 1 : 0) << 1;
				configuration |= ((map[x + 1, y + 1] < iso) ? 1 : 0) << 2;
				configuration |= ((map[x, y + 1] < iso) ? 1 : 0) << 3;

				float centreX = (-numValuesX / 2f + 1 + x) * scale;
				float centreY = (-numValuesY / 2f + 1 + y) * scale;

				// Create vertices
				for (int i = 0; i < TriangulationData.cellVertexIndices[configuration].Length; i++) {
					int cellVertexIndex = TriangulationData.cellVertexIndices[configuration][i];
					int currentVertexIndex;

					int vertexCoordX = x * 2 + 1 + TriangulationData.offsetsX[cellVertexIndex];
					int vertexCoordY = y * 2 + 1 + TriangulationData.offsetsY[cellVertexIndex];

					currentVertexIndex = vertexShareMap[vertexCoordX, vertexCoordY] - 1;

					// Vertex doesn't exist for re-use, so create new one
					if (currentVertexIndex == -1) {
						currentVertexIndex = vertices.Count;
						float vertexX = 0;
						float vertexY = 0;

						if (smooth && TriangulationData.IsMidpoint (cellVertexIndex)) {
							int cornerAX = TriangulationData.offsetsX[TriangulationData.GetCornerAFromMidpoint (cellVertexIndex)];
							int cornerAY = TriangulationData.offsetsY[TriangulationData.GetCornerAFromMidpoint (cellVertexIndex)];

							int cornerBX = TriangulationData.offsetsX[TriangulationData.GetCornerBFromMidpoint (cellVertexIndex)];
							int cornerBY = TriangulationData.offsetsY[TriangulationData.GetCornerBFromMidpoint (cellVertexIndex)];

							float isoA = map[x + (cornerAX + 1) / 2, y + (cornerAY + 1) / 2];
							float isoB = map[x + (cornerBX + 1) / 2, y + (cornerBY + 1) / 2];
							float t = Mathf.InverseLerp (isoA, isoB, iso);
							vertexX = Mathf.Lerp (cornerAX, cornerBX, t);
							vertexY = Mathf.Lerp (cornerAY, cornerBY, t);
						} else {
							vertexX = TriangulationData.offsetsX[cellVertexIndex];
							vertexY = TriangulationData.offsetsY[cellVertexIndex];
						}

						vertices.Add (new Vector2 (centreX + vertexX * halfScale, centreY + vertexY * halfScale));
						vertexShareMap[vertexCoordX, vertexCoordY] = currentVertexIndex + 1;
					}

					triangles.Add (currentVertexIndex);

					// Record which vertices are on the outline of the mesh to speed up edge collider generation
					if (TriangulationData.outlineFlags[configuration][i]) {
						outlineVertexIndices.Add (currentVertexIndex);
					}

				}
			}
		}
		return new MeshData () { vertices = vertices, triangles = triangles, outlineVertexIndices = outlineVertexIndices };
	}

}

public class MeshData {
	public List<Vector3> vertices;
	public List<int> triangles;
	// Pairs of two verts that lie on outline of mesh (used to speed up collider generation)
	public List<int> outlineVertexIndices;
}