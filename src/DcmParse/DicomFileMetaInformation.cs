// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DcmParser;

public class DicomFileMetaInformation
{
    public required string MediaStorageSOPInstanceUID { get; set; }
    public required string MediaStorageSOPClassUID { get; set; }
    public required string TransferSyntaxUID { get; set; }
    public required string ImplementationClassUID { get; set; }
    public string? ImplementationVersionName { get; set; }
}

