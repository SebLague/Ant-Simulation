using UnityEngine;

/*
	Contains functions for determining:
	• Triangle contains point
	• Triangle overlaps triangle

*/

public static class TriangleUtility {

	// Does triangle A-B-C contain point P?
	public static bool TriangleContainsPoint (Vector2 a, Vector2 b, Vector2 c, Vector2 p) {
		float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
		float s = 1 / (2 * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
		float t = 1 / (2 * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
		return s >= 0 && t >= 0 && (s + t) <= 1;
	}

	// Does triangle A1-A2-A3 overlap triangle B1-B2-B3
	public static bool TriangleOverlapsTriangle (Vector2 a1, Vector2 a2, Vector2 a3, Vector2 b1, Vector2 b2, Vector2 b3) {
		Vector2[][] segmentsA = {
			new Vector2[] { a1, a2 },
			new Vector2[] { a2, a3 },
			new Vector2[] { a3, a1 }
		};

		Vector2[][] segmentsB = {
			new Vector2[] { b1, b2 },
			new Vector2[] { b2, b3 },
			new Vector2[] { b3, b1 }
		};

		// If any of the two triangles' line segments intersect, then there's an overlap
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (MathUtility.LineSegmentsIntersect (segmentsA[i][0], segmentsA[i][1], segmentsB[j][0], segmentsB[j][1])) {
					return true;
				}
			}
		}
		// Check if one of the triangles completely contains the other
		if (TriangleContainsPoint (a1, a2, a3, b1) || TriangleContainsPoint (b1, b2, b3, a1)) {
			return true;
		}
		// No overlap found
		return false;
	}

}