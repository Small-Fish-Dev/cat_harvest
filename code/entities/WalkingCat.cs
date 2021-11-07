using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	public partial class WalkingCat : AnimEntity
	{

		public override void Spawn()
		{

			base.Spawn();

			Tags.Add( "Cat" );

			SetModel( "models/citizen/citizen.vmdl" );
			CollisionGroup = CollisionGroup.Prop;
			SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 16, 2 ) );

		}

		float nextMove = 0f;

		[Event.Tick.Server]
		public void Tick()
		{
			
			SetAnimFloat( "move_x", 7f * Velocity.Length );

			float friction = 0.2f;

			if ( nextMove <= Time.Now )
			{

				Velocity = new Vector3( Rand.Float( 10f ) - 5f, Rand.Float( 10f ) - 5f, 0f );

				nextMove = Time.Now + 6f + Rand.Float( 2f );

			}

			TraceResult traceGround = Trace.Ray( Position + Vector3.Up * 16, Position + Vector3.Down * 32 )
				.Ignore( this )
				.WithoutTags( "Player" )
				.WithoutTags( "Cat" )
				.Run();

			if ( traceGround.Hit )
			{

				Position = traceGround.EndPos;
				Position += Rotation.Forward * 10 * Velocity.Length * Time.Delta;

				Rotation rotation = Velocity.EulerAngles.ToRotation();
				Rotation = Rotation.Slerp( Rotation, rotation, 2 * Time.Delta );

			}
			else
			{

				Position += Vector3.Down * 100 * Time.Delta;

			}

			Velocity *= 1 - Time.Delta * friction;

		}

		[ServerCmd("spawncat")]
		public static void SpawnCat()
		{

			var pos = ConsoleSystem.Caller.Pawn.Position;
			var npc = new WalkingCat
			{

				Position = pos

			};

		}

	}

}
