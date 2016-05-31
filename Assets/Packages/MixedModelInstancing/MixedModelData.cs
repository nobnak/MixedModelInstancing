using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace MixedModelInstancing {

    public class MixedModelData : MonoBehaviour {
        public ModelData[] dataset;

        ModelBuffer[] _buffers;

        public ModelBuffer GetModelBuffer(int index) {
            return _buffers [index];
        }

        void OnEnable() {
            _buffers = new ModelBuffer[dataset.Length];
            for (var i = 0; i < _buffers.Length; i++)
                _buffers [i] = new ModelBuffer (dataset [i]);
        }
        void OnDisable() {
            if (_buffers != null)
                for (var i = 0; i < _buffers.Length; i++)
                    if (_buffers[i] != null)
                        _buffers [i].Dispose ();
        }

        [System.Serializable]
        public class ModelData {
            public Mesh mesh;
            public Texture[] textures;
        }

        public class ModelBuffer : System.IDisposable {
            public readonly ModelData model;
            public readonly ComputeBuffer triangles;
            public readonly ComputeBuffer vertices;
            public readonly ComputeBuffer uv;

            public ModelBuffer(ModelData model) {
                var mesh = model.mesh;
                this.model = model;
                this.triangles = Convert(mesh.triangles);
                this.vertices = Convert(mesh.vertices);
                this.uv = Convert(mesh.uv);
            }

            public static ComputeBuffer Convert<T>(T[] array) {
                var buf = new ComputeBuffer (array.Length, Marshal.SizeOf (typeof(T)));
                buf.SetData (array);
                return buf;
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
