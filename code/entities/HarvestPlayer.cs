using Sandbox;
using System;
using System.Linq;

namespace Cat_Harvest
{
	public partial class HarvestPlayer : Sandbox.Player
	{

		[Net] public int CatsUprooted { get; set; } = 0;
		[Net] public int CatsHarvested { get; set; } = 0;
		public bool OpenInventory { get; set; } = false;
		public bool CloseInstructions { get; set; } = false;
		[Net] public bool DisplayPopup { get; set; } = false;

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
		TimeSince autoClose = 0f;

		public override void Simulate( Client cl )
		{

			base.Simulate( cl );

			if ( IsClient )
			{

				if ( Input.Down( InputButton.Score ) )
				{

					OpenInventory = true;
					CloseInstructions = true;

				}
				else
				{

					OpenInventory = false;

				}

				if( autoClose >= 15f )
				{

					CloseInstructions = true;

				}

			}



			TraceResult eyeTrace = Trace.Ray( Input.Cursor, 100f )
			.Size( new Vector3( 20f, 20f, 20f ) )
			.Ignore( PhysicsWorld.WorldBody.Entity )
			.WithTag( "Cat" )
			.Run();

			if ( eyeTrace.Hit )
			{

				DisplayPopup = true;
				var cat = eyeTrace.Entity;

				if ( Input.Down( InputButton.Use ) && Input.Pressed( InputButton.Use ) ) 
				{

					Sound.FromEntity( $"meow{ Rand.Int( 10 ) }", cat);
					Particles.Create( "particles/uproot.vpcf", cat, "" );

					if ( IsServer )
					{

						cat.Delete();
						CatsUprooted++;
						CatsHarvested++;

					}

				}

			}
			else
			{

				DisplayPopup = false;

			}

			

			if ( Velocity.Length > 0f && lastStep >= 70 / Velocity.Length && GroundEntity != null )
			{

				string step = $"step{ Rand.Int( 5 ) }";
				Sound.FromEntity( step, this );
				lastStep = 0f;

			}

		}

		public override void OnKilled()
		{
			//Don't die! wtf
		}

	}

}
