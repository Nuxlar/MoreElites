using R2API;
using RoR2;
using RoR2.Navigation;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;

namespace MoreElites.Elites
{
  public class Echo : EliteBase<Echo>
  {
    public ConfigEntry<bool> enable;
    public override string EliteName => "Echo";
    public override string EliteAffixDesc => "Summon 2 copies of yourself.";
    public override Color EliteColor => Color.black;
    public override float EliteHealthMult => 18f;
    public override float EliteDamageMult => 6f;
    public override float EliteAffixDropChance => 0.00025f;
    public override Material EliteAffixMaterial => Addressables.LoadAssetAsync<Material>("RoR2/InDev/matEcho.mat").WaitForCompletion();
    public override Texture2D EliteRamp => MoreElites.CreateGradientTexture(new Color32[5] {
            new Color32(23, 22, 20, 255),
            new Color32(117, 64, 67, 255),
            new Color32(154, 136, 115, 255),
            new Color32(55, 66, 61, 255),
            new Color32(58, 38, 24, 255),
        }, 256, 8);
    public override Sprite EliteIcon => Addressables.LoadAssetAsync<Sprite>("RoR2/Base/EliteIce/texBuffAffixWhite.tif").WaitForCompletion();
    public override Sprite EliteAspectIcon => Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/EliteEarth/texAffixEarthIcon.png").WaitForCompletion();
    //  public override GameObject EliteCrown => PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteHaunted/DisplayEliteStealthCrown.prefab").WaitForCompletion(), "EchoCrown");
    public override GameObject EliteCrown => PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BarrierOnKill/DisplayBrooch.prefab").WaitForCompletion(), "EchoCrown");

    public static ItemDef summonedEcho = Addressables.LoadAssetAsync<ItemDef>("RoR2/InDev/SummonedEcho.asset").WaitForCompletion();
    private static Material overlayMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidoutro/matVoidRaidCrabEyeOverlay1BLUE.mat").WaitForCompletion();
    private static GameObject crownHolder = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BarrierOnKill/DisplayBrooch.prefab").WaitForCompletion(), "EchoCrownHolder");
    // Malachite Ramp Palette
    // new Color(0.808f, 0.729f, 0.420f, 0.804f)
    // new Color(0f, 0f, 0f, 0.804f)
    // new Color(0.808f, 0.729f, 0.420f, 1f)
    // new Color(0f, 0f, 0f, 0f)
    /*
colorPalette = new Color32[] {
            new Color32(74, 64, 99, 255),
            new Color32(191, 172, 200, 255),
            new Color32(200, 198, 215, 255),
            new Color32(120, 63, 142, 255),
            new Color32(79, 18, 113, 255)
        };
    */
    private Texture2D perfectedRamp = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampEliteLunar.png").WaitForCompletion();
    public override void Init(ConfigFile config)
    {
      EliteCrown.AddComponent<NetworkIdentity>();
      crownHolder.AddComponent<NetworkIdentity>();
      ContentAddition.AddItemDef(summonedEcho);
      crownHolder.transform.SetParent(EliteCrown.transform);
      crownHolder.transform.Rotate(new Vector3(0f, 180f, 0f));
      CreateConfig(config);
      CreateLang();
      CreateBuff();
      CreateEquip();
      CreateElite();
      AddContent();
      AddRamp();
      AddCrown();
      Hooks();
    }

    public override void CreateConfig(ConfigFile config)
    {
      enable = config.Bind<bool>("Elites", EliteName, true, "Should this Elite be enabled.");
    }

    public override void Hooks()
    {
      RecalculateStatsAPI.GetStatCoefficients += ReduceSummonHP;
      On.RoR2.CharacterBody.OnBuffFirstStackGained += AddEchoBehavior;
      On.RoR2.CharacterBody.OnBuffFinalStackLost += RemoveEchoBehavior;
      // On.RoR2.CharacterModel.UpdateOverlays += AddOverlay;
      On.RoR2.CombatDirector.Init += AddEliteToDirector;
    }

    private void AddEliteToDirector(On.RoR2.CombatDirector.orig_Init orig)
    {
      orig();
      if (EliteAPI.VanillaEliteTiers.Length > 3)
      {
        CombatDirector.EliteTierDef targetTier = EliteAPI.VanillaEliteTiers[3];
        List<EliteDef> elites = targetTier.eliteTypes.ToList();
        elites.Add(EliteDefinition);
        targetTier.eliteTypes = elites.ToArray();
      }
    }

    private void ReduceSummonHP(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
      if (sender && sender.inventory)
      {
        int stack = sender.inventory.GetItemCount(summonedEcho);
        if (stack > 0)
          args.baseCurseAdd += Mathf.Pow(1 / 0.1f, stack) - 1;
      }
    }

