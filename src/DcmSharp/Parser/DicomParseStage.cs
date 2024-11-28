// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DcmSharp.Parser;

public enum DicomParseStage: byte
{
    ParseGroup,
    ParseElement,
    ParseVR,
    ParseLength,
    ParseValue,
}
