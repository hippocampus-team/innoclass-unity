using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Networking {
public class NetworkPlayersLeaderboardCollector : MonoBehaviour {
	private List<NetworkMirrorCarController> players;
	
	public static NetworkPlayersLeaderboardCollector instance;

	private void Awake() {
		instance = this;
		players = new List<NetworkMirrorCarController>();
	}

	public void initiate() {
		StartCoroutine(tickCoroutine());
	}

	public void addPlayer(NetworkMirrorCarController carController) {
		players.Add(carController);
	}
	
	public void removePlayerWithClientId(ulong clientId) {
		for (int i = 0; i < players.Count; i++)
			if (players[i].NetworkObject.OwnerClientId == clientId)
				players.RemoveAt(i);
	}

	private IEnumerator tickCoroutine() {
		while (true) {
			yield return new WaitForSeconds(0.5f);

			UILeaderboard.instance.arrangePlayers(
				players.Select(car => new KeyValuePair<string, float>(car.usernameAccessor, car.progressAccessor))
					.ToList());
		}
	}
}
}