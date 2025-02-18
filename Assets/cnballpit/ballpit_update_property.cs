﻿
using UnityEngine;
using VRC.SDKBase;

#if UDON
using UdonSharp;
using VRC.Udon;

using BrokeredUpdates;


public class ballpit_update_property : UdonSharpBehaviour
{
	public float SetValueGravity;
	public float SetValueFriction;
	public int NumModes;
	public bool  UpdateGravityFriction;
	public bool  UpdateEnable;
	public bool  UpdateDrawMode;
	public bool  MasterOnly;
	public bool  AdjustQualityMode;
	public int   NumQualityModes;
	public GameObject MainControl;

	public void UpdateMaterialWithSelMode()
	{
		int mode = 1;
		ballpit_stable_control m = MainControl.GetComponent<ballpit_stable_control>();
		int enabled = (!MasterOnly || Networking.IsMaster)?1:0;
		
		if( UpdateEnable )
		{
			if( m.balls_reset )
				mode = 0;
		}
		
		if( UpdateDrawMode )
		{
			mode = m.mode + 1;
			enabled = 1;
		}
		
		if( AdjustQualityMode )
		{
			mode = m.qualitymode;
			enabled = 1;
		}

		if( UpdateGravityFriction )
		{
			if( m.gravityF != SetValueGravity )
				mode = 0;
			enabled = 1;
		}

		GetComponent<MeshRenderer> ().material.SetFloat( "_SelMode", mode );
		GetComponent<MeshRenderer> ().material.SetFloat( "_UserEnable", enabled );
		
	}

	public void _SnailUpdate()
	{
		UpdateMaterialWithSelMode();
	}
	void Start()
	{
		ballpit_stable_control m = MainControl.GetComponent<ballpit_stable_control>();
		m.AddUpdatable( this );
		GameObject.Find( "BrokeredUpdateManager" ).GetComponent<BrokeredUpdateManager>()._RegisterSnailUpdate( this );
	}
	

	public override void Interact()
	{
		if( !MasterOnly || Networking.IsMaster ) //|| Networking.IsInstanceOwner )
		{
			ballpit_stable_control m = MainControl.GetComponent<ballpit_stable_control>();
			Networking.SetOwner( Networking.LocalPlayer, MainControl );
			if( UpdateGravityFriction )
			{
				m.gravityF = SetValueGravity;
				m.friction = SetValueFriction;
			}
			if( UpdateEnable )
			{
				m.balls_reset = !m.balls_reset;
			}
			
			if( UpdateDrawMode )
			{
				m.mode = ( m.mode + 1 ) % NumModes;
			}
			
			if( AdjustQualityMode )
			{
				m.qualitymode = ( m.qualitymode + 1 ) % NumQualityModes;
			}
			
			m.ModeUpdate();
		}
	}
}

#else
public class ballpit_update_property : MonoBehaviour { }
#endif