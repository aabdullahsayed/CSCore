# SRP: Refactoring Walkthrough

Here's a step-by-step process for taking a God Class and splitting it along SRP lines,
using a `UserRegistrationService` as the running example.

## Step 0: The starting point

```python
class UserRegistrationService:
    def register(self, username, email, password):
        if "@" not in email:
            raise ValueError("Invalid email")
        if len(password) < 8:
            raise ValueError("Password too short")

        hashed = self._hash_password(password)

        conn = sqlite3.connect("users.db")
        conn.execute(
            "INSERT INTO users (username, email, password) VALUES (?, ?, ?)",
            (username, email, hashed),
        )
        conn.commit()

        self._send_email(email, "Welcome!", f"Hi {username}, thanks for joining.")
        print(f"[LOG] Registered user {username}")

    def _hash_password(self, password):
        import hashlib
        return hashlib.sha256(password.encode()).hexdigest()

    def _send_email(self, to, subject, body):
        print(f"Sending email to {to}: {subject} - {body}")
```

## Step 1: List every actor / reason to change

- **Validation rules** → product/compliance team
- **Password hashing strategy** → security team
- **Database schema/engine** → infrastructure team
- **Email content/provider** → marketing/notifications team
- **Logging format** → observability/ops team

Five actors, one class. That's five reasons to change.

## Step 2: Extract one class per responsibility

```python
class UserValidator:
    def validate(self, username, email, password):
        if "@" not in email:
            raise ValueError("Invalid email")
        if len(password) < 8:
            raise ValueError("Password too short")


class PasswordHasher:
    def hash(self, password):
        import hashlib
        return hashlib.sha256(password.encode()).hexdigest()


class UserRepository:
    def save(self, username, email, hashed_password):
        conn = sqlite3.connect("users.db")
        conn.execute(
            "INSERT INTO users (username, email, password) VALUES (?, ?, ?)",
            (username, email, hashed_password),
        )
        conn.commit()


class WelcomeEmailSender:
    def send(self, email, username):
        print(f"Sending email to {email}: Welcome! Hi {username}, thanks for joining.")
```

## Step 3: Reassemble with a thin orchestrator

```python
class UserRegistrationService:
    def __init__(self, validator, hasher, repository, emailer):
        self.validator = validator
        self.hasher = hasher
        self.repository = repository
        self.emailer = emailer

    def register(self, username, email, password):
        self.validator.validate(username, email, password)
        hashed = self.hasher.hash(password)
        self.repository.save(username, email, hashed)
        self.emailer.send(email, username)
        print(f"[LOG] Registered user {username}")
```

Note: `UserRegistrationService` itself now has a single, clear job — **orchestrating
the registration workflow** — and delegates every other concern. This is a common and
healthy pattern: a thin coordinator class composed of single-purpose collaborators.

## Step 4: Verify with tests

Each extracted class can now be tested alone:

```python
def test_validator_rejects_short_password():
    validator = UserValidator()
    with pytest.raises(ValueError):
        validator.validate("bob", "bob@example.com", "short")
```

No database, no email server, no mocking framework needed for this test — that's the
payoff of SRP.

Continue to `005-Exercises.md` to practice this yourself.
