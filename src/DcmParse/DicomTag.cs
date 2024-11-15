// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DcmParse;

public sealed record DicomTag(ushort Group, ushort Element, DicomVR VR, DicomVM VM, string Description)
{
    public override string ToString()
    {
        return $"({Group:x4},{Element:x4}) {VR} {Description}";
    }
}
