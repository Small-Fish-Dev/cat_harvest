
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sandbox.ui;

namespace Cat_Harvest
{

	public partial class HarvestGame : Sandbox.Game
	{

		[Net] public List<WalkingCat> AllCats { get; set; } = new();
		[Net] public WalkingCat SecretCat { get; set; }
		[Net] public bool Finishing { get; set; } = false;
		public HarvestHUD HUD { get; set; }
		public Sound Music { get; set; }

		public HarvestGame()
		{

			if ( IsServer )
			{

				HUD = new HarvestHUD();

				SecretCat = new WalkingCat
				{

					Position = new Vector3( Rand.Float( 1500f ) - 800f, Rand.Float( 1500f ), 25f ),
					Scale = 0.1f

				};

				SecretCat.SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -0.5f, -0.5f, -0.5f ), new Vector3( 0.5f, 0.5f, 0f ) ); //Needs physics to be able to be picked up;

			}
			else
			{

				Music = PlaySound( "relax" );

			}

			Precache.Add( "angry0" );
			Precache.Add( "sounds/angry0.vsnd" ); //To be sure

		}

		public override void ClientJoined( Client client )
		{

			base.ClientJoined( client );

			var player = new HarvestPlayer();
			client.Pawn = player;

			player.Respawn();

		}

	}

}
