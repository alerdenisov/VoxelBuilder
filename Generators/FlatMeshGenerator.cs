using System;
using VoxelBuilder.Interfaces;
using UnityEngine;
using VoxelBuilder.DataTypes;
using Uniful;

namespace VoxelBuilder.Generators
{
	public class FlatMeshGenerator : SimpleMeshGenerator
	{
		public override Vector3[] BuildNormals(
			ref int count, 
			ref CullingFaces c, ref Voxel v, 
			int x, int y, int z)
		{
			var l = 4 * (6 - ((byte)c).PopCount());
			count += l;

			var vertices = new Vector3[l];
			var index = 0;

			for(byte f = 1; f <= (byte)CullingFaces.Down; f = (byte)(f << 1)) {
				if (((byte)f).PopCount() != 1) continue;
				if (!c.HasFlag(f))
				{
					Array.Copy(
						MakeFaceNormals((CullingFaces)f, x, y, z),
						0,
						vertices,
						index,
						4);
					index += 4;
				}
			}

			return vertices;
		}

		private static Vector3[] MakeFaceNormals(CullingFaces face, int x, int y, int z)
		{
			var q = GetRotation((byte)face);
			var r = new[]
			{
				q*new Vector3(-.5f,  .5f, -.5f).normalized,
				q*new Vector3( .5f,  .5f, -.5f).normalized,
				q*new Vector3(-.5f, -.5f, -.5f).normalized,
				q*new Vector3( .5f, -.5f, -.5f).normalized,
			};
			return r;
		}

		public override Color[] BuildColors(ref int count, ref CullingFaces c, ref Voxel v, int x, int y, int z)
		{
			var l = 4 * (6 - ((byte)c).PopCount ());
			count += l;

			var r = new Color[l];

			for (int i = 0; i < r.Length; i++)
			{
				// TODO: Move to configurable
				r[i] = v.Color;
			}

			return r;
		}

		public override bool GenerateNormals { get { return true; } } 

		public override bool GenerateColors { get { return true; } }
	}
}

