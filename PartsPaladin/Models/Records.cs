using System.ComponentModel.DataAnnotations;

namespace PartsPaladin.Models;

public class Records
{
    [Key]
    public int record_id { get; set; }
    public string customer_name { get; set; }
    public string order_date { get; set; }
    public string order_status { get; set; }
    public int order_total { get; set; }
}
