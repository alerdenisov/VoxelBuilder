using UnityEngine;

namespace VoxelBuilder.DataTypes
{
    public struct Voxel
    {
		public byte Index { get; private set; }
        public Color32 Color { get; private set; }

        public float Time { get; private set; }

        public bool IsSolid
        {
            get { return Color.a > 0; }
        }

        public Voxel(byte index, float time) : this()
        {
			Index = index;
            // Color = color;
            Time = time;
		}
		public Voxel(Color32 color, float time) : this()
		{
//			Index = index;
			 Color = color;
			Time = time;
		}
    }
}