using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModuleBuilderSite.Data;
using ModuleBuilderSite.Models;

namespace ModuleBuilderSite.Controllers
{
    public class ModuleCreatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModuleCreatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        



        // GET: ModuleCreates
        public async Task<IActionResult> Index()
        {
            return View(await _context.ModuleCreate.ToListAsync());
        }

        // GET: ModuleCreates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moduleCreate = await _context.ModuleCreate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (moduleCreate == null)
            {
                return NotFound();
            }

            return View(moduleCreate);
        }

        // GET: ModuleCreates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ModuleCreates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModuleName")] ModuleCreate moduleCreate)
        {
            if (ModelState.IsValid)
            {
                moduleCreate.ModuleName = moduleCreate.ModuleName.Replace(" ", string.Empty);
                _context.Add(moduleCreate);
                await _context.SaveChangesAsync();
                
                var pathtomodule = ModuleBuilderStandard.ModuleBuilderStandardClass.Runner(moduleCreate.ModuleName);
                return RedirectToAction(nameof(Index));
            }
            return View(moduleCreate);
        }

        
        // GET: ModuleCreates/Edit/5
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }




            var moduleCreate = await _context.ModuleCreate.FindAsync(id);
            if (moduleCreate == null)
            {
                return NotFound();
            }
            string repoLocation = GetRepolocation();
           

            var doeszipexist = System.IO.File.Exists(repoLocation + "/" + moduleCreate.ModuleName + ".zip");

            if (doeszipexist ==false)
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(GetRepolocation()+"/"+ moduleCreate.ModuleName, repoLocation + "/" + moduleCreate.ModuleName + ".zip", System.IO.Compression.CompressionLevel.Fastest, false);

            }

            var filetodownload = System.IO.File.ReadAllBytes(repoLocation + "/" + moduleCreate.ModuleName + ".zip");
            var contentType = "APPLICATION/octet-stream";
            var fileName = moduleCreate.ModuleName + ".zip";

            return File(filetodownload, contentType, fileName);

           // return View(moduleCreate);
        }

        private string GetRepolocation()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
               return @"c:\DecisionsModuleBuilderTemp\";
            }
            else
            {
                return  Path.GetTempPath();
            }
            
        }

        // POST: ModuleCreates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModuleName")] ModuleCreate moduleCreate)
        {
            if (id != moduleCreate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(moduleCreate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleCreateExists(moduleCreate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(moduleCreate);
        }

        // GET: ModuleCreates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moduleCreate = await _context.ModuleCreate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (moduleCreate == null)
            {
                return NotFound();
            }

            return View(moduleCreate);
        }

        // POST: ModuleCreates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var moduleCreate = await _context.ModuleCreate.FindAsync(id);
            _context.ModuleCreate.Remove(moduleCreate);


            await _context.SaveChangesAsync();

            //var fmoduleCreate =  _context.ModuleCreate.Where(x => x.Id != 0).ToList();
            //foreach(var f in fmoduleCreate)
            //{
            //    _context.ModuleCreate.Remove(f);
            //}
          


            await _context.SaveChangesAsync();


            try
            {
                System.IO.Directory.Delete(GetRepolocation() + "/" + moduleCreate.ModuleName, true);
                System.IO.File.Delete(GetRepolocation() + "/" + moduleCreate.ModuleName + ".zip");

            }
            catch
            { }
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleCreateExists(int id)
        {
            return _context.ModuleCreate.Any(e => e.Id == id);
        }
    }
}
