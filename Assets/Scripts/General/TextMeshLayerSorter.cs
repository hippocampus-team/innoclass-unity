using UnityEngine;

namespace General {
public class TextMeshLayerSorter : MonoBehaviour {
	[SerializeField] private int sortingOrder;
	
	private void Awake() {
		GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
	}
}
}
