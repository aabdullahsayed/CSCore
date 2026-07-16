# ISP: Exercises

## Exercise 1: Split this interface

```python
class Vehicle(ABC):
    @abstractmethod
    def drive(self): ...
    @abstractmethod
    def fly(self): ...
    @abstractmethod
    def sail(self): ...
```

A `Car`, a `Plane`, and a `Boat` all need to implement `Vehicle`, but each only
supports one of the three actions. Redesign this to avoid forcing empty/throwing
implementations.

<details>
<summary>Suggested solution</summary>

```python
class Driveable(ABC):
    @abstractmethod
    def drive(self): ...

class Flyable(ABC):
    @abstractmethod
    def fly(self): ...

class Sailable(ABC):
    @abstractmethod
    def sail(self): ...

class Car(Driveable):
    def drive(self): print("Driving")

class Plane(Flyable):
    def fly(self): print("Flying")

class Boat(Sailable):
    def sail(self): print("Sailing")

# A flying car / amphibious vehicle can combine multiple:
class FlyingCar(Driveable, Flyable):
    def drive(self): print("Driving")
    def fly(self): print("Flying")
```
</details>

## Exercise 2: Identify the violation

```python
class Employee(ABC):
    @abstractmethod
    def calculate_salary(self): ...
    @abstractmethod
    def calculate_commission(self): ...
```

A salaried office employee has no commission. Explain the ISP problem and fix it.

<details>
<summary>Suggested answer</summary>

Forcing `SalariedEmployee` to implement `calculate_commission()` (probably returning
0 or raising) mixes two distinct client needs: "compute base pay" and "compute
commission for sales roles." Split into `SalaryCalculable` and
`CommissionEarner`, and have `SalesEmployee` implement both while
`SalariedEmployee` implements only the first.
</details>

## Exercise 3: Real-world API design

You're designing an interface for a cloud storage SDK used by many different client
apps: some only read files, some only upload, some need full file management
(rename/delete/list). Sketch an ISP-compliant set of interfaces (not full
implementations) that would let each client depend on only what it needs.

<details>
<summary>Suggested starting point</summary>

```python
class FileReader(ABC):
    @abstractmethod
    def read(self, path): ...

class FileWriter(ABC):
    @abstractmethod
    def write(self, path, data): ...

class FileManager(ABC):
    @abstractmethod
    def rename(self, old_path, new_path): ...
    @abstractmethod
    def delete(self, path): ...
    @abstractmethod
    def list_files(self, directory): ...
```

A read-only client depends on `FileReader` alone; an uploader depends on `FileWriter`
alone; an admin tool depends on all three.
</details>

## Exercise 4: When is a "fat" interface actually fine?

Explain why the built-in-style `Comparable` interface (`compare_to(self, other)` —
just one method) is never a candidate for ISP splitting, no matter how many different
kinds of clients use it.

<details>
<summary>Suggested answer</summary>

ISP only applies when an interface bundles **multiple, independently-useful**
capabilities that different clients need in different combinations. A single-method
interface has nothing to split — every client that needs comparison needs exactly
that one method, so there's no disjoint subset of unused methods to segregate away.
</details>
