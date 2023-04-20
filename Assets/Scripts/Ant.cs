using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
	public enum State { SearchingForFood, ReturningHome }

	public AntSettings settings;
	public Transform head;
	public LayerMask foodMask;
	public LayerMask homeMask;
	public LayerMask collisionMask;


	public Transform antennaLeft;
	public Transform antennaRight;
	public Transform perceptionCentre;

	State currentState;

	Vector2 currentVelocity;
	Vector2 collisionAvoidForce;

	float nextRandomSteerTime;

	Vector2[] sensors = new Vector2[3];
	float[] sensorData = new float[3];

	Transform collectedFood;

	Vector2 lastPheromonePos;
	Collider2D[] foodColliders;
	PerceptionMap.Entry[] pheromoneEntries;
	AntColony colony;
	float nextDirUpdateTime;

	Vector2 randomSteerForce;
	Vector2 pheromoneSteerForce;

	// State
	Vector2 currentForwardDir;
	Vector2 currentPosition;
	float colDst;
	Vector2 obstacleAvoidForce;
	float obstacleForceResetTime;
	bool antennaCollisionLastFrame;
	Vector2 homePos;

	enum Antenna { None, Left, Right }
	Antenna lastAntennaCollision;
	bool foodInSight;
	Transform targetFood;
	float deathTime;
	bool turningAround;
	Vector2 turnAroundForce;
	float turnAroundEndTime;

	float leftHomeTime;
	float leftFoodTime;

	public void SetColony(AntColony colony)
	{
		this.colony = colony;
	}

	void Start()
	{
		lastPheromonePos = transform.position;
		currentState = State.SearchingForFood;
		transform.eulerAngles = Vector3.forward * Random.value * 360;
		currentForwardDir = transform.right;
		currentPosition = transform.position;
		currentVelocity = currentForwardDir * settings.maxSpeed;

		foodColliders = new Collider2D[1];
		homePos = transform.position;

		const int maxPerceivedPheromones = 1024;
		pheromoneEntries = new PerceptionMap.Entry[maxPerceivedPheromones];
		nextDirUpdateTime = Random.value * settings.timeBetweenDirUpdate;
		colDst = settings.collisionRadius / 2f;
		deathTime = Time.time + settings.lifetime + Random.Range(0, settings.lifetime / 2f);
		leftHomeTime = Time.time;
	}

	void Update()
	{
		if (Time.time > deathTime && settings.useDeath)
		{
			Destroy(gameObject);
		}

		HandlePheromonePlacement();
		HandleRandomSteering();

		if (currentState == State.SearchingForFood)
		{
			HandleSearchForFood();
		}
		else if (currentState == State.ReturningHome)
		{
			HandleReturnHome();
		}

		HandleCollisionSteering();
		HandleMovement();
	}


	void HandleMovement()
	{
		Vector2 steerForce = randomSteerForce + pheromoneSteerForce + obstacleAvoidForce;

		if (turningAround)
		{
			steerForce += turnAroundForce * settings.targetSteerStrength;
			if (Time.time > turnAroundEndTime)
			{
				turningAround = false;
			}
		}

		Vector2 desiredVelocity = steerForce.normalized * settings.maxSpeed;
		SteerTowards(desiredVelocity);

		currentForwardDir = currentVelocity.normalized;
		float moveDst = currentVelocity.magnitude * Time.deltaTime;
		Vector2 desiredPos = currentPosition + currentVelocity * Time.deltaTime;

		RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentForwardDir, Mathf.Max(settings.collisionRadius, moveDst), collisionMask);
		if (hit)
		{
			if (!turningAround)
			{
				StartTurnAround(Vector2.Reflect(currentForwardDir, hit.normal), 2);
			}
			desiredPos = hit.point - currentForwardDir * settings.collisionRadius;
		}

		currentPosition = desiredPos;
		transform.SetPositionAndRotation(new Vector3(currentPosition.x, currentPosition.y, -0.1f), Quaternion.FromToRotation(Vector3.right, currentForwardDir));
	}

	void HandleCollisionSteering()
	{
		RaycastHit2D hitLeft = Physics2D.Raycast(antennaLeft.position, antennaLeft.right, settings.antennaDst, collisionMask);
		RaycastHit2D hitRight = Physics2D.Raycast(antennaRight.position, antennaRight.right, settings.antennaDst, collisionMask);
		//Debug.DrawRay (antennaLeft.position, antennaLeft.right * ((hitLeft) ? hitLeft.distance : settings.antennaDst), (hitLeft) ? Color.red : Color.green);
		//Debug.DrawRay (antennaRight.position, antennaRight.right * ((hitRight) ? hitRight.distance : settings.antennaDst), (hitRight) ? Color.red : Color.green);

		if (Time.time > obstacleForceResetTime)
		{
			obstacleAvoidForce = Vector2.zero;
			lastAntennaCollision = Antenna.None;
		}

		if (hitLeft || hitRight)
		{
			if (hitLeft && lastAntennaCollision != Antenna.Right && (!hitRight || hitLeft.distance < hitRight.distance))
			{
				obstacleAvoidForce = -transform.up * settings.collisionAvoidSteerStrength;
				lastAntennaCollision = Antenna.Left;
			}
			if (hitRight && lastAntennaCollision != Antenna.Left && (!hitLeft || hitRight.distance < hitLeft.distance))
			{
				obstacleAvoidForce = transform.up * settings.collisionAvoidSteerStrength;
				lastAntennaCollision = Antenna.Right;
			}

			obstacleForceResetTime = Time.time + 0.5f;
			randomSteerForce = obstacleAvoidForce.normalized * settings.randomSteerStrength;
		}
	}

	void SteerTowards(Vector2 desiredVelocity)
	{
		Vector2 steeringForce = desiredVelocity - currentVelocity;

		Vector2 acceleration = Vector2.ClampMagnitude(steeringForce * settings.acceleration, settings.acceleration);
		currentVelocity += acceleration * Time.deltaTime;
		currentVelocity = Vector2.ClampMagnitude(currentVelocity, settings.maxSpeed);
	}

	void HandleReturnHome()
	{
		Vector2 currentPos = transform.position;
		Collider2D home = Physics2D.OverlapCircle(perceptionCentre.position, settings.perceptionRadius, homeMask);
		if (home)
		{
			pheromoneSteerForce = ((Vector2)home.transform.position - currentPos).normalized * settings.targetSteerStrength;

			if (Vector2.SqrMagnitude(currentPos - homePos) < colony.radius * colony.radius)
			{
				deathTime = Time.time + settings.lifetime;
				Destroy(collectedFood.gameObject);
				currentState = State.SearchingForFood;
				nextDirUpdateTime = 0;
				StartTurnAround();
				colony.FoodCollected();
				leftHomeTime = Time.time;
			}
		}
		else
		{
			HandlePheromoneSteering();
		}

	}

	void StartTurnAround(Vector2 returnDir, float randomStrength = 0.2f)
	{
		turningAround = true;
		turnAroundEndTime = Time.time + 1.5f;
		Vector2 perpAxis = new Vector2(-returnDir.y, returnDir.x);
		turnAroundForce = returnDir + perpAxis * (Random.value - 0.5f) * 2 * randomStrength;
	}

	void StartTurnAround(float randomStrength = 0.2f)
	{
		StartTurnAround(-currentForwardDir, randomStrength);
	}

	void HandleSearchForFood()
	{
		if (colony)
		{
			if (Vector2.SqrMagnitude(currentPosition - homePos) < colony.radius * colony.radius)
			{
				deathTime = Time.time + settings.lifetime;
				leftHomeTime = Time.time;
			}
		}


		if (targetFood == null)
		{
			int numFoodInRadius = Physics2D.OverlapCircleNonAlloc(perceptionCentre.position, settings.perceptionRadius, foodColliders, foodMask);
			if (numFoodInRadius > 0)
			{
				targetFood = foodColliders[Random.Range(0, numFoodInRadius)].transform;
				targetFood.gameObject.layer = 0;
			}
		}

		if (targetFood != null)
		{
			Vector2 offsetToFood = targetFood.transform.position - transform.position;
			float dstToFood = offsetToFood.magnitude;
			Vector2 dirToFood = offsetToFood / dstToFood;
			pheromoneSteerForce = dirToFood * settings.targetSteerStrength;
			if (dstToFood < targetFood.transform.localScale.x * 1f)
			{

				collectedFood = targetFood.transform;
				targetFood.position = head.position;
				targetFood.SetParent(transform, true);
				targetFood.gameObject.layer = 0;
				currentState = State.ReturningHome;
				nextDirUpdateTime = 0;
				targetFood = null;
				StartTurnAround();
				leftFoodTime = Time.time;
			}
		}
		else
		{
			HandlePheromoneSteering();
		}

	}

	void HandlePheromonePlacement()
	{
		if (Vector2.Distance(transform.position, lastPheromonePos) > settings.dstBetweenMarkers)
		{
			if (currentState == State.SearchingForFood && settings.useHomeMarkers && (Time.time - leftHomeTime) < settings.pheromoneRunOutTime)
			{
				float t = 1 - (Time.time - leftHomeTime) / settings.pheromoneRunOutTime;
				t = Mathf.Lerp(0.5f, 1, t);
				colony.homeMarkers.Add(transform.position, t);
				lastPheromonePos = transform.position + (Vector3)Random.insideUnitCircle * settings.dstBetweenMarkers * 0.2f;
			}
			else if (currentState == State.ReturningHome && settings.useFoodMarkers && (Time.time - leftFoodTime) < settings.pheromoneRunOutTime)
			{
				float t = 1 - (Time.time - leftFoodTime) / settings.pheromoneRunOutTime;
				t = Mathf.Lerp(0.5f, 1, t);
				colony.foodMarkers.Add(transform.position, t);
				lastPheromonePos = transform.position + (Vector3)Random.insideUnitCircle * settings.dstBetweenMarkers * 0.2f;
			}
		}
	}

	void HandlePheromoneSteering()
	{
		if (Time.time > nextDirUpdateTime)
		{
			Vector2 leftSensorDir = (currentForwardDir + (Vector2)transform.up * settings.sensorDst).normalized;
			Vector2 rightSensorDir = (currentForwardDir - (Vector2)transform.up * settings.sensorDst).normalized;

			pheromoneSteerForce = Vector2.zero;
			float currentTime = Time.time;
			const int centreIndex = 0;
			const int leftIndex = 1;
			const int rightIndex = 2;
			nextDirUpdateTime = Time.time + settings.timeBetweenDirUpdate;
			// centre
			sensors[centreIndex] = currentPosition + currentForwardDir * settings.sensorDst;
			// left
			sensors[leftIndex] = currentPosition + leftSensorDir * settings.sensorDst;
			// right
			sensors[rightIndex] = currentPosition + rightSensorDir * settings.sensorDst;

			for (int i = 0; i < 3; i++)
			{
				sensorData[i] = 0;
				int numPheromones = 0;
				if (currentState == State.SearchingForFood && settings.useFoodMarkers)
				{
					numPheromones = colony.foodMarkers.GetAllInCircle(pheromoneEntries, sensors[i]);
				}
				if (currentState == State.ReturningHome && settings.useHomeMarkers)
				{
					numPheromones = colony.homeMarkers.GetAllInCircle(pheromoneEntries, sensors[i]);
				}
				for (int j = 0; j < numPheromones; j++)
				{
					float evaporateT = ((currentTime - pheromoneEntries[j].creationTime) / settings.pheromoneEvaporateTime);
					float strength = Mathf.Clamp01(1 - evaporateT);
					//strength = strength * strength;
					sensorData[i] += strength;
				}
			}

			float centre = sensorData[centreIndex];
			float left = sensorData[leftIndex];
			float right = sensorData[rightIndex];

			if (centre > left && centre > right)
			{
				pheromoneSteerForce = currentForwardDir * settings.pheromoneWeight;
			}
			else if (left > right)
			{
				pheromoneSteerForce = leftSensorDir * settings.pheromoneWeight;
			}
			else if (right > left)
			{
				pheromoneSteerForce = rightSensorDir * settings.pheromoneWeight;
			}

		}

	}

	void HandleRandomSteering()
	{
		if (targetFood != null)
		{
			randomSteerForce = Vector2.zero;
			return;
		}

		if (Time.time > nextRandomSteerTime)
		{
			nextRandomSteerTime = Time.time + Random.Range(settings.randomSteerMaxDuration / 3, settings.randomSteerMaxDuration);
			randomSteerForce = GetRandomDir(currentForwardDir, 5) * settings.randomSteerStrength;
		}

	}

	Vector2 GetRandomDir(Vector2 referenceDir, int similarity = 4)
	{
		Vector2 smallestRandomDir = Vector2.zero;
		float change = -1;
		const int iterations = 4;
		for (int i = 0; i < iterations; i++)
		{
			Vector2 randomDir = Random.insideUnitCircle.normalized;
			float dot = Vector2.Dot(referenceDir, randomDir);
			if (dot > change)
			{
				change = dot;
				smallestRandomDir = randomDir;
			}
		}
		return smallestRandomDir;
	}
}