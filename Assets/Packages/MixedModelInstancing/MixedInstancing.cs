using UnityEngine;
using System.Collections;

namespace MixedModelInstancing {

    [ExecuteInEditMode]
    public class MixedInstancing : MonoBehaviour {
        public Instancing[] instancings;
        public Cluster[] clusters;

        int[] _countInInstancings;
        Instancing.Item[][] _itemsInInstancings;

        void Update() {
            for (var i = 0; i < clusters.Length; i++)
                clusters [i].ManualUpdate ();

            if (_countInInstancings == null || _countInInstancings.Length != instancings.Length)
                _countInInstancings = new int[instancings.Length];
            System.Array.Clear (_countInInstancings, 0, _countInInstancings.Length);
            for (var i = 0; i < clusters.Length; i++) {
                var cluster = clusters [i];
                for (var j = 0; j < instancings.Length; j++)
                    _countInInstancings [j] += cluster.Count (j);
            }

            if (_itemsInInstancings == null || _itemsInInstancings.Length != _countInInstancings.Length)
                _itemsInInstancings = new Instancing.Item[_countInInstancings.Length][];
            for (var i = 0; i < _itemsInInstancings.Length; i++) {
                if (_itemsInInstancings [i] == null || _itemsInInstancings [i].Length != _countInInstancings [i]) {
                    _itemsInInstancings [i] = new Instancing.Item[_countInInstancings [i]];
                    var itemsInInstancing = _itemsInInstancings [i];
                    for (var j = 0; j < itemsInInstancing.Length; j++)
                        itemsInInstancing [j] = new Instancing.Item ();
                }
            }

            System.Array.Clear (_countInInstancings, 0, _countInInstancings.Length);
            for (var i = 0; i < clusters.Length; i++) {
                var cluster = clusters [i];
                for (var j = 0; j < instancings.Length; j++) {
                    var itemsInInstancing = _itemsInInstancings [j];
                    var typedCountInCluster = cluster.Count (j);
                    for (var k = 0; k < typedCountInCluster; k++) {
                        var itemIndex = _countInInstancings [j]++;
                        cluster.Assign (j, k, ref itemsInInstancing [itemIndex]);
                    }
                }
            }

            for (var i = 0; i < instancings.Length; i++)
                instancings [i].items = _itemsInInstancings [i];
        }

        [System.Serializable]
        public class Item {
            public int index;
            public Transform transform;
            public float time;
        }
    }
}
