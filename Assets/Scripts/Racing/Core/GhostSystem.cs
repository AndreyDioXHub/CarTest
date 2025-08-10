using System;
using System.Collections.Generic;
using UnityEngine;

public class GhostSystem : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float _recordInterval = 0.05f;

	[Header("Wheels")]
	[SerializeField] private List<Transform> _wheels = new List<Transform>();

	private bool _isRecording;
	private float _timer;
	private List<RecordState> _recording = new List<RecordState>();

	public void StartRecording()
	{
		_recording.Clear();
		_isRecording = true;
		_timer = 0f;
	}

	public List<RecordState> StopRecording()
	{
		_isRecording = false;
		return new List<RecordState>(_recording);
	}

	private void FixedUpdate()
	{
		if (!_isRecording) return;

		_timer += Time.fixedDeltaTime;

		if (_timer >= _recordInterval)
		{
			RecordSnapshot();
			_timer = 0;
		}
	}

	private void RecordSnapshot()
	{
		var state = new RecordState
		{
			position = transform.position,
			rotation = transform.rotation,
			timestamp = Time.time
		};

		foreach (var wheel in _wheels)
		{
			state.wheelRotations.Add(wheel.rotation);
		}

		_recording.Add(state);
	}
}

[Serializable]
public class RecordState
{
	public Vector3 position;
	public Quaternion rotation;
	public List<Quaternion> wheelRotations = new List<Quaternion>();
	public float timestamp; 
}