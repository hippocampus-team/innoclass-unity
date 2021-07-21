using System.Collections.Generic;
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

	public void arrangePlayers(Dictionary<string, float> progress) {
		// Will activate container (has effect only on very first call)
		listContainer.gameObject.SetActive(true);
		
		List<KeyValuePair<string, float>> list = progress.ToList();
		list.Sort((v0, v1) => v0.Value - v1.Value > 0 ? 1 : -1);
		setLeaderboardSize(list.Count);

		for (int i = 0; i < list.Count; i++) {
			items[i].indexText.text = (i + 1).ToString();
			items[i].nameText.text = list[i].Key;
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
