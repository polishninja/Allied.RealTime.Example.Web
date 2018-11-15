using System;

namespace Allied.RealTime.Example.Web.Models
{
    public class TransactionRequest
    {
        // NOTE: the transaction types are representative...you could
        // have separate endpoints for each type, different types,
        // or whatever matches your core system. Typically a
        // reversal must be accompanied by the same transaction
        // ID as the original debit.
        public enum TransactionTypes
        {
            DebitGL = 1, // funds come from GL
            DebitDDA = 2, // funds come from customer DDA
            CreditGL = 3, // reversal to GL
            CreditDDA = 4, // reversal to DDA
            MemoPost = 5 // customer account memo post
        }

        public Int64 transactionId { get; set; }

        // NOTE: the transactionKey is an idempotency key (see
        // https://en.wikipedia.org/wiki/Idempotence). This is
        // *required* and must be honored by your real time
        // system. See below for more explanation.
        public string transactionKey { get; set; }

        public decimal amount { get; set; }

        public decimal? feeAmount { get; set; }

        public string routingTransitNumber { get; set; }

        public string accountNumber { get; set; }

        public int? checkNumber { get; set; }

        public string payeeName { get; set; }

        public string description { get; set; }

        public TransactionTypes transactionType { get; set; }

        // TODO: place any additional required properties
        // below.
    }
}

/*
 * Idempotency:
 *
 * Networks are inherently faulty, and any system that relies on
 * network transmissions should be coded to anticipate failure
 * and respond accordingly. Consider this situation:
 *
 * You pay a bill online. The online bill service makes a RESTful
 * call to the financial institution's real time interface (the
 * prescriptive code you see here). But the network request
 * times out, leaving the online bill service to wonder if the
 * transaction was accomplished. Perhaps it was, perhaps it
 * wasn't. Since the online bill service doesn't know for sure,
 * the online bill service will continue to make the transaction
 * until it is successful.
 *
 * However, if the original transaction was successful, this will
 * result in a double posting and a very unhappy customer (imagine
 * making the same $500 car payment twice and you'll understand).
 *
 * To prevent this, your system should be idempotent, which is to
 * say that if you receive a transaction using a transaction key
 * you've already processed, you should return the same response
 * you returned for the first transaction request. DO NOT
 * re-process the transaction, even if it initially failed.
 *
 * This is typically accomplished using a simple cache, such as a
 * database table. By definition the idempotency key must be
 * unique (plan for 128 characters minimum), so use that as the
 * primary key into a table that stores the original response
 * (perhaps JSON encoded...just return the JSON you stored
 * previously).
 *
 * If you do not find the idempotency key in the table,
 * process the transaction with the core as per normal.
 *
 */
