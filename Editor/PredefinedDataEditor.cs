using VoxelBuilder.DataTypes;
using UnityEditor;
using UnityEngine;

namespace VoxelBuilder.Editor
{
    [CustomEditor(typeof(PredefinedData))]
    public class PredefinedDataEditor : UnityEditor.Editor
    {
        private int d = 0;

        protected PredefinedData Data
        {
            get { return (PredefinedData) target; }
        }

        public override void OnInspectorGUI()
        {
//            var settings = Resources.Load<Settings>("Settings");
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            Data.Width = EditorGUILayout.IntField(Data.Width);
            Data.Height = EditorGUILayout.IntField(Data.Height);
            Data.Depth = EditorGUILayout.IntField(Data.Depth);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("Build");
                Data.Data = new Color[Data.Width*Data.Height*Data.Depth];
                Data.BuildNow();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
                d = (d - 1)%Data.Depth;
            GUILayout.Label("Depth " + (d + 1));
            if (GUILayout.Button(">"))
                d = (d + 1) % Data.Depth;
            GUILayout.EndHorizontal();


            EditorGUI.BeginChangeCheck();
            for (int h = 0; h < Data.Height; h++)
            {
                GUILayout.BeginHorizontal();
                for (int w = 0; w < Data.Width; w++)
                {
					var index = d * Data.Height * Data.Width + h * Data.Width + w;
                    var cell = Data.Data[index];
                    GUI.color = Data.Data[index];
                    GUILayout.BeginVertical(GUI.skin.box);
//					TODO: Add colors 

					GUILayout.BeginHorizontal ();
					if (DrawColorButton (Color.red))
						Data.Data [index] = cell == Color.red ? new Color (0, 0, 0, 0) : Color.red;
					if (DrawColorButton (Color.blue))
						Data.Data [index] = cell == Color.blue ? new Color (0, 0, 0, 0) : Color.blue;
					GUILayout.EndHorizontal ();
					GUILayout.BeginHorizontal ();
					if (DrawColorButton (Color.yellow))
						Data.Data [index] = cell == Color.yellow ? new Color (0, 0, 0, 0) : Color.yellow;
					if (DrawColorButton (Color.green))
						Data.Data [index] = cell == Color.green ? new Color (0, 0, 0, 0) : Color.green;
					GUILayout.EndHorizontal ();

//                    for (int i = 0; i < settings.Colors.Length; i++)
//                    {
//                        if(i % 2 == 0)
//                            GUILayout.BeginHorizontal();
//
//                        var color = settings.Colors[i];
//                        GUI.color = color;
//                        if (GUILayout.Button("."))
//                            Data.Data[d*Data.Height*Data.Width + h*Data.Width + w] = cell == color
//                                ? new Color(0, 0, 0, 0)
//                                : color;
//
//                        if(i % 2 == 1)
//                            GUILayout.EndVertical();
//                    }
                    GUILayout.EndVertical();
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("Build");
                Data.BuildNow();
            }


            if(GUILayout.Button("Rebuild"))
                Data.BuildNow();

            SceneView.RepaintAll();
            Repaint();
        }

		bool DrawColorButton(Color color)
		{
			GUI.color = color;
			return GUILayout.Button (".");
		}
    }
}