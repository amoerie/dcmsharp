using DcmAnonymize.Names;
using FellowOakDicom;

namespace DcmAnonymize.Study;

public record AnonymizedStudy(
    DicomUID StudyInstanceUID,
    string AccessionNumber,
    DateTime StudyDateTime,
    RandomName RequestingPhysician,
    RandomName PerformingPhysician,
    string StudyId,
    string InstitutionName
);
