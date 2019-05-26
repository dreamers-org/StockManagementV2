using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Models;
using System;
using System.Collections.Generic;

namespace StockManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Titolare, Commesso")]
    public class StatisticheController : Controller
    {
        private readonly StockV2Context _context;
        private readonly IdentityContext _identityContext;

        public StatisticheController(StockV2Context Context, IdentityContext IdentityContext)
        {
            _context = Context;
            _identityContext = IdentityContext;
        }

        public ActionResult Index()
        {
            List<Statistiche.TotalePerRappresentanteCompletato> result = new List<Statistiche.TotalePerRappresentanteCompletato>();
            try
            {
                result = Statistiche.getTotalePerRappresentanteCompletato(_context, _identityContext);
                if (result != null)
                {
                    ViewData["NumeroOrdini"] = result.Count;
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            return View(result);
        }

        public ActionResult TotaleRappresentantiLoft()
        {
            List<Statistiche.TotalePerRappresentanteCompletato> result = new List<Statistiche.TotalePerRappresentanteCompletato>();
            try
            {
                result = Statistiche.getTotalePerRappresentanteCompletatoLoft(_context, _identityContext);
                
                if (result != null)
                {
                    ViewData["NumeroOrdini"] = result.Count;
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            ViewData["Collezione"] = "Loft";
            return View("Index", result);
        }

        public ActionResult TotaleRappresentantiZeroMeno()
        {
            List<Statistiche.TotalePerRappresentanteCompletato> result = new List<Statistiche.TotalePerRappresentanteCompletato>();
            try
            {
                result = Statistiche.getTotalePerRappresentanteCompletatoZeroMeno(_context, _identityContext);

                if (result != null)
                {
                    ViewData["NumeroOrdini"] = result.Count;
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            ViewData["Collezione"] = "Zero meno";
            return View("Index", result);
        }

        public ActionResult TotaleRappresentanteSenzaCliente()
        {
            List<Statistiche.RappresentanteTotaleEuro> result = new List<Statistiche.RappresentanteTotaleEuro>();
            try
            {
                var temp = Statistiche.getTotalePerRappresentanteCompletato(_context, _identityContext);
                result = Statistiche.GetRappresentanteTotaleEuro(temp);
                double incassoTotale = 0.0;

                foreach (var item in result)
                {
                    incassoTotale += item.totale;
                }
                ViewData["IncassoTotale"] = incassoTotale;
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            return View("TotaleRappresentanteSenzaCliente", result);
        }

        public ActionResult TotaleRappresentanteSenzaClienteLoft()
        {
            List<Statistiche.RappresentanteTotaleEuro> result = new List<Statistiche.RappresentanteTotaleEuro>();
            try
            {
                var temp = Statistiche.getTotalePerRappresentanteCompletatoLoft(_context, _identityContext);
                result = Statistiche.GetRappresentanteTotaleEuro(temp);
                double incassoTotale = 0.0;

                foreach (var item in result)
                {
                    incassoTotale += item.totale;
                }

                ViewData["IncassoTotale"] = incassoTotale;
                ViewData["Collezione"] = "Loft";
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            return View("TotaleRappresentanteSenzaCliente", result);
        }

        public ActionResult TotaleRappresentanteSenzaClienteZeroMeno()
        {
            List<Statistiche.RappresentanteTotaleEuro> result = new List<Statistiche.RappresentanteTotaleEuro>();
            try
            {
                var temp = Statistiche.getTotalePerRappresentanteCompletatoZeroMeno(_context, _identityContext);
                result = Statistiche.GetRappresentanteTotaleEuro(temp);
                double incassoTotale = 0.0;

                foreach (var item in result)
                {
                    incassoTotale += item.totale;
                }

                ViewData["IncassoTotale"] = incassoTotale;
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            ViewData["Collezione"] = "Zero meno";
            return View("TotaleRappresentanteSenzaCliente", result);
        }

        public ActionResult TotaleFornitore()
        {
            List<Statistiche.TotalePerFornitore> result = new List<Statistiche.TotalePerFornitore>();
            double totaleEuroComplessivo = 0.0;

            try
            {
                result = Statistiche.GetTotalePerFornitore(_context);

                foreach (var item in result)
                {
                    totaleEuroComplessivo += item.Totale;
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            ViewData["TotaleEuroComplessivo"] = totaleEuroComplessivo;
            return View("TotaleFornitore", result);
        }
        public ActionResult TotaleFornitoreLoft()
        {
            List<Statistiche.TotalePerFornitore> result = new List<Statistiche.TotalePerFornitore>();
            double totaleEuroComplessivo = 0.0;
            try
            {
                result = Statistiche.GetTotalePerFornitoreByCollezione(_context, "Loft");

                foreach (var item in result)
                {
                    totaleEuroComplessivo += item.Totale;
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            ViewData["TotaleEuroComplessivo"] = totaleEuroComplessivo;
            ViewData["Collezione"] = "Loft";
            return View("TotaleFornitore", result);
        }

        public ActionResult TotaleFornitoreZeroMeno()
        {
            List<Statistiche.TotalePerFornitore> result = new List<Statistiche.TotalePerFornitore>();
            double totaleEuroComplessivo = 0.0;
            try
            {
                result = Statistiche.GetTotalePerFornitoreByCollezione(_context, "Zero Meno");

                foreach (var item in result)
                {
                    totaleEuroComplessivo += item.Totale;
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            ViewData["TotaleEuroComplessivo"] = totaleEuroComplessivo;
            ViewData["Collezione"] = "Zero Meno";
            return View("TotaleFornitore", result);
        }
    }
}