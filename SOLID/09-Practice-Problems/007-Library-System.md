# Practice Problem: Library System

## The problem

Design a small library management system supporting books, DVDs, and magazines, each
with different loan periods and late-fee rules, plus notifications when an item is
overdue. Avoid `if isinstance()` / type-string branching.

## Requirements

1. Each item type defines its own loan period and late-fee formula (OCP + SRP).
2. The system must notify a patron through their preferred channel when an item is
   overdue, without the `Library` class needing to know delivery details (DIP,
   reusing the Observer/Strategy ideas from earlier folders).
3. Adding a new item type (e.g., "audiobook") must not require touching the `Library`
   class.

## Suggested solution

```python
from abc import ABC, abstractmethod
from datetime import date, timedelta

class LibraryItem(ABC):
    def __init__(self, title):
        self.title = title

    @abstractmethod
    def loan_period_days(self) -> int: ...

    @abstractmethod
    def late_fee_per_day(self) -> float: ...


class Book(LibraryItem):
    def loan_period_days(self): return 21
    def late_fee_per_day(self): return 0.25

class DVD(LibraryItem):
    def loan_period_days(self): return 7
    def late_fee_per_day(self): return 1.00

class Magazine(LibraryItem):
    def loan_period_days(self): return 14
    def late_fee_per_day(self): return 0.10


class NotificationChannel(ABC):
    @abstractmethod
    def notify(self, patron: str, message: str) -> None: ...

class EmailNotification(NotificationChannel):
    def notify(self, patron, message):
        print(f"Email to {patron}: {message}")


class Loan:
    def __init__(self, item: LibraryItem, patron: str, checkout_date: date):
        self.item = item
        self.patron = patron
        self.checkout_date = checkout_date

    def due_date(self) -> date:
        return self.checkout_date + timedelta(days=self.item.loan_period_days())

    def late_fee(self, today: date) -> float:
        overdue_days = max(0, (today - self.due_date()).days)
        return overdue_days * self.item.late_fee_per_day()


class Library:
    def __init__(self, notifier: NotificationChannel):
        self.notifier = notifier

    def check_overdue(self, loan: Loan, today: date):
        fee = loan.late_fee(today)
        if fee > 0:
            self.notifier.notify(
                loan.patron, f"'{loan.item.title}' is overdue. Fee: ${fee:.2f}"
            )
```

## Usage

```python
loan = Loan(DVD("Inception"), "alice@example.com", date(2026, 6, 1))
library = Library(EmailNotification())
library.check_overdue(loan, today=date(2026, 6, 20))
```

Adding `Audiobook` is a new `LibraryItem` subclass; adding SMS notifications is a new
`NotificationChannel` subclass. `Library` and `Loan` never change for either
extension.
