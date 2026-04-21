using System.Globalization;
using System.Text.RegularExpressions;
using DcmSharp;

namespace DcmFind
{
    public interface IQuery
    {
        bool Matches(IDicomDataset dicomDataset);
    }

    public class EqualsQuery : IQuery
    {
        private readonly Func<IDicomDataset, bool> _predicate;

        public EqualsQuery(DicomTag dicomTag, string value)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var pattern = $"^{Regex.Escape(value).Replace("%", ".*")}$";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
            _predicate = dicomDataset => regex.IsMatch(dicomDataset.TryGetString(dicomTag, out string? dicomTagValue) ? dicomTagValue : "");
        }

        public bool Matches(IDicomDataset dicomDataset)
        {
            return _predicate(dicomDataset);
        }
    }

    public class NotEqualsQuery : IQuery
    {
        private readonly Func<IDicomDataset, bool> _predicate;

        public NotEqualsQuery(DicomTag dicomTag, string value)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));
            var pattern = $"^{Regex.Escape(value).Replace("%", ".*")}$";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
            _predicate = dicomDataset => !regex.IsMatch(dicomDataset.TryGetString(dicomTag, out string? dicomTagValue) ? dicomTagValue : "");
        }

        public bool Matches(IDicomDataset dicomDataset)
        {
            return _predicate(dicomDataset);
        }
    }

    public class LowerThanQuery : IQuery
    {
        private readonly Func<IDicomDataset, bool> _predicate;

        public LowerThanQuery(DicomTag dicomTag, string value, bool inclusive)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
            {
                if (inclusive)
                {
                    _predicate = dicomDataset => dicomDataset.TryGetDateTime(dicomTag, out var dateTimeDicomTagValue)
                                                 && dateTimeDicomTagValue <= dateTimeValue;
                }
                else
                {
                    _predicate = dicomDataset => dicomDataset.TryGetDateTime(dicomTag, out var dateTimeDicomTagValue)
                                                 && dateTimeDicomTagValue < dateTimeValue;
                }
            }
            else if (long.TryParse(value, out var longValue))
            {
                if (inclusive)
                {
                    _predicate = dicomDataset => dicomDataset.TryGetLong(dicomTag, out var longDicomTagValue)
                                                 && longDicomTagValue <= longValue;
                }
                else
                {
                    _predicate = dicomDataset => dicomDataset.TryGetLong(dicomTag, out var longDicomTagValue)
                                                 && longDicomTagValue < longValue;
                }
            }
            else
            {
                if (inclusive)
                {
                    _predicate = dicomDataset => dicomDataset.TryGetString(dicomTag, out var dicomTagValue) && string.CompareOrdinal(dicomTagValue, value) <= 0;
                }
                else
                {
                    _predicate = dicomDataset => dicomDataset.TryGetString(dicomTag, out var dicomTagValue) && string.CompareOrdinal(dicomTagValue, value) < 0;
                }
            }
        }

        public bool Matches(IDicomDataset dicomDataset)
        {
            return _predicate(dicomDataset);
        }
    }

    public class GreaterThanQuery : IQuery
    {
        private readonly Func<IDicomDataset, bool> _predicate;

        public GreaterThanQuery(DicomTag dicomTag, string value, bool inclusive)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
            {
                if (inclusive)
                {
                    _predicate = dicomDataset => dicomDataset.TryGetDateTime(dicomTag, out var dateTimeDicomTagValue)
                                                 && dateTimeDicomTagValue >= dateTimeValue;
                }
                else
                {
                    _predicate = dicomDataset => dicomDataset.TryGetDateTime(dicomTag, out var dateTimeDicomTagValue)
                                                 && dateTimeDicomTagValue > dateTimeValue;
                }
            }
            else if (long.TryParse(value, out var longValue))
            {
                if (inclusive)
                {
                    _predicate = dicomDataset => dicomDataset.TryGetLong(dicomTag, out var longDicomTagValue)
                                                 && longDicomTagValue >= longValue;
                }
                else
                {
                    _predicate = dicomDataset => dicomDataset.TryGetLong(dicomTag, out var longDicomTagValue)
                                                 && longDicomTagValue > longValue;
                }
            }
            else
            {
                if (inclusive)
                {
                    _predicate = dicomDataset => dicomDataset.TryGetString(dicomTag, out var dicomTagValue) && string.CompareOrdinal(dicomTagValue, value) >= 0;
                }
                else
                {
                    _predicate = dicomDataset => dicomDataset.TryGetString(dicomTag, out var dicomTagValue) && string.CompareOrdinal(dicomTagValue, value) > 0;
                }
            }
        }

        public bool Matches(IDicomDataset dicomDataset)
        {
            return _predicate(dicomDataset);
        }
    }

    public class ContainsTagQuery: IQuery
    {
        private readonly DicomTag _dicomTag;

        public ContainsTagQuery(DicomTag dicomTag)
        {
            _dicomTag = dicomTag;
        }

        public bool Matches(IDicomDataset dicomDataset)
        {
            return dicomDataset.TryGetMemory(_dicomTag, out _, out _);
        }
    }
}
