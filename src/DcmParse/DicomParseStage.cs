// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DcmParser;

public enum DicomParseStage: byte
{
    ParseGroup,
    ParseElement,
    ParseVR,
    ParseLength,
    ParseValue,
}
