using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace Cat_Harvest
{

	public class CatCounter : Panel
	{

		Label catsLabel;

		public CatCounter()
		{

			Add.Label( "Cats uprooted", "subtitle" );
			catsLabel = Add.Label( "0/96", "title" );

		}

		public override void Tick()
		{

			HarvestPlayer ply = Local.Pawn as HarvestPlayer;
			catsLabel.Text = $"{ply.CatsUprooted}/96";

			HarvestGame current = HarvestGame.Current as HarvestGame;
			SetClass( "hidden", current.Finishing );

		}
		
	}

	public class Instructions : Panel
	{

		public Instructions()
		{

			Panel titleContainer = Add.Panel( "titleContainer" );
			titleContainer.Add.Label( "Instructions", "title" );

			string forward = Input.GetKeyWithBinding( "iv_forward" ).ToUpper();
			string left = Input.GetKeyWithBinding( "iv_left" ).ToUpper();
			string back = Input.GetKeyWithBinding( "iv_back" ).ToUpper();
			string right = Input.GetKeyWithBinding( "iv_right" ).ToUpper();
			string sprint = Input.GetKeyWithBinding( "iv_sprint" ).ToUpper();
			string use = Input.GetKeyWithBinding( "iv_use" ).ToUpper();
			string attack = Input.GetKeyWithBinding( "iv_attack" ).ToUpper();
			string score = Input.GetKeyWithBinding( "iv_score" ).ToUpper();


			Panel descriptionContainer = Add.Panel( "descriptionContainer" ); // Haha, fuck centering text
			descriptionContainer.Add.Label( $"[ { forward } ] [ { left } ] [ { back } ] [ { right } ] to walk.", "description" );
			descriptionContainer.Add.Label( $"Use [ { sprint } ] to run.", "description" );
			descriptionContainer.Add.Label( $"Uproot the cats by pressing [ { use } ]", "description" );
			descriptionContainer.Add.Label( $"Decide their fate with your cursor and press [ { attack } ]", "description" );
			descriptionContainer.Add.Label( $"Open the inventory by pressing [ { score } ]", "description" );
			descriptionContainer.Add.Label( $"Uproot all the cats and find all the secret endings", "objective" );

		}

		public override void Tick()
		{

			HarvestPlayer ply = Local.Pawn as HarvestPlayer;
			SetClass( "closed", ply.CloseInstructions );

		}

	}

	public class CatInventory : Panel
	{

		Label inventoryLabel;
		List<Panel> items = new();

		public CatInventory()
		{

			Panel titleContainer = Add.Panel( "titleContainer" );
			inventoryLabel = titleContainer.Add.Label( "Inventory (0/96)", "title" );

			Panel itemsContainer = Add.Panel( "itemsContainer" );

			for( int i = 0; i < 96; i++ )
			{

				Panel slot = itemsContainer.Add.Panel( "item" );
				slot.SetClass( "hide", true );
				items.Add( slot );

			}

		}

		public override void Tick()
		{

			HarvestPlayer ply = Local.Pawn as HarvestPlayer;
			inventoryLabel.Text = $"Inventory ({ply.CatsHarvested}/96)";

			for( int i = 0; i < 96; i++ )
			{

				if( i < ply.CatsHarvested )
				{

					items[i].SetClass( "hide", false );

				}
				else
				{

					items[i].SetClass( "hide", true );

				}

			}

			HarvestGame current = HarvestGame.Current as HarvestGame;
			SetClass( "closed", !ply.OpenInventory || current.Finishing );

		}

	}

	public class Popup : Panel
	{

		public Popup()
		{

			string use = Input.GetKeyWithBinding( "iv_use" ).ToUpper();

			Add.Label( $"Uproot the cat [ { use } ]", "title" );

		}

		public override void Tick()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;
			HarvestPlayer ply = Local.Pawn as HarvestPlayer;
			SetClass( "closed", !ply.DisplayPopup || current.Finishing );

		}

	}

	public class Choices : Panel
	{

		public Choices()
		{

			Add.Button( "", "button", () => 
			{

				HarvestPlayer.Harvest();

			} ).Add.Label( "HARVEST", "title" );

			Add.Button( "", "button", () => 
			
			{

				HarvestPlayer.Rescue(); 
			
			} ).Add.Label( "RESCUE", "title" );

		}

		public override void Tick()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;
			HarvestPlayer ply = Local.Pawn as HarvestPlayer;
			SetClass( "closed", !ply.HasCat || current.Finishing );

		}

	}

	public class EndingScreen : Panel
	{

		Label title;
		Label subtitle;

		public EndingScreen()
		{

			title = Add.Label( "ENDING", "title" );
			subtitle = Add.Label( "SUBTITLE HERE", "subtitle" );


		}

		public override void Tick()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			SetClass( "hidden", !current.EndState );
			title.Text = $"{ HarvestGame.EndingTitles[ current.Ending ] }";
			subtitle.Text = $"{HarvestGame.EndingDescriptions[ current.Ending ] }";

		}

	}

	public class Jumpscare : Panel
	{

		public Jumpscare()
		{


		}

		public override void Tick()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			SetClass( "hidden", !current.Jumpscare );

		}

	}

	public class CrossHair : Panel
	{

		public CrossHair()
		{


		}

		public override void Tick()
		{

			HarvestGame current = HarvestGame.Current as HarvestGame;

			SetClass( "hidden", current.Finishing );

		}

	}


	public partial class HarvestHUD : Sandbox.HudEntity<RootPanel>
	{

		public HarvestHUD()
		{

			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "ui/HarvestHUD.scss" );

			RootPanel.AddChild<CrossHair>( "CrossHair" );
			RootPanel.AddChild<CatCounter>( "CatCounter" );
			RootPanel.AddChild<CatInventory>( "Inventory" );
			RootPanel.AddChild<Instructions>( "Instructions" );
			RootPanel.AddChild<Popup>( "Popup" );
			RootPanel.AddChild<Choices>( "Choices" );
			RootPanel.AddChild<EndingScreen>( "EndingScreen" );
			RootPanel.AddChild<Jumpscare>( "Jumpscare" );

		}

	}

}
