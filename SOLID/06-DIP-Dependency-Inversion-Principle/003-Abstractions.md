# DIP: Designing Good Abstractions

Not every interface is a good DIP abstraction. This file covers what makes an
abstraction boundary effective versus leaky.

## Rule 1: The abstraction should be owned by the high-level module

In good DIP design, the *interface* (`CreditCheckProvider`, `Database`,
`PaymentGateway`) conceptually belongs to the business/domain layer — it's defined in
terms of what the business logic needs, not in terms of what a particular vendor's SDK
happens to expose.

**Leaky (interface shaped by the implementation):**

```python
class Database(ABC):
    @abstractmethod
    def execute_sql(self, query: str, params: tuple): ...   # leaks "SQL" into the abstraction
```

If you later want a `MongoDatabase` implementation, `execute_sql` makes no sense for
it — the abstraction leaked a relational-database-specific detail.

**Better (interface shaped by business need):**

```python
class OrderRepository(ABC):
    @abstractmethod
    def save_order(self, order: Order) -> None: ...
    @abstractmethod
    def find_order(self, order_id: str) -> Order: ...
```

This can be implemented by `SqlOrderRepository`, `MongoOrderRepository`, or even
`InMemoryOrderRepository` for tests — because it describes *what the business needs*
("save an order," "find an order"), not *how* any particular technology does it.

## Rule 2: Depend on the smallest abstraction that satisfies the need (ties to ISP)

Don't create one giant `Database` interface with 30 methods when `OrderService` only
ever needs `save_order` and `find_order`. This is DIP and ISP working together.

## Rule 3: Abstractions should be stable

A good abstraction rarely changes, even as its implementations change dramatically
underneath. `OrderRepository.save_order(order)` doesn't need to change whether the
underlying storage is Postgres, DynamoDB, or a flat file — that's the whole point.

## Rule 4: Don't abstract prematurely

If there is, and will only ever be, one implementation of something (e.g., a
company's proprietary internal calculation with no swappable variants and no test
double ever needed), introducing an interface purely "for DIP's sake" adds
indirection with no payoff. Apply DIP where:

- you need to unit test business logic without invoking real infrastructure, or
- you genuinely expect multiple implementations (different vendors, different
  environments — prod vs test, different regions), or
- you want to decouple release cycles between teams (frontend team codes against an
  interface while a backend team implements it in parallel).

## A test for a good DIP boundary

Ask: *"If I swapped the concrete implementation entirely — different vendor,
different technology, an in-memory fake for testing — would the interface's method
signatures still make complete sense, unchanged?"* If yes, the abstraction is well
designed. If you'd need to change the interface too, it wasn't really abstracting
anything — it was just renaming the concrete API.

Continue to `004-Constructor-Injection.md` for how to actually wire abstractions and
implementations together.
