using System.Collections.Generic;
using System.Linq;
using AI.Evolution;
using Cinemachine;
using Game.Car;
using Game.Track;
using MLAPI;
using Simulation;
using UI;
using UnityEditor;
using UnityEngine;

namespace Game {
public class GameStateManager : MonoBehaviour {
	// The camera object, to be referenced in Unity Editor.
	[SerializeField] private CinemachineTargetGroup cameraGroup;
	[SerializeField] private bool runMultiplayerAsHost;

	/// <summary>
	///     The UIController object.
	/// </summary>
	public UIController uiController { get; set; }

	public static GameStateManager instance { get; private set; }

	private void Awake() {
		if (instance != null) {
			Debug.LogError("Multiple GameStateManagers in the Scene.");
			return;
		}
		instance = this;
	}

	private void Start() {
		if (!isSimulationConfiguredCorrectly()) {
			EditorApplication.ExitPlaymode();
			return;
		}

		if (TrackConfiguration.instance.isNetworkedTrack) setupNetworking();
		if (TrackConfiguration.instance.isNetworkedTrack && runMultiplayerAsHost) {
			
		} else {
			TrackManager.instance.bestCarChanged += OnBestCarChanged;
			EvolutionManager.instance.startEvolution();
		}
	}

	private void setupNetworking() {
		if (runMultiplayerAsHost) NetworkManager.Singleton.StartHost();
		else NetworkManager.Singleton.StartClient();
	}

	private static bool isSimulationConfiguredCorrectly() {
		return ModelsManager.getInstance().isNumberOfActiveModelsValid();
	}
	
	public void onMirrorCarCreated(Transform carTransform) {
		if (!NetworkManager.Singleton.IsHost) return;
		List<CinemachineTargetGroup.Target> targets = cameraGroup.m_Targets.ToList();
		targets.Add(new CinemachineTargetGroup.Target {
			target = carTransform,
			weight = 1f,
			radius = 1f
		});
		cameraGroup.m_Targets = targets.ToArray();
	}
	
	private void OnBestCarChanged(CarController formerBestCar, CarController bestCar) {
		if (TrackConfiguration.instance.isNetworkedTrack && runMultiplayerAsHost) return;
		
		if (bestCar != null) {
			CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target {
				target = bestCar.transform,
				weight = 1f,
				radius = 1f
			};
			cameraGroup.m_Targets = new[] { target };
		}

		if (uiController != null) uiController.setDisplayTarget(bestCar);
	}
}
}