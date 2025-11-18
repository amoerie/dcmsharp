namespace DcmSharp;

public interface IDicomItem
{
    ushort Group { get; }
    ushort Element { get; }
}

public abstract record DicomItem<TValue>(ushort Group, ushort Element, TValue Value) : IDicomItem
{
}

public sealed record DicomApplicationEntity(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomAgeString(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomAttributeTag(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomCodeString(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomDate(ushort Group, ushort Element, DateOnly[] Value) : DicomItem<DateOnly[]>(Group, Element, Value);
public sealed record DicomDecimalString(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomDateTime(ushort Group, ushort Element, DateTime[] Value) : DicomItem<DateTime[]>(Group, Element, Value);
public sealed record DicomFloatingPointDouble(ushort Group, ushort Element, double[] Value) : DicomItem<double[]>(Group, Element, Value);
public sealed record DicomFloatingPointSingle(ushort Group, ushort Element, float[] Value) : DicomItem<float[]>(Group, Element, Value);
public sealed record DicomIntegerString(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomLongString(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomLongText(ushort Group, ushort Element, string Value) : DicomItem<string>(Group, Element, Value);
public sealed record DicomOtherByte(ushort Group, ushort Element, Memory<byte>[] Value) : DicomItem<Memory<byte>[]>(Group, Element, Value);
public sealed record DicomOtherDouble(ushort Group, ushort Element, Memory<double>[] Value) : DicomItem<Memory<double>[]>(Group, Element, Value);
public sealed record DicomOtherFloat(ushort Group, ushort Element, Memory<float>[] Value) : DicomItem<Memory<float>[]>(Group, Element, Value);
public sealed record DicomOtherLong(ushort Group, ushort Element, Memory<uint>[] Value) : DicomItem<Memory<uint>[]>(Group, Element, Value);
public sealed record DicomOtherWord(ushort Group, ushort Element, Memory<ushort>[] Value) : DicomItem<Memory<ushort>[]>(Group, Element, Value);
public sealed record DicomPersonName(ushort Group, ushort Element, PersonName[] Value) : DicomItem<PersonName[]>(Group, Element, Value);
public sealed record DicomShortString(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomSignedLong(ushort Group, ushort Element, int[] Value) : DicomItem<int[]>(Group, Element, Value);
public sealed record DicomSignedVeryLong(ushort Group, ushort Element, long[] Value) : DicomItem<long[]>(Group, Element, Value);
public sealed record DicomSignedShort(ushort Group, ushort Element, short[] Value) : DicomItem<short[]>(Group, Element, Value);
public sealed record DicomShortText(ushort Group, ushort Element, string Value) : DicomItem<string>(Group, Element, Value);
public sealed record DicomTime(ushort Group, ushort Element, TimeOnly[] Value) : DicomItem<TimeOnly[]>(Group, Element, Value);
public sealed record DicomUnlimitedCharacters(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomUniqueIdentifier(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomUnsignedLong(ushort Group, ushort Element, uint[] Value) : DicomItem<uint[]>(Group, Element, Value);
public sealed record DicomUnknown(ushort Group, ushort Element, Memory<byte>[] Value) : DicomItem<Memory<byte>[]>(Group, Element, Value);
public sealed record DicomUniversalResource(ushort Group, ushort Element, string Value) : DicomItem<string>(Group, Element, Value);
public sealed record DicomUnlimitedText(ushort Group, ushort Element, string Value) : DicomItem<string>(Group, Element, Value);
public sealed record DicomUrl(ushort Group, ushort Element, string[] Value) : DicomItem<string[]>(Group, Element, Value);
public sealed record DicomUnsignedShort(ushort Group, ushort Element, ushort[] Value) : DicomItem<ushort[]>(Group, Element, Value);
