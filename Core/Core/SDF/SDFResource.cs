using System;
using System.IO;
using System.Reflection;

namespace MonoGame.Samples.Library.SDF;

internal class SDFResource (string name)
{
    public static readonly SDFResource SDFEffect = new (SDFEffectName);

    public const string SDFEffectName = "MonoGame.Samples.Library.SDF.Resources.SDFEffect.mgfxo";

    private readonly object _locker = new ();

    private byte[]? _bytecode;

    public byte[] Bytecode
    {
        get
        {
            if (_bytecode is null)
            {
                lock (_locker)
                {
                    if (_bytecode is not null)
                    {
                        return _bytecode;
                    }

                    _bytecode = PlatformGetBytecode (name);
                }
            }

            return _bytecode;
        }
    }

    private static byte[] PlatformGetBytecode (string name)
    {
        Assembly? assembly = Assembly.GetAssembly (typeof (SDFResource));
        ArgumentNullException.ThrowIfNull (assembly);

        Stream? manifestResourceStream = assembly.GetManifestResourceStream (name);
        ArgumentNullException.ThrowIfNull (manifestResourceStream);

        using MemoryStream memoryStream = new ();
        manifestResourceStream.CopyTo (memoryStream);
        return memoryStream.ToArray ();
    }
}
