# Interview Revision: Scenario-Based Problems

These are the kind of live-coding / whiteboard prompts you might get in a design
interview. Each includes a suggested talking-point structure, not just code.

## Scenario 1: "Design a rate limiter that supports multiple algorithms"

**Talking points:**
- Define a `RateLimiterStrategy` interface (`allow_request(client_id) -> bool`).
- Implement `TokenBucketLimiter`, `FixedWindowLimiter`, `SlidingWindowLimiter` as
  separate classes (OCP — new algorithms are new classes).
- A `RateLimiter` facade depends only on the interface (DIP), chosen at construction
  time.
- Mention testability: each strategy can be unit tested with a fake clock, no need to
  spin up the whole system.

## Scenario 2: "This class handles user signup, validation, database save, and welcome
email — refactor it live."

**Talking points:**
- Name the actors (validation/compliance, DB/infra, marketing) — SRP.
- Extract `SignupValidator`, `UserRepository`, `WelcomeEmailSender`.
- Show the orchestrator (`SignupService`) receiving all three via constructor
  injection (DIP), and explain why this is now trivially unit-testable.
- This maps directly to `02-SRP/004-Refactoring.md` — recall that walkthrough.

## Scenario 3: "How would you add support for a new file export format without
redeploying the whole export module?"

**Talking points:**
- Identify the varying axis: "the export format."
- Define an `Exporter` interface; each format is a class.
- Mention a registry/factory pattern (`08-Design-Patterns/001-Factory.md`) so new
  formats can even be registered via plugin/config, not just new classes.
- Emphasize: zero changes to existing exporters or the core export-triggering code.

## Scenario 4: "A junior engineer's PR adds a boolean flag `is_premium_user` to 12
methods across a `UserService` class to handle premium-only behavior differently. What
do you tell them?"

**Talking points:**
- Diagnose: boolean-flag-driven branching scattered across many methods is a subtler
  version of the OCP `if/elif` smell — and often signals a missing type/strategy
  distinction (regular vs premium behavior should probably be two classes/strategies,
  not a flag checked repeatedly).
- Propose extracting a `UserTier` abstraction (`RegularTier`, `PremiumTier`) with
  methods like `discount_rate()`, `support_priority()`, so `UserService` delegates to
  the tier object instead of branching on the flag 12 times.
- Acknowledge nuance: if the flag only ever affects one or two lines total, this may
  be over-engineering — judgment call based on how much branching logic actually
  exists.

## Scenario 5: "Our tests are slow because `OrderService` hits a real database and a
real payment gateway even for simple business-logic tests. How would you fix this?"

**Talking points:**
- Diagnose: DIP violation — `OrderService` likely instantiates concrete
  `Database`/`PaymentGateway` clients directly.
- Propose introducing `OrderRepository` and `PaymentGateway` interfaces, injecting
  concrete implementations at the composition root, and using fakes/in-memory
  implementations in tests.
- Reference `06-DIP/005-Manual-Dependency-Injection.md` for the concrete pattern.

Continue to `004-Refactoring-Questions.md` for pure refactoring-focused prompts.
