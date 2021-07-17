using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
public class UICountdown : MonoBehaviour {
	private const int countFrom = 10;

	public Action onCountdownEnded;
	private Text countText;

	private void Awake() {
		countText = GetComponent<Text>();
	}

	public void count() {
		countText.enabled = true;
		StartCoroutine(countCoroutine());
	}

	private IEnumerator countCoroutine() {
		for (int i = countFrom; i >= 0; i++) {
			countText.text = i == 0 ? "GO" : i.ToString();
			yield return new WaitForSeconds(1f);
		}
		
		countText.enabled = false;
		onCountdownEnded();
	}
}
}