using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

namespace Game {
public class CameraManager : MonoBehaviour {
	[SerializeField] private new CinemachineVirtualCamera camera;
	[SerializeField] private CinemachineTargetGroup cameraGroup;
	
	public static CameraManager instance { get; private set; }

	private void Awake() {
		if (instance != null) {
			Debug.LogError("Multiple CameraManagers in the Scene!");
			return;
		}
		instance = this;
	}
	
	public void hardTrack(Transform targetTransform) {
		camera.Follow = targetTransform;
		camera.LookAt = targetTransform;
	}

	public void trackSolo(Transform targetTransform) {
		CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target {
			target = targetTransform,
			weight = 1f,
			radius = 1f
		};
		cameraGroup.m_Targets = new[] { target };
	}
	
	public void addToTrackingGroup(Transform targetTransform) {
		List<CinemachineTargetGroup.Target> targets = cameraGroup.m_Targets.ToList();
		targets.Add(new CinemachineTargetGroup.Target {
			target = targetTransform,
			weight = 1f,
			radius = 1f
		});
		cameraGroup.m_Targets = targets.ToArray();
	}
}
}