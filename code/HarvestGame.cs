
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cat_Harvest
{

	public partial class HarvestGame : Sandbox.Game
	{

		[Net] public static bool EndState { get; set; } = false;
		[Net] public static int Ending { get; set; } = 0;
		public static readonly string[] EndingTitles = new string[] {
			"NEUTRAL ENDING",
			"PEACEFUL ENDING",
			"BALANCED ENDING",
			"GENOCIDE ENDING",
			"SECRET ENDING"
		};
		public static readonly string[] EndingDescriptions = new string[] {
			"After a hard day of work, you went back home to sleep.",
			"You've rescued all kittens, they will live peacefully.",
			"Perfectly balandes, as all things should be.",
			"Run.",
			"You found El Wiwi. You passed out not long after."
		};

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

			switch ( harvested )
			{

				case 48:
					BalancedEnding( ply );
					break;

				case <= 0:
					PeacefulEnding( ply );
					break;

				case >= 96:
					GenocideEnding( ply );
					break;

				default:
					NeutralEnding( ply );
					break;

			}

		}

		public static void BalancedEnding( HarvestPlayer ply )
		{



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
