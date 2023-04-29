﻿using DocumentFormat.OpenXml.EMMA;
using FoodOrdersContracts.BindingModels;
using FoodOrdersContracts.BusinessLogicsContracts;
using FoodOrdersContracts.SearchModels;
using FoodOrdersContracts.ViewModels;
using FoodOrdersDataModels.Enums;
using Microsoft.Extensions.Logging;

namespace FoodOrdersBusinessLogic.BusinessLogics
{
    public class WorkModeling : IWorkProcess
    {
        private readonly ILogger _logger;

        private readonly Random _rnd;

        private IOrderLogic? _orderLogic;

        public WorkModeling(ILogger<WorkModeling> logger)
        {
            _logger = logger;
            _rnd = new Random(1000);
        }

        public void DoWork(IImplementerLogic implementerLogic, IOrderLogic orderLogic)
        {
            _orderLogic = orderLogic;
            var implementers = implementerLogic.ReadList(null);
            if (implementers == null)
            {
                _logger.LogWarning("DoWork. Implementer is null");
                return;
            }
            var orders = _orderLogic.ReadList(new OrderSearchModel { Status = OrderStatus.Принят });
            _logger.LogDebug("DoWork for {Count} orders", orders.Count);
            foreach (var implementer in implementers)
            {
                Task.Run(() => WorkerWorkAsync(implementer, orders));
            }
        }

        /// <summary>
        /// Иммитация работы исполнителя
        /// </summary>
        /// <param name="implementer"></param>
        /// <param name="orders"></param>
        private async Task WorkerWorkAsync(ImplementerViewModel implementer, List<OrderViewModel> orders)
        {
            if (_orderLogic == null || implementer == null)
            {
                return;
            }
            await RunExpectationInWork(implementer);
            await RunOrderInWork(implementer);

            await Task.Run(() =>
            {
                foreach (var order in orders)
                {
                    try
                    {
                        _logger.LogDebug("DoWork. Worker {Id} try get order {Order}", implementer.Id, order.Id);
                        // пытаемся назначить заказ на исполнителя
                        _orderLogic.TakeOrderInWork(new OrderBindingModel
                        {
                            Id = order.Id,
                            ImplementerId = implementer.Id
                        });
                        if (_orderLogic.ReadElement(new OrderSearchModel { Id = order.Id })!.Status == OrderStatus.Ожидание)
                        {
                            continue;
                        }
                        // делаем работу
                        Thread.Sleep(implementer.WorkExperience * _rnd.Next(100, 1000) * order.Count);
                        _logger.LogDebug("DoWork. Worker {Id} finish order {Order}", implementer.Id, order.Id);
                        _orderLogic.FinishOrder(new OrderBindingModel
                        {
                            Id = order.Id,
                            ImplementerId = implementer.Id
                        });
                        // отдыхаем
                        Thread.Sleep(implementer.Qualification * _rnd.Next(10, 100));
                    }
                    // кто-то мог уже перехватить заказ, игнорируем ошибку
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogWarning(ex, "Error try get work");
                    }
                    // заканчиваем выполнение имитации в случае иной ошибки
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while do work");
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// Ищем заказ, которые уже в работе (вдруг исполнителя прервали)
        /// </summary>
        /// <param name="implementer"></param>
        /// <returns></returns>
        private async Task RunOrderInWork(ImplementerViewModel implementer)
        {
            if (_orderLogic == null || implementer == null)
            {
                return;
            }
            try
            {
                var runOrder = await Task.Run(() => _orderLogic.ReadElement(new OrderSearchModel
                {
                    ImplementerId = implementer.Id,
                    Status = OrderStatus.Выполняется
                }));
                if (runOrder == null)
                {
                    return;
                }

                _logger.LogDebug("DoWork. Worker {Id} back to order {Order}", implementer.Id, runOrder.Id);
                // доделываем работу
                Thread.Sleep(implementer.WorkExperience * _rnd.Next(100, 300) * runOrder.Count);
                _logger.LogDebug("DoWork. Worker {Id} finish order {Order}", implementer.Id, runOrder.Id);
                _orderLogic.FinishOrder(new OrderBindingModel
                {
                    Id = runOrder.Id
                });
                // отдыхаем
                Thread.Sleep(implementer.Qualification * _rnd.Next(10, 100));
            }
            // заказа может не быть, просто игнорируем ошибку
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error try get work");
            }
            // а может возникнуть иная ошибка, тогда просто заканчиваем выполнение имитации
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while do work");
                throw;
            }
        }
        private async Task RunExpectationInWork(ImplementerViewModel implementer)
        {
            if (_orderLogic == null || implementer == null)
            {
                return;
            }
            var listExpectOrder = await Task.Run(() => _orderLogic.ReadList(new OrderSearchModel
            {
                ImplementerId = implementer.Id,
                Status = OrderStatus.Ожидание
            }));
            if (listExpectOrder == null)
            {
                return;
            }
            foreach (var order in listExpectOrder)
            {
                try
                {
                    _logger.LogDebug("DoWork. Worker {Id} back to order {Order}", implementer.Id, order.Id);
                    _logger.LogDebug("DoWork. Worker {Id} finish order {Order}", implementer.Id, order.Id);
                    _orderLogic.FinishOrder(new OrderBindingModel
                    {
                        Id = order.Id
                    });
                    // отдыхаем
                    Thread.Sleep(implementer.Qualification * _rnd.Next(10, 100));
                }
                // заказа может не быть, просто игнорируем ошибку
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Error try get work");
                }
                // а может возникнуть иная ошибка, тогда просто заканчиваем выполнение имитации
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while do work");
                    throw;
                }
            }

        }
    }
}