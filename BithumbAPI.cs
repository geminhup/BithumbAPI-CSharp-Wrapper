using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography;

namespace Bithumb
{
    public class BithumbAPI 
    {
        public MyHttpClient httpClient;
        public BithumbAPI(string accessKey, string secretKey)
        {
            this.httpClient = new MyHttpClient(accessKey, secretKey);
        }

        public class MyHttpClient : HttpClient
        {
            private string _accessKey;
            public string AccessKey
            {
                get { return _accessKey; }
                set { _accessKey = value; }
            }
            private string _secretKey;
            public string SecretKey
            {
                get { return _secretKey; }
                set { _secretKey = value; }
            }
            public MyHttpClient(string accessKey, string secretKey)
            {
                if (string.IsNullOrWhiteSpace(accessKey)) { throw new ArgumentNullException("accessKey"); }
                if (string.IsNullOrWhiteSpace(secretKey)) { throw new ArgumentNullException("secretKey"); }
                _accessKey = accessKey;
                _secretKey = secretKey;
            }
        }


        // Public API
        public string GetTicker_Public(string currency)
        {
            string url = "https://api.bithumb.com/public/ticker/";
            return CallAPI_Public(url + currency, null);
        }
        public string GetOrderbook(string currency, int group_orders, int count)
        {
            string url = "https://api.bithumb.com/public/orderbook/";
            return CallAPI_Public(url + currency, new NameValueCollection { { "group_orders", group_orders.ToString() }, { "count", count.ToString() } });
        }
        public string GetTransactionHistory(string currency, int cont_no = 0, int count = 20)
        {
            string url = "https://api.bithumb.com/public/transaction_history/";
            return CallAPI_Public(url + currency, new NameValueCollection { { "cont_no", cont_no.ToString() }, { "count", count.ToString() } });
        }


