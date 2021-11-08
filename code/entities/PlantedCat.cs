using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	public partial class PlantedCat : AnimEntity
	{

		public override void Spawn()
		{

			base.Spawn();

			Tags.Add( "Cat" );

			SetModel( "models/cat/cat.vmdl" );
			Scale = 2f;
			CollisionGroup = CollisionGroup.Prop;
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

					Position = pos + Vector3.Down * 20f,
					Rotation = Rotation.FromPitch( 90 )

				};

			}

		}

	}

}
