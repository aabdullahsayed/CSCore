# ISP: Interview Questions

## Q1: State the Interface Segregation Principle.

**A:** Clients should not be forced to depend on interfaces/methods they don't use.
Prefer many small, client-specific interfaces over one large, general-purpose
interface.

## Q2: How is ISP different from SRP?

**A:** SRP is about a *class* having a single reason to change; ISP is about an
*interface/contract* not bundling unrelated capabilities together. You can think of
ISP as SRP applied specifically to interface design — both are about cohesion, but at
different levels (implementation vs contract).

## Q3: What's a telltale code smell of an ISP violation?

**A:** Implementers of an interface that throw `NotImplementedError`, no-op, or return
dummy/null values for a subset of the interface's methods because those methods don't
apply to that particular implementer.

## Q4: How does violating ISP lead to violating LSP?

**A:** A fat interface often forces some implementers to fake or reject methods they
can't meaningfully support (e.g., `Robot.eat()` throwing). Any code that calls that
method generically — trusting the interface's contract — will then break at runtime
when handed that implementer, which is exactly an LSP violation. ISP violations are a
very common *root cause* of LSP violations.

## Q5: Does ISP mean every interface should have only one method?

**A:** No. Interfaces should be as small as necessary to represent one *cohesive
capability* consumed as a whole by the clients that need it. A `Comparable` interface
with just `compare_to` is fine at one method; a `Stack` interface with `push`, `pop`,
`peek` is also fine at three methods, because every stack client genuinely needs all
three together.

## Q6: How would you apply ISP to a REST API or SDK design (not just OO classes)?

**A:** The same principle applies: don't force every API consumer to implement a huge
webhook handler interface with 15 event types if a given integration only cares about
3 of them. Split subscriptions/interfaces so each consumer only needs to handle the
events relevant to it.

## Q7: A junior engineer proposes one giant `IService` interface with 20 methods so "all
services are consistent." How do you respond?

**A:** Explain that forcing every service implementation to satisfy all 20 methods
will lead to stub/no-op implementations for the ones that don't apply, that this
creates false capability signaling (a type says it can do something it actually
can't), and that it will make the interface impossible to evolve without touching
every implementer. Recommend splitting by actual capability groups instead, so each
service depends only on the sub-interfaces it truly needs.

## Q8: Can ISP conflict with DRY (don't repeat yourself)?

**A:** In a narrow sense, splitting one interface into several can mean a class that
needs "everything" ends up implementing multiple small interfaces instead of one big
one — slightly more declarations, but not more logic duplication. This is a reasonable
tradeoff: ISP optimizes for correctness and clarity of contracts per consumer, which
is usually worth a small amount of extra interface declaration.
