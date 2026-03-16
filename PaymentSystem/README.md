Make payments

Refund payments

Support multiple payment methods

Generate invoices

Send notifications

Apply discounts

PaymentSystem
│
├── Interfaces
│     IPaymentMethod.cs
│     IDiscountStrategy.cs
│     INotification.cs
│
├── Payments
│     CreditCardPayment.cs
│     PaypalPayment.cs
│     BkashPayment.cs
│
├── Discounts
│     PercentageDiscount.cs
│     FixedDiscount.cs
│
├── Notifications
│     EmailNotification.cs
│     SmsNotification.cs
│
├── Services
│     PaymentService.cs
│     InvoiceService.cs
│     NotificationService.cs
│
└── Program.cs