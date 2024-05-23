using crud.Data;
using crud.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace crud.Controllers
{
    public class WorkController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public WorkController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(prodinfo viewmodel, IFormFile ProdImage)
        {
            string imageUrl = null;

            if (ProdImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ProdImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProdImage.CopyToAsync(fileStream);
                }

                imageUrl = "/images/" + uniqueFileName;
            }

            var prod = new prodinfo
            {
                ProdName = viewmodel.ProdName,
                ProdPrice = viewmodel.ProdPrice,
                ProdImageUrl = imageUrl
            };

            await dbContext.AddAsync(prod);
            await dbContext.SaveChangesAsync();

            return View();
        }

        public IActionResult AllProducts()
        {
            var viewproducts = dbContext.Product.ToList();
            return View(viewproducts);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await dbContext.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, prodinfo viewModel, IFormFile ProdImage)
        {
          

            var product = await dbContext.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (ProdImage != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(product.ProdImageUrl))
                {
                    string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, product.ProdImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ProdImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProdImage.CopyToAsync(fileStream);
                }

                product.ProdImageUrl = "/images/" + uniqueFileName;
            }

            product.ProdName = viewModel.ProdName;
            product.ProdPrice = viewModel.ProdPrice;

            dbContext.Product.Update(product);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(AllProducts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await dbContext.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.ProdImageUrl))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, product.ProdImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            dbContext.Product.Remove(product);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(AllProducts));
        }
    }
}
