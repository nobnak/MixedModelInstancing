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

		Cluster _cluster;

		public void Build () {
			if (_cluster == null)
				_cluster = GetComponent<Cluster> ();

			Cluster.Item[] items = new Cluster.Item[count];
			for (var i = 0; i < items.Length; i++) {
				var item = items [i] = new Cluster.Item ();
				var tr = new GameObject ().transform;
				tr.SetParent (transform, false);
				tr.localPosition = radius * Random.onUnitSphere;
				tr.localRotation = Random.rotationUniform;
				tr.localScale = size * Vector3.one;
				item.transform = tr;
				item.type = types [Random.Range (0, types.Length)];
			}
			_cluster.items = items;
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
