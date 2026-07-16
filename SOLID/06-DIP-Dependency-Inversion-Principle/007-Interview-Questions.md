# DIP: Interview Questions

## Q1: State the Dependency Inversion Principle.

**A:** High-level modules should not depend on low-level modules; both should depend
on abstractions. Abstractions should not depend on details; details should depend on
abstractions. In practice: business logic should depend on interfaces, and
infrastructure code should implement those interfaces — not the other way around.

## Q2: What's the difference between Dependency Inversion and Dependency Injection?

**A:** DIP is a design *principle* about the direction of source-code dependencies
(depend on abstractions, not concretions). Dependency Injection is a *technique* —
usually constructor injection — for supplying a class's dependencies from the outside
rather than having it construct them itself. DI is the most common way to satisfy DIP,
but they are not the same thing; you can inject a concrete class (violating DIP) or
satisfy DIP without any injection framework at all.

## Q3: Why is it called "inversion"?

**A:** Without DIP, dependencies naturally flow "downward" from high-level policy to
low-level detail (high-level code directly instantiates/depends on low-level
classes). DIP inverts this: the abstraction is defined in terms of what the high-level
module needs, and the low-level module must conform to it — so the low-level detail
now depends on something shaped by the high-level module, reversing the traditional
direction.

## Q4: Give an example of a DIP violation and how you'd fix it.

**A:** A `PaymentService` (high-level) directly instantiating and calling
`StripeClient` (low-level) throughout its code. Fix: define a `PaymentGateway`
interface with the methods `PaymentService` actually needs, have `StripeClient`
implement it via an adapter class, and have `PaymentService` depend only on
`PaymentGateway`, injected via its constructor.

## Q5: Does every dependency need to be abstracted behind an interface?

**A:** No. DIP is a heuristic reserved for boundaries where you genuinely need
substitutability: unit testing without real infrastructure, supporting multiple
vendors/environments, or decoupling team release cycles. Abstracting a stable,
single-implementation utility "just in case" adds needless indirection.

## Q6: How does DIP improve testability specifically?

**A:** By depending on interfaces rather than concrete infrastructure classes,
business logic can be tested with lightweight fakes/stubs (in-memory implementations)
instead of a real database, network call, or third-party service — making tests fast,
deterministic, and independent of external systems.

## Q7: Who should "own" the abstraction — the high-level module or the low-level
module?

**A:** Conceptually, the high-level module. The interface should be shaped by what the
business logic needs (e.g., `save_order(order)`), not by what a particular vendor SDK
happens to expose (e.g., `execute_sql(query, params)`). This keeps the abstraction
stable even as low-level implementations are swapped.

## Q8: How does DIP relate to the other SOLID principles?

**A:** DIP is in many ways the principle that *enables* OCP in practice — you can only
add new implementations without modifying existing code (OCP) if high-level code
already depends on an abstraction rather than a concrete class (DIP). It also pairs
with ISP: the abstractions DIP introduces should be small and client-specific, not
fat, general-purpose interfaces.
