using AutoMapper;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentBookingOrderFacade;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentBookingOrderInterface;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentBookingOrderModel;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentBookingOrderViewModels;
using Com.Ambassador.Service.Sales.WebApi.Helpers;
using Com.Ambassador.Service.Sales.WebApi.Utilities;
using Com.Danliris.Service.Sales.Lib.ViewModels.GarmentBookingOrderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/sales/garment-booking-orders")]
    [Authorize]
    public class GarmentBookingOrderController : BaseController<GarmentBookingOrder, GarmentBookingOrderViewModel, IGarmentBookingOrder>
    {
        private readonly static string apiVersion = "1.0";
        private readonly IGarmentBookingOrder facades;
        private readonly IIdentityService Service;
        public GarmentBookingOrderController(IIdentityService identityService, IValidateService validateService, IGarmentBookingOrder facade, IMapper mapper, IServiceProvider serviceProvider) : base(identityService, validateService, facade, mapper, apiVersion)
        {
            facades = facade;
            Service = identityService;
        }

        [HttpPut("BOCancel/{id}")]
        public async Task<IActionResult> CancelLeftOvers([FromRoute]int id, [FromBody] GarmentBookingOrderViewModel viewModel)
        {
            IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            GarmentBookingOrder model = Mapper.Map<GarmentBookingOrder>(viewModel);
            try
            {
                await facades.BOCancel(id, model);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("BODelete/{id}")]
        public async Task<IActionResult> DeleteLeftOvers([FromRoute]int id, [FromBody] GarmentBookingOrderViewModel viewModel)
        {
            IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            GarmentBookingOrder model = Mapper.Map<GarmentBookingOrder>(viewModel);
            try
            {
                await facades.BODelete(id, model);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpGet("read-by-no")]
        public IActionResult Get(int page = 1, int size = 25, [Bind(Prefix = "Select[]")]List<string> select = null, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                ReadResponse<GarmentBookingOrder> read = Facade.ReadByBookingOrderNo(page, size, order, select, keyword, filter);

                List<GarmentBookingOrderViewModel> DataVM = Mapper.Map<List<GarmentBookingOrderViewModel>>(read.Data);

                Dictionary<string, object> Result =
                    new Utilities.ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok<GarmentBookingOrderViewModel>(Mapper, DataVM, page, size, read.Count, DataVM.Count, read.Order, read.Selected);
                return Ok(Result);

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new Utilities.ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("read-by-no-for-ccg")]
        public IActionResult GetForCCG(int page = 1, int size = 25, [Bind(Prefix = "Select[]")] List<string> select = null, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                ReadResponse<GarmentBookingOrderForCCGViewModel> read = Facade.ReadByBookingOrderNoForCCG(page, size, order, select, keyword, filter);

                List<GarmentBookingOrderForCCGViewModel> DataVM = Mapper.Map<List<GarmentBookingOrderForCCGViewModel>>(read.Data);

                Dictionary<string, object> Result =
                    new Utilities.ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok<GarmentBookingOrderForCCGViewModel>(Mapper, DataVM, page, size, read.Count, DataVM.Count, read.Order, read.Selected);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new Utilities.ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
