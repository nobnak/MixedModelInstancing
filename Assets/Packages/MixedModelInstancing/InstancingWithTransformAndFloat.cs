using UnityEngine;
using System.Collections;

namespace MixedModelInstancing {

	[ExecuteInEditMode]
	public class InstancingWithTransformAndFloat : AbstractInstancing {
		public string propTransformBuf = "_WorldMatBuf";
		public string propFloat = "_TimeBuf";

		public Item[] items;

		Matrix4x4[] _matrices;
		ComputeBuffer _matrixBuf;

		float[] _floats;
		ComputeBuffer _floatBuf;

		protected override void OnDisable() {
			base.OnDisable ();
			if (_matrixBuf != null) {
				_matrixBuf.Dispose ();			
				_matrixBuf = null;
			}
			if (_floatBuf != null) {
				_floatBuf.Dispose ();
				_floatBuf = null;
			}
		}

		#region implemented abstract members of AbstractInstanceData
		public override int Length {
			get { return items.Length; }
		}
		public override AbstractInstancing Set (Material mat) {
			base.Set (mat);

			var len = AbstractInstancing.CeilToNearestPowerOfTwo (items.Length);
			if (_matrices == null || _matrices.Length < len) {
				_matrices = new Matrix4x4[len];
				_floats = new float[len];
			}

			for (var i = 0; i < items.Length; i++) {
				var item = items [i];
				_matrices [i] = item.transform.localToWorldMatrix;
				_floats [i] = item.t;
			}
			if (items.Length < len) {
				System.Array.Clear (_matrices, items.Length, len - items.Length);
				System.Array.Clear (_floats, items.Length, len - items.Length);
			}

			if (_matrixBuf == null || _matrixBuf.count != len) {
				if (_matrixBuf != null) {
					_matrixBuf.Dispose ();
					_floatBuf.Dispose ();
				}
				_matrixBuf = AbstractInstancing.Create(_matrices);
				_floatBuf = AbstractInstancing.Create (_floats);
			}
			_matrixBuf.SetData (_matrices);
			_floatBuf.SetData (_floats);

			mat.SetBuffer (propTransformBuf, _matrixBuf);
			mat.SetBuffer (propFloat, _floatBuf);

			return this;
		}
		#endregion

		[System.Serializable]
		public class Item {
			public Transform transform;
			public float t;
		}
	}
}
