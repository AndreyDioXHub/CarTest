using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
	[SerializeField] 
	private List<Checkpoint> _checkpoints = new List<Checkpoint>();
	[SerializeField]
	private string _winer = "";


	private int _nextCheckpointIndex;
	private RaceState _raceState;


	private void Start()
	{
		for (int i = 0; i < _checkpoints.Count; i++)
		{
			_checkpoints[i].Initialize(i);
		}

		EventBus.Instance.OnCheckpointPassed.AddListener(HandleCheckpoint);
		EventBus.Instance.OnGhostCheckpointPassed.AddListener(HandleGhostCheckpoint);
		EventBus.Instance.OnRaceStateChanged.AddListener(HandleRaceState);
	}

	private void HandleRaceState(RaceState state)
	{
		_raceState = state;
	}

	private void HandleCheckpoint(int index)
	{
		if (index == _checkpoints.Count - 1)
		{
			if (_raceState == RaceState.Lap2)
			{
				if (string.IsNullOrEmpty(_winer))
				{
					_winer = "player";
					EventBus.Instance.OnWin?.Invoke(_winer);
				}
			}
		}

		if (index == _nextCheckpointIndex)
		{
			_nextCheckpointIndex++;

			if (_nextCheckpointIndex >= _checkpoints.Count)
			{
				EventBus.Instance.OnAllCheckPointsPassed?.Invoke();

				_nextCheckpointIndex = 0;
			}
		}
	}

	private void HandleGhostCheckpoint(int index)
	{
		if (index == _checkpoints.Count - 1)
		{
			if (_raceState == RaceState.Lap2)
			{
				if (string.IsNullOrEmpty(_winer))
				{
					_winer = "ghost";
					EventBus.Instance.OnWin?.Invoke(_winer);
				}
			}
		}
	}
}