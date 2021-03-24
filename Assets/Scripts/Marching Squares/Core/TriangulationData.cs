public static class TriangulationData {
	/* Diagram A (cell vertex indices)
	3–––––6–––––2
	|				|
	7				5
	|				|
	0–––––4–––––1
	*/

	// Triangulation data for every configuration: raw.githubusercontent.com/SebLague/Images/master/Marching%20squares%20cases.png
	// Values correspond to the vertex indices in Diagram A above, and each set of three represents a triangle
	public static int[][] cellVertexIndices = {
		new int[] { }, // case 0
		new int[] { 7, 4, 0 }, // case 1
		new int[] { 4, 5, 1 }, // case 2
		new int[] { 1, 0, 7, 7, 5, 1 }, // case 3
		new int[] { 5, 6, 2 }, // case 4
		new int[] { 4, 0, 7, 7, 6, 4, 6, 2, 5, 5, 4, 6 }, // case 5
		new int[] { 4, 6, 1, 1, 6, 2 }, // case 6
		new int[] { 7, 6, 0, 0, 6, 2, 2, 1, 0 }, // case 7
		new int[] { 6, 7, 3 }, // case 8
		new int[] { 0, 3, 4, 6, 4, 3 }, // case 9
		new int[] { 7, 3, 6, 6, 5, 7, 5, 1, 4, 4, 7, 5 }, // case 10
		new int[] { 6, 5, 3, 3, 5, 1, 1, 0, 3 }, // case 11
		new int[] { 3, 2, 7, 5, 7, 2 }, // case 12
		new int[] { 5, 4, 2, 2, 4, 0, 0, 3, 2 }, // case 13
		new int[] { 4, 7, 1, 1, 7, 3, 1, 3, 2 }, // case 14
		new int[] { 1, 0, 3, 3, 2, 1 }, // case 15
	};

	// Flags the vertex pairs which form an outline edge for the above triangulation data
	public static bool[][] outlineFlags = {
		new bool[] { }, // case 0
		new bool[] { true, true, false }, // case 1
		new bool[] { true, true, false }, // case 2
		new bool[] { false, false, false, true, true, false }, // case 3
		new bool[] { true, true, false }, // case 4
		new bool[] { false, false, false, true, true, false, false, false, false, true, true, false }, // case 5
		new bool[] { true, true, false, false, false, false }, // case 6
		new bool[] { true, true, false, false, false, false, false, false, false }, // case 7
		new bool[] { true, true, false }, // case 8
		new bool[] { false, false, false, true, true, false }, // case 9
		new bool[] { false, false, false, true, true, false, false, false, false, true, true, false }, // case 10
		new bool[] { true, true, false, false, false, false, false, false, false }, // case 11
		new bool[] { false, false, false, true, true, false }, // case 12
		new bool[] { true, true, false, false, false, false, false, false, false }, // case 13
		new bool[] { true, true, false, false, false, false, false, false, false }, // case 14
		new bool[] { false, false, false, false, false, false }, // case 15
	};

	// Offset from centre of each vertex
	public static readonly int[] offsetsX = {-1, 1, 1, -1, 0, 1, 0, -1 };
	public static readonly int[] offsetsY = {-1, -1, 1, 1, -1, 0, 1, 0 };

	// Is the given index one of the cell midpoints (index of 4, 5, 6, or 7)
	public static bool IsMidpoint (int index) {
		return (index & 4) != 0;
	}

	public static int GetCornerAFromMidpoint (int index) {
		return index - 4;
	}
	public static int GetCornerBFromMidpoint (int index) {
		return (index - 3) & ~4;
	}

}