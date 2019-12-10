## Allied ASPNET Core Prescriptive Real Time Payment Collection Interface

### Purpose

Provide a working example of the financial side real time interface. The solution includes working (sample) interfaces for performing a balance check, transacting payments, reversing payments, and memo posting.

This sample code is provided "as is" and is not meant to represent all or any specific financial institution's processing system. It is intended to instruct and provide guidance regarding general concepts involved with real time bill payment transactions.

### General Concepts

When you sit down to pay your bills, there are two primary steps you take:
1. You check your balance to make sure you have sufficient funds
2. You issue the payment

Here, Allied follows the same process. Before any funds are moved via a real time process, we first check the balance on the account. If the returned balance is sufficient to cover the payment, we then process the payment transaction. If the funds are insufficient, we place the payment on hold and notify the financial institution and then the customer. 

Therefore, you'll find two primary API entry points in this sample:
* Balance check
* Transaction
  * Debit/Credit to GL
  * Debit/Credit to DDA
  * Memo Post

When we collect funds to cover the payment, we're going to debit the customer account. If for some reason we need to reverse the payment (e.g.: customer cancels), we credit the customer account (a "reversal"). Under the specific condition that the customer is paying a payee by check and the check is drawn from the customer's account (not a central account), we issue a "memo post". A memo post is simply a credit hold on the customer account so that they cannot withdraw the funds necessary to cover the incoming check. Once the check clears, the memo post is withdrawn by the financial institution (_not_ Allied).

The API you see here is notional, or meant to be used as an example. Feel free to use it as starter code, or take concepts shown here and implement them in your own interface. You may, and probably will, have additional properties or parameters that need to be incorporated into your API. This is fine, even expected. Take this and mold it to meet your needs. However, ultimately all that is required, or should be required, is a balance check and the ability to debit/credit a customer account.

### Security

This sample application was initially created to support SSL (HTTPS). However, those aspects of the application have been commented out to allow direct access via IIS Express and Postman (scripts included) so you can interact with the application on your desktop.

This sample also does not assume any sort of authentication. Allied prefers to use HTTP Basic authentication. If your local policy mandates more complex authentication schemes, please contact Allied.

You _must_ secure your live API using SSL. It is not necessary to further encrypt the requests and responses. If your local policies are such that additional security is necessary, please contact Allied.

### Idempotency

In the sample you will find much support to demonstrate _idempotency_. Idempotency, simply defined, provides for the ability to make the same call repeatedly while receiving the same response.

Why is this necessary?

Imagine you made a car payment of $539.20 through an online bill payment service. Using your financial institution's real time interface, the online payment service checked your balance (of course it was sufficient!) and then attempted to transact your payment. But as sometimes happens, circumstances between the online bill payment service and your finanacial institution caused the transaction request to time out. In that case, the online transaction service doesn't know if the account was debited, so it retries and retries until it receives a successful transaction response.

In the case where the transaction wasn't made on the financial institution side, you're happy the online bill payment service kept trying.

However, if the financial institution did in fact debit your account, when the bill payment service makes the second and subsequent attempts to transact the payment, you will be debited a second $539.20! In fact, it could happen multiple times. Needless to say you're an unhappy customer! You've now paid $539.20 multiple times for a single car payment!

To avoid this, your real time collection system/API should employ idempotency. For each transaction you will receive an "idempotency key" (in the sample, the property is named "transactionKey"). When you process a transaction, you should record this key along with the response you intend to return. If further transaction requests are made using the same idempotency key, _do not_ process those transactions. Instead, retrieve the original (cached) response and return that. In this manner, the online bill payment service can make dozens, even hundreds of requests for the same transaction and the customer account would be debited only once. Of course, hopefully hundreds of requests aren't necessary! But regardless of the actual number of collection attempts, the answer will always be the same and the customer's financial happiness is greatly enhanced.

### About the Sample

The sample real time API uses Microsoft .NET Core, version 2.1. This framework was chosen for its simplicity and portability. You should find it a trivial task to port the concepts shown here to other languages/runtimes. However, if there are concepts and/or code here that inspire questions, but all means contact Allied for guidance.

The API includes three endpoints:
* [api/transactions/balance](api/transactions/balance)
* [api/transactions/transaction](api/transactions/transaction)
* [api/profile](api/profile)

Both use HTTP POST to provide the endpoint with relevant data in (example) JSON format:

##### Balance Payload

```JSON
{
	"routingTransitNumber": "1234567",
	"accountNumber": "112233"
}
```

##### Bill Payment Transaction Payload

```JSON
{
	"transactionId": "123456",
	"transactionKey": "f887aeb9-1d2e-4902-b476-0cf14bbae290",
	"amount": "123.45",
	"routingTransitNumber": "1234567",
	"accountNumber": "112233",
	"payeeName": "payeeName",
	"description": "customer payment memo",
	"transactionType": 2
}
```

##### Profile Payload

```JSON
{
	"username": "franksnbeans",
}
```

In code, the request classes are as follows:

##### Balance Request

```C#
public class BalanceRequest
{
    // Financial institution RTN
    public string routingTransitNumber { get; set; }

    // Customer account number
    public string accountNumber { get; set; }

    // TODO: place any additional properties necessary to
    // identify an individual account below.
}
```

##### Bill Payment Transaction Request

```C#
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

    // NOTE: the transactionKey is an idempotency key
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
```

##### Bill Payment Profile Request
```C#
public class ProfileRequest
{
    public string username { get; set; }
}
```

Responses from the API include a success flag and an error description, if any:

##### Balance Response

```JSON
{
    "balance": 955.47,
    "routingTransitNumber": "1234567",
    "accountNumber": "112233",
    "success": true,
    "errorMessage": null
}
```

##### Bill Payment Transaction Response

```JSON
{
    "transactionKey": "f887aeb9-1d2e-4902-b476-0cf14bbae290",
    "description": "Transaction successful",
    "success": true,
    "errorMessage": null
}
```

##### Profile Response

```JSON
{
    "firstName": "Frank",
    "lastname": "Beans",
    "companyName": null,
    "address1": "123 Fast Lane",
    "address2": null,
    "city": "Detroit",
    "state": "MI",
    "zip": "48118",
    "success": true,
    "errorMessage": null
}
```

The sample application will also return errors. If you transact a payment using a GL account, for example, you'll receive the following error response:

##### Error Response

```JSON
{
    "transactionKey": "f887aeb9-1d2e-4902-b476-0cf14bbae290",
    "description": "Failure: cannot transact GL accounts",
    "success": false,
    "errorMessage": "Failure: cannot transact GL accounts"
}
```

##### HTTP Response Codes

If you want to generate a heated debate, ask any web application developer if they should return a 2xx or 4xx response code if the business logic indicates error. By strict definition, the _HTTP_ response code in the 2xx range indicates _communication_ success. "I received your data. I understood your data. I processed your data." It does _not_ mean the processing succeeded. This is by definition.

However, there is much disagreement on this. So while the sample application follows this practice, your application can "go the other way" and return a 4xx code if payment transactions fail. The Allied real time processing system makes no assumptions in this regard.

It is mentioned here only to address any potential questions regarding the sample application. There is no need to argue that the sample should be returning a 4xx code if the memo post didn't include a check number (a validation exception you'll find in the sample). It isn't worth the argument. By all means create your application as you deem proper and all will work out.

### Disclaimer

As previously mentioned, this software is provided "as is" and is in no way guaranteed to be suitable for any given application. It is provided as reference material for implementing a real time payment collection system, and your system requirements may vary. 

### Questions

Contact Allied Payment Network for assistance.