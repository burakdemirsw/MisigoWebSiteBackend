using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class NebimWarehouseTransferModel
    {
        public int ModelType { get; set; }
        public string InnerNumber { get; set; }
        public string OfficeCode { get; set; }
        public string OperationDate { get; set; }
        public string StoreCode { get; set; }
        public string ToOfficeCode { get; set; }
        public string ToStoreCode { get; set; }
        public string ToWarehouseCode { get; set; }
        public string WarehouseCode { get; set; }
        public int CompanyCode { get; set; }
        public int InnerProcessType { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsInnerOrderBase { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPostingJournal { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsReturn { get; set; }
        public bool IsTransferApproved { get; set; }
        public List<NebimWarehouseTransferLineModel> Lines { get; set; }

    }
    public class NebimWarehouseTransferITAttributeModel
    {
        public string AttributeCode { get; set; }
        public int AttributeTypeCode { get; set; }
    }

    public class NebimWarehouseTransferLineModel
    {
        public int ItemTypeCode { get; set; }
        public string UsedBarcode { get; set; }
        public string ItemCode { get; set; }
        public string ColorCode { get; set; }
        public string ItemDim1Code { get; set; }
        public string BatchCode { get; set; }
        public int Qty1 { get; set; }
        public List<NebimWarehouseTransferITAttributeModel> ITAttributes { get; set; }
    }

}
