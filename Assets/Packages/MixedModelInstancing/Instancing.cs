using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace MixedModelInstancing {

    [ExecuteInEditMode]
    public class Instancing : MonoBehaviour {
        public static readonly float InvLogTwo = 1f / Mathf.Log (2);

        public Mesh mesh;

        public string propTriangleBuf = "_TriangleBuf";
        public string propVertexBuf = "_VertexBuf";
        public string propUvBuf = "_UvBuf";
        public int pass = 0;
        public Material mat;

        public string propTransformBuf = "_WorldMatBuf";
        public string propFloat = "_TimeBuf";
        public Item[] items;

        MeshBuf _meshBuf;

        Matrix4x4[] _matrices;
        ComputeBuffer _matrixBuf;

        float[] _floats;
        ComputeBuffer _floatBuf;

        void OnEnable() {
            _meshBuf = new MeshBuf (mesh);
        }
        void OnDisable() {
            if (_meshBuf != null) {
                _meshBuf.Dispose ();
                _meshBuf = null;
            }
            if (_matrixBuf != null) {
                _matrixBuf.Release ();          
                _matrixBuf = null;
            }
            if (_floatBuf != null) {
                _floatBuf.Release ();
                _floatBuf = null;
            }
        }
        void OnRenderObject() {
            var len = items.Length;
            if (len <= 0)
                return;

            mat.SetBuffer (propTriangleBuf, _meshBuf.triangles);
            mat.SetBuffer (propVertexBuf, _meshBuf.vertices);
            mat.SetBuffer (propUvBuf, _meshBuf.uv);

            mat.SetBuffer (propTransformBuf, _matrixBuf);
            mat.SetBuffer (propFloat, _floatBuf);

			mat.SetPass (0);
            Graphics.DrawProcedural (MeshTopology.Triangles, _meshBuf.vertexCount, len);
        }
        void Update() {
            var len = Instancing.CeilToNearestPowerOfTwo (items.Length);
			if (len <= 0)
				return;
			
            if (_matrices == null || _matrices.Length < len) {
                _matrices = new Matrix4x4[len];
                _floats = new float[len];
            }

            for (var i = 0; i < items.Length; i++) {
                var item = items [i];
                _matrices [i] = item.transform.localToWorldMatrix;
                _floats [i] = item.t;
            }
            if (items.Length < _matrices.Length) {
                System.Array.Clear (_matrices, items.Length, _matrices.Length - items.Length);
                System.Array.Clear (_floats, items.Length, _floats.Length - items.Length);
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

		[System.Serializable]
		public class Item {
			public Transform transform;
			public float t;
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
