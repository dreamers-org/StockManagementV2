using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using StockManagement.Models;

namespace StockManagement
{
    public static class Statistiche
    {
        //TotalePerRappresentanteCompletatoConClienti
        public static List<TotalePerRappresentanteCompletato> getTotalePerRappresentanteCompletato(StockV2Context _context, IdentityContext _identityContext)
        {
            List<TotalePerRappresentanteCompletato> result = new List<TotalePerRappresentanteCompletato>();
            try
            {
                List<RappresentanteCliente> listaRappresentantiOrdiniClienti = new List<RappresentanteCliente>();

                //Prendo tutti i rappresentanti con un numero di ordini completati > 0
                //Creo una lista di oggetti idRappresentante-idOrdine
                var query = from ordineCompletato in _context.OrdineCliente
                            where ordineCompletato.Completato == true
                            select new RappresentanteCliente() { idRappresentante = ordineCompletato.IdRappresentante, idOrdine = ordineCompletato.Id, idCliente = ordineCompletato.IdCliente };
                listaRappresentantiOrdiniClienti = query.ToList();

                //Per ogni rappresentante, prendo la lista degli ordini dove completato == 1
                foreach (var item in listaRappresentantiOrdiniClienti)
                {
                    //Creo un elemento della lista di ritorno
                    TotalePerRappresentanteCompletato temp = new TotalePerRappresentanteCompletato();
                    string nomeAgenzia = _identityContext.Users.Where(x => x.Id == item.idRappresentante.ToString()).Select(x => x.AgenziaRappresentanza).FirstOrDefault();
                    string nomeCliente = _context.Cliente.Where(x => x.Id == item.idCliente).Select(x => x.Nome).FirstOrDefault();

                    temp.nomeAgenzia = nomeAgenzia;
                    temp.nomeCliente = nomeCliente;

                    //calcolo il totale dell'ordine: mi servono le righe
                    var listaRigheOrdineCliente = _context.RigaOrdineCliente.Where(x => x.IdOrdine == item.idOrdine).ToList();

                    //variabile che contiene il totale dell'ordine
                    double totale = 0.0;

                    foreach (var rigaordine in listaRigheOrdineCliente)
                    {
                        var prezzoArticoloRiga = _context.Articolo.Where(x => x.Id == rigaordine.IdArticolo).Select(x => x.PrezzoVendita).First();
                        totale += prezzoArticoloRiga * (rigaordine.Xxs + rigaordine.Xs + rigaordine.S + rigaordine.M + rigaordine.L + rigaordine.Xl + rigaordine.Xxl + rigaordine.Xxxl + rigaordine.Xxxxl + rigaordine.TagliaUnica);
                    }

                    temp.totaleEuro = Math.Round(totale, 2);
                    result.Add(temp);
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            result = result.OrderBy(x => x.nomeAgenzia).ThenBy(x => x.nomeCliente).ToList();
            return result;
        }

        //TotalePerRappresentanteCompletatoLoftConCLienti
        public static List<TotalePerRappresentanteCompletato> getTotalePerRappresentanteCompletatoLoft(StockV2Context _context, IdentityContext _identityContext)
        {
            List<TotalePerRappresentanteCompletato> result = new List<TotalePerRappresentanteCompletato>();

            try
            {
                //Prendo tutti i rappresentanti con un numero di ordini completati > 0
                List<RappresentanteCliente> listaRappresentantiOrdiniClienti = new List<RappresentanteCliente>();

                //Ricavo tutti gli ordiniCompletati
                List<OrdineCliente> listaOrdini = _context.OrdineCliente.Where(x => x.Completato == true).ToList();

                //Prendo tutti gli ordini Loft
                List<OrdineCliente> listaOrdiniLoft = new List<OrdineCliente>();

                foreach (var item in listaOrdini)
                {
                    //Ricavo l'idCollezione di Loft
                    var idCollezioneLoft = _context.Collezione.Where(x => x.Nome == "Loft").Select(x => x.Id).First();

                    //Ricavlo gli articoli Loft
                    var articoliLoft = _context.Articolo.Where(x => x.IdCollezione == idCollezioneLoft).Select(x => x.Id).ToList();

                    //Ricavo la tutte la prima riga di ogni ordine
                    var rigaOrdine = _context.RigaOrdineCliente.Any(x => x.IdOrdine == item.Id && articoliLoft.Contains(x.IdArticolo));

                    //RigaOrdine == true se la collezione è loft
                    if (rigaOrdine) listaOrdiniLoft.Add(item);
                }

                //Creo una lista di oggetti idRappresentante-idOrdine
                var query = from ordineCompletato in listaOrdiniLoft
                            select new RappresentanteCliente() { idRappresentante = ordineCompletato.IdRappresentante, idOrdine = ordineCompletato.Id, idCliente = ordineCompletato.IdCliente };
                listaRappresentantiOrdiniClienti = query.ToList();


                //Per ogni rappresentante, prendo la lista degli ordini dove completato == 1
                foreach (var item in listaRappresentantiOrdiniClienti)
                {
                    //Creo un elemento della lista di ritorno
                    TotalePerRappresentanteCompletato temp = new TotalePerRappresentanteCompletato();
                    string nomeAgenzia = _identityContext.Users.Where(x => x.Id == item.idRappresentante.ToString()).Select(x => x.AgenziaRappresentanza).FirstOrDefault();
                    string nomeCliente = _context.Cliente.Where(x => x.Id == item.idCliente).Select(x => x.Nome).FirstOrDefault();

                    temp.nomeAgenzia = nomeAgenzia;
                    temp.nomeCliente = nomeCliente;

                    //calcolo il totale dell'ordine: mi servono le righe
                    var listaRigheOrdineCliente = _context.RigaOrdineCliente.Where(x => x.IdOrdine == item.idOrdine).ToList();

                    //variabile che contiene il totale dell'ordine
                    double totale = 0.0;

                    foreach (var rigaordine in listaRigheOrdineCliente)
                    {
                        var prezzoArticoloRiga = _context.Articolo.Where(x => x.Id == rigaordine.IdArticolo).Select(x => x.PrezzoVendita).First();

                        totale += prezzoArticoloRiga * (rigaordine.Xxs + rigaordine.Xs + rigaordine.S + rigaordine.M + rigaordine.L + rigaordine.Xl + rigaordine.Xxl + rigaordine.Xxxl + rigaordine.Xxxxl + rigaordine.TagliaUnica);
                    }

                    temp.totaleEuro = Math.Round(totale, 2);
                    result.Add(temp);
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            result = result.OrderBy(x => x.nomeAgenzia).ThenBy(x => x.nomeCliente).ToList();
            return result;
        }

        //TotalePerRappresentanteCompletatoZeroMenoConCLienti
        public static List<TotalePerRappresentanteCompletato> getTotalePerRappresentanteCompletatoZeroMeno(StockV2Context _context, IdentityContext _identityContext)
        {
            List<TotalePerRappresentanteCompletato> result = new List<TotalePerRappresentanteCompletato>();

            try
            {
                //Prendo tutti i rappresentanti con un numero di ordini completati > 0
                List<RappresentanteCliente> listaRappresentantiOrdiniClienti = new List<RappresentanteCliente>();

                //Ricavo tutti gli ordiniCompletati
                List<OrdineCliente> listaOrdini = _context.OrdineCliente.Where(x => x.Completato == true).ToList();

                //Prendo tutti gli ordini Loft
                List<OrdineCliente> listaOrdiniZeroMeno = new List<OrdineCliente>();

                foreach (var item in listaOrdini)
                {
                    //Ricavo l'idCollezione di Loft
                    var idCollezioneZeroMeno = _context.Collezione.Where(x => x.Nome == "Zero Meno").Select(x => x.Id).First();

                    //Ricavlo gli articoli Loft
                    var articoliLoft = _context.Articolo.Where(x => x.IdCollezione == idCollezioneZeroMeno).Select(x => x.Id).ToList();

                    //Ricavo la tutte la prima riga di ogni ordine
                    var rigaOrdine = _context.RigaOrdineCliente.Any(x => x.IdOrdine == item.Id && articoliLoft.Contains(x.IdArticolo));

                    //RigaOrdine == true se la collezione è loft
                    if (rigaOrdine) listaOrdiniZeroMeno.Add(item);
                }

                //Creo una lista di oggetti idRappresentante-idOrdine
                var query = from ordineCompletato in listaOrdiniZeroMeno
                            select new RappresentanteCliente() { idRappresentante = ordineCompletato.IdRappresentante, idOrdine = ordineCompletato.Id, idCliente = ordineCompletato.IdCliente };
                listaRappresentantiOrdiniClienti = query.ToList();


                //Per ogni rappresentante, prendo la lista degli ordini dove completato == 1
                foreach (var item in listaRappresentantiOrdiniClienti)
                {
                    //Creo un elemento della lista di ritorno
                    TotalePerRappresentanteCompletato temp = new TotalePerRappresentanteCompletato();
                    string nomeAgenzia = _identityContext.Users.Where(x => x.Id == item.idRappresentante.ToString()).Select(x => x.AgenziaRappresentanza).FirstOrDefault();
                    string nomeCliente = _context.Cliente.Where(x => x.Id == item.idCliente).Select(x => x.Nome).FirstOrDefault();

                    temp.nomeAgenzia = nomeAgenzia;
                    temp.nomeCliente = nomeCliente;

                    //calcolo il totale dell'ordine: mi servono le righe
                    var listaRigheOrdineCliente = _context.RigaOrdineCliente.Where(x => x.IdOrdine == item.idOrdine).ToList();

                    //variabile che contiene il totale dell'ordine
                    double totale = 0.0;

                    foreach (var rigaordine in listaRigheOrdineCliente)
                    {
                        var prezzoArticoloRiga = _context.Articolo.Where(x => x.Id == rigaordine.IdArticolo).Select(x => x.PrezzoVendita).First();

                        totale += prezzoArticoloRiga * (rigaordine.Xxs + rigaordine.Xs + rigaordine.S + rigaordine.M + rigaordine.L + rigaordine.Xl + rigaordine.Xxl + rigaordine.Xxxl + rigaordine.Xxxxl + rigaordine.TagliaUnica);
                    }

                    temp.totaleEuro = Math.Round(totale, 2);
                    result.Add(temp);
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }

            result = result.OrderBy(x => x.nomeAgenzia).ThenBy(x => x.nomeCliente).ToList();
            return result;
        }

        public static List<RappresentanteTotaleEuro> GetRappresentanteTotaleEuro(List<TotalePerRappresentanteCompletato> totalone)
        {
            List<RappresentanteTotaleEuro> result = new List<RappresentanteTotaleEuro>();

            try
            {
                //Prendo i rappresentanti dal totalone
                List<string> listaRappresentanti = new List<string>();

                listaRappresentanti = totalone.Select(x => x.nomeAgenzia).Distinct().ToList();

                foreach (var item in listaRappresentanti)
                {
                    RappresentanteTotaleEuro temp = new RappresentanteTotaleEuro();
                    temp.nomeAgenzia = item;

                    double totale = 0.0;

                    foreach (var totaleCliente in totalone)
                    {
                        if (totaleCliente.nomeAgenzia == item)
                        {
                            totale += totaleCliente.totaleEuro;
                        }
                    }
                    temp.totale = Math.Round(totale, 2);
                    result.Add(temp);
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
            return result;
        }

        //TotalePerFornitoreTotale
        public static List<TotalePerFornitore> GetTotalePerFornitore(StockV2Context _context)
        {
            Dictionary<string, double> totaleFornitoreMap = new Dictionary<string, double>();
            List<TotalePerFornitore> result = new List<TotalePerFornitore>();
            try
            {
                //trovo tutti gli id ordini completati
                var idOrdiniCompletati = _context.OrdineCliente.Where(x => x.Completato == true).Select(x => x.Id).ToList();

                //ricavo tutte le righe degli ordini clienti dove idOrdine è compreso nel set qui sopra
                var righeOrdineClienteCompletati = _context.RigaOrdineCliente.Where(x => idOrdiniCompletati.Contains(x.IdOrdine)).ToList();

                //ciclo tutte le righe ordine, le associo ad un articolo e quindi ad un fornitore, e calcolo il totale
                foreach (var item in righeOrdineClienteCompletati)
                {
                    var fornitore = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x.IdFornitore).FirstOrDefault();
                    var nomeFornitore = _context.Fornitore.Where(x => x.Id == fornitore).Select(x => x.Nome).FirstOrDefault();

                    var prezzoArticolo = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x.PrezzoVendita).FirstOrDefault();

                    var totaleRiga = Math.Round(prezzoArticolo * (item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.Xxxxl + item.TagliaUnica), 2);

                    if (totaleFornitoreMap.Keys.Contains(nomeFornitore))
                    {
                        totaleFornitoreMap[nomeFornitore] = totaleFornitoreMap[nomeFornitore] + totaleRiga;
                    }
                    else
                    {
                        totaleFornitoreMap.Add(nomeFornitore, totaleRiga);
                    }
                }
                foreach (var key in totaleFornitoreMap.Keys)
                {
                    result.Add(new TotalePerFornitore() { Fornitore = key, Totale = totaleFornitoreMap[key] });
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }

            result = result.OrderBy(x => x.Fornitore).ToList();
            return result;
        }

        //TotalePerFornitoreByCollezione
        public static List<TotalePerFornitore> GetTotalePerFornitoreByCollezione(StockV2Context _context, string collezione)
        {
            Dictionary<string, double> totaleFornitoreMap = new Dictionary<string, double>();
            List<TotalePerFornitore> result = new List<TotalePerFornitore>();
            try
            {
                //trovo tutti gli id ordini completati
                var idOrdiniCompletati = _context.OrdineCliente.Where(x => x.Completato == true).Select(x => x.Id).ToList();

                //ricavo tutte le righe degli ordini clienti dove idOrdine è compreso nel set qui sopra
                var righeOrdineClienteCompletati = _context.RigaOrdineCliente.Where(x => idOrdiniCompletati.Contains(x.IdOrdine)).ToList();

                //ciclo tutte le righe ordine, le associo ad un articolo e quindi ad un fornitore, e calcolo il totale
                foreach (var item in righeOrdineClienteCompletati)
                {
                    //Prendo l'id della collezione Loft
                    var idCollezione = _context.Collezione.Where(x => x.Nome == collezione).Select(x => x.Id).FirstOrDefault();

                    //Lo confronto con quello dell'articolo.Se diversi, vado avanti
                    var idCollezioneArticolo = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x.IdCollezione).FirstOrDefault();

                    if (idCollezione == idCollezioneArticolo)
                    {
                        var fornitore = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x.IdFornitore).FirstOrDefault();
                        var nomeFornitore = _context.Fornitore.Where(x => x.Id == fornitore).Select(x => x.Nome).FirstOrDefault();

                        var prezzoArticolo = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x.PrezzoVendita).FirstOrDefault();

                        var totaleRiga = Math.Round(prezzoArticolo * (item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.Xxxxl + item.TagliaUnica), 2);

                        if (totaleFornitoreMap.Keys.Contains(nomeFornitore))
                        {
                            totaleFornitoreMap[nomeFornitore] = totaleFornitoreMap[nomeFornitore] + totaleRiga;
                        }
                        else
                        {
                            totaleFornitoreMap.Add(nomeFornitore, totaleRiga);
                        }
                    }
                }
                foreach (var key in totaleFornitoreMap.Keys)
                {
                    result.Add(new TotalePerFornitore() { Fornitore = key, Totale = totaleFornitoreMap[key] });
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }

            result = result.OrderBy(x => x.Fornitore).ToList();
            return result;
        }

        public class TotalePerRappresentanteCompletato
        {
            [DisplayName("Agenzia")]
            public string nomeAgenzia { get; set; }
            [DisplayName("Cliente")]
            public string nomeCliente { get; set; }
            [DisplayName("Euro Totali")]
            public double totaleEuro { get; set; }
        }

        public class RappresentanteCliente
        {
            public Guid idRappresentante { get; set; }
            public Guid idOrdine { get; set; }
            public Guid idCliente { get; set; }
        }

        public class RappresentanteTotaleEuro
        {
            [DisplayName("Agenzia")]
            public string nomeAgenzia { get; set; }
            [DisplayName("Euro Totali")]
            public double totale { get; set; }
        }

        public class TotalePerFornitore
        {
            public string Fornitore { get; set; }
            public double Totale { get; set; }
        }


        public class TotalePerFornitoreConArticoli
        {
            public string Fornitore { get; set; }
            public string Codice { get; set; }
            public string Colore { get; set; }
            public double Totale { get; set; }
        }
    }
}
