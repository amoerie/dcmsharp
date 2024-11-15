namespace DcmParser;

/// <summary>
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
}
