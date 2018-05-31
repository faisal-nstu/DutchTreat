using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;

        public OrdersController(IDutchRepository repository, ILogger<OrdersController> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(bool includeItems = true)
        {
            try
            {
                var response = _repository.GetAllOrders(includeItems);
                return Ok(_mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(response));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get Orders: {ex}");
                return BadRequest("Failed to get Orders.");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                var order = _repository.GetAllOrderById(id);
                if (order != null) return Ok(_mapper.Map<Order, OrderViewModel>(order));
                else return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get Orders: {ex}");
                return BadRequest("Failed to get Orders.");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderViewModel model)
        {
            // add it to the db
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);

                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    _repository.AddEntity(newOrder);
                    if (_repository.SaveChanges())
                    {
                        return Created($"/api/orders/{newOrder.Id}", _mapper.Map<Order, OrderViewModel>(newOrder));
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save new Order: {ex}");
            }

            return BadRequest("Failed to save new Order");
        }
    }
}