        // Private API
        public string GetAccount(string currency)
        {
            string url = "https://api.bithumb.com/info/account";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "currency", currency } }, HttpMethod.Post);
        }
        public string GetBalance(string currency)
        {
            string url = "https://api.bithumb.com/info/balance";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "currency", currency } }, HttpMethod.Post);
        }
        public string GetWalletAddress(string currency)
        {
            string url = "https://api.bithumb.com/info/wallet_address";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "currency", currency } }, HttpMethod.Post);
        }
        public string GetTicker_Private(string order_currency)
        {
            string url = "https://api.bithumb.com/info/ticker";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "order_currency", order_currency }, { "payment_currency", "KRW" } }, HttpMethod.Post);
        }
        public string GetOrders(string order_id, BithumbOrderType type, int count, DateTime after, string currency)
        {
            string url = "https://api.bithumb.com/info/orders";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "order_id", order_id }, { "type", type.ToString() }, { "after", new DateTimeOffset(after).ToUnixTimeSeconds().ToString() }, { "currency", currency } }, HttpMethod.Post);
        }
        public string Getuser_transactions(int offset, int count, BithumbTransactionType searchGb, string currency)
        {
            string url = "https://api.bithumb.com/info/user_transactions";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "offset", offset.ToString() }, { "count", count.ToString() }, { "searchGb", ((int)searchGb).ToString() }, { "currency", currency } }, HttpMethod.Post);
        }
        public string PlaceOrder(string order_currency, float units, int price, BithumbOrderType type)
        {
            string url = "https://api.bithumb.com/trade/place";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "order_currency", order_currency }, { "payment_currency", "KRW" }, { "units", units.ToString() }, { "price", price.ToString() }, { "type", type.ToString() } }, HttpMethod.Post);
        }
        public string GetOrderDetail(string order_id, BithumbOrderType type, string currency)
        {
            string url = "https://api.bithumb.com/info/order_detail";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "order_id", order_id }, { "type", type.ToString() }, { "currency", currency } }, HttpMethod.Post);
        }
        public string CancelOrder(string order_id, BithumbOrderType type, string currency)
        {
            string url = "https://api.bithumb.com/trade/cancel";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "order_id", order_id }, { "type", type.ToString() }, { "currency", currency } }, HttpMethod.Post);
        }
        public string WithdrawCrypto(float units, string address, string destination, string currency)
        {
            string url = "https://api.bithumb.com/trade/btc_withdrawal";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "units", units.ToString() }, { "address", address }, { "destination", destination }, { "currency", currency } }, HttpMethod.Post);
        }
        public string GetKRWDepositInfo()
        {
            string url = "https://api.bithumb.com/trade/krw_deposit";
            return CallAPI_Private_NoParam(url, HttpMethod.Post);
        }
        public string WithdrawKRW(string bank, string account, int amount)
        {
            string url = "https://api.bithumb.com/trade/krw_withdrawal";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "bank", bank }, { "account", account }, { "price", amount.ToString() } }, HttpMethod.Post);
        }
        public string PlaceOrder_MarketBuy(float units, string currency)
        {
            string url = "https://api.bithumb.com/trade/market_buy";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "units", units.ToString() }, { "currency", currency } }, HttpMethod.Post);
        }
        public string PlaceOrder_MarketSell(float units, string currency)
        {
            string url = "https://api.bithumb.com/trade/market_sell";
            return CallAPI_Private_WithParam(url, new NameValueCollection { { "units", units.ToString() }, { "currency", currency } }, HttpMethod.Post);
        }




        private string CallAPI_Public(string url, NameValueCollection nvc)
        {
            var response = httpClient.GetAsync(url + "?" + ToQueryString(nvc)).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        private string CallAPI_Private_WithParam(string url, NameValueCollection nvc, HttpMethod httpMethod)
        {
            var requestMessage = BuildHttpRequestMessage(url, httpMethod, nvc);
            var response = httpClient.SendAsync(requestMessage).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        private string CallAPI_Private_NoParam(string url, HttpMethod httpMethod)
        {
            var requestMessage = BuildHttpRequestMessage(url, httpMethod);
            var response = httpClient.SendAsync(requestMessage).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        private HttpRequestMessage BuildHttpRequestMessage(string url, HttpMethod httpMethod, NameValueCollection nvc = null)
        {
            string endPoint = url.Replace("https://api.bithumb.com", "");
            string postData = (nvc == null) ? "" : ToQueryString(nvc) + "&endpoint=" + Uri.EscapeDataString(endPoint); ;
            long nonce = MicroSecTime();

            var requestMessage = new HttpRequestMessage(httpMethod, new Uri(url));
            requestMessage.Headers.Add("Api-Key", this.httpClient.AccessKey);
            requestMessage.Headers.Add("Api-Sign", Convert.ToBase64String(StringToByte(Hash_HMAC(httpClient.SecretKey, endPoint + (char)0 + postData + (char)0 + nonce.ToString()))));
            requestMessage.Headers.Add("Api-Nonce", nonce.ToString());
            requestMessage.Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");

            return requestMessage;
        }
        private string ToQueryString(NameValueCollection nvc)
        {
            if (nvc == null) { return ""; }
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", key, value))
                .ToArray();
            return string.Join("&", array);
        }
        private long MicroSecTime()
        {
            long nEpochTicks = new DateTime(1970, 1, 1).Ticks;
            DateTime DateTimeNow = DateTime.UtcNow;
            long nNowTicks = DateTimeNow.Ticks;
            long nowMiliseconds = DateTimeNow.Millisecond;
            long nUnixTimeStamp = ((nNowTicks - nEpochTicks) / TimeSpan.TicksPerSecond);
            string sNonce = nUnixTimeStamp.ToString() + nowMiliseconds.ToString("D03");
            return (Convert.ToInt64(sNonce));
        }
        private string Hash_HMAC(string sKey, string sData)
        {
            byte[] rgbyKey = Encoding.UTF8.GetBytes(sKey);
            using (var hmacsha512 = new HMACSHA512(rgbyKey))
            {
                hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(sData));
                return (ByteToString(hmacsha512.Hash));
            }
        }
        private string ByteToString(byte[] rgbyBuff)
        {
            string sHexStr = "";
            for (int nCnt = 0; nCnt < rgbyBuff.Length; nCnt++)
            {
                sHexStr += rgbyBuff[nCnt].ToString("x2"); // Hex format
            }
            return (sHexStr);
        }
        private byte[] StringToByte(string sStr)
        {
            return Encoding.UTF8.GetBytes(sStr);
        }


        public enum BithumbOrderType { bid, ask }
        public enum BithumbTransactionType { all = 0, bid_executed = 1, ask_executed = 2, withdrawal_processing = 3, deposit = 4, withdrawal = 5, KRW_deposit = 9 }
    }
}
