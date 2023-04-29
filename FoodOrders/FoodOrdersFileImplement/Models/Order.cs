﻿using FoodOrdersContracts.BindingModels;
using FoodOrdersContracts.ViewModels;
using FoodOrdersDataModels.Enums;
using FoodOrdersDataModels.Models;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace FoodOrdersFileImplement.Models
{
    [DataContract]
    public class Order : IOrderModel
    {
        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public int DishId { get; private set; }
        [DataMember]
        public int ClientId { get; private set; }
        [DataMember]
        public int? ImplementerId { get; set; }
        [DataMember]
        public int Count { get; private set; }
        [DataMember]
        public double Sum { get; private set; }
        [DataMember]
        public OrderStatus Status { get; private set; }
        [DataMember]
        public DateTime DateCreate { get; private set; }
        [DataMember]
        public DateTime? DateImplement { get; private set; }

        public static Order? Create(XElement element)
        {
            if (element == null)
            {
                return null;
            }
            return new Order()
            {
                Id = Convert.ToInt32(element.Attribute("Id")!.Value),
                ClientId = Convert.ToInt32(element.Element("ClientId")!.Value),
                ImplementerId = Convert.ToInt32(element.Element("ImplementerId")!.Value),
                DishId = Convert.ToInt32(element.Element("DishId")!.Value),
                Sum = Convert.ToDouble(element.Element("Sum")!.Value),
                Count = Convert.ToInt32(element.Element("Count")!.Value),
                Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), element.Element("Status")!.Value),
                DateCreate = Convert.ToDateTime(element.Element("DateCreate")!.Value),
                DateImplement = string.IsNullOrEmpty(element.Element("DateImplement")!.Value) ? null : Convert.ToDateTime(element.Element("DateImplement")!.Value)
            };
        }

        public static Order? Create(OrderBindingModel? model)
        {
            if (model == null)
            {
                return null;
            }
            return new Order
            {
                Id = model.Id,
                DishId = model.DishId,
                ClientId = model.ClientId,
                ImplementerId = model.ImplementerId,
                Count = model.Count,
                Sum = model.Sum,
                Status = model.Status,
                DateCreate = model.DateCreate,
                DateImplement = model.DateImplement,
            };
        }
        public void Update(OrderBindingModel? model)
        {
            if (model == null)
            {
                return;
            }
            Status = model.Status;
            DateImplement = model.DateImplement;
            ImplementerId = model.ImplementerId;
        }
        public OrderViewModel GetViewModel => new()
        {
            Id = Id,
            DishId = DishId,
            ClientId = ClientId,
            ImplementerFIO = DataFileSingleton.GetInstance().Implementers.FirstOrDefault(x => x.Id == ImplementerId)?.ImplementerFIO ?? string.Empty,
            Count = Count,
            Sum = Sum,
            Status = Status,
            DateCreate = DateCreate,
            DateImplement = DateImplement,
            ImplementerId = ImplementerId
        };

        public XElement GetXElement => new(
          "Order",
           new XAttribute("Id", Id),
           new XElement("DishId", DishId.ToString()),
           new XElement("ClientId", ClientId.ToString()),
           new XElement("ImplementerId", ClientId.ToString()),
           new XElement("Count", Count.ToString()),
           new XElement("Sum", Sum.ToString()),
           new XElement("Status", Status.ToString()),
           new XElement("DateCreate", DateCreate.ToString()),
           new XElement("DateImplement", DateImplement.ToString())
      );
    }
}
