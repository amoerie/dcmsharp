# dcmsharp

> **⚠️ Experimental — not production-ready**
>
> This library is under active development. APIs are unstable, behaviour may change without notice, and it has not been validated for clinical or production use. Use at your own risk.

A high-performance .NET 9 library for parsing DICOM files, plus three CLI tools built on top of it.

## Tools

| Tool | Command | Purpose |
|---|---|---|
| DcmFind | `dcmfind` | Search DICOM files by tag criteria |
| DcmAnonymize | `DcmAnonymize` | Anonymize patient and pixel data |
| DcmOrganize | `dcmorganize` | Reorganize files into folder structures |

## Building

```bash
dotnet build --configuration Release
dotnet test  --configuration Release
```

Requires [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

## License

MIT © Alexander Moerman
