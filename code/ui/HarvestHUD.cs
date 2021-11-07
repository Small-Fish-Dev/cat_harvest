using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

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

	public class CatInventory : Panel
	{

		Label inventoryLabel;

		public CatInventory()
		{

			Panel titleContainer = Add.Panel( "titleContainer" );
			inventoryLabel = titleContainer.Add.Label( "Inventory (0/96)", "title" );

			Panel itemsContainer = Add.Panel( "itemsContainer" );
			for( int i = 0; i < 96; i++ )
			{

				itemsContainer.Add.Panel( "item" );

			}

		}

		public override void Tick()
		{

			HarvestPlayer ply = Local.Pawn as HarvestPlayer;

			inventoryLabel.Text = $"Inventory ({ply.CatsHarvested}/96)";

		}

	}

	public partial class HarvestHUD : Sandbox.HudEntity<RootPanel>
	{

		public HarvestHUD()
		{

			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "ui/HarvestHUD.scss" );

			RootPanel.AddChild<CatCounter>( "CatCounter" );
			RootPanel.AddChild<CatInventory>( "Inventory" );

		}

	}

}
