using DcmAnonymize.Blanking;
using DcmAnonymize.Instance;
using DcmAnonymize.Names;
using DcmAnonymize.Order;
using DcmAnonymize.Patient;
using DcmAnonymize.Recursive;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.NativeCodec;
using FluentAssertions;
using Xunit;

namespace DcmAnonymize.Tests;

[Collection("DcmAnonymize")]
public class TestsForDicomAnonymizer
{
    public static readonly TheoryData<DicomTag> TagsToRemove = KnownDicomTags.TagsToRemove
        .Where(tag => tag != DicomTag.Unknown)
        .Aggregate(new TheoryData<DicomTag>(), (data, tag) =>
        {
            data.Add(tag);
            return data;
        });

    public static readonly TheoryData<DicomTag> UIDTagsToAnonymize = KnownDicomTags.UIDTagsToAnonymize.Aggregate(
        new TheoryData<DicomTag>(), (data, tag) =>
        {
            data.Add(tag);
            return data;
        });

    public static readonly TheoryData<DicomTag> TagsToClean = KnownDicomTags.TagsToClean.Aggregate(
        new TheoryData<DicomTag>(), (data, tag) =>
        {
            data.Add(tag);
            return data;
        });

    private readonly DicomAnonymizer _anonymizer;
    private readonly AnonymizationOptions _options;

    public TestsForDicomAnonymizer()
    {
        var randomNameGenerator = new RandomNameGenerator();
        var nationalNumberGenerator = new NationalNumberGenerator();
        var dummyValueFiller = new DicomTagCleaner(randomNameGenerator);
        _anonymizer = new DicomAnonymizer(
            new PatientAnonymizer(randomNameGenerator, nationalNumberGenerator),
            new StudyAnonymizer(randomNameGenerator),
            new SeriesAnonymizer(),
            new InstanceAnonymizer(),
            new RecursiveAnonymizer(dummyValueFiller),
            new OrderAnonymizer(),
            new BlankingAnonymizer());
        _options = new AnonymizationOptions(new List<RectangleToBlank>());


        // Configure Fellow Oak DICOM
        new DicomSetupBuilder()
            .RegisterServices(s =>
            {
                s.AddImageManager<ImageSharpImageManager>();
                s.AddTranscoderManager<NativeTranscoderManager>();
            })
            .Build();
    }

    [Fact]
    public async Task ShouldBeAbleToAnonymizeEmptyDataSet()
    {
        // Arrange
        var metaInfo = new DicomFileMetaInformation();
        var dicomDataSet = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        dicomDataSet.Validate();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet, _options);

