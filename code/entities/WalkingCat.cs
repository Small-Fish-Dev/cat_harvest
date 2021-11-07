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

			SetModel( "models/citizen/citizen.vmdl" );
			CollisionGroup = CollisionGroup.Prop;
			SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 16, 2 ) );

		}

		TimeSince lastMove = 0f;

		[Event.Tick.Server]
		public void Tick()
		{
			/*
			var forward = Vector3.Dot( Rotation.Forward, Velocity.Normal );
			var sideward = Vector3.Dot( Rotation.Right, Velocity.Normal );
			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();
			SetAnimFloat( "move_direction", angle );

			SetAnimFloat( "wishspeed", Velocity.Length * 1.5f );
			SetAnimFloat( "walkspeed_scale", 1.0f / 10.0f );
			SetAnimFloat( "runspeed_scale", 1.0f / 320.0f );
			SetAnimFloat( "duckspeed_scale", 1.0f / 80.0f );
			*/

			if ( lastMove > 2f )
			{

				Velocity = 1f;
				lastMove = 0f;

			}

			Position += Velocity;
			Velocity *= 0.95f;

		}

		protected virtual void Move( float speed = 0f)
		{

			Vector3 direction = Vector3.Zero;
			Vector3 curVelocity = Velocity * 0.95f;

			Position += curVelocity;
			Velocity = curVelocity;

		}

		[ServerCmd("spawncat")]
		public static void SpawnCat()
		{

			var pos = ConsoleSystem.Caller.Pawn.Position;
			var npc = new WalkingCat
			{

				Position = pos,

			};

		}

	}

}
