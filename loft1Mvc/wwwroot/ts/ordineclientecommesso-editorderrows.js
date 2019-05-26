"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("bootstrap");
//variabile globale usata per gestire gli articoli disponibili.
var articoliDisponibili = null;
function attivatorePaginaOrdineClienteCommessoEditOrderRows() {
    window["getColorePerArticolo"] = getColorePerArticolo;
    window["caricaArticoli"] = caricaArticoli;
    window["getTaglieDisponibiliArticolo"] = getTaglieDisponibiliArticolo;
    window["disableBtnInserisci"] = disableBtnInserisci;
    window["getFotoArticolo"] = getFotoArticolo;
    $("#btnVisualizzaImmagine").hide();
    //carico la combo degli articoli
    caricaArticoli();
    //nascondo il div d'errore.
    $("#divAlertError").hide();
}
exports.attivatorePaginaOrdineClienteCommessoEditOrderRows = attivatorePaginaOrdineClienteCommessoEditOrderRows;
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
        if (jQuery.inArray(codice.toUpperCase(), articoliDisponibili) != -1) {
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
                            $('#btnInserisci').attr("disabled", "disabled");
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
                        articoliDisponibili = data;
                        $("#txtCodiceArticolo").removeAttr("readonly");
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
function getTaglieDisponibiliArticolo() {
    var codice = $('#txtCodiceArticolo').val();
    var colore = $('#dropdownColore').val();
    if ((codice != null && codice != "") && (colore != null && colore != "-1")) {
        $.ajax({
            type: "POST",
            url: "/Articolo/getTaglieDisponibili",
            data: { codice: codice, colore: colore },
            success: function (data) {
                console.log(data);
                if (data.xxs) {
                    $('#txtXxs').val("0");
                    $('#txtXxs').attr("readonly", "readonly");
                }
                else {
                    $('#txtXxs').removeAttr("readonly");
                }
                if (data.xs) {
                    $('#txtXs').val("0");
                    $('#txtXs').attr("readonly", "readonly");
                }
                else {
                    $('#txtXs').removeAttr("readonly");
                }
                if (data.s) {
                    $('#txtS').val("0");
                    $('#txtS').attr("readonly", "readonly");
                }
                else {
                    $('#txtS').removeAttr("readonly");
                }
                if (data.m) {
                    $('#txtM').val("0");
                    $('#txtM').attr("readonly", "readonly");
                }
                else {
                    $('#txtM').removeAttr("readonly");
                }
                if (data.l) {
                    $('#txtL').val("0");
                    $('#txtL').attr("readonly", "readonly");
                }
                else {
                    $('#txtL').removeAttr("readonly");
                }
                if (data.xl) {
                    $('#txtXl').val("0");
                    $('#txtXl').attr("readonly", "readonly");
                }
                else {
                    $('#txtXl').removeAttr("readonly");
                }
                if (data.xxl) {
                    $('#txtXxl').val("0");
                    $('#txtXxl').attr("readonly", "readonly");
                }
                else {
                    $('#txtXxl').removeAttr("readonly");
                }
                if (data.xxxl) {
                    $('#txtXxxl').val("0");
                    $('#txtXxxl').attr("readonly", "readonly");
                }
                else {
                    $('#txtXxxl').removeAttr("readonly");
                }
                if (data.xxxxl) {
                    $('#txtXxxxl').val("0");
                    $('#txtXxxxl').attr("readonly", "readonly");
                }
                else {
                    $('#txtXxxxl').removeAttr("readonly");
                }
                if (data.tagliaUnica) {
                    $('#txtTagliaUnica').val("0");
                    $('#txtTagliaUnica').attr("readonly", "readonly");
                }
                else {
                    $('#txtTagliaUnica').removeAttr("readonly");
                }
                if (data.isArticoloValido) {
                    $('#btnInserisci').removeAttr("disabled");
                }
                else {
                    $('#btnInserisci').attr("disabled", "disabled");
                }
            },
            error: function () {
                alert("Errore");
            }
        });
        //mostro il pulsante per visualizzare l'immagine.
        $("#btnVisualizzaImmagine").show();
    }
    else {
        //nascondo il pulsante per visualizzare l'immagine.
        $("#btnVisualizzaImmagine").hide();
    }
}
;
function getFotoArticolo() {
    var codice = $('#txtCodiceArticolo').val();
    var colore = $('#dropdownColore').val();
    if ((codice != null && codice != "") && (colore != null && colore != "-1")) {
        $.ajax({
            type: "POST",
            url: "/Articolo/getFotoArticolo",
            data: { codice: codice, colore: colore },
            success: function (data) {
                console.log(data);
                if (data.isArticoloValido) {
                    $('#btnInserisci').removeAttr("disabled");
                }
                else {
                    $('#btnInserisci').attr("disabled", "disabled");
                }
                if (data.foto) {
                    $("#modalFoto").modal("show");
                    $('#divFoto').attr("src", data.foto);
                }
            },
            error: function () {
                alert("Errore");
            }
        });
    }
}
;
function disableBtnInserisci() {
    $('#btnInserisci').attr("disabled", "disabled");
}
//# sourceMappingURL=ordineclientecommesso-editorderrows.js.map