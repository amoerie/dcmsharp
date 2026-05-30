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

    [Fact]
    public async Task TryGet_String_ShouldReturnValue_FromReadOnlyDataset()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.SOPInstanceUID, out var value).Should().BeTrue();

        value.Should().Be("2.25.332838821141227624838581964210008219211");
    }

    [Fact]
    public async Task TryGet_String_ShouldReturnFalse_WhenTagMissing_FromReadOnlyDataset()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.InstitutionAddress, out string? _).Should().BeFalse();
    }

    [Fact]
    public async Task TryGet_UShort_ShouldReturnValue_FromReadOnlyDataset()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.Rows, out ushort value).Should().BeTrue();

        value.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TryGet_Date_ShouldReturnValue_FromReadOnlyDataset()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.StudyDate, out DateOnly value).Should().BeTrue();

        value.Year.Should().BeGreaterThan(2000);
    }

    [Fact]
    public async Task TryGet_PersonName_ShouldReturnValue_FromReadOnlyDataset()
    {
        var file = new FileInfo("./Dicom/ExplicitVR.dcm");
        using var dataset = await _dicomParser.ParseReadOnlyAsync(file);

        dataset.TryGet(DicomTags.PatientName, out PersonName value).Should().BeTrue();

        value.Should().NotBe(default(PersonName));
    }

    [Fact]
    public void TryGet_String_ShouldReturnValue_FromMutableDataset()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.AccessionNumber, "ABC123");

        dataset.TryGet(DicomTags.AccessionNumber, out var value).Should().BeTrue();

        value.Should().Be("ABC123");
    }

    [Fact]
    public void TryGet_UShort_ShouldReturnValue_FromMutableDataset()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.Rows, (ushort)512);

        dataset.TryGet(DicomTags.Rows, out ushort value).Should().BeTrue();

        value.Should().Be(512);
    }

    [Fact]
    public void TryGet_Double_ShouldReturnValue_FromMutableDataset()
    {
        var dataset = new DicomDataset();
        // Use a tag that's actually FD VM=1
        var fdTag = new DicomTag<double>(0x0018, 0x9087, DicomVR.FD, [], DicomVM.VM_1, "DiffusionBValue", "Diffusion b-value");
        dataset.Set(fdTag, 3.14);

        dataset.TryGet(fdTag, out double value).Should().BeTrue();

        value.Should().BeApproximately(3.14, 0.001);
    }

    [Fact]
    public void TryGet_Float_ShouldReturnValue_FromMutableDataset()
    {
        var dataset = new DicomDataset();
        // Use a tag that's actually FL VM=1
        var flTag = new DicomTag<float>(0x0018, 0x1151, DicomVR.FL, [], DicomVM.VM_1, "XRayTubeCurrent", "X-Ray Tube Current");
        dataset.Set(flTag, 2.5f);

        dataset.TryGet(flTag, out float value).Should().BeTrue();

        value.Should().BeApproximately(2.5f, 0.001f);
    }

    [Fact]
    public void Set_ShouldOverwriteExistingValue()
    {
        var dataset = new DicomDataset();
        dataset.Set(DicomTags.AccessionNumber, "OLD");
        dataset.Set(DicomTags.AccessionNumber, "NEW");

        dataset.TryGet(DicomTags.AccessionNumber, out var value).Should().BeTrue();

        value.Should().Be("NEW");
    }

    [Fact]
    public void TryGet_String_ShouldReturnFalse_WhenTagMissing_FromMutableDataset()
    {
        var dataset = new DicomDataset();

        dataset.TryGet(DicomTags.AccessionNumber, out string? _).Should().BeFalse();
    }

    [Fact]
    public void Set_PersonName_ShouldRoundTrip()
    {
        var dataset = new DicomDataset();
        var name = new PersonName("DOE", "JOHN", null, null, null, null, null, null, null);
        dataset.Set(DicomTags.PatientName, name);

        dataset.TryGet(DicomTags.PatientName, out PersonName? value).Should().BeTrue();

        value!.Value.FamilyName.Should().Be("DOE");
        value.Value.GivenName.Should().Be("JOHN");
    }

    [Fact]
    public void TypeSafety_TagTypeParameter_PreventsWrongAccess()
    {
        // This test verifies that DicomTags constants have the correct type parameter.
        // If these assignments compile, type safety is working correctly.
        DicomTag<string> stringTag = DicomTags.AccessionNumber;
        DicomTag<ushort> ushortTag = DicomTags.Rows;
        DicomTag<DateOnly> dateTag = DicomTags.StudyDate;
        DicomTag<PersonName> pnTag = DicomTags.PatientName;

        // All should be assignable to base DicomTag
        DicomTag baseTag1 = stringTag;
        DicomTag baseTag2 = ushortTag;
        DicomTag baseTag3 = dateTag;
        DicomTag baseTag4 = pnTag;

        // Verify they carry the expected metadata
        stringTag.ValueRepresentation.Should().Be(DicomVR.SH);
        ushortTag.ValueRepresentation.Should().Be(DicomVR.US);
        dateTag.ValueRepresentation.Should().Be(DicomVR.DA);
        pnTag.ValueRepresentation.Should().Be(DicomVR.PN);
    }
}
