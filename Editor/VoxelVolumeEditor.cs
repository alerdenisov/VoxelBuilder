using UnityEditor;
using UnityEngine;

namespace VoxelBuilder.Editor
{
    [CustomEditor(typeof(VoxelVolume))]
    public class VoxelVolumeEditor : UnityEditor.Editor
    {
        private GameObject _selection;

        protected GameObject Selection
        {
            get
            {
                if (!_selection)
                {
                    var found = GameObject.Find("VOXELVOLUMESELECTOR");
                    _selection = found ? found : MakeSelector();
                }

                return _selection;
            }
        }

        private GameObject MakeSelector()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "VOXELVOLUMESELECTOR";

            go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            go.GetComponent<MeshRenderer>().material.color = new Color(0,1,0,0.3f);

            DestroyImmediate(go.GetComponent<Collider>());

            return go;
        }

        void OnSceneGUI()
        {
			return;
            var volume = target as VoxelVolume;
            if (Event.current.type == EventType.MouseMove)
            {
                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    // do here
                    Selection.transform.position = hit.point;
                }
                else
                {
                    var plane = new Plane(Vector3.up, volume.transform.position);
                    var distance = -1f;
                    if (plane.Raycast(ray, out distance))
                    {
                        Selection.transform.position = ray.GetPoint(distance);
                    }
                }
            }
        }
    }
}
