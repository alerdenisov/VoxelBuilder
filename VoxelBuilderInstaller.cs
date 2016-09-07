using UnityEngine;
using Zenject;
using VoxelBuilder.Interfaces;
using VoxelBuilder.Generators;

namespace VoxelBuilder {
	public class VoxelBuilderInstaller : MonoInstaller<VoxelBuilderInstaller>
	{
	    public override void InstallBindings()
	    {
			Container
				.Bind<IMeshGeneratorConfiguration> ()
				.To<FlatMeshGenerator> ().AsSingle ();
			Container
				.Bind<IMeshCullingGenerator> ()
				.To<FlatMeshGenerator> ().AsSingle ();
			Container
				.Bind<IMeshVerticesGenerator> ()
				.To<FlatMeshGenerator> ().AsSingle ();
			Container
				.Bind<IMeshTrianglesGenerator> ()
				.To<FlatMeshGenerator> ().AsSingle ();
			Container
				.Bind<IMeshUVGenerator> ()
				.To<FlatMeshGenerator> ().AsSingle ();
			Container
				.Bind<IMeshColorsGenerator> ()
				.To<FlatMeshGenerator> ().AsSingle ();
			Container
				.Bind<IMeshNormalsGenerator> ()
				.To<FlatMeshGenerator> ().AsSingle ();
			Container
				.Bind<IMeshSecondUVGenerator> ()
				.To<FlatMeshGenerator> ().AsSingle ();

			Container
				.Bind<IBuilder> ()
				.To<BaseBuilder> ().AsSingle ();
	    }
	}
}