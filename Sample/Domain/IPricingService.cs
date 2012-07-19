using System;

namespace Sample.Domain
{
    /// <summary>
    /// <para>
    /// This is a sample of domain service, that will be injected by application service
    /// into aggregate for providing this specific behavior as <see cref="PricingService"/>. 
    /// </para>
    /// <para>
    /// During tests, this service will be replaced by test implementation of the same 
    /// interface (no, you don't need mocking framework, just see the unit tests project).
    /// </para>
    /// </summary>
    public interface IPricingService
    {
        CurrencyAmount GetOverdraftThreshold(Currency currency);
        CurrencyAmount GetWelcomeBonus(Currency currency);
    }

    /// <summary>
    /// <para>This is a sample implementation of a <em>Domain Service</em> for pricing. 
    /// Such services can be more complex than that (i.e.: providing access to payment
    /// gateways, cloud fabrics, remote catalogues, expert systems or other 3rd party
    /// services 
    /// </para> 
    /// </summary>
    public sealed class PricingService : IPricingService
    {
        public CurrencyAmount GetOverdraftThreshold(Currency currency)
        {
            if (currency == Currency.Eur)
                return (-10m).Eur();
            throw new NotImplementedException("TODO: implement other currencies");
        }

        public CurrencyAmount GetWelcomeBonus(Currency currency)
        {
            if (currency == Currency.Eur)
                return 15m.Eur();
            throw new NotImplementedException("TODO: implement other currencies");
        }
    }

}