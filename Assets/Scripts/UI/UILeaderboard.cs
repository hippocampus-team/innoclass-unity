using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace UI {
public class UILeaderboard : MonoBehaviour {
	[SerializeField] private RectTransform listContainer;
	[SerializeField] private GameObject listItemPrefab;
	[SerializeField] private GameObject leadingListItemPrefab;

	private List<UILeaderboardItem> items;
	private Dictionary<string, bool> winners;

	public static UILeaderboard instance;

	private void Awake() {
		instance = this;
		items = new List<UILeaderboardItem>();
		winners = new Dictionary<string, bool>();
	}

	public void arrangePlayers(List<KeyValuePair<string, float>> list) {
		// Will activate container (has effect only on very first call)
		listContainer.gameObject.SetActive(true);
		
		setLeaderboardSize(list.Count);
		list.Sort((v0, v1) => compareNumbers(v1.Value, v0.Value));

		for (int i = 0; i < list.Count; i++) {
			if (list[i].Value >= 1f && !winners.TryGetValue(list[i].Key, out bool _))
				winners.Add(list[i].Key, true);
			
			items[i].hasFinished = winners.TryGetValue(list[i].Key, out bool _);
			items[i].indexText.text = (i + 1).ToString();
			items[i].nameText.text = list[i].Key;
			items[i].progressText.text = list[i].Value.ToString(CultureInfo.InvariantCulture);
		}
	}

	private void setLeaderboardSize(int size) {
		if (items.Count == 0 && size != 0) {
			GameObject newItem = Instantiate(leadingListItemPrefab, listContainer);
			items.Add(newItem.GetComponent<UILeaderboardItem>());
		}
		
		while (items.Count > size)
			items.RemoveAt(items.Count - 1);

		while (items.Count < size) {
			GameObject newItem = Instantiate(listItemPrefab, listContainer);
			items.Add(newItem.GetComponent<UILeaderboardItem>());
		}
	}

	private static int compareNumbers(float a, float b) {
		float dif = a - b;
		if (dif > 0) return 1;
		if (dif < 0) return -1;
		return 0;
	}
}
}
