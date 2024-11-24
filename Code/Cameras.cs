using Sandbox.Entities;

namespace CatHarvest;

public class EndingCam : Component
{
	[Property] public CameraComponent Camera;
	public TimeSince tsCreated;
	public bool IsPeacefulEnding = false;
	public bool IsBalancedEnding = false;
	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		if (IsPeacefulEnding)
			PeacefulEndingCamera();
		if ( IsBalancedEnding )
			BalancedEndingCamera();
	}
	private void BalancedEndingCamera()
	{
		var ply = Scene.GetComponentInChildren<ShrimpleWalker>()?.GameObject?.Root;
		if ( ply.IsValid() )
			ply.Destroy();

		Camera.Enabled = true;
		Camera.WorldRotation = Rotation.FromPitch( 80f );
		
		Camera.WorldPosition =  new Vector3( -300f + tsCreated * 50f, 0f, 300f + tsCreated * 50f );
	}

	private void PeacefulEndingCamera()
	{
		var ply = Scene.GetComponentInChildren<ShrimpleWalker>()?.GameObject?.Root;
		if ( ply.IsValid() )
			ply.Destroy(); 
		
		Camera.Enabled = true;	
		Camera.WorldRotation = Rotation.From( new Angles( 20f, 90f, 0f ) );

		Camera.WorldPosition = new Vector3( 0f, -200f + tsCreated * 30f, 100f + tsCreated * 10f );
	}
}
