using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AddVuforiaEnginePackage
{
    const string VUFORIA_VERSION = "9.0.12";
    const string PACKAGE_KEY = "\"com.ptc.vuforia.engine\"";

    static readonly string sPackagesPath = Path.Combine(Application.dataPath, "..", "Packages");
    static readonly string sManifestJsonPath = Path.Combine(sPackagesPath, "manifest.json");


    static readonly ScopedRegistry sVuforiaRegistry = new ScopedRegistry()
    {
        name = "Vuforia",
        url = "https://registry.packages.developer.vuforia.com/",
        scopes = new[] { "com.ptc.vuforia" }
    };


    static AddVuforiaEnginePackage()
    {
        if (Application.isBatchMode)
            return;

        var manifest = Manifest.JsonDeserialize(sManifestJsonPath);
        var registries = manifest.ScopedRegistries.ToList();
        if (registries.Any(r => r == sVuforiaRegistry))
            return;

        if (EditorUtility.DisplayDialog("Add Vuforia Engine Package",
            "Would you like to update your project to always be able to find the latest version of the Vuforia Engine in the package manager window?\n" +
            $"If a Vuforia Engine package is already present in your project it will be upgraded to version {VUFORIA_VERSION}", "Update", "Cancel"))
        {
            UpdateManifest(manifest);
        }
    }

    static void UpdateManifest(Manifest manifest)
    {
        AddRegistry(manifest, sVuforiaRegistry);
        SetVuforiaVersion(manifest);

        manifest.JsonSerialize(sManifestJsonPath);
        
        AssetDatabase.Refresh();
    }

    static void SetVuforiaVersion(Manifest manifest)
    {
        var dependencies = manifest.Dependencies.Split(',').ToList();

        var versionSet = false;
        for(var i = 0; i < dependencies.Count; i++)
        {
            if(!dependencies[i].Contains(PACKAGE_KEY))
                continue;

            var kvp = dependencies[i].Split(':');

            kvp[1] = $"\"{VUFORIA_VERSION}\""; //version string of the package

            dependencies[i] = string.Join(":", kvp);

            versionSet = true;
        }

        if (!versionSet)
            dependencies.Insert(0, $"\n    {PACKAGE_KEY}: \"{VUFORIA_VERSION}\"");
        

        manifest.Dependencies = string.Join(",", dependencies);
    }

    static void AddRegistry(Manifest manifest, ScopedRegistry scopedRegistry)
    {
        var registries = manifest.ScopedRegistries.ToList();
        if (registries.Any(r => r == scopedRegistry))
            return;

        registries.Add(scopedRegistry);

        manifest.ScopedRegistries = registries.ToArray();
    }

    class Manifest
    {
        const int INDEX_NOT_FOUND = -1;
        const string DEPENDENCIES_KEY = "\"dependencies\"";

        public ScopedRegistry[] ScopedRegistries;
        public string Dependencies;

        public void JsonSerialize(string path)
        {
            var jsonString = JsonUtility.ToJson(
                new UnitySerializableManifest {scopedRegistries = ScopedRegistries, dependencies = new DependencyPlaceholder()},
                true);

            var startIndex = GetDependenciesStart(jsonString);
            var endIndex = GetDependenciesEnd(jsonString, startIndex);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(jsonString.Substring(0, startIndex));
            stringBuilder.Append(Dependencies);
            stringBuilder.Append(jsonString.Substring(endIndex, jsonString.Length - endIndex));

            File.WriteAllText(path, stringBuilder.ToString());
        }

        public static Manifest JsonDeserialize(string path)
        {
            var jsonString = File.ReadAllText(path);

            var registries = JsonUtility.FromJson<UnitySerializableManifest>(jsonString).scopedRegistries ?? new ScopedRegistry[0];
            var dependencies = DeserializeDependencies(jsonString);

            return new Manifest { ScopedRegistries = registries, Dependencies = dependencies };
        }

        static string DeserializeDependencies(string json)
        {
            var startIndex = GetDependenciesStart(json);
            var endIndex = GetDependenciesEnd(json, startIndex);

            if (startIndex == INDEX_NOT_FOUND || endIndex == INDEX_NOT_FOUND)
                return null;

            var dependencies = json.Substring(startIndex, endIndex - startIndex);
            return dependencies;
        }

        static int GetDependenciesStart(string json)
        {
            var dependenciesIndex = json.IndexOf(DEPENDENCIES_KEY, StringComparison.InvariantCulture);
            if (dependenciesIndex == INDEX_NOT_FOUND)
                return INDEX_NOT_FOUND;

            var dependenciesStartIndex = json.IndexOf('{', dependenciesIndex + DEPENDENCIES_KEY.Length);

            if (dependenciesStartIndex == INDEX_NOT_FOUND)
                return INDEX_NOT_FOUND;

            dependenciesStartIndex++; //add length of '{' to starting point

            return dependenciesStartIndex;
        }

        static int GetDependenciesEnd(string jsonString, int dependenciesStartIndex)
        {
            return jsonString.IndexOf('}', dependenciesStartIndex);
        }
    }

    class UnitySerializableManifest
    {
        public ScopedRegistry[] scopedRegistries;
        public DependencyPlaceholder dependencies;
    }

    [Serializable]
    struct ScopedRegistry
    {
        public string name;
        public string url;
        public string[] scopes;

        public override bool Equals(object obj)
        {
            if (!(obj is ScopedRegistry))
                return false;

            var other = (ScopedRegistry) obj;

            return name == other.name &&
                   url == other.url &&
                   scopes.SequenceEqual(other.scopes);
        }

        public static bool operator ==(ScopedRegistry a, ScopedRegistry b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ScopedRegistry a, ScopedRegistry b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            foreach (var scope in scopes)
                hash = hash * 23 + (scope == null ? 0 : scope.GetHashCode());

            hash = hash * 23 + (name == null ? 0 : name.GetHashCode());
            hash = hash * 23 + (url == null ? 0 : url.GetHashCode());

            return hash;
        }
    }

    [Serializable]
    struct DependencyPlaceholder { }
}
