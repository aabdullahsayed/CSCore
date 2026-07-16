# DIP: Manual Dependency Injection (No Framework Required)

Many languages have DI frameworks (Spring in Java, .NET's built-in DI container) that
automate wiring. Python and JavaScript projects frequently skip the framework and wire
dependencies by hand — this is completely valid and often clearer for small-to-medium
projects.

## The problem: wiring a multi-layer object graph

```python
class Database(ABC):
    @abstractmethod
    def query(self, sql): ...

class PostgresDatabase(Database):
    def query(self, sql):
        print(f"Postgres running: {sql}")

class OrderRepository:
    def __init__(self, db: Database):
        self.db = db
    def find_order(self, order_id):
        return self.db.query(f"SELECT * FROM orders WHERE id={order_id}")

class EmailSender(ABC):
    @abstractmethod
    def send(self, to, body): ...

class SmtpEmailSender(EmailSender):
    def send(self, to, body):
        print(f"Emailing {to}: {body}")

class OrderNotifier:
    def __init__(self, email_sender: EmailSender):
        self.email_sender = email_sender
    def notify(self, order, customer_email):
        self.email_sender.send(customer_email, f"Your order {order} shipped!")

class OrderService:
    def __init__(self, repository: OrderRepository, notifier: OrderNotifier):
        self.repository = repository
        self.notifier = notifier
    def ship_order(self, order_id, customer_email):
        order = self.repository.find_order(order_id)
        self.notifier.notify(order, customer_email)
```

## Manual wiring at the composition root

```python
def build_order_service() -> OrderService:
    db = PostgresDatabase()
    repository = OrderRepository(db)

    email_sender = SmtpEmailSender()
    notifier = OrderNotifier(email_sender)

    return OrderService(repository, notifier)

def main():
    service = build_order_service()
    service.ship_order(order_id=42, customer_email="alice@example.com")

if __name__ == "__main__":
    main()
```

## Swapping implementations for tests — the entire payoff of DIP

```python
class FakeDatabase(Database):
    def query(self, sql):
        return {"id": 42, "item": "Widget"}

class FakeEmailSender(EmailSender):
    def __init__(self):
        self.sent = []
    def send(self, to, body):
        self.sent.append((to, body))

def test_ship_order_notifies_customer():
    repo = OrderRepository(FakeDatabase())
    fake_email = FakeEmailSender()
    notifier = OrderNotifier(fake_email)
    service = OrderService(repo, notifier)

    service.ship_order(order_id=42, customer_email="alice@example.com")

    assert len(fake_email.sent) == 1
    assert fake_email.sent[0][0] == "alice@example.com"
```

No framework, no mocking library — just plain constructors and plain classes,
composed by hand. This scales comfortably up to dozens of classes; past that, many
teams introduce a DI container purely to reduce the boilerplate of the `build_*()`
composition functions, **not** to change the underlying design principle.

## When a DI framework earns its keep

- The object graph has hundreds of components, and manual wiring functions become
  unwieldy.
- You need per-request or per-scope lifetimes (e.g., one instance per web request) that
  are tedious to manage by hand.
- Multiple environments (test, staging, prod) need different implementation sets
  swapped in via configuration rather than code changes.

Continue to `006-Exercises.md` to practice applying DIP yourself.
