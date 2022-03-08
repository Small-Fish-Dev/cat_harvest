using Sandbox;
using System;
using System.Linq;

namespace Sandbox.entities
{
	[Library( "harvest_planted", Description = "Planted kitten to uproot." )]
	[Hammer.EditorModel( "models/tail/tail.vmdl" )]
	public partial class PlantedCat : AnimEntity
	{

		float playBack = Rand.Float( 1.5f ) + 0.2f;

		public override void Spawn()
		{

			base.Spawn();

			Tags.Add( "Cat" );

			SetModel( "models/tail/tail.vmdl" );
			Scale = 1.5f;
			CollisionGroup = CollisionGroup.Prop;
			PlaybackRate = playBack;
			Position -= Vector3.Up * 1; //Plant them a bit deeper
			Rotation = Rotation.FromYaw( Rand.Float( 360f ) ); //Random rotate
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

}
