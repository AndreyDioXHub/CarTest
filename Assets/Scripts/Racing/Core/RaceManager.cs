using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Ashsvp;

public enum RaceState { Lap1Preparation, Lap1, Lap1Finished, Lap2Preparation, Lap2, Lap2Finished }

public class RaceManager : MonoBehaviour
{
	public static RaceManager Instance;

	[Header("References")]

	[SerializeField]
	private GameObject _interactiveCar;
	[SerializeField]
	private GameObject _playerCar;
	[SerializeField]
	private GameObject _ghostCar;
	[SerializeField]
	private Transform _startPointPlayer;
	[SerializeField]
	private Transform _startPointGhost;

	private RaceState _currentState = RaceState.Lap1Preparation;
	private GhostSystem _ghostSystem;
	private List<RecordState> _ghostRecording;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		_ghostSystem = _interactiveCar.GetComponent<GhostSystem>();

		_playerCar.SetActive(true);
		_ghostCar.SetActive(false);

		SetState(RaceState.Lap1Preparation);

		EventBus.Instance.OnAllCheckPointsPassed.AddListener(FinishLap);
	}

	public void StartRace()
	{
		switch (_currentState)
		{
			case RaceState.Lap1Preparation:
				ResetCars();
				SetState(RaceState.Lap1);
				_ghostSystem.StartRecording();
				break;

			case RaceState.Lap2Preparation:
				SetState(RaceState.Lap2);
				ShowGhostCar();
				break;
		}
	}

	public async void FinishLap()
	{
		switch (_currentState)
		{
			case RaceState.Lap1:
				await UniTask.WaitForSeconds(0.1f);
				_ghostRecording = _ghostSystem.StopRecording();
				_playerCar.SetActive(false);
				_ghostCar.SetActive(true);
				SetState(RaceState.Lap1Finished);
				break;

			case RaceState.Lap2:
				SetState(RaceState.Lap2Finished);
				break;
		}
	}

	public async void PrepareToSecondRace()
	{
		_playerCar.SetActive(false);
		_ghostCar.SetActive(true);

		_interactiveCar.SetActive(false);
		await UniTask.Yield();
		var car = _interactiveCar.GetComponent<SimcadeVehicleController>();
		var carrb = _interactiveCar.GetComponent<Rigidbody>();
		car.enabled=false;
		carrb.isKinematic = true;
		await UniTask.Yield();
		_interactiveCar.transform.SetPositionAndRotation(_startPointPlayer.position, _startPointPlayer.rotation);
		await UniTask.Yield();
		carrb.linearVelocity = Vector3.zero;
		carrb.isKinematic = false;
		car.enabled = true;
		await UniTask.Yield();
		_interactiveCar.SetActive(true);

		SetState(RaceState.Lap2Preparation);
	}

	private void SetState(RaceState newState)
	{
		_currentState = newState;
		EventBus.Instance.OnRaceStateChanged?.Invoke(newState);
	}

	private void ShowGhostCar()
	{
		_ghostCar.SetActive(true);
		_ghostCar.GetComponent<GhostCar>().Initialize(_ghostRecording);
	}

	private void ResetCars()
	{
		_interactiveCar.transform.SetPositionAndRotation(_startPointGhost.position, _startPointGhost.rotation);
		_interactiveCar.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
	}
}