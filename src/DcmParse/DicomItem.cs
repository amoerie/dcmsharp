// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace DcmParser;

[StructLayout(LayoutKind.Auto)]
public readonly record struct DicomItem(
    ushort Group,
    ushort Element,
    DicomVR VR,
    DicomItemContent Content)
{
    public override string ToString()
    {
        return DicomTagsIndex.TryLookup(Group, Element, out var dicomTag)
            ? $"(0x{Group:x4},0x{Element:x4}) {VR} {dicomTag.Description}"
            : $"(0x{Group:x4},0x{Element:x4}) {VR}";
    }
}
