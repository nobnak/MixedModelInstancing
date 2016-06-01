using UnityEngine;
using System.Collections;

namespace MixedModelInstancing {

	[ExecuteInEditMode]
	public class InstancingWithTransform : AbstractInstancing {
		public string propTransformBuf = "_WorldMatBuf";

		public Transform[] instanceTransforms;

		Matrix4x4[] _matrices;
		ComputeBuffer _matrixBuf;

		protected override void OnDisable() {
			base.OnDisable ();
			if (_matrixBuf != null) {
				_matrixBuf.Dispose ();			
				_matrixBuf = null;
			}
		}

		#region implemented abstract members of AbstractInstanceData
		public override int Length {
			get { return instanceTransforms.Length; }
		}
		public override AbstractInstancing Set (Material mat) {
			base.Set (mat);

			var len = AbstractInstancing.CeilToNearestPowerOfTwo (instanceTransforms.Length);
			if (_matrices == null || _matrices.Length < len)
				_matrices = new Matrix4x4[len];
			
			for (var i = 0; i < instanceTransforms.Length; i++)
				_matrices [i] = instanceTransforms [i].localToWorldMatrix;
			if (instanceTransforms.Length < len)
				System.Array.Clear (_matrices, instanceTransforms.Length, len - instanceTransforms.Length);

			if (_matrixBuf == null || _matrixBuf.count != len) {
				if (_matrixBuf != null)
					_matrixBuf.Dispose ();
				_matrixBuf = AbstractInstancing.Create(_matrices);
			}
			_matrixBuf.SetData (_matrices);
			mat.SetBuffer (propTransformBuf, _matrixBuf);

			return this;
		}
		#endregion
	}
}
