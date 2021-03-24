using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSource : MonoBehaviour {

	public float radius;
	public GameObject foodPrefab;
	public float timeBetweenSpawns;
	public int amount;
	public bool maintainAmount;
	public int blobCount = 3;
	public int seed;
	System.Random prng;
	Vector3[] blobs;

	void Awake () {
		Random.InitState (seed);
		prng = new System.Random (seed);
		blobs = new Vector3[blobCount + 1];
		blobs[0] = new Vector3 (transform.position.x, transform.position.y, radius);
		for (int i = 0; i < blobCount; i++) {
			Vector2 newPos = (Vector2) transform.position + Random.insideUnitCircle * radius;
			float newRad = radius * Mathf.Lerp (radius * 0.2f, radius * 0.5f, Random.value);
			blobs[i + 1] = new Vector3 (newPos.x, newPos.y, newRad);
		}
		for (int i = 0; i < amount; i++) {
			SpawnFood ();
		}
	}

	void Update () {
		if (transform.childCount < amount && maintainAmount) {
			SpawnFood ();
		}
	}

	void SpawnFood () {
		Vector3 blob = blobs[prng.Next (0, blobs.Length)];
		Vector2 centre = (Vector2) blob + Random.insideUnitCircle.normalized * blob.z * Mathf.Min (Random.value, Random.value);
		Instantiate (foodPrefab, centre, Quaternion.identity, transform);
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireSphere (transform.position, radius);
	}
}