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

		public AbstractInstanceData[] instanceDataset;

		MeshBuf _meshBuf;

		void OnEnable() {
			_meshBuf = new MeshBuf (mesh);
		}
		void OnDisable() {
			if (_meshBuf != null)
				_meshBuf.Dispose ();
		}
		void OnRenderObject() {
			if (instanceDataset.Length == 0)
				return;
			var mainInstanceData = instanceDataset [0];
			var len = mainInstanceData.Length;
			if (len <= 0)
				return;

			mat.SetPass (0);
			mat.SetBuffer (propTriangleBuf, _meshBuf.triangles);
			mat.SetBuffer (propVertexBuf, _meshBuf.vertices);
			mat.SetBuffer (propUvBuf, _meshBuf.uv);
			for (var i = 0; i < instanceDataset.Length; i++)
				instanceDataset [i].Set (mat);
			Graphics.DrawProcedural (MeshTopology.Triangles, _meshBuf.vertexCount, len);
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

	public abstract class AbstractInstanceData : MonoBehaviour {
		public abstract int Length { get; }
		public abstract AbstractInstanceData Set (Material mat);
	}
}