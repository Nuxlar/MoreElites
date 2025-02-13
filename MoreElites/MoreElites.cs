using BepInEx;
using BepInEx.Configuration;
using System.Reflection;
using System.Collections.Generic;
using R2API;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using System.IO;
using Newtonsoft.Json;

namespace MoreElites
{
  [BepInPlugin("com.Nuxlar.MoreElites", "MoreElites", "2.0.0")]
  [BepInDependency(ItemAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency(LanguageAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency(EliteAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency(RecalculateStatsAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency(PrefabAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
  [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]

  public class MoreElites : BaseUnityPlugin
  {

    private static Material malachiteOverlayMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/ElitePoison/matElitePoisonOverlay.mat").WaitForCompletion());
    private ItemDisplayRuleSet[] idrsArray = new ItemDisplayRuleSet[] {
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Bandit2/idrsBandit2.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Beetle/idrsBeetle.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Beetle/idrsBeetleGuard.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Beetle/idrsBeetleQueen.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Bell/idrsBell.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Bison/idrsBison.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Captain/idrsCaptain.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/ClayBoss/idrsClayBoss.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/ClayBruiser/idrsClayBruiser.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Commando/idrsCommando.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Croco/idrsCroco.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Drones/idrsEquipmentDrone.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Engi/idrsEngi.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Engi/idrsEngiTurret.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Engi/idrsEngiWalkerTurret.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Golem/idrsGolem.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Grandparent/idrsGrandparent.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Gravekeeper/idrsGravekeeper.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/GreaterWisp/idrsGreaterWisp.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/HermitCrab/idrsHermitCrab.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Huntress/idrsHuntress.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Imp/idrsImp.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/ImpBoss/idrsImpBoss.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Jellyfish/idrsJellyfish.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Lemurian/idrsLemurian.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/LemurianBruiser/idrsLemurianBruiser.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Loader/idrsLoader.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Mage/idrsMage.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/MagmaWorm/idrsMagmaWorm.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Merc/idrsMerc.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/MiniMushroom/idrsMiniMushroom.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Nullifier/idrsNullifier.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/RoboBallBoss/idrsRoboBallBoss.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/RoboBallBoss/idrsRoboBallMini.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Scav/idrsScav.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Titan/idrsTitan.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Toolbot/idrsToolbot.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Treebot/idrsTreebot.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Vagrant/idrsVagrant.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Vulture/idrsVulture.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Wisp/idrsWisp.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Squid/idrsSquidTurret.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/AcidLarva/idrsAcidLarva.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/ClayGrenadier/idrsClayGrenadier.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/FlyingVermin/idrsFlyingVermin.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/Gup/idrsGeep.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/Gup/idrsGip.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/Gup/idrsGup.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/MajorAndMinorConstruct/idrsMajorConstruct.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/MajorAndMinorConstruct/idrsMinorConstruct.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/Railgunner/idrsRailGunner.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/Vermin/idrsVermin.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/VoidBarnacle/idrsVoidBarnacle.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/VoidJailer/idrsVoidJailer.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/VoidMegaCrab/idrsVoidMegaCrab.asset").WaitForCompletion(),
      Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/VoidSurvivor/idrsVoidSurvivor.asset").WaitForCompletion()
    };


    public void Awake()
    {
      // Delete any unused config entries
      WipeConfig(Config);

      //This section automatically scans the project for all elites
      IEnumerable<Type> EliteTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EliteBase)));
      foreach (Type eliteType in EliteTypes)
      {
        EliteBase elite = (EliteBase)System.Activator.CreateInstance(eliteType);
        elite.Init(Config);
      }

      // Malachite Overlay Material
      // _MainTex texCloudDifferenceBW2
      // _Cloud1Tex
      // _Cloud2Tex
      // _RemapTex set this to the ramp
      // Malachite Ramp Palette
      // new Color(0.808f, 0.729f, 0.420f, 0.804f)
      // new Color(0f, 0f, 0f, 0.804f)
      // new Color(0.808f, 0.729f, 0.420f, 1f)
      // new Color(0f, 0f, 0f, 0f)
      // RoR2Application.onLoad += CreateIDRS;
    }

    // First 2 colors will be the most prominent, use darker/bolder colors for these 2, lighter/pastel colors will look very bright.
    // The other 3 add depth and texture on some enemies, that"s why 5 are required, so you don"t get flat colors.
    public static Texture2D CreateGradientTexture(Color32[] colors, int width, int height)
    {
      Texture2D texture = new Texture2D(width, height);
      texture.wrapMode = TextureWrapMode.Clamp;

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          // Calculate the horizontal position as a value between 0 and 1
          float t = (float)x / (width - 1);

          // Determine which colors to interpolate between
          float scaledT = t * (colors.Length - 1);
          int colorIndex = Mathf.FloorToInt(scaledT);
          float lerpFactor = scaledT - colorIndex;

          // Ensure the last color is not out of bounds
          if (colorIndex >= colors.Length - 1)
          {
            colorIndex = colors.Length - 2;
            lerpFactor = 1.0f;
          }

          // Interpolate between the two colors
          Color32 color = LerpColor32(colors[colorIndex], colors[colorIndex + 1], lerpFactor);

          // Set the pixel color
          texture.SetPixel(x, y, color);
        }
      }

      // Apply changes to the texture
      texture.Apply();
      /*
      string fileName = "SavedTexture.png";
      SaveTextureToFile(texture, fileName);
      */
      malachiteOverlayMat.SetTexture("_RemapTex", texture);

      return texture;
    }

    /*
    static void SaveTextureToFile(Texture2D texture, string fileName)
    {
      byte[] bytes = texture.EncodeToPNG();
      string path = Path.Combine(Application.persistentDataPath, fileName);
      File.WriteAllBytes(path, bytes);
      Debug.Log("Texture saved to: " + path);
    }
    */

    public static Color32 LerpColor32(Color32 colorA, Color32 colorB, float t)
    {
      byte r = (byte)Mathf.Lerp(colorA.r, colorB.r, t);
      byte g = (byte)Mathf.Lerp(colorA.g, colorB.g, t);
      byte b = (byte)Mathf.Lerp(colorA.b, colorB.b, t);
      byte a = (byte)Mathf.Lerp(colorA.a, colorB.a, t);
      return new Color32(r, g, b, a);
    }

    private void WipeConfig(ConfigFile configFile)
    {
      PropertyInfo orphanedEntriesProp = typeof(ConfigFile).GetProperty("OrphanedEntries", BindingFlags.Instance | BindingFlags.NonPublic);
      Dictionary<ConfigDefinition, string> orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(configFile);
      orphanedEntries.Clear();

      configFile.Save();
    }

    public static ItemDisplayRuleDict CreateIDRS(GameObject crownPrefab, int scale = 1)
    {
      ItemDisplayRuleDict itemDisplayRules = new ItemDisplayRuleDict(Array.Empty<ItemDisplayRule>());

      itemDisplayRules.Add("mdlBandit2", new ItemDisplayRule[1]
      {
          new ItemDisplayRule()
          {
            ruleType = ItemDisplayRuleType.ParentedPrefab,
            followerPrefab = crownPrefab,
            childName = "Head",
            localPos = new Vector3(0f, 0.188f, -0.027f),
            localAngles = new Vector3(291.009949f, 179.999924f, 180.000061f),
            localScale = new Vector3(0.0222639f, 0.0222639f, 0.0222639f) * scale
          }
      });
      itemDisplayRules.Add("mdlBeetle", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(0f, 0f, 0.47f),
          localAngles = new Vector3(-0.00000171049135f, (float)Double.Parse("9.308214E-08", System.Globalization.NumberStyles.Float), 182.934158f),
          localScale = new Vector3(0.1f, 0.1f, 0.1f) * scale
        }
      });
      itemDisplayRules.Add("mdlBeetleGuard", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(-0.04f, 0.08f, 1.62f),
          localAngles = new Vector3(7.87473249f, 354.209442f, 180.862762f),
          localScale = new Vector3(0.2f, 0.2f, 0.2f) * scale
        }
      });
      itemDisplayRules.Add("mdlBeetleQueen", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(0f, 3.78f, 0.5f),
          localAngles = new Vector3(270f, 180.02298f, 0f),
          localScale = new Vector3(0.3f, 0.3f, 0.3f) * scale
        }
      });
      itemDisplayRules.Add("mdlBell", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Chain",
          localPos = new Vector3(-0.0122341728f, -0.646894932f, -0.0115990974f),
          localAngles = new Vector3(90f, 58.7155838f, 0f),
          localScale = new Vector3(0.199999988f, 0.199999988f, 0.199999988f) * scale
        }
      });
      itemDisplayRules.Add("mdlBison", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(0f, 0.173f, 0.742f),
          localAngles = new Vector3(354.31543f, 359.834076f, 180.0913f),
          localScale = new Vector3(0.07f, 0.07f, 0.07f) * scale
        }
      });
      itemDisplayRules.Add("mdlCaptain", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(0f, 0.26f, -0.037f),
          localAngles = new Vector3(291.030273f, 179.999985f, 180.000031f),
          localScale = new Vector3(0.03f, 0.03f, 0.03f) * scale
        }
      });
      itemDisplayRules.Add("mdlClayBoss", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "PotLidTop",
          localPos = new Vector3(0f, 0.86f, 1.06f),
          localAngles = new Vector3(270f, -0.0000214576812f, 0f),
          localScale = new Vector3(0.2f, 0.2f, 0.2f) * scale
        }
      });
      itemDisplayRules.Add("mdlClayBruiser", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(0.014f, 0.571f, 0.136f),
          localAngles = new Vector3(270f, 15.0000448f, 0f),
          localScale = new Vector3(0.07f, 0.07f, 0.07f) * scale
        }
      }); itemDisplayRules.Add("mdlCommando", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(0f, 0.403f, 0.031f),
          localAngles = new Vector3(271.569855f, 0.000847860647f, -0.000860085362f),
          localScale = new Vector3(0.03f, 0.03f, 0.03f) * scale
        }
      }); itemDisplayRules.Add("mdlCroco", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(0.03f, 0.86f, 1.4f),
          localAngles = new Vector3(23.4304237f, 2.97252321f, 182.965439f),
          localScale = new Vector3(0.2f, 0.2f, 0.2f) * scale
        }
      }); itemDisplayRules.Add("mdlEquipmentDrone", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "GunAxis",
          localPos = new Vector3(0.01f, -0.285f, -0.066f),
          localAngles = new Vector3(83.59243f, 0.236750633f, -0.00007865497f),
          localScale = new Vector3(0.09605703f, 0.09605704f, 0.09605707f) * scale
        }
      }); itemDisplayRules.Add("mdlEngi", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Chest",
          localPos = new Vector3(0f, 0.767f, 0.041f),
          localAngles = new Vector3(271.569855f, 0.000847860647f, -0.000860085362f),
          localScale = new Vector3(0.03f, 0.03f, 0.03f) * scale
        }
      }); itemDisplayRules.Add("mdlEngiTurret", new ItemDisplayRule[1]
      {
        new ItemDisplayRule()
        {
          ruleType = ItemDisplayRuleType.ParentedPrefab,
          followerPrefab = crownPrefab,
          childName = "Head",
          localPos = new Vector3(-0.047f, 1.79f, -0.163f),
          localAngles = new Vector3(274.6648f, -0.0000190572773f, 0.0000110999217f),
          localScale = new Vector3(0.126f, 0.126f, 0.126f) * scale
        }
      });

      return itemDisplayRules;
    }
  }
}

