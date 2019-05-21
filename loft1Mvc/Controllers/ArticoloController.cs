using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StockManagement.Models;
using StockManagement.Models.ViewModels;

namespace StockManagement.Controllers
{
    public class ArticoloController : Controller
    {
        #region Costanti&Readonly

        private const string A = "Id,Codice,Descrizione,IdFornitore,Colore,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,TagliaUnica,TrancheConsegna,Genere,IdTipo,PrezzoAcquisto,PrezzoVendita,IdCollezione";
        private const string RuoloCommesso = "SuperAdmin, Commesso, Titolare";
        private readonly StockV2Context _context;
        private IConfiguration _configuration { get; }

        #endregion

        public ArticoloController(StockV2Context context, IConfiguration Configuration)
        {
            _context = context;
            _configuration = Configuration;
        }
       
        #region Index

        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> Index(string orderBy)
        {
            try
            {
                IOrderedQueryable<Articolo> articoli;
                switch (orderBy)
                {
                    case "Data":
                        HttpContext.Session.SetString("OrderBy", "Data");
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderByDescending(x => x.DataInserimento);
                        return View("Index", await articoli.ToListAsync());
                    case "Codice":
                        HttpContext.Session.SetString("OrderBy", "Codice");
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderBy(x => x.Codice);
                        return View("Index", await articoli.ToListAsync());
                    case "Fornitore":
                        HttpContext.Session.SetString("OrderBy", "Fornitore");
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderBy(x => x.IdFornitoreNavigation.Nome);
                        return View("Index", await articoli.ToListAsync());
                    default:
                        var orderByParam = HttpContext.Session.GetString("OrderBy");
                        if (!string.IsNullOrEmpty(orderByParam))
                        {
                            return RedirectToAction(nameof(Index), new { orderBy = orderByParam });
                        }
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderByDescending(x => x.DataInserimento);
                        return View("Index", await articoli.ToListAsync());
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> IndexAnnullati()
        {
            try
            {
                var stockV2Context = _context.Articolo.Where(x => x.Annullato == true).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation);
                return View(await stockV2Context.ToListAsync());
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Create

        [Authorize(Roles = RuoloCommesso)]
        public IActionResult Create()
        {
            ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome");
            ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome");
            ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome");
            return View();
        }

        [Authorize(Roles = RuoloCommesso)]
        public IActionResult CreateAnnullamento(Guid id)
        {
            try
            {
                var articolo = _context.Articolo.Where(x => x.Id == id && x.Annullato == false).FirstOrDefault();
                ArticoloAnnullatoViewModel articoloDaAnnullare = GetArticoloAnnullato(articolo);
                return View("CreateAnnullamento", articoloDaAnnullare);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(A)] Articolo articolo, IFormFile Foto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Utility.ArticoloExists(articolo.Codice, articolo.Colore, _context))
                    {
                        HttpContext.Session.SetString("ErrorMessage", "L'articolo esiste già.");
                        return RedirectToAction("Create");
                    }

                    articolo.Id = Guid.NewGuid();
                    articolo.UtenteInserimento = User.Identity.Name;
                    articolo.DataModifica = DateTime.Now;
                    articolo.Video = "";

                    _context.Add(articolo);
                    await _context.SaveChangesAsync();

                    await InserisciFotoArticolo(articolo, Foto);

                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome", articolo.IdCollezione);
                ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome", articolo.IdFornitore);
                ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome", articolo.IdTipo);
                return View();
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnnullamento(ArticoloAnnullatoViewModel articoloAnnullato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!Utility.ArticoloExists(articoloAnnullato.Codice, articoloAnnullato.Colore, _context))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var articolo = _context.Articolo.Where(x => x.Codice == articoloAnnullato.Codice && x.Colore == articoloAnnullato.Colore && x.Annullato == false).FirstOrDefault();

                        AnnullaTaglie(articoloAnnullato, articolo);

                        _context.Update(articolo);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Update
        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var articolo = await _context.Articolo.FindAsync(id);

                if (articolo == null) return NotFound();

                ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome", articolo.IdCollezione);
                ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome", articolo.IdFornitore);
                ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome", articolo.IdTipo);

                var fotoArticolo = _context.ArticoloFoto.Where(x => x.IdArticolo == id).Select(x => x.Foto).FirstOrDefault();
                ViewData["Foto"] = fotoArticolo;

                return View(articolo);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind(A)] Articolo articolo, IFormFile Foto)
        {
            try
            {
                if (id != articolo.Id) return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        await ModificaFoto(articolo, Foto);

                        _context.Update(articolo);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!Utility.ArticoloExists(articolo.Id, _context)) return NotFound();

                        Utility.GestioneErrori(User.Identity.Name, ex);
                        throw;
                    }
                    return RedirectToAction(nameof(Index));
                }

                ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome", articolo.IdCollezione);
                ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome", articolo.IdFornitore);
                ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome", articolo.IdTipo);

                return View(articolo);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> Annulla(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var articolo = await _context.Articolo.FindAsync(id);
                if (articolo == null) return NotFound();
               
                articolo.Annullato = true;

                _context.Update(articolo);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Delete

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var articolo = await _context.Articolo.FirstOrDefaultAsync(m => m.Id == id);

                if (articolo == null) return NotFound();

                return View(articolo);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var articolo = await _context.Articolo.FindAsync(id);
                _context.Articolo.Remove(articolo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Altri

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var articolo = await _context.Articolo
                    .Include(a => a.IdCollezioneNavigation)
                    .Include(a => a.IdFornitoreNavigation)
                    .Include(a => a.IdTipoNavigation)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (articolo == null) return NotFound();

                return View(articolo);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        public async Task<IActionResult> Ordina(string Parametro)
        {
            try
            {
                IOrderedQueryable<Articolo> articoli;
                switch (Parametro)
                {
                    case "Data":
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderByDescending(x => x.DataInserimento);
                        return View("Index", await articoli.ToListAsync());
                    case "Codice":
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderBy(x => x.Codice);
                        return View("Index", await articoli.ToListAsync());
                    case "Fornitore":
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderBy(x => x.IdFornitoreNavigation.Nome);
                        return View("Index", await articoli.ToListAsync());
                    default:
                        articoli = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderByDescending(x => x.DataInserimento);
                        return View("Index", await articoli.ToListAsync());
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        public async Task<IActionResult> Listino()
        {
            try
            {
                var stockV2Context = _context.ViewArticoloOrdineFornitore.OrderBy(x => x.Codice);
                return View(await stockV2Context.ToListAsync());
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> DifferenzaOrdinatoVenduto(string orderBy)
        {
            try
            {
                List<ViewDifferenzaOrdinatoVendutoViewModel> articoli;

                switch (orderBy)
                {
                    case "Codice":
                        HttpContext.Session.SetString("OrderBy", "Codice");
                        articoli = await _context.ViewDifferenzaOrdinatoVenduto.OrderBy(x => x.Codice).ToListAsync();
                        break;
                    case "Fornitore":
                        HttpContext.Session.SetString("OrderBy", "Fornitore");
                        articoli = await _context.ViewDifferenzaOrdinatoVenduto.OrderBy(x => x.Fornitore).ToListAsync();
                        break;
                    default:
                        articoli = await _context.ViewDifferenzaOrdinatoVenduto.ToListAsync();
                        break;
                }

                return View(articoli);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> Export()
        {
            try
            {
                List<ViewDifferenzaOrdinatoVendutoViewModel> viewDifferenzaOrdinatoVenduto = _context.ViewDifferenzaOrdinatoVenduto.OrderBy(x => x.Codice).ToList();
                string sWebRootFolder = _configuration.GetValue<string>("ExcelFolder");
                string sFileName = @"Loft.xlsx";

                MemoryStream memory = await Utility.GetFileContent(viewDifferenzaOrdinatoVenduto, typeof(ViewDifferenzaOrdinatoVendutoViewModel), sWebRootFolder, sFileName);
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> ExportOrdinatiPerClienti()
        {
            try
            {
                List<ViewArticoliOrdinatiDaiClientiPerDataViewModel> ViewArticoliOrdinatiDaiClientiPerData = _context.ViewArticoliOrdinatiDaiClientiPerData.OrderBy(x => x.Codice).ToList();
                string sWebRootFolder = _configuration.GetValue<string>("ExcelFolder");
                string sFileName = @"Loft.xlsx";

                MemoryStream memory = await Utility.GetFileContent(ViewArticoliOrdinatiDaiClientiPerData, typeof(ViewArticoliOrdinatiDaiClientiPerDataViewModel), sWebRootFolder, sFileName);
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        [Authorize(Roles = RuoloCommesso)]
        public async Task<IActionResult> ExportZeroMenoCompletato()
        {
            try
            {
                List<ViewDifferenzaOrdinatoVendutoZeroMenoCompletatoViewModel> ViewArticoliOrdinatiDaiClientiPerData = _context.ViewDifferenzaOrdinatoVendutoZeroMenoCompletato.OrderBy(x => x.Codice).ToList();
                string sWebRootFolder = _configuration.GetValue<string>("ExcelFolder");
                string sFileName = @"Loft.xlsx";

                MemoryStream memory = await Utility.GetFileContent(ViewArticoliOrdinatiDaiClientiPerData, typeof(ViewDifferenzaOrdinatoVendutoZeroMenoCompletatoViewModel), sWebRootFolder, sFileName);
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region MetodiLatoCliente

        public ActionResult getFotoArticolo(string Codice, string Colore)
        {
            try
            {
                FotoArticolo result = new FotoArticolo{ isArticoloValido = false };

                if (Utility.ArticoloExists(Codice, Colore, _context))
                {
                    Articolo articolo = _context.Articolo.Where(x => x.Codice == Codice && x.Annullato == false && x.Colore == Colore).FirstOrDefault();

                    if (articolo != null)
                    {
                        result.isArticoloValido = true;

                        //Prendo la foto dell'articolo dall'altra tabella
                        var fotoArticolo = _context.ArticoloFoto.Where(x => x.IdArticolo == articolo.Id).Select(x => x.Foto).FirstOrDefault();

                        result.Foto = (fotoArticolo != null && fotoArticolo.Length > 0) ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(fotoArticolo)) : null;
                    };
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public ActionResult getTaglieDisponibili(string Codice, string Colore)
        {
            try
            {
                TaglieNonAttiveArticolo result = new TaglieNonAttiveArticolo();
                result.isArticoloValido = false;
                if (Utility.ArticoloExists(Codice, Colore,_context))
                {
                    Articolo articolo = _context.Articolo.Where(x => x.Codice == Codice && x.Annullato == false && x.Colore == Colore).FirstOrDefault();
                    if (articolo != null)
                    {
                        result.isArticoloValido = true;
                        result.Xxs = !(articolo.Xxs && articolo.isXxsActive);
                        result.Xs = !(articolo.Xs && articolo.isXsActive);
                        result.S = !(articolo.S && articolo.isSActive);
                        result.M = !(articolo.M && articolo.isMActive);
                        result.L = !(articolo.L && articolo.isLActive);
                        result.Xl = !(articolo.Xl && articolo.isXlActive);
                        result.Xxl = !(articolo.Xxl && articolo.isXxlActive);
                        result.Xxxl = !(articolo.Xxxl && articolo.isXxxlActive);
                        result.Xxxxl = !(articolo.Xxxxl && articolo.isXxxxlActive);
                        result.TagliaUnica = !(articolo.TagliaUnica && articolo.isTagliaUnicaActive);
                    };
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public async Task<bool> verifyCorrectness(string Codice, string Colore)
        {
            try
            {
                var articolo = await _context.Articolo.FirstOrDefaultAsync(m => m.Codice == Codice && m.Annullato == false && m.Colore == Colore);

                if (articolo == null) return true;

                return false;
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public IActionResult SelectColoriFromCodice(string codice)
        {
            try
            {
                Utility.CheckNull(codice);

                var listaColori = _context.Articolo.Where(x => x.Codice == codice)
                                             .Select(x => new
                                             {
                                                 Colore = x.Colore
                                             }).ToList();

                return Json(listaColori);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public IActionResult getTxtValuesOrdineFornitore(string codice)
        {
            try
            {
                Utility.CheckNull(codice);

                var listaColori = _context.Articolo.Where(x => x.Codice == codice)
                                             .Select(x => new
                                             {
                                                 Colore = x.Colore,
                                                 Descrizione = x.Descrizione,
                                                 Fornitore = x.IdFornitoreNavigation.Nome
                                             }).ToList();

                return Json(listaColori);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public IActionResult SelectDescrizioneFromCodice(string codice)
        {
            try
            {
                Utility.CheckNull(codice);

                string descrizione = _context.Articolo.Where(x => x.Codice == codice).Select(x => x.Descrizione).FirstOrDefault();
                return Json(descrizione);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public async Task<IActionResult> getTxtValues(string Codice)
        {
            try
            {
                Utility.CheckNull(Codice);

                TempObject result = new TempObject();

                var articolo = await _context.Articolo.FirstOrDefaultAsync(m => m.Codice == Codice && m.Annullato == false);

                if (articolo != null)
                {
                    Fornitore fornitore = await _context.Fornitore.FindAsync(articolo.IdFornitore);
                    Tipo tipo = await _context.Tipo.FindAsync(articolo.IdTipo);
                    Collezione collezione = await _context.Collezione.FindAsync(articolo.IdCollezione);
                    result = GetTempObject(articolo, fornitore, tipo, collezione);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }
        
        #endregion

        #region MetodiPrivati

        private async Task InserisciFotoArticolo(Articolo articolo, IFormFile Foto)
        {
            try
            {
                if (Foto == null)
                {
                    var fotoArticolo = new ArticoloFoto()
                    {
                        Id = Guid.NewGuid(),
                        Foto = null,
                        IdArticolo = articolo.Id
                    };
                    _context.Add(fotoArticolo);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    byte[] p1 = null;
                    using (var fs1 = Foto.OpenReadStream())
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    var fotoArticolo = new ArticoloFoto()
                    {
                        Id = Guid.NewGuid(),
                        Foto = p1,
                        IdArticolo = articolo.Id
                    };
                    _context.Add(fotoArticolo);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private void AnnullaTaglie(ArticoloAnnullatoViewModel articoloAnnullato, Articolo articolo)
        {
            try
            {
                if (articoloAnnullato.isAllToBeNullified)
                {
                    articolo.isXxsActive = false;
                    articolo.isXsActive = false;
                    articolo.isSActive = false;
                    articolo.isMActive = false;
                    articolo.isLActive = false;
                    articolo.isXlActive = false;
                    articolo.isXxlActive = false;
                    articolo.isXxxlActive = false;
                    articolo.isXxxxlActive = false;
                    articolo.isTagliaUnicaActive = false;
                    articolo.Annullato = true;
                }
                else
                {
                    articolo.isXxsActive = !articoloAnnullato.isXxsToBeNullified;
                    articolo.isXsActive = !articoloAnnullato.isXsToBeNullified;
                    articolo.isSActive = !articoloAnnullato.isSToBeNullified;
                    articolo.isMActive = !articoloAnnullato.isMToBeNullified;
                    articolo.isLActive = !articoloAnnullato.isLToBeNullified;
                    articolo.isXlActive = !articoloAnnullato.isXlToBeNullified;
                    articolo.isXxlActive = !articoloAnnullato.isXxlToBeNullified;
                    articolo.isXxxlActive = !articoloAnnullato.isXxxlToBeNullified;
                    articolo.isXxxxlActive = !articoloAnnullato.isXxxxlToBeNullified;
                    articolo.isTagliaUnicaActive = !articoloAnnullato.isTagliaUnicaToBeNullified;
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private ArticoloAnnullatoViewModel GetArticoloAnnullato(Articolo articolo)
        {
            try
            {
                return new ArticoloAnnullatoViewModel()
                {
                    Codice = articolo.Codice,
                    Colore = articolo.Colore,
                    isXxsToBeNullified = !articolo.isXxsActive,
                    isXsToBeNullified = !articolo.isXsActive,
                    isSToBeNullified = !articolo.isSActive,
                    isMToBeNullified = !articolo.isMActive,
                    isLToBeNullified = !articolo.isLActive,
                    isXlToBeNullified = !articolo.isXlActive,
                    isXxlToBeNullified = !articolo.isXxlActive,
                    isXxxlToBeNullified = !articolo.isXxxlActive,
                    isXxxxlToBeNullified = !articolo.isXxxxlActive,
                    isTagliaUnicaToBeNullified = !articolo.isTagliaUnicaActive,
                    isAllToBeNullified = articolo.Annullato
                };
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private async Task ModificaFoto(Articolo articolo, IFormFile Foto)
        {
            try
            {
                Articolo old = _context.Articolo.AsNoTracking().Where(x => x.Id == articolo.Id).FirstOrDefault();
                articolo.DataInserimento = old.DataInserimento;
                articolo.DataModifica = DateTime.Now;
                articolo.UtenteInserimento = old.UtenteInserimento;
                articolo.UtenteModifica = User.Identity.Name;
                articolo.Annullato = old.Annullato;
                articolo.Video = "";
                articolo.isXxsActive = old.isXxsActive;
                articolo.isXsActive = old.isXsActive;
                articolo.isSActive = old.isSActive;
                articolo.isMActive = old.isMActive;
                articolo.isLActive = old.isLActive;
                articolo.isXlActive = old.isXlActive;
                articolo.isXxlActive = old.isXxlActive;
                articolo.isXxxlActive = old.isXxxlActive;
                articolo.isTagliaUnicaActive = old.isTagliaUnicaActive;

                if (Foto != null)
                {
                    byte[] p1 = null;
                    using (var fs1 = Foto.OpenReadStream())
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    var fotoArticolo = _context.ArticoloFoto.Where(x => x.IdArticolo == articolo.Id).Select(x => x).FirstOrDefault();
                    fotoArticolo.Foto = p1;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private TempObject GetTempObject(Articolo articolo, Fornitore fornitore, Tipo tipo, Collezione collezione)
        {
            try
            {
                return new TempObject
                {
                    Fornitore = fornitore.Nome,
                    Descrizione = articolo.Descrizione,
                    PrezzoAcquisto = articolo.PrezzoAcquisto.ToString(),
                    PrezzoVendita = articolo.PrezzoVendita.ToString(),
                    TrancheConsegna = articolo.TrancheConsegna.ToString("yyyy-MM-dd"),
                    GenereProdotto = articolo.Genere,
                    TipoProdotto = tipo.Nome,
                    Collezione = collezione.Nome,
                    IdFornitore = articolo.IdFornitore.ToString(),
                    IdCollezione = articolo.IdCollezione.ToString(),
                    IdTipoProdotto = articolo.IdTipo.ToString(),
                    XXS = articolo.Xxs,
                    XS = articolo.Xs,
                    S = articolo.S,
                    M = articolo.M,
                    L = articolo.L,
                    XL = articolo.Xl,
                    XXL = articolo.Xxl,
                    XXXL = articolo.Xxxl,
                    XXXXL = articolo.Xxxxl,
                    TagliaUnica = articolo.TagliaUnica
                };
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        #endregion

        #region ClassiAppoggio
        protected class TempObject
        {
            public string Fornitore { get; set; }
            public string IdFornitore { get; set; }
            public string Descrizione { get; set; }
            public string PrezzoAcquisto { get; set; }
            public string PrezzoVendita { get; set; }
            public string TrancheConsegna { get; set; }
            public string GenereProdotto { get; set; }
            public string TipoProdotto { get; set; }
            public string IdTipoProdotto { get; set; }
            public string Collezione { get; set; }
            public string IdCollezione { get; set; }
            public bool XXS { get; set; }
            public bool XS { get; set; }
            public bool S { get; set; }
            public bool M { get; set; }
            public bool L { get; set; }
            public bool XL { get; set; }
            public bool XXL { get; set; }
            public bool XXXL { get; set; }
            public bool XXXXL { get; set; }
            public bool TagliaUnica { get; set; }
        }

        protected class TaglieNonAttiveArticolo
        {
            public bool isArticoloValido { get; set; }
            public bool Xxs { get; set; }
            public bool Xs { get; set; }
            public bool S { get; set; }
            public bool M { get; set; }
            public bool L { get; set; }
            public bool Xl { get; set; }
            public bool Xxl { get; set; }
            public bool Xxxl { get; set; }
            public bool Xxxxl { get; set; }
            public bool TagliaUnica { get; set; }
        }

        protected class FotoArticolo
        {
            public bool isArticoloValido { get; set; }
            public string Foto { get; set; }
        }
        #endregion
    }
}
