using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Ambassador.Service.Sales.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Com.Ambassador.Service.Sales.Lib.PDFTemplates;
using Com.Ambassador.Service.Sales.Lib.Helpers;
using Microsoft.AspNetCore.JsonPatch;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Com.Ambassador.Service.Sales.Lib.ViewModels.IntegrationViewModel;

namespace Com.Ambassador.Service.Sales.WebApi.Controllers
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("v{version:apiVersion}/cost-calculation-garments")]
	[Authorize]
	public class CostCalculationGarmentController : BaseController<CostCalculationGarment, CostCalculationGarmentViewModel, ICostCalculationGarment>
	{
		private readonly static string apiVersion = "1.0";
		private readonly IIdentityService Service;
        private readonly IHttpClientService HttpClientService;
        public CostCalculationGarmentController(IIdentityService identityService, IValidateService validateService, ICostCalculationGarment facade, IMapper mapper, IServiceProvider serviceProvider) : base(identityService, validateService, facade, mapper, apiVersion)
		{
			Service = identityService;
            HttpClientService = serviceProvider.GetService<IHttpClientService>();
        }

        [HttpGet("dynamic")]
        public IActionResult GetDynamic(int page = 1, int size = 25, string order = "{}", string select = null, string keyword = null, string filter = "{}", string search = "[]")
        {
            try
            {
                ValidateUser();

                ReadResponse<dynamic> read = Facade.ReadDynamic(page, size, order, select, keyword, filter, search);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok(Mapper, read.Data, page, size, read.Count, read.Data.Count, read.Order, read.Selected);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("materials/dynamic")]
        public IActionResult GetMaterials(int page = 1, int size = 25, string order = "{}", string select = null, string keyword = null, string filter = "{}", string search = "[]")
        {
            try
            {
                ValidateUser();

                ReadResponse<dynamic> read = Facade.ReadMaterials(page, size, order, select, keyword, filter, search);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok(Mapper, read.Data, page, size, read.Count, read.Data.Count, read.Order, read.Selected);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

		[HttpGet("pdf/{id}")]
		public async Task<IActionResult> GetPDF([FromRoute]int Id)
		{
			if (!ModelState.IsValid)
			{
			    return BadRequest(ModelState);
			}

			try
			{
				var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
				int timeoffsset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
				CostCalculationGarment model = await Facade.ReadByIdAsync(Id);
				CostCalculationGarmentViewModel viewModel = Mapper.Map<CostCalculationGarmentViewModel>(model);

				if (model == null)
				{
					Dictionary<string, object> Result =
						new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
						.Fail();
					return NotFound(Result);
				}
				else
				{
                    //Get GarmentCategory
                    List<GarmentCategoryViewModel> garmentCategoryViewModel;
                    var response = HttpClientService.GetAsync($@"{Lib.Helpers.APIEndpoint.Core}master/garment-categories?size=10000").Result.Content.ReadAsStringAsync();
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                    garmentCategoryViewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());

                    CostCalculationGarmentPdfTemplate PdfTemplate = new CostCalculationGarmentPdfTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, timeoffsset, garmentCategoryViewModel);

                    return new FileStreamResult(stream, "application/pdf")
					{
						FileDownloadName = "Cost Calculation Garment " + viewModel.RO_Number + (viewModel.IsPosted ? "" : " - DRAFT") + ".pdf"
					};

				}
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

		[HttpGet("budget/{id}")]
		public async Task<IActionResult> GetBudget([FromRoute]int Id)
		{
			try
			{
				Service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
				Service.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

				//await Service.GeneratePO(Id);
				CostCalculationGarment model = await Facade.ReadByIdAsync(Id);
				CostCalculationGarmentViewModel viewModel = Mapper.Map<CostCalculationGarmentViewModel>(model);

				int timeoffsset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);


                //Get GarmentCategory
                List<GarmentCategoryViewModel> garmentCategoryViewModel;
                var response = HttpClientService.GetAsync($@"{Lib.Helpers.APIEndpoint.Core}master/garment-categories?size=10000").Result.Content.ReadAsStringAsync();
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                garmentCategoryViewModel = JsonConvert.DeserializeObject<List<GarmentCategoryViewModel>>(result.GetValueOrDefault("data").ToString());


                CostCalculationGarmentBudgetPdfTemplate PdfTemplate = new CostCalculationGarmentBudgetPdfTemplate();
				MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, timeoffsset, garmentCategoryViewModel);

				return new FileStreamResult(stream, "application/pdf")
				{
					FileDownloadName = "Budget Garment " + viewModel.RO_Number + (viewModel.IsPosted ? "" : " - DRAFT") + ".pdf"
				};
			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

        [HttpGet("with-product-names/{Id}")]
        public async Task<IActionResult> GetById_RO_Garment_Validation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                Service.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                CostCalculationGarment model = await Facade.ReadByIdAsync(id);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    CostCalculationGarmentViewModel viewModel = Mapper.Map<CostCalculationGarmentViewModel>(model);

                    var productIds = viewModel.CostCalculationGarment_Materials.Select(m => m.Product.Id).Distinct().ToList();
                    var productDicts = await Facade.GetProductNames(productIds);

                    foreach (var material in viewModel.CostCalculationGarment_Materials)
                    {
                        material.Product.Name = productDicts.GetValueOrDefault(material.Product.Id);
                    }

                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                        .Ok<CostCalculationGarmentViewModel>(viewModel);
                    return Ok(Result);
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch([FromRoute]int id, [FromBody]JsonPatchDocument<CostCalculationGarment> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var model = await Facade.ReadByIdAsync(id);
                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    ValidateUser();

                    await Facade.Patch(id, patch);

                    return NoContent();
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("isvalidate-ro-sample/{Id}")]
        public async Task<IActionResult> PutRoSample([FromRoute] int id, [FromBody] CostCalculationGarmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var model = await Facade.ReadByIdAsync(id);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }

                model.IsValidatedROSample = true;
                IdentityService.Username = User.Claims.ToArray().SingleOrDefault(p => p.Type.Equals("username")).Value;
                IdentityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                await Facade.UpdateAsync(id, model);

                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("ro-acceptance")]
        public IActionResult GetForROAcceptance(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            try
            {
                ReadResponse<CostCalculationGarment> read = Facade.ReadForROAcceptance(page, size, order, select, keyword, filter);

                //Tuple<List<TModel>, int, Dictionary<string, string>, List<string>> Data = Facade.Read(page, size, order, select, keyword, filter);
                List<CostCalculationGarmentViewModel> DataVM = Mapper.Map<List<CostCalculationGarmentViewModel>>(read.Data);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok<CostCalculationGarmentViewModel>(Mapper, DataVM, page, size, read.Count, DataVM.Count, read.Order, read.Selected);
                return Ok(Result);

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("acceptance")]
        public async Task<IActionResult> AcceptCC([FromBody]List<long> listId)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await Facade.AcceptanceCC(listId, IdentityService.Username);

                return Ok();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("ro-available")]
        public IActionResult GetForROAvailable(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            try
            {
                ReadResponse<CostCalculationGarment> read = Facade.ReadForROAvailable(page, size, order, select, keyword, filter);

                //Tuple<List<TModel>, int, Dictionary<string, string>, List<string>> Data = Facade.Read(page, size, order, select, keyword, filter);
                List<CostCalculationGarmentViewModel> DataVM = Mapper.Map<List<CostCalculationGarmentViewModel>>(read.Data);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok<CostCalculationGarmentViewModel>(Mapper, DataVM, page, size, read.Count, DataVM.Count, read.Order, read.Selected);
                return Ok(Result);

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("available")]
        public async Task<IActionResult> AvailableCC([FromBody]List<long> listId)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await Facade.AvailableCC(listId, IdentityService.Username);

                return Ok();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("ro-distribute")]
        public IActionResult GetForRODistribute(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            try
            {
                ReadResponse<CostCalculationGarment> read = Facade.ReadForRODistribution(page, size, order, select, keyword, filter);

                //Tuple<List<TModel>, int, Dictionary<string, string>, List<string>> Data = Facade.Read(page, size, order, select, keyword, filter);
                List<CostCalculationGarmentViewModel> DataVM = Mapper.Map<List<CostCalculationGarmentViewModel>>(read.Data);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok<CostCalculationGarmentViewModel>(Mapper, DataVM, page, size, read.Count, DataVM.Count, read.Order, read.Selected);
                return Ok(Result);

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("distribute")]
        public async Task<IActionResult> DistributeCC([FromBody]List<long> listId)
        {
            try
            {
                IdentityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                await Facade.DistributeCC(listId, IdentityService.Username);

                return Ok();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostCC([FromBody]List<long> listId)
        {
            try
            {
                ValidateUser();

                int result = await Facade.PostCC(listId);
                if (result < 1)
                {
                    Dictionary<string, object> Result = 
                        new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, "No changes applied.")
                        .Fail();
                    return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
                }
                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("unpost/{id}")]
        public async Task<IActionResult> UnpostCC(long id, [FromBody]string reason)
        {
            try
            {
                ValidateUser();

                if (string.IsNullOrWhiteSpace(reason))
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.BAD_REQUEST_STATUS_CODE, Common.BAD_REQUEST_MESSAGE)
                        .Fail("Alasan tidak diisi.");
                    return BadRequest(Result);

                }

                await Facade.UnpostCC(id, reason);
                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("unpost-reason-creators")]
        public IActionResult ReadUnpostReasonCreators(string keyword = "", int page = 1, int size = 25)
        {
            try
            {
                List<string> creators = Facade.ReadUnpostReasonCreators(keyword, page, size);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok(creators);
                return Ok(Result);

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

		[HttpGet("data")]
		public IActionResult GetComodityQtyOrderHoursBuyerByRo([FromBody]string ro)
		{
			try
			{
				var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
				var viewModel = Facade.GetComodityQtyOrderHoursBuyerByRo(ro);
				Dictionary<string, object> Result =
				new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
				.Ok(viewModel);
				return Ok(Result);

			}
			catch (Exception e)
			{
				Dictionary<string, object> Result =
					new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
					.Fail();
				return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
			}
		}

        [HttpGet("dataforjournal")]
        public IActionResult GetCCByRo( string RO_Number)
        {
            try
            {
                var viewModel = Facade.GetCCGByRo(RO_Number);
                Dictionary<string, object> Result =
                new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                .Ok(viewModel);
                return Ok(Result);

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("materials/by-prmasteritemids")]
        public IActionResult GetMaterialsByPRMasterItemIds(int page = 1, int size = 25, string order = "{}", string select = null, string keyword = null, string filter = "{}", string search = "[]", string prmasteritemids = "[]")
        {
            try
            {
                ValidateUser();

                ReadResponse<dynamic> read = Facade.ReadMaterialsByPRMasterItemIds(page, size, order, select, keyword, filter, search, prmasteritemids);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok(Mapper, read.Data, page, size, read.Count, read.Data.Count, read.Order, read.Selected);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("by-ro/{ro}")]
        public async Task<IActionResult> GetByRO([FromRoute]string ro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Service.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                Service.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                CostCalculationGarment model = await Facade.ReadByRO(ro);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    CostCalculationGarmentViewModel viewModel = Mapper.Map<CostCalculationGarmentViewModel>(model);

                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                        .Ok<CostCalculationGarmentViewModel>(viewModel);
                    return Ok(Result);
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}