using System.Collections;
using VoxelBuilder.DataTypes;
using UnityEngine;

namespace VoxelBuilder
{
    public class PredefinedData : MonoBehaviour
    {
        public int Width;
        public int Height;
        public int Depth;

        public Color[] Data;

        void OnEnable()
        {
            GetComponent<MeshFilter>().mesh = null;
            BuildNow();
        }

        private IEnumerator BuildNowCoroutine()
        {
            var volume = GetComponent<VoxelVolume>();
            var data = volume.Data = new VoxelData(Width, Height, Depth);

			var time = Time.time;//Pools.system.timer.Time;
            var index = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        var color = Data[z * Height * Width + y * Width + x];
                        if (color.a > 0)
                            data.SetVoxel(x, y, z, new Color32(
                                (byte) Mathf.FloorToInt(color.r*byte.MaxValue),
                                (byte) Mathf.FloorToInt(color.g*byte.MaxValue),
                                (byte) Mathf.FloorToInt(color.b*byte.MaxValue), byte.MaxValue),
                                Application.isPlaying ? time + .5f : -100f);

                        index++;

                        if (Application.isPlaying && index % 3 == 0)
                            yield return null;
                    }
                }
            }
            yield return null;

            data.CommitChanges();
        }

        public void BuildNow()
        {
            if (Application.isPlaying)
                StartCoroutine(BuildNowCoroutine());
            else
            {
                var fakeCoroutine = BuildNowCoroutine();
                while (fakeCoroutine.MoveNext())
                {
                }
            }
        }
    }
}