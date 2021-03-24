using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BresenahmLine {
	// returns coords of tiles from given tile up to and including the target tile
	public static Vector2Int[] GetPath (int x, int y, int x2, int y2) {
		// bresenham line algorithm
		int w = x2 - x;
		int h = y2 - y;
		int absW = System.Math.Abs (w);
		int absH = System.Math.Abs (h);

		int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
		if (w < 0) {
			dx1 = -1;
			dx2 = -1;
		} else if (w > 0) {
			dx1 = 1;
			dx2 = 1;
		}
		if (h < 0) {
			dy1 = -1;
		} else if (h > 0) {
			dy1 = 1;
		}

		int longest = absW;
		int shortest = absH;
		if (longest <= shortest) {
			longest = absH;
			shortest = absW;
			if (h < 0) {
				dy2 = -1;
			} else if (h > 0) {
				dy2 = 1;
			}
			dx2 = 0;
		}

		int numerator = longest >> 1;
		Vector2Int[] path = new Vector2Int[longest + 1];
		for (int i = 0; i <= longest; i++) {
			path[i] = new Vector2Int (x, y);
			numerator += shortest;
			if (numerator >= longest) {
				numerator -= longest;
				x += dx1;
				y += dy1;
			} else {
				x += dx2;
				y += dy2;
			}
		}
		return path;
	}

}