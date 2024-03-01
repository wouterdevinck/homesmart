using System;
using System.Threading;

namespace Home.Core.Models {

    public enum TwinEventType {
        Reported, // The device reported a value.
        Desired,  // A request was made to the device to change a value.
        Timer     // Periodic check whether the desired value was reported back.
    }

    public enum TwinState {
        Empty,    // Uninitialized, no values set yet.
        Settled,  // Desired and reported are equal.
        Timeout,  // Desired and reported are not equal, allowed timeout is not over yet.
        Expired   // Desired and reported are not equal, allowed timeout has expired.
    }

    public enum TwinReportResult {
        NoChange, // Nothing was changed, reported and desired are equal and the same value was reported again.
        Update,   // Regular update, both reported and desired were updated to a new value.
        Settled,  // The desired value was reported back before the timeout expired (or after it expired, but before it was reverted).
        Ignored   // A value not equal to the desired value was reported back during the timeout period, it was ignored for now and stored in case revert is called later.
    }

    public class Twin<T> {
        
        private readonly TimeSpan _allowedTimeout;
        private readonly Func<T, T, bool> _equal;
        private readonly Mutex _mutex;

        private T _desired;
        private T _reported;
        private T _revertValue;

        private bool _initialized;

        public Twin(TimeSpan allowedTimeout, Func<T, T, bool> equal) {
            _allowedTimeout = allowedTimeout;
            _equal = equal;
            _initialized = false;
            _mutex = new Mutex();
        }

        public T Reported {
            get => _reported;
            private set {
                _mutex.WaitOne();
                _reported = value;
                _revertValue = value;
                _initialized = true;
                _mutex.ReleaseMutex();
            }
        }
        
        public T Desired {
            get => _desired;
            set {
                _mutex.WaitOne();
                _desired = value;
                _initialized = true;
                DesiredTime = DateTime.Now;
                _mutex.ReleaseMutex();
            }
        }

        public DateTime DesiredTime { get; private set; }

        public TwinState State {
            get {
                if (!_initialized) {
                    return TwinState.Empty;
                }
                if (_equal(Reported, Desired)) {
                    return TwinState.Settled;
                } 
                if (DateTime.Now - DesiredTime <= _allowedTimeout) {
                    return TwinState.Timeout;
                } 
                return TwinState.Expired;
            }
        }

        public void Revert() {
            Desired = _revertValue;
        }

        public TwinReportResult Report(T update) {
            if (State == TwinState.Empty) {
                Reported = update;
                Desired = update;
                return TwinReportResult.Update;
            }
            if (State == TwinState.Settled) {
                if (_equal(update, Desired)) {
                    return TwinReportResult.NoChange;
                }
                Reported = update;
                Desired = update;
                return TwinReportResult.Update;
            }
            if (_equal(update, Desired)) {
                Reported = update;
                return TwinReportResult.Settled;
            }
            _mutex.WaitOne();
            _revertValue = update;
            _mutex.ReleaseMutex();
            return TwinReportResult.Ignored;
        }

    }

}
