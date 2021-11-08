
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

			}

			Music = PlaySound( "relax" );

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
