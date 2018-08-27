using System;

// Bithumb official API reference : https://www.bithumb.com/u1/US127

namespace Bithumb
{
    class Program
    {
        static void Main(string[] args)
        {
            BithumbAPI B = new BithumbAPI("AccessKey", "SecretKey");

            // Public API
            Console.WriteLine(B.GetTicker_Public("ETH"));
            Console.WriteLine(B.GetOrderbook("ETH", 1, 3));
            Console.WriteLine(B.GetTransactionHistory("ETH", cont_no: 2, count: 2));

            // Private API
            Console.WriteLine(B.GetAccount("ICX"));
            Console.WriteLine(B.GetBalance("all"));
            Console.WriteLine(B.GetWalletAddress("ETH"));
            Console.WriteLine(B.GetTicker_Private("ETH"));
            Console.WriteLine(B.GetOrders("Order ID", BithumbAPI.BithumbOrderType.bid, 20, DateTime.Now, "ETH"));
            Console.WriteLine(B.Getuser_transactions(0, 20, BithumbAPI.BithumbTransactionType.all, "ETH"));
            Console.WriteLine(B.PlaceOrder("ETH", 0.01f, 200000, BithumbAPI.BithumbOrderType.bid));
            Console.WriteLine(B.GetOrderDetail("Order ID", BithumbAPI.BithumbOrderType.bid, "ETH"));
            Console.WriteLine(B.CancelOrder("Order ID", BithumbAPI.BithumbOrderType.bid, "ETH"));
            Console.WriteLine(B.WithdrawCrypto(0.01f, "Sender's wallet address", "Receiver's wallet address", "ETH"));
            Console.WriteLine(B.GetKRWDepositInfo());
            Console.WriteLine(B.WithdrawKRW("Bank name", "Accnount number", 1000));
            Console.WriteLine(B.PlaceOrder_MarketBuy(0.01f, "ETH"));
            Console.WriteLine(B.PlaceOrder_MarketSell(0.01f, "ETH"));

            Console.ReadLine();
        }
    }
}
