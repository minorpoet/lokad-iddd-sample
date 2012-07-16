using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Sample;
using Sample.Domain;

namespace UnitTests
{
    public abstract class customer_spec
    {
        public IList<IEvent> Given;
        public Expression<Action<Customer>> When;
        public IList<IEvent> Expect { set { AssertCustomer(Given, When, value); } }

        [SetUp]
        public void Setup()
        {
            Given = new List<IEvent>();
            When = null;
        }

        public static void AssertCustomer(ICollection<IEvent> given, Expression<Action<Customer>> when, ICollection<IEvent> then)
        {
            foreach (var @event in then)
            {
                Console.WriteLine("Given: " + @event);
            }

            var customer = new Customer(given);

            PrintWhen(when);
            when.Compile()(customer);

            foreach (var @event in then)
            {
                Console.WriteLine("Expect: " + @event);
            }

            AssertEquality(then.ToArray(), customer.Changes.ToArray());
        }

        static void PrintWhen(Expression<Action<Customer>> when)
        {
            // this output can be made prettier, if we 
            // either use expression helpers (see Greg Young's Simple testing for that)
            // or use commands at the application level (see tests in Lokad.CQRS for that)
            Console.WriteLine();
            Console.WriteLine("When: " + when);
            Console.WriteLine();
        }


        static void AssertEquality(IEvent[] expected, IEvent[] actual)
        {
            var actualBytes = ToBytes(actual);
            var expectedBytes = ToBytes(expected);
            bool areEqual = actualBytes.SequenceEqual(expectedBytes);
            if (areEqual) return;
            CollectionAssert.AreEqual(
                expected.Select(s => s.ToString()).ToArray(),
                actual.Select(s => s.ToString()).ToArray());

            CollectionAssert.AreEqual(expectedBytes, actualBytes, "Expected events differ from actual, but differences are not represented in ToString()");

        }

        static byte[] ToBytes(IEvent[] actual)
        {
            // this helper class transforms events to their binary representation
            BinaryFormatter formatter = new BinaryFormatter();
            using (var mem = new MemoryStream())
            {
                formatter.Serialize(mem, actual);
                return mem.ToArray();
            }
        }
    }
}