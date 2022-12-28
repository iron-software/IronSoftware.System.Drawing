using System;
using System.IO;
#if NETFRAMEWORK
using System.Reflection;
#else
using System.Runtime.InteropServices;
#endif

namespace IronSoftware.Drawing.Common.Tests
{
    public class TargetFramework
    {
        public TargetFramework()
        {
#if NETFRAMEWORK
            SuffixName = "framework";
            IsFramework = true;
#else
            SuffixName = "dotnet";
            IsDotNetCore = true;
#endif
        }

        public AppDomain CreateAppDomain(string domainName)
        {
#if NETFRAMEWORK
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Directory.GetCurrentDirectory();
            setup.ConfigurationFile = $"{Assembly.GetExecutingAssembly().Location}.config";
            setup.TargetFrameworkName = AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
            return AppDomain.CreateDomain(domainName, null, setup);
#else
            throw new NotImplementedException();
#endif
        }

        public void DoAppDomainCall(AppDomain domain, Action action)
        {
#if NETFRAMEWORK
            var crossAppDomainAction = new CrossAppDomainAction(action);
            domain.DoCallBack(crossAppDomainAction.Invoke);

            // rethrow exception from the domain into the current AppDomain. 
            var exception = crossAppDomainAction.GetException(domain);
            if(exception != null)
            {
                throw exception;
            }
#else
            throw new NotImplementedException();
#endif
        }

        public static string GetAppSettingsValue(string key)
        {
            if (!Environment.GetEnvironmentVariables().Contains(key))
            {
                throw new InvalidOperationException($"Can not find the Environment variable by the name='{key}'");
            }

            var configValue = Environment.GetEnvironmentVariable(key);
            return configValue;
        }

        public void UnloadAppDomain(AppDomain domain)
        {
            AppDomain.Unload(domain);
        }

        public bool IsDotNetCore { get; private set; }

        public bool IsFramework { get; private set; }

        public string OsGeneralName
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                    case PlatformID.WinCE:
                    case PlatformID.Xbox:
                        return "windows";
                    case PlatformID.Unix:
                        // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                        // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                        if (Directory.Exists("/Applications")
                            & Directory.Exists("/System")
                            & Directory.Exists("/Users")
                            & Directory.Exists("/Volumes"))
                            goto case PlatformID.MacOSX;
                        else
                            return "linux";
                    case PlatformID.MacOSX:
                        return "osx";
                }

#if NETFRAMEWORK
                return "windows";
#else
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return "linux";
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return "osx";
                }
#if !NETCOREAPP2_1
                if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    return "bsd";
                }
#endif

                return "windows";
#endif
            }
        }

        public string SuffixName { get; private set; }
        public string TestEnvironmentName => Environment.GetEnvironmentVariable("TEST_ENVIRONMENT_NAME");
    }
#if NETFRAMEWORK
    // do not use short name for the System.Serializable, because it conflicts with the SerializableAttribute
    // from the file PdfSharpCore\SilverlightInternals\AgHacks.cs : 55
    [System.Serializable]
    internal class CrossAppDomainAction : MarshalByRefObject
    {
        private static readonly string ExceptionStoreKey =
            $"{typeof(CrossAppDomainAction).FullName}.{nameof(SetException)}";

        private readonly Action _action;

        public CrossAppDomainAction(Action action)
        {
            _action = action;
        }

        private void SetException(Exception exception)
        {
            if(exception.GetType().IsSerializable)
            {
                AppDomain.CurrentDomain.SetData(ExceptionStoreKey, exception);
            }
            else
            {
                AppDomain.CurrentDomain.SetData(ExceptionStoreKey, new InvalidOperationException(exception.ToString()));
            }
        }

        public Exception GetException(AppDomain domain)
        {
            var data = domain.GetData(ExceptionStoreKey);
            return (Exception)data;
        }

        public void Invoke()
        {
            try
            {
                _action();
            }
            catch(Exception e)
            {
                SetException(e);
            }
        }
    }
#endif
}
