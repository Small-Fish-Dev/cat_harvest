
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cat_Harvest
{

	public partial class HarvestGame : Sandbox.Game
	{

		public HarvestGame()
		{
			if ( IsServer )
			{

				new HarvestHUD();

			}

			if ( IsClient )
			{

				PlaySound( "relax" );

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
