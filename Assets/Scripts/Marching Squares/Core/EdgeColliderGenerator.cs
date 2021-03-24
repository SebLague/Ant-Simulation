using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeColliderGenerator {

	Dictionary<int, OutlinePoint> outlinePoints = new Dictionary<int, OutlinePoint> ();
	List<Vector3> vertices;

	public Path[] Generate2DColliders (MeshData meshData) {
		this.vertices = meshData.vertices;

		outlinePoints.Clear ();
		for (int i = 0; i < meshData.outlineVertexIndices.Count; i += 2) {
			ProcessEdge (meshData.outlineVertexIndices[i], meshData.outlineVertexIndices[i + 1]);
		}

		// Set up edge colliders
		List<Path> paths = new List<Path> ();
		while (outlinePoints.Count > 0) {
			paths.Add (new Path (ExtractOutline ()));
		}

		return paths.ToArray ();

	}

	public static void SetColliders (GameObject gameObject, Path[] paths) {

		// Set up edge colliders
		var edgeColliders = gameObject.GetComponents<EdgeCollider2D> ();

		for (int i = 0; i < paths.Length; i++) {
			if (i < edgeColliders.Length) {
				edgeColliders[i].points = paths[i].points;
			} else {
				gameObject.AddComponent<EdgeCollider2D> ().points = paths[i].points;
			}
		}

		// Remove old, unused colliders
		for (int i = paths.Length; i < edgeColliders.Length; i++) {
			GameObject.Destroy (edgeColliders[i]);
		}

	}

	void ProcessEdge (int vertexIndexA, int vertexIndexB) {
		OutlinePoint outlinePointA;
		OutlinePoint outlinePointB;

		bool containsA = outlinePoints.TryGetValue (vertexIndexA, out outlinePointA);
		bool containsB = outlinePoints.TryGetValue (vertexIndexB, out outlinePointB);

		if (containsA && containsB) {
			outlinePointA.NextPoint = outlinePointB;
		} else if (containsA) {
			outlinePointB = new OutlinePoint (vertexIndexB);
			outlinePoints.Add (vertexIndexB, outlinePointB);
		} else if (containsB) {
			outlinePointA = new OutlinePoint (vertexIndexA);
			outlinePoints.Add (vertexIndexA, outlinePointA);
		} else {
			outlinePointA = new OutlinePoint (vertexIndexA);
			outlinePointB = new OutlinePoint (vertexIndexB);
			outlinePoints.Add (vertexIndexA, outlinePointA);
			outlinePoints.Add (vertexIndexB, outlinePointB);
		}
		outlinePointA.NextPoint = outlinePointB;
	}

	Vector2[] ExtractOutline () {

		OutlinePoint p = outlinePoints[outlinePoints.Keys.First<int> ()];;
		OutlinePoint startPoint = p;

		// Backtrack to find actual start of outline (if it's not cyclic)
		while (p.PreviousPoint != null && p.PreviousPoint.vertexIndex != startPoint.vertexIndex) {
			p = p.PreviousPoint;
		}
		startPoint = p;

		List<Vector2> edgePoints = new List<Vector2> ();

		do {
			outlinePoints.Remove (p.vertexIndex);

			Vector2 currentVertexPos = vertices[p.vertexIndex];
			edgePoints.Add (currentVertexPos);

			if (p.NextPoint == null) {
				break;
			}
			p = p.NextPoint;

		}
		while (p.vertexIndex != startPoint.vertexIndex);

		return edgePoints.ToArray ();
	}

	public class OutlinePoint {
		public int vertexIndex;
		OutlinePoint nextPoint;
		OutlinePoint previousPoint;

		public OutlinePoint (int vertexIndex) {
			this.vertexIndex = vertexIndex;
		}

		public OutlinePoint NextPoint {
			get {
				return nextPoint;
			}
			set {
				nextPoint = value;
				nextPoint.previousPoint = this;
			}
		}

		public OutlinePoint PreviousPoint {
			get {
				return previousPoint;
			}
			set {
				previousPoint = value;
				previousPoint.nextPoint = this;
			}
		}
	}
}

public class Path {
	public Vector2[] points;

	public Path (Vector2[] points) {
		this.points = points;
	}
}