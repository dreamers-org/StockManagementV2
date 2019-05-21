using loft1Mvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using StockManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StockManagement
{
    public static class Utility
    {
        internal static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            try
            {
                //adding custom roles
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<GenericUser>>();
                string[] roleNames = { "Rappresentante", "Commesso", "Titolare", "SuperAdmin" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    //creating the roles and seeding them to the database
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                var _user = await UserManager.FindByEmailAsync("luca@admin.it");
                if (_user != null)
                {
                    await UserManager.AddToRoleAsync(_user, "SuperAdmin");
                }
            }
            catch (Exception ex)
            {
                GestioneErrori(ex);
                throw;
            }
        }
        internal static async Task<MemoryStream> GetFileContent(dynamic list, Type tipoLista, string sWebRootFolder, string sFileName)
        {
            try
            {
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Differenza ordinato venduto");
                    IRow row = excelSheet.CreateRow(0);

                    var columns = tipoLista.GetProperties()
                            .Select(property => property.Name)
                            .ToList();

                    for (int i = 0; i < columns.Count - 1; i++)
                    {
                        row.CreateCell(i).SetCellValue(columns[i]);
                    }

                    foreach (var articolo in list)
                    {
                        row = excelSheet.CreateRow(list.IndexOf(articolo) + 1);
                        var arrayOfProperty = new List<PropertyInfo>(articolo.GetType().GetProperties());

                        foreach (PropertyInfo prop in arrayOfProperty)
                        {
                            if (arrayOfProperty.IndexOf(prop) != arrayOfProperty.Count - 1)
                            {
                                row.CreateCell(arrayOfProperty.IndexOf(prop)).SetCellValue(prop.GetValue(articolo).ToString());
                            }
                        }
                    }
                    workbook.Write(fs);
                }
                using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return memory;
            }
            catch (Exception ex)
            {
                GestioneErrori(ex);
                throw;
            }
        }
        internal static async Task<MemoryStream> GetFilesCombinedContent(dynamic list, Type tipoLista, dynamic list2, Type tipoLista2, string sWebRootFolder, string sFileName)
        {
            try
            {
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Report");
                    IRow row = excelSheet.CreateRow(0);

                    var columns = tipoLista.GetProperties()
                            .Select(property => property.Name)
                            .ToList();

                    for (int i = 0; i < columns.Count - 1; i++)
                    {
                        row.CreateCell(i).SetCellValue(columns[i]);
                    }

                    foreach (var item in list)
                    {
                        row = excelSheet.CreateRow(list.IndexOf(item) + 1);
                        var arrayOfProperty = new List<PropertyInfo>(item.GetType().GetProperties());

                        foreach (PropertyInfo prop in arrayOfProperty)
                        {
                            if (arrayOfProperty.IndexOf(prop) != arrayOfProperty.Count - 1)
                            {
                                row.CreateCell(arrayOfProperty.IndexOf(prop)).SetCellValue(prop.GetValue(item).ToString());
                            }
                        }
                    }

                    row = excelSheet.CreateRow(list.Count + 2);

                    var columns2 = tipoLista2.GetProperties()
                            .Select(property => property.Name)
                            .ToList();

                    for (int i = 0; i < columns2.Count - 1; i++)
                    {
                        row.CreateCell(i).SetCellValue(columns[i]);
                    }

                    int rowIndex = list.Count + 3;
                    foreach (var item in list2)
                    {
                        row = excelSheet.CreateRow(rowIndex);
                        var arrayOfProperty = new List<PropertyInfo>(item.GetType().GetProperties());

                        foreach (PropertyInfo prop in arrayOfProperty)
                        {
                            if (arrayOfProperty.IndexOf(prop) != arrayOfProperty.Count - 1)
                            {
                                row.CreateCell(arrayOfProperty.IndexOf(prop)).SetCellValue(prop.GetValue(item).ToString());
                            }
                        }
                        rowIndex++;
                    }


                    workbook.Write(fs);
                }
                using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return memory;
            }
            catch (Exception ex)
            {
                GestioneErrori(ex);
                throw;
            }
        }
        internal static bool ArticoloExists(Guid id, StockV2Context context) => context.Articolo.Any(e => e.Id == id);
        internal static bool ArticoloFotoExists(Guid id, StockV2Context context) => context.ArticoloFoto.Any(e => e.Id == id);
        internal static bool ArticoloExists(string codice, string colore, StockV2Context context) => context.Articolo.Any(e => e.Codice == codice && e.Colore == colore);
        internal static void CheckNull(dynamic Object)
        {
            if (Object == null) throw new ArgumentNullException(nameof(Object));
            //if (string.IsNullOrEmpty(Object)) throw new ArgumentNullException(nameof(Object));
        }
        internal static bool ClienteExists(string Email, string Nome, StockV2Context context) => context.Cliente.Any(e => e.Email == Email && e.Nome == Nome);
        internal static void GestioneErrori(string user, Exception ex) => Log.Error($"Utente: {user}. {ex.ToString()}");
        internal static void GestioneErrori(Exception ex) => Log.Error(ex.ToString());
        internal static string GenerateNumber()
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 0; i < 6; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }
        internal static string GetRandomNumber(StockV2Context context)
        {
            try
            {
                List<string> RandomNumbersList = context.OrdineCliente.Select(x => x.RandomNumber).ToList();
                string newRandomNumber = string.Empty;
                bool isRandomNumberValid = false;

                while (!isRandomNumberValid)
                {
                    newRandomNumber = Utility.GenerateNumber();
                    isRandomNumberValid = string.IsNullOrEmpty(RandomNumbersList.Where(x =>
                    {
                        return x == newRandomNumber;
                    }).FirstOrDefault());
                }

                return newRandomNumber;
            }
            catch (Exception ex)
            {
                GestioneErrori(ex);
                throw;
            }
        }
        internal static async Task Execute(IConfiguration configuration, StockV2Context _context, OrdineCliente ordineCliente, string emailCliente, string emailRappresentante, string collezione, string regione)
        {
            try
            {
                CheckNull(ordineCliente);
                CheckNull(emailCliente);
                CheckNull(emailRappresentante);

                var key = configuration.GetValue<string>("SendGridApiKey");

                if (string.IsNullOrEmpty(key))
                {
                    key = Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.User);
                }

                CheckNull(key);

                var client = new SendGridClient(key);
                var from = new EmailAddress();
                if (collezione == "Loft") from = new EmailAddress("info@loft1.it", "Loft1");
                else from = new EmailAddress("zero-meno@outlook.it", "Zero Meno");

                var tos = new List<EmailAddress>
            {
                from,
                new EmailAddress(emailCliente, emailCliente),
                new EmailAddress(emailRappresentante,emailRappresentante)
            };

                var cliente = _context.Cliente.Where(x => x.Id == ordineCliente.IdCliente).FirstOrDefault();
                var pagamento = _context.TipoPagamento.Where(x => x.Id == ordineCliente.IdTipoPagamento).Select(x => x.Nome).FirstOrDefault();

                var subject = $"Riepilogo ordine {collezione}: {cliente.Nome}";
                var plainTextContent = $"";

                var html = @"<!DOCTYPE html>
<html>
<head>

  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Ordine ricevuto</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
  /**
   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
   */
  @media screen {
    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 400;
      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
    }

    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 700;
      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
    }
  }

  /**
   * Avoid browser level font resizing.
   * 1. Windows Mobile
   * 2. iOS / OSX
   */
  body,
  table,
  td,
  a {
    -ms-text-size-adjust: 100%; /* 1 */
    -webkit-text-size-adjust: 100%; /* 2 */
  }

  /**
   * Remove extra space added to tables and cells in Outlook.
   */
  table,
  td {
    mso-table-rspace: 0pt;
    mso-table-lspace: 0pt;
  }

  /**
   * Better fluid images in Internet Explorer.
   */
  img {
    -ms-interpolation-mode: bicubic;
  }

  /**
   * Remove blue links for iOS devices.
   */
  a[x-apple-data-detectors] {
    font-family: inherit !important;
    font-size: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
    color: inherit !important;
    text-decoration: none !important;
  }

  /**
   * Fix centering issues in Android 4.4.
   */
  div[style*=""margin: 16px 0;""] {
    margin: 0 !important;
  }

  body {
    width: 100% !important;
    height: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }

  /**
   * Collapse table borders to avoid space between cells.
   */
  table {
    border-collapse: collapse !important;
  }

  a {
    color: #1a82e2;
  }

  img {
    height: auto;
    line-height: 100%;
    text-decoration: none;
    border: 0;
    outline: none;
  }
  </style>

