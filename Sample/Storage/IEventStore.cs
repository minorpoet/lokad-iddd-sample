#region (c) 2012-2012 Lokad - New BSD License

// Copyright (c) Lokad 2012-2012, http://www.lokad.com
// This code is released as Open Source under the terms of the New BSD Licence

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sample.Storage
{
    /// <summary>
    ///  事件存储用于序列化处理和强类型，处理对象是领域事件并且负责其和字节数组的相互转换； 而IAppendOnlyStore的实现类则用于对不同的存储引擎的底层访问，处理对象是字节数组
    /// </summary>
    public interface IEventStore
    {

        // 加载事件流中的所有事件 
        EventStream LoadEventStream(IIdentity id);

        // 加载事件流中的某个子集
        EventStream LoadEventStream(IIdentity id, long skipEvents, int maxCount);

        /// <summary>
        /// Appends events to server stream for the provided identity.
        /// </summary>
        /// <param name="id">identity to append to.</param>
        /// <param name="expectedVersion">The expected version (specify -1 to append anyway).</param>
        /// <param name="events">The events to append.</param>
        /// <exception cref="OptimisticConcurrencyException">when new events were added to server
        /// since <paramref name="expectedVersion"/>
        /// </exception>
        void AppendToStream(IIdentity id, long expectedVersion, ICollection<IEvent> events);
    }

    public class EventStream
    {
        // version of the event stream returned
        public long Version;
        // all events in the stream
        public List<IEvent> Events = new List<IEvent>();
    }

    /// <summary>
    /// Is thrown by event store if there were changes since our last version
    /// </summary>
    [Serializable]
    public class OptimisticConcurrencyException : Exception
    {
        public long ActualVersion { get; private set; }
        public long ExpectedVersion { get; private set; }
        public IIdentity Id { get; private set; }
        public IList<IEvent> ActualEvents { get; private set; }

        OptimisticConcurrencyException(string message, long actualVersion, long expectedVersion, IIdentity id,
            IList<IEvent> serverEvents)
            : base(message)
        {
            ActualVersion = actualVersion;
            ExpectedVersion = expectedVersion;
            Id = id;
            ActualEvents = serverEvents;
        }

        public static OptimisticConcurrencyException Create(long actual, long expected, IIdentity id,
            IList<IEvent> serverEvents)
        {
            var message = string.Format("Expected v{0} but found v{1} in stream '{2}'", expected, actual, id);
            return new OptimisticConcurrencyException(message, actual, expected, id, serverEvents);
        }

        protected OptimisticConcurrencyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Is supposed to be thrown by the client code, when it fails to resolve concurrency problem
    /// </summary>
    [Serializable]
    public class RealConcurrencyException : Exception
    {
        public RealConcurrencyException() { }
        public RealConcurrencyException(string message) : base(message) { }
        public RealConcurrencyException(string message, Exception inner) : base(message, inner) { }

        protected RealConcurrencyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}