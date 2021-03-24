using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class AntSettings : ScriptableObject {
	public float maxSpeed = 5;
	public float acceleration = 2;

	public float collisionAvoidSteerStrength = 5;
	public float targetSteerStrength = 3;
	public float randomSteerStrength = 1;
	public float randomSteerMaxDuration = 5;
	public float dstBetweenMarkers = 2;
	public float pheremoneEvaporateTime = 5;
	public float timeBetweenDirUpdate = 1;
	public float collisionRadius = 0.1f;
	public float antennaDst = 0.1f;
	public float homingForce = 0.1f;
	public float lifetime = 60;
	public bool useHomeMarkers;
	public bool useFoodMarkers;
	public bool useDeath;
	public float pheremoneRunOutTime = 35;

	public float pheremoneWeight;
	public float perceptionRad;

	[Header("New sensor settigns")]
		public float sensorSize = 2;
	public float sensorDst;
	public float sensorSpacing;
	
}