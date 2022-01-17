﻿using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.SniperClassicSkills
{
    class Snipe : BaseSnipeState
    {
        public override void SetStats()
        {
            internalDamage = damageCoefficient;
            internalRadius = radius;
            internalForce = force;
            internalBaseDuration = baseDuration;
            internalAttackSoundString = attackSoundString;
            internalChargedAttackSoundString = chargedAttackSoundString;
            internalRecoilAmplitude = recoilAmplitude;
            internalReloadDef = reloadDef;
            internalReloadBarLength = reloadBarLength;
        }

        public static float damageCoefficient = 4.4f;
        public static float radius = 0.4f;
        public static float force = 2000f;
        public static float baseDuration = 0.4f;
        public static float baseChargeDuration = 3f;
        public static float reloadBarLength = 0.6f;
        public static SkillDef reloadDef;
        public static string attackSoundString = "Play_SniperClassic_m1_shoot";
        public static string chargedAttackSoundString = "Play_SniperClassic_m2_shoot";
        public static float recoilAmplitude = 2.5f;

        /*protected float internalDamage;
        protected float internalRadius;
        protected float internalForce;
        protected float internalBaseDuration;
        protected string internalAttackSoundString;
        protected string internalChargedAttackSoundString;
        protected float internalRecoilAmplitude;
        protected float inernalBaseChargeDuration;
        protected SkillDef internalReloadDef;*/
    }
}