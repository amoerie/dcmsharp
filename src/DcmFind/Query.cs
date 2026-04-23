using System.Globalization;
using System.Text.RegularExpressions;
using DcmSharp;

namespace DcmFind
{
    public interface IQuery
    {
        bool Matches<TDataset>(TDataset dicomDataset) where TDataset : IDicomDataset;
    }

    public class EqualsQuery : IQuery
    {
        private readonly DicomTag _dicomTag;
        private readonly Regex _regex;

        public EqualsQuery(DicomTag dicomTag, string value)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));
            _dicomTag = dicomTag;
            var pattern = $"^{Regex.Escape(value).Replace("%", ".*")}$";
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        public bool Matches<TDataset>(TDataset dicomDataset) where TDataset : IDicomDataset
        {
            return _regex.IsMatch(dicomDataset.TryGetString(_dicomTag, out string? value) ? value : "");
        }
    }

    public class NotEqualsQuery : IQuery
    {
        private readonly DicomTag _dicomTag;
        private readonly Regex _regex;

        public NotEqualsQuery(DicomTag dicomTag, string value)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));
            _dicomTag = dicomTag;
            var pattern = $"^{Regex.Escape(value).Replace("%", ".*")}$";
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        public bool Matches<TDataset>(TDataset dicomDataset) where TDataset : IDicomDataset
        {
            return !_regex.IsMatch(dicomDataset.TryGetString(_dicomTag, out string? value) ? value : "");
        }
    }

    public class LowerThanQuery : IQuery
    {
        private enum ComparisonKind { DateTime, Long, String }

        private readonly DicomTag _dicomTag;
        private readonly bool _inclusive;
        private readonly ComparisonKind _kind;
        private readonly DateTime _dateTimeValue;
        private readonly long _longValue;
        private readonly string? _stringValue;

        public LowerThanQuery(DicomTag dicomTag, string value, bool inclusive)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));
            _dicomTag = dicomTag;
            _inclusive = inclusive;

            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTimeValue))
            {
                _kind = ComparisonKind.DateTime;
            }
            else if (long.TryParse(value, out _longValue))
            {
                _kind = ComparisonKind.Long;
            }
            else
            {
                _kind = ComparisonKind.String;
                _stringValue = value;
            }
        }

        public bool Matches<TDataset>(TDataset dicomDataset) where TDataset : IDicomDataset
        {
            switch (_kind)
            {
                case ComparisonKind.DateTime:
                    return dicomDataset.TryGetDateTime(_dicomTag, out var dt) && (_inclusive ? dt <= _dateTimeValue : dt < _dateTimeValue);
                case ComparisonKind.Long:
                    return dicomDataset.TryGetLong(_dicomTag, out var l) && (_inclusive ? l <= _longValue : l < _longValue);
                default:
                    return dicomDataset.TryGetString(_dicomTag, out var s) && (_inclusive ? string.CompareOrdinal(s, _stringValue) <= 0 : string.CompareOrdinal(s, _stringValue) < 0);
            }
        }
    }

    public class GreaterThanQuery : IQuery
    {
        private enum ComparisonKind { DateTime, Long, String }

        private readonly DicomTag _dicomTag;
        private readonly bool _inclusive;
        private readonly ComparisonKind _kind;
        private readonly DateTime _dateTimeValue;
        private readonly long _longValue;
        private readonly string? _stringValue;

        public GreaterThanQuery(DicomTag dicomTag, string value, bool inclusive)
        {
            if (dicomTag == null) throw new ArgumentNullException(nameof(dicomTag));
            if (value == null) throw new ArgumentNullException(nameof(value));
            _dicomTag = dicomTag;
            _inclusive = inclusive;

            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTimeValue))
            {
                _kind = ComparisonKind.DateTime;
            }
            else if (long.TryParse(value, out _longValue))
            {
                _kind = ComparisonKind.Long;
            }
            else
            {
                _kind = ComparisonKind.String;
                _stringValue = value;
            }
        }

        public bool Matches<TDataset>(TDataset dicomDataset) where TDataset : IDicomDataset
        {
            switch (_kind)
            {
                case ComparisonKind.DateTime:
                    return dicomDataset.TryGetDateTime(_dicomTag, out var dt) && (_inclusive ? dt >= _dateTimeValue : dt > _dateTimeValue);
                case ComparisonKind.Long:
                    return dicomDataset.TryGetLong(_dicomTag, out var l) && (_inclusive ? l >= _longValue : l > _longValue);
                default:
                    return dicomDataset.TryGetString(_dicomTag, out var s) && (_inclusive ? string.CompareOrdinal(s, _stringValue) >= 0 : string.CompareOrdinal(s, _stringValue) > 0);
            }
        }
    }

    public class ContainsTagQuery: IQuery
    {
        private readonly DicomTag _dicomTag;

        public ContainsTagQuery(DicomTag dicomTag)
        {
            _dicomTag = dicomTag;
        }

        public bool Matches<TDataset>(TDataset dicomDataset) where TDataset : IDicomDataset
        {
            return dicomDataset.TryGetMemory(_dicomTag, out _, out _);
        }
    }
}