    private void AddOverlay(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
    {
      orig(self);
      if (self.body && self.body.inventory)
      {
        if (self.activeOverlayCount >= CharacterModel.maxOverlays) return;
        if (self.body.HasBuff(EliteBuff))
        {
          Material[] array = self.currentOverlays;
          int num = self.activeOverlayCount;
          self.activeOverlayCount = num + 1;
          array[num] = overlayMat;
        }
        if (self.body.inventory.GetItemCount(summonedEcho) > 0)
        {
          Material[] array = self.currentOverlays;
          int num = self.activeOverlayCount;
          self.activeOverlayCount = num + 1;
          array[num] = EliteAffixMaterial;
        }
      }
    }

    private void AddEchoBehavior(
      On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig,
      CharacterBody self,
      BuffDef buffDef
      )
    {
      orig(self, buffDef);
      if (buffDef == EliteBuff)
        self.AddItemBehavior<CustomAffixEchoBehavior>(1);
    }

    private void RemoveEchoBehavior(
  On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig,
  CharacterBody self, BuffDef buffDef)
    {
      orig(self, buffDef);
      if (buffDef == EliteBuff)
        self.AddItemBehavior<CustomAffixEchoBehavior>(0);
    }

    public class CustomAffixEchoBehavior : CharacterBody.ItemBehavior
    {
      private DeployableMinionSpawner echoSpawner1;
      private DeployableMinionSpawner echoSpawner2;
      private CharacterSpawnCard spawnCard;
      private List<CharacterMaster> spawnedEchoes = new List<CharacterMaster>();

      private void FixedUpdate() => this.spawnCard.nodeGraphType = this.body.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground;

      private void Awake()
      {
        this.enabled = false;
        // Play_voidRaid_breakLeg Play_voidRaid_fall_pt2  
        // Play_affix_void_spawn
        Util.PlaySound("Play_voidRaid_fog_explode", this.gameObject);
      }

      private void OnEnable()
      {
        MasterCatalog.MasterIndex masterIndexForBody = MasterCatalog.FindAiMasterIndexForBody(this.body.bodyIndex);
        this.spawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
        this.spawnCard.prefab = MasterCatalog.GetMasterPrefab(masterIndexForBody);
        this.spawnCard.inventoryToCopy = this.body.inventory;
        this.spawnCard.equipmentToGrant = new EquipmentDef[1];
        this.spawnCard.itemsToGrant = new ItemCountPair[1]
        {
          new ItemCountPair()
          {
            itemDef = summonedEcho,
            count = 1
          }
        };
        this.CreateSpawners();
      }

      private void OnDisable()
      {
        UnityEngine.Object.Destroy(this.spawnCard);
        this.spawnCard = (CharacterSpawnCard)null;
        for (int index = this.spawnedEchoes.Count - 1; index >= 0; --index)
        {
          if ((bool)this.spawnedEchoes[index])
            this.spawnedEchoes[index].TrueKill();
        }
        this.DestroySpawners();
      }

      private void CreateSpawners()
      {
        Xoroshiro128Plus rng = new Xoroshiro128Plus(Run.instance.seed ^ (ulong)this.GetInstanceID());
        CreateSpawner(ref this.echoSpawner1, DeployableSlot.RoboBallRedBuddy, (SpawnCard)this.spawnCard);
        CreateSpawner(ref this.echoSpawner2, DeployableSlot.RoboBallGreenBuddy, (SpawnCard)this.spawnCard);

        void CreateSpawner(
          ref DeployableMinionSpawner buddySpawner,
          DeployableSlot deployableSlot,
          SpawnCard spawnCard)
        {
          buddySpawner = new DeployableMinionSpawner(this.body.master, deployableSlot, rng)
          {
            respawnInterval = 30f,
            spawnCard = spawnCard
          };
          buddySpawner.onMinionSpawnedServer += new Action<SpawnCard.SpawnResult>(this.OnMinionSpawnedServer);
        }
      }

      private void DestroySpawners()
      {
        this.echoSpawner1?.Dispose();
        this.echoSpawner1 = null;
        this.echoSpawner2?.Dispose();
        this.echoSpawner2 = null;
      }

      private void OnMinionSpawnedServer(SpawnCard.SpawnResult spawnResult)
      {
        GameObject spawnedInstance = spawnResult.spawnedInstance;
        if (!(bool)spawnedInstance)
          return;
        CharacterMaster spawnedMaster = spawnedInstance.GetComponent<CharacterMaster>();
        if (!(bool)spawnedMaster)
          return;
        this.spawnedEchoes.Add(spawnedMaster);
        OnDestroyCallback.AddCallback(spawnedMaster.gameObject, _ => this.spawnedEchoes.Remove(spawnedMaster));
      }
    }
  }
}
