### Open Closed Principle
`Classes should open for extension but closed for modification`

The main idea of this principle is to keep existing code from
breaking when you implement new features.


Open for extension: You should be able to add new functionality to a class to accommodate new requirements.

Closed for modification: ou should not have to rewrite, edit, or tamper with existing, well-tested code to add that new functionality.


**Before violating OCP:**

```c#
public class Order
{
    public double Weight { get; set; }
    public string ShippingType { get; set; }

    public double CalculateShipping()
    {
        // VIOLATION: Every time the company adds a new shipping method,
        // you have to open this class and add another if/else or switch case.
        if (ShippingType == "Ground")
        {
            return Weight * 1.5;
        }
        else if (ShippingType == "Air")
        {
            return Weight * 3.0;
        }
        else
        {
            return 0; 
        }
    }
}

```


**After OCP**:
To fix this, we extract the shipping logic into separate classes that share a common interface

`Step A: Create the Interface ` This interface represents the "contract" for any current or future shipping method.

```c#
public interface IShippingMethod
{
    double CalculateCost(Order order);
}
```
`Step B: Create Concrete Strategies`
Now, we create separate classes for each shipping method. Notice how they all implement the IShippingMethod interface.

```c#
public class GroundShipping : IShippingMethod
{
    public double CalculateCost(Order order)
    {
        return order.Weight * 1.5;
    }
}

public class AirShipping : IShippingMethod
{
    public double CalculateCost(Order order)
    {
        return order.Weight * 3.0;
    }
}

// When the manager asks for Drone Delivery, you just create this new file. 
// You don't have to touch any of the old code! It is OPEN for extension.
public class DroneShipping : IShippingMethod
{
    public double CalculateCost(Order order)
    {
        return (order.Weight * 5.0) + 15.0; // Drone base fee
    }
}

```

`Step C: Refactor the Order Class`

```c#

public class Order
{
    public double Weight { get; set; }
    private readonly IShippingMethod _shippingMethod;

    // We "inject" the specific shipping method when we create the order.
    public Order(IShippingMethod shippingMethod)
    {
        _shippingMethod = shippingMethod;
    }

    public double CalculateShipping()
    {
        // The Order class no longer cares HOW the math is done. 
        // It just trusts the interface. It is CLOSED for modification.
        return _shippingMethod.CalculateCost(this);
    }
}


// Customer chooses Air Shipping
IShippingMethod method = new AirShipping();
Order myOrder = new Order(method) { Weight = 10.5 };

double cost = myOrder.CalculateShipping(); // Calculates Air Shipping perfectly

```