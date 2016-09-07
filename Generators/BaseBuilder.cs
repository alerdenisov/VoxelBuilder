using System;
using VoxelBuilder.Interfaces;
using Zenject;
using VoxelBuilder.DataTypes;
using System.Collections.Generic;
using UnityEngine;
using System.Configuration;
using UnityEditor.VersionControl;

namespace VoxelBuilder.Generators
{
	public class BaseBuilder : IBuilder
	{
		IMeshGeneratorConfiguration _config;
		IMeshCullingGenerator 		_cull;
		IMeshVerticesGenerator 		_verts;
		IMeshTrianglesGenerator 	_tris;
		IMeshUVGenerator 			_uvs;
		IMeshColorsGenerator 		_colors;
		IMeshNormalsGenerator 		_normals;
		IMeshSecondUVGenerator 		_uv2s;


		List<Vector3>	_vBuffer 	= new List<Vector3>();
		List<int> 		_tBuffer 	= new List<int>();
		List<Color> 	_cBuffer 	= new List<Color>();
		List<Vector2> 	_uvBuffer 	= new List<Vector2>();
		List<Vector2> 	_uv2Buffer 	= new List<Vector2> ();
		List<Vector3> 	_nBuffer 	= new List<Vector3>();

		public BaseBuilder(
			IMeshGeneratorConfiguration config,
			IMeshCullingGenerator genCulling,
			IMeshVerticesGenerator genVerts,
			IMeshTrianglesGenerator genTriangles,
			IMeshUVGenerator genUVs,
			IMeshColorsGenerator genColors,
			IMeshNormalsGenerator genNormals,
			IMeshSecondUVGenerator genSecondUVs)
		{
			_config 	= config;
			_cull 		= genCulling;
			_verts 		= genVerts;
			_tris 		= genTriangles;
			_uvs 		= genUVs;
			_colors 	= genColors;
			_normals 	= genNormals;
			_uv2s 		= genSecondUVs;
		}

		#region IBuilder implementation
		public ThreadableMesh Build(VoxelData voxels)
		{
			int vCount = 0,
				tCount = 0,
				cCount = 0, 
				nCount = 0, 
				uvCount = 0, 
				uv2Count = 0, 
				lastTrisIndex = 0;


			_vBuffer.Clear ();
			_tBuffer.Clear ();
			_cBuffer.Clear ();
			_uvBuffer.Clear ();
			_uv2Buffer.Clear ();
			_nBuffer.Clear ();


			for (int z = 0; z < voxels.Depth; z++)
			{
				for (int y = 0; y < voxels.Height; y++)
				{
					for (int x = 0; x < voxels.Width; x++)
					{
						var voxel = voxels.GetVoxel (x, y, z);
						if (!voxel.IsSolid)
							continue;

						var mask = _cull.BuildCulling (ref voxels, x, y, z);

						if (mask == CullingFaces.All)
							continue;

						CopyToBuffer(
							ref _vBuffer, ref vCount, 
							_verts.BuildVertices (ref vCount, ref mask, ref voxel, x, y, z));
						
						CopyToBuffer(
							ref _tBuffer, ref tCount, 
							_tris.BuildTrianlges (ref tCount, ref mask, ref voxel, ref lastTrisIndex, x, y, z));

						if (_config.GenerateUV)
							CopyToBuffer(
								ref _uvBuffer, ref uvCount, 
								_uvs.BuildUV (ref uvCount, ref mask, ref voxel, x, y, z));

						if (_config.GenerateColors)
							CopyToBuffer(
								ref _cBuffer, ref cCount, 
								_colors.BuildColors (ref cCount, ref mask, ref voxel, x, y, z));

						if (_config.GenerateNormals)
							CopyToBuffer(
								ref _nBuffer, ref nCount, 
								_normals.BuildNormals (ref nCount, ref mask, ref voxel, x, y, z));

						if (_config.GenerateUV2)
							CopyToBuffer(
								ref _uv2Buffer, ref uv2Count, 
								_uv2s.BuildUV2 (ref uv2Count, ref mask, ref voxel, x, y, z));
					}
				}
			}

			var mesh = new ThreadableMesh ();
			CopyFromBuffer (ref _vBuffer, ref vCount, ref mesh.Vertices);
			CopyFromBuffer (ref _tBuffer, ref tCount, ref mesh.Triangles);

			if (_config.GenerateUV)
				CopyFromBuffer (ref _uvBuffer, ref uvCount, ref mesh.Uvs);
			 
			if (_config.GenerateColors)
				CopyFromBuffer (ref _cBuffer, ref cCount, ref mesh.Colors);

			if (_config.GenerateNormals)
				CopyFromBuffer (ref _nBuffer, ref nCount, ref mesh.Normals);

			if (_config.GenerateUV2)
				CopyFromBuffer (ref _uv2Buffer, ref uv2Count, ref mesh.Uvs1);

			if (_config.GenerateTangents)
				mesh.RecalculateTangets ();

			return mesh;
		}

		void CopyToBuffer<T>(ref List<T> buffer, ref int count, T[] data) where T : struct
		{
//			while (buffer.Count < count)
//				buffer.Add (default(T));
//			if (buffer.Count < count)
//				buffer.AddRange (new T[count - buffer.Count]);

			buffer.AddRange (data);
		}
		#endregion

		void CopyFromBuffer<T>(ref List<T> buffer, ref int count, ref T[] dest) where T : struct
		{
			dest = new T[count];
			buffer.CopyTo (0, dest, 0, count);
		}
	}
}

