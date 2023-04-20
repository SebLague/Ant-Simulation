using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AntSettings : ScriptableObject
{
	[Header("Movement")]
	public float maxSpeed = 2;
	public float acceleration = 3;
	public float collisionAvoidSteerStrength = 5;
	public float targetSteerStrength = 3;
	public float randomSteerStrength = 0.6f;
	public float randomSteerMaxDuration = 1;
	public float timeBetweenDirUpdate = 0.15f;
	public float collisionRadius = 0.15f;

	[Header("Pheromones")]
	public float dstBetweenMarkers = 0.75f;
	public float pheromoneEvaporateTime = 45;
	public float pheromoneRunOutTime = 30;
	public float pheromoneWeight = 1;
	public float perceptionRadius = 2.5f;
	public bool useHomeMarkers = true;
	public bool useFoodMarkers = true;

	[Header("Sensing")]
	public float sensorSize = 0.75f;
	public float sensorDst = 1.25f;
	public float sensorSpacing = 1;
	public float antennaDst = 0.25f;

	[Header("Lifetime")]
	public float lifetime = 150;
	public bool useDeath = false;
}