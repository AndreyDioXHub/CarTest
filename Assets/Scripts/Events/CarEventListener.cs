using Ashsvp;
using UnityEngine;

public class CarEventListener : MonoBehaviour
{
    [SerializeField]
    private SimcadeVehicleController _car;

	void Start()
    {
		_car.StopCar();

		EventBus.Instance.OnRaceStateChanged.AddListener(HandleRaceState);
	}

	private void HandleRaceState(RaceState state)
	{
		switch (state)
		{
			case RaceState.Lap1:
				_car.UnBunCar(); 
				break;
			case RaceState.Lap1Finished:
				_car.StopCar();
				break;
			case RaceState.Lap2:
				_car.UnBunCar();
				break;
			case RaceState.Lap2Finished:
				_car.StopCar();
				break;
		}
	}
}
