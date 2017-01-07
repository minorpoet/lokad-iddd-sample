using System.Collections.Generic;

namespace Sample.Domain
{
    /// <summary>
    /// This is the state of the customer aggregate.
    /// It can be mutated only by passing events to it.
    /// </summary>
    public class CustomerState
    {
        public string Name { get; private set; }
        public bool Created { get; private set; }
        public CustomerId Id { get; private set; }
        public bool ConsumptionLocked { get; private set; }
        public bool ManualBilling { get; private set; }
        public Currency Currency { get; private set; }
        public CurrencyAmount Balance { get; private set; }

        public int MaxTransactionId { get; private set; }

        public CustomerState(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        public void When(CustomerLocked e)
        {
            ConsumptionLocked = true;
        }

        public void When(CustomerPaymentAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }
        public void When(CustomerChargeAdded e)
        {
            Balance = e.NewBalance;
            MaxTransactionId = e.Transaction;
        }

        public void When(CustomerCreated e)
        {
            Created = true;
            Name = e.Name;
            Id = e.Id;
            Currency = e.Currency;
            Balance = new CurrencyAmount(0, e.Currency);
        }

        public void When(CustomerRenamed e)
        {
            Name = e.Name;
        }

        public void Mutate(IEvent e)
        {
            // .NET magic to call one of the 'When' handlers with 
            // matching signature 
            this.When((dynamic)e);
        }
    }
}