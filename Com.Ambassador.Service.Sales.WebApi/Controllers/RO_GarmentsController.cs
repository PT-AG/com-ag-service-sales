using AutoMapper;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.ROGarmentInterface;
using Com.Ambassador.Service.Sales.Lib.Models.ROGarments;
using Com.Ambassador.Service.Sales.Lib.PDFTemplates;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentROViewModels;
using Com.Ambassador.Service.Sales.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/ro-garments")]
    [Authorize]
    public class RO_GarmentsControllerprivate : BaseController<RO_Garment, RO_GarmentViewModel, IROGarment>
    {
        readonly static string apiVersion = "1.0";
        private readonly IIdentityService Service;

        private readonly ICostCalculationGarment costCalculationFacade;

        public RO_GarmentsControllerprivate(IIdentityService identityService, IValidateService validateService, IROGarment facade, IMapper mapper, IServiceProvider serviceProvider) : base(identityService, validateService, facade, mapper, apiVersion)
        {
            Service = identityService;
            costCalculationFacade = (ICostCalculationGarment)serviceProvider.GetService(typeof(ICostCalculationGarment));
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
                RO_Garment model = await Facade.ReadByIdAsync(Id);
                RO_GarmentViewModel viewModel = Mapper.Map<RO_GarmentViewModel>(model);

                if (model == null)
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, Common.NOT_FOUND_STATUS_CODE, Common.NOT_FOUND_MESSAGE)
                        .Fail();
                    return NotFound(Result);
                }
                else
                {
                    Service.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

                    var productIds = viewModel.CostCalculationGarment.CostCalculationGarment_Materials.Select(m => m.Product.Id).Distinct().ToList();
                    var productDicts = await costCalculationFacade.GetProductNames(productIds);

                    foreach (var material in viewModel.CostCalculationGarment.CostCalculationGarment_Materials)
                    {
                        material.Product.Name = productDicts.GetValueOrDefault(material.Product.Id);
                    }

                    ROGarmentPdfTemplate PdfTemplate = new ROGarmentPdfTemplate();
                    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, timeoffsset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "RO Garment " + viewModel.Code + ".pdf"
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


        [HttpPost("post")]
        public async Task<IActionResult> PostRO([FromBody]List<long> listId)
        {
            try
            {
                ValidateUser();

                int result = await Facade.PostRO(listId);
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
        public async Task<IActionResult> UnpostRO(long id)
        {
            try
            {
                ValidateUser();

                await Facade.UnpostRO(id);
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

        [HttpPut("reject-sample/{id}")]
        public async Task<IActionResult> RejectSample(int id, [FromBody] RO_GarmentViewModel viewModel)
        {
            try
            {
                ValidateUser();

                await Facade.RejectSample(id, viewModel);
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
    }
}