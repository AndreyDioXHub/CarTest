using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

	[SerializeField] 
	private int _index;
	[SerializeField] 
	private MeshRenderer _renderer;
	[SerializeField] 
	private Color _activeColor = Color.green;
	[SerializeField] 
	private Color _inactiveColor = Color.gray;

	public void Initialize(int index)
	{
		_index = index;
		SetActive(false);

		EventBus.Instance.OnRaceStateChanged.AddListener(HandleRaceState);
	}

	private void HandleRaceState(RaceState state)
	{
		switch (state)
		{
			case RaceState.Lap1:
				break;
			case RaceState.Lap1Finished:
				SetActive(false);
				break;
			case RaceState.Lap2:
				break;
			case RaceState.Lap2Finished:
				break;
		}
	}

	private void SetActive(bool active)
	{
		_renderer.material.color = active ? _activeColor : _inactiveColor;
	}

	private void OnTriggerEnter(Collider other)
	{
		switch (other.tag)
		{
			case "Player":
				EventBus.Instance.OnCheckpointPassed?.Invoke(_index);
				SetActive(true);
				break;
			case "GhostCar":
				EventBus.Instance.OnGhostCheckpointPassed?.Invoke(_index);
				break;
		}
	}
}