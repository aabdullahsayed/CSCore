## Strategy pattern

`Strategy is a behavioral design pattern that lets you define a
family of algorithms, put each of them into a separate class,
and make their objects interchangeable.`

<img src="image.svg">

`IPaymentStrategy` is the contract. It defines one method  `Pay(decimal amount)`  that all payment methods must implement. Neither the client nor the context knows (or cares) what happens inside.
`BkashPayment`, `CardPayment`, and `NagadPayment` are the concrete strategies. Each implements the interface with its own logic.


`PaymentProcess` is the context. It holds a reference to whichever `IPaymentStrategy` was injected at runtime. When you call `ProcessPayment()`, it just delegates to `_strategy.Pay(amount)`  it never asks which one?

The key benefit is the `Open/Closed Principle` in action: adding a new payment method (say, `SslcommerzPayment`) requires zero changes to `PaymentProcess` or `Program.cs`  you just write a new class that implements the interface.

### When Do We Use It?
You use the Strategy Pattern when you have one specific task that can be done in multiple different ways, and you want to be able to switch between those ways on the fly (at runtime).

### Real-world examples:

- Payment Processing: Pay by Card, PayPal, or Crypto.

- Navigation Apps: Calculate a route by Walking, Driving, or Cycling.

- File Export: Export a document as PDF, DOCX, or TXT.

- Data Sorting: Sort a list using QuickSort, MergeSort, or BubbleSort.