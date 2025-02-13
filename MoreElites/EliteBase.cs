using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoreElites
{
    public abstract class EliteBase<T> : EliteBase where T : EliteBase<T>
    {
        public static T instance { get; private set; }

        public EliteBase()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting EliteBase was instantiated twice");
            instance = this as T;
        }
    }

    public abstract class EliteBase
    {
        public abstract string EliteName { get; }
        public abstract string EliteAffixDesc { get; }
        public abstract Color EliteColor { get; }
        public abstract float EliteHealthMult { get; }
        public abstract float EliteDamageMult { get; }
        public abstract float EliteAffixDropChance { get; }
        public abstract Material EliteAffixMaterial { get; }
        public abstract Texture2D EliteRamp { get; }
        public abstract Sprite EliteIcon { get; }
        public abstract Sprite EliteAspectIcon { get; }
        public abstract GameObject EliteCrown { get; }

        public EquipmentDef EliteEquip;
        public BuffDef EliteBuff;
        public EliteDef EliteDefinition;

        public abstract void Init(ConfigFile config);

        public virtual void CreateConfig(ConfigFile config) { }

        protected virtual void CreateLang()
        {
            LanguageAPI.Add("ELITE_NUX_" + EliteName.ToUpper() + "_NAME", EliteName);
            LanguageAPI.Add("ELITE_NUX_MODIFIER_" + EliteName.ToUpper(), EliteName + " {0}");
            LanguageAPI.Add("ELITE_NUX_EQUIPMENT_AFFIX_" + EliteName.ToUpper() + "_NAME", $"{EliteName} Aspect");
            LanguageAPI.Add("ELITE_NUX_AFFIX_" + EliteName.ToUpper() + "_DESCRIPTION", $"{EliteName} Aspect");
        }

        protected void AddRamp()
        {
            R2API.EliteRamp.AddRamp(EliteDefinition, EliteRamp);
        }

        protected void AddContent()
        {
            ContentAddition.AddEliteDef(EliteDefinition);
            ContentAddition.AddBuffDef(EliteBuff);
            ContentAddition.AddEquipmentDef(EliteEquip);
        }

        protected void AddCrown()
        {
            ItemDisplays itemDisplays = new ItemDisplays();
            //ItemAPI.Add(new CustomEquipment(EliteEquip, MoreElites.CreateIDRS(EliteCrown, 10)));
            //ItemAPI.Add(new CustomEquipment(EliteEquip, itemDisplays.CreateItemDisplayRules(EliteCrown, EliteAffixMaterial)));
        }

        protected void CreateBuff()
        {
            EliteBuff = ScriptableObject.CreateInstance<BuffDef>();
            EliteBuff.name = "Elite" + EliteName + "Buff";
            EliteBuff.canStack = false;
            EliteBuff.isCooldown = false;
            EliteBuff.isDebuff = false;
            EliteBuff.buffColor = EliteColor;
            EliteBuff.iconSprite = EliteIcon;
            (EliteBuff as UnityEngine.Object).name = EliteBuff.name;
        }

        protected void CreateEquip()
        {
            EliteEquip = ScriptableObject.CreateInstance<EquipmentDef>();
            EliteEquip.appearsInMultiPlayer = true;
            EliteEquip.appearsInSinglePlayer = true;
            EliteEquip.canBeRandomlyTriggered = false;
            EliteEquip.canDrop = false;
            EliteEquip.colorIndex = ColorCatalog.ColorIndex.Equipment;
            EliteEquip.cooldown = 0.0f;
            EliteEquip.isLunar = false;
            EliteEquip.isBoss = false;
            EliteEquip.passiveBuffDef = EliteBuff;
            EliteEquip.dropOnDeathChance = EliteAffixDropChance;
            EliteEquip.enigmaCompatible = false;
            EliteEquip.pickupIconSprite = EliteIcon;
            EliteEquip.pickupModelPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/PickupEliteFire.prefab").WaitForCompletion(), "PickupAffixEmpowering", false);
            foreach (Renderer componentsInChild in EliteEquip.pickupModelPrefab.GetComponentsInChildren<Renderer>())
                componentsInChild.material = EliteAffixMaterial;
            EliteEquip.nameToken = "ELITE_NUX_EQUIPMENT_AFFIX_" + EliteName.ToUpper() + "_NAME";
            EliteEquip.name = "Affix" + EliteName;

            EliteEquip.pickupToken = "Aspect of " + EliteName;
            EliteEquip.descriptionToken = EliteAffixDesc;
            EliteEquip.loreToken = "";
        }

        protected void CreateElite()
        {
            EliteDefinition = ScriptableObject.CreateInstance<EliteDef>();
            EliteDefinition.color = EliteColor;
            EliteDefinition.eliteEquipmentDef = EliteEquip;
            EliteDefinition.modifierToken = "ELITE_NUX_MODIFIER_" + EliteName.ToUpper();
            EliteDefinition.name = EliteName;
            EliteDefinition.healthBoostCoefficient = EliteHealthMult;
            EliteDefinition.damageBoostCoefficient = EliteDamageMult;
            EliteBuff.eliteDef = EliteDefinition;
            (EliteDefinition as ScriptableObject).name = EliteName;
        }

        public virtual void Hooks() { }
    }
}