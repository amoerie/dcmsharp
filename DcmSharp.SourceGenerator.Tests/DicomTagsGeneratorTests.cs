using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace DcmSharp.SourceGenerator.Tests;

public class TestsForDicomTagsGenerator
{
    [Fact]
    public async Task ShouldProduceDicomTags()
    {
        // Arrange
        var testCode = """
                       public sealed record DicomTag(ushort Group, ushort Element, DicomVR[] VRs, DicomVM VM, string Keyword, string Name);

                       // <summary>
                       /// DICOM value multiplicity
                       /// </summary>
                       public enum DicomVM
                       {
                           /** 1*/
                           VM_1,

                           /** 1-2*/
                           VM_1_2,

                           /** 1-3*/
                           VM_1_3,

                           /** 1-8*/
                           VM_1_8,

                           /** 1-32*/
                           VM_1_32,

                           /** 1-99*/
                           VM_1_99,

                           /** 1-n*/
                           VM_1_n,

                           /** 2*/
                           VM_2,

                           /** 2-n*/
                           VM_2_n,

                           /** 2-2n*/
                           VM_2_2n,

                           /** 3*/
                           VM_3,

                           /** 3-n*/
                           VM_3_n,

                           /** 3-3n*/
                           VM_3_3n,

                           /** 4*/
                           VM_4,

                           /** 4-n*/
                           VM_4_n,

                           /** 6*/
                           VM_6,

                           /** 6-n*/
                           VM_6_n,

                           /** 9 */
                           VM_9,

                           /** 16*/
                           VM_16,

                           /** Unknown */
                           VM_UN
                       }

                       /// <summary>
                       /// Represents a DICOM value representation (VR).
                       /// </summary>
                       public enum DicomVR : byte
                       {
                           /// <summary>Application Entity</summary>
                           AE,

                           /// <summary>Age String</summary>
                           AS,

                           /// <summary>Attribute Tag</summary>
                           AT,

                           /// <summary>Code String</summary>
                           CS,

                           /// <summary>Date</summary>
                           DA,

                           /// <summary>Decimal String</summary>
                           DS,

                           /// <summary>DateTime</summary>
                           DT,

                           /// <summary>Floating Point Single</summary>
                           FL,

                           /// <summary>Floating Point Double</summary>
                           FD,

                           /// <summary>Integer String</summary>
                           IS,

                           /// <summary>Long String</summary>
                           LO,

                           /// <summary>Long Text</summary>
                           LT,

                           /// <summary>Other Byte</summary>
                           OB,

                           /// <summary>Other Double</summary>
                           OD,

                           /// <summary>Other Float</summary>
                           OF,

                           /// <summary>Other Long</summary>
                           OL,

                           /// <summary>Other Word</summary>
                           OW,

                           /// <summary>Other Very Long</summary>
                           OV,

                           /// <summary>Person Name</summary>
                           PN,

                           /// <summary>Short String</summary>
                           SH,

                           /// <summary>Signed Long</summary>
                           SL,

                           /// <summary>Sequence of Items</summary>
                           SQ,

                           /// <summary>Signed Short</summary>
                           SS,

                           /// <summary>Short Text</summary>
                           ST,

                           /// <summary>Signed Very Long</summary>
                           SV,

                           /// <summary>Time</summary>
                           TM,

                           /// <summary>Unlimited Characters</summary>
                           UC,

                           /// <summary>Unique Identifier (UID)</summary>
                           UI,

                           /// <summary>Unsigned Long</summary>
                           UL,

                           /// <summary>Unknown</summary>
                           UN,

                           /// <summary>URI/URL</summary>
                           UR,

                           /// <summary>Unsigned Short</summary>
                           US,

                           /// <summary>Unlimited Text,</summary>
                           UT,

                           /// <summary>Unsigned Very Long</summary>
                           UV
                       }
                       """;

        var test = new CSharpSourceGeneratorTest<DicomTagsGenerator, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            TestCode = testCode,
            TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck,
        };

        // Act
        await test.RunAsync();
    }
}
