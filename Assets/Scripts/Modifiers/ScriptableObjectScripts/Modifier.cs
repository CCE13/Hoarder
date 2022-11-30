using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Modifier")]
[System.Serializable]
public class Modifier : ScriptableObject
{
    public enum ModifierType
    {
        damageOverTime,
        damage,
        explosion,
        critChance,
        critMutliplier,
        healthToRegen,
        healthIncrease,
        lifeSteal
    }
    public ModifierType modifierType;

    public Sprite spriteToShow;

    [Header("damageToAdd")]
    public int damageToAdd;
    public int critChanceToAdd;


    public int healthToRegenToAdd;
    public int healthAmountToGain;
    public int lifeStealToAdd;
    public string modifierDescription;

    /// <summary>
    /// Adds on crit multiplier to the _player
    /// </summary>
    public float critMultiplierToAdd;

    [Header("CheckDamageType Overtime If Needed")]
    public int DO_damageToDeal;
    public float DO_duration;
    public float DO_rateTime;
    public ParticleSystem DO_particleEffect;


    [Header("Explosion")]
    public float explosionRadius;
    public float explosionVelocity;
    public int explosiveDamage;

    #region editor
#if UNITY_EDITOR
    [CustomEditor(typeof(Modifier))]
    [System.Serializable]
    public class ModifierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Modifier modifier = (Modifier)target;
            EditorUtility.SetDirty(target);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings");
            EditorGUILayout.Space();

            modifier.modifierType = (ModifierType)EditorGUILayout.EnumPopup(modifier.modifierType);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Varaibles");

            if (modifier.modifierType == ModifierType.healthIncrease)
            {
                modifier.healthAmountToGain = EditorGUILayout.IntField("Health To Increase", modifier.healthAmountToGain);
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription, GUILayout.Height(100));

            }
            if (modifier.modifierType == ModifierType.lifeSteal)
            {
                modifier.lifeStealToAdd = EditorGUILayout.IntField("LifeSteal", modifier.lifeStealToAdd);
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription, GUILayout.Height(100));
            }
            if (modifier.modifierType == ModifierType.damage)
            {
                modifier.damageToAdd = EditorGUILayout.IntField("Damage To Add", modifier.damageToAdd);
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription,GUILayout.Height(100));
            }
            if (modifier.modifierType == ModifierType.critChance)
            {
                modifier.critChanceToAdd = EditorGUILayout.IntField("Crit Chance", modifier.critChanceToAdd);
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription, GUILayout.Height(100));
            }
            if (modifier.modifierType == ModifierType.critMutliplier)
            {
                modifier.critMultiplierToAdd = EditorGUILayout.Slider("Crit Multiplier", modifier.critMultiplierToAdd,0,1);
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription, GUILayout.Height(100));
            }
            if (modifier.modifierType == ModifierType.healthToRegen)
            {
                modifier.healthToRegenToAdd = EditorGUILayout.IntField("Health Regen /s to Add",modifier.healthToRegenToAdd);
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription, GUILayout.Height(100));
            }

            if (modifier.modifierType == ModifierType.damageOverTime)
            {
                modifier.DO_damageToDeal = EditorGUILayout.IntField("Damage To Deal", modifier.DO_damageToDeal);
                modifier.DO_duration = EditorGUILayout.FloatField("Duration", modifier.DO_duration);
                modifier.DO_rateTime = EditorGUILayout.FloatField("Rate", modifier.DO_rateTime);
                modifier.DO_particleEffect = EditorGUILayout.ObjectField("Particle Effect", modifier.DO_particleEffect, typeof(ParticleSystem), true) as ParticleSystem;
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription, GUILayout.Height(100));

            }
            if(modifier.modifierType == ModifierType.explosion)
            {
                modifier.explosionRadius = EditorGUILayout.FloatField("Explosion Radius", modifier.explosionRadius);
                modifier.explosionVelocity = EditorGUILayout.FloatField("Explosion Velocity",modifier.explosionVelocity);
                modifier.explosiveDamage = EditorGUILayout.IntField("Explosion Damage", modifier.explosiveDamage);
                modifier.spriteToShow = EditorGUILayout.ObjectField("Sprite to Show", modifier.spriteToShow, typeof(Sprite), true) as Sprite;
                EditorGUILayout.LabelField("Description");
                modifier.modifierDescription = EditorGUILayout.TextArea(modifier.modifierDescription, GUILayout.Height(100));
                modifier.DO_particleEffect = EditorGUILayout.ObjectField("Particle Effect", modifier.DO_particleEffect, typeof(ParticleSystem), true) as ParticleSystem;
            }
        }

    }
#endif
    #endregion
}
