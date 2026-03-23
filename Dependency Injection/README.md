
**The Core Problem**
Typically, applications are built with multiple layers, such as **controllers**, **services**, and **repositories**. In a standard Java application, if a controller wants to use service methods, you need the object of that service. Because Java is object-driven, you have to "literally create an object by using a new keyword". When an application has hundreds of classes, managing the entire cycle of creating and destroying these objects for every request becomes highly complex.

**Inversion of Control (IoC)**
IoC is a principle and a philosophy that makes the Java developer's work simpler. The core idea is: "let me focus on the logic let someone else in the world take care of it". Instead of creating the objects yourself (which gives you control), you give the control of object creation to an external power, which is the "inversion" of control.

**Dependency Injection (DI)**
While IoC is just the philosophy, DI is the "actual implementation" and "design pattern" used to achieve IoC in Spring. In this case, Spring acts as the external power and says, "every time you want an object just ask for the object I will give it to you". Instead of you typing `new service`, you ask the Spring framework to inject the object using just the reference.

### The Code Representations


**The Old Way (Manual Object Creation)**
If a controller needs the object of a service, you previously had to manually use the new keyword.
```java
class Controller {
    // "we have to literally create an object by using a new keyword"
    Service service = new Service(); 
}
```

**1. Constructor Injection**
```java
class Controller {
    Service service;
    
    // "what you do in the controller class is you create a Constructor and in that Constructor you pass the reference of service"
    public Controller(Service service) {
        this.service = service;
    }
}
```

**2. Setter Injection**
```java
class Controller {
    Service service;
    
    // "you can create a set of methods for that service reference and you can do the set of injection"
    public void setService(Service service) {
        this.service = service;
    }
}
```

**3. Field Injection**
```java
class Controller {
    // "you just mentioned the reference spring will give you the object"
    Service service; 
}
```
