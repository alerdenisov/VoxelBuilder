using VoxelBuilder.DataTypes;
using UniRx;
using UnityEngine;
using Zenject;
using VoxelBuilder.Interfaces;
using System.Diagnostics;

namespace VoxelBuilder
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class VoxelVolume : MonoBehaviour
    {
        #region Dependencies

        private MeshFilter _filter;
        private MeshRenderer _renderer;

        protected MeshFilter Filter
        {
            get
            {
                if (!_filter) _filter = GetComponent<MeshFilter>();
                return _filter;
            }
        }

        protected MeshRenderer Renderer
        {
            get
            {
                if (!_renderer) _renderer = GetComponent<MeshRenderer>();
                return _renderer;
            }
        }

        #endregion

		[Inject]
		private IBuilder _builder;

        private VoxelData _data;

		private Stopwatch _timer;

        public VoxelData Data
        {
            get { return _data; }
            set { SetNewData(value); }
        }

        private void SetNewData(VoxelData value)
        {
            _data = value;
			_data.Stream.Subscribe (OnNewData);
        }

        private void OnNewData(Voxel[] voxels)
        {
			_timer = new Stopwatch ();
			_timer.Start ();
			var buildStream = Observable.Start (() => BuildVoxels (Data), Scheduler.MainThread);//.ObserveOnMainThread();
			buildStream.Subscribe (ChangeMesh);
			
        }

        private void ChangeMesh(ThreadableMesh mesh)
        {
            Mesh unityMesh = mesh;
            Filter.mesh = unityMesh;

			UnityEngine.Debug.Log (_timer.ElapsedMilliseconds);
			_timer.Stop ();
        }

		public void SetupBuilder(IBuilder builder)
		{
			_builder = builder;
		}

        private ThreadableMesh BuildVoxels(VoxelData voxels)
        {
			return _builder.Build(voxels);
        }
    }
}