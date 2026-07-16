# LSP: Interview Questions

## Q1: State the Liskov Substitution Principle in your own words.

**A:** If `S` is a subtype of `T`, you should be able to use an instance of `S`
anywhere a `T` is expected, without the program's correctness breaking. Subclasses
must honor the full behavioral contract of their base class/interface, not just its
method signatures.

## Q2: Why does the Rectangle/Square example violate LSP?

**A:** Because `Rectangle` (in the mutable version) implicitly promises that setting
width doesn't affect height and vice versa. `Square` can't honor that promise — it
must change both dimensions together to remain a square — so code that relies on
independent width/height mutation breaks silently when given a `Square`.

## Q3: What are the four typical ways a subclass violates LSP?

**A:**
1. Strengthening preconditions (requiring more than the base class did).
2. Weakening postconditions (guaranteeing less than the base class did).
3. Throwing new exceptions / changing expected behavior for methods the base class
   documented as safe/always-succeeding.
4. Violating invariants the base class guaranteed.

## Q4: How does LSP relate to the "bird that can't fly" problem?

**A:** It's the same underlying issue as Rectangle/Square: `Bird.fly()` is a promise
made to every caller of `Bird`. `Penguin` can't honor it, so substituting a `Penguin`
wherever a `Bird` is expected breaks calling code. The fix is to not put `fly()` on
the shared base type at all — split it into a more specific `FlyingBird`
interface/subtype.

## Q5: How would you detect an LSP violation during code review, without running the
code?

**A:** Look for subclasses that: override a method to throw an exception the base
class never documented; override a method to silently do nothing where the base class
guaranteed an effect; add new required parameters/preconditions; or return a
narrower/different type than the base class's documented return contract.

## Q6: Is LSP purely a static-typing concern, or does it matter in dynamically-typed
languages like Python or JavaScript too?

**A:** It matters in both. Static typing can catch *structural* mismatches (wrong
method signature) but not *behavioral* mismatches (a method that exists with the
right signature but violates the caller's reasonable expectations). LSP is fundamentally
about behavior, so it applies regardless of the type system — dynamically typed
languages actually need *more* discipline here since the compiler offers even less
protection.

## Q7: How does LSP support the Open/Closed Principle?

**A:** OCP relies on being able to add new subclasses/implementations without
touching existing calling code. That's only safe if every new subclass is truly
substitutable for the base type — i.e., only safe if LSP holds. Without LSP, "open for
extension" becomes dangerous, because each new extension might silently break existing
callers.

## Q8: Give an example of a valid, LSP-compliant inheritance hierarchy.

**A:** `SavingsAccount` and `CheckingAccount` both extending an abstract `Account`
with `deposit()` and `withdraw()`, where both fully honor the contract: deposits
always increase balance, withdrawals always decrease it (perhaps with different
overdraft rules that are part of the *documented* contract from the start, not a
surprise override). As long as every documented guarantee of `Account` holds for both
subclasses, substitution is safe.
