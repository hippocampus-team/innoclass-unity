using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace UI {
public class UILeaderboard : MonoBehaviour {
	[SerializeField] private RectTransform listContainer;
	[SerializeField] private GameObject listItemPrefab;

	private List<UILeaderboardItem> items;

	public static UILeaderboard instance;

	private void Awake() {
		instance = this;
		items = new List<UILeaderboardItem>();
	}

	public void arrangePlayers(List<KeyValuePair<string, float>> list) {
		// Will activate container (has effect only on very first call)
		listContainer.gameObject.SetActive(true);
		
		list.Sort((v0, v1) => Convert.ToInt32(Math.Floor(v1.Value - v0.Value)));
		setLeaderboardSize(list.Count);

		for (int i = 0; i < list.Count; i++) {
			items[i].indexText.text = (i + 1).ToString();
			items[i].nameText.text = list[i].Key;
			items[i].progressText.text = list[i].Value.ToString(CultureInfo.InvariantCulture);
		}
	}

	private void setLeaderboardSize(int size) {
		while (items.Count > size)
			items.RemoveAt(items.Count - 1);

		while (items.Count < size) {
			GameObject newItem = Instantiate(listItemPrefab, listContainer);
			items.Add(newItem.GetComponent<UILeaderboardItem>());
		}
	}
}
}
