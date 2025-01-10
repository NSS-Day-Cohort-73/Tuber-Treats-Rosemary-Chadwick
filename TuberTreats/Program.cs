using Microsoft.VisualBasic;
using TuberTreats.Models;
using TuberTreats.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

List<TuberDriver> tuberDrivers = new List<TuberDriver>()
{
    new TuberDriver { Id = 1, Name = "John Drive" },
    new TuberDriver { Id = 2, Name = "Sarah Speed" },
    new TuberDriver { Id = 3, Name = "Mike Rush" },
};

List<Customer> customers = new List<Customer>()
{
    new Customer
    {
        Id = 1,
        Name = "Alice Smith",
        Address = "123 Potato Lane",
    },
    new Customer
    {
        Id = 2,
        Name = "Bob Jones",
        Address = "456 Spud Street",
    },
    new Customer
    {
        Id = 3,
        Name = "Carol Wilson",
        Address = "789 Tuber Ave",
    },
    new Customer
    {
        Id = 4,
        Name = "David Brown",
        Address = "321 Yam Road",
    },
    new Customer
    {
        Id = 5,
        Name = "Eve Johnson",
        Address = "654 Sweet Potato Blvd",
    },
};

List<Topping> toppings = new List<Topping>()
{
    new Topping { Id = 1, Name = "Sour Cream" },
    new Topping { Id = 2, Name = "Chives" },
    new Topping { Id = 3, Name = "Bacon Bits" },
    new Topping { Id = 4, Name = "Cheese" },
    new Topping { Id = 5, Name = "Butter" },
};

List<TuberOrder> tuberOrders = new List<TuberOrder>()
{
    new TuberOrder
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = DateTime.Now.AddMinutes(30),
    },
    new TuberOrder
    {
        Id = 2,
        OrderPlacedOnDate = DateTime.Now.AddHours(1),
        CustomerId = 3,
        TuberDriverId = 2,
        DeliveredOnDate = DateTime.Now.AddHours(1).AddMinutes(30),
    },
    new TuberOrder
    {
        Id = 3,
        OrderPlacedOnDate = DateTime.Now.AddHours(2),
        CustomerId = 5,
        TuberDriverId = null,
        DeliveredOnDate = null,
    },
};

List<TuberTopping> tuberToppings = new List<TuberTopping>()
{
    new TuberTopping
    {
        Id = 1,
        TuberOrderId = 1,
        ToppingId = 1,
    },
    new TuberTopping
    {
        Id = 2,
        TuberOrderId = 1,
        ToppingId = 2,
    },
    new TuberTopping
    {
        Id = 3,
        TuberOrderId = 2,
        ToppingId = 3,
    },
    new TuberTopping
    {
        Id = 4,
        TuberOrderId = 2,
        ToppingId = 4,
    },
    new TuberTopping
    {
        Id = 5,
        TuberOrderId = 3,
        ToppingId = 5,
    },
};

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();

//add endpoints here

app.MapGet(
    "/tuberorders",
    () =>
    {
        return tuberOrders.Select(to => new TuberOrderDTO
        {
            Id = to.Id,
            OrderPlacedOnDate = to.OrderPlacedOnDate,
            CustomerId = to.CustomerId,
            TuberDriverId = to.TuberDriverId,
            DeliveredOnDate = to.DeliveredOnDate,
        });
    }
);

