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

			SetClass( "closed", !ply.OpenInventory );

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

			HarvestPlayer ply = Local.Pawn as HarvestPlayer;

			SetClass( "closed", !ply.DisplayPopup );

		}

	}

	public partial class HarvestHUD : Sandbox.HudEntity<RootPanel>
	{

		public HarvestHUD()
		{

			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "ui/HarvestHUD.scss" );

			RootPanel.Add.Panel( "CrossHair" );
			RootPanel.AddChild<CatCounter>( "CatCounter" );
			RootPanel.AddChild<CatInventory>( "Inventory" );
			RootPanel.AddChild<Instructions>( "Instructions" );
			RootPanel.AddChild<Popup>( "Popup" );

		}

	}


}
