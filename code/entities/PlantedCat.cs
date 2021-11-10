using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	[Library( "harvest_planted", Description = "Planted kitten to uproot." )]
	[Hammer.EditorModel( "models/tail/tail.vmdl" )]
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
			Position -= Vector3.Up * 1; //Plant them a bit deeper
			Rotation = Rotation.FromYaw( Rand.Float( 360f ) ); //Random rotate
			SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -0.5f, -0.5f, -0.5f ), new Vector3( 0.5f, 0.5f, 0f ) );
			

		}

	}

}
