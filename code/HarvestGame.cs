
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cat_Harvest
{

	public partial class HarvestGame : Sandbox.Game
	{

		[Net] public List<WalkingCat> AllCats { get; set; } = new();
		public Sound Music { get; set; }

		public HarvestGame()
		{

			if ( IsServer )
			{

				new HarvestHUD();

				new WalkingCat
				{

					Position = new Vector3( Rand.Float( 1500f ) - 800f, Rand.Float( 1500f ) - 800f, 25f ),
					Boring_DoNotActivate = true,
					Scale = 0.7f

				}.SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -0.5f, -0.5f, -0.5f ), new Vector3( 0.5f, 0.5f, 0f ) ); //Needs physics to be able to be picked up;

			}
			else
			{

				Music = PlaySound( "relax" );

			}


		}

		public override void ClientJoined( Client client )
		{

			base.ClientJoined( client );

			var player = new HarvestPlayer();
			client.Pawn = player;

			player.Respawn();

		}


		[ServerCmd( "cat" )] // REMEMBER TO REMOVE THIS
		public static void Cat()
		{

			foreach( Client client in Client.All )
			{

				HarvestPlayer ply = client.Pawn as HarvestPlayer;

				ply.CatsUprooted++;
				ply.CatsHarvested++;

			}

		}

	}

}
