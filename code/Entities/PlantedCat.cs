public sealed class HarvestableCat : Component
{
	[Property] public SkinnedModelRenderer Renderer { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		Transform.Rotation = Rotation.FromYaw( Game.Random.Float( 360f ) ); // Random yaw
		Transform.Rotation *= Rotation.FromPitch( Game.Random.Float( 15f ) ); // Some random pitch
		Transform.Rotation *= Rotation.FromRoll( Game.Random.Float( 15f ) ); // Some random roll
	}

	protected override void OnFixedUpdate()
	{
		Transform.Scale = Transform.Scale + 1 / 300f * Time.Delta; // Grow 100% every 300 seconds
	}
}


/*
	float playBack = Game.Random.Float( 1.5f ) + 0.2f;

	public override void Spawn()
	{

		base.Spawn();

		Tags.Add( "Cat" );

		SetModel( "models/tail/tail.vmdl" );
		Scale = 1.5f;

		PlaybackRate = playBack;
		Position -= Vector3.Up * 1; //Plant them a bit deeper
		Rotation = Rotation.FromYaw( Game.Random.Float( 360f ) ); //Random rotate
		SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -0.5f, -0.5f, -0.5f ), new Vector3( 0.5f, 0.5f, 0f ) );


	}

	TimeSince spawned = 0f;

	[Event.Tick.Server]
	public void OnTick()
	{

		Scale = 1.5f + spawned / 300f; //Make them grow after some time if people can't find them
		PlaybackRate = playBack + spawned / 300f; //Make them faster

	}

}

*/
