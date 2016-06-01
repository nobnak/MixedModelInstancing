using UnityEngine;
using System.Collections;

namespace MixedModelInstancing {

    [ExecuteInEditMode]
    public class Cluster : MonoBehaviour {
        public MixedInstancing facade;

        public float time;
        public Item[] items;

        int _latestFrame = int.MinValue;
        int[] _countInTypes;
        Transform[][] _typedItems;

        public int Count(int type) {
            return _countInTypes [type];
        }
		public void Assign(int type, int index, ref CustomInstancing.Item item) {
            item.t = time;
            item.transform = _typedItems [type] [index];
        }
        public void ManualUpdate() {
            if (_latestFrame == Time.frameCount)
                return;
            _latestFrame = Time.frameCount;

            var numInstancings = facade.instancings.Length;

            if (_countInTypes == null || _countInTypes.Length != numInstancings)
                _countInTypes = new int[numInstancings];
            System.Array.Clear (_countInTypes, 0, _countInTypes.Length);
            for (var i = 0; i < items.Length; i++)
                _countInTypes [items [i].type]++;

            if (_typedItems == null || _typedItems.Length != numInstancings) {
                _typedItems = new Transform[numInstancings][];
            }
            for (var i = 0; i < numInstancings; i++) {
                var countInType = _countInTypes [i];
                var itemsInType = _typedItems [i];
                if (itemsInType == null || itemsInType.Length != countInType)
                    _typedItems [i] = new Transform[countInType];
            }

            System.Array.Clear (_countInTypes, 0, _countInTypes.Length);
            for (var i = 0; i < items.Length; i++) {
                var item = items [i];
                _typedItems [item.type] [_countInTypes [item.type]++] = item.transform;
            }
        }

		void OnEnable() {
			if (facade != null)
				facade.Add(this);
		}
		void OnDisable() {
			if (facade != null)
				facade.Remove (this);
		}
        void Update() {
            ManualUpdate ();
        }

        [System.Serializable]
        public class Item {
            public int type;
            public Transform transform;
        }
    }
}
