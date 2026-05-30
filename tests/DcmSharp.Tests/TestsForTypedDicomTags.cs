using DcmSharp;
using DcmSharp.Parser;
using FluentAssertions;
using Xunit;
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

        dataset.TryGet(DicomTags.SOPInstanceUID, out string? value).Should().BeTrue();
        value.Should().Be("2.25.332838821141227624838581964210008219211");
    }

    [Fact]
    public async Task ReadOnly_TryGet_String_AccessionNumber()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.AccessionNumber, out string? value).Should().BeTrue();
        value.Should().NotBeNull();
    }

    [Fact]
    public async Task ReadOnly_TryGet_PersonName_PatientName()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.PatientName, out PersonName value).Should().BeTrue();
        value.Should().NotBe(default(PersonName));
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShort_Rows()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.Rows, out ushort value).Should().BeTrue();
        value.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShort_Columns()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.Columns, out ushort value).Should().BeTrue();
        value.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateOnly_StudyDate()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.StudyDate, out DateOnly value).Should().BeTrue();
        value.Should().NotBe(default);
    }

    [Fact]
    public async Task ReadOnly_TryGet_TimeOnly_StudyTime()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.StudyTime, out TimeOnly value).Should().BeTrue();
        value.Should().NotBe(default);
    }



    // ===========================
    // ReadOnlyDicomDataset TryGet — array types from SingleValues.dcm
    // ===========================

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_AE()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorAEValue, out string[]? value).Should().BeTrue();
        value.Should().Contain("MODALITY1");
    }

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_LO()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorLOValue, out string[]? value).Should().BeTrue();
        value.Should().Contain("Medical Center A");
    }

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_SH()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorSHValue, out string[]? value).Should().BeTrue();
        value.Should().Contain("CT123");
    }

    [Fact]
    public async Task ReadOnly_TryGet_StringArray_UI()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorUIValue, out string[]? value).Should().BeTrue();
        value.Should().Contain("1.2.3");
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateOnlyArray_DA()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorDAValue, out DateOnly[]? value).Should().BeTrue();
        value.Should().Contain(DateOnly.Parse("2024-12-03"));
    }

    [Fact]
    public async Task ReadOnly_TryGet_TimeOnlyArray_TM()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorTMValue, out TimeOnly[]? value).Should().BeTrue();
        value.Should().Contain(TimeOnly.Parse("12:00:00"));
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateTimeArray_DT()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorDTValue, out DateTime[]? value).Should().BeTrue();
        value.Should().Contain(DateTime.Parse("2024-12-03T12:00:00"));
    }

    [Fact]
    public async Task ReadOnly_TryGet_ShortArray_SS()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorSSValue, out short[]? value).Should().BeTrue();
        value.Should().Contain((short)-32768);
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShortArray_US()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorUSValue, out ushort[]? value).Should().BeTrue();
        value.Should().Contain((ushort)1);
    }

    [Fact]
    public async Task ReadOnly_TryGet_IntArray_SL()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorSLValue, out int[]? value).Should().BeTrue();
        value.Should().Contain(-1);
    }

    [Fact]
    public async Task ReadOnly_TryGet_UIntArray_UL()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorULValue, out uint[]? value).Should().BeTrue();
        value.Should().Contain(4294967295u);
    }

    [Fact]
    public async Task ReadOnly_TryGet_LongArray_SV()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorSVValue, out long[]? value).Should().BeTrue();
        value.Should().Contain(9223372036854775807L);
    }

    [Fact]
    public async Task ReadOnly_TryGet_ULongArray_UV()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorUVValue, out ulong[]? value).Should().BeTrue();
        value.Should().Contain(18446744073709551615UL);
    }

    [Fact]
    public async Task ReadOnly_TryGet_FloatArray_FL()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorFLValue, out float[]? value).Should().BeTrue();
        value.Should().Contain(100.5f);
    }

    [Fact]
    public async Task ReadOnly_TryGet_DoubleArray_FD()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorFDValue, out double[]? value).Should().BeTrue();
        value.Should().Contain(-100.123);
    }

    [Fact]
    public async Task ReadOnly_TryGet_PersonNameArray_PN()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SelectorPNValue, out PersonName[]? value).Should().BeTrue();
        value.Should().Contain(new PersonName("Dr", "Smith", null, null, null, null, null, null, null));
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
        dataset.TryGet(DicomTags.AccessionNumber, out string? _).Should().BeFalse();
    }

    [Fact]
    public async Task ReadOnly_TryGet_DateOnly_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.StudyDate, out DateOnly _).Should().BeFalse();
    }

    [Fact]
    public async Task ReadOnly_TryGet_UShort_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.Rows, out ushort _).Should().BeFalse();
    }

    [Fact]
    public async Task ReadOnly_TryGet_PersonName_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.PatientName, out PersonName _).Should().BeFalse();
    }

    [Fact]
    public async Task ReadOnly_TryGet_Memory_ReturnsFalse_WhenMissing()
    {
        var file = new FileInfo("./Dicom/SingleValues.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        var obTag = new DicomTag<ReadOnlyMemory<byte>>(0x7FFF, 0x0001, DicomVR.OB, [], DicomVM.VM_1, "FakeTag", "Fake Tag");
        dataset.TryGet(obTag, out ReadOnlyMemory<byte>? _).Should().BeFalse();
    }

    // ===========================
    // DicomDataset Set + TryGet roundtrips — scalars
    // ===========================

    [Fact]
    public void Mutable_Set_TryGet_String()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.AccessionNumber, "ABC123");

        dataset.TryGet(DicomTags.AccessionNumber, out string? value).Should().BeTrue();
        value.Should().Be("ABC123");
    }

    [Fact]
    public void Mutable_Set_TryGet_DateOnly()
    {
        var dataset = new DicomDataset();
        var date = new DateOnly(2024, 6, 15);
        dataset.Set(DicomTags.StudyDate, date);

        dataset.TryGet(DicomTags.StudyDate, out DateOnly value).Should().BeTrue();
        value.Should().Be(date);
    }

    [Fact]
    public void Mutable_Set_TryGet_Short()
    {
        var tag = new DicomTag<short>(0x0072, 0x0074, DicomVR.SS, [], DicomVM.VM_1, "SelectorSSValue", "Selector SS Value");
        var dataset = new DicomDataset();
        dataset.Set(tag, (short)-12345);

        dataset.TryGet(tag, out short value).Should().BeTrue();
        value.Should().Be(-12345);
    }

    [Fact]
    public void Mutable_Set_TryGet_UShort()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.Rows, (ushort)512);

        dataset.TryGet(DicomTags.Rows, out ushort value).Should().BeTrue();
        value.Should().Be(512);
    }

    [Fact]
    public void Mutable_Set_TryGet_Int()
    {
        var tag = new DicomTag<int>(0x0072, 0x0076, DicomVR.SL, [], DicomVM.VM_1, "SelectorSLValue", "Selector SL Value");
        var dataset = new DicomDataset();
        dataset.Set(tag, -999999);

        dataset.TryGet(tag, out int value).Should().BeTrue();
        value.Should().Be(-999999);
    }

    [Fact]
    public void Mutable_Set_TryGet_UInt()
    {
        var tag = new DicomTag<uint>(0x0072, 0x0078, DicomVR.UL, [], DicomVM.VM_1, "SelectorULValue", "Selector UL Value");
        var dataset = new DicomDataset();
        dataset.Set(tag, 4294967295u);

        dataset.TryGet(tag, out uint value).Should().BeTrue();
        value.Should().Be(4294967295u);
    }

    [Fact]
    public void Mutable_Set_TryGet_Long()
    {
        var tag = new DicomTag<long>(0x0072, 0x007A, DicomVR.SV, [], DicomVM.VM_1, "SelectorSVValue", "Selector SV Value");
        var dataset = new DicomDataset();
        dataset.Set(tag, long.MaxValue);

        dataset.TryGet(tag, out long value).Should().BeTrue();
        value.Should().Be(long.MaxValue);
    }

    [Fact]
    public void Mutable_Set_TryGet_Float()
    {
        var tag = new DicomTag<float>(0x0072, 0x006E, DicomVR.FL, [], DicomVM.VM_1, "SelectorFLValue", "Selector FL Value");
        var dataset = new DicomDataset();
        dataset.Set(tag, 3.14f);

        dataset.TryGet(tag, out float value).Should().BeTrue();
        value.Should().Be(3.14f);
    }

    [Fact]
    public void Mutable_Set_TryGet_Double()
    {
        var tag = new DicomTag<double>(0x0072, 0x0070, DicomVR.FD, [], DicomVM.VM_1, "SelectorFDValue", "Selector FD Value");
        var dataset = new DicomDataset();
        dataset.Set(tag, -100.123);

        dataset.TryGet(tag, out double value).Should().BeTrue();
        value.Should().Be(-100.123);
    }

    [Fact]
    public void Mutable_Set_TryGet_PersonName()
    {
        var dataset = new DicomDataset();
        var name = new PersonName("DOE", "JOHN", "M", "DR", "JR", null, null, null, null);
        dataset.Set(DicomTags.PatientName, name);

        dataset.TryGet(DicomTags.PatientName, out PersonName value).Should().BeTrue();
        value.FamilyName.Should().Be("DOE");
        value.GivenName.Should().Be("JOHN");
        value.MiddleName.Should().Be("M");
        value.NamePrefix.Should().Be("DR");
        value.NameSuffix.Should().Be("JR");
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

        dataset.TryGet(DicomTags.AccessionNumber, out string? value).Should().BeTrue();
        value.Should().Be("NEW");
    }

    [Fact]
    public void Mutable_Set_OverwritesExistingUShort()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.Rows, (ushort)100);
        dataset.Set(DicomTags.Rows, (ushort)200);

        dataset.TryGet(DicomTags.Rows, out ushort value).Should().BeTrue();
        value.Should().Be(200);
    }

    [Fact]
    public void Mutable_Set_OverwritesExistingPersonName()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.PatientName, new PersonName("OLD", null, null, null, null, null, null, null, null));
        dataset.Set(DicomTags.PatientName, new PersonName("NEW", null, null, null, null, null, null, null, null));

        dataset.TryGet(DicomTags.PatientName, out PersonName value).Should().BeTrue();
        value.FamilyName.Should().Be("NEW");
    }

    // ===========================
    // DicomDataset TryGet — missing tags return false
    // ===========================

    [Fact]
    public void Mutable_TryGet_String_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        dataset.TryGet(DicomTags.AccessionNumber, out string? _).Should().BeFalse();
    }

    [Fact]
    public void Mutable_TryGet_UShort_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        dataset.TryGet(DicomTags.Rows, out ushort _).Should().BeFalse();
    }

    [Fact]
    public void Mutable_TryGet_DateOnly_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        dataset.TryGet(DicomTags.StudyDate, out DateOnly _).Should().BeFalse();
    }

    [Fact]
    public void Mutable_TryGet_Double_ReturnsFalse_WhenMissing()
    {
        var tag = new DicomTag<double>(0x0072, 0x0070, DicomVR.FD, [], DicomVM.VM_1, "SelectorFDValue", "Selector FD Value");
        var dataset = new DicomDataset();
        dataset.TryGet(tag, out double _).Should().BeFalse();
    }

    [Fact]
    public void Mutable_TryGet_PersonName_ReturnsFalse_WhenMissing()
    {
        var dataset = new DicomDataset();
        dataset.TryGet(DicomTags.PatientName, out PersonName _).Should().BeFalse();
    }

    [Fact]
    public void Mutable_TryGet_Long_ReturnsFalse_WhenMissing()
    {
        var tag = new DicomTag<long>(0x0072, 0x007A, DicomVR.SV, [], DicomVM.VM_1, "SelectorSVValue", "Selector SV Value");
        var dataset = new DicomDataset();
        dataset.TryGet(tag, out long _).Should().BeFalse();
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

        stringTag.ValueRepresentation.Should().Be(DicomVR.SH);
        ushortTag.ValueRepresentation.Should().Be(DicomVR.US);
        dateTag.ValueRepresentation.Should().Be(DicomVR.DA);
        pnTag.ValueRepresentation.Should().Be(DicomVR.PN);
    }

    [Fact]
    public void TypeSafety_TypedTag_IsAssignableToBaseTag()
    {
        DicomTag<string> typed = DicomTags.AccessionNumber;
        DicomTag untyped = typed;

        untyped.Keyword.Should().Be("AccessionNumber");
        untyped.Group.Should().Be(0x0008);
        untyped.Element.Should().Be(0x0050);
    }

    [Fact]
    public void TypeSafety_MultiVR_Tags_RemainUntyped()
    {
        DicomTag tag = DicomTags.SmallestImagePixelValue;
        tag.AdditionalValueRepresentations.Should().NotBeEmpty();
        tag.Should().NotBeOfType<DicomTag<ushort>>();
        tag.Should().NotBeOfType<DicomTag<short>>();
    }

    [Fact]
    public void TypeSafety_ArrayTypedTag_IsCorrect()
    {
        DicomTag<string[]> arrayTag = DicomTags.SelectorAEValue;
        DicomTag untyped = arrayTag;

        untyped.ValueRepresentation.Should().Be(DicomVR.AE);
    }
}
