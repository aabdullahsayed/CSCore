# Refactoring Technique: Extract Class

**Extract Class** is the primary mechanical technique for fixing SRP violations: pull
a cohesive group of fields/methods out of a bloated class into a new class of their
own.

## Step-by-step process

1. **Identify a cluster.** Look for fields and methods within the class that are used
   together, but not used by the rest of the class's methods.
2. **Create the new class.** Give it a name that describes the cluster's single
   responsibility.
3. **Move fields.** Move the relevant fields to the new class.
4. **Move methods**, one at a time, running tests after each move.
5. **Establish a link.** The original class typically keeps a reference to (or
   delegates to) the new class, so external callers aren't immediately broken.
6. **Clean up callers.** Once stable, update external callers to talk to the new class
   directly where appropriate, and remove now-redundant delegation methods.

## Example

**Before:**

```python
class Employee:
    def __init__(self, name, street, city, zip_code, salary):
        self.name = name
        self.street = street
        self.city = city
        self.zip_code = zip_code
        self.salary = salary

    def get_full_address(self):
        return f"{self.street}, {self.city}, {self.zip_code}"

    def annual_salary(self):
        return self.salary * 12
```

`Employee` mixes two responsibilities: identity/pay information, and address
formatting. The address fields (`street`, `city`, `zip_code`) form a natural cluster.

**After — Extract Class applied:**

```python
class Address:
    def __init__(self, street, city, zip_code):
        self.street = street
        self.city = city
        self.zip_code = zip_code

    def full_address(self):
        return f"{self.street}, {self.city}, {self.zip_code}"


class Employee:
    def __init__(self, name, address: Address, salary):
        self.name = name
        self.address = address
        self.salary = salary

    def annual_salary(self):
        return self.salary * 12
```

## Signals that Extract Class is the right move

- A subset of a class's fields are always read/written together, and rarely touched by
  the rest of the class's methods.
- A subset of methods could be renamed with a completely different, coherent noun
  ("Address" vs "Employee") without losing any meaning.
- Two different actors/teams care about the two clusters (see `02-SRP/001-Definition`).

## Related techniques

- **Extract Interface** (next file) — the analogous technique applied to contracts
  instead of implementations, used for ISP and DIP.
- **Move Method** — a smaller-scale cousin, moving a single method (rather than a
  cluster) to the class that's actually most cohesive with it.

Continue to `003-Extract-Interface.md`.
