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

			catsLabel.Text = $"{ply.CatsCollected}/96";

		}

		

	}

	public partial class HarvestHUD : Sandbox.HudEntity<RootPanel>
	{

		public HarvestHUD()
		{

			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "ui/HarvestHUD.scss" );

			RootPanel.AddChild<CatCounter>( "CatCounter" );

		}

	}

}
