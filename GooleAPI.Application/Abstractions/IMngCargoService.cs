using GoogleAPI.Domain.Entities;
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
        Task<CreatePackage_MNG_RR> CreateCargo(CreatePackage_MNG_RM Order); //CreateOrder
        Task<CreateBarcode_MNG_Response> CreateBarcode(string referenceId);//CreateBarcode
        Task<bool> PrintSingleBarcode(string zplCode);
        Task ConvertAndPrintBarcode(string Zpl);
        Task<GetPackageStatus_MNG_Response> GetPackageStatus(string ShipmentId);

        Task<List<CargoBarcode_VM>> GetShippedCargos( );
        Task<bool> DeleteShippedCargo(DeletePackage_MNG_Request Request);
        //void UpdateCargo( );
        //void GetCargoById( );
        //void GetCargoList( );
    }
}
