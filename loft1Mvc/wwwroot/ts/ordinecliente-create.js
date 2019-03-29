"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
//variabile globale usata per gestire gli articoli disponibili.
var articoliDisponibili = null;
function attivatorePaginaCreate() {
    window["getColorePerArticolo"] = getColorePerArticolo;
    window["caricaArticoli"] = caricaArticoli;
    //carico la combo degli articoli
    caricaArticoli();
    //nascondo il div d'errore.
    $("#divAlertError").hide();
}
exports.attivatorePaginaCreate = attivatorePaginaCreate;
//ottiene la lista dei colori per il codice selezionato.
function getColorePerArticolo() {
    var txtCodice = $("#txtCodiceArticolo");
    $("#dropdownColore").removeAttr("readonly");
    $("#dropdownColore").empty();
    $('#txtDescrizione').empty();
    //nascondo il div d'errore.
    $("#divAlertError").hide();
    if (txtCodice != null && txtCodice.val().toString() != "") {
        var codice = txtCodice.val().toString();
        if (jQuery.inArray(codice, articoliDisponibili) != -1) {
            //carico la lista dei colori.
            $.ajax({
                type: "POST",
                url: "/OrdineCliente/SelectColoriFromCodice",
                data: { codice: codice },
                success: function (data) {
                    console.log(data.length);
                    $('#dropdownColore').removeAttr("disabled");
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
            //carico la descrizione dell'articolo.
            $.ajax({
                type: "POST",
                url: "/OrdineCliente/SelectDescrizioneFromCodice",
                data: { codice: codice },
                success: function (data) {
                    console.log(data);
                    $('#txtDescrizione').attr("value", data);
                }
            });
        }
        else {
            $("#dropdownColore").attr("readonly", "readonly");
            $("#divAlertError").show();
            $("#divAlertError").text("L'articolo non esiste o non è disponibile per la data di consegna selezionata.");
        }
    }
}
//carica gli articoli in base alla data di consegna selezionata.
function caricaArticoli() {
    try {
        var txtDataConsegna = $("#txtDataConsegna");
        if (txtDataConsegna != null) {
            var valoreDataConsegna = txtDataConsegna.val().toString();
            if (valoreDataConsegna != null && valoreDataConsegna != "") {
                var dataConsegna = valoreDataConsegna;
                $.ajax({
                    type: "POST",
                    url: "/OrdineCliente/SelectCodiciArticoli",
                    data: { dataConsegna: dataConsegna },
                    success: function (data) {
                        console.log("caricamento articoli");
                        //let txtCodiceArticolo:  JQuery<HTMLElement> =/*;*/
                        articoliDisponibili = data;
                        $("#txtCodiceArticolo").removeAttr("readonly");
                        $("#txtCodiceArticolo").autocomplete({ source: articoliDisponibili });
                    },
                    error: function () {
                        console.log("Errore caricaArticoli");
                    }
                });
            }
        }
    }
    catch (e) {
        console.log("Errore caricaArticoli");
    }
}
//# sourceMappingURL=ordinecliente-create.js.map