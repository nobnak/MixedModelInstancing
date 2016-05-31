using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace MixedModelInstancing {

	public class Instancing : MonoBehaviour {
		public static readonly float InvLogTwo = 1f / Mathf.Log (2);

		public Mesh mesh;

		public Material mat;
		public int pass = 0;
		public string propTriangleBuf = "_TriangleBuf";
		public string propVertexBuf = "_VertexBuf";
		public string propUvBuf = "_UvBuf";
		public string propTransformBuf = "_WorldMatBuf";

		public Transform[] instanceTransforms;

		MeshBuf _meshBuf;
		Matrix4x4[] _matrices;
		ComputeBuffer _matrixBuf;

		void OnEnable() {
			_meshBuf = new MeshBuf (mesh);
		}
		void OnDisable() {
			if (_meshBuf != null)
				_meshBuf.Dispose ();
			if (_matrixBuf != null)
				_matrixBuf.Dispose ();
		}
		void OnRenderObject() {
			var len = CeilToNearestPowerOfTwo (instanceTransforms.Length);
			if (len <= 0)
				return;

			if (_matrices == null || _matrices.Length != len)
				_matrices = new Matrix4x4[len];
			for (var i = 0; i < instanceTransforms.Length; i++)
				_matrices [i] = instanceTransforms [i].localToWorldMatrix;
			if (instanceTransforms.Length < len)
				System.Array.Clear (_matrices, instanceTransforms.Length, len - instanceTransforms.Length);

			if (_matrixBuf == null || _matrixBuf.count != len) {
				if (_matrixBuf != null)
					_matrixBuf.Dispose ();
				_matrixBuf = Create(_matrices);
			}
			_matrixBuf.SetData (_matrices);

			mat.SetPass (0);
			mat.SetBuffer (propTriangleBuf, _meshBuf.triangles);
			mat.SetBuffer (propVertexBuf, _meshBuf.vertices);
			mat.SetBuffer (propUvBuf, _meshBuf.uv);
			mat.SetBuffer (propTransformBuf, _matrixBuf);
			Graphics.DrawProcedural (MeshTopology.Triangles, _meshBuf.vertexCount, instanceTransforms.Length);
		}

		public static ComputeBuffer Create<T>(T[] array) {
			return new ComputeBuffer (array.Length, Marshal.SizeOf (typeof(T)));
		}
		public static ComputeBuffer Convert<T>(T[] array) {
			var buf = Create (array);
			buf.SetData (array);
			return buf;
		}
		public static int CeilToNearestPowerOfTwo (int len) {
			return len > 0 ? (1 << Mathf.CeilToInt (Mathf.Log (len) * InvLogTwo)) : 0;
		}

		public class MeshBuf : System.IDisposable {
			public readonly Mesh mesh;
			public readonly int vertexCount;
			public readonly ComputeBuffer triangles;
			public readonly ComputeBuffer vertices;
			public readonly ComputeBuffer uv;

			public MeshBuf(Mesh mesh) {
				this.mesh = mesh;
				this.triangles = Convert(mesh.triangles);
				this.vertices = Convert(mesh.vertices);
				this.uv = Convert(mesh.uv);
				this.vertexCount = this.triangles.count;
			}

			#region IDisposable implementation
			public void Dispose () {
				triangles.Dispose ();
				vertices.Dispose ();
				uv.Dispose ();
			}
			#endregion
		}
	}
}