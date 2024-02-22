﻿namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class NebimWarehouseTransferModel
    {
        public int ModelType { get; set; }
        public string? InnerNumber { get; set; }
        public string? OfficeCode { get; set; }
        public string? OperationDate { get; set; }
        public string? StoreCode { get; set; }
        public string? ToOfficeCode { get; set; }
        public string? ToStoreCode { get; set; }
        public string? ToWarehouseCode { get; set; }
        public string? WarehouseCode { get; set; }
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




}
