using System;
using System.Collections.Generic;
using Allied.RealTime.Example.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Allied.RealTime.Example.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        // Idempotency cache (demonstration purposes only...
        // a real implementation would place this information in
        // a durable store, like a database table).
        private const string CacheKey = "IdempotencyCache";
        private readonly IMemoryCache _cache;

        public TransactionsController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        // GET api/transactions
        [HttpPost("balance")]
        public ActionResult<BalanceResponse> Post([FromBody] BalanceRequest balanceRequest)
        {
            // TODO: query the core for a balance. Here, we'll just simulate
            // that by returning a random value between $0.00 and $999.99:
            Random rand = new Random(DateTime.Now.Millisecond);
            decimal balance = (decimal)(rand.Next(99999) / 100.0);

            BalanceResponse response = new BalanceResponse
            {
                success = true,
                balance = balance,
                routingTransitNumber = balanceRequest.routingTransitNumber,
                accountNumber = balanceRequest.accountNumber
            };
            return response;
        }

        // POST api/transactions
        [HttpPost("transaction")]
        public ActionResult<TransactionResponse> Post([FromBody] TransactionRequest transactionRequest)
        {
            // Idempotency check...if we've seen this transaction before, we'll
            // return the prior response.
            TransactionResponse response = GetCachedTransactionResponse(transactionRequest.transactionKey);
            if (response != null)
                return response;

            // For demonstration purposes, we'll automatically fail any GL transaction...a live
            // system would access the banking core and generate a response based on that
            // access.
            try
            {
                // NOTE: not all banking cores handle memo posting. This sample
                // real time banking interface is for illustration purposes. If your
                // core doesn't handle memo posts or GL transactions, by all means
                // create a real time interface that works well with your core.
                // Just keep in mind your API consumer likely cannot deal with
                // core-specific concepts and constructs (e.g.: CIF information),
                // so your real time interface (this code) must translate that 
                // information into common structures (e.g.: strings, integers...).

                response = new TransactionResponse
                {
                    transactionKey = transactionRequest.transactionKey,
                    description = "Transaction successful"
                };

                if (transactionRequest.transactionType == TransactionRequest.TransactionTypes.DebitGL ||
                    transactionRequest.transactionType == TransactionRequest.TransactionTypes.CreditGL)
                {
                    // NOTE: in failure case, HTTP RFP says to return 200 with failure
                    // indication in response body. However, if you can't imagine not
                    // returning a failure status, return some 4xx value (but probably
                    // not 401/403 unless those are the cause of the failure). We will
                    // check both cases.
                    response.success = false;
                    response.description = response.errorMessage = "Failure: cannot transact GL accounts";
                }

                if (transactionRequest.transactionType == TransactionRequest.TransactionTypes.CreditDDA)
                {
                    // Reversal
                    response.description = "Transaction reversal successful";
                }

                // With a memo post we need a check number (at least for this sample
                // application).
                if (transactionRequest.transactionType == TransactionRequest.TransactionTypes.MemoPost)
                {
                    if (transactionRequest.checkNumber.HasValue)
                    {
                        response.description = "Memo post successful";
                    }
                    else
                    {
                        response.success = false;
                        response.description = response.errorMessage = "Failure: must provide a check number when issuing a memo post";
                    }
                }
            }
            catch (Exception ex)
            {
                // NOTE: you may not want to return the actual exception...
                // by all means return what makes sense and is secure.
                response = new TransactionResponse(ex);
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            // New (successful) transaction, so add to our cache...we won't
            // add errored transactions since the client could (should)
            // correct the condition and re-submit. In that case we don't
            // want to return the prior error.
            if (response.success)
                AddTransactionResponseToCache(response);
            return response;
        }

        #region Idempotency caching

        private TransactionResponse GetCachedTransactionResponse(string transactionKey)
        {
            Dictionary<string, TransactionResponse> idempotencyCache;
            if (!_cache.TryGetValue(CacheKey, out idempotencyCache))
            {
                idempotencyCache = new Dictionary<string, TransactionResponse>();
                _cache.Set<Dictionary<string, TransactionResponse>>(CacheKey, idempotencyCache);
            }

            TransactionResponse response = null;
            if (idempotencyCache.ContainsKey(transactionKey))
                response = idempotencyCache[transactionKey]; // must check for key, else MissingKeyException
            return response;
        }

        private void AddTransactionResponseToCache(TransactionResponse transactionResponse)
        {
            Dictionary<string, TransactionResponse> idempotencyCache;
            if (!_cache.TryGetValue(CacheKey, out idempotencyCache))
            {
                idempotencyCache = new Dictionary<string, TransactionResponse>();
                _cache.Set<Dictionary<string, TransactionResponse>>(CacheKey, idempotencyCache);
            }

            idempotencyCache[transactionResponse.transactionKey] = transactionResponse;
        }

        #endregion
    }
}
