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

        // koneki AG appps MDP --API--//
        [HttpGet("RoWithComponent")]
        public IActionResult RoWithComponent(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")] List<string> select = null, string keyword = null, string filter = "{}")
        {
            try
            {
                ValidateUser();

                var data = Facade.RoWithComponent(page, size, order, select, keyword, filter);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, Common.OK_STATUS_CODE, Common.OK_MESSAGE)
                    .Ok(data.ToList());
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

        #region Report Reject RO
        [HttpGet("reject-ro/report")]
        public IActionResult GetReportRejectRO(DateTime? dateFrom, DateTime? dateTo, int page, int size)
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var data = costCalculationFacade.ReadRejectRO(DateFrom, DateTo, page, size, offset);

                return Ok(new
                {
                    apiVersion = apiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
                    message = Common.OK_MESSAGE,
                    statusCode = Common.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new Utilities.ResultFormatter(apiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("reject-ro/download")]
        public IActionResult GetXlsRejectRO(DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = costCalculationFacade.GenerateExcelReadRejectRO(dateFrom, dateTo, offset);

                string filename = String.Format("Laporan Reject RO Garment - {0} - {1}.xlsx", DateFrom.ToString("dd-MM-yyyy"), DateTo.ToString("dd-MM-yyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new Utilities.ResultFormatter(apiVersion, Common.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(Common.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
        #endregion

    }
}