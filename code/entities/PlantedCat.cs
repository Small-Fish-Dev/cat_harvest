using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	[Library( "harvest_planted", Description = "Planted kitten to uproot." )]
	public partial class PlantedCat : AnimEntity
	{

		public override void Spawn()
		{

			base.Spawn();

			Tags.Add( "Cat" );

			SetModel( "models/tail/tail.vmdl" );
			Scale = 1.5f;
			CollisionGroup = CollisionGroup.Prop;
			PlaybackRate = Rand.Float( 1.5f ) + 0.2f;
			Position -= Vector3.Up * 2;
			SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 16, 2 ) ); //Remove collisions when done? Can't pick them up in final game anyways
			

		}

		[ServerCmd("plantcat")] //TODO REMEMBER DELETE!!!
		public static void SpawnCat()
		{

			var pos = ConsoleSystem.Caller.Pawn.Position;

			for ( int i = 0; i < 1; i++ )
			{

				var npc = new PlantedCat
				{

					Position = pos + Vector3.Down * 6f,
					Rotation = Rotation.FromPitch( 90 )

				};

			}

		}

	}

}
