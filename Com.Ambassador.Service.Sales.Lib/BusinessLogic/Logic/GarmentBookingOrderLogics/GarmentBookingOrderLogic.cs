using Com.Ambassador.Service.Sales.Lib.Models.GarmentBookingOrderModel;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseClass;
using Com.Danliris.Service.Sales.Lib.ViewModels.GarmentBookingOrderViewModels;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentBookingOrderLogics
{
    public class GarmentBookingOrderLogic : BaseLogic<GarmentBookingOrder>
    {
        private readonly SalesDbContext DbContext;
        private GarmentBookingOrderItemLogic GarmentBookingOrderItemsLogic;
        public GarmentBookingOrderLogic(GarmentBookingOrderItemLogic GarmentBookingOrderItemsLogic, IIdentityService IdentityService, SalesDbContext dbContext) : base(IdentityService, dbContext)
        {
            this.GarmentBookingOrderItemsLogic = GarmentBookingOrderItemsLogic;
            this.DbContext = dbContext;
        }

        public override void Create(GarmentBookingOrder model)
        {
            GenerateBookingOrderNo(model);
            if (model.Items.Count > 0)
            {
                model.HadConfirmed = true;
            }

            EntityExtension.FlagForCreate(model, IdentityService.Username, "sales-service");
            DbSet.Add(model);
        }
        public override async Task<GarmentBookingOrder> ReadByIdAsync(long id)
        {
            var garmentBookingOrder = await DbSet.Include(p => p.Items).FirstOrDefaultAsync(d => d.Id.Equals(id) && d.IsDeleted.Equals(false));
            if(garmentBookingOrder!=null)
                garmentBookingOrder.Items = garmentBookingOrder.Items.Where(s => s.IsCanceled == false).OrderBy(s => s.Id).ToArray();
            return garmentBookingOrder;
        }

        public override void UpdateAsync(long id, GarmentBookingOrder newModel)
        {
            var model = DbSet.AsNoTracking().Include(d => d.Items).FirstOrDefault(d => d.Id == id);
            newModel.IsBlockingPlan = model.IsBlockingPlan;

            foreach (var item in model.Items)
            {
                if (item.IsCanceled == false)
                {
                    model.ConfirmedQuantity -= item.ConfirmQuantity;
                }
            }
            newModel.ConfirmedQuantity = model.ConfirmedQuantity;

            if (newModel.ConfirmedQuantity == 0)
            {
                newModel.HadConfirmed = false;
            }

            foreach (var newItem in newModel.Items)
            {
                if (newItem.IsCanceled == false)
                {
                    if (newItem.Id == 0)
                    {
                        GarmentBookingOrderItemsLogic.Create(newItem);
                        newModel.ConfirmedQuantity += newItem.ConfirmQuantity;
                        newModel.HadConfirmed = true;

                        EntityExtension.FlagForCreate(newItem, IdentityService.Username, "sales-service");
                    }
                    else
                    {
                        //GarmentBookingOrderItemsLogic.UpdateAsync(itemId, data);
                        newModel.ConfirmedQuantity += newItem.ConfirmQuantity;

                        EntityExtension.FlagForUpdate(newItem, IdentityService.Username, "sales-service");
                    }
                }
                else
                {
                    newItem.CanceledDate = DateTimeOffset.Now;
                    //newModel.ConfirmedQuantity -= newItem.ConfirmQuantity;
                    EntityExtension.FlagForUpdate(newItem, IdentityService.Username, "sales-service");
                    newModel.HadConfirmed = true;
                }

            }

            DbSet.Update(newModel);

            foreach (var oldItem in model.Items)
            {
                if (oldItem.IsCanceled == false)
                {
                    var newItem = newModel.Items.FirstOrDefault(i => i.Id == oldItem.Id);
                    if (newItem == null)
                    {
                        EntityExtension.FlagForDelete(oldItem, IdentityService.Username, "sales-service");
                        //newModel.ConfirmedQuantity -= oldItem.ConfirmQuantity;
                        DbContext.GarmentBookingOrderItems.Update(oldItem);
                    }
                }
                else
                {
                    newModel.HadConfirmed = true;
                }

            }

            EntityExtension.FlagForUpdate(newModel, IdentityService.Username, "sales-service");

            

            if (newModel.IsBlockingPlan == true)
            {
                var blockingPlan = DbContext.GarmentSewingBlockingPlans.FirstOrDefault(b => b.BookingOrderId == newModel.Id);
                if (blockingPlan != null)
                {
                    blockingPlan.Status = "Booking Ada Perubahan";
                }
            }

            
            //EntityExtension.FlagForUpdate(newModel, IdentityService.Username, "sales-service");
            //DbSet.Update(newModel);
        }

        public override async Task DeleteAsync(long id)
        {
            var model = await DbSet.Include(d => d.Items).FirstOrDefaultAsync(d => d.Id == id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, "sales-service", true);
            var blockingPlan = DbContext.GarmentSewingBlockingPlans.FirstOrDefault(b => b.BookingOrderId == model.Id);
            if (model.IsBlockingPlan == true && blockingPlan != null)
            {
                blockingPlan.Status = "Booking Dihapus";
            }
            
            foreach (var item in model.Items)
            {
                EntityExtension.FlagForDelete(item, IdentityService.Username, "sales-service", true);
            }
        }

        private void GenerateBookingOrderNo(GarmentBookingOrder model)
        {
            DateTime Now = DateTime.Now;
            string Year = Now.ToString("yy");

            string no = $"{model.SectionCode}-{model.BuyerCode}-{Year}-";
            int Padding = 5;

            var lastData = DbSet.IgnoreQueryFilters().Where(w => w.BookingOrderNo.StartsWith(no) && !w.IsDeleted).OrderByDescending(o => o.BookingOrderNo).FirstOrDefault();
            // DbContext
            if (lastData == null)
            {
                model.BookingOrderNo = no + "1".PadLeft(Padding, '0');
            }
            else
            {
                int lastNoNumber = Int32.Parse(lastData.BookingOrderNo.Replace(no, "")) + 1;
                model.BookingOrderNo = no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }

        }

        public override ReadResponse<GarmentBookingOrder> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentBookingOrder> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "BookingOrderNo","BuyerName"
            };

            Query = QueryHelper<GarmentBookingOrder>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentBookingOrder>.Filter(Query, FilterDictionary);
            
            List<string> SelectedFields = new List<string>()
            {
                  "Id", "BookingOrderNo", "BookingOrderDate", "SectionName", "BuyerName", "OrderQuantity", "LastModifiedUtc","Remark",
                    "IsBlockingPlan", "IsCanceled", "CanceledDate", "DeliveryDate", "CanceledQuantity", "ExpiredBookingDate", "ExpiredBookingQuantity",
                      "ConfirmedQuantity", "HadConfirmed","Items","BuyerCode","BuyerId"
            };

            Query = Query.Where(d => d.OrderQuantity > 0)
                 .Select(bo => new GarmentBookingOrder
                 {
                     Id = bo.Id,
                     BookingOrderNo = bo.BookingOrderNo,
                     BookingOrderDate = bo.BookingOrderDate,
                     BuyerCode = bo.BuyerCode,
                     BuyerId = bo.BuyerId,
                     BuyerName = bo.BuyerName,
                     SectionId = bo.SectionId,
                     SectionCode = bo.SectionCode,
                     SectionName = bo.SectionName,
                     DeliveryDate = bo.DeliveryDate,
                     OrderQuantity = bo.OrderQuantity,
                     Remark = bo.Remark,
                     IsBlockingPlan = bo.IsBlockingPlan,
                     IsCanceled = bo.IsCanceled,
                     CanceledDate = bo.CanceledDate,
                     CanceledQuantity = bo.CanceledQuantity,
                     ExpiredBookingDate = bo.ExpiredBookingDate,
                     ExpiredBookingQuantity = bo.ExpiredBookingQuantity,
                     ConfirmedQuantity = bo.ConfirmedQuantity,
                     HadConfirmed = bo.HadConfirmed,
                     LastModifiedUtc = bo.LastModifiedUtc,
                     Items = bo.Items.ToList()
                 });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentBookingOrder>.Order(Query, OrderDictionary);

            Pageable<GarmentBookingOrder> pageable = new Pageable<GarmentBookingOrder>(Query, page - 1, size);
            List<GarmentBookingOrder> data = pageable.Data.ToList<GarmentBookingOrder>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<GarmentBookingOrder>(data, totalData, OrderDictionary, SelectedFields);
        }

        public void BOCancel(int id, GarmentBookingOrder model)
        {
            double cancelsQuantity = 0;

            cancelsQuantity = model.OrderQuantity - model.ConfirmedQuantity;

            model.CanceledQuantity += cancelsQuantity;
            model.OrderQuantity -= cancelsQuantity;
            model.CanceledDate = DateTimeOffset.Now;
            foreach (var item in model.Items)
            {
                GarmentBookingOrderItemsLogic.UpdateAsync((int)item.Id, item);
            }

            if (model.ConfirmedQuantity == 0)
            {
                model.IsCanceled = true;
            }
            EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");

            if (model.IsBlockingPlan == true)
            {
                var blockingPlan = DbContext.GarmentSewingBlockingPlans.FirstOrDefault(b => b.BookingOrderId == model.Id);
                if (blockingPlan != null)
                {
                    if (model.OrderQuantity == 0)
                    {
                        blockingPlan.Status = "Booking Dibatalkan";
                    }
                    else if (model.OrderQuantity > 0 && model.CanceledQuantity > 0)
                    {
                        blockingPlan.Status = "Booking Ada Perubahan";
                    }
                }
            }

            DbSet.Update(model);
        }

        public void BODelete(int id, GarmentBookingOrder model)
        {
            double cancelsQuantity = 0;

            cancelsQuantity = model.OrderQuantity - model.ConfirmedQuantity;

            model.ExpiredBookingQuantity += cancelsQuantity;
            model.OrderQuantity -= cancelsQuantity;
            model.ExpiredBookingDate = DateTimeOffset.Now;

            foreach (var item in model.Items)
            {
                GarmentBookingOrderItemsLogic.UpdateAsync((int)item.Id, item);
            }

            EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
            if (model.IsBlockingPlan == true)
            {
                var blockingPlan = DbContext.GarmentSewingBlockingPlans.FirstOrDefault(b => b.BookingOrderId == model.Id);
                if (blockingPlan != null)
                {
                    if (model.OrderQuantity == 0)
                    {
                        blockingPlan.Status = "Booking Expired";
                    }
                    else if (model.OrderQuantity > 0 && model.ExpiredBookingQuantity > 0)
                    {
                        blockingPlan.Status = "Booking Ada Perubahan";
                    }
                }
            }
            DbSet.Update(model);
        }

        public ReadResponse<GarmentBookingOrder> ReadByBookingOrderNo(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentBookingOrder> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "BookingOrderNo"
            };

            Query = QueryHelper<GarmentBookingOrder>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentBookingOrder>.Filter(Query, FilterDictionary);

            List<string> SelectedFields = new List<string>()
            {
                  "BookingOrderNo"
            };

            Query = Query
                 .Select(bo => new GarmentBookingOrder
                 {
                     Id = bo.Id,
                     BookingOrderNo = bo.BookingOrderNo,
                 });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentBookingOrder>.Order(Query, OrderDictionary);

            Pageable<GarmentBookingOrder> pageable = new Pageable<GarmentBookingOrder>(Query, page - 1, size);
            List<GarmentBookingOrder> data = pageable.Data.ToList<GarmentBookingOrder>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<GarmentBookingOrder>(data, totalData, OrderDictionary, SelectedFields);
        }

        public ReadResponse<GarmentBookingOrderForCCGViewModel> ReadByBookingOrderNoForCCG(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentBookingOrder> Query1 = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "BookingOrderNo"
            };

            Query1 = QueryHelper<GarmentBookingOrder>.Search(Query1, SearchAttributes, keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(filter);
            //Query1 = QueryHelper<GarmentBookingOrder>.Filter(Query1, FilterDictionary);

            string buyer = FilterDictionary["BuyerCode"];
            string section = FilterDictionary["SectionCode"];
            string comodity = FilterDictionary["ComodityCode"];


            List<string> SelectedFields = new List<string>()
            {
                   "BookingOrderId", "BookingOrderItemId", "BookingOrderNo", "ConfirmDate", "BuyerId", "BuyerCode", "BuyerName", "SectionId", "SectionCode", "SectionName", "ComodityId", "ComodityCode", "ComodityName", "ConfirmQuantity"
            };

            var QueryX = (from a in Query1
                          join b in DbContext.GarmentBookingOrderItems on a.Id equals b.BookingOrderId
                          join c in DbContext.CostCalculationGarments on b.Id equals c.BookingOrderItemId into cc
                          from CCG in cc.DefaultIfEmpty()
                          where a.HadConfirmed == true && a.IsCanceled == false && b.IsCanceled == false
                                && a.BuyerCode == buyer && a.SectionCode == section && b.ComodityCode == comodity

                          select new GarmentBookingOrderForCCGViewModel
                          {
                              BookingOrderId = a.Id,
                              BookingOrderItemId = b.Id,
                              BookingOrderNo = a.BookingOrderNo,
                              ConfirmDate = b.ConfirmDate,
                              BuyerId = a.BuyerId,
                              BuyerCode = a.BuyerCode,
                              BuyerName = a.BuyerName,
                              SectionId = a.SectionId,
                              SectionCode = a.SectionCode,
                              SectionName = a.SectionName,
                              ComodityId = b.ComodityId,
                              ComodityCode = b.ComodityCode,
                              ComodityName = b.ComodityName,
                              ConfirmQuantity = b.ConfirmQuantity,
                              CCQuantity = CCG == null ? 0 : CCG.Quantity,
                          });

            var QueryY = (from x in QueryX
                          group new { Qty = x.CCQuantity } by new
                          {
                              x.BookingOrderId,
                              x.BookingOrderNo,
                              x.BookingOrderItemId,
                              x.ConfirmDate,
                              x.ConfirmQuantity,
                              x.BuyerId,
                              x.BuyerCode,
                              x.BuyerName,
                              x.SectionId,
                              x.SectionCode,
                              x.SectionName,
                              x.ComodityId,
                              x.ComodityCode,
                              x.ComodityName
                          } into G

                          select new GarmentBookingOrderForCCGViewModel
                          {
                              BookingOrderId = G.Key.BookingOrderId,
                              BookingOrderItemId = G.Key.BookingOrderItemId,
                              BookingOrderNo = G.Key.BookingOrderNo,
                              ConfirmDate = G.Key.ConfirmDate,
                              BuyerId = G.Key.BuyerId,
                              BuyerCode = G.Key.BuyerCode,
                              BuyerName = G.Key.BuyerName,
                              SectionId = G.Key.SectionId,
                              SectionCode = G.Key.SectionCode,
                              SectionName = G.Key.SectionName,
                              ComodityId = G.Key.ComodityId,
                              ComodityCode = G.Key.ComodityCode,
                              ComodityName = G.Key.ComodityName,
                              ConfirmQuantity = (G.Key.ConfirmQuantity * 1.05) - G.Sum(m => m.Qty),
                          }).OrderBy(x => x.BookingOrderNo).ThenBy(x => x.SectionCode).ThenBy(x => x.BuyerCode).ThenBy(x => x.ComodityCode);


            var Query = (from a in QueryY
                         where a.ConfirmQuantity > 0

                         select new GarmentBookingOrderForCCGViewModel
                         {
                             BookingOrderId = a.BookingOrderId,
                             BookingOrderItemId = a.BookingOrderItemId,
                             BookingOrderNo = a.BookingOrderNo,
                             ConfirmDate = a.ConfirmDate,
                             BuyerId = a.BuyerId,
                             BuyerCode = a.BuyerCode,
                             BuyerName = a.BuyerName,
                             SectionId = a.SectionId,
                             SectionCode = a.SectionCode,
                             SectionName = a.SectionName,
                             ComodityId = a.ComodityId,
                             ComodityCode = a.ComodityCode,
                             ComodityName = a.ComodityName,
                             ConfirmQuantity = a.ConfirmQuantity,
                         });
            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //Query = QueryHelper<GarmentBookingOrder>.Order(Query, OrderDictionary);

            Pageable<GarmentBookingOrderForCCGViewModel> pageable = new Pageable<GarmentBookingOrderForCCGViewModel>(Query, page - 1, size);
            List<GarmentBookingOrderForCCGViewModel> data = pageable.Data.ToList<GarmentBookingOrderForCCGViewModel>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<GarmentBookingOrderForCCGViewModel>(data, totalData, OrderDictionary, SelectedFields);
        }
        public ReadResponse<GarmentBookingOrder> ReadExpired(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentBookingOrder> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "BookingOrderNo","BuyerName"
            };

            Query = QueryHelper<GarmentBookingOrder>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentBookingOrder>.Filter(Query, FilterDictionary);

            List<string> SelectedFields = new List<string>()
            {
                  "Id", "BookingOrderNo", "BookingOrderDate", "SectionName", "BuyerName", "OrderQuantity", "LastModifiedUtc","Remark",
                    "IsBlockingPlan", "IsCanceled", "CanceledDate", "DeliveryDate", "CanceledQuantity", "ExpiredBookingDate", "ExpiredBookingQuantity",
                      "ConfirmedQuantity", "HadConfirmed","Items","BuyerCode","BuyerId"
            };
            var today = DateTime.Today;
            Query = Query
                .Where(d => d.ConfirmedQuantity<d.OrderQuantity && d.DeliveryDate <= today.AddDays(45))
                 .Select(bo => new GarmentBookingOrder
                 {
                     Id = bo.Id,
                     BookingOrderNo = bo.BookingOrderNo,
                     BookingOrderDate = bo.BookingOrderDate,
                     BuyerCode = bo.BuyerCode,
                     BuyerId = bo.BuyerId,
                     BuyerName = bo.BuyerName,
                     SectionId = bo.SectionId,
                     SectionCode = bo.SectionCode,
                     SectionName = bo.SectionName,
                     DeliveryDate = bo.DeliveryDate,
                     OrderQuantity = bo.OrderQuantity,
                     Remark = bo.Remark,
                     IsBlockingPlan = bo.IsBlockingPlan,
                     IsCanceled = bo.IsCanceled,
                     CanceledDate = bo.CanceledDate,
                     CanceledQuantity = bo.CanceledQuantity,
                     ExpiredBookingDate = bo.ExpiredBookingDate,
                     ExpiredBookingQuantity = bo.ExpiredBookingQuantity,
                     ConfirmedQuantity = bo.ConfirmedQuantity,
                     HadConfirmed = bo.HadConfirmed,
                     LastModifiedUtc = bo.LastModifiedUtc,
                     Items = bo.Items.ToList()
                 });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentBookingOrder>.Order(Query, OrderDictionary);

            Pageable<GarmentBookingOrder> pageable = new Pageable<GarmentBookingOrder>(Query, page - 1, size);
            List<GarmentBookingOrder> data = pageable.Data.ToList<GarmentBookingOrder>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<GarmentBookingOrder>(data, totalData, OrderDictionary, SelectedFields);
        }

        public int BOCancelExpired(List<GarmentBookingOrder> list, string user)
        {
            int Updated = 0;
            double cancelsQuantity = 0;
            using (var transaction = this.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var Ids = list.Select(d => d.Id).ToList();
                    var listData = this.DbSet
                        .Where(m => Ids.Contains(m.Id) && !m.IsDeleted)
                        .Include(d => d.Items)
                        .ToList();
                    listData.ForEach(m =>
                    {
                        cancelsQuantity = 0;

                        cancelsQuantity = m.OrderQuantity - m.ConfirmedQuantity;

                        m.ExpiredBookingQuantity += cancelsQuantity;
                        m.OrderQuantity -= cancelsQuantity;
                        m.ExpiredBookingDate = DateTimeOffset.Now;
                        foreach (var item in m.Items)
                        {
                            GarmentBookingOrderItemsLogic.UpdateAsync((int)item.Id, item);
                        }

                        if (m.ConfirmedQuantity == 0)
                        {
                            m.IsCanceled = true;
                        }
                        EntityExtension.FlagForUpdate(m, IdentityService.Username, "sales-service");

                        if (m.IsBlockingPlan == true)
                        {
                            var blockingPlan = DbContext.GarmentSewingBlockingPlans.FirstOrDefault(b => b.BookingOrderId == m.Id);
                            if (blockingPlan != null)
                            {
                                if (m.ConfirmedQuantity == 0)
                                {
                                    blockingPlan.Status = "Booking Expired";
                                }
                                else if (m.ConfirmedQuantity > 0)
                                {
                                    blockingPlan.Status = "Booking Ada Perubahan";
                                }
                            }
                        }
                    });
                    Updated = DbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Updated;
        }
    }
}
