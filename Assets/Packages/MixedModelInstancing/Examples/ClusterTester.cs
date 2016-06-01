using UnityEngine;
using System.Collections;

namespace MixedModelInstancing {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Cluster))]
	public class ClusterTester : MonoBehaviour {
		public int[] types;
		public int count = 100;
		public float radius = 5f;
		public float size = 1f;

		Cluster.Item[] _items;
		Cluster _cluster;

		public void Build () {
			if (_cluster == null)
				_cluster = GetComponent<Cluster> ();

			if (_items != null)
				for (var i = 0; i < _items.Length; i++)
					if (_items[i] != null)
						Destroy(_items [i].transform.gameObject);
			_items = new Cluster.Item[count];
			for (var i = 0; i < _items.Length; i++) {
				var item = _items [i] = new Cluster.Item ();
				var tr = new GameObject ().transform;
				tr.hideFlags = HideFlags.DontSave;
				tr.SetParent (transform, false);
				tr.localPosition = radius * Random.onUnitSphere;
				tr.localRotation = Random.rotationUniform;
				tr.localScale = size * Vector3.one;
				item.transform = tr;
				item.type = types [Random.Range (0, types.Length)];
			}
			_cluster.items = _items;
		}

		void OnEnable() {
			Build ();
		}
		void OnValidate() {
			if (isActiveAndEnabled)
				Build ();
		}
	}
}
