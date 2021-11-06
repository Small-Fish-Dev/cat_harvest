using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	partial class HarvestPlayer : Sandbox.Player
	{

		[Net]
		public int CatsCollected { get; set; } = 0;

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WalkController();

			Animator = new StandardPlayerAnimator();

			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = false;

			base.Respawn();

		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( IsServer && Input.Pressed( InputButton.Attack1 ) )
			{
				var ragdoll = new ModelEntity();
				ragdoll.SetModel( "models/citizen/citizen.vmdl" );  
				ragdoll.Position = EyePos + EyeRot.Forward * 40;
				ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
				ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 1000;
			}
		}

		public override void OnKilled()
		{
			//Don't die!
		}
	}
}
