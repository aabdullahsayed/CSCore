# SRP: Good Example

We split `Invoice` from the previous file into three classes, each answering to a
single actor.

```python
class Invoice:
    """Pure data + business logic about what an invoice IS."""
    def __init__(self, items):
        self.items = items  # list of (name, price, qty)

    def calculate_total(self):
        return sum(price * qty for _, price, qty in self.items)


class InvoicePrinter:
    """Owns presentation. Changes when the display format changes."""
    def print(self, invoice: Invoice):
        print("---- INVOICE ----")
        for name, price, qty in invoice.items:
            print(f"{name}: {qty} x ${price} = ${price * qty}")
        print(f"TOTAL: ${invoice.calculate_total()}")


class InvoiceRepository:
    """Owns persistence. Changes when the storage mechanism changes."""
    def save(self, invoice: Invoice, path: str):
        with open(path, "w") as f:
            f.write(f"Total: {invoice.calculate_total()}\n")
            for name, price, qty in invoice.items:
                f.write(f"{name},{price},{qty}\n")
```

## Usage

```python
invoice = Invoice(items=[("Widget", 9.99, 3), ("Gadget", 19.99, 1)])

InvoicePrinter().print(invoice)
InvoiceRepository().save(invoice, "invoice.txt")
```

## What we gained

- **Testability:** `Invoice.calculate_total()` can be unit tested with zero I/O,
  instantly, with no mocking.
- **Independent evolution:** switching `InvoiceRepository` to save to a database
  instead of a file touches only that class — `Invoice` and `InvoicePrinter` are
  untouched and don't need to be re-reviewed or re-tested.
- **Reuse:** `Invoice` (the core data + math) can be reused in a totally different
  context — e.g., a REST API that returns JSON — without dragging along printing or
  file-saving code.
- **Clear ownership:** in a real team, the finance team owns `Invoice`, a
  platform/infra team owns `InvoiceRepository`, and a UX-adjacent team owns
  `InvoicePrinter`. Each can ship changes independently.

## Note on granularity

This doesn't mean every project needs three files for every concept — for a 50-line
throwaway script, one `Invoice` class is fine. SRP earns its keep once a class is
touched by more than one team, or grows past the point where you can describe its
purpose in a single clause.

Continue to `004-Refactoring.md` to practice identifying and fixing SRP violations
step by step.
