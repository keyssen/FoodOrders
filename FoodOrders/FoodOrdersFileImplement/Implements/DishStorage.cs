﻿using FoodOrdersContracts.BindingModels;
using FoodOrdersContracts.SearchModels;
using FoodOrdersContracts.StoragesContracts;
using FoodOrdersContracts.ViewModels;
using FoodOrdersFileImplement.Models;
using FoodOrdersFileImplement;

namespace FoodOrdersListImplement.Implements
{
    public class DishStorage : IDishStorage
    {
        private readonly DataFileSingleton _source;
        public DishStorage()
        {
            _source = DataFileSingleton.GetInstance();
        }
        public List<DishViewModel> GetFullList()
        {
            return source.Components.Select(x => x.GetViewModel).ToList();
        }
        public List<DishViewModel> GetFilteredList(DishSearchModel
       model)
        {
            var result = new List<DishViewModel>();
            if (string.IsNullOrEmpty(model.DishName))
            {
                return result;
            }
            foreach (var dish in _source.Dishes)
            {
                if (dish.DishName.Contains(model.DishName))
                {
                    result.Add(dish.GetViewModel);
                }
            }
            return result;
        }
        public DishViewModel? GetElement(DishSearchModel model)
        {
            if (string.IsNullOrEmpty(model.DishName) && !model.Id.HasValue)
            {
                return null;
            }
            foreach (var dish in _source.Dishes)
            {
                if ((!string.IsNullOrEmpty(model.DishName) &&
                dish.DishName == model.DishName) ||
                (model.Id.HasValue && dish.Id == model.Id))
                {
                    return dish.GetViewModel;
                }
            }
            return null;
        }
        public DishViewModel? Insert(DishBindingModel model)
        {
            model.Id = 1;
            foreach (var dish in _source.Dishes)
            {
                if (model.Id <= dish.Id)
                {
                    model.Id = dish.Id + 1;
                }
            }
            var newDish = Dish.Create(model);
            if (newDish == null)
            {
                return null;
            }
            _source.Dishes.Add(newDish);
            return newDish.GetViewModel;
        }
        public DishViewModel? Update(DishBindingModel model)
        {
            foreach (var dish in _source.Dishes)
            {
                if (dish.Id == model.Id)
                {
                    dish.Update(model);
                    return dish.GetViewModel;
                }
            }
            return null;
        }
        public DishViewModel? Delete(DishBindingModel model)
        {
            for (int i = 0; i < _source.Dishes.Count; ++i)
            {
                if (_source.Dishes[i].Id == model.Id)
                {
                    var element = _source.Dishes[i];
                    _source.Dishes.RemoveAt(i);
                    return element.GetViewModel;
                }
            }
            return null;
        }
    }
}