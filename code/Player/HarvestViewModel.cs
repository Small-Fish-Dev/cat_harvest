namespace CatHarvest.Player;

public class HarvestViewModel : BaseViewModel
{
	float walkBob = 0f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/viewmodel/viewmodel.vmdl" );
	}

	public override void PlaceViewmodel()
	{
		base.PlaceViewmodel();

		if ( Game.LocalPawn is not HarvestPlayer ply || !ply.IsValid() )
			return;

		// Thank you DM98 for this piece of code
		var rotationDistance = Rotation.Distance( Camera.Rotation );

		Position = Camera.Position;
		Rotation = Rotation.Lerp( Rotation, Camera.Rotation, RealTime.Delta * rotationDistance * 1.1f );

		var speed = Game.LocalPawn.Velocity.Length.LerpInverse( 0, 400 );
		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;

		if ( ply.GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		Position += up * MathF.Sin( walkBob ) * speed * -1;
		Position += left * MathF.Sin( walkBob * 0.6f ) * speed * -0.5f;
	}
}
