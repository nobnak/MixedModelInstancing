using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace MixedModelInstancing {

    [ExecuteInEditMode]
	public class CustomInstancing : Instancing {
        public string propTransformBuf = "_WorldMatBuf";
        public string propFloat = "_TimeBuf";
        
		Item[] _items;

        Matrix4x4[] _matrices;
        ComputeBuffer _matrixBuf;

        float[] _floats;
        ComputeBuffer _floatBuf;

		public Item[] Items { set { _items = value; } }
		public override void CheckSetup() {
			base.CheckSetup ();

			var len = Instancing.CeilToNearestPowerOfTwo (_items.Length);
			if (_matrices == null || _matrices.Length < len) {
				_matrices = new Matrix4x4[len];
				_floats = new float[len];
			}

			for (var i = 0; i < _items.Length; i++) {
				var item = _items [i];
				_matrices [i] = item.transform.localToWorldMatrix;
				_floats [i] = item.t;
			}
			if (_items.Length < _matrices.Length) {
				System.Array.Clear (_matrices, _items.Length, _matrices.Length - _items.Length);
				System.Array.Clear (_floats, _items.Length, _floats.Length - _items.Length);
			}

			if (_matrixBuf == null || _matrixBuf.count != _matrices.Length) {
				if (_matrixBuf != null) {
					_matrixBuf.Dispose ();
					_floatBuf.Dispose ();
				}
				_matrixBuf = Instancing.Create(_matrices);
				_floatBuf = Instancing.Create (_floats);
			}
			_matrixBuf.SetData (_matrices);
			_floatBuf.SetData (_floats);            
		}
		public override int Length {
			get { return _items == null ? 0 : _items.Length; }
		}

		protected override void OnDisable() {
            if (_matrixBuf != null) {
                _matrixBuf.Release ();          
                _matrixBuf = null;
            }
            if (_floatBuf != null) {
                _floatBuf.Release ();
                _floatBuf = null;
            }
			base.OnDisable ();
        }
		protected override void OnRenderObject() {
			var len = Length;
			if (!isActiveAndEnabled || len <= 0)
                return;

			CheckSetup ();

            mat.SetBuffer (propTransformBuf, _matrixBuf);
            mat.SetBuffer (propFloat, _floatBuf);

			base.OnRenderObject ();
        }

		[System.Serializable]
		public class Item {
			public Transform transform;
			public float t;
        }
	}
}
