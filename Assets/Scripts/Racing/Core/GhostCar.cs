using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class GhostCar : MonoBehaviour
{
	[Header("Wheels")]
	[SerializeField] private List<Transform> _wheels = new List<Transform>();

	private List<RecordState> _recording;
	private CancellationTokenSource _cancellationTokenSource;
	private bool _isPlaying;
	private float _playbackSpeed = 1.0f;

	public void Initialize(List<RecordState> recording, float playbackSpeed = 1.0f)
	{
		_recording = recording;
		_playbackSpeed = playbackSpeed;
		_isPlaying = true;

		// Отменяем предыдущее воспроизведение если было
		_cancellationTokenSource?.Cancel();
		_cancellationTokenSource = new CancellationTokenSource();

		// Запускаем асинхронное воспроизведение
		Replay().Forget();
	}

	private async UniTaskVoid Replay()
	{
		if (_recording == null || _recording.Count == 0)
		{
			Debug.LogWarning("No recording to replay!");
			return;
		}

		try
		{
			float startTime = Time.time;
			float recordingStartTime = _recording[0].timestamp;
			int currentIndex = 0;

			while (currentIndex < _recording.Count - 1 && _isPlaying)
			{
				// Рассчитываем текущее время воспроизведения
				float elapsedTime = (Time.time - startTime) * _playbackSpeed;
				float currentPlaybackTime = recordingStartTime + elapsedTime;

				// Находим текущий сегмент для интерполяции
				while (currentIndex < _recording.Count - 2 &&
					   _recording[currentIndex + 1].timestamp < currentPlaybackTime)
				{
					currentIndex++;
				}

				// Получаем точки для интерполяции
				RecordState from = _recording[currentIndex];
				RecordState to = _recording[currentIndex + 1];

				// Рассчитываем коэффициент интерполяции
				float t = Mathf.InverseLerp(
					from.timestamp,
					to.timestamp,
					currentPlaybackTime
				);

				// Применяем интерполяцию
				ApplyInterpolation(from, to, t);

				// Ожидаем следующий кадр
				await UniTask.Yield(PlayerLoopTiming.FixedUpdate, _cancellationTokenSource.Token);
			}

			// Принудительно устанавливаем последнюю позицию
			if (_recording.Count > 0)
			{
				ApplyState(_recording[^1]);
			}
		}
		catch (System.OperationCanceledException)
		{
			// Воспроизведение было отменено
		}
		finally
		{
			_isPlaying = false;
		}
	}

	private void ApplyInterpolation(RecordState from, RecordState to, float t)
	{
		transform.position = Vector3.Lerp(from.position, to.position, t);
		transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, t);

		for (int i = 0; i < _wheels.Count; i++)
		{
			if (i < from.wheelRotations.Count && i < to.wheelRotations.Count)
			{
				_wheels[i].rotation = Quaternion.Slerp(
					from.wheelRotations[i],
					to.wheelRotations[i],
					t
				);
			}
		}
	}

	private void ApplyState(RecordState state)
	{
		transform.position = state.position;
		transform.rotation = state.rotation;

		for (int i = 0; i < _wheels.Count; i++)
		{
			if (i < state.wheelRotations.Count)
			{
				_wheels[i].rotation = state.wheelRotations[i];
			}
		}
	}

	public void StopPlayback()
	{
		_isPlaying = false;
		_cancellationTokenSource?.Cancel();
	}

	private void OnDestroy()
	{
		StopPlayback();
		_cancellationTokenSource?.Dispose();
	}
}