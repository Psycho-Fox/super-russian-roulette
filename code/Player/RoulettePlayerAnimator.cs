using Sandbox;
using Roulette;
//using Obsolete;
using System;
using System.Buffers;
using Sandbox.Internal;
using static Sandbox.Event;

public partial class RoulettePlayerAnimator : PawnAnimator
{
	public override void Simulate()
	{
		var pawn = Pawn as RoulettePlayer;
		var rot = Rotation;
		var tSpeed = 0.02f;

		Vector3 WishVelocity = pawn.Velocity;
		AnimatedEntity entity = pawn;

		DoRotation( rot, tSpeed, WishVelocity );
		DoMovement(rot, Velocity, WishVelocity, entity );
		DoLook(entity, pawn.EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		DoStates(entity, pawn);
	}

	public virtual void DoRotation( Rotation rot, float TurnSpeed, Vector3 WishVelocity)
	{
		var idealRot = Rotation.LookAt( Input.Rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp( rot, idealRot, WishVelocity.Length * Time.Delta * TurnSpeed );
		Rotation = Rotation.Clamp( idealRot, 45.0f, out var shuffle );
	}
	
	private void DoMovement(Rotation rot, Vector3 vel, Vector3 WishVel, AnimatedEntity ent)
	{
		// Velocity
		var forward = rot.Forward.Dot( vel );
		var sideward = rot.Right.Dot( vel );
		var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

		ent.SetAnimParameter( "move_direction", angle );
		ent.SetAnimParameter( "move_speed", vel.Length);
		ent.SetAnimParameter( "move_groundspeed", vel.WithZ( 0 ).Length );
		ent.SetAnimParameter( "move_y", sideward);
		ent.SetAnimParameter( "move_x", forward);
		ent.SetAnimParameter( "move_z", vel.z );

		// Wish Velocity

		var WishForward = rot.Forward.Dot( WishVel );
		var WishSideward = rot.Right.Dot( WishVel );
		var WishAngle = MathF.Atan2( WishSideward, WishForward ).RadianToDegree().NormalizeDegrees();

		ent.SetAnimParameter( "wish_direction", angle );
		ent.SetAnimParameter( "wish_speed", WishVel.Length );
		ent.SetAnimParameter( "wish_groundspeed", WishVel.WithZ( 0 ).Length );
		ent.SetAnimParameter( "wish_y", WishSideward );
		ent.SetAnimParameter( "wish_x", WishForward );
		ent.SetAnimParameter( "wish_z", WishVel.z );
	}

	public virtual void DoLook(AnimatedEntity ent, Vector3 look, float EyesWeight = 1.0f, float HeadWeight = 1.0f, float BodyWeight = 1.0f )
	{
		ent.SetAnimLookAt( "aim_eyes", look );
		ent.SetAnimLookAt( "aim_head", look );
		ent.SetAnimLookAt( "aim_body", look );

		ent.SetAnimParameter( "aim_eyes_weight", EyesWeight );
		ent.SetAnimParameter( "aim_head_weight", HeadWeight );
		ent.SetAnimParameter( "aim_body_weight", BodyWeight );
	}

	public virtual void DoStates( AnimatedEntity entity, RoulettePlayer pawn)
	{
		bool isSitting = HasTag( "sitting" );
		bool isNoclipping = HasTag( "climbing" );
		bool isGrounded = GroundEntity != null || isSitting;
		bool isSwimming = pawn.WaterLevel > 0.5f && !isSitting;

		SetAnimParameter( "b_sit", isSitting );
		SetAnimParameter( "b_grounded", isGrounded );
		SetAnimParameter( "b_noclip", isNoclipping );
		SetAnimParameter( "b_swim", isSwimming && !isSitting );

		var inverseRot = entity.Rotation.Inverse * Input.Rotation;
		var ang = inverseRot.Angles();

		SetAnimParameter( "aim_body_pitch", ang.pitch );
		SetAnimParameter( "aim_body_yaw", ang.yaw );

		if ( pawn != null && pawn.ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( this );
		}
		else
		{
			SetAnimParameter( "holdtype", 0 );
			SetAnimParameter( "aim_body_weight", 0.5f );
		}
	}
}



/*
 * void SimulateAnimation( Obsolete.PawnController controller ) // Note: PawnController is deprecated, expect refactors. 💀
	{
		if ( controller == null ) return;

		var turnSpeed = 0.02f;
		var idealRotation = Rotation.LookAt( Input.Rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp( Rotation, idealRotation, controller.WishVelocity.Length * Time.Delta * turnSpeed );
		Rotation = Rotation.Clamp( idealRotation, 45.0f, out var shuffle );

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( this );

		animHelper.WithWishVelocity( controller.WishVelocity );
		animHelper.WithVelocity( controller.Velocity );
		animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = Input.Rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, controller.HasTag( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );
		animHelper.IsGrounded = GroundEntity != null;
		animHelper.IsSitting = controller.HasTag( "sitting" );
		animHelper.IsNoclipping = controller.HasTag( "climbing" ); // This is fucking useless for my purposes.
		animHelper.IsSwimming = WaterLevel >= 0.5f; // Who the fuck is going to be swimming in a saloon?
		animHelper.IsWeaponLowered = false;

		if ( controller.HasEvent( "jump" ) ) animHelper.TriggerJump();
		if ( ActiveChild != lastWeapon ) animHelper.TriggerDeploy();

		if ( ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( animHelper );
		}
		else
		{
			animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			animHelper.AimBodyWeight = 0.5f;
		}

		lastWeapon = ActiveChild;
	}*/
