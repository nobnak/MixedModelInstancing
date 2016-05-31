using UnityEngine;
using System.Collections;

namespace MixedModelInstancing {
        
    public class TestInstancing : MonoBehaviour {
        public MixedModelData modelData;
        public Material matInstancing;

        void OnRenderObject() {
            var modelIndex = 0;
            var model = modelData.dataset [modelIndex];
            var buf = modelData.GetModelBuffer (modelIndex);

            matInstancing.SetPass (0);
            Graphics.DrawProcedural (MeshTopology.Triangles, model.mesh.vertexCount);
        }
    }
}
