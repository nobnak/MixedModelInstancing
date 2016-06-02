using UnityEngine;
using System.Collections;

namespace MixedModelInstancing {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Cluster))]
	public class ClusterTester : MonoBehaviour {
		public int[] types;
		public int count = 100;
        public Vector3 size = new Vector3(0.1f, 0.1f, 3f);

        Transform[] _cache;
		Cluster.Item[] _items;
		Cluster _cluster;

		public void Build () {
			if (_cluster == null)
				_cluster = GetComponent<Cluster> ();
            
            if (_cache == null || _cache.Length < count)
                System.Array.Resize (ref _cache, count);

			_items = new Cluster.Item[count];
			for (var i = 0; i < _items.Length; i++) {
				var item = _items [i] = new Cluster.Item ();
                var tr = _cache [i];
                if (tr == null)
                    tr = _cache [i] = new GameObject ().transform;
                tr.hideFlags = HideFlags.HideInHierarchy;
				tr.SetParent (transform, false);
                tr.localPosition = Vector3.zero;
				tr.localRotation = Random.rotationUniform;
                tr.localScale = size;
				item.transform = tr;
				item.type = types [Random.Range (0, types.Length)];
			}
			_cluster.items = _items;
		}

		void Start() {
			Build ();
		}
		void OnValidate() {
			if (isActiveAndEnabled)
				Build ();
		}
	}
}
