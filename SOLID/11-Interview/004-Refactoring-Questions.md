# Interview Revision: Refactoring Questions

## Question 1

```python
class ReportGenerator:
    def generate(self, data, format, destination):
        if format == "pdf":
            content = f"PDF report: {data}"
        elif format == "csv":
            content = f"CSV report: {data}"

        if destination == "email":
            print(f"Emailing report: {content}")
        elif destination == "s3":
            print(f"Uploading to S3: {content}")
```

**Prompt:** Identify every SOLID violation and refactor.

**Talking points for your answer:**
- Two independent axes of variation (`format` and `destination`) are both handled via
  `if/elif` in the same method — a double OCP violation.
- Splitting: a `ReportFormatter` interface (`Pdf`, `Csv`) and a
  `ReportDestination` interface (`Email`, `S3`), composed together in a thin
  `ReportGenerator.generate(data, formatter, destination)`.
- Mention this is essentially two independent Strategy pattern applications
  composed together.

## Question 2

```python
class Rectangle:
    def __init__(self, w, h):
        self.w, self.h = w, h
    def area(self): return self.w * self.h

class Square(Rectangle):
    def __init__(self, s):
        super().__init__(s, s)
    def set_width(self, w):
        self.w = self.h = w
```

**Prompt:** What's wrong here, and how would you know without even seeing calling
code?

**Talking points:**
- `Square` overrides mutation behavior in a way that breaks the implicit contract
  `Rectangle.set_width` (if it existed on the base) would carry: "changes width only."
- Even without seeing a caller, the mere existence of an override that changes *two*
  fields where the base type's naming/semantics implies *one* is the LSP red flag.
- Reference `04-LSP/002-Rectangle-Square-Problem.md` for both valid fixes (immutability
  or dropping the inheritance relationship for a shared `Shape` interface instead).

## Question 3

```python
class Notifier(ABC):
    @abstractmethod
    def send_email(self, msg): ...
    @abstractmethod
    def send_sms(self, msg): ...
    @abstractmethod
    def send_push(self, msg): ...

class EmailOnlyNotifier(Notifier):
    def send_email(self, msg): print(msg)
    def send_sms(self, msg): pass
    def send_push(self, msg): pass
```

**Prompt:** Diagnose and fix.

**Talking points:**
- Classic ISP violation: `EmailOnlyNotifier` is forced to implement two no-op
  methods it will never meaningfully support.
- Fix: split into `EmailNotifier`, `SmsNotifier`, `PushNotifier` interfaces;
  `EmailOnlyNotifier` implements only `EmailNotifier`.
- Bonus point: mention that the no-op methods are a *milder* form of the same problem
  as throwing (`004-Refactoring-Questions` Q2/LSP) — silently doing nothing can still
  surprise a caller who expected the SMS to actually be sent.

## Question 4

```python
class ReportService:
    def __init__(self):
        self.db = PostgresConnection("prod-db-url")
```

**Prompt:** What's wrong, and what would you ask before proposing a fix?

**Talking points:**
- DIP violation: hardcoded, concrete, environment-specific dependency constructed
  inside the class.
- Before proposing a fix, ask: is this ever tested in isolation? Are there multiple
  environments (dev/staging/prod) needing different DB targets? Is there a team
  boundary where infra changes shouldn't require redeploying `ReportService`'s
  logic? These questions justify *why* DIP is worth applying here, rather than
  reflexively abstracting.
- Fix: extract a `ReportDataSource` interface, inject a concrete implementation at
  the composition root.

Continue to `005-Cheat-Sheet.md` for the final condensed reference.
