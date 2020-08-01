using System.Collections.Generic;

namespace UnityEngine.XR.Management.Tests.Standalone
{
    public class StandaloneLoader : XRLoaderHelper
    {
        static List<StandaloneSubsystemDescriptor> s_StandaloneSubsystemDescriptors = new List<StandaloneSubsystemDescriptor>();

        public StandaloneSubsystem standaloneSubsystem
        {
            get
            {
                return GetLoadedSubsystem<StandaloneSubsystem>();
            }
        }

        public bool started { get; protected set; }
        public bool stopped { get; protected set; }
        public bool deInitialized { get; protected set; }

        void OnStartCalled()
        {
            started = true;
        }

        void OnStopCalled()
        {
            stopped = true;
        }

        void OnDestroyCalled()
        {
            deInitialized = true;
        }

        public override bool Initialize()
        {
            started = false;
            stopped = false;
            deInitialized = false;

            CreateSubsystem<StandaloneSubsystemDescriptor, StandaloneSubsystem>(s_StandaloneSubsystemDescriptors, "Standalone Subsystem");
            if (standaloneSubsystem == null)
                return false;

            standaloneSubsystem.startCalled += OnStartCalled;
            standaloneSubsystem.stopCalled += OnStopCalled;
            standaloneSubsystem.destroyCalled += OnDestroyCalled;
            return true;
        }

        public override bool Start()
        {
            StartSubsystem<StandaloneSubsystem>();
            return true;
        }

        public override bool Stop()
        {
            StopSubsystem<StandaloneSubsystem>();
            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<StandaloneSubsystem>();
            standaloneSubsystem.startCalled -= OnStartCalled;
            standaloneSubsystem.stopCalled -= OnStopCalled;
            standaloneSubsystem.destroyCalled -= OnDestroyCalled;
            return true;
        }

    }
}
