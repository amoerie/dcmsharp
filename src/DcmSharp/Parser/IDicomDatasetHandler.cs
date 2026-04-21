using System.Text;

namespace DcmSharp.Parser;

public interface IDicomDatasetHandler
{
    void OnItem(ushort group, ushort element, DicomVR vr, ReadOnlyMemory<byte> rawValue);
    void OnSequenceStart(ushort group, ushort element);
    void OnSequenceItemStart();
    void OnSequenceItemEnd();
    void OnSequenceEnd();
    void OnFragmentsStart(ushort group, ushort element, DicomVR vr);
    void OnFragment(ReadOnlyMemory<byte> fragment);
    void OnFragmentsEnd();
    void OnEncoding(Encoding encoding);
}