app.MapGet(
    "/tuberorders/{id}",
    (int id) =>
    {
        TuberOrder tuberOrder = tuberOrders.FirstOrDefault(to => to.Id == id);
        if (tuberOrder == null)
        {
            return Results.NotFound();
        }
        var orderDTO = new TuberOrderDTO
        {
            Id = tuberOrder.Id,
            OrderPlacedOnDate = tuberOrder.OrderPlacedOnDate,
            CustomerId = tuberOrder.CustomerId,
            TuberDriverId = tuberOrder.TuberDriverId,
            DeliveredOnDate = tuberOrder.DeliveredOnDate,
        };

        var orderCustomer = customers.FirstOrDefault(c => c.Id == tuberOrder.CustomerId);
        var customerDTO = new CustomerDTO
        {
            Id = orderCustomer.Id,
            Name = orderCustomer.Name,
            Address = orderCustomer.Address,
        };
        orderDTO.Customer = customerDTO;
        //var orderDriver = tuberOrders.FirstOrDefault(o => o.TuberDriverId == tuberOrder.TuberDriverId);
        //If TuberDriverId has been assigned, we can use .Value to get the actual ID and look up the driver
        TuberDriver orderDriver = null;
        if (tuberOrder.TuberDriverId.HasValue)
        {
            orderDriver = tuberDrivers.FirstOrDefault(d => d.Id == tuberOrder.TuberDriverId.Value);
        }
        if (orderDriver != null)
        {
            var driverDTO = new TuberDriverDTO { Id = orderDriver.Id, Name = orderDriver.Name };
            orderDTO.TuberDriver = driverDTO;
        }

        var orderToppings = tuberToppings.Where(ot => ot.TuberOrderId == tuberOrder.Id);
        var toppingDTOs = orderToppings
            .Select(ot =>
            {
                var topping = toppings.FirstOrDefault(t => t.Id == ot.ToppingId);
                return new ToppingDTO { Id = topping.Id, Name = topping.Name };
            })
            .ToList();
        orderDTO.Toppings = toppingDTOs;
        return Results.Ok(orderDTO);
    }
);

app.MapPost(
    "/tuberorders",
    (TuberOrder order) =>
    {
        order.Id = tuberOrders.Max(o => o.Id) + 1;
        order.OrderPlacedOnDate = DateTime.Now;
        tuberOrders.Add(order);

        return Results.Created(
            $"/tuberorders/{order.Id}",
            new TuberOrderDTO
            {
                Id = order.Id,
                OrderPlacedOnDate = order.OrderPlacedOnDate,
                CustomerId = order.CustomerId,
            }
        );
    }
);

//  TuberOrderDTO is a summary of what was created as confirmation


app.MapPut(
    "/tuberorders/{id}",
    (int id, int driverId) =>
    {
        TuberOrder selectedOrder = tuberOrders.FirstOrDefault(so => so.Id == id);
        if (selectedOrder == null)
        {
            return Results.NotFound();
        }
        selectedOrder.TuberDriverId = driverId;
        return Results.Ok(selectedOrder);
    }
);

// selectedOrder.Id = tuberOrders.Id,
// selectedOrder.OrderPlacedOnDate = tuberOrders.OrderPlacedOnDate,
// selectedOrder.CustomerId = tuberOrders.CustomerId,

app.MapPost(
    "/tuberorders/{id}/complete",
    (int id) =>
    {
        TuberOrder completeOrder = tuberOrders.FirstOrDefault(oc => oc.Id == id);

        if (completeOrder == null)
        {
            return Results.NotFound();
        }
        completeOrder.DeliveredOnDate = DateTime.Now;
        return Results.Ok(completeOrder);
    }
);
app.MapGet(
    "/toppings",
    () =>
    {
        return toppings.Select(t => new ToppingDTO { Id = t.Id, Name = t.Name });
    }
);
app.MapGet(
    "/toppings/{id}",
    (int id) =>
    {
        Topping topping = toppings.FirstOrDefault(t => t.Id == id);

        if (topping == null)
        {
            return Results.NotFound();
        }

        var toppingDTO = new ToppingDTO { Id = topping.Id, Name = topping.Name };

        return Results.Ok(toppingDTO);
    }
);
app.MapGet(
    "/tubertoppings",
    () =>
    {
        return tuberToppings.Select(tt => new TuberToppingDTO
        {
            Id = tt.Id,
            TuberOrderId = tt.TuberOrderId,
            ToppingId = tt.ToppingId,
        });
    }
);
app.MapPost(
    "/tubertoppings",
    (TuberToppingDTO tuberToppingDTO) =>
    {
        Topping topping = toppings.FirstOrDefault(t => t.Id == tuberToppingDTO.ToppingId);
        if (topping == null)
        {
            return Results.NotFound("No topping found.");
        }
        TuberOrder order = tuberOrders.FirstOrDefault(to => to.Id == tuberToppingDTO.TuberOrderId);
        if (order == null)
        {
            return Results.NotFound("No order found.");
        }

        int newId = toppings.Max(c => c.Id) + 1;

        var newTuberTopping = new TuberTopping
        {
            Id = newId,
            TuberOrderId = tuberToppingDTO.TuberOrderId,
            ToppingId = tuberToppingDTO.ToppingId,
        };
        tuberToppings.Add(newTuberTopping);

        return Results.Ok(
            new TuberToppingDTO
            {
                Id = newTuberTopping.Id,
                TuberOrderId = newTuberTopping.TuberOrderId,
                ToppingId = newTuberTopping.ToppingId,
            }
        );
    }
);
app.MapDelete(
    "/tubertoppings/{id}",
    (int id) =>
    {
        TuberTopping tuberTopping = tuberToppings.FirstOrDefault(tt => tt.Id == id);
        if (tuberTopping == null)
        {
            return Results.NotFound();
        }
        tuberToppings.Remove(tuberTopping);
        return Results.NoContent();
    }
);
app.MapGet(
    "/customers",
    () =>
    {
        return customers.Select(c => new CustomerDTO
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
        });
    }
);

