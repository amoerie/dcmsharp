using DcmSharp.Parser;
using Xunit.Abstractions;

namespace DcmSharp.Tests;

[Collection(nameof(DicomParserCollection))]
public sealed class TestsForTypedDicomTags
{
    private readonly IDicomParser _dicomParser;

    public TestsForTypedDicomTags(DicomParserFixture fixture, ITestOutputHelper output)
    {
        fixture.OutputHelper = output;
        _dicomParser = fixture.DicomParser;
    }

    // ===========================
    // ReadOnlyDicomDataset TryGet — scalar types from ExplicitVR.dcm
    // ===========================

    [Fact]
    public async Task ReadOnly_TryGet_String_SOPInstanceUID()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SOPInstanceUID, out string? value));
        Assert.Equal("2.25.332838821141227624838581964210008219211", value);
    }

    [Fact]
    public async Task ReadOnly_TryGet_String_AccessionNumber()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.AccessionNumber, out string? value));
        Assert.NotNull(value);
    }

    [Fact]
    public async Task ReadOnly_TryGet_PersonName_PatientName()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.PatientName, out PersonName value));
        Assert.NotEqual(default, value);
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShort_Rows()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.Rows, out ushort value));
        Assert.True(value > 0);
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShort_Columns()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.Columns, out ushort value));
        Assert.True(value > 0);
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateOnly_StudyDate()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.StudyDate, out DateOnly value));
        Assert.NotEqual(default, value);
    }

    [Fact]
    public async Task ReadOnly_TryGet_TimeOnly_StudyTime()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.StudyTime, out TimeOnly value));
        Assert.NotEqual(default, value);
    }

    // ===========================
    // ReadOnlyDicomDataset TryGet — array types from SingleValues.dcm
    // ===========================

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_AE()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorAEValue, out string[]? value));
        Assert.Contains("MODALITY1", value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_LO()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorLOValue, out string[]? value));
        Assert.Contains("Medical Center A", value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_SH()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorSHValue, out string[]? value));
        Assert.Contains("CT123", value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_UI()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorUIValue, out string[]? value));
        Assert.Contains("1.2.3", value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateOnlyArray_DA()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorDAValue, out DateOnly[]? value));
        Assert.Contains(DateOnly.Parse("2024-12-03"), value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_TimeOnlyArray_TM()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorTMValue, out TimeOnly[]? value));
        Assert.Contains(TimeOnly.Parse("12:00:00"), value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateTimeArray_DT()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorDTValue, out DateTime[]? value));
        Assert.Contains(DateTime.Parse("2024-12-03T12:00:00"), value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_ShortArray_SS()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorSSValue, out short[]? value));
        Assert.Contains((short)-32768, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShortArray_US()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorUSValue, out ushort[]? value));
        Assert.Contains((ushort)1, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_IntArray_SL()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorSLValue, out int[]? value));
        Assert.Contains(-1, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_UIntArray_UL()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorULValue, out uint[]? value));
        Assert.Contains(4294967295u, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_LongArray_SV()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorSVValue, out long[]? value));
        Assert.Contains(9223372036854775807L, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_ULongArray_UV()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorUVValue, out ulong[]? value));
        Assert.Contains(18446744073709551615UL, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_FloatArray_FL()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorFLValue, out float[]? value));
        Assert.Contains(100.5f, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_DoubleArray_FD()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorFDValue, out double[]? value));
        Assert.Contains(-100.123, value!);
    }

    [Fact]
    public async Task ReadOnly_TryGet_PersonNameArray_PN()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.True(dataset.TryGet(DicomTags.SelectorPNValue, out PersonName[]? value));
        Assert.Contains(
            new PersonName("Dr", "Smith", null, null, null, null, null, null, null),
            value!
        );
    }

    // ===========================
    // ReadOnlyDicomDataset TryGet — missing tags return false
    // ===========================

    [Fact]
    public async Task ReadOnly_TryGet_String_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        // Use a tag that definitely exists as DicomTag<string> but isn't in this file
        Assert.False(dataset.TryGet(DicomTags.AccessionNumber, out string? _));
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateOnly_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.False(dataset.TryGet(DicomTags.StudyDate, out DateOnly _));
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShort_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.False(dataset.TryGet(DicomTags.Rows, out ushort _));
    }

    [Fact]
    public async Task ReadOnly_TryGet_PersonName_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        Assert.False(dataset.TryGet(DicomTags.PatientName, out PersonName _));
    }

    [Fact]
    public async Task ReadOnly_TryGet_Memory_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        var obTag = new DicomTag<ReadOnlyMemory<byte>>(
            0x7FFF,
            0x0001,
            DicomVR.OB,
            [],
            DicomVM.VM_1,
            "FakeTag",
            "Fake Tag"
        );
        Assert.False(dataset.TryGet(obTag, out ReadOnlyMemory<byte>? _));
    }

    // ===========================
    // DicomDataset Set + TryGet roundtrips — scalars
    // ===========================

    [Fact]
    public void Mutable_Set_TryGet_String()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.AccessionNumber, "ABC123");

        Assert.True(dataset.TryGet(DicomTags.AccessionNumber, out string? value));
        Assert.Equal("ABC123", value);
    }

    [Fact]
    public void Mutable_Set_TryGet_DateOnly()
    {
        var dataset = new DicomDataset();
        var date = new DateOnly(2024, 6, 15);
        dataset.Set(DicomTags.StudyDate, date);

        Assert.True(dataset.TryGet(DicomTags.StudyDate, out DateOnly value));
        Assert.Equal(date, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_Short()
    {
        var tag = new DicomTag<short>(
            0x0072,
            0x0074,
            DicomVR.SS,
            [],
            DicomVM.VM_1,
            "SelectorSSValue",
            "Selector SS Value"
        );
        var dataset = new DicomDataset();
        dataset.Set(tag, (short)-12345);

        Assert.True(dataset.TryGet(tag, out short value));
        Assert.Equal(-12345, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_UShort()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.Rows, (ushort)512);

        Assert.True(dataset.TryGet(DicomTags.Rows, out ushort value));
        Assert.Equal(512, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_Int()
    {
        var tag = new DicomTag<int>(
            0x0072,
            0x0076,
            DicomVR.SL,
            [],
            DicomVM.VM_1,
            "SelectorSLValue",
            "Selector SL Value"
        );
        var dataset = new DicomDataset();
        dataset.Set(tag, -999999);

        Assert.True(dataset.TryGet(tag, out int value));
        Assert.Equal(-999999, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_UInt()
    {
        var tag = new DicomTag<uint>(
            0x0072,
            0x0078,
            DicomVR.UL,
            [],
            DicomVM.VM_1,
            "SelectorULValue",
            "Selector UL Value"
        );
        var dataset = new DicomDataset();
        dataset.Set(tag, 4294967295u);

        Assert.True(dataset.TryGet(tag, out uint value));
        Assert.Equal(4294967295u, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_Long()
    {
        var tag = new DicomTag<long>(
            0x0072,
            0x007A,
            DicomVR.SV,
            [],
            DicomVM.VM_1,
            "SelectorSVValue",
            "Selector SV Value"
        );
        var dataset = new DicomDataset();
        dataset.Set(tag, long.MaxValue);

        Assert.True(dataset.TryGet(tag, out long value));
        Assert.Equal(long.MaxValue, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_Float()
    {
        var tag = new DicomTag<float>(
            0x0072,
            0x006E,
            DicomVR.FL,
            [],
            DicomVM.VM_1,
            "SelectorFLValue",
            "Selector FL Value"
        );
        var dataset = new DicomDataset();
        dataset.Set(tag, 3.14f);

        Assert.True(dataset.TryGet(tag, out float value));
        Assert.Equal(3.14f, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_Double()
    {
        var tag = new DicomTag<double>(
            0x0072,
            0x0070,
            DicomVR.FD,
            [],
            DicomVM.VM_1,
            "SelectorFDValue",
            "Selector FD Value"
        );
        var dataset = new DicomDataset();
        dataset.Set(tag, -100.123);

        Assert.True(dataset.TryGet(tag, out double value));
        Assert.Equal(-100.123, value);
    }

    [Fact]
    public void Mutable_Set_TryGet_PersonName()
    {
        var dataset = new DicomDataset();
        var name = new PersonName("DOE", "JOHN", "M", "DR", "JR", null, null, null, null);
        dataset.Set(DicomTags.PatientName, name);

        Assert.True(dataset.TryGet(DicomTags.PatientName, out PersonName value));
        Assert.Equal("DOE", value.FamilyName);
        Assert.Equal("JOHN", value.GivenName);
        Assert.Equal("M", value.MiddleName);
        Assert.Equal("DR", value.NamePrefix);
        Assert.Equal("JR", value.NameSuffix);
    }

    // ===========================
    // DicomDataset Set overwrite behavior (upsert)
    // ===========================

    [Fact]
    public void Mutable_Set_OverwritesExistingString()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.AccessionNumber, "OLD");
        dataset.Set(DicomTags.AccessionNumber, "NEW");

        Assert.True(dataset.TryGet(DicomTags.AccessionNumber, out string? value));
        Assert.Equal("NEW", value);
    }

    [Fact]
    public void Mutable_Set_OverwritesExistingUShort()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.Rows, (ushort)100);
        dataset.Set(DicomTags.Rows, (ushort)200);

        Assert.True(dataset.TryGet(DicomTags.Rows, out ushort value));
        Assert.Equal(200, value);
    }

    [Fact]
    public void Mutable_Set_OverwritesExistingPersonName()
    {
        var dataset = new DicomDataset();
        dataset.Set(
            DicomTags.PatientName,
            new PersonName("OLD", null, null, null, null, null, null, null, null)
        );
        dataset.Set(
            DicomTags.PatientName,
            new PersonName("NEW", null, null, null, null, null, null, null, null)
        );

        Assert.True(dataset.TryGet(DicomTags.PatientName, out PersonName value));
        Assert.Equal("NEW", value.FamilyName);
    }

    // ===========================
    // DicomDataset TryGet — missing tags return false
    // ===========================

    [Fact]
    public void Mutable_TryGet_String_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        Assert.False(dataset.TryGet(DicomTags.AccessionNumber, out string? _));
    }

    [Fact]
    public void Mutable_TryGet_UShort_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        Assert.False(dataset.TryGet(DicomTags.Rows, out ushort _));
    }

    [Fact]
    public void Mutable_TryGet_DateOnly_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        Assert.False(dataset.TryGet(DicomTags.StudyDate, out DateOnly _));
    }

    [Fact]
    public void Mutable_TryGet_Double_ReturnsFalse_WhenMissing()
    {
        var tag = new DicomTag<double>(
            0x0072,
            0x0070,
            DicomVR.FD,
            [],
            DicomVM.VM_1,
            "SelectorFDValue",
            "Selector FD Value"
        );
        var dataset = new DicomDataset();
        Assert.False(dataset.TryGet(tag, out double _));
    }

    [Fact]
    public void Mutable_TryGet_PersonName_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        Assert.False(dataset.TryGet(DicomTags.PatientName, out PersonName _));
    }

    [Fact]
    public void Mutable_TryGet_Long_ReturnsFalse_WhenMissing()
    {
        var tag = new DicomTag<long>(
            0x0072,
            0x007A,
            DicomVR.SV,
            [],
            DicomVM.VM_1,
            "SelectorSVValue",
            "Selector SV Value"
        );
        var dataset = new DicomDataset();
        Assert.False(dataset.TryGet(tag, out long _));
    }

    // ===========================
    // Type safety — compile-time guarantees
    // ===========================

    [Fact]
    public void TypeSafety_GeneratedTags_HaveCorrectTypeParameter()
    {
        DicomTag<string> stringTag = DicomTags.AccessionNumber;
        DicomTag<ushort> ushortTag = DicomTags.Rows;
        DicomTag<DateOnly> dateTag = DicomTags.StudyDate;
        DicomTag<PersonName> pnTag = DicomTags.PatientName;

        Assert.Equal(DicomVR.SH, stringTag.ValueRepresentation);
        Assert.Equal(DicomVR.US, ushortTag.ValueRepresentation);
        Assert.Equal(DicomVR.DA, dateTag.ValueRepresentation);
        Assert.Equal(DicomVR.PN, pnTag.ValueRepresentation);
    }

    [Fact]
    public void TypeSafety_TypedTag_IsAssignableToBaseTag()
    {
        DicomTag<string> typed = DicomTags.AccessionNumber;
        DicomTag untyped = typed;

        Assert.Equal("AccessionNumber", untyped.Keyword);
        Assert.Equal(0x0008, untyped.Group);
        Assert.Equal(0x0050, untyped.Element);
    }

    [Fact]
    public void TypeSafety_MultiVR_Tags_RemainUntyped()
    {
        DicomTag tag = DicomTags.SmallestImagePixelValue;
        Assert.NotEmpty(tag.AdditionalValueRepresentations);
        Assert.IsNotType<DicomTag<ushort>>(tag);
        Assert.IsNotType<DicomTag<short>>(tag);
    }

    [Fact]
    public void TypeSafety_ArrayTypedTag_IsCorrect()
    {
        DicomTag<string[]> arrayTag = DicomTags.SelectorAEValue;
        DicomTag untyped = arrayTag;

        Assert.Equal(DicomVR.AE, untyped.ValueRepresentation);
    }
}
