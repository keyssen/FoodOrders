﻿using FoodOrdersDataModels.Enums;

namespace FoodOrdersDataModels.Models
{
    public interface IOrderModel : IId
    {
        int DishId { get; }
        int Count { get; }
        double Sum { get; }
        OrderStatus Status { get; }
        DateTime DateCreate { get; }
        DateTime? DateImplement { get; }
    }
}