app.MapGet(
    "/customers/{id}",
    (int id) =>
    {
        Customer customer = customers.FirstOrDefault(c => c.Id == id);

        if (customer == null)
        {
            return Results.NotFound();
        }

        var customerOrders = tuberOrders.Where(o => o.CustomerId == id);

        var customerDTO = new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address,
            TuberOrders = customerOrders
                .Select(order => new TuberOrderDTO
                {
                    Id = order.Id,
                    OrderPlacedOnDate = order.OrderPlacedOnDate,
                    CustomerId = order.CustomerId,
                    TuberDriverId = order.TuberDriverId,
                    DeliveredOnDate = order.DeliveredOnDate,
                })
                .ToList(),
        };

        return Results.Ok(customerDTO);
    }
);

//CreateCustomerDTO is the type or structure  --  customerDto is the name we give to the actual data when it arrives
//use string.IsNullOrEmpty() instead of == null because it checks for both null and empty strings
app.MapPost(
    "/customers",
    (CreateCustomerDTO customerDTO) =>
    {
        if (string.IsNullOrEmpty(customerDTO.Name) || string.IsNullOrEmpty(customerDTO.Address))
        {
            return Results.BadRequest("Name and address are required");
        }

        int newId = customers.Max(c => c.Id) + 1;

        var newCustomer = new Customer
        {
            Id = newId,
            Name = customerDTO.Name,
            Address = customerDTO.Address,
        };

        customers.Add(newCustomer);

        return Results.Ok(
            new CustomerDTO
            {
                Id = newCustomer.Id,
                Name = newCustomer.Name,
                Address = newCustomer.Address,
            }
        );
    }
);

app.MapDelete(
    "/customers/{id}",
    (int id) =>
    {
        Customer customer = customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            return Results.NotFound();
        }
        customers.Remove(customer);
        return Results.NoContent();
    }
);

app.MapGet(
    "/tuberdrivers",
    () =>
    {
        return tuberDrivers.Select(td => new TuberDriverDTO { Id = td.Id, Name = td.Name });
    }
);

app.MapGet(
    "/tuberdrivers/{id}",
    (int id) =>
    {
        TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == id);

        if (tuberDriver == null)
        {
            return Results.NotFound();
        }
        ;

        var driverDeliveries = tuberOrders.Where(o => o.TuberDriverId == id);

        var tuberDriverDTO = new TuberDriverDTO
        {
            Id = tuberDriver.Id,
            Name = tuberDriver.Name,
            TuberDeliveries = driverDeliveries
                .Select(deliveries => new TuberOrderDTO
                {
                    Id = deliveries.Id,
                    OrderPlacedOnDate = deliveries.OrderPlacedOnDate,
                    CustomerId = deliveries.CustomerId,
                    TuberDriverId = deliveries.TuberDriverId,
                    DeliveredOnDate = deliveries.DeliveredOnDate,
                })
                .ToList(),
        };
        return Results.Ok(tuberDriverDTO);
    }
);

app.Run();

//don't touch or move this!
public partial class Program { }
