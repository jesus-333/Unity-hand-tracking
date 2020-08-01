using System.Collections.Generic;

using UnityEditor;
using UnityEditor.XR.Management.Metadata;

namespace Samples
{
    class SampleLoaderMetadata : IXRLoaderMetadata 
    {
        public string loaderName { get; set; }
        public string loaderType { get; set; }
        public List<BuildTargetGroup> supportedBuildTargets { get; set; }
    }

    class SamplePackageMetadata : IXRPackageMetadata
    {
        public string packageName { get; set; }
        public string packageId { get; set; }
        public string settingsType { get; set; }
        public List<IXRLoaderMetadata> loaderMetadata { get; set; } 
    }

    static class SampleMetadata
    {
        static SamplePackageMetadata s_Metadata = null;

        internal static SamplePackageMetadata CreateAndGetMetadata()
        {
            if (s_Metadata == null)
            {
                s_Metadata = new SamplePackageMetadata();
                s_Metadata.packageName = "Sample Package";
                s_Metadata.packageId = "com.unity.xr.samplespackage";
                s_Metadata.settingsType = typeof(SampleSettings).FullName;

                s_Metadata.loaderMetadata = new List<IXRLoaderMetadata>() {
                    new SampleLoaderMetadata() {
                        loaderName = "Sample Loader One",
                        loaderType = typeof(SampleLoader).FullName,
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Standalone,
                            BuildTargetGroup.WSA
                        }
                    },
                    new SampleLoaderMetadata() {
                        loaderName = "Sample Loader Two",
                        loaderType = typeof(SampleLoader).FullName,
                        supportedBuildTargets = new List<BuildTargetGroup>() {
                            BuildTargetGroup.Android,
                            BuildTargetGroup.iOS,
                            BuildTargetGroup.Lumin
                        }
                    }
                };
            }

            return s_Metadata;
        }
    }
}
