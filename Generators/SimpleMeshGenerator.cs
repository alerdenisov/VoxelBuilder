using UnityEngine;
using System;
using VoxelBuilder.Interfaces;
using VoxelBuilder.DataTypes;
using Uniful;

namespace VoxelBuilder.Generators
{
	public class SimpleMeshGenerator : AbstractMeshGenerator
	{
		public override bool GenerateUV { get { return true; } }

		public override CullingFaces BuildCulling(ref VoxelData d, int x, int y, int z)
		{
			var r = CullingFaces.All;
			if (!d.GetVoxel(x + 1, y, z).IsSolid)
				r -= CullingFaces.East;
			if (!d.GetVoxel(x - 1, y, z).IsSolid)
				r -= CullingFaces.West;
			if (!d.GetVoxel(x, y, z - 1).IsSolid)
				r -= CullingFaces.South;
			if (!d.GetVoxel(x, y, z + 1).IsSolid)
				r -= CullingFaces.North;
			if (!d.GetVoxel(x, y - 1, z).IsSolid)
				r -= CullingFaces.Down;
			if (!d.GetVoxel(x, y + 1, z).IsSolid)
				r -= CullingFaces.Up;

			return r;
		}

		#region IMeshVerticesGeneratorInterface implementation
		public override Vector3[] BuildVertices(ref int count, ref CullingFaces c, ref Voxel v, int x, int y, int z)
		{
			var l = 4 * (6 - ((byte)c).PopCount ());
			count += l;
			var vertices = new Vector3[l];
			var index = 0;

			var o = new Vector3 (x, y, z);
			// Faster itterate over culling faces against Enum.GetValues
			for(byte fi = 1 << 0; fi <= (byte)CullingFaces.Down; fi = (byte)(fi << 1)) 
			{
				if(!c.HasFlag(fi)) {
					Array.Copy(
						MakeFaceVerts(fi, ref o),
						0,
						vertices,
						index,
						4);
					index += 4;
				}
			}

			return vertices;
		}

		protected static Vector3[] MakeFaceVerts(byte face, ref Vector3 o)
		{
			var q = GetRotation(face);
			var r = new []
			{
				q*new Vector3(-.5f,  .5f, -.5f) + o,
				q*new Vector3( .5f,  .5f, -.5f) + o,
				q*new Vector3(-.5f, -.5f, -.5f) + o,
				q*new Vector3( .5f, -.5f, -.5f) + o,
			};
			return r;
		}

		protected static Quaternion GetRotation(byte face)
		{
			switch (face)
			{
				case 1: //CullingFaces.East:
					return Quaternion.Euler(0, 270, 0);
				case 2: //CullingFaces.West:
					return Quaternion.Euler(0, 90, 0);
				case 4: //CullingFaces.North:
					return Quaternion.Euler(0, 180, 0);
				case 16: //CullingFaces.Up:
					return Quaternion.Euler(90, 0, 0);
				case 32: //CullingFaces.Down:
					return Quaternion.Euler(-90, 0, 0);
			}

			return Quaternion.identity;
		}

		#endregion

		#region IMeshUVGeneratorInterface implementation

		public override Vector2[] BuildUV(ref int count, ref CullingFaces c, ref Voxel v, int x, int y, int z)
		{
			var l = 4 * (6 - ((byte)c).PopCount ());
			count += l;
			var r = new Vector2[l];

			for (int i = r.Length - 4; i >= 0; i -= 4)
			{
				r [i + 0] = new Vector2 (0, 1);
				r [i + 1] = new Vector2 (1, 1);
				r [i + 2] = new Vector2 (0, 0);
				r [i + 3] = new Vector2 (1, 0);
			}

			return r;
		}

		#endregion

		#region implemented abstract members of AbstractMeshGenerator
		public override int[] BuildTrianlges(ref int count, ref CullingFaces c, ref Voxel v, ref int triangleIndex, int x, int y, int z)
		{
			var l = 6 * (6 - ((byte)c).PopCount());
			count += l;
			var triangles = new int[l];
			var index = 0;
			for(byte f = 1 << 0; f <= (byte)CullingFaces.Down; f = (byte)(f << 1)) {
				if (((byte)f).PopCount() != 1) continue;
				if (!c.HasFlag(f))
				{
					Array.Copy(MakeFaceTriangles((CullingFaces)f, triangleIndex), 0, triangles, index, 6);
					index += 6;
					triangleIndex += 4;
				}
			}

			return triangles;
		}

		private static int[] MakeFaceTriangles(CullingFaces face, int i)
		{
			return new[] { i + 0, i + 1, i + 2, i + 3, i + 2, i + 1 };
		}
		#endregion
	}
}

