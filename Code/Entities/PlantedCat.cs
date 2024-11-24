using Sandbox;
using System;

[Category( "CatHarvest" )]
[Title( "Planted Cat" )]
[Icon( "cat" )]
[Alias( "PlantedCat" )]
[EditorHandle("materials/pebble.png")]
public sealed class HarvestableCat : Component
{
	[Property] public SkinnedModelRenderer Renderer { get; set; }
	private TimeSince spawn;
	protected override void OnStart()
	{
		base.OnStart();
		Tags.Add( "Cat" );
		WorldRotation = Rotation.FromYaw( Game.Random.Float( 360f ) ); // Random yaw
		WorldRotation *= Rotation.FromPitch( Game.Random.Float( 15f ) ); // Some random pitch
		WorldRotation *= Rotation.FromRoll( Game.Random.Float( 15f ) ); // Some random roll
	}

	protected override void OnFixedUpdate()
	{
		WorldScale += 1 / 300f * Time.Delta; // Grow 100% every 300 seconds
	}
}
