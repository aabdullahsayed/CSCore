# SRP: Bad Example

Consider a class that manages everything about a user's invoice: computing totals,
formatting the invoice text, and saving it to disk.

```python
class Invoice:
    def __init__(self, items):
        self.items = items  # list of (name, price, qty)

    def calculate_total(self):
        return sum(price * qty for _, price, qty in self.items)

    def print_invoice(self):
        print("---- INVOICE ----")
        for name, price, qty in self.items:
            print(f"{name}: {qty} x ${price} = ${price * qty}")
        print(f"TOTAL: ${self.calculate_total()}")

    def save_to_file(self, path):
        with open(path, "w") as f:
            f.write(f"Total: {self.calculate_total()}\n")
            for name, price, qty in self.items:
                f.write(f"{name},{price},{qty}\n")
```

## Why this violates SRP

Three separate actors care about this class:

1. **Finance/business logic** owns `calculate_total()`. If pricing rules change (e.g.
   adding tax, discounts, currency conversion), this method changes.
2. **UI/presentation** owns `print_invoice()`. If the invoice needs to look different
   (HTML, PDF, colored terminal output), this method changes.
3. **Persistence/infrastructure** owns `save_to_file()`. If invoices move from flat
   files to a database or S3 bucket, this method changes.

## The pain this causes in practice

- A developer asked to "just change how invoices are printed" opens `Invoice`, sees
  `calculate_total` and `save_to_file` right next to the code they need to touch, and
  risks breaking pricing logic by accident.
- Unit testing `calculate_total()` in isolation is awkward because the class also
  depends on file I/O — every test of `Invoice` risks becoming an integration test
  that touches disk.
- Reusing just the pricing logic in another context (e.g., a shopping cart preview)
  means dragging in printing and file-saving code you don't need.

## The tell

Ask: *"If the file format changes, does `calculate_total` need to be touched?"* No —
but they're both in the same class, so a change to one risks accidental breakage of
the other, and both get recompiled/redeployed together even though they're logically
unrelated.

Continue to `003-Good-Example.md` for the SRP-compliant version.