</head>
<body style=""background-color: #D2C7BA;"">

  <!-- start preheader -->
  <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
    Conferma dell'ordine.
  </div>
  <!-- end preheader -->

  <!-- start body -->
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

    <!-- start logo -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end logo -->

    <!-- start hero -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 1200px;"">
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;"">Ordine effettuato con successo.</h1>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end hero -->

    <!-- start copy block -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 1200px;"">

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Ecco il riepilogo degli articoli ordinati. Restiamo in attesa di ricevere la foto dell'avvenuta accettazione delle condizioni da parte del cliente consumatore.</p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start receipt table -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                <tr>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""75%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Codice</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Colore</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Descrizione</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>2XS/40</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Xs/42</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>S/44</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>M/46</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>L/48</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Xl/50</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>2XL/52</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>3XL/54</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>4XL/56</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>VU</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Tot.</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>€/cad.</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>TOT. €</strong></td>
                </tr>";
                var righeOrdine = _context.RigaOrdineCliente.Where(x => x.IdOrdine == ordineCliente.Id).ToList();
                var totale = 0.0;

                foreach (var item in righeOrdine)
                {
                    var articolo = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x).FirstOrDefault();
                    html +=
                       $@"<tr><td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.Codice}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.Colore}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.Descrizione}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xxs}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xs}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.S}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.M}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.L}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xl}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xxl}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xxxl}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xxxxl}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.TagliaUnica}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{(item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.Xxxxl + item.TagliaUnica)}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.PrezzoVendita}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{ articolo.PrezzoVendita * (item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.Xxxxl + item.TagliaUnica)}</td></tr>";
                    totale += (item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.Xxxxl + item.TagliaUnica) * articolo.PrezzoVendita;
                }

                html += $@"
                <tr>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>Totale</strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>€</strong></td>
                  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>{totale}</strong></td>
                </tr>
              </table>
            </td>
          </tr>
          <!-- end reeipt table -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end copy block -->

    <!-- start receipt address block -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"" valign=""top"" width=""100%"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table align=""center"" bgcolor=""#ffffff"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 1200px;"">
          <tr>
            <td align=""left"" valign=""top"" style=""font-size: 0; border-bottom: 3px solid #d4dadf"">
              <!--[if (gte mso 9)|(IE)]>
              <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
              <tr>
              <td align=""left"" valign=""top"" width=""300"">
              <![endif]-->
              <div style=""display: inline-block; width: 100%; max-width: 50%; min-width: 240px; vertical-align: top;"">
                <table align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 300px;"">
                <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Numero ordine</strong></p>
                      <p>{ordineCliente.RandomNumber}</p>
                    </td>
                </tr>
                <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Regione/rappresentante</strong></p>
                      <p>{regione}</p>
                    </td>
                </tr>
                <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Cliente</strong></p>
                      <p>{cliente.Nome}</p>
                    </td>
                  </tr>      
                 <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Indirizzo email cliente</strong></p>
                      <p>{cliente.Email}</p>
                    </td>
                  </tr>  
                  <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Indirizzo di spedizione</strong></p>
                      <p>{cliente.Indirizzo}</p>
                    </td>
                  </tr>
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Data consegna</strong></p>
                      <p>{ordineCliente.DataConsegna.ToString("dd/MM/yyyy")}</p>
                    </td>
                  </tr>
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Metodo pagamento</strong></p>
                      <p>{pagamento}</p>
                    </td>
                  </tr>
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>{(!string.IsNullOrEmpty(ordineCliente.Note) ? "Note" : "")}</strong></p>
                      <p>{(!string.IsNullOrEmpty(ordineCliente.Note) ? ordineCliente.Note : "")}</p>
                    </td>
                  </tr>
                </table>
              </div>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end receipt address block -->

    <!-- start footer -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"" style=""padding: 24px;"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 800px;"">
          <!-- start permission -->
          <tr>
            <td align=""center"" bgcolor=""#D2C7BA"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">Ai sensi del D. Lgs. 196/03 e dal regolamento UE 2016/679 per la protezione dei dati personali, questo messaggio è destinato unicamente alla persona o al soggetto al quale è indirizzato e può contenere informazioni riservate e/o coperte da segreto professionale, la cui divulgazione è proibita. Qualora non siate i destinatari designati non dovrete leggere, utilizzare, diffondere o copiare le informazioni trasmesse. Nel caso aveste ricevuto questo messaggio per errore, vogliate cortesemente contattare il mittente e cancellare il materiale dai vostri computer.
             <br>
            According to D. Lgs. 196/03 and by the EU regulation 2016/679 for the protection of personal data, this message is intended only for the person or entity to which it is addressed and may contain confidential and/or privileged information, the disclosure of which is prohibited. If you are not the intended recipient you may not read, use, disseminate or copy the information transmitted. If you have received this message in error, please contact the sender and delete the material from any computer.
            Area degli allegati
            </p>
            </td>
          </tr>
          <!-- end permission -->
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end footer -->
  </table>
  <!-- end body -->
</body>
</html>";

                var showAllRecipients = false; // Set to true if you want the recipients to see each others email addresses

                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, html, showAllRecipients);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
               GestioneErrori(ex);
                throw;
            }
        }

    }
}
