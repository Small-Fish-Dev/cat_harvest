
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cat_Harvest
{

	public partial class HarvestGame : Sandbox.Game
	{

		[Net] public bool EndState { get; set; } = false;
		[Net] public int Ending { get; set; } = 0;
		public static readonly string[] EndingTitles = new string[] {
			"NEUTRAL ENDING",
			"PEACEFUL ENDING",
			"BALANCED ENDING",
			"GENOCIDE ENDING",
			"SECRET ENDING"
		};
		public static readonly string[] EndingDescriptions = new string[] {
			"After a hard day of work, you went back home to sleep.",
			"The world has been restored - and everyone is much happier.",
			"Perfectly balanced, as all things should be.",
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

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 2;

		}

		public static void NeutralEnding( HarvestPlayer ply  )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 0;

		}

		public static void PeacefulEnding( HarvestPlayer ply  )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 1;

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

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 3;

		}

		public static async void SecretEnding( HarvestPlayer ply )
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			current.EndState = true;
			current.Ending = 4;

			await current.Task.Delay( 6000 );

			ply.Client.Kick(); // fuck you that's why

		}

		[ServerCmd( "ending" )] //TODO REMEMBER DELETE!!!
		public static void DoEnding( string ending)
		{

			var ply = ConsoleSystem.Caller.Pawn as HarvestPlayer;

			switch ( ending )
			{

				case "balanced":
					BalancedEnding( ply );
					break;

				case "peaceful":
					PeacefulEnding( ply );
					break;

				case "genocide":
					GenocideEnding( ply );
					break;

				case "secret":
					SecretEnding( ply );
					break;

				default:
					NeutralEnding( ply );
					break;

			};
				
		}

	}

}
