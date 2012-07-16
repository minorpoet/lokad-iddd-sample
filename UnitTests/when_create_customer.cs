using System;
using NUnit.Framework;
using Sample.Domain;

namespace Sample
{
    public class when_create_customer : customer_specs
    {
        [Test]
        public void given_new_customer_and_bonus()
        {
            // dependencies
            var pricing = new TestPricingService(17m);

            // call
            var dateTime = new DateTime(2012, 07, 16);
            When = customer => customer.Create(
                new CustomerId(1), 
                "Lokad", 
                Currency.Eur, pricing, dateTime);

            // expectations
            Then = new IEvent[]
                {
                    new CustomerCreated
                        {
                            Currency = Currency.Eur,
                            Id = new CustomerId(1),
                            Name = "Lokad",
                            Created = dateTime
                        },
                    new CustomerPaymentAdded()
                        {
                            Id = new CustomerId(1),
                            NewBalance = 17m.Eur(),
                            Transaction = 1,
                            Payment = 17m.Eur(),
                            PaymentName = "Welcome bonus",
                            TimeUtc = dateTime
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
                Currency.Rur, pricing, new DateTime(2012, 07, 16));

            // expectations
            Then = new IEvent[]
                {
                    new CustomerCreated()
                        {
                            Currency = Currency.Rur,
                            Id = new CustomerId(1),
                            Name = "Lokad",
                            Created = new DateTime(2012, 07, 16)
                        },
                };
        }
    }
}