        // Assert
        dicomDataSet.Contains(DicomTag.PatientName).Should().BeTrue();
        dicomDataSet.GetSingleValue<string>(DicomTag.PatientName).Should().NotBe("Bar^Foo");
        dicomDataSet.GetSingleValue<string>(DicomTag.StudyInstanceUID).Should().NotBe("1");
        dicomDataSet.GetSingleValue<string>(DicomTag.SeriesInstanceUID).Should().NotBe("1.1");
        dicomDataSet.GetSingleValue<string>(DicomTag.SOPInstanceUID).Should().NotBe("1.1.1");
    }

    [Fact]
    public async Task ShouldAnonymizeSamePatient()
    {
        // Arrange
        var dicomDataSet1 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        var metaInfo1 = new DicomFileMetaInformation();
        var dicomDataSet2 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "2" },
            { DicomTag.SeriesInstanceUID, "2.2" },
            { DicomTag.SOPInstanceUID, "2.2.2" },
        };
        var metaInfo2 = new DicomFileMetaInformation();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1, _options);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2, _options);

        // Assert
        var patientName1 = dicomDataSet1.GetSingleValue<string>(DicomTag.PatientName);
        var patientName2 = dicomDataSet2.GetSingleValue<string>(DicomTag.PatientName);
        patientName1.Should().NotBe("Bar^Foo");
        patientName2.Should().NotBe("Bar^Foo");
        patientName1.Should().Be(patientName2);
    }

    [Fact]
    public async Task ShouldAnonymizeSameOrder()
    {
        // Arrange
        var metaInfo1 = new DicomFileMetaInformation();
        var metaInfo2 = new DicomFileMetaInformation();
        var metaInfo3 = new DicomFileMetaInformation();
        var dicomDataSet1 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
            { DicomTag.PlacerOrderNumberImagingServiceRequest, "ORDER1" },
        };
        var dicomDataSet2 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "2" },
            { DicomTag.SeriesInstanceUID, "2.1" },
            { DicomTag.SOPInstanceUID, "2.1.1" },
            { DicomTag.PlacerOrderNumberImagingServiceRequest, "ORDER1" },
        };
        var dicomDataSet3 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Fuu" },
            { DicomTag.StudyInstanceUID, "3" },
            { DicomTag.SeriesInstanceUID, "3.1" },
            { DicomTag.SOPInstanceUID, "3.1.1" },
            { DicomTag.PlacerOrderNumberImagingServiceRequest, "ORDER2" },
        };

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1, _options);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2, _options);
        await _anonymizer.AnonymizeAsync(metaInfo3, dicomDataSet3, _options);

        // Assert
        var orderNumber1 = dicomDataSet1.GetSingleValue<string>(DicomTag.PlacerOrderNumberImagingServiceRequest);
        var orderNumber2 = dicomDataSet2.GetSingleValue<string>(DicomTag.PlacerOrderNumberImagingServiceRequest);
        var orderNumber3 = dicomDataSet3.GetSingleValue<string>(DicomTag.PlacerOrderNumberImagingServiceRequest);
        orderNumber1.Should().NotBe("ORDER1");
        orderNumber2.Should().NotBe("ORDER1");
        orderNumber3.Should().NotBe("ORDER2");
        orderNumber1.Should().Be(orderNumber2);
        orderNumber3.Should().NotBe(orderNumber1);
    }

    [Fact]
    public async Task ShouldAnonymizeSameStudy()
    {
        // Arrange
        var dicomDataSet1 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        var metaInfo1 = new DicomFileMetaInformation();
        var dicomDataSet2 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.2" },
            { DicomTag.SOPInstanceUID, "1.2.2" },
        };
        var metaInfo2 = new DicomFileMetaInformation();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1, _options);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2, _options);

        // Assert
        var studyInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.StudyInstanceUID);
        var studyInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.StudyInstanceUID);
        studyInstanceUID1.Should().NotBe("1");
        studyInstanceUID2.Should().NotBe("1");
        studyInstanceUID1.Should().Be(studyInstanceUID2);
    }

    [Fact]
    public async Task ShouldAnonymizeSameSeries()
    {
        // Arrange
        var dicomDataSet1 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        var metaInfo1 = new DicomFileMetaInformation();
        var dicomDataSet2 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.2" },
        };
        var metaInfo2 = new DicomFileMetaInformation();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1, _options);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2, _options);

        // Assert
        var seriesInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
        var seriesInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
        seriesInstanceUID1.Should().NotBe("1");
        seriesInstanceUID2.Should().NotBe("1");
        seriesInstanceUID1.Should().Be(seriesInstanceUID2);
    }

    [Fact]
    public async Task ShouldAnonymizeSameInstances()
    {
        // Arrange
        var dicomDataSet1 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        var metaInfo1 = new DicomFileMetaInformation();
        var dicomDataSet2 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        var metaInfo2 = new DicomFileMetaInformation();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo1, dicomDataSet1, _options);
        await _anonymizer.AnonymizeAsync(metaInfo2, dicomDataSet2, _options);

        // Assert
        var sopInstanceUID1 = dicomDataSet1.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        var sopInstanceUID2 = dicomDataSet2.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        sopInstanceUID1.Should().NotBe("1.1.1");
        sopInstanceUID2.Should().NotBe("1.1.1");
        sopInstanceUID1.Should().Be(sopInstanceUID2);
    }

    [Fact]
    public async Task ShouldBeAbleToAnonymizeSampleDicomFile()
    {
        // Arrange
        var sampleDicomFile = await DicomFile.OpenAsync("./TestData/SampleDicomFile.dcm");
        var metaInfo = sampleDicomFile.FileMetaInfo;
        var dicomDataSet = sampleDicomFile.Dataset;

        dicomDataSet.Validate();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet, _options);

        // Assert
        dicomDataSet.Validate();
    }

    [Fact]
    public async Task ShouldRetainReferences()
    {
        // Arrange
        const string file1 = "./TestData/Im1-removedByRJ1-113037.dcm";
        const string file2 = "./TestData/RJ1-113037.dcm";
        var dicomFile1 = await DicomFile.OpenAsync(file1);
        var dicomFile2 = await DicomFile.OpenAsync(file2);
        var sopInstanceUID = dicomFile1.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        var currentRequestedProcedureEvidenceSequence =
            dicomFile2.Dataset.GetSequence(DicomTag.CurrentRequestedProcedureEvidenceSequence);
        var referencedSeriesSequence = currentRequestedProcedureEvidenceSequence.Single()
            .GetSequence(DicomTag.ReferencedSeriesSequence);
        var referencedSopSequence = referencedSeriesSequence.Single().GetSequence(DicomTag.ReferencedSOPSequence);
        var firstReferencedSopInstanceUID =
            referencedSopSequence.First().GetSingleValue<string>(DicomTag.ReferencedSOPInstanceUID);

        // Ensure our test data is correct
        sopInstanceUID.Should().NotBeNullOrEmpty();
        firstReferencedSopInstanceUID.Should().NotBeNullOrEmpty();
        firstReferencedSopInstanceUID.Should().Be(sopInstanceUID);

        // Act
        await _anonymizer.AnonymizeAsync(dicomFile1.FileMetaInfo, dicomFile1.Dataset, _options);
        await _anonymizer.AnonymizeAsync(dicomFile2.FileMetaInfo, dicomFile2.Dataset, _options);

        // Assert
        var sopInstanceUID2 = dicomFile1.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
        var currentRequestedProcedureEvidenceSequence2 =
            dicomFile2.Dataset.GetSequence(DicomTag.CurrentRequestedProcedureEvidenceSequence);
        var referencedSeriesSequence2 = currentRequestedProcedureEvidenceSequence2.Single()
            .GetSequence(DicomTag.ReferencedSeriesSequence);
        var referencedSopSequence2 = referencedSeriesSequence2.Single().GetSequence(DicomTag.ReferencedSOPSequence);
        var firstReferencedSopInstanceUID2 =
            referencedSopSequence2.First().GetSingleValue<string>(DicomTag.ReferencedSOPInstanceUID);

        // Ensure data is still present
        sopInstanceUID2.Should().NotBeNullOrEmpty();
        firstReferencedSopInstanceUID2.Should().NotBeNullOrEmpty();

        // Ensure data is anonymized
        sopInstanceUID.Should().NotBe(sopInstanceUID2);
        firstReferencedSopInstanceUID.Should().NotBe(firstReferencedSopInstanceUID2);

        // Ensure referential integrity is retained
        sopInstanceUID2.Should().Be(firstReferencedSopInstanceUID2);
    }


    [Theory]
    [MemberData(nameof(TagsToRemove))]
    public async Task ShouldRemoveKnownTags(DicomTag tag)
    {
        // Arrange
        var metaInfo = new DicomFileMetaInformation();
        var dataSet = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" },
        };
        var valueRepresentation = DicomDictionary.Default[tag].ValueRepresentations.First();
        if (valueRepresentation == DicomVR.SQ)
        {
            dataSet.Add(new DicomSequence(tag, new DicomDataset()));
        }
        else if (valueRepresentation == DicomVR.OB)
        {
            dataSet.Add(tag, Array.Empty<byte>());
        }
        else
        {
            dataSet.Add(tag, string.Empty);
        }

        var originalDataset = dataSet.Clone();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dataSet, _options);

        // Assert
        originalDataset.Contains(tag).Should().BeTrue();
        dataSet.Contains(tag).Should().BeFalse();
        dataSet.Contains(DicomTag.PatientName).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(UIDTagsToAnonymize))]
    public async Task ShouldAnonymizeKnownUIDTags(DicomTag tag)
    {
        // Arrange
        var metaInfo1 = new DicomFileMetaInformation();
        var metaInfo2 = new DicomFileMetaInformation();
        var originalUID = DicomUIDGenerator.GenerateDerivedFromUUID();
        var dataSet1 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" }
        };
        var dataSet2 = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" }
        };
        dataSet1.AddOrUpdate(tag, originalUID);
        dataSet2.AddOrUpdate(tag, originalUID);
        var originalDataset1 = dataSet1.Clone();
        var originalDataset2 = dataSet2.Clone();

        // Act
        await Task.WhenAll(
            Task.Run(() => _anonymizer.AnonymizeAsync(metaInfo1, dataSet1, _options)),
            Task.Run(() => _anonymizer.AnonymizeAsync(metaInfo2, dataSet2, _options))
        );

        // Assert
        originalDataset1.GetSingleValue<DicomUID>(tag).Should().Be(originalUID);
        originalDataset2.GetSingleValue<DicomUID>(tag).Should().Be(originalUID);
        dataSet1.GetSingleValue<DicomUID>(tag).Should().NotBe(originalUID);
        dataSet2.GetSingleValue<DicomUID>(tag).Should().NotBe(originalUID);
        dataSet1.GetSingleValue<DicomUID>(tag).Should().Be(dataSet2.GetSingleValue<DicomUID>(tag));
    }

    [Theory]
    [MemberData(nameof(TagsToClean))]
    public async Task ShouldCleanKnownTags(DicomTag tag)
    {
        // Arrange
        var metaInfo = new DicomFileMetaInformation();
        var dataSet = new DicomDataset
        {
            { DicomTag.PatientName, "Bar^Foo" },
            { DicomTag.StudyInstanceUID, "1" },
            { DicomTag.SeriesInstanceUID, "1.1" },
            { DicomTag.SOPInstanceUID, "1.1.1" }
        };
        var valueRepresentation = DicomDictionary.Default[tag].ValueRepresentations.First();
        if (valueRepresentation == DicomVR.SQ)
        {
            dataSet.Add(new DicomSequence(tag, new DicomDataset()));
        }
        else if (valueRepresentation == DicomVR.OB)
        {
            dataSet.Add(tag, new byte[] { 1, 2, 3 });
        }
        else if (valueRepresentation == DicomVR.DA
                 || valueRepresentation == DicomVR.DT
                 || valueRepresentation == DicomVR.TM)
        {
            dataSet.Add(tag, DateTime.Now);
        }
        else if (valueRepresentation == DicomVR.AS)
        {
            dataSet.Add(tag, "065Y");
        }
        else if (valueRepresentation == DicomVR.UN)
        {
            dataSet.Add(tag, (byte)1);
        }
        else
        {
            dataSet.Add(tag, "123");
        }

        var originalDataset = dataSet.Clone();

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dataSet, _options);

        // Assert
        originalDataset.Contains(tag).Should().BeTrue();
        dataSet.Contains(tag).Should().BeTrue();
        if (valueRepresentation == DicomVR.SQ)
        {
            dataSet.GetSequence(tag).ToList().Should().HaveCount(1);
        }
        else if (valueRepresentation == DicomVR.OB || valueRepresentation == DicomVR.UN)
        {
            dataSet.GetValues<byte>(tag).Should().BeEmpty();
        }
        else if (valueRepresentation == DicomVR.PN)
        {
            var personName = dataSet.GetDicomItem<DicomPersonName>(tag);
            personName.First.Should().NotBeEmpty();
            personName.Last.Should().NotBeEmpty();
        }
        else
        {
            dataSet.GetString(tag).Should().Be(string.Empty);
        }
    }

    [Fact]
    public async Task ShouldBeAbleToBlankRectangles()
    {
        // Arrange
        var sampleDicomFile = await DicomFile.OpenAsync("./TestData/SampleDicomFile.dcm");
        var metaInfo = sampleDicomFile.FileMetaInfo;
        var dicomDataSet = sampleDicomFile.Dataset;

        dicomDataSet.Validate();

        var options = _options with
        {
            RectanglesToBlank = new List<RectangleToBlank>
            {
                new RectangleToBlank(300, 300, 1000, 1000)
            }
        };

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet, options);

        // Assert
        dicomDataSet.Validate();
        var dicomImage = new DicomImage(dicomDataSet);
        using var image = dicomImage.RenderImage();
        var color = image.GetPixel(700, 700);
        color.Should().Be(Color32.Black);
    }

    [Fact]
    public async Task ShouldBeAbleToBlankRectanglesOfMultiFrame()
    {
        // Arrange
        var sampleDicomFile = await DicomFile.OpenAsync(@"TestData/SampleMultiFrame.dcm");
        var metaInfo = sampleDicomFile.FileMetaInfo;
        var dicomDataSet = sampleDicomFile.Dataset;

        dicomDataSet.Validate();

        var options = _options with
        {
            RectanglesToBlank = new List<RectangleToBlank>
            {
                new RectangleToBlank(50, 50, 350, 350)
            }
        };

        // Act
        await _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet, options);

        // Assert
        dicomDataSet.Validate();
        var dicomImage = new DicomImage(dicomDataSet);
        for (var frame = 0; frame < dicomImage.NumberOfFrames; frame++)
        {
            using var image = dicomImage.RenderImage(frame);
            var color = image.GetPixel(100, 100);
            color.Should().Be(Color32.Black);
        }
    }

    [Fact]
    public async Task ShouldBeAbleToAnonymizeMoreThan1000Studies()
    {
        // Arrange
        var randomNameGenerator = new RandomNameGenerator();
        for (int i = 0; i < 10_000; i++)
        {
            var patientName = randomNameGenerator.GenerateRandomName();
            var originalStudyInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
            var originalAccessionNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            var originalSeriesInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
            var originalSopInstanceUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID;
            var dicomDataSet = new DicomDataset
            {
                new DicomPersonName(DicomTag.PatientName, patientName.LastName, patientName.FirstName),
                { DicomTag.AccessionNumber, originalAccessionNumber },
                { DicomTag.StudyInstanceUID, originalStudyInstanceUID },
                { DicomTag.SeriesInstanceUID, originalSeriesInstanceUID },
                { DicomTag.SOPInstanceUID, originalSopInstanceUID },
            };
            var metaInfo = new DicomFileMetaInformation();

            // Act
            await _anonymizer.AnonymizeAsync(metaInfo, dicomDataSet, _options);

            // Assert
            var anonymizedStudyInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            var anonymizedSeriesInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            var anonymizedSopInstanceUID = dicomDataSet.GetSingleValue<string>(DicomTag.SOPInstanceUID);
            var anonymizedAccessionNumber = dicomDataSet.GetSingleValue<string>(DicomTag.AccessionNumber);
            anonymizedStudyInstanceUID.Should().NotBe(originalStudyInstanceUID);
            anonymizedSeriesInstanceUID.Should().NotBe(originalSeriesInstanceUID);
            anonymizedSopInstanceUID.Should().NotBe(originalSopInstanceUID);
            anonymizedAccessionNumber.Should().NotBe(originalAccessionNumber);
        }
    }
}
