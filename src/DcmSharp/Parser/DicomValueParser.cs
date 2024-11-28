using DcmSharp.Parser.ValueRepresentations;

namespace DcmSharp.Parser;

internal sealed record DicomValueParser(
    AEParser AE,
    ASParser AS,
    ATParser AT,
    CSParser CS,
    DAParser DA,
    DSParser DS,
    DTParser DT,
    FDParser FD,
    FLParser FL,
    ISParser IS,
    LOParser LO,
    LTParser LT,
    PNParser PN,
    SHParser SH,
    SLParser SL,
    SSParser SS,
    STParser ST,
    SVParser SV,
    TMParser TM,
    UCParser UC,
    UIParser UI,
    ULParser UL,
    USParser US,
    UTParser UT,
    UVParser UV
);
