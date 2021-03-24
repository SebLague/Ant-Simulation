using UnityEngine;

public struct Bounds2D {
	public readonly Vector2 centre;
	public readonly Vector2 size;

	public readonly Vector2 min;
	public readonly Vector2 max;

	public readonly Vector2 topLeft;
	public readonly Vector2 bottomLeft;
	public readonly Vector2 topRight;
	public readonly Vector2 bottomRight;

	public Bounds2D (Vector2 centre, Vector2 size) {
		this.centre = centre;
		this.size = size;

		Vector2 halfSize = size / 2;

		min = centre - halfSize;
		max = centre + halfSize;

		topLeft = new Vector2 (min.x, max.y);
		bottomLeft = min;
		topRight = max;
		bottomRight = new Vector2 (max.x, min.y);
	}

	public Bounds2D (params Vector2[] points) {
		float minX = float.MaxValue;
		float minY = float.MaxValue;
		float maxX = float.MinValue;
		float maxY = float.MinValue;

		for (int i = 0; i < points.Length; i++) {
			Vector2 p = points[i];
			minX = Mathf.Min (minX, p.x);
			minY = Mathf.Min (minY, p.y);
			maxX = Mathf.Max (maxX, p.x);
			maxY = Mathf.Max (maxY, p.y);
		}

		min = new Vector2 (minX, minY);
		max = new Vector2 (maxX, maxY);
		bottomLeft = new Vector2 (minX, minY);
		topLeft = new Vector2 (minX, maxY);
		topRight = new Vector2 (maxX, maxY);
		bottomRight = new Vector2 (maxX, minY);

		size = new Vector2 (maxX - minX, maxY - minY);
		centre = (bottomLeft + topRight) * 0.5f;
	}

	public bool OverlapBounds (Bounds2D other) {
		if (other.min.x > max.x || min.x > other.max.x) {
			return false;
		}
		if (other.min.y > max.y || min.y > other.max.y) {
			return false;
		}
		return true;
	}

	public bool ContainsPoint (Vector2 point) {
		return point.x > min.x && point.x < max.x && point.y > min.y && point.y < max.y;
	}

	public void DebugDraw (Color drawCol) {
		Debug.DrawLine (bottomLeft, topLeft, drawCol);
		Debug.DrawLine (topLeft, topRight, drawCol);
		Debug.DrawLine (topRight, bottomRight, drawCol);
		Debug.DrawLine (bottomRight, bottomLeft, drawCol);
	}

}