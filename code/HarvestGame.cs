
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cat_Harvest
{

	public partial class HarvestGame : Sandbox.Game
	{

		[Net]
		public static int CatsCollected { get; set; } = 0;

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

		[ServerCmd( "cat" )]
		public static void Cat()
		{

			Assert.NotNull( ConsoleSystem.Caller );

			CatsCollected++;

		}
	}

}
