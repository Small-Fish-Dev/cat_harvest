
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

		public static void EndGame( HarvestPlayer ply, int harvested )
		{

			if ( harvested >= 1 && harvested < 96 )
			{

				NeutralEnding( ply );

			}
			else if ( harvested <= 0 )
			{

				PeacefulEnding( ply );

			}
			else if ( harvested >= 96 )
			{

				GenocideEnding( ply );

			}

		}

		public static void NeutralEnding( HarvestPlayer ply  )
		{



		}

		public static void PeacefulEnding( HarvestPlayer ply  )
		{



		}

		public static void GenocideEnding( HarvestPlayer ply  )
		{

			for ( int i = 0; i < ply.CatsHarvested; i++ )
			{

				var cat = new WalkingCat
				{

					Position = ply.Position

				};

			}

			ply.CatsHarvested = 0;

		}

	}

}
