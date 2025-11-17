using System.Diagnostics.CodeAnalysis;

namespace DcmSharp.Parser;

public static class DicomVRParser
{
    /// <summary>
    /// Try to get VR for given UTF8 encoded string value.
    /// </summary>
    /// <param name="b1">Byte 1 of UTF8 encoded byte representation of VR</param>
    /// <param name="b2">Byte 1 of UTF8 encoded byte representation of VR</param>
    /// <param name="result">VR code</param>
    /// <returns>true if VR was successfully parsed, false otherwise</returns>
    public static bool TryParse(byte b1, byte b2, [NotNullWhen(true)] out DicomVR? result)
    {
        result = TryParse(b1, b2, out bool valid);
        return valid;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Precompiled switch table for performance reasons")]
    private static DicomVR? TryParse(byte b1, byte b2, out bool valid)
    {
        valid = true;

        // This is a precompiled switch table
        switch (b1)
        {
            case 65:
                switch (b2)
                {
                    case 69: return DicomVR.AE;
                    case 83: return DicomVR.AS;
                    case 84: return DicomVR.AT;
                }
                break;
            case 67:
                switch (b2)
                {
                    case 83: return DicomVR.CS;
                }
                break;
            case 68:
                switch (b2)
                {
                    case 65: return DicomVR.DA;
                    case 83: return DicomVR.DS;
                    case 84: return DicomVR.DT;
                }
                break;
            case 70:
                switch (b2)
                {
                    case 68: return DicomVR.FD;
                    case 76: return DicomVR.FL;
                }
                break;
            case 73:
                switch (b2)
                {
                    case 83: return DicomVR.IS;
                }
                break;
            case 76:
                switch (b2)
                {
                    case 79: return DicomVR.LO;
                    case 84: return DicomVR.LT;
                }
                break;
            case 79:
                switch (b2)
                {
                    case 66: return DicomVR.OB;
                    case 68: return DicomVR.OD;
                    case 70: return DicomVR.OF;
                    case 76: return DicomVR.OL;
                    case 86: return DicomVR.OV;
                    case 87: return DicomVR.OW;
                }
                break;
            case 80:
                switch (b2)
                {
                    case 78: return DicomVR.PN;
                }
                break;
            case 83:
                switch (b2)
                {
                    case 72: return DicomVR.SH;
                    case 76: return DicomVR.SL;
                    case 81: return DicomVR.SQ;
                    case 83: return DicomVR.SS;
                    case 84: return DicomVR.ST;
                    case 86: return DicomVR.SV;
                }
                break;
            case 84:
                switch (b2)
                {
                    case 77: return DicomVR.TM;
                }
                break;
            case 85:
                switch (b2)
                {
                    case 67: return DicomVR.UC;
                    case 73: return DicomVR.UI;
                    case 76: return DicomVR.UL;
                    case 78: return DicomVR.UN;
                    case 82: return DicomVR.UR;
                    case 83: return DicomVR.US;
                    case 84: return DicomVR.UT;
                    case 86: return DicomVR.UV;
                }
                break;
        }

        valid = false;
        return null;
    }
}
