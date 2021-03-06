"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function attivatorePaginaCreateArticolo() {
    window["getTxtValues"] = getTxtValues;
    window["verifyCorrectness"] = verifyCorrectness;
}
exports.attivatorePaginaCreateArticolo = attivatorePaginaCreateArticolo;
function getTxtValues() {
    var codice = $('#txtCodice').val();
    $.ajax({
        type: "POST",
        url: "/Articolo/getTxtValues",
        data: { codice: codice },
        success: function (data) {
            console.log(data);
            if (data.fornitore) {
                $('#txtDescrizione').val(data.descrizione);
                ;
                $('#txtPrezzoAcquisto').val(data.prezzoAcquisto);
                ;
                $('#txtPrezzoVendita').val(data.prezzoVendita);
                ;
                $('#txtTrancheConsegna').val(data.trancheConsegna);
                ;
                $('#ddlGenereProdotto').empty().append(new Option(data.genereProdotto, data.genereProdotto)).val(data.genereProdotto);
                $('#ddlTipoProdotto').empty().append(new Option(data.tipoProdotto, data.idTipoProdotto)).val(data.idTipoProdotto);
                $('#ddlFornitore').empty().append(new Option(data.fornitore, data.idFornitore)).val(data.idFornitore);
                $('#ddlCollezione').empty().append(new Option(data.collezione, data.idCollezione)).val(data.idCollezione);
                $('.form-control').attr("readonly", "readonly");
                $('#chkXXS').prop('checked', data.xxs);
                $('#chkXS').prop('checked', data.xs);
                $('#chkS').prop('checked', data.s);
                $('#chkM').prop('checked', data.m);
                $('#chkL').prop('checked', data.l);
                $('#chkXL').prop('checked', data.xl);
                $('#chkXXL').prop('checked', data.xxl);
                $('#chkXXXL').prop('checked', data.xxxl);
                $('#chkXXXXL').prop('checked', data.xxxxl);
                $('#chkTagliaUnica').prop('checked', data.tagliaUnica);
                $('#txtColore').removeAttr("readonly");
            }
        },
        error: function () {
            alert("Errore");
        }
    });
}
;
function verifyCorrectness() {
    var codice = $('#txtCodice').val();
    var colore = $('#txtColore').val();
    $.ajax({
        type: "POST",
        url: "/Articolo/verifyCorrectness",
        data: { codice: codice, colore: colore },
        success: function (data) {
            console.log(data);
            if (!(data)) {
                $('#divError').show();
                $('#btnInserisci').attr("disabled", "disabled");
            }
            else {
                $('#divError').hide();
                $('#btnInserisci').removeAttr("disabled");
            }
        },
        error: function () {
            alert("Errore");
        }
    });
}
;
//# sourceMappingURL=articolo-create.js.map