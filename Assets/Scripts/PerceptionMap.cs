using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionMap : MonoBehaviour {

	public Vector2 area;
	public AntSettings antSettings;
	public ParticleSystem particleDisplay;
	ParticleSystem.EmitParams particleEmitParams;
	float sqrPerceptionRadius;
	public Color pheremoneColor;
	public float pheremoneSize = 0.05f;
	public float initialAlpha = 1;

	int numCellsX;
	int numCellsY;
	Vector2 halfSize;
	float cellSizeReciprocal;
	Cell[, ] cells;

	void Awake () {
		Init ();
	}

	void Update () {
		particleEmitParams.startLifetime = antSettings.pheromoneEvaporateTime;
	}

	void Init () {
		float perceptionRadius = Mathf.Max (0.01f, antSettings.sensorSize);
		sqrPerceptionRadius = perceptionRadius * perceptionRadius;
		numCellsX = Mathf.CeilToInt (area.x / perceptionRadius);
		numCellsY = Mathf.CeilToInt (area.y / perceptionRadius);
		halfSize = new Vector2 (numCellsX * perceptionRadius, numCellsY * perceptionRadius) * 0.5f;
		cellSizeReciprocal = 1 / perceptionRadius;
		cells = new Cell[numCellsX, numCellsY];

		for (int y = 0; y < numCellsY; y++) {
			for (int x = 0; x < numCellsX; x++) {
				cells[x, y] = new Cell ();
			}
		}

		particleEmitParams.startLifetime = antSettings.pheromoneEvaporateTime;
		particleEmitParams.startSize = pheremoneSize;
		var m = particleDisplay.main;
		m.maxParticles = 100 * 1000;
		var c = particleDisplay.colorOverLifetime;
		c.enabled = true;

		Gradient grad = new Gradient ();
		grad.colorKeys = new GradientColorKey[] { new GradientColorKey (Color.white, 0), new GradientColorKey (Color.white, 1) };
		grad.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey (initialAlpha, 0.0f), new GradientAlphaKey (0.0f, 1.0f) };

		c.color = grad;
	}

	public void Add (Vector2 point, float initialWeight) {
		Vector2Int cellCoord = CellCoordFromPos (point);
		Cell cell = cells[cellCoord.x, cellCoord.y];
		Entry entry = new Entry () { position = point, creationTime = Time.time, initialWeight = initialWeight };
		cell.Add (entry);
		particleEmitParams.startColor = new Color (pheremoneColor.r, pheremoneColor.g, pheremoneColor.b, initialWeight);
		particleEmitParams.position = point;
		particleDisplay.Emit (particleEmitParams, 1);
	}

	public int GetAllInCircle (Entry[] result, Vector2 centre) {
		Vector2Int cellCoord = CellCoordFromPos (centre);
		int i = 0;
		float currentTime = Time.time;

		for (int offsetY = -1; offsetY <= 1; offsetY++) {
			for (int offsetX = -1; offsetX <= 1; offsetX++) {
				int cellX = cellCoord.x + offsetX;
				int cellY = cellCoord.y + offsetY;
				if (cellX >= 0 && cellX < numCellsX && cellY >= 0 && cellY < numCellsY) {
					Cell cell = cells[cellX, cellY];

					var currentEntryNode = cell.entries.First;
					while (currentEntryNode != null) {
						Entry currentEntry = currentEntryNode.Value;
						float currentLifetime = currentTime - currentEntry.creationTime;
						// Remove expired entries
						if (currentLifetime > antSettings.pheromoneEvaporateTime) {
							cell.entries.Remove (currentEntryNode);
						}
						// Check if entry is inside perception radius
						else if ((currentEntry.position - centre).sqrMagnitude < sqrPerceptionRadius) {
							if (i >= result.Length) {
								return result.Length;
							}
							result[i] = currentEntry;
							i++;

						}
						currentEntryNode = currentEntryNode.Next;
					}
				}
			}
		}
		return i;
	}

	Vector2Int CellCoordFromPos (Vector2 point) {
		int x = (int) ((point.x + halfSize.x) * cellSizeReciprocal);
		int y = (int) ((point.y + halfSize.y) * cellSizeReciprocal);
		return new Vector2Int (Mathf.Clamp (x, 0, numCellsX - 1), Mathf.Clamp (y, 0, numCellsY - 1));
	}
	/*
		void OnDrawGizmosSelected () {
			if (!Application.isPlaying) {
				Init ();
			}

			Vector2Int selected = CellCoordFromPos (transform.position);
			float width = cells.GetLength (0) * perceptionRadius;
			float height = cells.GetLength (1) * perceptionRadius;
			for (int y = 0; y < cells.GetLength (1); y++) {
				for (int x = 0; x < cells.GetLength (0); x++) {
					float centreX = -width / 2 + x * perceptionRadius + perceptionRadius / 2;
					float centreY = -height / 2 + y * perceptionRadius + perceptionRadius / 2;
					Gizmos.color = new Color (1, 1, 1, 0.1f);
					Gizmos.DrawWireCube (new Vector3 (centreX, centreY), new Vector3 (perceptionRadius, perceptionRadius));

					for (int offsetY = -1; offsetY <= 1; offsetY++) {
						for (int offsetX = -1; offsetX <= 1; offsetX++) {
							if (selected.x + offsetX == x && selected.y + offsetY == y) {
								if (offsetX == 0 && offsetY == 0) {
									Gizmos.color = new Color (1, 0, 0, 0.3f);
								} else {
									Gizmos.color = new Color (1, 0, 0, 0.1f);
								}
								Gizmos.DrawCube (new Vector3 (centreX, centreY), new Vector3 (perceptionRadius, perceptionRadius));
							}
						}
					}
				}
			}

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (transform.position, perceptionRadius);
		}
		*/

	public class Cell {
		public LinkedList<Entry> entries;

		public Cell () {
			entries = new LinkedList<Entry> ();
		}

		public void Add (Entry entry) {
			entries.AddLast (entry);

		}

	}

	public struct Entry {
		public Vector2 position;
		public float initialWeight;
		public float creationTime;
	}
}