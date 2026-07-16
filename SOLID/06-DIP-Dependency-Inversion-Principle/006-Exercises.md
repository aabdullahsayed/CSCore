# DIP: Exercises

## Exercise 1: Invert this dependency

```python
class FileLogger:
    def log(self, message):
        with open("app.log", "a") as f:
            f.write(message + "\n")

class PaymentProcessor:
    def __init__(self):
        self.logger = FileLogger()   # direct dependency on a concrete, low-level detail

    def process(self, amount):
        self.logger.log(f"Processing payment of {amount}")
        # ... payment logic
```

Refactor `PaymentProcessor` to depend on an abstraction instead, and show a
`ConsoleLogger` alternative implementation plus a `FakeLogger` used in a unit test.

<details>
<summary>Suggested solution</summary>

```python
class Logger(ABC):
    @abstractmethod
    def log(self, message): ...

class FileLogger(Logger):
    def log(self, message):
        with open("app.log", "a") as f:
            f.write(message + "\n")

class ConsoleLogger(Logger):
    def log(self, message):
        print(message)

class PaymentProcessor:
    def __init__(self, logger: Logger):
        self.logger = logger

    def process(self, amount):
        self.logger.log(f"Processing payment of {amount}")

class FakeLogger(Logger):
    def __init__(self):
        self.messages = []
    def log(self, message):
        self.messages.append(message)

def test_process_logs_amount():
    fake = FakeLogger()
    processor = PaymentProcessor(fake)
    processor.process(100)
    assert "100" in fake.messages[0]
```
</details>

## Exercise 2: Identify the abstraction owner

For a `WeatherDashboard` high-level module that displays weather using a third-party
`OpenWeatherMapClient` SDK, sketch the abstraction that `WeatherDashboard` should own,
rather than depending on `OpenWeatherMapClient` directly.

<details>
<summary>Suggested solution</summary>

```python
class WeatherProvider(ABC):
    @abstractmethod
    def get_temperature(self, city: str) -> float: ...

class OpenWeatherMapProvider(WeatherProvider):
    def __init__(self, sdk_client):
        self.client = sdk_client
    def get_temperature(self, city):
        return self.client.fetch(city)["temp"]   # adapts the vendor SDK shape

class WeatherDashboard:
    def __init__(self, provider: WeatherProvider):
        self.provider = provider
    def show(self, city):
        print(f"{city}: {self.provider.get_temperature(city)}°")
```

`WeatherProvider` is shaped around what `WeatherDashboard` needs ("give me a
temperature for a city"), not around the vendor SDK's actual method names/response
shape — the adapter class absorbs that mismatch.
</details>

## Exercise 3: Spot when DIP is unnecessary

A small internal CLI script reads a single hardcoded CSV file and prints statistics.
It will never read from anywhere else. Should you introduce a `DataSource` interface
for this? Justify your answer.

<details>
<summary>Suggested answer</summary>

Probably not. If there's genuinely one implementation forever, with no testing need
that requires swapping it, and no team boundary requiring parallel independent
development, introducing an interface adds indirection with no real payoff. DIP earns
its cost when there's a genuine need for substitutability (tests, multiple
environments, multiple vendors, or parallel team development) — not universally.
</details>

## Exercise 4: Composition root design

Sketch (pseudocode is fine) the composition root for a small web app with an
`OrderService` (needs a `Database` and an `EmailSender`), a `UserService` (needs the
same `Database` and a `PasswordHasher`), showing how both services can share the same
`Database` instance while depending only on the abstraction.
