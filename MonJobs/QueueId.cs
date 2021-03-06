using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace MonJobs
{
    /// <summary>
    /// The id that segments one queue from another.  Access to these queues should be controlled via authorization checks.
    /// </summary>
    [ImmutableObject(true)]
    public sealed class QueueId : IComparable<QueueId>, IConvertible<string>
    {

        private readonly string _sourceString;

        private QueueId(string queueId)
        {
            Contract.Requires(queueId != null);
            Contract.Requires(queueId != string.Empty);
            Contract.Ensures(_sourceString != null);
            Contract.Ensures(_sourceString != string.Empty);
            if (queueId == null) throw new ArgumentNullException(nameof(queueId));
            _sourceString = queueId;
        }

        #region Equality

        private sealed class SourceStringEqualityComparer : IEqualityComparer<QueueId>
        {
            public bool Equals(QueueId x, QueueId y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x._sourceString, y._sourceString);
            }

            public int GetHashCode(QueueId obj)
            {
                return obj._sourceString?.GetHashCode() ?? 0;
            }
        }

        public static IEqualityComparer<QueueId> SourceStringComparer { get; } = new SourceStringEqualityComparer();

        private bool Equals(QueueId other)
        {
            return string.Equals(_sourceString, other._sourceString);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is QueueId && Equals((QueueId)obj);
        }

        public override int GetHashCode()
        {
            return _sourceString?.GetHashCode() ?? 0;
        }

        #endregion

        #region Parsing

        public static QueueId Parse(string queueId)
        {
            QueueId result;
            if (!QueueId.TryParse(queueId, out result))
            {
                throw new ArgumentException("Invalid QueueId: " + queueId, nameof(queueId));
            };
            return result;
        }

        public static bool TryParse(string queueId, out QueueId result)
        {
            if (queueId == null)
            {
                result = QueueId.Empty();
                return false;
            }
            result = new QueueId(queueId);
            return true;
        }

        #endregion

        public static QueueId Empty()
        {
            return new QueueId(string.Empty);
        }

        public string ToValueType()
        {
            return _sourceString;
        }

        public int CompareTo(QueueId other)
        {
            return string.CompareOrdinal(this._sourceString, other._sourceString);
        }

        public override string ToString()
        {
            return _sourceString;
        }

        public static implicit operator string(QueueId queueId)
        {
            return queueId?._sourceString ?? string.Empty;
        }

    }
}