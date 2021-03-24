using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntColony : MonoBehaviour {

	public AntSettings settings;
	public Ant antPrefab;
	public int numToSpawn = 10;
	public Transform antHolder;
	public bool replenishDead;

	public PerceptionMap homeMarkers;
	public PerceptionMap foodMarkers;
	float nextPossibleRespawnTime;
	public float radius;
	public Transform graphic;

	[Header ("Debug")]
	public int numFoodCollected;
	public float timePassed;
	bool hasPrinted10MinMark;
	public TextMesh numFoodUI;

	void Start () {
		Random.InitState (System.Environment.TickCount);
		for (int i = 0; i < numToSpawn; i++) {
			SpawnAnt ();
		}
	}

	void Update () {
		timePassed = Time.timeSinceLevelLoad;
		if (!hasPrinted10MinMark && timePassed > 60 * 10) {
			hasPrinted10MinMark = true;
			Debug.Log ("Num food collected: " + numFoodCollected);
		}

		int numDead = numToSpawn - antHolder.childCount;
		if (Time.time > nextPossibleRespawnTime) {
			nextPossibleRespawnTime = Time.time;
			if (numDead > 0 && replenishDead) {
				SpawnAnt ();
			}
		}
	}

	void SpawnAnt () {
		Ant ant = Instantiate (antPrefab, transform.position, Quaternion.identity, antHolder);
		ant.SetColony (this);
	}

	public void FoodCollected () {
		numFoodCollected++;
		numFoodUI.text = numFoodCollected + "";
	}

	void OnValidate () {
		graphic.transform.localScale = Vector3.one * radius * 2;
	}

}