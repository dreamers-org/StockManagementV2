"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function attivatorePaginaOrdineFornitoreCreate() {
    window["getTxtValues"] = getTxtValues;
    window["verifyCorrectness"] = verifyCorrectness;
    window["getColorePerArticolo"] = getColorePerArticolo;
}
exports.attivatorePaginaOrdineFornitoreCreate = attivatorePaginaOrdineFornitoreCreate;
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
//ottiene la lista dei colori per il codice selezionato.
function getColorePerArticolo() {
    var txtCodice = $("#txtCodice");
    if (txtCodice != null) {
        var codice = txtCodice.val().toString();
        $.ajax({
            type: "POST",
            url: "/OrdineCliente/SelectColoriFromCodice",
            data: { codice: codice },
            success: function (data) {
                console.log(data.length);
                $('#ddlColore').removeAttr("disabled");
                if (data.length == 0) {
                    var s = '<option value="-1">Seleziona un colore</option>';
                    $("#dropdownColore").html(s);
                }
                else {
                    for (var i = 0; i < data.length; i++) {
                        var s = '<option value="-1">Seleziona un colore</option>';
                        for (var i = 0; i < data.length; i++) {
                            console.log(data[i]);
                            s += '<option value="' + data[i].colore + '">' + data[i].colore + '</option>';
                        }
                        $("#dropdownColore").html(s);
                        $("#dropdownColore").removeAttr("disabled");
                    }
                }
            },
            error: function () {
                console.log("Errore");
            }
        });
    }
}
//# sourceMappingURL=ordinefornitore-create.js.map