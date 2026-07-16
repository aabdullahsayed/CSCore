# DIP: Constructor Injection

Constructor injection is the most common, simplest way to satisfy DIP: a class
receives its dependencies (as abstractions) through its constructor, rather than
constructing them itself internally.

## The pattern

```python
from abc import ABC, abstractmethod

class EmailSender(ABC):
    @abstractmethod
    def send(self, to: str, subject: str, body: str) -> None: ...

class SmtpEmailSender(EmailSender):
    def send(self, to, subject, body):
        print(f"[SMTP] to={to} subject={subject}")

class SendGridEmailSender(EmailSender):
    def send(self, to, subject, body):
        print(f"[SendGrid API] to={to} subject={subject}")


class WelcomeEmailService:
    def __init__(self, email_sender: EmailSender):   # <-- constructor injection
        self.email_sender = email_sender

    def welcome(self, user_email, username):
        self.email_sender.send(
            user_email, "Welcome!", f"Hi {username}, glad to have you."
        )
```

## Why constructor injection specifically (vs setter injection, vs global singletons)

- **Immutability & correctness:** once constructed, `WelcomeEmailService` always has a
  valid `EmailSender` — there's no window where it's in a half-configured state
  (unlike setter injection, where you could forget to call the setter).
- **Explicitness:** anyone reading the constructor signature immediately sees every
  external dependency the class needs — this is effectively free, accurate
  documentation.
- **Testability:** tests construct the class with a fake/stub dependency directly, no
  global state to reset between tests.

```python
class FakeEmailSender(EmailSender):
    def __init__(self):
        self.sent = []
    def send(self, to, subject, body):
        self.sent.append((to, subject, body))

def test_welcome_sends_email():
    fake_sender = FakeEmailSender()
    service = WelcomeEmailService(fake_sender)
    service.welcome("alice@example.com", "Alice")
    assert len(fake_sender.sent) == 1
    assert fake_sender.sent[0][0] == "alice@example.com"
```

No mocking library, no real SMTP server, no network — a pure, fast unit test.

## Composition root

Somewhere near the entry point of your application (often called the "composition
root"), you decide which concrete implementation to use and wire everything together:

```python
def main():
    email_sender = SmtpEmailSender()          # decision made once, in one place
    service = WelcomeEmailService(email_sender)
    service.welcome("bob@example.com", "Bob")

if __name__ == "__main__":
    main()
```

Everything *between* the composition root and the leaves of the call graph depends
only on abstractions — the concrete choice is made exactly once, at the top.

Continue to `005-Manual-Dependency-Injection.md` to see how this scales to a larger
object graph without a DI framework.
