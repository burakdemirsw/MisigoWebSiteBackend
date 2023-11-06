using GoogleAPI.Domain.DTO;
using GoogleAPI.Domain.Entities;
using GoogleAPI.Domain.Models;
using GoogleAPI.Persistance.Contexts;
using GoogleAPI.Persistance.Repositories;
using GooleAPI.Application.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace GoogleAPI.API.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly GooleAPIDbContext _context;
        private readonly IProductWriteRepository _p;
        private readonly string ErrorTextBase = "İstek Sırasında Hata Oluştu: ";
        public ProductsController(
           GooleAPIDbContext context, IProductWriteRepository p
        )
        {
            _p = p;
            _context = context;
        }

        [HttpGet("GetProductVariation")]
        public async  Task<IActionResult>  GetVariationsByFilter( string StockCode)
        {
            try
            {
                var query = $"GetProductVariation '{StockCode}'";
                List<ProductVariation_VM> models =  _context.ProductVariation_VM.FromSqlRaw(query).AsEnumerable().ToList();

                return Ok(models);  
                
            }
            catch (Exception)
            {
                return BadRequest();
               
            }
        }
        [HttpGet("GetProductDetail/{brandName}")]
        public async Task<IActionResult> GetProductDetail( string brandName)
        {
            try
            {
                var query = $"GetProductDetail '{brandName}'";
                List<ProductDetail_DTO> models = _context.ProductDetail_DTO.FromSqlRaw(query).AsEnumerable().ToList();

                // Convert the ProductDetail_DTO list to ProductDetail_VM
                List<ProductDetail_VM> productDetailVMs = models.Select(dto => new ProductDetail_VM
                {
                    StockCode = dto.StockCode,
                    Description = dto.Description,
                    Color = dto.Color,
                    NormalPrice = dto.NormalPrice,
                    PurchasePrice = dto.PurchasePrice,
                    DiscountedPrice = dto.DiscountedPrice,
                    Brand = dto.Brand,
                    PhotoUrl = JsonConvert.DeserializeObject<List<Photo_VM>>(dto.PhotoUrl),
                Variations = JsonConvert.DeserializeObject<List<Variant_VM>>(dto.Variations)
                }).ToList();

                return Ok(productDetailVMs);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("GetProductCards")]
        public async Task<IActionResult> GetProductCards(string? stockCode)
        {
            try
            {
                var query = $"GetProductCard '{stockCode}'";
                List<ProductCard_VM> models = _context.ProductCard_VM.FromSqlRaw(query).AsEnumerable().ToList();

                return Ok(models);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                
            }
        }

        [HttpPost("GetProductCardsByBrandId")]
        public async Task<IActionResult> GetProductCardsByBrandId(string? brandId)
        {
            try
            {
                var query = $"GetProductCardByBrand '{brandId}'";
                List<ProductCard_VM> models = _context.ProductCard_VM.FromSqlRaw(query).AsEnumerable().ToList();

                return Ok(models);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }


        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductAdd_DTO productDto)
        {
            try
            {
                //aynı stok kodu - renk - beden daha önce sistemde varsa onu listeden sil!
                
                if (productDto == null)
                {
                    return BadRequest("Invalid data provided.");
                }
                List<Product> productList  = new List<Product>();

                foreach (var color in productDto.ColorIdList)
                {
                    foreach (var dimention in productDto.ItemDimList)
                    {
                        var product = new Product
                        {
                            StockCode = productDto.StockCode,
                            Description = productDto.Description,
                            DimensionId = dimention,
                            ColorId = color,
                            MainCategoryId = productDto.MainCategoryId,
                            BrandId = productDto.BrandId,
                            Explanation = productDto.Explanation,
                            CoverLetter = productDto.CoverLetter,
                            Barcode = "DENEME",
                            NormalPrice = productDto.NormalPrice,
                            PurchasePrice = productDto.PurchasePrice,
                            DiscountedPrice = productDto.DiscountedPrice,
                            StockAmount = productDto.StockAmount,
                            VATRate = productDto.VatRate,
                            IsActive = productDto.IsActive,
                            IsNew = productDto.IsNew,
                            IsFreeCargo = productDto.IsFreeCargo,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                        };
                        productList.Add(product);
                    }
                }


                foreach (var item2 in productList)
                {
                    

                  await  _p.AddAsync(item2);

                }

                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message+"\n"+e.InnerException);
            }
        }



    }
}