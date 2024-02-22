namespace GoogleAPI.Domain.Models.NEBIM.Request
{


    public class ErrorResponseModel
    {
        //[JsonProperty("ModelType")]
        public int ModelType { get; set; }

        //[JsonProperty("ExceptionMessage")]
        public string? ExceptionMessage { get; set; }

        //[JsonProperty("StackTrace")]
        public string? StackTrace { get; set; }

        //[JsonProperty("ErrorSource")]
        public string? ErrorSource { get; set; }

        //[JsonProperty("StatusCode")]
        public int StatusCode { get; set; }
    }
    public class ExceptionMessage
    {
        //[JsonProperty("ItemInfo")]
        public ItemInfo ItemInfo { get; set; }

        //[JsonProperty("Message")]
        public string? Message { get; set; }

    }


    public class ItemInfo
    {
        //[JsonProperty("ItemTypeCode")]
        public int ItemTypeCode { get; set; }

        //[JsonProperty("ItemCode")]
        public string? ItemCode { get; set; }

        //[JsonProperty("ItemDescription")]
        public string? ItemDescription { get; set; }

        //[JsonProperty("ColorCode")]
        public string? ColorCode { get; set; }

        //[JsonProperty("ItemDim1Code")]
        public string? ItemDim1Code { get; set; }

        //[JsonProperty("ItemDim2Code")]
        public string? ItemDim2Code { get; set; }

        //[JsonProperty("ItemDim3Code")]
        public string? ItemDim3Code { get; set; }

        //[JsonProperty("BatchCode")]
        public string? BatchCode { get; set; }

        //[JsonProperty("SectionCode")]
        public string? SectionCode { get; set; }

        //[JsonProperty("Quantity")]
        public int Quantity { get; set; }

        //[JsonProperty("Permit")]
        public bool Permit { get; set; }

        //[JsonProperty("AvailableInventoryQty1")]
        public int AvailableInventoryQty1 { get; set; }

        //[JsonProperty("Description")]
        public string? Description { get; set; }

        //[JsonProperty("IsBatchProduct")]
        public bool IsBatchProduct { get; set; }
    }

}
