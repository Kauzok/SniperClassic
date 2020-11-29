﻿using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SniperClassicSkills
{
	class CombatRoll2 : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();

			scopeController = base.gameObject.GetComponent<SniperClassic.ScopeController>();

			Util.PlaySound(CombatRoll2.dodgeSoundString, base.gameObject);
			this.animator = base.GetModelAnimator();
			ChildLocator component = this.animator.GetComponent<ChildLocator>();
			if (base.isAuthority && base.inputBank && base.characterDirection)
			{
				this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
			}
			Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.forwardDirection;
			Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
			float num = Vector3.Dot(this.forwardDirection, rhs);
			float num2 = Vector3.Dot(this.forwardDirection, rhs2);
			this.animator.SetFloat("forwardSpeed", num, 0.1f, Time.fixedDeltaTime);
			this.animator.SetFloat("rightSpeed", num2, 0.1f, Time.fixedDeltaTime);
			if (Mathf.Abs(num) > Mathf.Abs(num2))
			{
				base.PlayAnimation("Body", (num > 0f) ? "DodgeForward" : "DodgeBackward", "Dodge.playbackRate", this.duration);
			}
			else
			{
				base.PlayAnimation("Body", (num2 > 0f) ? "DodgeRight" : "DodgeLeft", "Dodge.playbackRate", this.duration);
			}
			if (CombatRoll2.jetEffect)
			{
				Transform transform = component.FindChild("LeftJet");
				Transform transform2 = component.FindChild("RightJet");
				if (transform)
				{
					UnityEngine.Object.Instantiate<GameObject>(CombatRoll2.jetEffect, transform);
				}
				if (transform2)
				{
					UnityEngine.Object.Instantiate<GameObject>(CombatRoll2.jetEffect, transform2);
				}
			}
			this.RecalculateRollSpeed();
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity.y = 0f;
				base.characterMotor.velocity = this.forwardDirection * this.rollSpeed;
			}
			Vector3 b = base.characterMotor ? base.characterMotor.velocity : Vector3.zero;
			this.previousPosition = base.transform.position - b;

			TriggerReload();
		}

		private void RecalculateRollSpeed()
		{
			this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(this.initialSpeedCoefficient, this.finalSpeedCoefficient, base.fixedAge / this.duration);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.RecalculateRollSpeed();
			if (base.cameraTargetParams && (!scopeController || !scopeController.IsScoped()))
			{
				base.cameraTargetParams.fovOverride = Mathf.Lerp(CombatRoll2.dodgeFOV, 60f, base.fixedAge / this.duration);
			}
			Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
			if (base.characterMotor && base.characterDirection && normalized != Vector3.zero)
			{
				Vector3 vector = normalized * this.rollSpeed;
				float y = vector.y;
				vector.y = 0f;
				float d = Mathf.Max(Vector3.Dot(vector, this.forwardDirection), 0f);
				vector = this.forwardDirection * d;
				vector.y += Mathf.Max(y, 0f);
				base.characterMotor.velocity = vector;
			}
			this.previousPosition = base.transform.position;
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override void OnExit()
		{
			if (base.cameraTargetParams && (!scopeController || !scopeController.IsScoped()))
			{
				base.cameraTargetParams.fovOverride = -1f;
			}
			base.OnExit();
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.forwardDirection);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.forwardDirection = reader.ReadVector3();
		}

		private void TriggerReload()
        {
			if (base.skillLocator)
			{
				if (base.skillLocator && base.skillLocator.primary)
				{
					EntityStateMachine stateMachine = skillLocator.primary.stateMachine;
					if (stateMachine)
					{
						ReloadSnipe reloadSnipe = stateMachine.state as ReloadSnipe;
						if (reloadSnipe != null)
						{
							reloadSnipe.AutoReload();
						}
						else
						{
							Snipe snipe = stateMachine.state as Snipe;
							if (snipe != null)
							{
								snipe.AutoReload();
							}
							else
							{
								SniperClassic.ReloadController rc = base.gameObject.GetComponent<SniperClassic.ReloadController>();
								if (rc && rc.GetReloadQuality() != SniperClassic.ReloadController.ReloadQuality.Perfect)
								{
									rc.SetReloadQuality(SniperClassic.ReloadController.ReloadQuality.Perfect);
								}
							}
						}
					}
				}
			}
		}

		public float duration = 0.5f;
		public float initialSpeedCoefficient = 7.5f;
		public float finalSpeedCoefficient = 2.5f;

		public static string dodgeSoundString = EntityStates.Commando.DodgeState.dodgeSoundString;
		public static GameObject jetEffect = EntityStates.Commando.DodgeState.jetEffect;
		public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;

		private float rollSpeed;
		private Vector3 forwardDirection;
		private Animator animator;
		private Vector3 previousPosition;
		private SniperClassic.ScopeController scopeController;
	}
}