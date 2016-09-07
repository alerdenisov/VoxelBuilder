using System;
using VoxelBuilder.Interfaces;
using UnityEngine;
using VoxelBuilder.DataTypes;

namespace VoxelBuilder.Generators
{
	public abstract class AbstractMeshGenerator 
		: IMeshGeneratorConfiguration
		, IMeshVerticesGenerator
		, IMeshUVGenerator
	, IMeshSecondUVGenerator
		, IMeshNormalsGenerator
		, IMeshColorsGenerator
		, IMeshTrianglesGenerator
		, IMeshCullingGenerator

	{
		#region IMeshGeneratorConfiguration implementation
		public virtual bool GenerateNormals { get; }
		public virtual bool GenerateTangents { get; }
		public virtual bool GenerateUV { get; }
		public virtual bool GenerateUV2 { get; }
		public virtual bool GenerateColors { get; }
		#endregion

		#region IMeshCullingGenerator implementation
		public abstract CullingFaces BuildCulling(ref VoxelData d, int x, int y, int z);
		#endregion

		#region IMeshVerticesGeneratorInterface implementation
		public abstract Vector3[] BuildVertices(
			ref int count, ref CullingFaces c, ref Voxel v, int x, int y, int z);
		#endregion

		#region IMeshTrianglesGeneratorInterface implementation
		public abstract int[] BuildTrianlges(
			ref int count,ref CullingFaces c, ref Voxel v, ref int triangleIndex, int x, int y, int z);
		#endregion

		#region IMeshUVGeneratorInterface implementation
		public virtual Vector2[] BuildUV(
			ref int count,ref CullingFaces c, ref Voxel v, int x, int y, int z) 
		{ 
			throw new NotImplementedException ();
		}
		#endregion

		#region IMeshNormalsGeneratorInterface implementation
		public virtual Vector3[] BuildNormals(
			ref int count,ref CullingFaces c, ref Voxel v, int x, int y, int z)
		{ 
			throw new NotImplementedException ();
		}
		#endregion

		#region IMeshColorsGeneratorInterface implementation
		public virtual Color[] BuildColors(
			ref int count,ref CullingFaces c, ref Voxel v, int x, int y, int z)
		{ 
			throw new NotImplementedException ();
		}
		#endregion

		#region IMeshSecondUVGenerator implementation
		public virtual Vector2[] BuildUV2(ref int count, ref CullingFaces c, ref Voxel v, int x, int y, int z)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

