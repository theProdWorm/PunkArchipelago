using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PunkArchipelago;

public static class AssetBundleHelper
{
    private static readonly Dictionary<string, AssetBundle> ASSET_BUNDLES = new();
    
    public static AssetBundle GetAssetBundle(string bundleName)
    {
        if (ASSET_BUNDLES.TryGetValue(bundleName, out AssetBundle bundle))
            return bundle;
                
        bundle = AssetBundle.LoadFromFile(Path.Combine(Directory.GetCurrentDirectory(), 
            "BepInEx", "plugins", "PUNK Archipelago", bundleName));
        
        if (!bundle)
        {
            ArchipelagoPlugin.Logger.LogError($"Failed to load asset bundle {bundleName}");
            return null;
        }
        
        ASSET_BUNDLES.Add(bundleName, bundle);
        
        return bundle;
    }
}