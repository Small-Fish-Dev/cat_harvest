﻿namespace CatHarvest.UI
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
			var ply = Scene.GetComponentInChildren<HarvestPlayer>();
			if ( !ply.IsValid() ) return;
			catsLabel.Text = $"{ply.CatsUprooted}/96";

			SetClass( "hidden", HarvestGame.The.Finishing );
		}
	}

	public class Instructions : Panel
	{
		public Instructions()
		{
			var titleContainer = Add.Panel( "titleContainer" );
			titleContainer.Add.Label( "Instructions", "title" );

			var forward = Input.GetButtonOrigin( "Forward" ).ToUpper();
			var left = Input.GetButtonOrigin( "Left" ).ToUpper();
			var back = Input.GetButtonOrigin( "Backward" ).ToUpper();
			var right = Input.GetButtonOrigin( "Right" ).ToUpper();
			var sprint = Input.GetButtonOrigin( "Run" ).ToUpper();
			var use = Input.GetButtonOrigin( "Pick" ).ToUpper();
			var score = Input.GetButtonOrigin( "Inventory" ).ToUpper();


			var descriptionContainer = Add.Panel( "descriptionContainer" ); // Haha, fuck centering text
			descriptionContainer.Add.Label( $"[ {forward} ] [ {left} ] [ {back} ] [ {right} ] to walk.", "description" );
			descriptionContainer.Add.Label( $"Use [ {sprint} ] to run.", "description" );
			descriptionContainer.Add.Label( $"Uproot the cats by pressing [ {use} ]", "description" );
			descriptionContainer.Add.Label( $"Open the inventory by pressing [ {score} ]", "description" );
			descriptionContainer.Add.Label( $"Uproot all the cats and find all the 5 endings", "objective" );
		}

		public override void Tick()
		{
			var ply = Scene.GetComponentInChildren<HarvestPlayer>();
			if ( !ply.IsValid() ) return;
			SetClass( "closed", ply.CloseInstructions );
		}
	}

	public class CatInventory : Panel
	{
		Label inventoryLabel;
		List<Panel> items = new();

		public CatInventory()
		{
			var titleContainer = Add.Panel( "titleContainer" );
			inventoryLabel = titleContainer.Add.Label( "Inventory (0/96)", "title" );

			var itemsContainer = Add.Panel( "itemsContainer" );

			for ( var i = 0; i < 96; i++ )
			{
				var slot = itemsContainer.Add.Panel( "item" );
				slot.SetClass( "hide", true );
				items.Add( slot );
			}
		}

		public override void Tick()
		{
			var ply = Scene.GetComponentInChildren<HarvestPlayer>();
			if ( !ply.IsValid() ) return;
			inventoryLabel.Text = $"Inventory ({ply.CatsHarvested}/96)";

			for ( var i = 0; i < 96; i++ )
			{
				if ( i < ply.CatsHarvested )
				{
					items[i].SetClass( "hide", false );
				}
				else
				{
					items[i].SetClass( "hide", true );
				}
			}

			SetClass( "closed", !ply.OpenInventory || HarvestGame.The.Finishing );
		}
	}

	public class Popup : Panel
	{
		public Popup()
		{
			var use = Input.GetButtonOrigin( "Pick" ).ToUpper();

			Add.Label( $"Uproot the cat [ {use} ]", "title" );
		}

		public override void Tick()
		{
			var ply = Scene.GetComponentInChildren<HarvestPlayer>();
			if ( !ply.IsValid() ) return;
			SetClass( "closed", ply.Popup != HarvestPlayer.PopupType.Uproot || HarvestGame.The.Finishing );
		}
	}

	public class SecretPopup : Panel
	{
		public SecretPopup()
		{
			var use = Input.GetButtonOrigin( "Pick" ).ToUpper();

			Add.Label( $"Pick up El Wiwi [ {use} ]", "title" );
		}

		public override void Tick()
		{
			var ply = Scene.GetComponentInChildren<HarvestPlayer>();
			if ( !ply.IsValid() ) return;
			SetClass( "closed", ply.Popup != HarvestPlayer.PopupType.SecretPickUp || HarvestGame.The.Finishing );
		}
	}

	public class Choices : Panel
	{
		public Choices()
		{
			Add.Button( "", "button", () => { HarvestPlayer.Harvest(); } ).Add.Label( "HARVEST", "title" );

			Add.Button( "", "button", () => { HarvestPlayer.Rescue(); } ).Add.Label( "RESCUE", "title" );
		}

		public override void Tick()
		{
			var ply = Scene.GetComponentInChildren<HarvestPlayer>();
			if ( !ply.IsValid() ) return;
			SetClass( "closed", !ply.HasCat || HarvestGame.The.Finishing );
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
			SetClass( "hidden", !HarvestGame.The.EndState );
			title.Text = $"{HarvestGame.EndingTitles[HarvestGame.The.Ending]}";
			subtitle.Text = $"{HarvestGame.EndingDescriptions[HarvestGame.The.Ending]}";
		}
	}

	public class Jumpscare : Panel
	{
		public override void Tick()
		{
			SetClass( "hidden", !HarvestGame.The.Jumpscare );
		}
	}

	public class CrossHair : Panel
	{
		public override void Tick()
		{
			SetClass( "hidden", HarvestGame.The.Finishing );
		}
	}


	public partial class HarvestHUD : PanelComponent
	{
		protected override void OnStart()
		{
			Panel.StyleSheet.Load( "ui/HarvestHUD.scss" );
			Panel.AddChild<CrossHair>( "CrossHair" );
			Panel.AddChild<CatCounter>( "CatCounter" );
			Panel.AddChild<CatInventory>( "Inventory" );
			Panel.AddChild<Instructions>( "Instructions" );
			Panel.AddChild<Popup>( "Popup" );
			Panel.AddChild<SecretPopup>( "Popup" );
			Panel.AddChild<Choices>( "Choices" );
			Panel.AddChild<EndingScreen>( "EndingScreen" );
			Panel.AddChild<Jumpscare>( "Jumpscare" );
		}
	}

}
