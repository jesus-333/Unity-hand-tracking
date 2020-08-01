# End-user documentation

## Installing and using XR Plug-in Management

For instructions on how to install the XR Plug-in Manager, see the [XR Plug-in Framework](https://docs.unity3d.com/2020.1/Documentation/Manual/XRPluginArchitecture.html) page in the Unity Manual.

## Automatic XR loading

By default, XR Plug-in Management intializes automatically and starts your XR environment when the application loads. At runtime, this happens immediately before the first Scene loads. In Play mode, this happens immediately after the first Scene loads, but before `Start` is called on your GameObjects. In both scenarios, XR should be set up before calling the MonoBehaviour [Start](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html) method, so you should be able to query the state of XR in the `Start` method of your GameObjects.

If you want to start XR on a per-Scene basis (for example, to start in 2D and transition into VR), follow these steps:

1. Access the **Project Settings** window (menu: **Edit** &gt; **Project Settings**).
2. Select the **XR Plug-in Management** tab on the left.
3. Disable the **Initialize on start** option for each platform you support.
4. At runtime, call the following methods on `XRGeneralSettings.Instance.Manager` to add/create, remove, and reorder the Loaders from your scripts:

|Method|Description|
|---|---|
|`InitializeLoader(Async)`|Sets up the XR environment to run manually.|
|`StartSubsystems`|Starts XR and puts your application into XR mode.|
|`StopSubsystems`|Stops XR and takes your application out of XR mode. You can call `StartSubsystems` again to go back into XR mode.|
|`DeinitializeLoader`|Shuts down XR and removes it entirely. You must call `InitializeLoader(Async)` before you can run XR again.|

To handle pause state changes in the Editor, subscribe to the [`EditorApplication.pauseStateChanged`](https://docs.unity3d.com/ScriptReference/EditorApplication-pauseStateChanged.html) API, then stop and start the subsystems according to the new pause state that the `pauseStateChange` delegate method returns.

## Customizing build and runtime settings

Any package that needs build or runtime settings should provide a settings data type for use. This data type appears in the **Project Settings** window, underneath a top level **XR** node.

You can use scripts to configure the settings for a specific plug-in, or change the active and inactive plug-ins per build target.

### Example: Accessing custom settings

**Note**: This doesn't install any plug-ins for you. Make sure your plug-ins are installed and available before you try this script.

```
    var metadata = XRPackageMetadataStore.GetMetadataForPackage(my_pacakge_id);
    assets = AssetDatabase.FindAssets($"t:{metadata.settingsType}");
    var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

    // Settings access is type specific. You will need information from your plug-in documentation
    // to know how to get at specific instances and properties.

    // You must know the type of the settings you are accessing.
    var directInstance  = AssetDatabase.LoadAssetAtPath(assetPath, typeof(full.typename.for.pluginsettings));
    
    // You must know the access method for getting build target specific settings data.
    var buildTargetSettings = directInstance.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);

    // Do something with settings...

    // Mark instance dirty and save any changes.
    EditorUtility.SetDirty(directInstance);
    AssetDatabase.SaveAssets();
```

### Example: Configuring plug-ins per build target

**Note**: This doesn't install any plug-ins for you. Make sure your plug-ins are installed and available before you try this script.

Adding a plug-in to the set of assigned plug-ins for a build target:

```
    var buildTargetSettings = XRGeneralSettingsPerBuildTarget.SettingsForBuildTarget(BuildTarget.Standalone);
    var pluginsSettings = buildTargetSettings.AssignedSettings;
    var didAssign = XRPackageMetadataStore.AssignLoader(pluginsSettings, "full.typename.for.pluginloader", BuildTargetGroup.Standalone);

    if (!didAssign)
    {
        // Report error or do something here.
        ...
    }
```

Removing a plug-in from the set of assigned plug-ins for a build target:

```
    var buildTargetSettings = XRGeneralSettingsPerBuildTarget.SettingsForBuildTarget(BuildTarget.Standalone);
    var pluginsSettings = buildTargetSettings.AssignedSettings;
    var didRemove = XRPackageMetadataStore.RemoveLoader(pluginsSettings, "full.typename.for.pluginloader", BuildTargetGroup.Standalone);

    if (!didRemove)
    {
        // Report error or do something here.
        ...
    }
```

### Example: Reordering the loader list

By default, the XR Plug-in Management UI displays loaders in strict alphabetical order, based on their names. In most scenarios, you don't need to change this order. However, if you need the loaders to load in a different order, you can reorder the loaders from script like this:

```
    var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTarget.Standalone);
    var settingManager = generalSettings.Manager;
    var loaders = settingsManager.loaders;

    // Add/Remove/Reorder loader list;

    settingsManager.loaders = loaders;
```

You would most likely place this script in a custom build script, but that isn't required. Regardless of the script's location location, you should do this as a setup step before you start a build. This is because the first thing that XR Plug-in Manager does at build time is to serialize the loader list to the build target.

**Note**: Any operation on the XR Plug-in Manager UI will reset the ordering to the original alphabetical ordering.

## Installing the XR Plug-in Management package

Most XR Plug-in provider packages typically include XR Plug-in Management, so you shouldn't need to install it. If you do need to install it, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html).

## Installing the Legacy Input Helpers package

Unity requires the Legacy Input Helpers package to operate XR devices correctly. To check if the Legacy Input Helpers package is installed, open the **Project Settings** window and navigate to **XR Plug-in Management** &gt; **Input Helpers**. If Unity can't locate the package, click the **Install Legacy Helpers Package** button to install it.
