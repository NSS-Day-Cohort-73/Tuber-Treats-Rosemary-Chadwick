namespace TuberTreats.Models;

public class TuberOrder
{
    public int Id { get; set; }
    public DateTime OrderPlacedOnDate { get; set; }
    public DateTime? DeliveredOnDate { get; set; } //nullable if not delivered
    public int CustomerId { get; set; }
    public int? TuberDriverId { get; set; } //nullable if not assigned yet
    public List<Topping> Toppings { get; set; } = new List<Topping>();
    public Customer Customer { get; set; }
}
//Customer Customer { get; set; } - Navigation property that links to the full Customer object, letting you access customer details directly through the order...  order.Customer.Name
// TuberDriver TuberDriver { get; set; } - Links to the driver object
// List<TuberTopping> TuberToppings { get; set; } - Collection of toppings for this order

// These properties enable Entity Framework to navigate between related entities and make it easier to access related data through the order object... order.Customer.Name
