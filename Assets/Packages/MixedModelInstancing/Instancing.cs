using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace MixedModelInstancing {

    [ExecuteInEditMode]
	public abstract class Instancing : MonoBehaviour {
        public static readonly float InvLogTwo = 1f / Mathf.Log (2);

        public Mesh mesh;

        public string propTriangleBuf = "_TriangleBuf";
        public string propVertexBuf = "_VertexBuf";
        public string propUvBuf = "_UvBuf";
        public string propUv1Buf = "_Uv1Buf";
        public int pass = 0;
        public Material mat;

        MeshBuf _meshBuf;

		public virtual void CheckSetup() {
			if (_meshBuf == null || _meshBuf.mesh != mesh) {
				if (_meshBuf != null)
					_meshBuf.Dispose ();
				_meshBuf = new MeshBuf (mesh);
			}
		}
		public abstract int Length { get; }

        protected virtual void OnDisable() {
            if (_meshBuf != null) {
                _meshBuf.Dispose ();
                _meshBuf = null;
            }
        }
        protected virtual void OnRenderObject() {
			mat.SetBuffer (propTriangleBuf, _meshBuf.triangles);
			mat.SetBuffer (propVertexBuf, _meshBuf.vertices);
            if (_meshBuf.uv != null)
			    mat.SetBuffer (propUvBuf, _meshBuf.uv);
            if (_meshBuf.uv1 != null)
                mat.SetBuffer (propUv1Buf, _meshBuf.uv1);

			mat.SetPass (0);
			Graphics.DrawProcedural (MeshTopology.Triangles, _meshBuf.vertexCount, Length);
        }

        public static ComputeBuffer Create<T>(T[] array) {
            return (array == null) ? null : new ComputeBuffer (array.Length, Marshal.SizeOf (typeof(T)));
        }
        public static ComputeBuffer Convert<T>(T[] array) {
            var buf = Create (array);
            if (buf == null)
                return null;
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
            public readonly ComputeBuffer uv1;

            public MeshBuf(Mesh mesh) {
                this.mesh = mesh;
                this.triangles = Convert(mesh.triangles);
                this.vertices = Convert(mesh.vertices);
                this.uv = Convert(mesh.uv);
                this.uv1 = Convert(mesh.uv2);
                this.vertexCount = this.triangles.count;
            }

            #region IDisposable implementation
            public void Dispose () {
                triangles.Dispose ();
                vertices.Dispose ();
                uv.Dispose ();
                uv1.Dispose ();
            }
            #endregion
        }
	}
}
