using System;
using NUnit.Framework;
using Sample;
using Sample.Domain;

namespace UnitTests
{
    public class when_create_customer : customer_spec
    {
        [Test]
        public void given_new_customer_and_bonus()
        {
            // dependencies
            var pricing = new TestPricingService(17m);

            // call
            When = customer => customer.Create(
                new CustomerId(1), 
                "Lokad", 
                Currency.Eur, pricing, new DateTime(2012, 07, 16));

            // expectations
            Expect = new IEvent[]
                {
                    new CustomerCreated()
                        {
                            Currency = Currency.Eur,
                            Id = new CustomerId(1),
                            Name = "Lokad",
                            Created = new DateTime(2012, 07, 16)
                        },
                    new CustomerPaymentAdded()
                        {
                            Id = new CustomerId(1),
                            NewBalance = 17m.Eur(),
                            Transaction = 1,
                            Payment = 17m.Eur(),
                            PaymentName = "Welcome bonus",
                            TimeUtc = new DateTime(2012, 07, 16)
                        }
                };
        }

        [Test]
        public void given_new_customer_and_no_bonus()
        {
            // dependencies
            var pricing = new TestPricingService(0);

            // call
            When = customer => customer.Create(
                new CustomerId(1),
                "Lokad",
                Currency.Eur, pricing, new DateTime(2012, 07, 16));

            // expectations
            Expect = new IEvent[]
                {
                    new CustomerCreated()
                        {
                            Currency = Currency.Eur,
                            Id = new CustomerId(1),
                            Name = "Lokad",
                            Created = new DateTime(2012, 07, 16)
                        },
                };
        }
    }

    /// <summary>
    /// This simple class allows our tests to stay clean and decopled
    /// from complicated mock frameworks
    /// </summary>
    class TestPricingService : IPricingService
    {
        readonly decimal _substitute;

        public TestPricingService(decimal substitute)
        {
            _substitute = substitute;
        }

        public CurrencyAmount GetOverdraftThreshold(Currency currency)
        {
            return new CurrencyAmount(_substitute, currency);
        }

        public CurrencyAmount GetWelcomeBonus(Currency currency)
        {
            return new CurrencyAmount(_substitute, currency);
        }
    }
}
