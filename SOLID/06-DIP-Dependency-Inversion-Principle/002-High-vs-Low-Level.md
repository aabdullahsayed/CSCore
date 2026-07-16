# DIP: High-Level vs Low-Level Modules

## What makes a module "high-level"?

A high-level module encodes **business policy** — the rules that make your
application what it is. Examples: "an order can't be placed if inventory is zero,"
"a loan is approved if credit score exceeds 700," "a user is logged out after 30
minutes of inactivity."

## What makes a module "low-level"?

A low-level module encodes **mechanism/detail** — *how* something is technically
done, which is usually replaceable and not unique to your business: which SQL dialect
runs a query, which HTTP library sends a request, which file format serializes data.

## Why the direction of dependency matters so much

Business rules are the most valuable, most stable, most expensive-to-get-wrong part of
a system. Infrastructure details (which database, which cloud vendor, which email
provider) change far more often, and are usually interchangeable.

If high-level policy code directly depends on low-level detail code, then:

- Every low-level change (switching from SendGrid to AWS SES for email) risks
  touching, breaking, or requiring re-testing of business logic that has nothing to do
  with email providers.
- You cannot unit test business logic without also spinning up (or mocking) the real
  infrastructure — databases, network calls, file systems.
- The "important" code (business rules) becomes entangled with, and dependent on the
  release cycle of, the "replaceable" code (infrastructure).

DIP flips this: infrastructure code depends on interfaces defined by/for the business
logic, so business logic can be developed, tested, and reasoned about in complete
isolation from infrastructure.

## A concrete comparison

```python
# WITHOUT DIP: high-level LoanApprovalService depends directly on low-level detail
class ExperianCreditCheck:
    def get_score(self, ssn):
        # calls Experian's real API
        ...

class LoanApprovalService:
    def __init__(self):
        self.credit_check = ExperianCreditCheck()   # tightly bound to one vendor

    def approve(self, applicant):
        score = self.credit_check.get_score(applicant.ssn)
        return score > 700
```

```python
# WITH DIP: both sides depend on an abstraction owned by the business layer
class CreditCheckProvider(ABC):
    @abstractmethod
    def get_score(self, ssn): ...

class ExperianCreditCheck(CreditCheckProvider):
    def get_score(self, ssn):
        ...  # calls Experian's real API

class LoanApprovalService:
    def __init__(self, credit_check: CreditCheckProvider):
        self.credit_check = credit_check

    def approve(self, applicant):
        score = self.credit_check.get_score(applicant.ssn)
        return score > 700
```

Now `LoanApprovalService` — the valuable, stable business rule ("approve if score >
700") — can be fully unit tested with a fake `CreditCheckProvider`, and switching from
Experian to Equifax never touches `LoanApprovalService` at all.

Continue to `003-Abstractions.md` to dig into what makes a good abstraction boundary.
