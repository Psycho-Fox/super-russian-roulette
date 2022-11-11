using Sandbox;
using Roulette;
using Obsolete;
using System;

namespace Roulette;
public partial class RoulettePlayer : Player
{
	// Need later: private TimeSince timeSinceJumpReleased;

	// Load the appearance of each player who passes in.
	public ClothingContainer Clothing = new();

	// Init player
	public RoulettePlayer()
	{

	}

	// Init client
	public RoulettePlayer(Client client) : this()
	{
		Clothing.LoadFromClient( client );
	}

	public override void Spawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		/*{
			SprintSpeed = 200f,
			WalkSpeed = 100f
		};*/

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Clothing.DressEntity( this );

		CameraMode = new FirstPersonCamera();
		Animator = new RoulettePlayerAnimator();

		base.Respawn();
	}

	public override void Simulate(Client client)
	{
		base.Simulate( client );

		if (Input.ActiveChild != null)
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		var controller = GetActiveController();
		if (controller != null)
		{
			EnableSolidCollisions = !controller.HasTag( "noclip" );

			//SimulateAnimation( controller );
		}

		TickPlayerUse();
		SimulateActiveChild( client, ActiveChild );

		if (Input.Pressed(InputButton.View) )
		{
			if (CameraMode is RouletteThirdPersonCamera)
			{
				CameraMode = new FirstPersonCamera();
				client.AddInt( "myass" );
				Log.Info( $"My Ass: {client.GetInt("myass")}" );
			}
			else
			{
				CameraMode = new RouletteThirdPersonCamera();
			}
		}
		
		/*if (Input.Released(InputButton.Jump))
		{
			if(timeSinceJumpReleased < 0.3f)
			{
				Game.Current?.DoPlayerNoclip( client );
			}

			timeSinceJumpReleased = 0;
		}


		if(Input.Left!=0 || Input.Forward != 0)
		{
			timeSinceJumpReleased = 1;
		}*/	
	}

	// Unused atm, readonly Entity lastWeapon;

	// NOTE: Moved to RoulettePlayerAnimator.cs

}
