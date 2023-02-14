using Sandbox;

namespace CatHarvest;

public static class Cameras
{
	public abstract class BaseCamera : BaseNetworkable
	{
		protected TimeSince tsCreated;

		protected BaseCamera()
		{
			tsCreated = 0;
		}

		public abstract void Update();
	}

	public sealed class BalancedEndingCamera : BaseCamera
	{
		public BalancedEndingCamera()
		{
			Camera.Rotation = Rotation.FromPitch( 80f );
		}

		public override void Update()
		{
			Camera.Position = new Vector3( -300f + tsCreated * 50f, 0f, 300f + tsCreated * 50f );
		}
	}

	public sealed class PeacefulEndingCamera : BaseCamera
	{
		public PeacefulEndingCamera()
		{
			Camera.Rotation = Rotation.From( new Angles( 20f, 90f, 0f ) );
		}

		public override void Update()
		{
			Camera.Position = new Vector3( 0f, -200f + tsCreated * 30f, 100f + tsCreated * 10f );
		}
	}
}