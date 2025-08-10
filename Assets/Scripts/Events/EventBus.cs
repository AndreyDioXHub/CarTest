using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
	public static EventBus Instance { get; private set; }

	public UnityEvent<RaceState> OnRaceStateChanged = new UnityEvent<RaceState>();

	public UnityEvent<int> OnCheckpointPassed = new UnityEvent<int>();
	public UnityEvent<int> OnGhostCheckpointPassed = new UnityEvent<int>();
	public UnityEvent OnAllCheckPointsPassed = new UnityEvent();
	public UnityEvent<string> OnWin = new UnityEvent<string>();

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}
	}
}
