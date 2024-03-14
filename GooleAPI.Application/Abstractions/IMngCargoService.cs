using GoogleAPI.Domain.Models.Cargo.Mng.Request;
using GoogleAPI.Domain.Models.Cargo.Mng.Response;
using GoogleAPI.Domain.Models.Cargo.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.Abstractions
{
    public interface IMNGCargoService
    {
        Task<CreateTokenResponse_MNG> CreateToken( );
        Task<CreatePackage_MNG_RR> CreateCargo(CreatePackage_MNG_Request Order); //CreateOrder
        Task<CreateBarcode_MNG_Response> CreateBarcode(CreateBarcode_MNG_Request Request);//CreateBarcode
        Task<bool> PrintBarcode2(string zplCode);
        Task ConvertAndPrintBarcode(string zpl);
        Task<bool> DeleteCargo( );
        //void UpdateCargo( );
        //void GetCargoById( );
        //void GetCargoList( );
    }
}
