using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	public partial class HarvestPlayer : Sandbox.Player
	{

		[Net] public int CatsUprooted { get; set; } = 0;
		[Net] public int CatsHarvested { get; set; } = 0;
		[Net] public bool OpenInventory { get; set; } = false;

		public override void Respawn()
		{

			SetModel( "models/citizen/citizen.vmdl" );

			Tags.Add( "Player" );

			Controller = new WalkController() { WalkSpeed = 100.0f, DefaultSpeed = 100.0f, SprintSpeed = 160.0f };

			Animator = new StandardPlayerAnimator();

			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = false;

			base.Respawn();

		}

		TimeSince lastStep = 0f;

		public override void Simulate( Client cl )
		{

			base.Simulate( cl );

			if ( IsServer )
			{
				if( Input.Down( InputButton.Score ) )
				{

					OpenInventory = true;

				}
				else
				{

					OpenInventory = false;

				}
				
			}

			if ( Velocity.Length > 0f  && lastStep >= 70/Velocity.Length && GroundEntity != null )
			{

				string step = $"step{Rand.Int( 5 )}";
				PlaySound( step );
				lastStep = 0f;

			}
		}

		public override void OnKilled()
		{
			//Don't die!
		}

	}

}
