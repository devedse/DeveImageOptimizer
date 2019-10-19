using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DeveImageOptimizer.Tests.ExternalTools
{
    public class ZoneIdentifier : IDisposable
    {
        private IPersistFile _persistFile;
        private IZoneIdentifier _zoneIdentifier;
        private UrlZone _urlZone;

        public ZoneIdentifier(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }
            _persistFile = (IPersistFile)new PersistFile();
            _persistFile.Load(fileName, StorageMode.Read /* ignored for IZoneIdentifier apparently */);
            _zoneIdentifier = (IZoneIdentifier)_persistFile;
            _zoneIdentifier.GetId(out _urlZone);
        }

        public UrlZone Zone
        {
            get
            {
                if (_persistFile == null)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _urlZone;
            }
            set
            {
                if (_persistFile == null)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (_urlZone == value)
                {
                    return;
                }

                if (!Enum.IsDefined(typeof(UrlZone), value) || value == UrlZone.Invalid)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _urlZone = value;
                _zoneIdentifier.SetId(_urlZone);
                _persistFile.Save(null, false);
            }
        }

        public void Remove()
        {
            if (_persistFile == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (_urlZone == UrlZone.LocalMachine)
            {
                return;
            }

            _urlZone = UrlZone.LocalMachine;
            _zoneIdentifier.Remove();
            _persistFile.Save(null, false);
        }

        public void Dispose()
        {
            if (_persistFile == null)
            {
                return;
            }

            _zoneIdentifier = null;
            _persistFile = null;
        }
    }

    [ComImport]
    [Guid("0000010c-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersist
    {
        [PreserveSig]
        void GetClassID(out Guid pClassId);
    }

    [ComImport]
    [Guid("0000010b-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistFile : IPersist
    {
        new void GetClassID(out Guid pClassId);

        [PreserveSig]
        int IsDirty();

        // http://msdn.microsoft.com/en-us/library/ie/ms687284(v=vs.85).aspx
        [PreserveSig]
        void Load([In, MarshalAs(UnmanagedType.LPWStr)]string pszFileName, [MarshalAs(UnmanagedType.U4)] StorageMode dwMode);

        [PreserveSig]
        void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

        [PreserveSig]
        void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        [PreserveSig]
        void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
    }

    [ComImport]
    [Guid("0968e258-16c7-4dba-aa86-462dd61e31a3")]
    internal class PersistFile { }

    // http://msdn.microsoft.com/en-us/library/ms537032(v=vs.85).aspx
    [ComImport]
    [Guid("cd45f185-1b21-48e2-967b-ead743a8914e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IZoneIdentifier
    {
        [PreserveSig]
        void GetId([Out] out UrlZone dwZone);

        [PreserveSig]
        void SetId(UrlZone dwZone);

        [PreserveSig]
        void Remove();
    }

    // http://msdn.microsoft.com/en-us/library/ms537175(v=vs.85).aspx
    public enum UrlZone
    {
        Invalid = -1,
        LocalMachine = 0,
        Intranet,
        Trusted,
        Internet,
        Untrusted,
    }

    // http://msdn.microsoft.com/en-us/library/ie/aa380337(v=vs.85).aspx
    [Flags]
    internal enum StorageMode
    {
        Read = 0,
        Write = 1,
        ReadWrite = 2,
        ShareDenyNone = 0x40,
        ShareDenyRead = 0x30,
        ShareDenyWrite = 0x20,
        ShareExclusive = 0x10,
        Priority = 0x40000,
        Create = 0x1000,
        Convert = 0x20000,
        FailIfThere = 0,
        Direct = 0,
        Transacted = 0x10000,
        NoScratch = 0x100000,
        NoSnapshot = 0x200000,
        Simple = 0x8000000,
        DirectSWMR = 0x400000,
        DeleteOnRelease = 0x4000000
    }
}
